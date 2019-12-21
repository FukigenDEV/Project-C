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
		/// Check if we can delete a datatable
		/// </summary>
		[TestMethod]
		public void DELETE_ValidArguments() {
			CreateTestTable();
			ResponseProvider Response = ExecuteSimpleRequest("/Data?table=Table1", HttpMethod.DELETE, new JObject() {
				{"RowIDs", new JArray(){ 1, 2} }
			});

			Assert.IsTrue(Response.StatusCode == HttpStatusCode.OK);
			JObject Data = GenericDataTable.GetTableByName(Connection, "Table1").GetRows();
			Assert.IsTrue(Data.ContainsKey("Columns"));
			Assert.IsTrue(Data.ContainsKey("Rows"));
			Assert.IsTrue(((JArray)Data["Rows"]).Count == 1);
		}

		[SuppressMessage("Code Quality", "IDE0051")]
		static IEnumerable<object[]> InvalidDeleteTestData => new[]{
			//### Misc
			new object[] {
				//Missing params
				new JObject(){
					{"RowIDs", new JArray(){ 1, 2} }
				},
				"/Data",
				HttpStatusCode.BadRequest,
				"Missing params"
			},
			new object[] {
				//Missing params
				new JObject(){
					{"RowIDs", new JArray(){ 1, 2} }
				},
				"/Data?table=Table2",
				HttpStatusCode.NotFound,
				"No such table"
			},
			new object[] {
				//Missing params
				new JObject(),
				"/Data?table=Table1",
				HttpStatusCode.BadRequest,
				"Missing fields"
			},
		};

		/// <summary>
		/// Check if we get an error if we specify invalid arguments
		/// </summary>
		[TestMethod]
		[DynamicData("InvalidDeleteTestData")]
		public void DELETE_InvalidArguments(JObject JSON, string URL, HttpStatusCode StatusCode, string ResponseMessage = null) {
			GenericDataTable TestTable = CreateTestTable();
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
					{"IntegerColumn", 3 }
				} }
			});

			ResponseProvider Response = ExecuteSimpleRequest(URL, HttpMethod.DELETE, JSON);
			Assert.IsTrue(Response.StatusCode == StatusCode);
			if (ResponseMessage != null) Assert.IsTrue(Encoding.UTF8.GetString(Response.Data) == ResponseMessage);
		}
	}
}