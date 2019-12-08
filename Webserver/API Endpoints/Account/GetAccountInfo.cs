﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Dapper;
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
			if (Params.ContainsKey("email")) {

				//Get all user objects
				foreach(string Email in Params["email"]) {
					if (Email == "CurrentUser") {
						Users.Add(RequestUser);
						continue;
					}
					Users.Add(User.GetUserByEmail(Connection, Email));
				}

			//If email is missing, assume all users
			} else {
				Users = User.GetAllUsers(Connection);
			}

			//Convert to JSON and add permissionlevels
			List<Department> Departments = Department.GetAllDepartments(Connection);
			JArray JSON = JArray.FromObject(Users);
			foreach (JObject Entry in JSON) {
				Entry.Remove("PasswordHash"); //Security
				JObject PermissionInfo = new JObject();
				foreach(Department Dept in Departments) {
					PermissionInfo.Add(Dept.Name, User.GetPermissionLevel(Connection, (int)Entry["ID"], Dept.ID).ToString());
				}

				Entry.Add("Permissions", PermissionInfo);
			}

			//Send response
			Response.Send(JSON);
		}
	}
}
