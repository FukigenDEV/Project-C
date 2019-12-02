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
			string Email;
			if ( RequestParams.ContainsKey("email") ) {
				Email = RequestParams["email"][0];
			} else {
				Email = RequestUser.Email;
			}

			//Check if the specified user exists. If it doesn't, send a 404 Not Found
			User Acc = User.GetUserByEmail(Connection, Email);
			if ( Acc == null ) {
				Send("No such user", HttpStatusCode.NotFound);
				return;
			}

			//If a department was specified, only return permission level and department
			JObject JSON;
			if ( RequestParams.ContainsKey("department") ) {
				//Get department. If no department is found, return a 404
				Department Dept = Department.GetByName(Connection, RequestParams["department"][0]);
				if ( Dept == null ) {
					Send("No such department", HttpStatusCode.NotFound);
					return;
				}
				//Build JSON
				JSON = new JObject {
					{ "Department", Dept.Name },
					{ "Permission:", Acc.GetPermissionLevel(Connection, Dept).ToString() }
				};
			} else {
				//Convert user object to JSON
				JSON = JObject.FromObject(Acc);
				JSON.Remove("PasswordHash"); //For security
			}

			//Send response
			Send(JSON, HttpStatusCode.OK);
		}
	}
}
