using Dapper.Contrib.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Webserver.Data;

namespace Webserver.API_Endpoints {

	/// <summary>
	/// API endpoint to manage user logins.
	/// </summary>
	[EndpointInfo("application/json", "/login")]
	class Login : APIEndpoint {
		[SkipSessionCheck]
		public override void GET() {
			//Check if a session cookie was sent.
			Cookie SessionIDCookie = Request.Cookies["SessionID"];
			Session s;
			if(SessionIDCookie != null) {
				//Get the session belonging to this session ID. If the session is still valid, renew it. If it isn't, send back a 401 Unauthorized, signaling that the client should send an email and password to reauthenticate
				s = Session.GetUserSession(Connection, SessionIDCookie.Value);
				
				if(s != null) {
					s.Renew(Connection);
					Send(StatusCode: HttpStatusCode.NoContent);
				} else {
					Send(StatusCode: HttpStatusCode.Unauthorized);
				}
			}
			
			//Get the email and password from the request. If one of the values is missing, send a 400 Bad Request.
			bool foundEmail = Content.TryGetValue("Email", out JToken Email);
			bool foundPassword = Content.TryGetValue("Password", out JToken Password);
			bool foundRememberMe = Content.TryGetValue("RememberMe", out JToken RememberMe);
			if(!foundEmail || !foundPassword || !foundRememberMe) {
				Send("Missing fields", HttpStatusCode.BadRequest);
				return;
			}

			//Check if the email is valid. If it isn't, send a 400 Bad Request.
			Regex rx = new Regex("[A-z0-9]*@[A-z0-9]*.[A-z]*");
			if (!rx.IsMatch((string)Email) && (string)Email != "Administrator") {
				Send("Invalid Email", HttpStatusCode.BadRequest);
				return;
			}

			//Check if the user exists. If it doesn't, send a 400 Bad Request
			User Account = User.GetUserByEmail(Connection, (string)Email);
			if(Account == null) {
				Send("No such user", HttpStatusCode.BadRequest);
				return;
			}
			
			//Check password. If its invalid, return a 401 Unauthorized
			if(Account.PasswordHash != User.CreateHash((string)Password, (string)Email)) {
				Send(StatusCode: HttpStatusCode.Unauthorized);
				return;
			}

			//At this point, we know the user exists and that the credentials are valid. The user will now be logged in.
			//Create a new session, store it, and send back the Session ID
			s = new Session(Account.ID, (bool)RememberMe);
			Connection.Insert(s);

			AddCookie("SessionID", s.SessionID);
			Send(StatusCode:HttpStatusCode.NoContent);
		}
	}
}
