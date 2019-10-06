using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

namespace Webserver {
	static class Utils {
		/// <summary>
		/// Given a HttpStatusCode, returns the error page set for it.
		/// If no custom error page is set, a built-in default will be used instead.
		/// </summary>
		/// <param name="StatusCode">The HttpStatusCode</param>
		/// <returns></returns>
		public static string GetErrorPage(HttpStatusCode StatusCode, string Message = "An error occured, and the request couldn't be processed. Please try again.") {
			///Check if a custom error page exists
			if (!Program.ErrorPages.ContainsKey((int)StatusCode)) {
				//No custom page exists. Return the built-in page
				using StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Webserver.DefaultErrorPage.html"));
				return reader.ReadToEnd()
					.Replace("{ERRORTEXT}", StatusCode.ToString())
					.Replace("{STATUSCODE}", ((int)StatusCode).ToString())
					.Replace("{MSG}", Message);
			} else {
				//Return the custom page
				using StreamReader reader = File.OpenText(Program.ErrorPages[(int)StatusCode]);
				return reader.ReadToEnd();
			}
		}

		/// <summary>
		/// Sends data to the client, answering the request.
		/// </summary>
		/// <param name="Data">The data to be sent to the client.</param>
		/// <param name="Response">The Response object</param>
		/// <param name="StatusCode">The HttpStatusCode. Defaults to HttpStatusCode.OK (200)</param>
		public static void Send(object Data, HttpListenerResponse Response, HttpStatusCode StatusCode = HttpStatusCode.OK) {
			Response.StatusCode = (int)StatusCode;
			byte[] Buffer = Encoding.UTF8.GetBytes(Data?.ToString());
			Response.OutputStream.Write(Buffer, 0, Buffer.Length);
			Response.OutputStream.Close();
		}
	}
}
