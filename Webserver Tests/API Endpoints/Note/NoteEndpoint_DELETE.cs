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
    public partial class NoteEndpoint_DELETE : APITestMethods
    {
        /// <summary>
        /// Check if we can delete a note
        /// </summary>
        [TestMethod]
        public void DELETE_ValidArguments()
        {
            new Note("SomeTitle", "SomeText");

            ResponseProvider response = ExecuteSimpleRequest("/note", HttpMethod.DELETE, new JObject() {
                {"Title", "SomeTitle"},
            });

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
            Assert.IsNull(Note.GetNoteByTitle(Connection, "SomeTitle"));
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
            ResponseProvider response = ExecuteSimpleRequest("/note", HttpMethod.DELETE, JSON);

            Assert.IsTrue(response.StatusCode == statusCode);

            if (responseMessage != null)
            {
                Assert.IsTrue(Encoding.UTF8.GetString(response.Data) == responseMessage);
            }
        }
    }
}
