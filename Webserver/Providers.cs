﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Webserver {
	/// <summary>
	/// Acts as an in-between for HttpListenerContext instances and the RequestWorkers. This allows us to create mock requests for testing purposes.
	/// </summary>
	public class ContextProvider {
		public RequestProvider Request;
		public ResponseProvider Response;

		public ContextProvider(HttpListenerContext Context) {
			Request = new RequestProvider(Context.Request);
			Response = new ResponseProvider(Context.Response);
		}

		public ContextProvider(RequestProvider Request) {
			this.Request = Request;
			this.Response = new ResponseProvider();
		}
	}

	/// <summary>
	/// Acts as an in-between for HttpListenerRequest instances and the RequestWorkers. This allows us to create mock requests for testing purposes.
	/// </summary>
	public class RequestProvider {
		//private readonly HttpListenerRequest Request;

		public Encoding ContentEncoding { get; set; }
		public string ContentType { get; set; }
		public CookieCollection Cookies { get; set; } = new CookieCollection();
		public HttpMethod HttpMethod { get; set; }
		public Dictionary<string, List<string>> Params { get; set; } = new Dictionary<string, List<string>>();
		public Dictionary<string, List<string>> Headers { get; set; } = new Dictionary<string, List<string>>();
		public Stream InputStream { get; set; }
		public IPEndPoint LocalEndPoint { get; set; }
		public Uri Url { get; set; }

		public RequestProvider(HttpListenerRequest Request) {
			//this.Request = Request;

			//Set values
			ContentEncoding = Request.ContentEncoding;
			ContentType = Request.ContentType;
			Cookies = Request.Cookies;
			Params = Utils.NameValueToDict(Request.QueryString);
			Headers = Utils.NameValueToDict(Request.Headers);
			HttpMethod = Enum.Parse<HttpMethod>(Request.HttpMethod);
			InputStream = Request.InputStream;
			LocalEndPoint = Request.LocalEndPoint;
			Url = Request.Url;
		}

		public RequestProvider(Uri Url, HttpMethod HttpMethod) {
			this.Url = Url;
			this.Params = Utils.NameValueToDict(HttpUtility.ParseQueryString(Url.Query));
			this.HttpMethod = HttpMethod;
			this.ContentEncoding = Encoding.UTF8;
		}
	}

	/// <summary>
	/// Acts as an in-between for HttpListenerResponse instances and the RequestWorkers. This allows us to create mock requests for testing purposes.
	/// </summary>
	public class ResponseProvider {
		private readonly HttpListenerResponse Response;

		public byte[] Data;
		public string TextData;

		/// <summary>
		/// Gets or sets the MIME type of the content returned.
		/// </summary>
		private string _ContentType;
		public string ContentType {
			get => _ContentType;
			set {
				if (Response != null) Response.ContentType = value;
				_ContentType = value;
			}
		}

		/// <summary>
		/// Gets or sets the HTTP status code to be returned to the client.
		/// </summary>
		private HttpStatusCode _StatusCode;
		public HttpStatusCode StatusCode {
			get => _StatusCode;
			set {
				if ( Response != null ) Response.StatusCode = (int)value;
				_StatusCode = value;
			}
		}

		public readonly WebHeaderCollection Headers = new WebHeaderCollection();
		public string RedirectURL;

		public ResponseProvider(HttpListenerResponse Response) {
			this.Response = Response;


			ContentType = Response.ContentType;
			StatusCode = (HttpStatusCode)Response.StatusCode;
		}

		public ResponseProvider() {}

		public void AppendHeader(string name, string value) {
			Response?.AppendHeader(name, value);
			Headers.Add(name, value);
		}

		/// <summary>
		/// Send just a status code to the client, answering the request.
		/// </summary>
		/// <param name="StatusCode"></param>
		public void Send(HttpStatusCode StatusCode = HttpStatusCode.OK) => Send(string.Empty, StatusCode);

		/// <summary>
		/// Send JSON data to the client, answering the request.
		/// </summary>
		/// <param name="JSON"></param>
		/// <param name="StatusCode"></param>
		public void Send(JToken JSON, HttpStatusCode StatusCode = HttpStatusCode.OK) => Send(JSON.ToString(Formatting.None), StatusCode, "application/json");

		/// <summary>
		/// Sends data to the client in the form of a byte array.
		/// </summary>
		/// <param name="Data">The data to be sent to the client.</param>
		/// <param name="Response">The Response object</param>
		/// <param name="StatusCode">The HttpStatusCode. Defaults to HttpStatusCode.OK (200)</param>
		public void Send(byte[] Data, HttpStatusCode StatusCode = HttpStatusCode.OK, string ContentType = "text/html") {
			if ( Data == null ) Data = Array.Empty<byte>();
			this.Data = Data;
			this.StatusCode = StatusCode;
			this.ContentType = ContentType;

			if(Response != null ) {
				try {
					Response.OutputStream.Write(Data, 0, Data.Length);
					Response.OutputStream.Close();
				} catch ( HttpListenerException e ) {
					Program.Log?.Error("Failed to send data to host: " + e.Message);
				}
			}
		}

		/// <summary>
		/// Sends data to the client, answering the request.
		/// </summary>
		/// <param name="Data">The data to be sent to the client.</param>
		/// <param name="Response">The Response object</param>
		/// <param name="StatusCode">The HttpStatusCode. Defaults to HttpStatusCode.OK (200)</param>
		public void Send(string Data, HttpStatusCode StatusCode = HttpStatusCode.OK, string ContentType = "text/plain") {
			TextData = Data;
			if ( Data == null ) Data = string.Empty;
			byte[] Buffer = Encoding.UTF8.GetBytes(Data.ToString());
			Send(Buffer, StatusCode, ContentType);
		}

		public void Redirect(string URL) {
			RedirectURL = URL;
			Response?.Redirect(URL);
		}
	}

	public enum HttpMethod {
		GET,
		HEAD,
		POST,
		PUT,
		DELETE,
		CONNECT,
		OPTIONS,
		TRACE,
		PATCH
	}
}
