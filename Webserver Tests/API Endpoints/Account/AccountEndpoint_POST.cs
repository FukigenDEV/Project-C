using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Webserver.Data;

namespace Webserver.API_Endpoints.Tests {
	public partial class AccountEndpointTests : APITestMethods {

		/// <summary>
		/// Check if we can create an account using valid arguments
		/// </summary>
		[TestMethod]
		public void POST_ValidArguments() {
			ResponseProvider Response = ExecuteSimpleRequest("/account", HttpMethod.POST, new JObject() {
				{"Email", "user@example.com"},
				{"Password", "examplepassword12345"},
				{"MemberDepartments", new JObject() {
					{"All Users", "Manager" }
				}}
			});

			Assert.IsTrue(Response.StatusCode == HttpStatusCode.Created);
			User Account = User.GetUserByEmail(Connection, "user@example.com");
			Assert.IsNotNull(Account);
			Assert.IsTrue(Account.GetPermissionLevel(Connection, 2) == PermLevel.Manager);
		}

		[SuppressMessage("Code Quality", "IDE0051")]
		static IEnumerable<object[]> InvalidTestData => new[]{
			new object[] {
				new JObject() {
					{"Email", "SomeEmail"},
					{"Password", "examplepassword12345"},
					{"MemberDepartments", new JObject() {
						{"All Users", "Manager" }
					} }
				},
				HttpStatusCode.BadRequest,
				"Invalid email"
			},
			new object[] {
				new JObject() {
					{"Email", "user@example.com"},
					{"Password", "a"},
					{"MemberDepartments", new JObject() {
						{"All Users", "Manager" }
					}}
				},
				HttpStatusCode.BadRequest,
				"Password does not meet requirements"
			}
		};

		/// <summary>
		/// Check invalid arguments
		/// </summary>
		[TestMethod]
		[DynamicData("InvalidTestData")]
		public void POST_InvalidArguments(JObject Request, HttpStatusCode StatusCode, string ResponseMsg) {
			ResponseProvider Response = ExecuteSimpleRequest("/account", HttpMethod.POST, Request);
			Assert.IsTrue(Response.StatusCode == StatusCode);
			string Result = Encoding.UTF8.GetString(Response.Data);
			Assert.IsTrue(Result == ResponseMsg);
		}

		[TestMethod]
		public void POST_AlreadyExists() {
			new User("user@example.com", "SomePassword", Connection);
			ResponseProvider Response = ExecuteSimpleRequest("/account", HttpMethod.POST, new JObject() {
				{"Email", "user@example.com"},
				{"Password", "examplepassword12345"},
				{"MemberDepartments", new JObject() {
					{"All Users", "Manager" }
				}}
			});

			Assert.IsTrue(Response.StatusCode == HttpStatusCode.BadRequest);
			Assert.IsTrue(Encoding.UTF8.GetString(Response.Data) == "A user with this email already exists");
		}

		/// <summary>
		/// Check if MemberDepartments entries with an invalid account type are silently ignored
		/// </summary>
		[TestMethod]
		public void POST_InvalidAccountType() {
			ResponseProvider Response = ExecuteSimpleRequest("/account", HttpMethod.POST, new JObject() {
				{"Email", "user@example.com"},
				{"Password", "examplepassword12345"},
				{"MemberDepartments", new JObject() {
					{"All Users", "SomeAccountType" }
				}}
			});

			Assert.IsTrue(Response.StatusCode == HttpStatusCode.Created);
			User Account = User.GetUserByEmail(Connection, "user@example.com");
			Assert.IsNotNull(Account);
			Assert.IsTrue(Account.GetPermissionLevel(Connection, 2) == PermLevel.User);
		}

		/// <summary>
		/// Check if MemberDepartments entries with an invalid department are silently ignored
		/// </summary>
		[TestMethod]
		public void POST_InvalidDepartment() {
			ResponseProvider Response = ExecuteSimpleRequest("/account", HttpMethod.POST, new JObject() {
				{"Email", "user@example.com"},
				{"Password", "examplepassword12345"},
				{"MemberDepartments", new JObject() {
					{"SomeDepartment", "Manager" }
				}}
			});
			Assert.IsTrue(Response.StatusCode == HttpStatusCode.Created);
		}

		/// <summary>
		/// Check if we can properly apply optional fields.
		/// </summary>
		[TestMethod]
		public void POST_OptionalArguments() {
			ResponseProvider Response = ExecuteSimpleRequest("/account", HttpMethod.POST, new JObject() {
				{"Email", "user@example.com"},
				{"Password", "examplepassword12345"},
				{"MemberDepartments", new JObject() {
					{"SomeDepartment", "Manager" }
				}},
				{"Firstname", "Person" },
				{"Lastname", "McPersonface" }
			});

			Assert.IsTrue(Response.StatusCode == HttpStatusCode.Created);
			User Account = User.GetUserByEmail(Connection, "user@example.com");
			Assert.IsNotNull(Account);
			Assert.IsTrue(Account.Firstname == "Person");
			Assert.IsTrue(Account.Lastname == "McPersonface");
		}
	}
}