using Configurator;
using Dapper.Contrib.Extensions;
using Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
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

				//Append wwwroot to target
				string Target = Config.GetValue("WebserverSettings.wwwroot") + Request.RawUrl.ToLower();
				Console.WriteLine(Request.RawUrl.ToLower());

				//Switch to contentType
				switch (Request.ContentType) {

					//JSON
					case "application/json":

						bool Handled = false;
						//Search for the right endpoint.
						foreach(Type T in Program.Endpoints) {
							//Get endpoint info attribute. If the attribute is missing, show an error in console and continue to the next.
							EndpointInfo Info = (EndpointInfo)Attribute.GetCustomAttribute(T, typeof(EndpointInfo));
							if(Info == null) {
								Log.Error("Endpoint " + T.Name + " has no EndpointInfo attribute");
								continue;
							}

							//Check if this is the correct endpoint. If it is, call it.
							if (Info.ContentType == Request.ContentType && Config.GetValue("WebserverSettings.wwwroot") + Info.URL == Target) {
								Handled = true;

								//Create an instance of the endpoint and set the fields.
								APIEndpoint ep = (APIEndpoint)Activator.CreateInstance(T);
								ep.Connection = Connection;
								ep.Context = Context;
								ep.Response = Context.Response;
								ep.Request = Context.Request;
								using StreamReader streamReader = new StreamReader(Request.InputStream, Request.ContentEncoding);
								ep.Content = JObject.Parse(streamReader.ReadToEnd());

								//Get the method
								MethodInfo Method = ep.GetType().GetMethod(Request.HttpMethod);

								//Check session cookie if necessary.
								if( Method.GetCustomAttribute<SkipSessionCheck>() == null) {
									Cookie SessionIDCookie = Request.Cookies["SessionID"];
									if(SessionIDCookie == null) {
										ep.Send(StatusCode: HttpStatusCode.Unauthorized);
										continue;
									}
									Session s = Session.GetUserSession(Connection, SessionIDCookie.Value);
									if(s == null) {
										ep.Send(StatusCode: HttpStatusCode.Unauthorized);
										continue;
									}
									s.Renew(Connection);
									ep.RequestUser = Connection.Get<User>(s.User);
								}

								try {
									Method.Invoke(ep, null);
#pragma warning disable CA1031 // Do not catch general exception types
								} catch (Exception e) {
									Utils.Send(Context.Response, Utils.GetErrorPage(HttpStatusCode.InternalServerError, e.Message), HttpStatusCode.InternalServerError);
								}
#pragma warning restore CA1031 // Do not catch general exception types
							}
						}
						//If no endpoint was found, send a 404.
						if (!Handled) {
							Log.Info("Received " + Request.HttpMethod + " request for invalid endpoint at address " + Target + " from " + Request.UserHostName);
							Utils.Send(Context.Response, Utils.GetErrorPage(HttpStatusCode.NotFound), HttpStatusCode.NotFound);
						}
						break;

					//HTML, CSS, JS, images
					default:
						//Browsers apparently don't set content type when asking for webpages.
						//If the file exists, load and send it. Otherwise, send a 404.
						if (Program.WebPages.Contains(Target) && File.Exists(Target)) {
							Utils.Send(Context.Response, File.ReadAllBytes(Target), HttpStatusCode.OK);
						} else {
							Log.Info("Received " + Request.HttpMethod + " request for invalid webpage at address " + Request.RawUrl + " from " + Request.UserHostName);
							Utils.Send(Context.Response, Utils.GetErrorPage(HttpStatusCode.NotFound), HttpStatusCode.NotFound);
						}
						
						break;
				}
			}
		}
	}
}
