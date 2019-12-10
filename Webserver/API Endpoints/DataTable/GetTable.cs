using Dapper;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Webserver.Data;

namespace Webserver.API_Endpoints {
    partial class DataTable : APIEndpoint {
        [RequireContentType("application/json")]
        [PermissionLevel(PermLevel.User)]
        public override void GET() {
            // If there's a table name, send the table by name and return
            if (Params.ContainsKey("name"))
            {
                // The requested table
                GenericDataTable table = GenericDataTable.GetTableByName(Connection, Params["name"][0]);

                if (table == null) {
                    Response.Send("No such table", HttpStatusCode.NotFound);
                } else {
                    JObject JSON = (JObject)JToken.FromObject(table);
                    JSON["Department"] = Connection.Get<Department>((int)JSON["Department"]).Name;

                    Response.Send(JSON.ToString(), HttpStatusCode.OK);
                }

                return;
            }

            int departmentID = 0;

            // If there's a department, set departmentID to its ID
            if (Params.ContainsKey("department")) {
                Department department = Department.GetByName(Connection, Params["department"][0]);

                // Check if the specified department exists. If it doesn't, send a 404 Not Found
                if (department == null) {
                    Response.Send("No such department", HttpStatusCode.NotFound);
                    return;
                }

                departmentID = department.ID;
            }

            // Get the tableNames of the specified department
            List<string> tableNames = GenericDataTable.GetTableNames(Connection, departmentID);

            // Fill the list of tables with the table that belongs to the tableName
            List<GenericDataTable> tables = new List<GenericDataTable>();
            foreach (string name in tableNames) {
                tables.Add(GenericDataTable.GetTableByName(Connection, name));
            }

            // Convert tables into JSON string and send it.
            Response.Send(JsonConvert.SerializeObject(tables), HttpStatusCode.OK);
        }
    }
}
