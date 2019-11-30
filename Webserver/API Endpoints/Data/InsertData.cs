using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Webserver.Data;

namespace Webserver.API_Endpoints {
	partial class Data : APIEndpoint {

		/// <summary>
		/// Inserts data into the specified table.
		/// </summary>
		[RequireBody]
		[RequireContentType("application/json")]
		public override void POST() {
			//Get required fields
			if (!RequestParams.ContainsKey("table")) {
				Send("Missing params", HttpStatusCode.BadRequest);
				return;
			}

			//Check if all specified table exist
			GenericDataTable Table = GenericDataTable.GetTableByName(Connection, RequestParams["table"][0]);
			if (Table == null) {
				Send("No such table", HttpStatusCode.NotFound);
				return;
			}

			//Build insertion dict
			Dictionary<string, DataType> Columns = Table.GetColumns();
			Dictionary<string, dynamic> Dict = new Dictionary<string, dynamic>();
			foreach(KeyValuePair<string, JToken> Entry in JSON) {
				if (!Columns.ContainsKey(Entry.Key)) {
					Send("No such column: " + Entry.Key);
					return;
				}
				if(Entry.Key == "rowid" ) {
					Send("Can't set row ID", HttpStatusCode.BadRequest);
					return;
				}
				Dict.Add(Entry.Key, Columns[Entry.Key] == DataType.Integer ? (int)Entry.Value : (dynamic)Entry.Value);
			}

			//Insert data and return response
			Table.Insert(Dict);
			Send("Data successfully added", HttpStatusCode.Created);
		}
	}
}
