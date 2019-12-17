using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Webserver;

namespace Webserver.API_Endpoints.Tests {
	[TestClass]
	public class Authentication_Tests : APITestMethods {

		/// <summary>
		/// Call base ClassInit because it can't be inherited
		/// </summary>
		[ClassInitialize]
		public new static void ClassInit(TestContext C) => APITestMethods.ClassInit(C);

		/// <summary>
		/// Check if we get an Unauthorized status code if we try to use an API method without being logged in.
		/// </summary>
		[TestMethod]
		public void Authentication_LoggedOut() {
			ResponseProvider Response = ExecuteSimpleRequest("/account?email=Administrator", HttpMethod.GET, Login: false);
			Assert.IsTrue(Response.StatusCode == HttpStatusCode.Unauthorized);
		}
	}
}
