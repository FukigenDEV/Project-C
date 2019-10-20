using Newtonsoft.Json.Linq;
using System.Data.SQLite;
using System.Net;
using Webserver.Data;

namespace Webserver {
	public abstract class APIEndpoint {
		public HttpListenerContext Context;
		public SQLiteConnection Connection;
		public HttpListenerRequest Request;
		public HttpListenerResponse Response;
		public JObject Content;
		public User RequestUser;
		public PermLevel RequestUserLevel;
		public Session UserSession;

		/// <summary>
		/// Called when a HTTP.GET request is sent to this endpoint.
		/// A GET request should only be used to retrieve data from the server.
		/// </summary>
		public virtual void GET() => Utils.Send(Response, Utils.GetErrorPage(HttpStatusCode.MethodNotAllowed), HttpStatusCode.MethodNotAllowed);

		/// <summary>
		/// Called when a HTTP.HEAD request is sent to this endpoint.
		/// HEAD requests ask for the same response as GET requests do, but without a response body.
		/// </summary>
		public virtual void HEAD() => Utils.Send(Response, Utils.GetErrorPage(HttpStatusCode.MethodNotAllowed), HttpStatusCode.MethodNotAllowed);
		/// <summary>
		/// Called when a HTTP.POST request is sent to this endpoint.
		/// POST requests are used to submit new data to the server.
		/// </summary>
		public virtual void POST() => Utils.Send(Response, Utils.GetErrorPage(HttpStatusCode.MethodNotAllowed), HttpStatusCode.MethodNotAllowed);
		/// <summary>
		/// Called when a HTTP.PUT request is sent to this endpoint.
		/// PUT requests are used to replace data on the server.
		/// </summary>
		public virtual void PUT() => Utils.Send(Response, Utils.GetErrorPage(HttpStatusCode.MethodNotAllowed), HttpStatusCode.MethodNotAllowed);

		/// <summary>
		/// Called when a HTTP.DELETE request is sent to this endpoint.
		/// DELETE requests are used to delete data from the server.
		/// </summary>
		public virtual void DELETE() => Utils.Send(Response, Utils.GetErrorPage(HttpStatusCode.MethodNotAllowed), HttpStatusCode.MethodNotAllowed);

		/// <summary>
		/// Called when a HTTP.CONNECT request is sent to this endpoint.
		/// A CONNECT request is used to establish a tunnel to the server, identified by the targeted resource
		/// </summary>
		public virtual void CONNECT() => Utils.Send(Response, Utils.GetErrorPage(HttpStatusCode.MethodNotAllowed), HttpStatusCode.MethodNotAllowed);

		/// <summary>
		/// Called when a HTTP.OPTIONS request is sent to this endpoint.
		/// An OPTIONS request is used to describe the communication options for the target resource.
		/// </summary>
		public virtual void OPTIONS() => Utils.Send(Response, Utils.GetErrorPage(HttpStatusCode.MethodNotAllowed), HttpStatusCode.MethodNotAllowed);

		/// <summary>
		/// Called when a HTTP.TRACE request is sent to this endpoint.
		/// A TRACE request is used to perform a message loop-back test along the path to the target resource.
		/// </summary>
		public virtual void TRACE() => Utils.Send(Response, Utils.GetErrorPage(HttpStatusCode.MethodNotAllowed), HttpStatusCode.MethodNotAllowed);

		/// <summary>
		/// Called when a HTTP.PATCH request is sent to this endpoint.
		/// A PATH request is used to partially modify data.
		/// </summary>
		public virtual void PATCH() => Utils.Send(Response, Utils.GetErrorPage(HttpStatusCode.MethodNotAllowed), HttpStatusCode.MethodNotAllowed);

		/// <summary>
		/// Sends data to the client, answering the request.
		/// </summary>
		/// <param name="Data">The data to send.</param>
		/// <param name="StatusCode"></param>
		public void Send(object Data = null, HttpStatusCode StatusCode = HttpStatusCode.OK) => Utils.Send(Response, Data?.ToString(), StatusCode);

		/// <summary>
		/// Send a cookie to the client.
		/// </summary>
		/// <param name="cookie">The Cookie object to send. Only the Name and Value fields will be used.</param>
		public void AddCookie(Cookie cookie) => AddCookie(cookie.Name, cookie.Value);

		/// <summary>
		/// Send a cookie to the client. Always use this function to add cookies, as the built-in functions don't work properly.
		/// </summary>
		/// <param name="name">The cookie's name</param>
		/// <param name="value">The cookie's value</param>
		public void AddCookie(string name, string value) => Response.AppendHeader("Set-Cookie", name + "=" + value);
	}
}
