using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using Webserver.Data;

namespace Webserver.API_Endpoints {
	internal partial class DataTable : APIEndpoint {
		[PermissionLevel(PermLevel.Manager)]
		public override void DELETE() {
			// Get all required values
			if ( !Params.ContainsKey("table") ) {
				Response.Send("Missing fields", HttpStatusCode.BadRequest);
				return;
			}

			//If table exists, delete it
			GenericDataTable Table = GenericDataTable.GetTableByName(Connection, Params["table"][0]);
			if ( Table == null ) {
				Response.Send("No such table", HttpStatusCode.BadRequest);
				return;
			} else {
				Table.DropTable();
				Response.Send(StatusCode: HttpStatusCode.OK);
			}
		}
	}
}
