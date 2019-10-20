﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using Webserver.Data;

namespace Webserver.API_Endpoints {
	[EndpointInfo("application/json", "/account")]
	internal partial class Account : APIEndpoint {
		[PermissionLevel(PermLevel.Manager)]
		public override void GET() {
			//Get required fields
			if(!Content.TryGetValue("Email", out JToken Email)) {
				Send("Missing fields", HttpStatusCode.BadRequest);
				return;
			}

			//Check if the specified user exists. If it doesn't, send a 404 Not Found
			User Acc = User.GetUserByEmail(Connection, (string)Email);
			if(Acc == null) {
				Send("No such user", HttpStatusCode.NotFound);
				return;
			}

			//Build and send response
			JObject JSON = JObject.FromObject(Acc);
			JSON.Remove("PasswordHash"); //For security
			Send(JSON.ToString(Formatting.None), HttpStatusCode.OK);
		}
	}
}