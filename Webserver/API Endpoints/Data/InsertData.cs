using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using Webserver.Data;

namespace Webserver.API_Endpoints {
	internal partial class Data : APIEndpoint {

		/// <summary>
		/// Inserts data into the specified table.
		/// </summary>
		[RequireBody]
		[RequireContentType("application/json")]
		public override void POST() {
			//Get required fields
			if ( !Params.ContainsKey("table") ) {
				Response.Send("Missing params", HttpStatusCode.BadRequest);
				return;
			}

			//Check if all specified table exist
			GenericDataTable Table = GenericDataTable.GetTableByName(Connection, Params["table"][0]);
			if ( Table == null ) {
				Response.Send("No such table", HttpStatusCode.NotFound);
				return;
			}

			//Build insertion dict
			Dictionary<string, DataType> Columns = Table.GetColumns();
			Dictionary<string, dynamic> Dict = new Dictionary<string, dynamic>();
			foreach ( KeyValuePair<string, JToken> Entry in JSON ) {
				if ( !Columns.ContainsKey(Entry.Key) ) {
					Response.Send("No such column: " + Entry.Key, HttpStatusCode.BadRequest);
					return;
				}
				if ( Entry.Key == "rowid" ) {
					Response.Send("Can't set row ID", HttpStatusCode.BadRequest);
					return;
				}
				if (Columns[Entry.Key] == DataType.Integer) {
					try {
						int input = (int)Entry.Value;
					} catch {
						Response.Send("Invalid row value at " + Entry.Key, HttpStatusCode.BadRequest);
						return;
					}
				}
				Dict.Add(Entry.Key, Columns[Entry.Key] == DataType.Integer ? (int)Entry.Value : (dynamic)Entry.Value);
			}

			//Insert data and return response
			Table.Insert(Dict);
			Response.Send(HttpStatusCode.Created);
		}
	}
}
