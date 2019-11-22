using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Webserver.Data;
using System.Linq;

namespace Webserver.API_Endpoints {
	partial class Data : APIEndpoint {

		[RequireBody]
		[RequireContentType("application/json")]
		public override void DELETE() {
			//Get required fields
			if (!RequestParams.ContainsKey("table")) {
				Send("Missing params", HttpStatusCode.BadRequest);
				return;
			}
			if(!JSON.TryGetValue<JArray>("RowIDs", out JToken RowIDs)) {
				Send("Missing fields", HttpStatusCode.BadRequest);
				return;
			}

			//Check if all specified table exist
			GenericDataTable Table = GenericDataTable.GetTableByName(Connection, RequestParams["table"][0]);
			if (Table == null) {
				Send("No such table", HttpStatusCode.NotFound);
				return;
			}

			List<int> IDs = (((JArray)RowIDs).Where(ID => ID.Type == JTokenType.Integer).Select(ID => (int)ID)).ToList();
			Table.Delete(IDs);
			Send(HttpStatusCode.OK);
		}
	}
}
