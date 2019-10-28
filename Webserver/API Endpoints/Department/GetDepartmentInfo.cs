using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Collections.Generic;
using System.Text;

namespace Webserver.API_Endpoints
{
    [EndpointInfo("application/json", "/department")]
    internal partial class Department : APIEndpoint
    {
        public override void GET()
        {
            // Get required fields
            if (!RequestParams.ContainsKey("name"))
            {
                Send("Missing fields", HttpStatusCode.BadRequest);
                return;
            }

            //Check if the specified department exists. If it doesn't, send a 404 Not Found
            Data.Department department = Data.Department.GetDepartmentByName(Connection, RequestParams["name"][0]);
            if (department == null)
            {
                Send("No such department", HttpStatusCode.NotFound);
                return;
            }

            // Build and send response
            JObject JSON = JObject.FromObject(department);
            Send(JSON.ToString(Formatting.None), HttpStatusCode.OK);
        }
    }
}
