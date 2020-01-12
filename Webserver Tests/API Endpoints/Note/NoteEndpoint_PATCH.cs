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
    public partial class NoteEndpoint_PATCH : APITestMethods
    {
        [TestMethod]
        public void EDIT_ValidArguments()
        {
            new Note(Connection, "SomeTitle", "SomeText");

            ResponseProvider response = ExecuteSimpleRequest("/note?title=SomeTitle", HttpMethod.PATCH, new JObject() {
                {"Title", "SomeCoolTitle" },
                {"Text", "SomeCoolText"}
            });

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);

            Note note = Note.GetNoteByTitle(Connection, "SomeCoolTitle");

            Assert.IsNotNull(note);
            Assert.IsTrue(note.Title == "SomeCoolTitle");
            Assert.IsTrue(note.Text == "SomeCoolText");
        }

        [SuppressMessage("Code Quality", "IDE0051")]
        static IEnumerable<object[]> InvalidPatchTestData => new[]{
            new object[] {
                new JObject() {
                    { "Title", "SomeTitle" }
                },
                "/note?title=SomeOtherTitle",
                HttpStatusCode.NotFound,
                "No such note"
            },
            new object[] {
                new JObject(),
                "/note",
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
            new Note(Connection, "SomeTitle", "SomeText");

            ResponseProvider response = ExecuteSimpleRequest(URL, HttpMethod.PATCH, JSON);

            Assert.IsTrue(response.StatusCode == statusCode);

            if (responseMessage != null)
            {
                Assert.IsTrue(Encoding.UTF8.GetString(response.Data) == responseMessage);
            }
        }
    }
}
