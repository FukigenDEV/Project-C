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
	/// <summary>
	/// Request handlers. Meant to run in a separate thread.
	/// </summary>
	class RequestWorker {
		private readonly Logger Log;
		private readonly BlockingCollection<HttpListenerContext> Queue;
		private readonly SQLiteConnection Connection;

		/// <summary>
		/// Create a new RequestWorker, which processes incoming HttpListener requests. Meant to run in a separate thread.
		/// </summary>
		/// <param name="Log">A Logger object</param>
		/// <param name="Queue">A BlockingCollection queue that will contain all incoming requests.</param>
		public RequestWorker(Logger Log, BlockingCollection<HttpListenerContext> Queue) {
			this.Log = Log;
			this.Queue = Queue;
			this.Connection = Database.CreateConnection();
		}

		/// <summary>
		/// Start this RequestWorker. Meant to run in a separate thread.
		/// </summary>
		public void Run() {
			while (true) {
				HttpListenerContext Context = Queue.Take();
				DateTime Started = DateTime.Now;
				HttpListenerRequest Request = Context.Request;
				HttpListenerResponse Response = Context.Response;

				//Resolve redirects, if any
				string URL = Redirect.Resolve(Request.RawUrl.ToLower());
				if ( URL.EndsWith('/') ) URL = URL.Remove(URL.Length-1);

				if (URL == null) {
					Log.Error("Couldn't resolve URL; infinite redirection loop. URL: " + Request.RawUrl.ToLower());
					Utils.Send(Response, Utils.GetErrorPage(HttpStatusCode.LoopDetected, "An infinite loop was detected while trying to access the specified URL."), HttpStatusCode.LoopDetected);
					continue;
				} else if (URL != Request.RawUrl.ToLower()) {
					Response.Redirect(URL);
					Utils.Send(Response, null, HttpStatusCode.Redirect);
					continue;
				}

				//Find this request's target
				Type T = FindEndpoint(Request);
				if ( T != null ) {
					ProcessEndpoint(T, Context);
				} else {
					//No endpoint was found, so see if a resource exists at this address instead.
					//Add wwwroot to URL
					string Target = Config.GetValue("WebserverSettings.wwwroot") + Request.Url.LocalPath.ToLower();

					if ( WebFiles.WebPages.Contains(Target) && File.Exists(Target) ) {
						ProcessResource(Target, Context);
					} else {
						//No endpoint or resource was found, so send a 404.
						Log.Warning("Refused request for " + Request.Url.LocalPath.ToLower() + ": Not Found");
						Utils.Send(Response, Utils.GetErrorPage(HttpStatusCode.NotFound), HttpStatusCode.NotFound);
					}
				}
				double TimeSpent = (int)( DateTime.Now - Started ).TotalMilliseconds;
				Log.Debug("Operation complete. Took " + TimeSpent + "ms");
				if(TimeSpent >= 250 ) {
					Log.Warning("An operation took too long to complete. Took " + TimeSpent + " ms, should be less than 250ms");
				}
			}
		}

		/// <summary>
		/// Find the API endpoint that this request is directed to. Returns null if none is found.
		/// </summary>
		/// <param name="Request"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Calls the given API endpoint.
		/// </summary>
		/// <param name="T"></param>
		/// <param name="Context"></param>
		[SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
		public void ProcessEndpoint(Type T, HttpListenerContext Context) {
			HttpListenerRequest Request = Context.Request;
			HttpListenerResponse Response = Context.Response;

			//Create an instance of the specified endpoint and set the required values.
			APIEndpoint Endpoint = (APIEndpoint)Activator.CreateInstance(T);
			Endpoint.Connection = Connection;
			Endpoint.Context = Context;
			Endpoint.Response = Context.Response;
			Endpoint.Request = Context.Request;

			//Convert query string to a Dict because NameValueCollections are trash
			foreach (string key in Request.QueryString) {
				Endpoint.RequestParams.Add(key?.ToLower() ?? "null", new List<string>(Request.QueryString[key]?.Split(',')));
			}

			//Set access control headers for CORS support.
			List<string> AllowedMethods = new List<string>();
			foreach ( MethodInfo M in T.GetMethods() ) {
				if ( M.DeclaringType == GetType() ) {
					AllowedMethods.Add(M.Name);
				}
			}
			string Origin = Request.Headers.Get("Origin");
			if ( Program.CORSAddresses.Contains(Origin + '/') ) {
				Response.Headers.Add("Access-Control-Allow-Origin", Origin);
			}
			Response.Headers.Add("Allow", string.Join(", ", AllowedMethods));
			Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");

			// Get the endpoint method
			MethodInfo Method = Endpoint.GetType().GetMethod(Request.HttpMethod);

			//Check this method's required content type, if any.
			RequireContentType CT = Method.GetCustomAttribute<RequireContentType>();
			if(CT != null) {
				if(CT.ContentType == "application/json") {
					//Try to parse the JSON. If that failed for some reason, check if the response body is actually necessary.
					//Return null if it isn't, return a 400 Bad Request if it is.
					using StreamReader Reader = new StreamReader(Request.InputStream, Request.ContentEncoding);
					string JSONText = Reader.ReadToEnd();
					try {
						Endpoint.JSON = JObject.Parse(JSONText);
					} catch (JsonReaderException e) {
						if (Method.GetCustomAttribute<RequireBody>() != null) {
							Log.Warning("Refused request for endpoint " + T.Name + ": Could not parse JSON");
							Log.Debug("Message: " + e.Message);
							Log.Debug("Received JSON: "+JSONText);
							Utils.Send(Response, "Invalid JSON: " + e.Message, HttpStatusCode.BadRequest);
							return;
						}
						Endpoint.JSON = null;
					}
				}
			}

			//Check whether the user is allowed to use this endpoint method, if necessary.
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

			//Attempt to invoke the API endpoint method. If this fails, send a 500 Internal Server Error to the client.
			try {
				Method.Invoke(Endpoint, null);
			} catch (Exception e) {
				Log.Error("Failed to fullfill request for endpoint " + T.Name + ": " + e.GetType().Name + ", " + e.Message);
				Utils.Send(Response, Utils.GetErrorPage(HttpStatusCode.InternalServerError, e.Message), HttpStatusCode.InternalServerError);
			}
		}

		/// <summary>
		/// Process a request for a resource.
		/// </summary>
		/// <param name="Target">The URL the resource is located at.</param>
		/// <param name="Context"></param>
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
					//Send the resource to the client. Content type will be set according to the resource's file extension.
					Utils.Send(Response, File.ReadAllBytes(Target), HttpStatusCode.OK, Path.GetExtension(Target).ToString() switch {
						".css" => "text/css",
						".png" => "image/png",
						".js" => "text/javascript",
						".jpg" => "image/jpeg",
						".jpeg" => "image/jpeg",
						_ => "text/html"
					});
					return;

				case "HEAD":
					//A HEAD request is the same as GET, except without the body. Since the resource exists, we can just send back a 200 OK and call it a day.
					Utils.Send(Response, null, HttpStatusCode.OK);
					return;

				case "OPTIONS":
					//Return a list of allowed HTTP methods for this resource (which is always the same), along with access-control headers for CORS support.
					Response.Headers.Add("Allow", "GET, HEAD, OPTIONS");
					if (Program.CORSAddresses.Contains("http://" + Request.LocalEndPoint.ToString())) {
						Response.Headers.Add("Access-Control-Allow-Origin", Request.LocalEndPoint.ToString());
					}
					Utils.Send(Response, null, HttpStatusCode.OK);
					return;

				default:
					//Resources only support the three methods defined above, so send back a 405 Method Not Allowed.
					Log.Warning("Refused request for resource " + Target + ": Method Not Allowed (" + Request.HttpMethod + ")");
					Utils.Send(Response, Utils.GetErrorPage(HttpStatusCode.MethodNotAllowed), HttpStatusCode.MethodNotAllowed);
					return;
			}
		}
	}
}