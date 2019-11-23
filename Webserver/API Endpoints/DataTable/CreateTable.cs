using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Webserver.Data;

namespace Webserver.API_Endpoints.DataTable {
	[EndpointURL("/datatable")]
	partial class DataTable : APIEndpoint {
		[RequireBody]
		[RequireContentType("application/json")]
		[PermissionLevel(PermLevel.Manager)]
		public override void POST() {
			//Get all required fields
			if (
				!JSON.TryGetValue<string>("Name", out JToken Name) ||
				!JSON.TryGetValue<JObject>("Columns", out JToken Columns) ||
				!JSON.TryGetValue<string>("Department", out JToken DepartmentVal) ||
				!JSON.TryGetValue<bool>("RequireValidation", out JToken RequireValidation)
			) {
				Send("Missing fields", HttpStatusCode.BadRequest);
				return;
			}

			//Check name
			if(!Regex.IsMatch((string)Name, "[0-9A-Za-z_]")){
				Send("Invalid name", HttpStatusCode.BadRequest);
				return;
			}
			if (GenericDataTable.Exists(Connection, (string)Name)) {
				Send("Already exists", HttpStatusCode.BadRequest);
				return;
			}

			//Check Department
			Department Dept = Department.GetByName(Connection, (string)DepartmentVal);
			if(Dept == null) {
				Send("No such department", HttpStatusCode.BadRequest);
				return;
			}

			//Convert columns
			Dictionary<string, DataType> ColumnDict = new Dictionary<string, DataType>();
			foreach (KeyValuePair<string, JToken> Entry in (JObject)Columns) {
				if(GenericDataTable.ReservedColumns.Contains(Entry.Key) || !Regex.IsMatch(Entry.Key, "[0-9A-Za-z_]")) {
					Send("Invalid or reserved column name");
					return;
				}
				if(!Enum.TryParse<DataType>((string)Entry.Value, out DataType DT)) {
					Send("Invalid column type. Type must be either Integer, String, Real, or Blob");
					return;
				}
				ColumnDict.Add(Entry.Key, DT);
			}

			new GenericDataTable(Connection, (string)Name, ColumnDict, Dept, (bool)RequireValidation);
			Send(StatusCode: HttpStatusCode.Created);
		}
	}
}
