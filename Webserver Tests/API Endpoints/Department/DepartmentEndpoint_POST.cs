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
    public partial class DepartmentEndpoint_POST : APITestMethods
    {
        /// <summary>
        /// Check if we can create a department using valid arguments
        /// </summary>
        [TestMethod]
        public void POST_ValidArguments()
        {
            ResponseProvider response = ExecuteSimpleRequest("/department", HttpMethod.POST, new JObject() {
                {"Name", "SomeDepartment"},
                {"Description", "A department to test the application."}
            });

            Assert.IsTrue(response.StatusCode == HttpStatusCode.Created);

            Department department = Department.GetByName(Connection, "SomeDepartment");

            Assert.IsNotNull(department);
        }

        [SuppressMessage("Code Quality", "IDE0051")]
        static IEnumerable<object[]> InvalidPostTestData => new[]{
            new object[] {
                new JObject() {
                    {"Name", "Administrators"},
                    {"Description", "Department for administrators."}
                },
                HttpStatusCode.BadRequest,
                "A department with this name already exists."
            },
            new object[] {
                new JObject() {
                    {"Name", "All Users"},
                    {"Description", "Department for all users."}
                },
                HttpStatusCode.BadRequest,
                "A department with this name already exists."
            },
            new object[] {
                new JObject() {
                    {"Name", "Some Department"},
                    {"Description", "Some Department"}
                },
                HttpStatusCode.Created,
                null
            }
        };

        /// <summary>
        /// Check invalid arguments
        /// </summary>
        [TestMethod]
        [DynamicData("InvalidPostTestData")]
        public void POST_InvalidArguments(JObject request, HttpStatusCode statusCode, string responseMessage)
        {
            ResponseProvider response = ExecuteSimpleRequest("/department", HttpMethod.POST, request);

            Assert.IsTrue(response.StatusCode == statusCode);

            if (responseMessage != null)
            {
                Assert.IsTrue(Encoding.UTF8.GetString(response.Data) == responseMessage);
            }
        }
    }
}
