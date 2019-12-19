using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Webserver;
using Webserver.Data;
using Webserver.API_Endpoints.Tests;

namespace Webserver_Tests.API_Endpoints.Tests
{
    [TestClass()]
    public partial class DepartmentEndpoint_GET : APITestMethods
    {
        /// <summary>
        /// Call base ClassInit because it can't be inherited
        /// </summary>
        [ClassInitialize]
        public new static void ClassInit(TestContext c) => APITestMethods.ClassInit(c);

        private readonly JObject infoTemplate1 = new JObject() {
            {"ID", 1 },
            {"Name", "Administrators" },
            {"Description", "Department for Administrators" }
        };

        private readonly JObject infoTemplate2 = new JObject() {
            {"ID", 2 },
            {"Name", "All Users" },
            {"Description", "Default Department" }
        };

        private readonly JObject infoTemplate3 = new JObject() {
            {"ID", 3 },
            {"Name", "SomeDepartment" },
            {"Description", "A department to test the application" }
        };

        /// <summary>
        /// Check if we can retrieve a single department when given valid arguments
        /// </summary>
        [TestMethod]
        public void GET_ValidArguments()
        {
            new Department(Connection, "SomeDepartment", "A department to test the application");

            ResponseProvider response = ExecuteSimpleRequest("/department?name=SomeDepartment", HttpMethod.GET);

            //Verify results
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);

            JObject data = JObject.Parse(Encoding.UTF8.GetString(response.Data));
            JArray expected = new JArray() {
                infoTemplate3
            };

            Assert.IsTrue(JToken.DeepEquals(data, JObject.Parse(expected[0].ToString())));
        }

        /// <summary>
        /// Check if we get a 404 when given invalid params
        /// </summary>
        [TestMethod]
        public void GET_InvalidArguments()
        {
            ResponseProvider response = ExecuteSimpleRequest("/department?name=SomeDepartment", HttpMethod.GET);

            // Verify results
            Assert.IsTrue(response.StatusCode == HttpStatusCode.NotFound);
        }

        /// <summary>
        /// Check if we get one result when given one valid and one invalid param
        /// </summary>
        [TestMethod]
        public void GET_MixedArguments()
        {
            ResponseProvider response = ExecuteSimpleRequest("/department?name=Administrators,SomeDepartment", HttpMethod.GET);

            // Verify results
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }

        /// <summary>
        /// Check if we can retrieve all departments if we give no department parameter
        /// </summary>
        [TestMethod]
        public void GET_AllDepartments()
        {
            // Create test departments
            new Department(Connection, "SomeDepartment1", "A department to test the application (1)");
            new Department(Connection, "SomeDepartment2", "A department to test the application (2)");

            // Create mock request
            ResponseProvider response = ExecuteSimpleRequest("/department?name=", HttpMethod.GET);

            // Verify results
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);

            JArray data = JArray.Parse(Encoding.UTF8.GetString(response.Data));
            JArray expected = new JArray() { infoTemplate1, infoTemplate2, infoTemplate3, infoTemplate3 };

            expected[2]["ID"] = 3;
            expected[2]["Name"] = "SomeDepartment1";
            expected[2]["Description"] = "A department to test the application (1)";

            expected[3]["ID"] = 4;
            expected[3]["Name"] = "SomeDepartment2";
            expected[3]["Description"] = "A department to test the application (2)";

            Assert.IsTrue(JToken.DeepEquals(data, JArray.Parse(expected.ToString())));
        }
    }
}
