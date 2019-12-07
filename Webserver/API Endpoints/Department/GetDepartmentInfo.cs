using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Webserver.Data;

namespace Webserver.API_Endpoints {
	[EndpointURL("/department")]
	internal partial class DepartmentEndPoint : APIEndpoint {
		public override void GET() {
			// Get required fields
			if ( !Params.ContainsKey("name") ) {
				Response.Send("Missing params", HttpStatusCode.BadRequest);
				return;
			}

			if ( Params["name"][0].Length == 0 ) {
				List<Department> departments = Department.GetAllDepartments(Connection);
				Response.Send(JsonConvert.SerializeObject(departments), HttpStatusCode.OK);
				return;
			}

			// Check if the specified department exists. If it doesn't, send a 404 Not Found
			Department department = Department.GetByName(Connection, Params["name"][0]);
			if ( department == null ) {
				Response.Send("No such department", HttpStatusCode.NotFound);
				return;
			}

			// Build and send response
			JObject JSON = JObject.FromObject(department);
			Response.Send(JSON, HttpStatusCode.OK);
		}
	}
}
