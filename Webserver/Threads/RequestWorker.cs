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
					Log.Error("Couldn't resolve URL; infinite redirection loop. URL: "+ Request.Url.LocalPath.ToLower());
					Utils.Send(Response, Utils.GetErrorPage(HttpStatusCode.LoopDetected, "An infinite loop was detected while trying to access the specified URL."), HttpStatusCode.LoopDetected);
					continue;
				} else if (URL != Request.RawUrl.ToLower()) {
					Response.Redirect(URL);
					Utils.Send(Response, null, HttpStatusCode.Redirect);
					continue;
				}

				//Append wwwroot to target
				string Target = Config.GetValue("WebserverSettings.wwwroot") + Request.Url.LocalPath.ToLower();

				//Switch to contentType
				switch (Request.ContentType) {

					//JSON
					case "application/json":

						//Search for the right endpoint.
						APIEndpoint ep = null;
						foreach (Type T in Program.Endpoints) {
							//Get endpoint info attribute. If the attribute is missing, show an error in console and continue to the next.
							EndpointInfo Info = (EndpointInfo)Attribute.GetCustomAttribute(T, typeof(EndpointInfo));
							if (Info == null) {
								Log.Error("Endpoint " + T.Name + " has no EndpointInfo attribute");
								continue;
							}

							//Check if this is the correct endpoint. If it is, continue.
							if (Info.ContentType == Request.ContentType && Config.GetValue("WebserverSettings.wwwroot") + Info.URL == Target) {
								ep = (APIEndpoint)Activator.CreateInstance(T);
							}
						}

						//If no endpoint was found, send a 404
						if (ep == null) {
							Log.Info("Received " + Request.HttpMethod + " request for invalid endpoint at address " + Target + " from " + Request.UserHostName);
							Utils.Send(Response, "No such endpoint", HttpStatusCode.NotFound);
							continue;
						}

						//Set endpoint properties
						ep.Connection = Connection;
						ep.Context = Context;
						ep.Response = Context.Response;
						ep.Request = Context.Request;

						//Convert query string to a Dict because NameValueCollections are trash
						foreach (string key in Request.QueryString) {
							ep.RequestParams.Add(key?.ToLower() ?? "null", new List<string>(Request.QueryString[key]?.Split(',')));
						}

						//Get the method
						MethodInfo Method = ep.GetType().GetMethod(Request.HttpMethod);

						//Try to parse the JSON. If that failed for some reason, check if the response body is actually necessary.
						//Return null if it isn't, return a 400 Bad Request if it is.
						try {
							using StreamReader streamReader = new StreamReader(Request.InputStream, Request.ContentEncoding);
							ep.Content = JObject.Parse(streamReader.ReadToEnd());
						} catch (JsonReaderException e) {
							if (Method.GetCustomAttribute<RequireBody>() != null) {
								Utils.Send(Response, "Invalid JSON: "+e.Message, HttpStatusCode.BadRequest);
								continue;
							}
							ep.Content = null;
						}

						#region PermissionLevel Attribute
						PermissionLevel Attr = Method.GetCustomAttribute<PermissionLevel>();
						if (Attr != null) {
							//Check cookie. If its missing, send a 401 Unauthorized if the cookie is missing. (because a user MUST be logged in to use an endpoint with a PermissionLevel attribute)
							Cookie SessionIDCookie = Request.Cookies["SessionID"];
							if (SessionIDCookie == null) {
								Utils.Send(Response, "No Session", HttpStatusCode.Unauthorized);
								continue;
							}

							//Check if a session exists with the Session ID contained in the session cookie. If none is found or the session has expired, send back a 401 Unauthorized
							Session s = Session.GetUserSession(Connection, SessionIDCookie.Value);
							if (s == null) {
								Utils.Send(Response, "Expired session", HttpStatusCode.Unauthorized);
								continue;
							}
							//Renew the session, and save it in the endpoint object.
							s.Renew(Connection);
							ep.UserSession = s;

							//Save user in endpoint object
							ep.RequestUser = Connection.Get<User>(s.User);

							//Get department name. If none was found, assume Administrators
							string DepartmentName;
							if (ep.RequestParams.ContainsKey("Department")) {
								DepartmentName = ep.RequestParams["Department"][0];
							} else {
								DepartmentName = "Administrators";
							}

							//Get department. If none was found, send 400 Bad Request
							Department Dept = Department.GetDepartmentByName(Connection, DepartmentName);
							if (Dept == null) {
								Utils.Send(Response, "No such Department", HttpStatusCode.BadRequest);
								continue;
							}

							//Get permissionlevel
							PermLevel Level = ep.RequestUser.GetPermissionLevel(Connection, Dept.ID);
							ep.RequestUserLevel = Level;

							//Check permission level
							if (Level < Attr.Level) {
								Log.Warning("User " + ep.RequestUser.Email + " attempted to access API endpoint " + Target + " without sufficient permissions");
								Log.Warning("Department: '" + DepartmentName + "', User is '" + Level + "' but must be at least '" + Attr.Level + "'");
								Utils.Send(Response, null, HttpStatusCode.Forbidden);
								continue;
							}
						}
						#endregion

						try {
							Method.Invoke(ep, null);
#pragma warning disable CA1031 // Do not catch general exception types
						} catch (Exception e) {
							Utils.Send(Response, Utils.GetErrorPage(HttpStatusCode.InternalServerError, e.Message), HttpStatusCode.InternalServerError);
						}
#pragma warning restore CA1031 // Do not catch general exception types
						break;

					//HTML, CSS, JS, images
					default:
						//Browsers apparently don't set content type when asking for webpages.
						//If the file exists, load and send it. Otherwise, send a 404.
						if (WebFiles.WebPages.Contains(Target) && File.Exists(Target)) {
							Utils.Send(Response, File.ReadAllBytes(Target), HttpStatusCode.OK);
						} else {
							Log.Info("Received " + Request.HttpMethod + " request for invalid webpage at address " + Request.RawUrl + " from " + Request.UserHostName);
							Utils.Send(Response, Utils.GetErrorPage(HttpStatusCode.NotFound), HttpStatusCode.NotFound);
						}

						break;
				}
			}
		}
	}
}
