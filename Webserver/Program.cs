using Configurator;
using Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Webserver {
	class Program {
		public static void Main(string[] _) {
			Logger.Init();
			Logger Log = new Logger();

			Config.AddConfig(new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Webserver.DefaultConfig.json")));
			Config.SaveDefaultConfig();
			Dictionary<string, List<string>> Missing = Config.LoadConfig();

			//TODO Properly show invalid config options
			foreach (string Key in Missing.Keys) {
				if (Missing[Key].Count == 0) { Console.Write(Key); }
				foreach (string Key2 in Missing[Key]) {
					Console.WriteLine(Key + "." + Key2);
				}
			}

			Console.ReadKey();
		}
	}
}
