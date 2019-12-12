using System;
using System.Collections.Generic;
using System.Text;
using Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Webserver;

namespace Webserver_Tests {
	[TestClass]
	public class AssemblyInit {
		[AssemblyInitialize]
		public static void Init(TestContext _) {
			Program.Log = new Logger();
		}
	}
}
