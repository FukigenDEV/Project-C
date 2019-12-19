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
    public partial class CompanyEndpoint_PATCH : APITestMethods
    {
        [TestMethod]
        public void EDIT_ValidArguments()
        {
            new Company(Connection, "SomeCompany", "SomeStreet", 1, "1234AB", "SomeCity", "SomeCountry", "SomePhoneNumber", "SomeEmail");

            ResponseProvider response = ExecuteSimpleRequest("/company?name=SomeCompany", HttpMethod.PATCH, new JObject() {
                {"Name", "SomeCoolCompany" },
                {"Street", "SomeCoolStreet"},
                {"HouseNumber", 2},
                {"PostCode", "EFGH34"},
                {"City", "SomeCoolCity"},
                {"Country", "SomeCoolCountry"},
                {"PhoneNumber", "SomeCoolPhoneNumber"},
                {"Email", "SomeCoolEmail"},
            });

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);

            Company company = Company.GetCompanyByName(Connection, "SomeCoolCompany");

            Assert.IsNotNull(company);
            Assert.IsTrue(company.Name == "SomeCoolCompany");
            Assert.IsTrue(company.Street == "SomeCoolStreet");
            Assert.IsTrue(company.HouseNumber == 2);
            Assert.IsTrue(company.PostCode == "EFGH34");
            Assert.IsTrue(company.City == "SomeCoolCity");
            Assert.IsTrue(company.Country == "SomeCoolCountry");
            Assert.IsTrue(company.PhoneNumber == "SomeCoolPhoneNumber");
            Assert.IsTrue(company.Email == "SomeCoolEmail");
        }

        [SuppressMessage("Code Quality", "IDE0051")]
        static IEnumerable<object[]> InvalidPatchTestData => new[]{
            new object[] {
                new JObject() {
                    { "Name", "SomeCompany" }
                },
                "/company?name=SomeOtherCompany",
                HttpStatusCode.NotFound,
                "No such company"
            },
            new object[] {
                new JObject(),
                "/company",
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
            new Company(Connection, "SomeCompany", "SomeStreet", 1, "1234AB", "SomeCity", "SomeCountry", "SomePhoneNumber", "SomeEmail");

            ResponseProvider response = ExecuteSimpleRequest(URL, HttpMethod.PATCH, JSON);

            Assert.IsTrue(response.StatusCode == statusCode);

            if (responseMessage != null)
            {
                Assert.IsTrue(Encoding.UTF8.GetString(response.Data) == responseMessage);
            }
        }
    }
}
