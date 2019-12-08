using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using System.Text;
using Configurator;
using Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Webserver;
using Webserver.Threads;

namespace Webserver.API_Endpoints.Tests {
	public class APITestMethods {
		/// <summary>
		/// Request queue.
		/// </summary>
		public BlockingCollection<ContextProvider> Queue = new BlockingCollection<ContextProvider>();

		[TestInitialize()]
		public void Init() {
			//Init config
			Config.AddConfig(new StreamReader(Assembly.LoadFrom("Webserver").GetManifestResourceStream("Webserver.DefaultConfig.json")));
			Config.SaveDefaultConfig();
			Config.LoadConfig();

			//Init database and create initial connection + table
			if ( File.Exists("Database.db") ) File.Delete("Database.db"); //Database doesn't always get wiped after debugging a failed test.
			Database.Init();

			Program.DiscoverEndpoints();
		}

		/// <summary>
		/// Creates a RequestWorker and runs it. The RequestWorker will continue to run until all requests in the queue have been processed.
		/// </summary>
		public void ExecuteQueue() {
			RequestWorker Worker = new RequestWorker(new Logger(), Queue, true);
			Worker.Run();
		}

		[TestCleanup()]
		public void Cleanup() => File.Delete("Database.db");
	}
}
