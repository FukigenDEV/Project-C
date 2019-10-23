using Dapper.Contrib.Extensions;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using Webserver.Data;

namespace Webserver.API_Endpoints {
	internal partial class Account : APIEndpoint {
		[PermissionLevel(PermLevel.Manager)]
		public override void PATCH() {
			//Get required fields
			if (!Content.TryGetValue("Email", out JToken Email)) {
				Send("Missing fields", HttpStatusCode.BadRequest);
				return;
			}

			//Administrator account can't be modified;
			if((string)Email == "Administrator") {
				Send(StatusCode: HttpStatusCode.Forbidden);
				return;
			}

			//Check if the specified user exists. If it doesn't, send a 404 Not Found
			User Acc = User.GetUserByEmail(Connection, (string)Email);
			if (Acc == null) {
				Send("No such user", HttpStatusCode.NotFound);
				return;
			}

			//Change email if necessary
			if(Content.TryGetValue("NewEmail", out JToken NewEmail)){
				//Check if the new address is valid
				Regex rx = new Regex("[A-z0-9]*@[A-z0-9]*.[A-z]*");
				if (rx.IsMatch((string)NewEmail)) {
					Acc.Email = (string)NewEmail;
				} else {
					Send("Invalid Email", HttpStatusCode.BadRequest);
					return;
				}
			}

			//Change password if necessary
			if(Content.TryGetValue("Password", out JToken Password)) {
				Acc.PasswordHash = User.CreateHash((string)Password, Acc.Email);
			}

			//Set optional fields
			foreach (var x in Content) {
				if (x.Key == "Email" || x.Key == "PasswordHash") {
					continue;
				}
				PropertyInfo Prop = Acc.GetType().GetProperty(x.Key);
				if (Prop == null) {
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
