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
    public partial class CompanyEndpoint_POST : APITestMethods
    {
        /// <summary>
        /// Check if we can create a company using valid arguments
        /// </summary>
        [TestMethod]
        public void POST_ValidArguments()
        {
            ResponseProvider response = ExecuteSimpleRequest("/company", HttpMethod.POST, new JObject() {
                {"Name", "SomeCompany"},
                {"Street", "SomeStreet"},
                {"HouseNumber", 1},
                {"PostCode", "1234AB"},
                {"City", "SomeCity"},
                {"Country", "SomeCountry"},
                {"PhoneNumber", "SomePhoneNumber"},
                {"Email", "SomeEmail"}
            });

            Assert.IsTrue(response.StatusCode == HttpStatusCode.Created);

            Company company = Company.GetCompanyByName(Connection, "SomeCompany");

            Assert.IsNotNull(company);
        }

        [SuppressMessage("Code Quality", "IDE0051")]
        static IEnumerable<object[]> InvalidPostTestData => new[]{
            new object[] {
                new JObject() {
                    {"Name", "Some Company"},
                    {"Street", "Some Street"},
                    {"HouseNumber", 1},
                    {"PostCode", "Some PostCode"},
                    {"City", "Some City"},
                    {"Country", "Some Country"},
                    {"PhoneNumber", "Some PhoneNumber"},
                    {"Email", "Some Email"}
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
            ResponseProvider response = ExecuteSimpleRequest("/company", HttpMethod.POST, request);

            Assert.IsTrue(response.StatusCode == statusCode);

            if (responseMessage != null)
            {
                Assert.IsTrue(Encoding.UTF8.GetString(response.Data) == responseMessage);
            }
        }
    }
}
