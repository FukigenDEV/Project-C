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
		/// Check if we can properly modify a table
		/// </summary>
		[TestMethod]
		public void PATCH_ValidArguments() {
			CreateTestTable();
			ResponseProvider Response = ExecuteSimpleRequest("/data?table=Table1", HttpMethod.PATCH, new JObject() {
				{"1", new JObject() {
					{"StringColumn", "Hello World!" },
					{"IntegerColumn", 12345 }
				}}
			});

			Assert.IsTrue(Response.StatusCode == HttpStatusCode.OK);

			GenericDataTable Table = GenericDataTable.GetTableByName(Connection, "Table1");
			JArray Expected = new JArray() { 1, "Hello World!", 12345, 0 };
			JArray Actual = (JArray)Table.GetRows()["Rows"][0];
			Assert.IsTrue(JArray.DeepEquals(Expected, JArray.Parse(Actual.ToString())));
			Expected = new JArray() { 2, "Text2", 2, 0 };
			Actual = (JArray)Table.GetRows()["Rows"][1];
			Assert.IsTrue(JArray.DeepEquals(Expected, JArray.Parse(Actual.ToString())));
		}

		public void PATCH_NoArguments() {
			ResponseProvider Response = ExecuteSimpleRequest("/data?table=Table1", HttpMethod.PATCH);
			Assert.IsTrue(Response.StatusCode == HttpStatusCode.OK);
		}

		[SuppressMessage("Code Quality", "IDE0051")]
		static IEnumerable<object[]> InvalidPatchTestData => new[]{
			new object[] {
				new JObject() {
					{"1", new JObject() {
						{"StringColumn", "Hello World!" },
						{"IntegerColumn", 12345 }
					}}
				},
				"/Data",
				HttpStatusCode.BadRequest,
				"Missing params",
			},
			new object[] {
				new JObject() {
					{"1", new JObject() {
						{"StringColumn", "Hello World!" },
						{"IntegerColumn", 12345 }
					}}
				},
				"/Data?table=SomeTable",
				HttpStatusCode.NotFound,
				"No such table",
			},
			new object[] {
				new JObject() {
					{"text", new JObject() {
						{"StringColumn", "Hello World!" },
						{"IntegerColumn", 12345 }
					}}
				},
				"/Data?table=Table1",
				HttpStatusCode.BadRequest,
				"Invalid row ID",
			},
			new object[] {
				new JObject() {
					{"1", "Hello World" }
				},
				"/Data?table=Table1",
				HttpStatusCode.BadRequest,
				"Invalid row data",
			},
			new object[] {
				new JObject() {
					{"1", new JObject() {
						{"StringColumn", "Hello World!" },
						{"rowid", 12345 }
					}}
				},
				"/Data?table=Table1",
				HttpStatusCode.BadRequest,
				"Can't modify row ID",
			},
			new object[] {
				new JObject() {
					{"1", new JObject() {
						{"StringColumn", "Hello World!" },
						{"IntegerColumn", "Foobar!" }
					}}
				},
				"/Data?table=Table1",
				HttpStatusCode.BadRequest,
				"Invalid datatype (IntegerColumn should be Integer)",
			},
			new object[] {
				new JObject() {
					{"1", new JObject() {
						{"SomeColumn", "Hello World!" },
						{"IntegerColumn", 12345 }
					}}
				},
				"/Data?table=Table1",
				HttpStatusCode.BadRequest,
				"Unknown column SomeColumn",
			},
		};

		/// <summary>
		/// Check if we get an error if we specify invalid arguments
		/// </summary>
		[TestMethod]
		[DynamicData("InvalidPatchTestData")]
		public void PATCH_InvalidArguments(JObject JSON, string URL, HttpStatusCode StatusCode, string ResponseMessage = null) {
			CreateTestTable();
			ResponseProvider Response = ExecuteSimpleRequest(URL, HttpMethod.PATCH, JSON);
			Assert.IsTrue(Response.StatusCode == StatusCode);
			string Message = Encoding.UTF8.GetString(Response.Data);
			if (ResponseMessage != null) Assert.IsTrue(Message == ResponseMessage);
		}
	}
}