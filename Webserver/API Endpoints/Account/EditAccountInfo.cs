using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json.Linq;
using Webserver.Data;

namespace Webserver.API_Endpoints {
	internal partial class AccountEndpoint : APIEndpoint {
		[PermissionLevel(PermLevel.Manager)]
		[RequireBody]
		[RequireContentType("application/json")]
		public override void PATCH() {
			//Get required fields
			if ( !JSON.TryGetValue<string>("Email", out JToken Email) ) {
				Send("Missing fields", HttpStatusCode.BadRequest);
				return;
			}

			//Administrator account can't be modified;
			if ( (string)Email == "Administrator" ) {
				Send(StatusCode: HttpStatusCode.Forbidden);
				return;
			}

			//Check if the specified user exists. If it doesn't, send a 404 Not Found
			User Acc = User.GetUserByEmail(Connection, (string)Email);
			if ( Acc == null ) {
				Send("No such user", HttpStatusCode.NotFound);
				return;
			}

			//Change email if necessary
			if ( JSON.TryGetValue<string>("NewEmail", out JToken NewEmail) ) {
				//Check if the new address is valid
				Regex rx = new Regex("[A-z0-9]*@[A-z0-9]*.[A-z]*");
				if ( rx.IsMatch((string)NewEmail) ) {
					Acc.Email = (string)NewEmail;
				} else {
					Send("Invalid Email", HttpStatusCode.BadRequest);
					return;
				}
			}

			//Change password if necessary
			if ( JSON.TryGetValue<string>("Password", out JToken Password) ) {
				Acc.PasswordHash = User.CreateHash((string)Password, Acc.Email);
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
			Send(StatusCode: HttpStatusCode.OK);
		}
	}
}
