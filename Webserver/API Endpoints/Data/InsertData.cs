using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Webserver.Data;

namespace Webserver.API_Endpoints {
	partial class Data : APIEndpoint {

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
			List<string> Columns = Table.GetColumns().Keys.ToList();
			Dictionary<string, dynamic> Dict = new Dictionary<string, dynamic>();
			foreach(KeyValuePair<string, JToken> Entry in JSON) {
				if (!Columns.Contains(Entry.Key)) {
					Send("No such column: " + Entry.Key);
					return;
				}
				Dict.Add(Entry.Key, Entry.Value);
			}

			//Insert data and return response
			Table.Insert(Dict);
			Send(HttpStatusCode.Created);
		}
	}
}
