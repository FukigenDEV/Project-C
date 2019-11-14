﻿using Dapper.Contrib.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Webserver.Data;

namespace Webserver.API_Endpoints
{
    internal partial class DepartmentEndPoint : APIEndpoint
    {
		[RequireContentType("application/json")]
		[RequireBody]
		public override void PATCH()
        {
            // Get required fields
            if (!JSON.TryGetValue<string>("name", out JToken name))
            {
                Send("Missing fields", HttpStatusCode.BadRequest);
                return;
            }

            // Check if the specified department exists. If it doesn't, send a 404 Not Found
            Department department = Department.GetDepartmentByName(Connection, (string)name);
            if (department == null)
            {
                Send("No such department", HttpStatusCode.NotFound);
                return;
            }

            // Change name if necessary
            if (JSON.TryGetValue<string>("newName", out JToken newName))
            {
                department.Name = (string)newName;
            }

            // Change description if necessary
            if (JSON.TryGetValue<string>("newDescription", out JToken newDescription))
            {
                department.Description = (string)newDescription;
            }

            // Update DB row
            Connection.Update<Department>(department);
            Send("Department has successfully been edited.", StatusCode: HttpStatusCode.OK);
        }
    }
}
