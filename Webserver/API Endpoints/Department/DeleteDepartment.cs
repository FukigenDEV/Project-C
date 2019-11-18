using Dapper.Contrib.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Webserver.Data;

namespace Webserver.API_Endpoints
{
    internal partial class DepartmentEndPoint : APIEndpoint
    {
		[RequireContentType("application/json")]
		[RequireBody]
		public override void DELETE()
        {
            // Get required fields
            if (!JSON.TryGetValue<string>("name", out JToken name))
            {
                Send("Missing fields", HttpStatusCode.BadRequest);
                return;
            }

			//Don't allow users to delete the Administrators and All Users departments.
			if((string) name == "Administrators" || (string)name == "All Users") {
				Send("Cannot delete system department", HttpStatusCode.Forbidden);
				return;
			}

            //Check if the specified department exists. If it doesn't, send a 404 Not Found
            Department department = Department.GetDepartmentByName(Connection, (string)name);
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
