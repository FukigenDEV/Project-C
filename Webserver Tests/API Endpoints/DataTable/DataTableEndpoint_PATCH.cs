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
		/// Check if we can properly modify a table
		/// </summary>
		[TestMethod]
		public void PATCH_ValidArguments() {
			CreateTestTable(ReqValidation: false);

			ResponseProvider Response = ExecuteSimpleRequest("/DataTable?table=Table1", HttpMethod.PATCH, new JObject() {
				{"Add", new JObject() {
					{"Column1", "Blob" },
					{"Validated", "Integer"}
				}},
				{"Delete", new JArray() {
					"IntegerColumn"
				}},
				{"Rename", new JObject() {
					{"StringColumn", "Column2" }
				}}
			});

			//Verify results
			Assert.IsTrue(Response.StatusCode == HttpStatusCode.OK);
			GenericDataTable Table = GenericDataTable.GetTableByName(Connection, "Table1");
			Dictionary<string, DataType> Columns = Table.GetColumns();
			Assert.IsTrue(Columns.Count == 4);
			Assert.IsTrue(Columns["Column1"] == DataType.Blob);
			Assert.IsTrue(Columns["Column2"] == DataType.String);
			Assert.IsTrue(Table.ReqValidation);
		}

		[SuppressMessage("Code Quality", "IDE0051")]
		static IEnumerable<object[]> InvalidPatchTestData => new[]{
			//### Misc
			new object[] {
				//Missing table parameter
				new JObject(){
					{"Add", new JObject() {
						{ "Column1", "String" }
					} }
				},
				"/DataTable",
				HttpStatusCode.BadRequest,
				"Missing params"
			},
			new object[] {
				//Missing JSON fields
				new JObject(),
				"/DataTable?table=Table1",
				HttpStatusCode.BadRequest,
				"Missing fields"
			},
			new object[] {
				//Attempting to edit a nonexistent table
				new JObject(){
					{"Add", new JObject() {
						{ "Column1", "String" }
					} }
				},
				"/DataTable?table=SomeNonexistentTable",
				HttpStatusCode.NotFound,
				"No such table"
			},

			//#### Add Columns
			new object[] {
				//Add invalid name (regex)
				new JObject(){
					{"Add", new JObject() {
						{ "12345", "String" }
					} }
				},
				"/DataTable?table=Table1",
				HttpStatusCode.BadRequest,
				"Invalid entry in Add (12345)"
			},
			new object[] {
				//Add invalid name (rowid)
				new JObject(){
					{"Add", new JObject() {
						{ "rowid", "String" }
					} }
				},
				"/DataTable?table=Table1",
				HttpStatusCode.BadRequest,
				"Invalid entry in Add (rowid)"
			},
			new object[] {
				//Add invalid name (already exists)
				new JObject(){
					{"Add", new JObject() {
						{ "StringColumn", "String" }
					} }
				},
				"/DataTable?table=Table1",
				HttpStatusCode.BadRequest,
				"Invalid entry in Add (StringColumn)"
			},

			//### Delete Columns
			new object[] {
				//Delete invalid name (invalid type)
				new JObject(){
					{"Delete", new JArray() {
						12345
					} }
				},
				"/DataTable?table=Table1",
				HttpStatusCode.BadRequest,
				"Invalid entry in Delete (12345)"
			},
			new object[] {
				//Delete invalid name (doesn't exist)
				new JObject(){
					{"Delete", new JArray() {
						"SomeColumn"
					} }
				},
				"/DataTable?table=Table1",
				HttpStatusCode.BadRequest,
				"Invalid entry in Delete (SomeColumn)"
			},
			new object[] {
				//Delete invalid name (rowid)
				new JObject(){
					{"Delete", new JArray() {
						"rowid"
					} }
				},
				"/DataTable?table=Table1",
				HttpStatusCode.BadRequest,
				"Invalid entry in Delete (rowid)"
			},

			//### Rename Columns
			new object[] {
				//Rename invalid name (target not a string)
				new JObject(){
					{"Rename", new JObject() {
						{"StringColumn", 12345 }
					} }
				},
				"/DataTable?table=Table1",
				HttpStatusCode.BadRequest,
				"Invalid entry in Rename (StringColumn)"
			},
			new object[] {
				//Rename invalid name (rename to Validated)
				new JObject(){
					{"Rename", new JObject() {
						{"StringColumn", "Validated" }
					} }
				},
				"/DataTable?table=Table1",
				HttpStatusCode.BadRequest,
				"Invalid entry in Rename (StringColumn)"
			},
			new object[] {
				//Rename invalid name (rename to rowid)
				new JObject(){
					{"Rename", new JObject() {
						{"StringColumn", "rowid" }
					} }
				},
				"/DataTable?table=Table1",
				HttpStatusCode.BadRequest,
				"Invalid entry in Rename (StringColumn)"
			},
			new object[] {
				//Rename invalid name (rename Validated)
				new JObject(){
					{"Rename", new JObject() {
						{"Validated", "Column1" }
					} }
				},
				"/DataTable?table=Table1",
				HttpStatusCode.BadRequest,
				"Invalid entry in Rename (Validated)"
			},
			new object[] {
				//Rename invalid name (rename rowid)
				new JObject(){
					{"Rename", new JObject() {
						{"rowid", "Column1" }
					} }
				},
				"/DataTable?table=Table1",
				HttpStatusCode.BadRequest,
				"Invalid entry in Rename (rowid)"
			},
			new object[] {
				//Rename invalid name (target already exists)
				new JObject(){
					{"Rename", new JObject() {
						{"StringColumn", "IntegerColumn" }
					} }
				},
				"/DataTable?table=Table1",
				HttpStatusCode.BadRequest,
				"Invalid entry in Rename (StringColumn)"
			},
			new object[] {
				//Rename invalid name (source doesn't exist)
				new JObject(){
					{"Rename", new JObject() {
						{"SomeColumn", "Column1" }
					} }
				},
				"/DataTable?table=Table1",
				HttpStatusCode.BadRequest,
				"Invalid entry in Rename (SomeColumn)"
			},
			new object[] {
				//Rename invalid name (regex)
				new JObject(){
					{"Rename", new JObject() {
						{"StringColumn", "12345" }
					} }
				},
				"/DataTable?table=Table1",
				HttpStatusCode.BadRequest,
				"Invalid entry in Rename (StringColumn)"
			},
		};

		/// <summary>
		/// Check if we get an error if we specify invalid arguments
		/// </summary>
		[TestMethod]
		[DynamicData("InvalidPatchTestData")]
		public void EDIT_InvalidArguments(JObject JSON, string URL, HttpStatusCode StatusCode, string ResponseMessage = null) {
			CreateTestTable();
			ResponseProvider Response = ExecuteSimpleRequest(URL, HttpMethod.PATCH, JSON);
			Assert.IsTrue(Response.StatusCode == StatusCode);
			if (ResponseMessage != null) Assert.IsTrue(Encoding.UTF8.GetString(Response.Data) == ResponseMessage);
		}
	}
}