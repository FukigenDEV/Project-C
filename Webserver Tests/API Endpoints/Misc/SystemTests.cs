using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Webserver;
using Webserver.API_Endpoints.Tests;

namespace Webserver.API_Endpoints.Tests {
	[TestClass]
	public class SystemTests : APITestMethods {

		/// <summary>
		/// Call base ClassInit because it can't be inherited
		/// </summary>
		[ClassInitialize]
		public new static void ClassInit(TestContext C) => APITestMethods.ClassInit(C);
		[ClassCleanup]
		public new static void ClassCleanup() => APITestMethods.ClassCleanup();

		[TestMethod]
		public void NotFoundTest() {
			ResponseProvider Response = ExecuteSimpleRequest("/SomeURL", HttpMethod.GET);
			Assert.IsTrue(Response.StatusCode == HttpStatusCode.NotFound);
		}
	}
}
