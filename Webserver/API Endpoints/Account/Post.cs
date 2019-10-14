using Dapper.Contrib.Extensions;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Reflection;
using Webserver.Data;

namespace Webserver.API_Endpoints {
	internal partial class Account : APIEndpoint {
		public override void POST() {
			//Get all required fields
			bool foundEmail = Content.TryGetValue("Email", out JToken Email);
			bool foundPassword = Content.TryGetValue("Password", out JToken Password);
			if(!foundEmail || !foundPassword) {
				Send("Missing fields", HttpStatusCode.BadRequest);
				return;
			}

			//Check if a user already exists with this email.
			if (User.GetUserByEmail(Connection, (string)Email) != null) {
				Send("Already exists", HttpStatusCode.BadRequest);
				return;
			}

			//Create a new user
			User NewUser = new User((string)Email, (string)Password);

			//Set optional fields
			foreach(var x in Content) {
				if(x.Key == "Email" || x.Key == "Password") {
					continue;
				}
				PropertyInfo Prop = NewUser.GetType().GetProperty(x.Key);
				if(Prop == null) {
					continue;
				}
				dynamic Value = x.Value.ToObject(Prop.PropertyType);
				Prop.SetValue(NewUser, Value);
			}

			//Upload account to database
			Connection.Insert(NewUser);

			//Send OK
			Send(StatusCode: HttpStatusCode.OK);
		}
	}
}
