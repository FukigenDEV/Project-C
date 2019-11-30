﻿using System.Net;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json.Linq;
using Webserver.Data;

namespace Webserver.API_Endpoints {
	internal partial class AccountEndpoint : APIEndpoint {
		[PermissionLevel(PermLevel.Administrator)]
		[RequireBody]
		[RequireContentType("application/json")]
		public override void DELETE() {
			//Get required fields
			if ( !JSON.TryGetValue<string>("Email", out JToken Email) ) {
				Send("Missing fields", HttpStatusCode.BadRequest);
				return;
			}

			//Cancel if Email is "Administrator", because the built-in Admin shouldn't ever be deleted.
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

			Connection.Delete(Acc);
			Send(StatusCode: HttpStatusCode.OK);
		}
	}
}
