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
		/// Deletes data from the specified table.
		/// </summary>
		[RequireBody]
		[RequireContentType("application/json")]
		[PermissionLevel(PermLevel.User)]
		public override void DELETE() {
			//Get required fields
			if ( !Params.ContainsKey("table") ) {
				Response.Send("Missing params", HttpStatusCode.BadRequest);
				return;
			}
			if ( !JSON.TryGetValue<JArray>("RowIDs", out JToken RowIDs) ) {
				Response.Send("Missing fields", HttpStatusCode.BadRequest);
				return;
			}

			//Check if all specified table exist
			GenericDataTable Table = GenericDataTable.GetTableByName(Connection, Params["table"][0]);
			if ( Table == null ) {
				Response.Send("No such table", HttpStatusCode.NotFound);
				return;
			}

			List<int> IDs = ( ( (JArray)RowIDs ).Where(ID => ID.Type == JTokenType.Integer).Select(ID => (int)ID) ).ToList();
			Table.Delete(IDs);
			Response.Send(HttpStatusCode.OK);
		}
	}
}
