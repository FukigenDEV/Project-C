using Microsoft.VisualStudio.TestTools.UnitTesting;
using Webserver.API_Endpoints;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Newtonsoft.Json.Linq;
using Webserver.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Webserver.API_Endpoints.Tests {
	public partial class DataEndpoint : APITestMethods {
		/// <summary>
		/// Check if we can properly create a table
		/// </summary>
		[TestMethod]
		public void POST_ValidArguments() {
			CreateTestTable();
			ResponseProvider Response = ExecuteSimpleRequest("/data?table=Table1", HttpMethod.POST, new JObject() {
				{"StringColumn", "SomeText"},
				{"IntegerColumn", 12345 },
				{"Validated", 1 }
			});
			Assert.IsTrue(Response.StatusCode == HttpStatusCode.Created);
			GenericDataTable Table = GenericDataTable.GetTableByName(Connection, "Table1");
			JObject Data = Table.GetRows();
			Assert.IsTrue(((JArray)Data["Rows"][3]).Count == 4);
			Assert.IsTrue((int)Data["Rows"][3][0] == 4);
			Assert.IsTrue((string)Data["Rows"][3][1] == "SomeText");
			Assert.IsTrue((int)Data["Rows"][3][2] == 12345);
			Assert.IsTrue((int)Data["Rows"][3][3] == 1);
		}

		[SuppressMessage("Code Quality", "IDE0051")]
		static IEnumerable<object[]> InvalidPostTestData => new[]{
			//### Misc
			new object[] {
				new JObject() {
					{"StringColumn", "SomeText"},
					{"IntegerColumn", 12345 },
				},
				"/data",
				HttpStatusCode.BadRequest,
				"Missing params",
			},
			new object[] {
				new JObject() {
					{"StringColumn", "SomeText"},
					{"IntegerColumn", 12345 },
				},
				"/data?table=SomeTable",
				HttpStatusCode.NotFound,
				"No such table",
			},
			new object[] {
				new JObject() {
					{"SomeColumn", "SomeText"},
					{"IntegerColumn", 12345 },
				},
				"/data?table=Table1",
				HttpStatusCode.BadRequest,
				"No such column: SomeColumn",
			},
			new object[] {
				new JObject() {
					{"rowid", 12345},
					{"IntegerColumn", 12345 },
				},
				"/data?table=Table1",
				HttpStatusCode.BadRequest,
				"Can't set row ID",
			},
		};

		/// <summary>
		/// Check if we get an error if we specify invalid arguments
		/// </summary>
		[TestMethod]
		[DynamicData("InvalidPostTestData")]
		public void POST_InvalidArguments(JObject JSON, string URL, HttpStatusCode StatusCode, string ResponseMessage = null) {
			CreateTestTable();
			ResponseProvider Response = ExecuteSimpleRequest(URL, HttpMethod.POST, JSON);
			Assert.IsTrue(Response.StatusCode == StatusCode);
			if (ResponseMessage != null) Assert.IsTrue(Encoding.UTF8.GetString(Response.Data) == ResponseMessage);
		}
	}
}