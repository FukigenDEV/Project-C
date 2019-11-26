﻿using Dapper.Contrib.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Webserver.Data;

namespace Webserver.API_Endpoints
{
    internal partial class CompanyEndPoint : APIEndpoint
    {
        public override void DELETE()
        {
            // Get required fields
            if (!Content.TryGetValue<string>("name", out JToken name))
            {
                Send("Missing fields", HttpStatusCode.BadRequest);
                return;
            }

            //Check if the specified company exists. If it doesn't, send a 404 Not Found
            Company company = Company.GetCompanyByName(Connection, (string)name);
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
