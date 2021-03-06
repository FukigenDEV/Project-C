﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using Webserver.Data;

namespace Webserver.API_Endpoints {
	internal partial class Data : APIEndpoint {

		/// <summary>
		/// Updates data in the specified table.
		/// </summary>
		[RequireBody]
		[RequireContentType("application/json")]
		public override void PATCH() {
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

			//Convert JSON to update dict
			Dictionary<int, Dictionary<string, dynamic>> Dict = new Dictionary<int, Dictionary<string, dynamic>>();
			Dictionary<string, DataType> Columns = Table.GetColumns();
			foreach ( KeyValuePair<string, JToken> Entry in JSON ) {
				if ( !int.TryParse(Entry.Key, out int RowID) ) {
					Response.Send("Invalid row ID", HttpStatusCode.BadRequest);
					return;
				}
				if ( Entry.Value.Type != JTokenType.Object ) {
					Response.Send("Invalid row data", HttpStatusCode.BadRequest);
					return;
				}
				Dict.Add(RowID, new Dictionary<string, dynamic>());
				foreach ( KeyValuePair<string, JToken> Column in (JObject)Entry.Value ) {
					if ( Columns.ContainsKey(Column.Key) ) {
						if ( Column.Key == "rowid" ) {
							Response.Send("Can't modify row ID", HttpStatusCode.BadRequest);
							return;
						}
						if ( Columns[Column.Key] == DataType.Integer && Column.Value.Type != JTokenType.Integer ) {
							Response.Send("Invalid datatype (" + Column.Key + " should be Integer)", HttpStatusCode.BadRequest);
							return;
						}
						Dict[RowID].Add(Column.Key, Columns[Column.Key] == DataType.Integer ? (int)Column.Value : (dynamic)Column.Value);
					} else {
						Response.Send("Unknown column " + Column.Key, HttpStatusCode.BadRequest);
						return;
					}
				}
			}
			Table.Update(Dict);
			Response.Send(HttpStatusCode.OK);
		}
	}
}
