using Logging;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

namespace Webserver {
	/// <summary>
	/// Static class containing methods that aren't big enough to warrant their own files, and are unique enough that they can't be grouped with anything else.
	/// </summary>
	static class Utils {
		public static Logger Log;

		/// <summary>
		/// Given a HttpStatusCode, returns the error page set for it.
		/// If no custom error page is set, a built-in default will be used instead.
		/// </summary>
		/// <param name="StatusCode">The HttpStatusCode</param>
		/// <returns></returns>
		public static string GetErrorPage(HttpStatusCode StatusCode, string Message = "An error occured, and the request couldn't be processed. Please try again.") {
			///Check if a custom error page exists
			if (!WebFiles.ErrorPages.ContainsKey((int)StatusCode)) {
				//No custom page exists. Return the built-in page
				using StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Webserver.DefaultErrorPage.html"));
				return reader.ReadToEnd()
					.Replace("{ERRORTEXT}", StatusCode.ToString())
					.Replace("{STATUSCODE}", ((int)StatusCode).ToString())
					.Replace("{MSG}", Message);
			} else {
				//Return the custom page
				using StreamReader reader = File.OpenText(WebFiles.ErrorPages[(int)StatusCode]);
				return reader.ReadToEnd();
			}
		}

		/// <summary>
		/// Sends data to the client in the form of a byte array.
		/// </summary>
		/// <param name="Data">The data to be sent to the client.</param>
		/// <param name="Response">The Response object</param>
		/// <param name="StatusCode">The HttpStatusCode. Defaults to HttpStatusCode.OK (200)</param>
		public static void Send(HttpListenerResponse Response, byte[] Data = null, HttpStatusCode StatusCode = HttpStatusCode.OK) {
			if (Data == null) {
				Data = Array.Empty<byte>();
			}

			try {
				Response.StatusCode = (int)StatusCode;
				Response.OutputStream.Write(Data, 0, Data.Length);
				Response.OutputStream.Close();
			} catch (HttpListenerException e) {
				Log.Error("Failed to send data to host: " + e.Message);
			}

		}
		/// <summary>
		/// Sends data to the client, answering the request.
		/// </summary>
		/// <param name="Data">The data to be sent to the client.</param>
		/// <param name="Response">The Response object</param>
		/// <param name="StatusCode">The HttpStatusCode. Defaults to HttpStatusCode.OK (200)</param>

		public static void Send(HttpListenerResponse Response, object Data = null, HttpStatusCode StatusCode = HttpStatusCode.OK) {
			if (Data == null) {
				Data = "";
			}
			byte[] Buffer = Encoding.UTF8.GetBytes(Data.ToString());
			Send(Response, Buffer, StatusCode);
		}

		/// <summary>
		/// Get the current UNIX timestamp
		/// </summary>
		/// <returns></returns>
		public static int GetUnixTimestamp() => (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;


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
		public static bool TryGetValue<T>(this JObject obj, string propertyName, out JToken Value) where T : class {
			bool Found = obj.TryGetValue(propertyName, out Value);
			if (!Found) {
				return false;
			}
			try {
				Value.ToObject<T>();
			#pragma warning disable CA1031 // Silence "Do not catch general exception types" message.
			} catch (ArgumentException) {
				return false;
			} catch (InvalidCastException) {
				return false;
			}
			#pragma warning restore CA1031
			return true;
		}
	}
}
