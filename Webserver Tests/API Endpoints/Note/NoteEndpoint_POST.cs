﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public partial class NoteEndpoint_POST : APITestMethods
    {
        /// <summary>
        /// Check if we can create a note using valid arguments
        /// </summary>
        [TestMethod]
        public void POST_ValidArguments()
        {
            ResponseProvider response = ExecuteSimpleRequest("/note", HttpMethod.POST, new JObject() {
                {"Title", "SomeTitle"},
                {"Text", "SomeText"}
            });

            Assert.IsTrue(response.StatusCode == HttpStatusCode.Created);

            Note note = Note.GetNoteByTitle(Connection, "SomeTitle");

            Assert.IsNotNull(note);
        }

        [SuppressMessage("Code Quality", "IDE0051")]
        static IEnumerable<object[]> InvalidPostTestData => new[]{
            new object[] {
                new JObject() {
                    {"Title", "Some Title"},
                    {"Text", "Some Text"}
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
            ResponseProvider response = ExecuteSimpleRequest("/note", HttpMethod.POST, request);

            Assert.IsTrue(response.StatusCode == statusCode);

            if (responseMessage != null)
            {
                Assert.IsTrue(Encoding.UTF8.GetString(response.Data) == responseMessage);
            }
        }
    }
}
