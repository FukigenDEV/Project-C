﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using Configurator;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json.Linq;
using Webserver.Data;

namespace Webserver.API_Endpoints {
	public partial class AccountEndpoint : APIEndpoint {
		[PermissionLevel(PermLevel.Manager)]
		[RequireBody]
		[RequireContentType("application/json")]
		public override void PATCH() {
			//Get required fields
			if ( !Params.ContainsKey("email") ) {
				Response.Send("Missing fields", HttpStatusCode.BadRequest);
				return;
			}

			//Administrator account can't be modified;
			if (Params["email"][0] == "Administrator" ) {
				Response.Send(HttpStatusCode.Forbidden);
				return;
			}

			//Check if the specified user exists. If it doesn't, send a 404 Not Found
			User Acc = User.GetUserByEmail(Connection, Params["email"][0]);
			if ( Acc == null ) {
				Response.Send("No such user", HttpStatusCode.NotFound);
				return;
			}

			//Change email if necessary
			if ( JSON.TryGetValue<string>("Email", out JToken NewEmail) ) {
				//Check if the new address is valid
				Regex rx = new Regex("^[A-z0-9]*@[A-z0-9]*.[A-z]*$");
				if (!rx.IsMatch((string)NewEmail)) {
					Response.Send("Invalid Email", HttpStatusCode.BadRequest);
					return;
				}

				//Check if the new address is already in use
				if(User.GetUserByEmail(Connection, (string)NewEmail) != null) {
					Response.Send("New Email already in use", HttpStatusCode.BadRequest);
					return;
				}
				Acc.Email = (string)NewEmail;

			}

			//Change password if necessary
			if ( JSON.TryGetValue<string>("Password", out JToken Password) ) {
				Regex PasswordRx = new Regex((string)Config.GetValue("AuthenticationSettings.PasswordRegex"));
				if (!PasswordRx.IsMatch((string)Password) || ((string)Password).Length == 0) {
					Response.Send("Password does not meet requirements", HttpStatusCode.BadRequest);
					return;
				}
				Acc.ChangePassword(Connection, (string)Password);
			}

			//Set department permissions if necessary
			if ( JSON.TryGetValue<JObject>("MemberDepartments", out JToken MemberDepartment) ) {
				JObject Perms = (JObject)MemberDepartment;
				foreach ( KeyValuePair<string, JToken> Entry in Perms ) {
					//Check if the specified department exists, skip if it doesn't.
					Department Dept = Department.GetByName(Connection, Entry.Key);
					if ( Dept == null ) {
						continue;
					}
					//Check if the specified account type is valid. If it isn't, skip it.
					if ( !PermLevel.TryParse((string)Entry.Value, out PermLevel Level) ) {
						continue;
					}
					//If the new user has a greater perm than the requestuser, skip it.
					if ( Level > RequestUserLevel ) {
						continue;
					}

					//Set level
					Acc.SetPermissionLevel(Connection, Level, Dept);
				}
			}

			//Set optional fields
			foreach ( var x in JSON ) {
				if ( x.Key == "Email" || x.Key == "PasswordHash" ) {
					continue;
				}
				PropertyInfo Prop = Acc.GetType().GetProperty(x.Key);
				if ( Prop == null ) {
					continue;
				}
				dynamic Value = x.Value.ToObject(Prop.PropertyType);
				Prop.SetValue(Acc, Value);
			}

			//Update DB row
			Connection.Update<User>(Acc);
			Response.Send(StatusCode: HttpStatusCode.OK);
		}
	}
}
