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
	[TestClass()]
	public partial class DataEndpoint : APITestMethods {

		/// <summary>
		/// Call base ClassInit because it can't be inherited
		/// </summary>
		[ClassInitialize]
		public new static void ClassInit(TestContext C) => APITestMethods.ClassInit(C);

		/// <summary>
		/// Create a datatable for testing purposes.
		/// The name of the table will be "Table1", and it will have 2 columns (StringColumn and IntegerColumn)
		/// </summary>
		/// <param name="Connection"></param>
		/// <returns></returns>
		private static GenericDataTable CreateTestTable(string Name = "Table1", Dictionary<string, DataType> Columns = null, int DepartmentID = 1, bool ReqValidation = true) {
			if (Columns == null) {
				Columns = new Dictionary<string, DataType>() {
					{"StringColumn", DataType.String },
					{"IntegerColumn", DataType.Integer },
				};
			}
			GenericDataTable TestTable = new GenericDataTable(Connection, Name, Columns, DepartmentID, ReqValidation);
			TestTable.Insert(new List<Dictionary<string, dynamic>> {
				{new Dictionary<string, dynamic>() {
					{"StringColumn", "Text1" },
					{"IntegerColumn", 1 }
				} },
				{new Dictionary<string, dynamic>() {
					{"StringColumn", "Text2" },
					{"IntegerColumn", 2 }
				} },
				{new Dictionary<string, dynamic>() {
					{"StringColumn", "Text3" },
					{"IntegerColumn", 3 },
					{"Validated", 1 }
				} }
			});
			return TestTable;
		}

		/// <summary>
		/// Check if we can retrieve data from a table
		/// </summary>
		[SuppressMessage("Code Quality", "IDE0051")]
		static IEnumerable<object[]> ValidGetData => new[]{
			new object[] {
				"/data?table=Table1",
				new JObject() {
					{"Table1", new JObject() {
						{"Columns", new JObject() {
							{"rowid", "Integer" },
							{"StringColumn", "String" },
							{"IntegerColumn", "Integer" },
							{"Validated", "Integer" }
						} },
						{"Rows", new JArray() {
							new JArray(){1, "Text1", 1, 0},
							new JArray(){2, "Text2", 2, 0},
							new JArray(){3, "Text3", 3, 1},
						} }
					} }
				}
			},
			new object[] {
				"/data?table=Table1&isunvalidated=true",
				new JObject() {
					{"Table1", new JObject() {
						{"Columns", new JObject() {
							{"rowid", "Integer" },
							{"StringColumn", "String" },
							{"IntegerColumn", "Integer" },
							{"Validated", "Integer" }
						} },
						{"Rows", new JArray() {
							new JArray(){1, "Text1", 1, 0},
							new JArray(){2, "Text2", 2, 0},
						} }
					} }
				}
			},
			new object[] {
				"/data?table=Table1&begin=2",
				new JObject() {
					{"Table1", new JObject() {
						{"Columns", new JObject() {
							{"rowid", "Integer" },
							{"StringColumn", "String" },
							{"IntegerColumn", "Integer" },
							{"Validated", "Integer" }
						} },
						{"Rows", new JArray() {
							new JArray(){2, "Text2", 2, 0},
							new JArray(){3, "Text3", 3, 1},
						} }
					} }
				}
			},
			new object[] {
				"/data?table=Table1&end=2",
				new JObject() {
					{"Table1", new JObject() {
						{"Columns", new JObject() {
							{"rowid", "Integer" },
							{"StringColumn", "String" },
							{"IntegerColumn", "Integer" },
							{"Validated", "Integer" }
						} },
						{"Rows", new JArray() {
							new JArray(){1, "Text1", 1, 0},
							new JArray(){2, "Text2", 2, 0},
						} }
					} }
				}
			},
			new object[] {
				"/data?table=Table1&begin=2&end=2",
				new JObject() {
					{"Table1", new JObject() {
						{"Columns", new JObject() {
							{"rowid", "Integer" },
							{"StringColumn", "String" },
							{"IntegerColumn", "Integer" },
							{"Validated", "Integer" }
						} },
						{"Rows", new JArray() {
							new JArray(){2, "Text2", 2, 0},
						} }
					} }
				}
			},
			new object[] {
				"/data?table=Table1&begin=2&isunvalidated=true",
				new JObject() {
					{"Table1", new JObject() {
						{"Columns", new JObject() {
							{"rowid", "Integer" },
							{"StringColumn", "String" },
							{"IntegerColumn", "Integer" },
							{"Validated", "Integer" }
						} },
						{"Rows", new JArray() {
							new JArray(){2, "Text2", 2, 0},
						} }
					} }
				}
			},
			new object[] {
				"/data?table=Table1&end=2&isunvalidated=true",
				new JObject() {
					{"Table1", new JObject() {
						{"Columns", new JObject() {
							{"rowid", "Integer" },
							{"StringColumn", "String" },
							{"IntegerColumn", "Integer" },
							{"Validated", "Integer" }
						} },
						{"Rows", new JArray() {
							new JArray(){1, "Text1", 1, 0},
							new JArray(){2, "Text2", 2, 0},
						} }
					} }
				}
			},
			new object[] {
				"/data?table=Table1&begin=2&end=2&isunvalidated=true",
				new JObject() {
					{"Table1", new JObject() {
						{"Columns", new JObject() {
							{"rowid", "Integer" },
							{"StringColumn", "String" },
							{"IntegerColumn", "Integer" },
							{"Validated", "Integer" }
						} },
						{"Rows", new JArray() {
							new JArray(){2, "Text2", 2, 0},
						} }
					} }
				}
			},
		};

		/// <summary>
		/// Check if we get an error if we specify invalid arguments
		/// </summary>
		[TestMethod]
		[DynamicData("ValidGetData")]
		public void GET_ValidArguments(string URL, JObject Expected) {
			CreateTestTable();
			ResponseProvider Response = ExecuteSimpleRequest(URL, HttpMethod.GET);
			Assert.IsTrue(Response.StatusCode == HttpStatusCode.OK);
			JObject Data = JObject.Parse(Encoding.UTF8.GetString(Response.Data));
			Assert.IsTrue(JToken.DeepEquals(Data, JObject.Parse(Expected.ToString())));
		}

		[TestMethod]
		public void GET_InvalidArguments() {
			ResponseProvider Response = ExecuteSimpleRequest("/data?table=SomeTable", HttpMethod.GET);
			Assert.IsTrue(Response.StatusCode == HttpStatusCode.NotFound);
			Assert.IsTrue(Encoding.UTF8.GetString(Response.Data) == "No such table");
		}
	}
}