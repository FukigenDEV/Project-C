using Dapper.Contrib.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Collections.Generic;
using System.Text;
using Webserver.Data;

namespace Webserver.API_Endpoints
{
    internal partial class CompanyEndPoint : APIEndpoint
    {
        public override void POST()
        {
			// Get all required values
            if (!Content.TryGetValue<string>("name", out JToken name) ||
                !Content.TryGetValue<string>("street", out JToken street) ||
                !Content.TryGetValue<int>("houseNumber", out JToken houseNumber) ||
                !Content.TryGetValue<string>("postCode", out JToken postCode) ||
                !Content.TryGetValue<string>("city", out JToken city) ||
                !Content.TryGetValue<string>("country", out JToken country) ||
                !Content.TryGetValue<string>("phoneNumber", out JToken phoneNumber) ||
                !Content.TryGetValue<string>("email", out JToken email))
            {
                Send("Missing fields", HttpStatusCode.BadRequest);
                return;
            }

            Company company = new Company((string)name, (string)street, (int)houseNumber, (string)postCode, (string)city, (string)country, (string)phoneNumber, (string)email);

            // Store companty to database
            Connection.Insert(company);

            // Send success message
            Send("Company successfully created", HttpStatusCode.OK);
        }
    }
}
