using Dapper.Contrib.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Webserver.API_Endpoints
{
    internal partial class Department : APIEndpoint
    {
        public override void DELETE()
        {
            // Get required fields
            if (!Content.TryGetValue<string>("name", out JToken name))
            {
                Send("Missing fields", HttpStatusCode.BadRequest);
                return;
            }

            //Check if the specified department exists. If it doesn't, send a 404 Not Found
            Data.Department department = Data.Department.GetDepartmentByName(Connection, (string)name);
            if (department == null)
            {
                Send("No such department", HttpStatusCode.NotFound);
                return;
            }

            Connection.Delete(department);
            Send("Department successfully deleted", StatusCode: HttpStatusCode.OK);
        }
    }
}
