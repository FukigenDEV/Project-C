using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
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

		[DataRow("/a => /a", "/a", HttpStatusCode.LoopDetected, null)]
		[DataRow("/ => /index.html", "/", HttpStatusCode.Redirect, "/index.html")]
		[TestMethod]
		public void RedirectTest(string Entry, string Source, HttpStatusCode StatusCode, string Destination) {
			File.WriteAllText("Redirects.config", Entry);
			Redirect.Init();

			ResponseProvider Response = ExecuteSimpleRequest(Source, HttpMethod.GET, null);
			Assert.IsTrue(Response.StatusCode == StatusCode);
			Assert.IsTrue(Response.RedirectURL == Destination);
		}
	}
}
