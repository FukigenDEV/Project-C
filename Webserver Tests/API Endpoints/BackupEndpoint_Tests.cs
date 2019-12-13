using Microsoft.VisualStudio.TestTools.UnitTesting;
using Webserver.API_Endpoints;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Webserver.API_Endpoints.Tests {
	[TestClass()]
	public class BackupEndpoint_Tests : APITestMethods {

		public string Timestamp;

		[ClassInitialize]
		public new static void ClassInit(TestContext C) => APITestMethods.ClassInit(C);


		[TestInitialize]
		public new void Init() {
			if (Directory.Exists("Backups")) {
				Directory.Delete("Backups", true);
			}
		}

		[TestMethod]
		public void GET_GetList() {
			POST();

			ResponseProvider Response = ExecuteSimpleRequest("/backup", HttpMethod.GET);

			//Verify results
			Assert.IsTrue(Response.StatusCode == HttpStatusCode.OK);
			Assert.IsTrue(Response.ContentType == "application/json");
			JArray Data = JArray.Parse(Encoding.UTF8.GetString(Response.Data));
			JArray Expected = new JArray() {
				"Backup_"+Timestamp+"_0"
			};
			Assert.IsTrue(JToken.DeepEquals(Data, JArray.Parse(Expected.ToString())));
		}

		[TestMethod]
		public void GET_GetFile() {
			POST();

			//Create mock request
			ResponseProvider Response = ExecuteSimpleRequest("/backup?name=Backup_" + Timestamp + "_0", HttpMethod.GET);

			//Verify results
			Assert.IsTrue(Response.StatusCode == HttpStatusCode.OK);
			Assert.IsTrue(Response.ContentType == "application/zip");
			Assert.IsTrue(Response.Headers.Get("Content-disposition") == "attachment; filename=Backup_" + Timestamp + "_0.zip");
		}

		/// <summary>
		/// Check if we get a 404 Not Found if we try to retrieve an invalid file
		/// </summary>
		[TestMethod]
		public void GET_GetInvalidFile() {
			ResponseProvider Response = ExecuteSimpleRequest("/backup?name=SomeFile", HttpMethod.GET);

			//Verify results
			Assert.IsTrue(Response.StatusCode == HttpStatusCode.NotFound);
		}

		/// <summary>
		/// Check if we can create a manual backup
		/// </summary>
		[TestMethod]
		public void POST() {
			ResponseProvider Response = ExecuteSimpleRequest("/backup", HttpMethod.POST);

			//Verify results
			Assert.IsTrue(Response.StatusCode == HttpStatusCode.OK);
			Assert.IsTrue(Directory.Exists("Backups"));
			Assert.IsTrue(Directory.GetFiles("Backups").Length == 1);
			Timestamp = DateTime.Now.ToString(BackupManager.Format);
			Assert.IsTrue(File.Exists("Backups\\Backup_" + Timestamp + "_0.zip"));
		}
	}
}