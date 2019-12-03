﻿using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Webserver.Data;

namespace Webserver.API_Endpoints {
	[EndpointURL("/account")]
	internal partial class AccountEndpoint : APIEndpoint {
		[PermissionLevel(PermLevel.Manager)]
		[RequireContentType("application/json")]
		public override void GET() {
			//Get required fields
			List<User> Users = new List<User>();
			if ( RequestParams.ContainsKey("email") ) {

				//Check if the specified user exists. If it doesn't, send a 404 Not Found
				User Acc = User.GetUserByEmail(Connection, RequestParams["email"][0]);
				if ( Acc == null ) {
					Send("No such user", HttpStatusCode.NotFound);
					return;
				}
				Users.Add(Acc);

			} else {
				Users = User.GetAllUsers(Connection);
			}

			//If a department was specified, only return permission level and department
			JObject JSON = new JObject();
			if ( RequestParams.ContainsKey("department") ) {
				//Get department. If no department is found, return a 404
				Department Dept = Department.GetByName(Connection, RequestParams["department"][0]);
				if ( Dept == null ) {
					Send("No such department", HttpStatusCode.NotFound);
					return;
				}

				foreach(User Acc in Users ) {
					JSON.Add(Acc.Email, Acc.GetPermissionLevel(Connection, Dept).ToString());
				}

			} else {
				//Convert user objects to JSON
				foreach ( User Acc in Users ) {
					JSON.Add(Acc.Email, JObject.FromObject(Acc));
					((JObject)JSON[Acc.Email]).Remove("PasswordHash"); //Security
					( (JObject)JSON[Acc.Email] ).Remove("Email"); //Already exists as key
				}
			}

			//Send response
			Send(JSON, HttpStatusCode.OK);
		}
	}
}
