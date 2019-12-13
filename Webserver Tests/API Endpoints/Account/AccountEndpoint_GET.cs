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
	public partial class AccountEndpointTests : APITestMethods {

		/// <summary>
		/// Call base ClassInit because it can't be inherited
		/// </summary>
		[ClassInitialize]
		public new static void ClassInit(TestContext C) => APITestMethods.ClassInit(C);

		/// <summary>
		/// Account template to prevent code reuse
		/// </summary>
		private readonly JObject InfoTemplate = new JObject() {
			{"ID", 1 },
			{"Email", "Administrator" },
			{"Firstname", null },
			{"MiddleInitial", null },
			{"Lastname", null },
			{"Function", null },
			{"WorkPhone", null },
			{"MobilePhone", null },
			{"Birthday", "0001-01-01T00:00:00" },
			{"Country", null },
			{"Address", null },
			{"Postcode", null },
			{"Permissions", new JObject(){
				{"Administrators", "Administrator"},
				{"All Users", "Administrator" }
			}}
		};

		/// <summary>
		/// Check if we can retrieve a single user when given valid arguments
		/// </summary>
		[TestMethod]
		public void GET_ValidArguments() {
			ResponseProvider Response = ExecuteSimpleRequest("/account?email=Administrator", HttpMethod.GET);

			//Verify results
			Assert.IsTrue(Response.StatusCode == HttpStatusCode.OK);
			JArray Data = JArray.Parse(Encoding.UTF8.GetString(Response.Data));
			JArray Expected = new JArray() {
				InfoTemplate
			};
			Assert.IsTrue(JToken.DeepEquals(Data, JArray.Parse(Expected.ToString())));
		}

		/// <summary>
		/// Check if we can retrieve multiple users when given valid params
		/// </summary>
		[TestMethod]
		public void GET_BulkValidArguments() {
			//Create test user
			new User("TestUser1@example.com", "TestPassword1", Connection);
			new User("TestUser2@example.com", "TestPassword2", Connection);

			ResponseProvider Response = ExecuteSimpleRequest("/account?email=Administrator,TestUser1@example.com", HttpMethod.GET);

			//Verify results
			Assert.IsTrue(Response.StatusCode == HttpStatusCode.OK);
			JArray Data = JArray.Parse(Encoding.UTF8.GetString(Response.Data));
			JArray Expected = new JArray() {InfoTemplate, InfoTemplate};
			Expected[1]["ID"] = 2;
			Expected[1]["Email"] = "TestUser1@example.com";
			Expected[1]["Permissions"] = new JObject(){
				{"Administrators", "User" },
				{"All Users", "User" }
			};
			Assert.IsTrue(JToken.DeepEquals(Data, JArray.Parse(Expected.ToString())));
		}

		/// <summary>
		/// Check if we get no results when given invalid params
		/// </summary>
		[TestMethod]
		public void GET_InvalidArguments() {
			ResponseProvider Response = ExecuteSimpleRequest("/account?email=SomeAccount", HttpMethod.GET);

			//Verify results
			Assert.IsTrue(Response.StatusCode == HttpStatusCode.OK);
			JArray Data = JArray.Parse(Encoding.UTF8.GetString(Response.Data));
			Assert.IsTrue(Data.Count == 0);
		}

		/// <summary>
		/// Check if we get no results when given multiple invalid params
		/// </summary>
		[TestMethod]
		public void GET_BulkInvalidArguments() {
			ResponseProvider Response = ExecuteSimpleRequest("/account?email=SomeAccount,SomeOtherAccount", HttpMethod.GET);

			//Verify results
			Assert.IsTrue(Response.StatusCode == HttpStatusCode.OK);
			JArray Data = JArray.Parse(Encoding.UTF8.GetString(Response.Data));
			Assert.IsTrue(Data.Count == 0);
		}

		/// <summary>
		/// Check if we get one result when given one valid and one invalid param
		/// </summary>
		[TestMethod]
		public void GET_MixedArguments() {
			ResponseProvider Response = ExecuteSimpleRequest("/account?email=Administrator,SomeUser", HttpMethod.GET);

			//Verify results
			Assert.IsTrue(Response.StatusCode == HttpStatusCode.OK);
			JArray Data = JArray.Parse(Encoding.UTF8.GetString(Response.Data));
			JArray Expected = new JArray() {InfoTemplate};
			Assert.IsTrue(JToken.DeepEquals(Data, JArray.Parse(Expected.ToString())));
		}

		/// <summary>
		/// Check if we can retrieve the current logged in user if we give CurrentUser as email parameter
		/// </summary>
		[TestMethod]
		public void GET_CurrentUser() {
			//Create mock request
			ResponseProvider Response = ExecuteSimpleRequest("/account?email=CurrentUser", HttpMethod.GET);

			//Verify results
			Assert.IsTrue(Response.StatusCode == HttpStatusCode.OK);
			JArray Data = JArray.Parse(Encoding.UTF8.GetString(Response.Data));
			JArray Expected = new JArray() { InfoTemplate };
			Assert.IsTrue(JToken.DeepEquals(Data, JArray.Parse(Expected.ToString())));
		}

		/// <summary>
		/// Check if we can retrieve all users if we give no email parameter
		/// </summary>
		[TestMethod]
		public void GET_AllUsers() {
			//Create test users
			new User("TestUser1@example.com", "TestPassword1", Connection);
			new User("TestUser2@example.com", "TestPassword2", Connection);

			//Create mock request
			ResponseProvider Response = ExecuteSimpleRequest("/account", HttpMethod.GET);

			//Verify results
			Assert.IsTrue(Response.StatusCode == HttpStatusCode.OK);
			JArray Data = JArray.Parse(Encoding.UTF8.GetString(Response.Data));
			JArray Expected = new JArray() { InfoTemplate, InfoTemplate, InfoTemplate};
			Expected[1]["Email"] = "TestUser1@example.com";
			Expected[1]["ID"] = 2;
			Expected[1]["Permissions"] = new JObject(){
				{"Administrators", "User" },
				{"All Users", "User" }
			};
			Expected[2]["Email"] = "TestUser2@example.com";
			Expected[2]["ID"] = 3;
			Expected[2]["Permissions"] = new JObject(){
				{"Administrators", "User" },
				{"All Users", "User" }
			};
			Assert.IsTrue(JToken.DeepEquals(Data, JArray.Parse(Expected.ToString())));
		}
	}
}