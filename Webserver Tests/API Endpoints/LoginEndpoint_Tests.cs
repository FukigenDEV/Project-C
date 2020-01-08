using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Text;
using Webserver;
using Webserver.API_Endpoints.Tests;
using Webserver.Data;

namespace Webserver.API_Endpoints.Tests {
	[TestClass]
	public class LoginEndpoint_Tests : APITestMethods {

		/// <summary>
		/// Call base ClassInit because it can't be inherited
		/// </summary>
		[ClassInitialize]
		public new static void ClassInit(TestContext C) => APITestMethods.ClassInit(C);

		[TestMethod]
		public void POST_ValidArguments() {
			ResponseProvider Response = ExecuteSimpleRequest("/login", HttpMethod.POST, new JObject() {
				{"Email", "Administrator" },
				{"Password", "W@chtw00rd" },
				{"RememberMe", true }
			}, false);

			Assert.IsTrue(Response.StatusCode == HttpStatusCode.NoContent);
			Assert.IsTrue(Response.Headers.AllKeys.Contains("Set-Cookie"));
			Session S = Session.GetUserSession(Connection, Response.Headers.Get("Set-Cookie").Split(";")[0].Replace("SessionID=", ""));
			Assert.IsNotNull(S);
			Assert.IsTrue(S.User == 1);
			Assert.IsTrue(S.RememberMe);
		}

		[TestMethod]
		public void POST_RenewSession() {
			Cookie SessionCookie = CreateSession("Administrator", true);
			long CurrentToken = Session.GetUserSession(Connection, SessionCookie.Value).Token;

			ResponseProvider Response = ExecuteSimpleRequest("/login", HttpMethod.POST, new JObject(), false, SessionCookie);

			Assert.IsTrue(Response.StatusCode == HttpStatusCode.OK);
			Assert.IsTrue(Encoding.UTF8.GetString(Response.Data) == "Renewed");
			Session S = Session.GetUserSession(Connection, SessionCookie.Value);
			Assert.IsNotNull(S);
			Assert.IsTrue(S.Token == CurrentToken);
		}

		[SuppressMessage("Code Quality", "IDE0051")]
		static IEnumerable<object[]> InvalidPostTestData => new[]{
			new object[] {
				new JObject() {
					{ "Email", "Administrator" }
				},
				HttpStatusCode.BadRequest,
				"Missing fields",
			},
			new object[] {
				new JObject() {
					{"Email", "SomeEmail" },
					{"Password", "W@chtw00rd" },
					{"RememberMe", true }
				},
				HttpStatusCode.BadRequest,
				"Invalid Email",
			},
			new object[] {
				new JObject() {
					{"Email", "user@example.com" },
					{"Password", "W@chtw00rd" },
					{"RememberMe", true }
				},
				HttpStatusCode.BadRequest,
				"No such user",
			},
			new object[] {
				new JObject() {
					{"Email", "Administrator" },
					{"Password", "" },
					{"RememberMe", true }
				},
				HttpStatusCode.BadRequest,
				"Empty password",
			},
			new object[] {
				new JObject() {
					{"Email", "Administrator" },
					{"Password", "SomePassword" },
					{"RememberMe", true }
				},
				HttpStatusCode.Unauthorized,
				null,
			}
		};

		/// <summary>
		/// Check invalid arguments
		/// </summary>
		[TestMethod]
		[DynamicData("InvalidPostTestData")]
		public void POST_InvalidArguments(JObject Request, HttpStatusCode StatusCode, string ResponseMsg) {
			ResponseProvider Response = ExecuteSimpleRequest("/login", HttpMethod.POST, Request, false);
			Assert.IsTrue(Response.StatusCode == StatusCode);
			if (ResponseMsg != null) Assert.IsTrue(Encoding.UTF8.GetString(Response.Data) == ResponseMsg);
		}

		[TestMethod]
		public void DELETE() {
			Cookie SessionCookie = CreateSession();
			ResponseProvider Response = ExecuteSimpleRequest("/login", HttpMethod.DELETE, null, false, SessionCookie);
			Assert.IsTrue(Response.StatusCode == HttpStatusCode.OK);
			Assert.IsNull(Session.GetUserSession(Connection, SessionCookie.Value));
		}
	}
}
