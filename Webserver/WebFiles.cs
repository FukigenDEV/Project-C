﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Configurator;
using PrettyConsole;

namespace Webserver {
	public static class WebFiles {
		public static List<string> WebPages;
		public static Dictionary<int, string> ErrorPages;
		private static readonly Logger Log = Program.Log;

		/// <summary>
		/// Crawls through the wwwroot and errorpages folders to recursively find all available web pages and other files.
		/// </summary>
		public static void Init() {
			WebPages = CrawlWebFolder((string)Config.GetValue("WebserverSettings.wwwroot"));
			ErrorPages = CrawlErrorFolder((string)Config.GetValue("WebserverSettings.errorpages"));
		}

		/// <summary>
		/// Recursively finds all files in the specified directory.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static List<string> CrawlWebFolder(string path) {
			List<string> Result = new List<string>();

			//If the folder doesn't exist, create it and return an empty list.
			if ( !Directory.Exists(path) ) {
				Directory.CreateDirectory(path);
				return Result;
			}

			//Add files to list
			foreach ( string Item in Directory.GetFiles(path) ) {
				Result.Add(Item.Replace('\\', '/'));
			}
			//Crawl subfolders
			foreach ( string Dir in Directory.GetDirectories(path) ) {
				Result = Result.Concat(CrawlWebFolder(Dir)).ToList();
			}

			return Result;
		}

		/// <summary>
		/// Recursively finds all error pages in the specified directory
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static Dictionary<int, string> CrawlErrorFolder(string path) {
			Dictionary<int, string> Result = new Dictionary<int, string>();

			//If the folder doesn't exist, create it and return an empty list.
			if ( !Directory.Exists(path) ) {
				Directory.CreateDirectory(path);
				return Result;
			}

			//Add files to list
			foreach ( string Item in Directory.GetFiles(path) ) {
				if ( Path.GetExtension(Item) == ".html" && int.TryParse(Path.GetFileNameWithoutExtension(Item), out int Code) ) {
					if ( !Enum.IsDefined(typeof(HttpStatusCode), Code) ) {
						Log.Warning("Skipping invalid errorpage at " + path + ": No such HTTP Status Code");
					}
					Result.Add(Code, Item.Replace('\\', '/').ToLower());
				}
			}

			return Result;
		}
	}
}
