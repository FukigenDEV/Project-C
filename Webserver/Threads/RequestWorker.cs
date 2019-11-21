using Configurator;
using Dapper.Contrib.Extensions;
using Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Reflection;
using Webserver.Data;

namespace Webserver.Threads {
	class RequestWorker {
		private readonly Logger Log;
		private readonly BlockingCollection<HttpListenerContext> Queue;
		private readonly SQLiteConnection Connection;

		public RequestWorker(Logger Log, BlockingCollection<HttpListenerContext> Queue) {
			this.Log = Log;
			this.Queue = Queue;
			this.Connection = Database.createConnection();
		}

		public void Run() {
			while (true) {
				HttpListenerContext Context = Queue.Take();
				HttpListenerRequest Request = Context.Request;
				HttpListenerResponse Response = Context.Response;

				//Resolve redirects, if any
				string URL = Redirect.Resolve(Request.RawUrl.ToLower());
				if (URL == null) {
					Log.Error("Couldn't resolve URL; infinite redirection loop. URL: " + Request.Url.LocalPath.ToLower());
					Utils.Send(Response, Utils.GetErrorPage(HttpStatusCode.LoopDetected, "An infinite loop was detected while trying to access the specified URL."), HttpStatusCode.LoopDetected);
					continue;
				} else if (URL != Request.RawUrl.ToLower()) {
					Response.Redirect(URL);
					Utils.Send(Response, null, HttpStatusCode.Redirect);
					continue;
				}

				//Find this request's target
				Type T = FindEndpoint(Request);
				if(T != null) {
					ProcessEndpoint(T, Context);
					continue;
				}

				//No endpoint was found, so see if a resource exists at this address instead.
				//Add wwwroot to URL
				string Target = Config.GetValue("WebserverSettings.wwwroot") + Request.Url.LocalPath.ToLower();

				if (WebFiles.WebPages.Contains(Target) && File.Exists(Target)) {
					ProcessResource(Target, Context);
				} else {
					//No endpoint or resource was found, so send a 404.
					Log.Warning("Refused request for " + Request.Url.LocalPath.ToLower() + ": Not Found");
					Utils.Send(Response, Utils.GetErrorPage(HttpStatusCode.NotFound), HttpStatusCode.NotFound);
				}
			}
		}

		public Type FindEndpoint(HttpListenerRequest Request) {
			//Search through endpoints
			foreach (Type T in Program.Endpoints) {
				//Get endpoint info attribute. If the attribute is missing, show an error in console and continue to the next.
				EndpointURL Info = (EndpointURL)Attribute.GetCustomAttribute(T, typeof(EndpointURL));
				if (Info == null) {
					Log.Error("Endpoint " + T.Name + " has no EndpointURL attribute");
					continue;
				}

				//Check if this is the correct endpoint. If it is, execute it.
				if (Info.URL == Request.Url.LocalPath.ToLower()) {
					return T;
				}
			}
			return null;
		}

		[SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
		public void ProcessEndpoint(Type T, HttpListenerContext Context) {
			HttpListenerRequest Request = Context.Request;
			HttpListenerResponse Response = Context.Response;

			APIEndpoint Endpoint = (APIEndpoint)Activator.CreateInstance(T);
			Endpoint.Connection = Connection;
			Endpoint.Context = Context;
			Endpoint.Response = Context.Response;
			Endpoint.Request = Context.Request;

			//Convert query string to a Dict because NameValueCollections are trash
			foreach (string key in Request.QueryString) {
				Endpoint.RequestParams.Add(key?.ToLower() ?? "null", new List<string>(Request.QueryString[key]?.Split(',')));
			}

			// Get the method
			MethodInfo Method = Endpoint.GetType().GetMethod(Request.HttpMethod);

			//Check this method's required content type, if any.
			RequireContentType CT = Method.GetCustomAttribute<RequireContentType>();
			if(CT != null) {
				if(CT.ContentType == "application/json") {
					//Try to parse the JSON. If that failed for some reason, check if the response body is actually necessary.
					//Return null if it isn't, return a 400 Bad Request if it is.
					try {
						using StreamReader streamReader = new StreamReader(Request.InputStream, Request.ContentEncoding);
						Endpoint.JSON = JObject.Parse(streamReader.ReadToEnd());
					} catch (JsonReaderException e) {
						if (Method.GetCustomAttribute<RequireBody>() != null) {
							Log.Warning("Refused request for endpoint " + T.Name + ": Could not parse JSON");
							Utils.Send(Response, "Invalid JSON: " + e.Message, HttpStatusCode.BadRequest);
							return;
						}
						Endpoint.JSON = null;
					}
				}
			}

			#region PermissionLevel Attribute
			PermissionLevel Attr = Method.GetCustomAttribute<PermissionLevel>();
			if (Attr != null) {
				//Check cookie. If its missing, send a 401 Unauthorized if the cookie is missing. (because a user MUST be logged in to use an endpoint with a PermissionLevel attribute)
				Cookie SessionIDCookie = Request.Cookies["SessionID"];
				if (SessionIDCookie == null) {
					Utils.Send(Response, "No Session", HttpStatusCode.Unauthorized);
					return;
				}

				//Check if a session exists with the Session ID contained in the session cookie. If none is found or the session has expired, send back a 401 Unauthorized
				Session s = Session.GetUserSession(Connection, SessionIDCookie.Value);
				if (s == null) {
					Utils.Send(Response, "Expired session", HttpStatusCode.Unauthorized);
					return;
				}
				//Renew the session, and save it in the endpoint object.
				s.Renew(Connection);
				Endpoint.UserSession = s;

				//Save user in endpoint object
				Endpoint.RequestUser = Connection.Get<User>(s.User);

				//Get department name. If none was found, assume Administrators
				string DepartmentName;
				if (Endpoint.RequestParams.ContainsKey("Department")) {
					DepartmentName = Endpoint.RequestParams["Department"][0];
				} else {
					DepartmentName = "Administrators";
				}

				//Get department. If none was found, send 400 Bad Request
				Department Dept = Department.GetByName(Connection, DepartmentName);
				if (Dept == null) {
					Utils.Send(Response, "No such Department", HttpStatusCode.BadRequest);
					return;
				}

				//Get permissionlevel
				PermLevel Level = Endpoint.RequestUser.GetPermissionLevel(Connection, Dept.ID);
				Endpoint.RequestUserLevel = Level;

				//Check permission level
				if (Level < Attr.Level) {
					Log.Warning("User " + Endpoint.RequestUser.Email + " attempted to access endpoint " + T.Name + " without sufficient permissions");
					Log.Warning("Department: '" + DepartmentName + "', User is '" + Level + "' but must be at least '" + Attr.Level + "'");
					Utils.Send(Response, null, HttpStatusCode.Forbidden);
					return;
				}
			}
			#endregion

			try {
				Method.Invoke(Endpoint, null);
			} catch (Exception e) {
				Log.Error("Failed to fullfill request for endpoint " + T.Name + ": " + e.GetType().Name + ", " + e.Message);
				Utils.Send(Response, Utils.GetErrorPage(HttpStatusCode.InternalServerError, e.Message), HttpStatusCode.InternalServerError);
			}
		}

		public void ProcessResource(string Target, HttpListenerContext Context) {
			HttpListenerRequest Request = Context.Request;
			HttpListenerResponse Response = Context.Response;

			//Content-type header shouldn't be set for resources
			if (Request.ContentType != null) {
				Log.Warning("Refused request for resource " + Target + ": Unsupported Media Type (" + Request.ContentType + ")");
				Utils.Send(Response, Utils.GetErrorPage(HttpStatusCode.UnsupportedMediaType), HttpStatusCode.UnsupportedMediaType);
				return;
			}

			//Switch to the request's HTTP method
			switch (Request.HttpMethod) {
				case "GET":
					Utils.Send(Response, File.ReadAllBytes(Target), HttpStatusCode.OK);
					return;

				case "HEAD":
					Utils.Send(Response, null, HttpStatusCode.OK);
					return;

				case "OPTIONS":
					Response.Headers.Add("Allow", "GET, HEAD, OPTIONS");
					if (Program.CORSAddresses.Contains("http://" + Request.LocalEndPoint.ToString())) {
						Response.Headers.Add("Access-Control-Allow-Origin", Request.LocalEndPoint.ToString());
					}
					Utils.Send(Response, null, HttpStatusCode.OK);
					return;

				default:
					Log.Warning("Refused request for resource " + Target + ": Method Not Allowed (" + Request.HttpMethod + ")");
					Utils.Send(Response, Utils.GetErrorPage(HttpStatusCode.MethodNotAllowed), HttpStatusCode.MethodNotAllowed);
					return;
			}
		}
	}
}