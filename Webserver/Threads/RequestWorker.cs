﻿using Configurator;
using Dapper.Contrib.Extensions;
using Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
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
					Log.Error("Couldn't resolve URL; infinite redirection loop");
					Utils.Send(Response, null, HttpStatusCode.LoopDetected);

				} else if (URL != Request.RawUrl.ToLower()) {
					Response.Redirect(URL);
					Utils.Send(Response, null, HttpStatusCode.Redirect);
					continue;
				}

				//Append wwwroot to target
				string Target = Config.GetValue("WebserverSettings.wwwroot") + Request.RawUrl.ToLower();

				//Switch to contentType
				switch (Request.ContentType) {

					//JSON
					case "application/json":

						bool Handled = false;
						//Search for the right endpoint.
						foreach (Type T in Program.Endpoints) {
							//Get endpoint info attribute. If the attribute is missing, show an error in console and continue to the next.
							EndpointInfo Info = (EndpointInfo)Attribute.GetCustomAttribute(T, typeof(EndpointInfo));
							if (Info == null) {
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

								#region SkipSessionCheck
								//Check session cookie if necessary.
								if (Method.GetCustomAttribute<SkipSessionCheck>() == null) {
									Cookie SessionIDCookie = Request.Cookies["SessionID"];
									if (SessionIDCookie == null) {
										Utils.Send(Response, "No Session", HttpStatusCode.Unauthorized);
										continue;
									}
									Session s = Session.GetUserSession(Connection, SessionIDCookie.Value);
									if (s == null) {
										Utils.Send(Response, "Expired session", HttpStatusCode.Unauthorized);
										continue;
									}
									s.Renew(Connection);
									ep.UserSession = s;
									ep.RequestUser = Connection.Get<User>(s.User);
								}
								#endregion

								#region PermissionLevel
								//Check permission
								PermissionLevel Attr = Method.GetCustomAttribute<PermissionLevel>();
								if (Attr != null) {
									//Check for endpoint conflicts
									if (ep.RequestUser == null) {
										Log.Error("Endpoint attribute conflict for endpoint " + Target);
										Utils.Send(Response, null, HttpStatusCode.InternalServerError);
										continue;
									}

									PermLevel Level;
									if (ep.RequestUser.GetPermissionLevel(Connection, 1) == PermLevel.Administrator) {
										ep.RequestUserLevel = PermLevel.Administrator;
									} else {
										//Get Department value
										if (!ep.Content.TryGetValue<string>("Department", out JToken DepartmentVal)) {
											Utils.Send(Response, "Missing Department", HttpStatusCode.BadRequest);
											continue;
										}

										//Get department
										Department Dept = Department.GetDepartmentByName(Connection, (string)DepartmentVal);
										if (Dept == null) {
											Utils.Send(Response, "No such Department", HttpStatusCode.BadRequest);
											continue;
										}

										//Get permissionlevel
										Level = ep.RequestUser.GetPermissionLevel(Connection, Dept.ID);
										ep.RequestUserLevel = Level;

										//Check permission level
										if (Level < Attr.Level) {
											Log.Warning("User " + ep.RequestUser.Email + " attempted to access API endpoint " + Target + " without sufficient permissions");
											Log.Warning("Department: '" + (string)DepartmentVal + "', User is '" + Level + "' but must be at least '" + Attr.Level + "'");
											Utils.Send(Response, null, HttpStatusCode.Forbidden);
											continue;
										}
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
							}
						}
						//If no endpoint was found, send a 404.
						if (!Handled) {
							Log.Info("Received " + Request.HttpMethod + " request for invalid endpoint at address " + Target + " from " + Request.UserHostName);
							Utils.Send(Response, Utils.GetErrorPage(HttpStatusCode.NotFound), HttpStatusCode.NotFound);
						}
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
