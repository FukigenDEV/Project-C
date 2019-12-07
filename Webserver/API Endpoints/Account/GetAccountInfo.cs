﻿using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Webserver.Data;

namespace Webserver.API_Endpoints {
	[EndpointURL("/account")]
	public partial class AccountEndpoint : APIEndpoint {
		[PermissionLevel(PermLevel.Manager)]
		public override void GET() {
			//Get required fields
			List<User> Users = new List<User>();
			if ( Params.ContainsKey("email") ) {

				//Check if the specified user exists. If it doesn't, send a 404 Not Found
				User Acc = User.GetUserByEmail(Connection, Params["email"][0]);
				if ( Acc == null ) {
					Response.Send("No such user", HttpStatusCode.NotFound);
					return;
				}
				Users.Add(Acc);

			} else {
				Users = User.GetAllUsers(Connection);
			}

			//If a department was specified, only return permission level and department
			JArray JSON = new JArray();
			if ( Params.ContainsKey("department") ) {
				//Get department. If no department is found, return a 404
				Department Dept = Department.GetByName(Connection, Params["department"][0]);
				if ( Dept == null ) {
					Response.Send("No such department", HttpStatusCode.NotFound);
					return;
				}

				foreach ( User Acc in Users ) {

					JSON.Add(new JObject() { { "Email", Acc.Email }, { "Level", Acc.GetPermissionLevel(Connection, Dept).ToString() } });
				}

			} else {
				//Convert user objects to JSON
				foreach ( User Acc in Users ) {
					JObject UserObject = JObject.FromObject(Acc);
					UserObject.Remove("PasswordHash"); //Security
					JSON.Add(UserObject);
				}
			}

			//Send response
			Response.Send(JSON);
		}
	}
}
