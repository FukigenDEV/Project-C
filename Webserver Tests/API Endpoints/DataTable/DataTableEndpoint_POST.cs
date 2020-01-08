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
	public partial class DataTableEndpoint : APITestMethods {
		/// <summary>
		/// Check if we can properly create a table
		/// </summary>
		[TestMethod]
		public void CREATE_ValidArguments() {
			ResponseProvider Response = ExecuteSimpleRequest("/DataTable", HttpMethod.POST, new JObject() {
				{"Name", "Table1" },
				{"Columns", new JObject() {
					{"StringColumn", "String" },
					{"IntegerColumn", "Integer" }
				} },
				{"Department", "Administrators" },
				{"RequireValidation", true }
			});

			//Verify results
			Assert.IsTrue(Response.StatusCode == HttpStatusCode.Created);
			GenericDataTable Table = GenericDataTable.GetTableByName(Connection, "Table1");
			Assert.IsNotNull(Table);

			Dictionary<string, DataType> Columns = Table.GetColumns();
			Assert.IsTrue(Columns.Count == 4);
			Assert.IsTrue(Columns.ContainsKey("rowid") && Columns["rowid"] == DataType.Integer);
			Assert.IsTrue(Columns.ContainsKey("StringColumn") && Columns["StringColumn"] == DataType.String);
			Assert.IsTrue(Columns.ContainsKey("IntegerColumn") && Columns["IntegerColumn"] == DataType.Integer);
			Assert.IsTrue(Columns.ContainsKey("Validated") && Columns["Validated"] == DataType.Integer);
		}

		[SuppressMessage("Code Quality", "IDE0051")]
		static IEnumerable<object[]> InvalidPostTestData => new[]{
			//### Misc
			new object[] {
				//Missing fields
				new JObject(),
				"/DataTable",
				HttpStatusCode.BadRequest,
				"Missing fields"
			},
			new object[] {
				//Invalid name
				new JObject(){
					{"Name", "12345" },
					{"Columns", new JObject() {
						{"StringColumn", "String" },
						{"IntegerColumn", "Integer" }
					} },
					{"Department", "Administrators" },
					{"RequireValidation", true }
				},
				"/DataTable",
				HttpStatusCode.BadRequest,
				"Invalid name"
			},
			new object[] {
				//Already exists
				new JObject(){
					{"Name", "Table1" },
					{"Columns", new JObject() {
						{"StringColumn", "String" },
						{"IntegerColumn", "Integer" }
					} },
					{"Department", "Administrators" },
					{"RequireValidation", true }
				},
				"/DataTable",
				HttpStatusCode.BadRequest,
				"Already exists"
			},
			new object[] {
				//Invalid Department
				new JObject(){
					{"Name", "Table2" },
					{"Columns", new JObject() {
						{"StringColumn", "String" },
						{"IntegerColumn", "Integer" }
					} },
					{"Department", "SomeDepartment" },
					{"RequireValidation", true }
				},
				"/DataTable",
				HttpStatusCode.BadRequest,
				"No such department"
			},
			new object[] {
				//Reserved column
				new JObject(){
					{"Name", "Table2" },
					{"Columns", new JObject() {
						{"StringColumn", "String" },
						{"Validated", "Integer" }
					} },
					{"Department", "Administrators" },
					{"RequireValidation", true }
				},
				"/DataTable",
				HttpStatusCode.BadRequest,
				"Invalid or reserved column name"
			},
			new object[] {
				//Invalid column type
				new JObject(){
					{"Name", "Table2" },
					{"Columns", new JObject() {
						{"StringColumn", "SomeType" },
						{"IntegerColumn", "Integer" }
					} },
					{"Department", "Administrators" },
					{"RequireValidation", true }
				},
				"/DataTable",
				HttpStatusCode.BadRequest,
				"Invalid column type. Type must be either Integer, String, Real, or Blob"
			}
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