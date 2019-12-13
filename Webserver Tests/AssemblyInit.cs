﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Configurator;
using Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Webserver;

namespace Webserver_Tests {
	[TestClass]
	public class AssemblyInit {
		[AssemblyInitialize]
		public static void Init(TestContext _) {
			Config.AddConfig(new StreamReader(Assembly.LoadFrom("Webserver").GetManifestResourceStream("Webserver.DefaultConfig.json")));
			Config.SaveDefaultConfig();
			Config.LoadConfig();

			Program.Log = new Logger();
		}
	}
}
