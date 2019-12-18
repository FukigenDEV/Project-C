using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using Webserver;
using Webserver.API_Endpoints.Tests;
using Webserver.Data;

namespace Webserver_Tests.API_Endpoints.Tests
{
    public partial class DepartmentEndpoint_PATCH : APITestMethods
    {
        [TestMethod]
        public void EDIT_ValidArguments()
        {
            new Department(Connection, "SomeDepartment", "A department to test the application.");

            ResponseProvider response = ExecuteSimpleRequest("/department?name=SomeDepartment", HttpMethod.PATCH, new JObject() {
                {"Name", "SomeCoolDepartment" },
                {"Description", "A cool department to test the application."}
            });

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);

            Department department = Department.GetByName(Connection, "SomeCoolDepartment");

            Assert.IsNotNull(department);
            Assert.IsTrue(department.Name == "SomeCoolDepartment");
            Assert.IsTrue(department.Description == "A cool department to test the application.");
		}

		[SuppressMessage("Code Quality", "IDE0051")]
		static IEnumerable<object[]> InvalidPatchTestData => new[]{
			new object[] {
				new JObject() {
					{ "Name", "SomeDepartment" }
				},
				"/department?name=SomeOtherDepartment",
				HttpStatusCode.NotFound,
				"No such department"
			},
			new object[] {
				new JObject(),
				"/department",
				HttpStatusCode.BadRequest,
				"Missing fields"
			}
		};

		/// <summary>
		/// Check if we get an error if we specify invalid arguments
		/// </summary>
		[TestMethod]
        [DynamicData("InvalidPatchTestData")]
        public void EDIT_InvalidArguments(JObject JSON, string URL, HttpStatusCode statusCode, string responseMessage)
        {
            new Department(Connection, "SomeDepartment", "A department to test the application.");

            ResponseProvider response = ExecuteSimpleRequest(URL, HttpMethod.PATCH, JSON);

            Assert.IsTrue(response.StatusCode == statusCode);

            if (responseMessage != null)
            {
                Assert.IsTrue(Encoding.UTF8.GetString(response.Data) == responseMessage);
            }
        }
    }
}