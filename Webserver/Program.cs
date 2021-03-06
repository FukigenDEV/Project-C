﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using Configurator;
using PrettyConsole;
using Webserver.Threads;

namespace Webserver {
	public static class Program {
		public static Logger Log { get; set; }
		public static List<Type> Endpoints;
		public static List<string> CORSAddresses = new List<string>();

		public static void Main() {
			//Initialize logger
			LogTab Tab = new LogTab("General");
			Log = Tab.GetLogger();
			RequestWorker.RequestLoggerTab = new LogTab("Workers");
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
			SQLiteConnection Connection = Database.Init();
			WebFiles.Init();
			Redirect.Init();

			//Find all API endpoints
			DiscoverEndpoints();

			//Create Queue and launch listener
			Thread ListenerThread = new Thread(() => Listener.Run());
			ListenerThread.Start();

			//Create performance tab + watchers
			MonitorTab pTab = new MonitorTab("PerfMon");
			RequestWorker.RequestTimeWatcher = pTab.CreateNumWatcher("Request time", ShowMin: true, ShowAverage: true, ShowMax: true);
			Listener.QueueSizeWatcher = pTab.CreateNumWatcher("Queue size", ShowMax: true);

			//Launch worker threads
			List<Thread> WorkerThreads = new List<Thread>();
			for ( int i = 0; i < (int)Config.GetValue("PerformanceSettings.WorkerThreadCount"); i++ ) {
				RequestWorker Worker = new RequestWorker((SQLiteConnection)Connection.Clone());
				Thread WorkerThread = new Thread(new ThreadStart(Worker.Run)) {
					Name = "RequestWorker" + i
				};
				WorkerThread.Start();
				WorkerThreads.Add(WorkerThread);
			}

			//Launch maintenance thread
			Timer Maintenance = new Timer(new MaintenanceThread { Log = Log }.Run, null, 0, 3600 * 1000);

			//Wait for an exit command, then exit.			
			foreach(Thread Worker in WorkerThreads) {
				Worker.Join();
			}
		}

		public static void DiscoverEndpoints() {
			Endpoints = new List<Type>();
			foreach ( Type type in Assembly.GetExecutingAssembly().GetTypes() ) {
				if ( typeof(APIEndpoint).IsAssignableFrom(type) && !type.IsAbstract ) {
					Endpoints.Add(type);
				}
			}
		}
	}
}
