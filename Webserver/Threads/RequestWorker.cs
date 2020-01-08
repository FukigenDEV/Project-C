using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SQLite;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using Configurator;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PrettyConsole;
using Webserver.Data;

namespace Webserver.Threads {
	/// <summary>
	/// Request handlers. Meant to run in a separate thread.
	/// </summary>
	public class RequestWorker {
		public static LogTab RequestLoggerTab;
		private readonly Logger Log;
		public static BlockingCollection<ContextProvider> Queue { get; set; } = new BlockingCollection<ContextProvider>();
		public SQLiteConnection Connection;
		private readonly bool Debug;
		public static NumWatcher RequestTimeWatcher;

		/// <summary>
		/// Create a new RequestWorker, which processes incoming HttpListener requests. Meant to run in a separate thread.
		/// </summary>
		/// <param name="Log">A Logger object</param>
		/// <param name="Queue">A BlockingCollection queue that will contain all incoming requests.</param>
		public RequestWorker(SQLiteConnection Connection, bool Debug = false) {
			this.Log = RequestLoggerTab.GetLogger();
			this.Connection = Connection;
			this.Debug = Debug;
		}

		/// <summary>
		/// Start this RequestWorker. Meant to run in a separate thread.
		/// </summary>
		public void Run() {
			Log.Info(Thread.CurrentThread.Name + " now running.");

			do {
				ContextProvider Context = Queue.Take();
				Stopwatch S = new Stopwatch();
				S.Start();
				RequestProvider Request = Context.Request;
				ResponseProvider Response = Context.Response;

				Log.Info("Processing "+Request.HttpMethod+" request for " + Request.Url.LocalPath);

				#region Redirect
				//Resolve redirects, if any
				string URL = Redirect.Resolve(Request.Url.LocalPath.ToLower());
				//If null is returned, an infinite loop was detected.
				if (URL == null) {
					Log.Error("Couldn't resolve URL; infinite redirection loop. URL: " + Request.Url.LocalPath.ToLower());
					Response.Send(Utils.GetErrorPage(HttpStatusCode.LoopDetected, "An infinite loop was detected while trying to access the specified URL."), HttpStatusCode.LoopDetected);
					continue;
				}
				//Remove trailing /
				if (URL.EndsWith('/') && URL.Length > 1) URL = URL.Remove(URL.Length - 1);

				//Redirect if necessary
				if ( URL != Request.Url.LocalPath.ToLower() ) {
					Log.Info("Request redirected to " + URL);
					Response.Redirect(URL);
					Response.Send(HttpStatusCode.PermanentRedirect);
					continue;
				}
				#endregion

				//Find this request's target
				Type T = FindEndpoint(Request);
				if ( T != null ) {
					ProcessEndpoint(T, Context);
				} else {
					//No endpoint was found, so see if a resource exists at this address instead.
					ProcessResource(Request.Url.LocalPath.ToLower(), Context);
				}
				long TimeSpent = S.ElapsedMilliseconds;
				Log.Info("Operation complete. Took " + TimeSpent + "ms");
				if ( TimeSpent >= 250 ) {
					Log.Warning("An operation took too long to complete. Took " + TimeSpent + " ms, should be less than 250ms");
				}
				RequestTimeWatcher.Update(TimeSpent);
				Listener.QueueSizeWatcher.Update(RequestWorker.Queue.Count);
			} while ( !Debug || Queue.Count != 0);
			//Connection.Close();
		}

		/// <summary>
		/// Find the API endpoint that this request is directed to. Returns null if none is found.
		/// </summary>
		/// <param name="Request"></param>
		/// <returns></returns>
		public Type FindEndpoint(RequestProvider Request) {
			//Search through endpoints
			foreach ( Type T in Program.Endpoints ) {
				//Get endpoint info attribute. If the attribute is missing, show an error in console and continue to the next.
				EndpointURL Info = (EndpointURL)Attribute.GetCustomAttribute(T, typeof(EndpointURL));
				if ( Info == null ) {
					Log.Error("Endpoint " + T.Name + " has no EndpointURL attribute");
					continue;
				}

				//Check if this is the correct endpoint. If it is, execute it.
				if ( Info.URL == Request.Url.LocalPath.ToLower() ) {
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
		public void ProcessEndpoint(Type T, ContextProvider Context) {
			RequestProvider Request = Context.Request;
			ResponseProvider Response = Context.Response;

			//Create an instance of the specified endpoint and set the required values.
			//We're not using a constructor because it would require each individual endpoint to have its own constructor that just calls the base APIEndpoint constructor, and that's a pain.
			APIEndpoint Endpoint = (APIEndpoint)Activator.CreateInstance(T);
			Endpoint.Connection = Connection;
			Endpoint.Context = Context;
			Endpoint.Response = Context.Response;
			Endpoint.Request = Context.Request;
			Endpoint.Params = Context.Request.Params;

			//Set access control headers for CORS support.
			List<string> AllowedMethods = new List<string>();
			foreach ( MethodInfo M in T.GetMethods() ) {
				if ( M.DeclaringType == GetType() ) {
					AllowedMethods.Add(M.Name);
				}
			}

			bool OriginHeader = Request.Headers.TryGetValue("origin", out List<string> Origins);
			if ( OriginHeader && Program.CORSAddresses.Contains(Origins[0] + '/') ) {
				Response.Headers.Add("Access-Control-Allow-Origin", Origins[0]);
			}
			Response.Headers.Add("Allow", string.Join(", ", AllowedMethods));
			Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");

			// Get the endpoint method
			MethodInfo Method = Endpoint.GetType().GetMethod(Request.HttpMethod.ToString());

			//Check this method's required content type, if any.
			RequireContentType CT = Method.GetCustomAttribute<RequireContentType>();
			if ( CT != null ) {
				if ( CT.ContentType == "application/json" ) {
					//Try to parse the JSON. If that failed for some reason, check if the response body is actually necessary.
					//Return null if it isn't, return a 400 Bad Request if it is.
					using StreamReader Reader = new StreamReader(Request.InputStream, Request.ContentEncoding);
					string JSONText = Reader.ReadToEnd();
					try {
						Endpoint.JSON = JObject.Parse(JSONText);
					} catch ( JsonReaderException e ) {
						if ( Method.GetCustomAttribute<RequireBody>() != null ) {
							Log.Warning("Refused request for endpoint " + T.Name + ": Could not parse JSON");
							Log.Debug("Message: " + e.Message);
							Log.Debug("Received JSON: " + JSONText);
							Response.Send("Invalid JSON: " + e.Message, HttpStatusCode.BadRequest);
							return;
						}
						Endpoint.JSON = null;
					}
				}
			}

			//Check whether the user is allowed to use this endpoint method, if necessary.
			#region PermissionLevel Attribute
			PermissionLevel Attr = Method.GetCustomAttribute<PermissionLevel>();
			if ( Attr != null ) {
				//Check cookie. If its missing, send a 401 Unauthorized if the cookie is missing. (because a user MUST be logged in to use an endpoint with a PermissionLevel attribute)
				Cookie SessionIDCookie = Request.Cookies["SessionID"];
				if ( SessionIDCookie == null ) {
					Response.Send("No Session", HttpStatusCode.Unauthorized);
					return;
				}

				//Check if a session exists with the Session ID contained in the session cookie. If none is found or the session has expired, send back a 401 Unauthorized
				Session s = Session.GetUserSession(Connection, SessionIDCookie.Value);
				if ( s == null ) {
					Response.Send("Expired session", HttpStatusCode.Unauthorized);
					return;
				}
				//Renew the session, and save it in the endpoint object.
				s.Renew(Connection);
				Endpoint.UserSession = s;

				//Save user in endpoint object
				Endpoint.RequestUser = Connection.Get<User>(s.User);

				//Get department name. If none was found, assume Administrators
				string DepartmentName;
				if ( Endpoint.Params.ContainsKey("Department") ) {
					DepartmentName = Endpoint.Params["Department"][0];
				} else {
					DepartmentName = "Administrators";
				}

				//Get department. If none was found, send 400 Bad Request
				Department Dept = Department.GetByName(Connection, DepartmentName);
				if ( Dept == null ) {
					Response.Send("No such Department", HttpStatusCode.BadRequest);
					return;
				}

				//Get permissionlevel
				PermLevel Level = Endpoint.RequestUser.GetPermissionLevel(Connection, Dept.ID);
				Endpoint.RequestUserLevel = Level;

				//Check permission level
				if ( Level < Attr.Level ) {
					Log.Warning("User " + Endpoint.RequestUser.Email + " attempted to access endpoint " + T.Name + " without sufficient permissions");
					Log.Warning("Department: '" + DepartmentName + "', User is '" + Level + "' but must be at least '" + Attr.Level + "'");
					Response.Send(HttpStatusCode.Forbidden);
					return;
				}
			}
			#endregion

			//Attempt to invoke the API endpoint method. If this fails, send a 500 Internal Server Error to the client.
			try {
				Method.Invoke(Endpoint, null);
			} catch ( Exception e ) {
				Log.Error("Failed to fullfill request for endpoint " + T.Name + ": " + e.GetType().Name + ", " + e.Message);
				Response.Send(Utils.GetErrorPage(HttpStatusCode.InternalServerError, e.Message), HttpStatusCode.InternalServerError);
			}
		}

		/// <summary>
		/// Process a request for a resource.
		/// </summary>
		/// <param name="Target">The URL the resource is located at.</param>
		/// <param name="Context"></param>
		public void ProcessResource(string Target, ContextProvider Context) {
			RequestProvider Request = Context.Request;
			ResponseProvider Response = Context.Response;
			string wwwroot = Config.GetValue("WebserverSettings.wwwroot");
			Target = wwwroot + Target;

			//If target is '/', send index.html if it exists
			if(Target == wwwroot+"/" && File.Exists(wwwroot + "/index.html")) Target += "index.html";

			//If the file doesn't exist, send a 404
			if(!WebFiles.WebPages.Contains(Target) || !File.Exists(Target)){
				Log.Warning("Refused request for " + Request.Url.LocalPath.ToLower() + ": Not Found");
				Response.Send(Utils.GetErrorPage(HttpStatusCode.NotFound), HttpStatusCode.NotFound, "text/html");
				return;
			}

			//Content-type header shouldn't be set for resources
			if ( Request.ContentType != null ) {
				Log.Warning("Refused request for resource " + Target + ": Unsupported Media Type (" + Request.ContentType + ")");
				Response.Send(Utils.GetErrorPage(HttpStatusCode.UnsupportedMediaType), HttpStatusCode.UnsupportedMediaType, "text/html");
				return;
			}

			//Switch to the request's HTTP method
			switch ( Request.HttpMethod ) {
				case HttpMethod.GET:
					//Send the resource to the client. Content type will be set according to the resource's file extension.
					Response.Send(File.ReadAllBytes(Target), HttpStatusCode.OK, Path.GetExtension(Target).ToString() switch
					{
						".css" => "text/css",
						".png" => "image/png",
						".js" => "text/javascript",
						".jpg" => "image/jpeg",
						".jpeg" => "image/jpeg",
						_ => "text/html"
					});
					return;

				case HttpMethod.HEAD:
					//A HEAD request is the same as GET, except without the body. Since the resource exists, we can just send back a 200 OK and call it a day.
					Response.Send(HttpStatusCode.OK);
					return;

				case HttpMethod.OPTIONS:
					//Return a list of allowed HTTP methods for this resource (which is always the same), along with access-control headers for CORS support.
					Response.Headers.Add("Allow", "GET, HEAD, OPTIONS");
					if ( Program.CORSAddresses.Contains("http://" + Request.LocalEndPoint.ToString()) ) {
						Response.Headers.Add("Access-Control-Allow-Origin", Request.LocalEndPoint.ToString());
					}
					Response.Send(HttpStatusCode.OK);
					return;

				default:
					//Resources only support the three methods defined above, so send back a 405 Method Not Allowed.
					Log.Warning("Refused request for resource " + Target + ": Method Not Allowed (" + Request.HttpMethod + ")");
					Response.Send(Utils.GetErrorPage(HttpStatusCode.MethodNotAllowed), HttpStatusCode.MethodNotAllowed, "text/html");
					return;
			}
		}
	}
}