using Configurator;
using Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Webserver {
	static class Redirect {
		private static readonly Dictionary<string, string> RedirectionDict = new Dictionary<string, string>();
		private static readonly Logger Log = Program.Log;

		public static void Init() {
			string wwwroot = (string)Config.GetValue("WebserverSettings.wwwroot");

			if (!File.Exists("Redirects.config")) {
				using StreamWriter RedirectsFile = File.CreateText("Redirects.config");
				if (File.Exists(wwwroot + "/index.html")) {
					RedirectsFile.WriteLine("/ => /index.html");
				}
			}

			ParseRedirectFile("Redirects.config");
		}

		/// <summary>
		/// Given a path, returns the path that it redirects to. If the path doesn't redirect anywhere, the same path is returned.
		/// Returns null if the path redirects in a way that would cause a loop.
		/// </summary>
		/// <param name="Path"></param>
		/// <returns></returns>
		public static string Resolve(string Path) {
			Stack<string> ResolveStack = new Stack<string>();
			while (RedirectionDict.ContainsKey(Path)) {
				if (ResolveStack.Contains(Path)) {
					return null;
				}

				ResolveStack.Push(Path);
				Path = RedirectionDict[Path];
			}
			return Path;
		}

		/// <summary>
		/// Parses a redirection file.
		/// </summary>
		/// <param name="Path"></param>
		public static void ParseRedirectFile(string Path) {
			string wwwroot = (string)Config.GetValue("WebserverSettings.wwwroot");

			if (!File.Exists(Path)) {
				throw new FileNotFoundException();
			}
			using StreamReader Reader = File.OpenText(Path);
			string Line;
			int LineCount = 1;
			while ((Line = Reader.ReadLine()) != null) {
				string[] LineContents = Line.Split(" => ");

				//Check if the line is valid
				if(LineContents.Length != 2) {
					Log.Warning("Skipping invalid redirection in " + Path + " (line: " + LineCount + "): Invalid format");
					continue;
				}

				//Check if the source URI is valid
				Regex ex = new Regex("(\\/{1}[A-z0-9-._~:?#[\\]@!$&'()*+,;=]{1,}){1,}");
				if (!ex.IsMatch(LineContents[0]) && LineContents[0] != "/") {
					Log.Warning("Skipping invalid redirection in " + Path + " (line: " + LineCount + "): Incorrect source URI");
				}

				//Check if the destination is valid
				if (!ex.IsMatch(LineContents[1]) && LineContents[1] != "/") {
					Log.Warning("Skipping invalid redirection in " + Path + " (line: " + LineCount + "): Incorrect destination URI");
				}

				//Add to dict
				RedirectionDict.Add(LineContents[0], LineContents[1]);

				LineCount++;
			}
		}
	}
}
