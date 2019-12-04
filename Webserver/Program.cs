using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using Configurator;
using Logging;
using Webserver.Threads;

namespace Webserver {
	internal class Program {
		public static Logger Log;
		public static List<Type> Endpoints;
		public static List<string> CORSAddresses = new List<string>();

		public static void Main() {
			//Initialize logger
			Logger.Init();
			Log = new Logger();
			Utils.Log = Log;
			Log.Info("Server is starting!");

			//Create default config
			Log.Info("Loading configuration files...");
			Config.AddConfig(new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Webserver.DefaultConfig.json")));
			Config.SaveDefaultConfig();

			//Load the configuration file
			Dictionary<string, List<string>> Missing = Config.LoadConfig();

			//Display all missing and/or invalid config settings, if any.
			if ( Missing == null ) {
				Log.Fatal("The config file could not be read. Ensure that it is a valid JSON file. Press any key to exit.");
				Console.ReadKey();
				return;
			} else if ( Missing.Count > 0 ) {
				Log.Fatal("Found one or more invalid or missing configuration settings;");
				foreach ( string Key in Missing.Keys ) {
					if ( Missing[Key].Count == 0 ) { Console.Write(Key); }
					foreach ( string Key2 in Missing[Key] ) {
						Log.Fatal(Key + "." + Key2);
					}
				}
				Log.Fatal("Please check the configuration file. Press any key to exit.");
				Console.ReadKey();
				return;
			}

			//Check CORS addresses
			CORSAddresses = Utils.ParseAddresses(Config.GetValue("ConnectionSettings.AccessControl").ToObject<List<string>>());
			List<string> Addresses = Utils.ParseAddresses(Configurator.Config.GetValue("ConnectionSettings.ServerAddresses").ToObject<List<string>>());
			CORSAddresses = CORSAddresses.Concat(Addresses).ToList();

			//Run inits
			Database.Init();
			WebFiles.Init();
			Redirect.Init();

			//Find all API endpoints
			Endpoints = new List<Type>();
			foreach ( Type type in Assembly.GetExecutingAssembly().GetTypes() ) {
				if ( typeof(APIEndpoint).IsAssignableFrom(type) && !type.IsAbstract ) {
					Endpoints.Add(type);
				}
			}

			//Create Queue and launch listener
			using BlockingCollection<HttpListenerContext> Queue = new BlockingCollection<HttpListenerContext>();
			Thread ListenerThread = new Thread(() => Listener.Run(Log, Queue));
			ListenerThread.Start();

			//Launch worker threads
			List<Thread> WorkerThreads = new List<Thread>();
			for ( int i = 0; i < (int)Config.GetValue("PerformanceSettings.WorkerThreadCount"); i++ ) {
				RequestWorker Worker = new RequestWorker(Log, Queue);
				Thread WorkerThread = new Thread(new ThreadStart(Worker.Run));
				WorkerThread.Start();
				WorkerThreads.Add(WorkerThread);
			}

			//Launch maintenance thread
			Timer Maintenance = new Timer(new MaintenanceThread { Log = Log }.Run, null, 0, 3600 * 1000);

			//Wait for an exit command, then exit.
			Log.Info("Type 'Exit' to exit.");
			while ( Console.ReadLine().ToLower() != "exit" ) ;
			Environment.Exit(0);
		}
	}
}
