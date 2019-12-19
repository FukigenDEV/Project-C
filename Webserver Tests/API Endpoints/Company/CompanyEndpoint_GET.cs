using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Webserver;
using Webserver.API_Endpoints.Tests;
using Webserver.Data;

namespace Webserver_Tests.API_Endpoints.Tests
{
    [TestClass()]
    public partial class CompanyEndpoint_GET : APITestMethods
    {
        /// <summary>
        /// Call base ClassInit because it can't be inherited
        /// </summary>
        [ClassInitialize]
        public new static void ClassInit(TestContext c) => APITestMethods.ClassInit(c);

        private readonly JObject infoTemplate = new JObject() {
            {"ID", 1 },
            {"Name", "SomeCompany" },
            {"Street", "SomeStreet" },
            {"HouseNumber", 1 },
            {"PostCode", "1234AB" },
            {"City", "SomeCity" },
            {"Country", "SomeCountry" },
            {"PhoneNumber", "SomePhoneNumber" },
            {"Email", "SomeEmail" }
        };

        /// <summary>
        /// Check if we can retrieve a single company when given valid arguments
        /// </summary>
        [TestMethod]
        public void GET_ValidArguments()
        {
            new Company(Connection, "SomeCompany", "SomeStreet", 1, "1234AB", "SomeCity", "SomeCountry", "SomePhoneNumber", "SomeEmail");

            ResponseProvider response = ExecuteSimpleRequest("/company?name=SomeCompany", HttpMethod.GET);

            //Verify results
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);

            JObject data = JObject.Parse(Encoding.UTF8.GetString(response.Data));
            JArray expected = new JArray() {
                infoTemplate
            };

            Assert.IsTrue(JToken.DeepEquals(data, JObject.Parse(expected[0].ToString())));
        }

        /// <summary>
        /// Check if we get a 404 when given invalid params
        /// </summary>
        [TestMethod]
        public void GET_InvalidArguments()
        {
            ResponseProvider response = ExecuteSimpleRequest("/company?name=SomeDepartment", HttpMethod.GET);

            // Verify results
            Assert.IsTrue(response.StatusCode == HttpStatusCode.NotFound);
        }

        /// <summary>
        /// Check if we can retrieve all companies if we give no department parameter
        /// </summary>
        [TestMethod]
        public void GET_AllCompanies()
        {
            // Create test companies
            new Company(Connection, "SomeCompany1", "SomeStreet", 1, "1234AB", "SomeCity", "SomeCountry", "SomePhoneNumber", "SomeEmail");
            new Company(Connection, "SomeCompany2", "SomeStreet", 1, "1234AB", "SomeCity", "SomeCountry", "SomePhoneNumber", "SomeEmail");

            // Create mock request
            ResponseProvider response = ExecuteSimpleRequest("/company?name=", HttpMethod.GET);

            // Verify results
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);

            JArray data = JArray.Parse(Encoding.UTF8.GetString(response.Data));
            JArray expected = new JArray() { infoTemplate, infoTemplate };

            expected[0]["ID"] = 1;
            expected[0]["Name"] = "SomeCompany1";
            expected[0]["Street"] = "SomeStreet";
            expected[0]["HouseNumber"] = 1;
            expected[0]["PostCode"] = "1234AB";
            expected[0]["City"] = "SomeCity";
            expected[0]["Country"] = "SomeCountry";
            expected[0]["PhoneNumber"] = "SomePhoneNumber";
            expected[0]["Email"] = "SomeEmail";

            expected[1]["ID"] = 2;
            expected[1]["Name"] = "SomeCompany2";
            expected[1]["Street"] = "SomeStreet";
            expected[1]["HouseNumber"] = 1;
            expected[1]["PostCode"] = "1234AB";
            expected[1]["City"] = "SomeCity";
            expected[1]["Country"] = "SomeCountry";
            expected[1]["PhoneNumber"] = "SomePhoneNumber";
            expected[1]["Email"] = "SomeEmail";

            Assert.IsTrue(JToken.DeepEquals(data, JArray.Parse(expected.ToString())));
        }
    }
}
