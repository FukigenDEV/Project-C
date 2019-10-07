using Configurator;
using Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using Webserver.Threads;
using System.Collections.Concurrent;
using System.Net;
using System.Linq;

namespace Webserver {
	class Program {
		public static Logger Log;
		public static List<string> WebPages;
		public static Dictionary<int, string> ErrorPages;
		public static List<Type> Endpoints;

		public static void Main(string[] _) {
			Logger.Init();
			Log = new Logger();
			Log.Info("Server is starting!");

			Log.Info("Loading configuration files...");
			Config.AddConfig(new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Webserver.DefaultConfig.json")));
			Config.SaveDefaultConfig();
			Dictionary<string, List<string>> Missing = Config.LoadConfig();

			//Display all missing and/or invalid config settings, if any.
			if (Missing.Count > 0) {
				Log.Fatal("Found one or more invalid or missing configuration settings;");
				foreach (string Key in Missing.Keys) {
					if (Missing[Key].Count == 0) { Console.Write(Key); }
					foreach (string Key2 in Missing[Key]) {
						Log.Fatal(Key + "." + Key2);
					}
				}
				Log.Fatal("Please check the configuration file. Press any key to exit.");
				Console.ReadKey();
				return;
			}

			//Thread count check
			if (Environment.ProcessorCount - 1 - (int)Config.GetValue("PerformanceSettings.WorkerThreadCount") < 2) {
				Log.Warning("Thread Count is set too high, which may impact performance. Consider lowering it.");
			}

			//Setup database
			Database.Init(Log);

			//Find all webpages and error pages, and store their filepaths.
			WebPages = CrawlWebFolder((string)Config.GetValue("WebserverSettings.wwwroot"));
			ErrorPages = CrawlErrorFolder((string)Config.GetValue("WebserverSettings.errorpages"));

			//Find all API endpoints
			Endpoints = new List<Type>();
			foreach (Type type in Assembly.GetExecutingAssembly().GetTypes()) {
				if (typeof(APIEndpoint).IsAssignableFrom(type) && !type.IsAbstract) {
					Endpoints.Add(type);
				}
			}

			//Create Queue and launch all threads
			using BlockingCollection<HttpListenerContext> Queue = new BlockingCollection<HttpListenerContext>();
			Thread ListenerThread = new Thread(() => Listener.Run(Log, Queue));
			ListenerThread.Start();
			List<Thread> WorkerThreads = new List<Thread>();
			for (int i = 0; i < (int)Config.GetValue("PerformanceSettings.WorkerThreadCount"); i++) {
				Thread Worker = new Thread(() => RequestWorker.Run(Log, Queue));
				Worker.Start();
				WorkerThreads.Add(Worker);
			}

			Console.ReadKey();
		}

		/// <summary>
		/// Recursively finds all web pages
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static List<string> CrawlWebFolder(string path) {
			//Throw an exception if the folder can't be found.
			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}

			List<string> Result = new List<string>();
			
			//Add files to list
			foreach(string Item in Directory.GetFiles(path)) {
				Result.Add(Item.Replace('\\', '/').ToLower());
			}
			//Crawl subfolders
			foreach(string Dir in Directory.GetDirectories(path)) {
				Result = Result.Concat(CrawlWebFolder(Dir)).ToList();
			}

			return Result;
		}

		public static Dictionary<int, string> CrawlErrorFolder(string path) {
			//Throw an exception if the folder can't be found.
			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}

			Dictionary<int, string> Result = new Dictionary<int, string>();

			//Add files to list
			foreach (string Item in Directory.GetFiles(path)) {
				if (Path.GetExtension(Item) == ".html" && int.TryParse(Path.GetFileNameWithoutExtension(Item), out int Code)) {
					Result.Add(Code, Item.Replace('\\', '/').ToLower());
				}

			}

			return Result;
		}
	}
}
