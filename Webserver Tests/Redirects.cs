using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Webserver;

namespace Webserver_Tests {
	[TestClass]
	public class RedirectTests {

		/// <summary>
		/// Check if Resolve works properly;
		/// The function will recursively find the destination path. If the route results in a loop, null will be returned.
		/// If a destination path has no entry, it will be returned.
		/// </summary>
		/// <param name="Source"></param>
		/// <param name="Destination"></param>
		[DataRow("/a", "/b")]
		[DataRow("/c", "/b")]
		[DataRow("/e", null)]
		[DataRow("/f", "/f")]
		[TestMethod]
		public void Resolve_Test(string Source, string Destination) {
			StringBuilder SB = new StringBuilder();
			SB.AppendLine("/a => /b");
			SB.AppendLine("/c => /a");
			SB.AppendLine("/e => /e");
			File.WriteAllText("Redirects.config", SB.ToString());
			Redirect.ParseRedirectFile("Redirects.config");
			Assert.IsTrue(Redirect.Resolve(Source) == Destination);
		}

		/// <summary>
		/// Check if ParseRedirectsFile can parse a Redirects.config file and properly ignore all invalid entries.
		/// </summary>
		/// <param name="Entry"></param>
		[DataRow("=>")]
		[DataRow("/a => bbb")]
		[DataRow("bbb => /a")]
		[DataRow("/a => /c")]
		[TestMethod]
		public void ParseRedirectFile_Test(string Entry) {
			StringBuilder SB = new StringBuilder();
			SB.AppendLine("/a => /b");
			SB.AppendLine(Entry);
			File.WriteAllText("Redirects.config", SB.ToString());
			Redirect.ParseRedirectFile("Redirects.config");

			Assert.IsTrue(Redirect.RedirectionDict.Count == 1);
			Assert.IsTrue(Redirect.RedirectionDict["/a"] == "/b");
		}

		/// <summary>
		/// Check if Init works.
		/// </summary>
		public void Init_Test() => Redirect.Init();
	}
}
