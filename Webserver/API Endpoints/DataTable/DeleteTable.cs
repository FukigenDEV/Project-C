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
			if ( !RequestParams.ContainsKey("table") ) {
				Send("Missing fields", HttpStatusCode.BadRequest);
				return;
			}

			//If table exists, delete it
			GenericDataTable Table = GenericDataTable.GetTableByName(Connection, RequestParams["table"][0]);
			if ( Table == null ) {
				Send("No such table", HttpStatusCode.BadRequest);
				return;
			} else {
				Table.DropTable();
				Send(StatusCode: HttpStatusCode.OK);
			}
		}
	}
}
