using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Collections.Generic;
using System.Text;
using Webserver.Data;

namespace Webserver.API_Endpoints
{
    [EndpointURL("/department")]
    internal partial class DepartmentEndPoint : APIEndpoint
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

            if (RequestParams["name"][0] == "all")
            {
                List<Department> departments = Department.GetAllDepartments(Connection);
                Send(JsonConvert.SerializeObject(departments), HttpStatusCode.OK);

                return;
            }

            // Check if the specified department exists. If it doesn't, send a 404 Not Found
            Department department = Department.GetDepartmentByName(Connection, RequestParams["name"][0]);
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
