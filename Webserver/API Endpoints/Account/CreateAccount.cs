﻿using Configurator;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using Webserver.Data;

namespace Webserver.API_Endpoints {
	internal partial class AccountEndpoint : APIEndpoint {
		[PermissionLevel(PermLevel.Manager)]
		[RequireBody]
		[RequireContentType("application/json")]
		public override void POST() {
			//Get all required fields
			if (
				!JSON.TryGetValue<string>("Email", out JToken Email) ||
				!JSON.TryGetValue<string>("Password", out JToken Password) ||
				!JSON.TryGetValue<string>("AccountType", out JToken AccountType) ||
				!JSON.TryGetValue<string>("MemberOf", out JToken MemberDept)
			) {
				Send("Missing fields", HttpStatusCode.BadRequest);
				return;
			}

			//Check if a user already exists with this email. If it isn't, send a 400 Bad Request
			if (User.GetUserByEmail(Connection, (string)Email) != null) {
				Send("A user with this email already exists", HttpStatusCode.BadRequest);
				return;
			}

			//Check if the email is valid. If it isn't, send a 400 Bad Request.
			Regex EmailRx = new Regex("[A-z0-9]*@[A-z0-9]*\\.[A-z]{1}");
			if (!EmailRx.IsMatch((string)Email)) {
				Send("Invalid email", HttpStatusCode.BadRequest);
				return;
			}

			//Check if the password is valid. If it isn't, send a 400 Bad Request.
			Regex PasswordRx = new Regex((string)Config.GetValue("AuthenticationSettings.PasswordRegex"));
			if (!PasswordRx.IsMatch((string)Password) || ((string)Password).Length == 0) {
				Send("Password does not meet requirements", HttpStatusCode.BadRequest);
				return;
			}

			//Check if the specified account type is valid.
			if (!PermLevel.TryParse((string)AccountType, out PermLevel level)) {
				Send("Invalid accountType", HttpStatusCode.BadRequest);
				return;
			}
			//If the new user has a greater perm than the requestuser, send a 403 Forbidden.
			if (level > RequestUserLevel) {
				Send("Can't create " + level + " as " + RequestUserLevel, HttpStatusCode.Forbidden);
				return;
			}

			//Check if the department is valid
			Department Dept = Department.GetDepartmentByName(Connection, (string)MemberDept);
			if (Dept == null) {
				Send("No such department", HttpStatusCode.BadRequest);
				return;
			}

			//Create a new user
			User NewUser = new User((string)Email, (string)Password);

			//Set optional fields
			foreach (var x in JSON) {
				if (x.Key == "Email" || x.Key == "Password") {
					continue;
				}
				PropertyInfo Prop = NewUser.GetType().GetProperty(x.Key);
				if (Prop == null) {
					continue;
				}
				dynamic Value = x.Value.ToObject(Prop.PropertyType);
				Prop.SetValue(NewUser, Value);
			}

			//Upload account to database and set permission
			Connection.Insert(NewUser);
			NewUser.SetPermissionLevel(Connection, level, Dept);
			NewUser.SetPermissionLevel(Connection, PermLevel.User, 2);

			//Send OK
			Send(StatusCode: HttpStatusCode.Created);
		}
	}
}
