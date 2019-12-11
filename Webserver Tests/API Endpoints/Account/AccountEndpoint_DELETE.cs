using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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

		/// <summary>
		/// Check if we get a Forbidden statuscode if we try to delete Administrator
		/// </summary>
		[TestMethod]
		public void DELETE_Admin() {
			ResponseProvider Response = ExecuteSimpleRequest("/account", HttpMethod.DELETE, new JObject() {
				{"Email", "Administrator"},
			});

			Assert.IsTrue(Response.StatusCode == HttpStatusCode.Forbidden);
		}

		/// <summary>
		/// Check if we get a NotFound statuscode if we try to delete a nonexistent account
		/// </summary>
		[TestMethod]
		public void DELETE_NoSuchUser() {
			ResponseProvider Response = ExecuteSimpleRequest("/account", HttpMethod.DELETE, new JObject() {
				{"Email", "user@example.com"},
			});

			Assert.IsTrue(Response.StatusCode == HttpStatusCode.NotFound);
			Assert.IsTrue(Encoding.UTF8.GetString(Response.Data) == "No such user");
		}

		/// <summary>
		/// Check if we get a BadRequest if we don't specify an email
		/// </summary>
		[TestMethod]
		public void DELETE_MissingFields() {
			ResponseProvider Response = ExecuteSimpleRequest("/account", HttpMethod.DELETE, new JObject());

			Assert.IsTrue(Response.StatusCode == HttpStatusCode.BadRequest);
			Assert.IsTrue(Encoding.UTF8.GetString(Response.Data) == "Missing fields");
		}
	}
}
