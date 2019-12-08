using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Webserver.Data;

namespace Webserver.API_Endpoints {
	[EndpointURL("/data")]
	internal partial class Data : APIEndpoint {

		[RequireContentType("application/json")]
		[PermissionLevel(PermLevel.User)]
		public override void GET() {
			//Get required fields
			if ( !Params.ContainsKey("table") ) {
				Response.Send("Missing params", HttpStatusCode.BadRequest);
				return;
			}

			//If any optional parameters were given, overwrite the default values with those specified by the parameters
			int Begin = 0;
			if ( Params.ContainsKey("begin") ) {
				int.TryParse(Params["begin"][0], out Begin);
			}
			int End = 25;
			if ( Params.ContainsKey("end") ) {
				int.TryParse(Params["end"][0], out End);
			}
			bool isUnvalidated = false;
			if ( Params.ContainsKey("isvalidated") ) {
				bool.TryParse(Params["isvalidated"][0], out isUnvalidated);
			}

			//Check if all specified tables exist
			if ( !GenericDataTable.Exists(Connection, Params["table"]) ) {
				Response.Send("No such table", HttpStatusCode.NotFound);
				return;
			}

			//Create response JSON
			JObject Result = new JObject();
			foreach ( string TableName in Params["table"] ) {
				GenericDataTable Table = GenericDataTable.GetTableByName(Connection, TableName);
				Result.Add(TableName, isUnvalidated ? Table.GetUnvalidatedRows(Begin, End) : Table.GetRows(Begin, End));
			}

			Response.Send(Result, HttpStatusCode.OK);
		}
	}
}
