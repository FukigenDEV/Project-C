﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Webserver.Data;

namespace Webserver.API_Endpoints.DataTable {
	partial class DataTable : APIEndpoint {
		[RequireBody]
		[RequireContentType("application/json")]
		[PermissionLevel(PermLevel.Manager)]
		public override void DELETE() {
			// Get all required values
			if (!JSON.TryGetValue<string>("Name", out JToken name)) {
				Send("Missing fields", HttpStatusCode.BadRequest);
				return;
			}

			//If table exists, delete it
			GenericDataTable Table = GenericDataTable.GetTableByName(Connection, (string)name);
			if(Table == null) {
				Send("No such table", HttpStatusCode.BadRequest);
				return;
			} else {
				Table.DropTable();
				Send(StatusCode: HttpStatusCode.OK);
			}
		}
	}
}
