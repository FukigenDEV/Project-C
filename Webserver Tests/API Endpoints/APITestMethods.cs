using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Configurator;
using Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Webserver;
using Webserver.Data;
using Webserver.Threads;

namespace Webserver.API_Endpoints.Tests {
	public class APITestMethods {
		/// <summary>
		/// Request queue.
		/// </summary>
		public BlockingCollection<ContextProvider> Queue = new BlockingCollection<ContextProvider>();
		/// <summary>
		/// SQL transaction
		/// </summary>
		public SQLiteTransaction Transaction;

		/// <summary>
		/// Database connection.
		/// </summary>
		public static SQLiteConnection Connection;

		public static Logger Log = Program.Log = new Logger();

		public TestContext TestContext { get; set; }

		[ClassInitialize]
		public static void ClassInit(TestContext _) {
			//Run inits
			Config.AddConfig(new StreamReader(Assembly.LoadFrom("Webserver").GetManifestResourceStream("Webserver.DefaultConfig.json")));
			Config.SaveDefaultConfig();
			Config.LoadConfig();
			WebFiles.Init();
			Redirect.Init();
			Connection = Database.Init(true);

			Program.DiscoverEndpoints();
		}

		[ClassCleanup]
		public static void ClassCleanup() => Connection.Close();

		/// <summary>
		/// Sends a simple request to a RequestWorker
		/// </summary>
		public ContextProvider ExecuteSimpleRequest(string URL, HttpMethod Method, JObject JSON = null, bool Login = true) {
			RequestProvider Request = new RequestProvider(new Uri("http://localhost"+URL), Method);
			if(Login) Request.Cookies.Add(CreateSession());
			if ( JSON != null ) Request.InputStream.Write(Encoding.UTF8.GetBytes(JSON.ToString()));
			ContextProvider Context = new ContextProvider(Request);
			Queue.Add(Context);
			ExecuteQueue();
			return Context;
		}

		/// <summary>
		/// Creates a RequestWorker and runs it. The RequestWorker will continue to run until all requests in the queue have been processed.
		/// </summary>
		public void ExecuteQueue() {
			RequestWorker Worker = new RequestWorker(Log, Queue, Connection, true);
			Worker.Run();
		}

		/// <summary>
		/// Creates a new session. Bypasses the Login endpoint to save time.
		/// </summary>
		/// <param name="Email">The user to login. Defaults to Administrator</param>
		/// <param name="RememberMe">If true, delays session expiration</param>
		/// <returns>A cookie named SessionID, which contains the session ID</returns>
		public Cookie CreateSession(string Email = "Administrator", bool RememberMe = false) => new Cookie("SessionID", new Session(User.GetUserByEmail(Connection, Email).ID, RememberMe, Connection).SessionID);

		[TestInitialize]
		public void Init() {
			//Check if init should be skipped
			if (GetType().GetMethod(TestContext.TestName).GetCustomAttributes<SkipInitCleanup>().Any()) return;
			Transaction = Connection.BeginTransaction();
		}
		[TestCleanup]
		public void Cleanup() {
			//Check if cleanup should be skipped
			if (GetType().GetMethod(TestContext.TestName).GetCustomAttributes<SkipInitCleanup>().Any()) return;

			Transaction?.Rollback();
		}

		public class SkipInitCleanup : Attribute { }
	}
}
