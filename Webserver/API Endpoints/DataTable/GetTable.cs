using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Webserver.Data;

namespace Webserver.API_Endpoints {
	internal partial class DataTable : APIEndpoint {
		[RequireContentType("application/json")]
		[PermissionLevel(PermLevel.User)]
		public override void GET() {
			int departmentID = 0;

			// If there's a department, set departmentID to its ID
			if ( Params.ContainsKey("Department") ) {
				Department department = Department.GetByName(Connection, Params["Department"][0]);

				// Check if the specified department exists. If it doesn't, send a 404 Not Found
				if ( department == null ) {
					Response.Send("No such department", HttpStatusCode.NotFound);
					return;
				}

				departmentID = department.ID;
			}

			// Get the tableNames of the specified department
			List<string> tableNames = GenericDataTable.GetTableNames(Connection, departmentID);

			// Fill the list of tables with the table that belongs to the tableName
			List<GenericDataTable> tables = new List<GenericDataTable>();
			foreach ( string name in tableNames ) {
				tables.Add(GenericDataTable.GetTableByName(Connection, name));
			}

			// Convert tables into JSON string and send it.
			Response.Send(JsonConvert.SerializeObject(tables), HttpStatusCode.OK);
		}
	}
}
