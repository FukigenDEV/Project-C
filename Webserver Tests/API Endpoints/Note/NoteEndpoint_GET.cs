﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public partial class NoteEndpoint_GET : APITestMethods
    {
        /// <summary>
        /// Call base ClassInit because it can't be inherited
        /// </summary>
        [ClassInitialize]
        public new static void ClassInit(TestContext c) => APITestMethods.ClassInit(c);

        private readonly JObject infoTemplate = new JObject() {
            {"ID", 1 },
            {"Title", "SomeTitle" },
            {"Text", "SomeText" }
        };

        /// <summary>
        /// Check if we can retrieve a single note when given valid arguments
        /// </summary>
        [TestMethod]
        public void GET_ValidArguments()
        {
            ResponseProvider response = ExecuteSimpleRequest("/note?title=SomeTitle", HttpMethod.GET);

            //Verify results
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);

            JArray data = JArray.Parse(Encoding.UTF8.GetString(response.Data));
            JArray expected = new JArray() {
                infoTemplate
            };

            Assert.IsTrue(JToken.DeepEquals(data, JArray.Parse(expected.ToString())));
        }

        /// <summary>
        /// Check if we can retrieve multiple notes when given valid params
        /// </summary>
        [TestMethod]
        public void GET_BulkValidArguments()
        {
            // Create test departments
            new Note("SomeTitle1", "SomeText1");
            new Note("SomeTitle2", "SomeText2");

            ResponseProvider response = ExecuteSimpleRequest("/note?=SomeTitle1,SomeTitle2", HttpMethod.GET);

            // Verify results
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);

            JArray data = JArray.Parse(Encoding.UTF8.GetString(response.Data));
            JArray expected = new JArray() { infoTemplate, infoTemplate };
            expected[0]["ID"] = 1;
            expected[0]["Title"] = "SomeTitle";
            expected[0]["Text"] = "SomeText";

            Assert.IsTrue(JToken.DeepEquals(data, JArray.Parse(expected.ToString())));
        }

        /// <summary>
        /// Check if we get no results when given invalid params
        /// </summary>
        [TestMethod]
        public void GET_InvalidArguments()
        {
            ResponseProvider response = ExecuteSimpleRequest("/note?title=SomeTitle", HttpMethod.GET);

            // Verify results
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);

            JArray data = JArray.Parse(Encoding.UTF8.GetString(response.Data));

            Assert.IsTrue(data.Count == 0);
        }

        /// <summary>
        /// Check if we get no results when given multiple invalid params
        /// </summary>
        [TestMethod]
        public void GET_BulkInvalidArguments()
        {
            ResponseProvider response = ExecuteSimpleRequest("/note?title=SomeTitle,SomeOtherTitle", HttpMethod.GET);

            // Verify results
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);

            JArray Data = JArray.Parse(Encoding.UTF8.GetString(response.Data));

            Assert.IsTrue(Data.Count == 0);
        }

        /// <summary>
        /// Check if we get one result when given one valid and one invalid param
        /// </summary>
        [TestMethod]
        public void GET_MixedArguments()
        {
            new Note("SomeTitle", "SomeText");

            ResponseProvider response = ExecuteSimpleRequest("/note?title=SomeTitle,SomeOtherTitle", HttpMethod.GET);

            // Verify results
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);

            JArray data = JArray.Parse(Encoding.UTF8.GetString(response.Data));
            JArray expected = new JArray() { infoTemplate };

            Assert.IsTrue(JToken.DeepEquals(data, JArray.Parse(expected.ToString())));
        }

        /// <summary>
        /// Check if we can retrieve all departments if we give no department parameter
        /// </summary>
        [TestMethod]
        public void GET_AllNotes()
        {
            // Create test departments
            new Note("SomeTitle1", "SomeText1");
            new Note("SomeTitle2", "SomeText2");

            // Create mock request
            ResponseProvider response = ExecuteSimpleRequest("/note", HttpMethod.GET);

            // Verify results
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);

            JArray data = JArray.Parse(Encoding.UTF8.GetString(response.Data));
            JArray expected = new JArray() { infoTemplate, infoTemplate, infoTemplate };

            expected[0]["ID"] = 1;
            expected[0]["Title"] = "SomeTitle1";
            expected[0]["Text"] = "SomeText1";

            expected[1]["ID"] = 1;
            expected[1]["Title"] = "SomeTitle2";
            expected[1]["Text"] = "SomeText2";

            Assert.IsTrue(JToken.DeepEquals(data, JArray.Parse(expected.ToString())));
        }
    }
}
