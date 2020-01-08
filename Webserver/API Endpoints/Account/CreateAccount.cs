using System;
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
		public override void POST() {
			//Get all required fields
			if (
				!JSON.TryGetValue<string>("Email", out JToken Email) ||
				!JSON.TryGetValue<string>("Password", out JToken Password)
			) {
				Response.Send("Missing fields", HttpStatusCode.BadRequest);
				return;
			}

			//Check if a user already exists with this email. If it isn't, send a 400 Bad Request
			if ( User.GetUserByEmail(Connection, (string)Email) != null ) {
				Response.Send("A user with this email already exists", HttpStatusCode.BadRequest);
				return;
			}

			//Check if the email is valid. If it isn't, send a 400 Bad Request.

			Regex EmailRx = new Regex("^[A-z0-9]*@[A-z0-9]*\\.[A-z]{1,}$");
			if ( !EmailRx.IsMatch((string)Email) ) {
				Response.Send("Invalid email", HttpStatusCode.BadRequest);
				return;
			}

			//Check if the password is valid. If it isn't, send a 400 Bad Request.
			Regex PasswordRx = new Regex((string)Config.GetValue("AuthenticationSettings.PasswordRegex"));
			if ( !PasswordRx.IsMatch((string)Password) || ( (string)Password ).Length == 0 ) {
				Response.Send("Password does not meet requirements", HttpStatusCode.BadRequest);
				return;
			}

			//Create a new user
			User NewUser = new User((string)Email, (string)Password, Connection);

			//Set department permissions where necessary
			NewUser.SetPermissionLevel(Connection, PermLevel.User, 2);
			if (JSON.TryGetValue<JObject>("MemberDepartments", out JToken MemberDepartment)) {
				JObject Perms = (JObject)MemberDepartment;
				foreach (KeyValuePair<string, JToken> Entry in Perms) {
					//Check if the specified department exists, skip if it doesn't.
					Department Dept = Department.GetByName(Connection, Entry.Key);
					if (Dept == null) {
						continue;
					}
					//Check if the specified account type is valid. If it isn't, skip it.
					if (!Enum.TryParse((string)Entry.Value, out PermLevel Level)) {
						continue;
					}
					//If the new user has a greater perm than the requestuser, skip it.
					if (Level > RequestUserLevel) {
						continue;
					}

					//Set level
					NewUser.SetPermissionLevel(Connection, Level, Dept);
				}
			}

			//Set optional fields
			foreach ( var x in JSON ) {
				if ( x.Key == "Email" || x.Key == "Password" ) {
					continue;
				}
				PropertyInfo Prop = NewUser.GetType().GetProperty(x.Key);
				if ( Prop == null ) {
					continue;
				}
				dynamic Value = x.Value.ToObject(Prop.PropertyType);
				Prop.SetValue(NewUser, Value);
			}

			//Upload account to database
			Connection.Update<User>(NewUser);
			
			//Send OK
			Response.Send(HttpStatusCode.Created);
		}
	}
}
