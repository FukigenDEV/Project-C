using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Configurator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PrettyConsole;
using Webserver;
using Webserver.Threads;

namespace Webserver_Tests {
	[TestClass]
	public class AssemblyInit {
		[AssemblyInitialize]
		public static void Init(TestContext _) {
			Config.AddConfig(new StreamReader(Assembly.LoadFrom("Webserver").GetManifestResourceStream("Webserver.DefaultConfig.json")));
			Config.SaveDefaultConfig();
			Config.LoadConfig();

			LogTab Tab = new LogTab("General");
			Program.Log = Tab.GetLogger();
			RequestWorker.RequestLoggerTab = new LogTab("Worker");

		}
	}
}
