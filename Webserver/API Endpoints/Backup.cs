using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Configurator;
using Newtonsoft.Json.Linq;

namespace Webserver.API_Endpoints {

	/// <summary>
	/// Endpoint for managing backups
	/// </summary>
	[EndpointURL("/backup")]
	internal class BackupEndpoint : APIEndpoint {
		private readonly string BackupDir = (string)Config.GetValue("BackupSettings.BackupFolder");

		/// <summary>
		/// List and send backup files.
		/// If "name" parameter is given, the specified backup file will be offered as a download.
		/// If no parameter is given, a list of available backup files will be given.
		/// </summary>
		[PermissionLevel(PermLevel.Administrator)]
		public override void GET() {
			if ( RequestParams.ContainsKey("name") ) {
				string Name = RequestParams["name"][0];

				//Check if the specified file exists
				//If the name contains a dot, return a NotFound to prevent path traversal.
				if ( Name.Contains('.') || !File.Exists(BackupDir + "\\" + Name + ".zip") ) {
					Send(HttpStatusCode.NotFound);
					return;
				}
				Response.AddHeader("Content-disposition", "attachment; filename=" + Name + ".zip");
				Send(File.ReadAllBytes(BackupDir + "\\" + Name + ".zip"), "application/zip");

			} else {
				//No backup name was specified, so send
				List<FileInfo> BackupFiles = new List<FileInfo>(new DirectoryInfo(BackupDir).GetFiles());
				JArray Names = new JArray();
				foreach ( FileInfo File in BackupFiles ) {
					Names.Add(Path.GetFileNameWithoutExtension(File.Name));
				}
				Send(Names.ToString(), HttpStatusCode.OK, "application/json");
			}
		}

		/// <summary>
		/// Initiate a manual backup. Requires no additional data.
		/// </summary>
		[PermissionLevel(PermLevel.Administrator)]
		public override void POST() {
			BackupManager.CreateManualBackup();
			Send(HttpStatusCode.OK);
		}
	}
}
