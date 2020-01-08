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
		[TestMethod]
		public void EDIT_ValidArguments() {
			new User("user@example.com", "SomePassword", Connection);
			ResponseProvider Response = ExecuteSimpleRequest("/account?email=user@example.com", HttpMethod.PATCH, new JObject() {
				{"Email", "test@example.com" },
				{"Firstname", "Person"},
				{"Lastname", "McPersonface" },
				{"MemberDepartments", new JObject() {
					{"Administrators", "Administrator" }
				}}
			});

			Assert.IsTrue(Response.StatusCode == HttpStatusCode.OK);
			User Acc = User.GetUserByEmail(Connection, "test@example.com");
			Assert.IsNotNull(Acc);
			Assert.IsTrue(Acc.Email == "test@example.com");
			Assert.IsTrue(Acc.Firstname == "Person");
			Assert.IsTrue(Acc.Lastname == "McPersonface");
			Assert.IsTrue(Acc.IsAdmin(Connection));
		}

		[SuppressMessage("Code Quality", "IDE0051")]
		static IEnumerable<object[]> InvalidPatchTestData => new[]{
			new object[] {
				new JObject() {
					{ "Email", "user@example.com" }
				},
				"/account?email=Administrator",
				HttpStatusCode.Forbidden,
				null
			},
			new object[] {
				new JObject() {
					{ "Email", "user@example.com" }
				},
				"/account?email=SomeUser",
				HttpStatusCode.NotFound,
				"No such user"
			},
			new object[] {
				new JObject(),
				"/account",
				HttpStatusCode.BadRequest,
				"Missing fields"
			},
			new object[] {
				new JObject() {
					{ "Email", "SomeEmail" }
				},
				"/account?email=user@example.com",
				HttpStatusCode.BadRequest,
				"Invalid Email"
			},
			new object[] {
				new JObject() {
					{ "Email", "user@example.com" }
				},
				"/account?email=user@example.com",
				HttpStatusCode.BadRequest,
				"New Email already in use"
			},
			new object[] {
				new JObject() {
					{ "Password", "a" }
				},
				"/account?email=user@example.com",
				HttpStatusCode.BadRequest,
				"Password does not meet requirements"
			},
			new object[] {
				new JObject() {
					{"MemberDepartments", new JObject() {
						{"All Users", "SomeAccountType" }
					}}
				},
				"/account?email=user@example.com",
				HttpStatusCode.OK,
				null
			},
			new object[] {
				new JObject() {
					{"MemberDepartments", new JObject() {
						{"SomeDepartment", "Manager" }
					}}
				},
				"/account?email=user@example.com",
				HttpStatusCode.OK,
				null
			}
		};

		/// <summary>
		/// Check if we get an error if we specify invalid arguments
		/// </summary>
		[TestMethod]
		[DynamicData("InvalidPatchTestData")]
		public void EDIT_InvalidArguments(JObject JSON, string URL, HttpStatusCode StatusCode, string ResponseMessage) {
			new User("user@example.com", "SomePassword", Connection);
			ResponseProvider Response = ExecuteSimpleRequest(URL, HttpMethod.PATCH, JSON);
			Assert.IsTrue(Response.StatusCode == StatusCode);
			if (ResponseMessage != null) Assert.IsTrue(Encoding.UTF8.GetString(Response.Data) == ResponseMessage);
		}
	}
}
