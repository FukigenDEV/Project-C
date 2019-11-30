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
            // Checkt of de waardes omgezet kunnen worden naar een string, hij gaat alleen door als dit bij allebei zo is, ander gelijk naar de else
            if (JSON.TryGetValue<string>("name", out JToken name) &&
                JSON.TryGetValue<string>("description", out JToken description))
            {
                //Check of allebei de ingevulde velden minstens 1 karakter lang zijn, anders is er niks ingevuld
                if (((string)name).Length < 1 || ((string)description).Length < 1)
                {
                    Send("Please fill in all fields", HttpStatusCode.BadRequest);
                    return;
                }
            }
            else
            {
                Send("Expected a string", HttpStatusCode.BadRequest);
                return;
            }

			//Check if the specified department exists. If it doesn't, send a 404 Not Found
			Department department = Department.GetByName(Connection, (string)name);
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
