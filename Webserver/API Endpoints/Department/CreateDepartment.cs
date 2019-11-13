using Dapper.Contrib.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Collections.Generic;
using System.Text;
using Webserver.Data;

namespace Webserver.API_Endpoints
{
    internal partial class DepartmentEndPoint : APIEndpoint
    {
        public override void POST()
        {
            // Get all required values
            if (!Content.TryGetValue<string>("name", out JToken name) ||
                !Content.TryGetValue<string>("description", out JToken description))
            {
                Send("Missing fields", HttpStatusCode.BadRequest);
                return;
            }

			//Check if the specified department exists. If it doesn't, send a 404 Not Found
			Department department = Department.GetDepartmentByName(Connection, (string)name);
			if (department != null) {
				Send("Department already exists", HttpStatusCode.BadRequest);
				return;
			}

			Department newDepartment = new Department((string)name, (string)description);

            // Store department to database
            Connection.Insert(newDepartment);

            // Send success message
            Send("Department successfully created", HttpStatusCode.OK);
        }
    }
}
