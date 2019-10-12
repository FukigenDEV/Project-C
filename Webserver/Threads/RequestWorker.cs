using Configurator;
using Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

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
								APIEndpoint ep = (APIEndpoint)Activator.CreateInstance(T, new object[] { Connection, Context });

								MethodInfo Method = ep.GetType().GetMethod(Request.HttpMethod);
								try {
									Method.Invoke(ep, null);
#pragma warning disable CA1031 // Do not catch general exception types
								} catch (Exception e) {
									Utils.Send(Utils.GetErrorPage(HttpStatusCode.InternalServerError, e.Message), Context.Response, HttpStatusCode.InternalServerError);
								}
#pragma warning restore CA1031 // Do not catch general exception types

								Handled = true;
							}
						}
						//If no endpoint was found, send a 404.
						if (!Handled) {
							Log.Info("Received " + Request.HttpMethod + " request for invalid endpoint at address " + Target + " from " + Request.UserHostName);
							Utils.Send(Utils.GetErrorPage(HttpStatusCode.NotFound), Context.Response, HttpStatusCode.NotFound);
						}
						break;

					//HTML
					default:
						//Browsers apparently don't set content type when asking for webpages.
						//If the page exists, load and send it. Otherwise, send a 404.
						if (Program.WebPages.Contains(Target)) {
							using StreamReader Reader = new StreamReader(File.Open(Target, FileMode.Open));
							Utils.Send(Reader.ReadToEnd(), Context.Response, HttpStatusCode.OK);
						} else {
							Log.Info("Received "+Request.HttpMethod+" request for invalid webpage at address "+ Request.RawUrl + " from "+Request.UserHostName);
							Utils.Send(Utils.GetErrorPage(HttpStatusCode.NotFound), Context.Response, HttpStatusCode.NotFound);
						}
						break;
				}
			}
		}
	}
}
