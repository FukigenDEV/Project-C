using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Webserver.Data;

namespace Webserver.API_Endpoints {
	[EndpointURL("/data")]
	partial class Data : APIEndpoint {

		[RequireContentType("application/json")]
		[PermissionLevel(PermLevel.User)]
		public override void GET() {
			//Get required fields
			if (!RequestParams.ContainsKey("table")) {
				Send("Missing params", HttpStatusCode.BadRequest);
				return;
			}
			int Begin = 0;
			if (RequestParams.ContainsKey("begin")) {
				int.TryParse(RequestParams["begin"][0], out Begin);
			}
			int End = 25;
			if (RequestParams.ContainsKey("end")) {
				int.TryParse(RequestParams["end"][0], out End);
			}
			bool isUnvalidated = false;
			if (RequestParams.ContainsKey("isvalidated")){
				bool.TryParse(RequestParams["isvalidated"][0], out isUnvalidated);
			}

			//Check if all specified tables exist
			if (!GenericDataTable.Exists(Connection, RequestParams["table"])) {
				Send("No such table", HttpStatusCode.NotFound);
				return;
			}

			//Create response JSON
			JObject Response = new JObject();
			foreach(string TableName in RequestParams["table"]) {
				GenericDataTable Table = GenericDataTable.GetTableByName(Connection, TableName);
				Response.Add(TableName, isUnvalidated? Table.GetUnvalidatedRows(Begin, End) : Table.GetRows(Begin, End));
			}

			Send(Response, HttpStatusCode.OK);
		}
	}
}
