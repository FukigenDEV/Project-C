using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using Webserver.Data;

namespace Webserver {
	public abstract class APIEndpoint {
		public HttpListenerContext Context;
		public SQLiteConnection Connection;
		public HttpListenerRequest Request;
		public HttpListenerResponse Response;

		/// <summary>
		/// The request's body, converted to a JObject. Only available for endpoint methods that require "application/json" as content type.
		/// </summary>
		public JObject JSON = null;
		/// <summary>
		/// The request's body. Null if the content type requires conversion first. In this case, the body will be located in another variable.
		/// </summary>
		public dynamic Content = null;
		/// <summary>
		/// The user who sent the request. Null if no user was logged in. 
		/// </summary>
		public User RequestUser = null;
		/// <summary>
		/// The permission level of the user who sent the request.
		/// </summary>
		public PermLevel RequestUserLevel;
		/// <summary>
		/// The session object of the user who sent the request.
		/// </summary>
		public Session UserSession = null;
		/// <summary>
		/// The request parameters that were used with this request.
		/// </summary>
		public Dictionary<string, List<string>> RequestParams = new Dictionary<string, List<string>>();

		/// <summary>
		/// Called when a HTTP.GET request is sent to this endpoint.
		/// A GET request should only be used to retrieve data from the server.
		/// </summary>
		public virtual void GET() => Utils.Send(Response, null, HttpStatusCode.MethodNotAllowed);

		/// <summary>
		/// Called when a HTTP.HEAD request is sent to this endpoint.
		/// HEAD requests ask for the same response as GET requests do, but without a response body.
		/// </summary>
		public virtual void HEAD() => Utils.Send(Response, null, HttpStatusCode.MethodNotAllowed);

		/// <summary>
		/// Called when a HTTP.POST request is sent to this endpoint.
		/// POST requests are used to submit new data to the server.
		/// </summary>
		public virtual void POST() => Utils.Send(Response, null, HttpStatusCode.MethodNotAllowed);

		/// <summary>
		/// Called when a HTTP.PUT request is sent to this endpoint.
		/// PUT requests are used to replace data on the server.
		/// </summary>
		public virtual void PUT() => Utils.Send(Response, null, HttpStatusCode.MethodNotAllowed);

		/// <summary>
		/// Called when a HTTP.DELETE request is sent to this endpoint.
		/// DELETE requests are used to delete data from the server.
		/// </summary>
		public virtual void DELETE() => Utils.Send(Response, null, HttpStatusCode.MethodNotAllowed);

		/// <summary>
		/// Called when a HTTP.CONNECT request is sent to this endpoint.
		/// A CONNECT request is used to establish a tunnel to the server, identified by the targeted resource
		/// </summary>
		public virtual void CONNECT() => Utils.Send(Response, null, HttpStatusCode.MethodNotAllowed);

		/// <summary>
		/// Called when a HTTP.OPTIONS request is sent to this endpoint.
		/// The OPTIONS method returns the HTTP methods that the server supports for the specified URL.
		/// The method cannot be overriden, as its implementation must be the same for all endpoints.
		/// </summary>
		public void OPTIONS() {
			List<string> AllowedMethods = new List<string>();
			foreach(MethodInfo Method in GetType().GetMethods()) {
				if(Method.DeclaringType == GetType()) {
					AllowedMethods.Add(Method.Name);
				}
			}
			if (Program.CORSAddresses.Contains("http://"+Request.LocalEndPoint.ToString())) {
				Console.WriteLine("yeet");
				Response.Headers.Add("Access-Control-Allow-Origin", Request.LocalEndPoint.ToString());
			}
			Response.Headers.Add("Allow", string.Join(", ", AllowedMethods));
			Utils.Send(Response, null, HttpStatusCode.OK);
		}

		/// <summary>
		/// Called when a HTTP.TRACE request is sent to this endpoint.
		/// A TRACE request is used to perform a message loop-back test along the path to the target resource.
		/// </summary>
		public virtual void TRACE() => Utils.Send(Response, null, HttpStatusCode.MethodNotAllowed);

		/// <summary>
		/// Called when a HTTP.PATCH request is sent to this endpoint.
		/// A PATH request is used to partially modify data.
		/// </summary>
		public virtual void PATCH() => Utils.Send(Response, null, HttpStatusCode.MethodNotAllowed);

		/// <summary>
		/// Sends data to the client, answering the request.
		/// </summary>
		/// <param name="Data">The data to send.</param>
		/// <param name="StatusCode"></param>
		/// <param name="ContentType"></param>,
		public void Send(object Data = null, HttpStatusCode StatusCode = HttpStatusCode.OK, string ContentType = null) {
			if(ContentType == null) {
				ContentType = new StackTrace().GetFrame(1).GetMethod().GetCustomAttribute<RequireContentType>().ContentType;
			}
			Utils.Send(Response, Data?.ToString(), StatusCode, ContentType);
		}

		/// <summary>
		/// Send a cookie to the client.
		/// </summary>
		/// <param name="cookie">The Cookie object to send. Only the Name and Value fields will be used.</param>
		public void AddCookie(Cookie cookie) => AddCookie(cookie.Name, cookie.Value, (int)cookie.Expires.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds);

		/// <summary>
		/// Send a cookie to the client. Always use this function to add cookies, as the built-in functions don't work properly.
		/// </summary>
		/// <param name="name">The cookie's name</param>
		/// <param name="value">The cookie's value</param>
		public void AddCookie(string name, string value, long Expire) {
			string CookieVal = name + "=" + value;

			if(Expire < 0) {
				throw new ArgumentOutOfRangeException("Negative cookie expiration");
			}
			CookieVal += "; Max-Age=" + Expire;

			Response.AppendHeader("Set-Cookie", CookieVal);
		}
	}
}
