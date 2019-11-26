using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Collections.Generic;
using System.Text;
using Webserver.Data;

namespace Webserver.API_Endpoints
{
    [EndpointURL("/company")]
    internal partial class CompanyEndpoint : APIEndpoint
    {
        [RequireContentType("application/json")]
        public override void GET()
        {
            // Get required fields
            if (!RequestParams.ContainsKey("name"))
            {
                Send("Missing fields", HttpStatusCode.BadRequest);
                return;
            }

            if (string.IsNullOrEmpty(RequestParams["name"][0]))
            {
                List<Company> companies = Company.GetAllCompanies(Connection);
                Send(JsonConvert.SerializeObject(companies), HttpStatusCode.OK);

                return;
            }

            // Check if the specified company exists. If it doesn't, send a 404 Not Found
            Company company = Company.GetCompanyByName(Connection, RequestParams["name"][0]);
            if (company == null)
            {
                Send("No such company", HttpStatusCode.NotFound);
                return;
            }

            // Build and send response
            JObject JSON = JObject.FromObject(company);
            Send(JSON.ToString(Formatting.None), HttpStatusCode.OK);
        }
    }
}
