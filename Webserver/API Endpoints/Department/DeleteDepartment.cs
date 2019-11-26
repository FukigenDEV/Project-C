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
		public override void DELETE()
        {
			// Get required fields
			if (!RequestParams.ContainsKey("name")) {
				Send("Missing params", HttpStatusCode.BadRequest);
				return;
			}
			string Name = RequestParams["name"][0];

			//Don't allow users to delete the Administrators and All Users departments.
			if (Name == "Administrators" || Name == "All Users") {
				Send("Cannot delete system department", HttpStatusCode.Forbidden);
				return;
			}

            //Check if the specified department exists. If it doesn't, send a 404 Not Found
            Department department = Department.GetByName(Connection, Name);
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
