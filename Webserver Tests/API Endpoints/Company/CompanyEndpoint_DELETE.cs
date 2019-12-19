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
    public partial class CompanyEndpoint_DELETE : APITestMethods
    {
        /// <summary>
        /// Check if we can delete a company
        /// </summary>
        [TestMethod]
        public void DELETE_ValidArguments()
        {
            new Company(Connection, "SomeCompany", "SomeStreet", 1, "1234AB", "SomeCity", "SomeCountry", "SomePhoneNumber", "SomeEmail");

            ResponseProvider response = ExecuteSimpleRequest("/company", HttpMethod.DELETE, new JObject() {
                {"Name", "SomeCompany"},
            });

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
            Assert.IsNull(Department.GetByName(Connection, "SomeCompany"));
        }

        [SuppressMessage("Code Quality", "IDE0051")]
        static IEnumerable<object[]> InvalidDeleteTestData => new[]{
            new object[] {
                new JObject(),
                HttpStatusCode.BadRequest,
                "Missing fields"
            }
        };

        /// <summary>
        /// Check if we get an error if we specify invalid arguments
        /// </summary>
        [TestMethod]
        [DynamicData("InvalidDeleteTestData")]
        public void DELETE_InvalidArguments(JObject JSON, HttpStatusCode statusCode, string responseMessage)
        {
            ResponseProvider response = ExecuteSimpleRequest("/company", HttpMethod.DELETE, JSON);

            Assert.IsTrue(response.StatusCode == statusCode);

            if (responseMessage != null)
            {
                Assert.IsTrue(Encoding.UTF8.GetString(response.Data) == responseMessage);
            }
        }
    }
}
