﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Webserver.Data;

namespace Webserver.API_Endpoints {
	[EndpointURL("/datatable")]
	internal partial class DataTable : APIEndpoint {
		[RequireBody]
		[RequireContentType("application/json")]
		[PermissionLevel(PermLevel.Manager)]
		public override void POST() {
			const string RX = "^[A-z]{1}[0-9A-Za-z_]*$";

			//Get all required fields
			if (
				!JSON.TryGetValue<string>("Name", out JToken Name) ||
				!JSON.TryGetValue<JObject>("Columns", out JToken Columns) ||
				!JSON.TryGetValue<string>("Department", out JToken DepartmentVal) ||
				!JSON.TryGetValue<bool>("RequireValidation", out JToken RequireValidation)
			) {
				Response.Send("Missing fields", HttpStatusCode.BadRequest);
				return;
			}

			//Check name
			if ( !Regex.IsMatch((string)Name, RX) ) {
				Response.Send("Invalid name", HttpStatusCode.BadRequest);
				return;
			}
			if ( GenericDataTable.Exists(Connection, (string)Name) ) {
				Response.Send("Already exists", HttpStatusCode.BadRequest);
				return;
			}

			//Check Department
			Department Dept = Department.GetByName(Connection, (string)DepartmentVal);
			if ( Dept == null ) {
				Response.Send("No such department", HttpStatusCode.BadRequest);
				return;
			}

			//Convert columns
			Dictionary<string, DataType> ColumnDict = new Dictionary<string, DataType>();
			foreach ( KeyValuePair<string, JToken> Entry in (JObject)Columns ) {
				if ( GenericDataTable.ReservedColumns.Contains(Entry.Key) || !Regex.IsMatch(Entry.Key, RX) ) {
					Response.Send("Invalid or reserved column name", HttpStatusCode.BadRequest);
					return;
				}
				if ( !Enum.TryParse((string)Entry.Value, out DataType DT) ) {
					Response.Send("Invalid column type. Type must be either Integer, String, Real, or Blob", HttpStatusCode.BadRequest);
					return;
				}
				ColumnDict.Add(Entry.Key, DT);
			}

			new GenericDataTable(Connection, (string)Name, ColumnDict, Dept, (bool)RequireValidation);
			Response.Send(HttpStatusCode.Created);
		}
	}
}
