﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using Newtonsoft.Json.Linq;
using PrettyConsole;

namespace Webserver {
	/// <summary>
	/// Static class containing methods that aren't big enough to warrant their own files, and are unique enough that they can't be grouped with anything else.
	/// </summary>
	internal static class Utils {
		public static Logger Log = Program.Log;

		/// <summary>
		/// Given a HttpStatusCode, returns the error page set for it.
		/// If no custom error page is set, a built-in default will be used instead.
		/// </summary>
		/// <param name="StatusCode">The HttpStatusCode</param>
		/// <returns></returns>
		public static string GetErrorPage(HttpStatusCode StatusCode, string Message = "An error occured, and the request couldn't be processed. Please try again.") {
			///Check if a custom error page exists
			if ( !WebFiles.ErrorPages.ContainsKey((int)StatusCode) ) {
				//No custom page exists. Return the built-in page
				using StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Webserver.DefaultErrorPage.html"));
				return reader.ReadToEnd()
					.Replace("{ERRORTEXT}", StatusCode.ToString())
					.Replace("{STATUSCODE}", ( (int)StatusCode ).ToString())
					.Replace("{MSG}", Message);
			} else {
				//Return the custom page
				using StreamReader reader = File.OpenText(WebFiles.ErrorPages[(int)StatusCode]);
				return reader.ReadToEnd();
			}
		}

		/// <summary>
		/// Get the current UNIX timestamp
		/// </summary>
		/// <returns></returns>
		public static int GetUnixTimestamp() => (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

		/// <summary>
		/// Given a list of addresses, adds a "http://" prefix and "/" suffix where necessary.
		/// </summary>
		/// <param name="Addresses"></param>
		/// <returns></returns>
		public static List<string> ParseAddresses(List<string> Addresses) {
			List<string> Result = new List<string>();
			foreach ( string Address in Addresses ) {
				if ( Address == "*" ) {
					continue;
				}

				//There has to be a better way!
				string addr;
				if ( !Address.StartsWith("http://") ) {
					addr = "http://" + Address;
				} else {
					addr = Address;
				}

				if ( addr[^1] != '/' ) {
					addr += '/';
				}
				Result.Add(addr);
			}
			return Result;
		}

		public static Dictionary<string, List<string>> NameValueToDict(NameValueCollection Data) {
			Dictionary<string, List<string>> Result = new Dictionary<string, List<string>>();
			foreach ( string key in Data ) {
				Result.Add(key?.ToLower() ?? "null", new List<string>(Data[key]?.Split(',')));
			}
			return Result;
		}
	}

	/// <summary>
	/// Permission level enum
	/// </summary>
	public enum PermLevel {
		User,
		DeptMember,
		Manager,
		Administrator
	}

	/// <summary>
	/// JObject extension class
	/// </summary>
	public static class JObjectExtension {
		/// <summary>
		/// Tries to get the JToken with the specified property name. Returns false if the JToken can't be cast to the specified type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="propertyName"></param>
		/// <param name="Value"></param>
		/// <returns></returns>
		public static bool TryGetValue<T>(this JObject obj, string propertyName, out JToken Value) {
			bool Found = obj.TryGetValue(propertyName, out Value);
			if ( !Found ) {
				return false;
			}
			try {
				Value.ToObject<T>();
#pragma warning disable CA1031 // Silence "Do not catch general exception types" message.
			} catch ( ArgumentException ) {
				return false;
			} catch ( InvalidCastException ) {
				return false;
			}
#pragma warning restore CA1031
			return true;
		}
	}
}
