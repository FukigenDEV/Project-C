using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using Webserver.Data;

namespace Webserver.API_Endpoints.Tests {
	public partial class AccountEndpointTests : APITestMethods {
		/// <summary>
		/// Check if we can delete an account
		/// </summary>
		[TestMethod]
		public void DELETE_ValidArguments() {
			new User("user@example.com", "SomePassword", Connection);
			ResponseProvider Response = ExecuteSimpleRequest("/account", HttpMethod.DELETE, new JObject() {
				{"Email", "user@example.com"},
			});

			Assert.IsTrue(Response.StatusCode == HttpStatusCode.OK);
			Assert.IsNull(User.GetUserByEmail(Connection, "user@example.com"));
		}

		[SuppressMessage("Code Quality", "IDE0051")]
		static IEnumerable<object[]> InvalidDeleteTestData => new[]{
			new object[] {
				new JObject() {
					{"Email", "Administrator"},
				},
				HttpStatusCode.Forbidden,
				null
			},
			new object[] {
				new JObject() {
					{"Email", "user@example.com"},
				},
				HttpStatusCode.NotFound,
				"No such user"
			},
			new object[] {
				new JObject(),
				HttpStatusCode.BadRequest,
				"Missing fields"
			}
		};

		/// <summary>
		/// Check if we get an error if we specify invalid arguments
		/// </summary>
		[TestMethod]
		[DynamicData("InvalidDeleteTestData")]
		public void DELETE_InvalidArguments(JObject JSON, HttpStatusCode StatusCode, string ResponseMessage) {
			ResponseProvider Response = ExecuteSimpleRequest("/account", HttpMethod.DELETE, JSON);
			Assert.IsTrue(Response.StatusCode == StatusCode);
			if (ResponseMessage != null) Assert.IsTrue(Encoding.UTF8.GetString(Response.Data) == ResponseMessage);
		}
	}
}
