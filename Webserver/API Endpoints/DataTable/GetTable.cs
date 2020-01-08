using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Webserver.Data;

namespace Webserver.API_Endpoints {
	internal partial class DataTable : APIEndpoint {
		[PermissionLevel(PermLevel.User)]
		public override void GET() {
			int departmentID = 0;

			// If there's a department, set departmentID to its ID
			if ( Params.ContainsKey("department") ) {
				Department department = Department.GetByName(Connection, Params["department"][0]);

				// Check if the specified department exists. If it doesn't, send a 404 Not Found
				if ( department == null ) {
					Response.Send("No such department", HttpStatusCode.NotFound);
					return;
				}

				departmentID = department.ID;
			}

			// Retrieve tables, convert to JSON, and send.
			Response.Send(JsonConvert.SerializeObject(GenericDataTable.GetTables(Connection, departmentID)), HttpStatusCode.OK);
		}
	}
}
