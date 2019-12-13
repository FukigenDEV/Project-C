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
	public partial class DataTableEndpoint : APITestMethods {
		/// <summary>
		/// Check if we can delete a datatable
		/// </summary>
		[TestMethod]
		public void DELETE_ValidArguments() {
			CreateTestTable();
			ResponseProvider Response = ExecuteSimpleRequest("/DataTable?table=Table1", HttpMethod.DELETE, null);

			Assert.IsTrue(Response.StatusCode == HttpStatusCode.OK);
			Assert.IsNull(GenericDataTable.GetTableByName(Connection, "Table1"));
		}

		/// <summary>
		/// Check if we get a NotFound status code if we try to delete a nonexistent datatable
		/// </summary>
		[TestMethod]
		public void DELETE_NoSuchTable() {
			ResponseProvider Response = ExecuteSimpleRequest("/DataTable?table=Table1", HttpMethod.DELETE, null);
			Assert.IsTrue(Response.StatusCode == HttpStatusCode.NotFound);
			Assert.IsTrue(Encoding.UTF8.GetString(Response.Data) == "No such table");
		}

		/// <summary>
		/// Check if we get a BadRequest statuscode if we don't specify a table
		/// </summary>
		[TestMethod]
		public void DELETE_MissingParams() {
			ResponseProvider Response = ExecuteSimpleRequest("/DataTable", HttpMethod.DELETE, null);
			Assert.IsTrue(Response.StatusCode == HttpStatusCode.BadRequest);
			Assert.IsTrue(Encoding.UTF8.GetString(Response.Data) == "Missing params");
		}
	}
}