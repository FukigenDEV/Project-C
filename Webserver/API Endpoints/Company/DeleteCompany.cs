using Dapper.Contrib.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Webserver.Data;

namespace Webserver.API_Endpoints
{
    internal partial class CompanyEndpoint : APIEndpoint
    {
        public override void DELETE()
        {
            // Get required fields
            if (!RequestParams.ContainsKey("name"))
            {
                Send("Missing params", HttpStatusCode.BadRequest);
                return;
            }

            //Check if the specified company exists. If it doesn't, send a 404 Not Found
            Company company = Company.GetCompanyByName(Connection, RequestParams["name"][0]);
            if (company == null)
            {
                Send("No such company", HttpStatusCode.NotFound);
                return;
            }

            Connection.Delete(company);
            Send("Company successfully deleted", StatusCode: HttpStatusCode.OK);
        }
    }
}
