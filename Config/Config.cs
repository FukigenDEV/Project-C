using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Configurator {
	public static class Config {
		private static string DefaultConfigString;
		private static JObject LoadedConfig;

		/// <summary>
		/// Saves a default config file. If the specified file already exists, nothing will happen unless Overwrite is true.
		/// </summary>
		/// <param name="File"></param>
		public static void SaveDefaultConfig(string Path = "Config.json", bool Overwrite = false) {
			if ( !File.Exists(Path) || Overwrite ) {
				using StreamWriter ConfigFile = File.CreateText(Path);
				ConfigFile.Write(DefaultConfigString);
			}
		}

		/// <summary>
		/// Load the specified config file.
		/// </summary>
		/// <param name="File">Path to the config file.</param>
		public static Dictionary<string, List<string>> LoadConfig(string Path = "Config.json") {
			if ( !File.Exists(Path) ) {
				throw new FileNotFoundException(Path + " does not exist.");
			}

			//Parse JSON
			JObject Config;
			try {
				Config = JObject.Parse(File.ReadAllText(Path));
#pragma warning disable CA1031 // Silence "Do not catch general exception types" message.
			} catch ( JsonReaderException e) {
				Console.WriteLine(e.Message);
				return null;
			}
#pragma warning restore CA1031


			//Get differences, if any
			Dictionary<string, List<string>> Missing = new Dictionary<string, List<string>>();
			Dictionary<string, Dictionary<string, object>> DefaultConfigKeys = JObject.Parse(DefaultConfigString).ToObject<Dictionary<string, Dictionary<string, object>>>();
			Dictionary<string, Dictionary<string, object>> ConfigKeys = Config.ToObject<Dictionary<string, Dictionary<string, object>>>();
			foreach ( string Key in DefaultConfigKeys.Keys ) {
				if ( !ConfigKeys.ContainsKey(Key) ) {
					Missing.Add(Key, new List<string>(DefaultConfigKeys[Key].Keys.ToList()));
				} else {
					foreach ( string Key2 in DefaultConfigKeys[Key].Keys ) {
						if ( !ConfigKeys[Key].ContainsKey(Key2) || ConfigKeys[Key][Key2].GetType() != DefaultConfigKeys[Key][Key2].GetType() ) {
							Missing.TryAdd(Key, new List<string>());
							Missing[Key].Add(Key2);
						}
					}
				}
			}

			LoadedConfig = Config;

			return ( Missing );
		}

		/// <summary>
		/// Retrieves a configuration setting.
		/// </summary>
		/// <param name="Setting">The setting to retrieve.</param>
		/// <returns></returns>
		public static dynamic GetValue(string Setting) {
			string[] Pair = Setting.Split('.');
			return LoadedConfig.GetValue(Pair[0]).ToObject<JObject>().GetValue(Pair[1]).ToObject(LoadedConfig.GetValue(Pair[0]).ToObject<JObject>().GetValue(Pair[1]).GetType());
		}

		/// <summary>
		/// Adds external configuration (eg. from a plugin or module) to the global configuration.
		/// After adding all external configs, run Config.LoadConfig()
		/// </summary>
		/// <param name="ExtConfig"></param>
		public static void AddConfig(StreamReader Reader) {
			if ( DefaultConfigString == null ) {
				DefaultConfigString = Reader.ReadToEnd();
			} else {
				string NewConfig = Reader.ReadToEnd().Replace("\r\n", "\n").Remove(0, 1);
				DefaultConfigString = DefaultConfigString.Replace("\r\n", "\n");
				DefaultConfigString = DefaultConfigString.Remove(DefaultConfigString.Length - 2) + "," + NewConfig;
			}
		}
	}
}
