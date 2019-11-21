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
		[RequireContentType("application/json")]
		[RequireBody]
		public override void POST()
        {
            // Get all required values
            if (!JSON.TryGetValue<string>("name", out JToken name) ||
                !JSON.TryGetValue<string>("description", out JToken description))
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
			Send(StatusCode: HttpStatusCode.Created);
		}
	}
}
