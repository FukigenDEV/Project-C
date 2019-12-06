using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json.Linq;
using Webserver.Data;

namespace Webserver.API_Endpoints {
	[EndpointURL("/company")]
	internal partial class CompanyEndpoint : APIEndpoint {
		[RequireContentType("application/json")]
		[RequireBody]
		[PermissionLevel(PermLevel.Manager)]
		public override void POST() {
			// Get all required values
			if ( !JSON.TryGetValue<string>("name", out JToken name) ||
				!JSON.TryGetValue<string>("street", out JToken street) ||
				!JSON.TryGetValue<int>("houseNumber", out JToken houseNumber) ||
				!JSON.TryGetValue<string>("postCode", out JToken postCode) ||
				!JSON.TryGetValue<string>("city", out JToken city) ||
				!JSON.TryGetValue<string>("country", out JToken country) ||
				!JSON.TryGetValue<string>("phoneNumber", out JToken phoneNumber) ||
				!JSON.TryGetValue<string>("email", out JToken email) ) {
				Response.Send("Missing fields", HttpStatusCode.BadRequest);
				return;
			}

			Company company = new Company((string)name, (string)street, (int)houseNumber, (string)postCode, (string)city, (string)country, (string)phoneNumber, (string)email);

			// Store companty to database
			Connection.Insert(company);

			// Send success message
			Response.Send(StatusCode: HttpStatusCode.Created);
		}
	}
}
