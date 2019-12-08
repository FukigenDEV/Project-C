using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Webserver;

namespace Webserver.API_Endpoints.Tests {
	[TestClass]
	public class Authentication : APITestMethods {

		/// <summary>
		/// Check if we get an Unauthorized status code if we try to use an API method without being logged in.
		/// </summary>
		[TestMethod]
		public void Authentication_LoggedOut() {
			RequestProvider Request = new RequestProvider(new Uri("http://localhost/account?email=Administrator"), HttpMethod.GET);
			ResponseProvider Response = new ResponseProvider();

			Queue.Add(new ContextProvider(Request, Response));
			ExecuteQueue();

			Assert.IsTrue(Response.StatusCode == HttpStatusCode.Unauthorized);
		}
	}
}
