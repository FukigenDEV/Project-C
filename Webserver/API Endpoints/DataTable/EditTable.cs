using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Webserver.Data;

namespace Webserver.API_Endpoints.DataTable {
	partial class DataTable : APIEndpoint {
		[RequireBody]
		[RequireContentType("application/json")]
		[PermissionLevel(PermLevel.Manager)]
		public override void PATCH() {
			// Get all required values
			if (!RequestParams.ContainsKey("table")) {
				Send("Missing params", HttpStatusCode.BadRequest);
				return;
			}

			bool FoundAdd = JSON.TryGetValue<JObject>("Add", out JToken AddValue);
			bool FoundDelete = JSON.TryGetValue<JArray>("Delete", out JToken DeleteValue);
			bool FoundRename = JSON.TryGetValue<JObject>("Rename", out JToken RenameValue);
			if(!FoundAdd && !FoundDelete && !FoundRename) {
				Send("Missing fields", HttpStatusCode.BadRequest);
				return;
			}

			//Check if table exists
			GenericDataTable Table = GenericDataTable.GetTableByName(Connection, RequestParams["table"][0]);
			if(Table == null) {
				Send("No such table", HttpStatusCode.NotFound);
				return;
			}

			Dictionary<string, DataType> ToAdd = new Dictionary<string, DataType>();
			List<string> ToDelete = new List<string>();
			Dictionary<string, string> ToRename = new Dictionary<string, string>();

			Dictionary<string, DataType> Columns = Table.GetColumns();

			//Add all specified columns, if any;
			if (FoundAdd) {
				JObject Add = (JObject)AddValue;
				foreach(KeyValuePair<string, JToken> Entry in Add) {
					if(Entry.Value.Type != JTokenType.String || Columns.ContainsKey((string)Entry.Value) || !Regex.IsMatch((string)Entry.Value, GenericDataTable.RX)) {
						Send("Invalid entry in Add", HttpStatusCode.BadRequest);
						return;
					}
					if(!Enum.TryParse((string)Entry.Value, out DataType DT)) {
						Send("Invalid type", HttpStatusCode.BadRequest);
						return;
					}
					ToAdd.Add(Entry.Key, DT);
				}
			}

			//Delete all specified columns, if any;
			if(FoundDelete) {
				JArray Delete = (JArray)DeleteValue;
				foreach(JToken Entry in Delete) {
					if(Entry.Type != JTokenType.String || !Columns.ContainsKey((string)Entry)) {
						Send("Invalid entry in Delete", HttpStatusCode.BadRequest);
						return;
					}
					ToDelete.Add((string)Entry);
				}
			}

			//Rename all specified columns, if any;
			if(FoundRename) {
				JObject Rename = (JObject)RenameValue;
				foreach (KeyValuePair<string, JToken> Entry in Rename) {
					if (Entry.Value.Type != JTokenType.String || Columns.ContainsKey((string)Entry.Value) || !Columns.ContainsKey(Entry.Key) || !Regex.IsMatch((string)Entry.Value, GenericDataTable.RX)) {
						Send("Invalid entry in Rename", HttpStatusCode.BadRequest);
						return;
					}
					ToRename.Add(Entry.Key, (string)Entry.Value);
				}
			}

			if (ToAdd.Count > 0) Table.AddColumn(ToAdd);
			if (ToDelete.Count > 0) Table.DropColumn(ToDelete);
			if (ToRename.Count > 0) Table.RenameColumn(ToRename);
			Send(HttpStatusCode.OK);
		}
	}
}
