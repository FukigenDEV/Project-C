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

namespace Webserver.API_Endpoints.Tests {
	[TestClass()]
	public partial class DataTableEndpoint : APITestMethods {

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
			return new GenericDataTable(Connection, Name, Columns, DepartmentID, ReqValidation);
		}

		/// <summary>
		/// Check if we can retrieve all tables that belong to a specific department
		/// </summary>
		[TestMethod]
		public void GET_ValidSingleDepartment() {
			CreateTestTable();
			CreateTestTable(Name: "Table2", DepartmentID: 2);

			ResponseProvider Response = ExecuteSimpleRequest("/DataTable?department=Administrators", HttpMethod.GET);

			//Verify results
			Assert.IsTrue(Response.StatusCode == HttpStatusCode.OK);
			JArray Data = JArray.Parse(Encoding.UTF8.GetString(Response.Data));
			JArray Expected = new JArray() {
				new JObject() {
					{"Name", "Table1" },
					{"ReqValidation", true },
					{"Department", 1 }
				}
			};
			Assert.IsTrue(JToken.DeepEquals(Data, JArray.Parse(Expected.ToString())));
		}

		/// <summary>
		/// Check if we can retrieve all tables
		/// </summary>
		[TestMethod]
		public void GET_ValidAllDepartments() {
			CreateTestTable();
			CreateTestTable(Name: "Table2", DepartmentID: 2);

			ResponseProvider Response = ExecuteSimpleRequest("/DataTable", HttpMethod.GET);

			//Verify results
			Assert.IsTrue(Response.StatusCode == HttpStatusCode.OK);
			JArray Data = JArray.Parse(Encoding.UTF8.GetString(Response.Data));
			JArray Expected = new JArray() {
				new JObject() {
					{"Name", "Table1" },
					{"ReqValidation", true },
					{"Department", 1 }
				},
				new JObject() {
					{"Name", "Table2" },
					{"ReqValidation", true },
					{"Department", 2 }
				}
			};
			Assert.IsTrue(JToken.DeepEquals(Data, JArray.Parse(Expected.ToString())));
		}

		/// <summary>
		/// Check if we get a NotFound statuscode if we try to get tables that belong to a nonexistent department
		/// </summary>
		[TestMethod]
		public void GET_InvalidDepartment() {
			ResponseProvider Response = ExecuteSimpleRequest("/DataTable?department=SomeDepartment", HttpMethod.GET);
			Assert.IsTrue(Response.StatusCode == HttpStatusCode.NotFound);
			Assert.IsTrue(Encoding.UTF8.GetString(Response.Data) == "No such department");
		}
	}
}