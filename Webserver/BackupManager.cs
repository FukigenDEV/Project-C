using Configurator;
using Logging;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Webserver {
	public static class BackupManager {
		/// <summary>
		/// DateTime string format
		/// </summary>
		private const string Format = "yyyy-MM-dd_HH-mm";
		private static readonly Logger Log = Program.Log;

		/// <summary>
		/// Creates a new backup if a backup hasn't been created recently. Otherwise, this function does nothing.
		/// </summary>
		public static void CreateScheduledBackup() {
			string BackupDir = (string)Config.GetValue("BackupSettings.BackupFolder");
			Directory.CreateDirectory(BackupDir);
			FileInfo LastBackupFile = new DirectoryInfo(BackupDir).GetFiles().OrderByDescending(f => f.LastWriteTime).FirstOrDefault();
			if(LastBackupFile == null || (DateTime.Now - LastBackupFile.CreationTime).TotalSeconds > (int)Config.GetValue("BackupSettings.BackupInterval")) {
				CreateManualBackup();
			} else {
				Log.Debug("No backup necessary");
			}
		}

		/// <summary>
		/// Creates a new backup and stores it in the backup folder specified in the server configuration file.
		/// </summary>
		/// <returns>A string containing the newly created backup file</returns>
		public static string CreateManualBackup() {
			DateTime Started = DateTime.Now;
			Log.Debug("Starting backup procedure.");

			//Create backup dir if it doesn't exist already
			string BackupDir = (string)Config.GetValue("BackupSettings.BackupFolder");
			if (Directory.Exists(BackupDir + "\\temp")) {
				Directory.Delete(BackupDir + "\\temp");
			}
			Directory.CreateDirectory("Backups\\temp");

			//Backup database
			Log.Debug("Cloning database...");
			using (SQLiteConnection BackupDBConnection = new SQLiteConnection("Data Source=" + BackupDir + "\\temp\\Database.db")) {
				using SQLiteConnection Connection = Database.CreateConnection();
				BackupDBConnection.Open();
				Connection.BackupDatabase(BackupDBConnection, "main", "main", -1, BackupLog, 1000);
			}

			//Backup config files
			File.Copy("Config.json", BackupDir + "\\temp\\Config.json");
			File.Copy("Redirects.config", BackupDir + "\\temp\\Redirects.config");

			string Timestamp = DateTime.Now.ToString(Format);
			int FileCount = Directory.GetFiles(BackupDir, "Backup_" + Timestamp + "_*.zip").Length;
			ZipFile.CreateFromDirectory(BackupDir + "\\temp", BackupDir + "\\Backup_" + Timestamp + "_" + FileCount + ".zip");

			//Delete temp folder
			Directory.Delete(BackupDir + "\\temp", true);

			Log.Debug("Backup complete. Took " + (int)(DateTime.Now - Started).TotalMilliseconds + "ms");
			return BackupDir + "\\Backup_" + Timestamp + "_" + FileCount + ".zip";
		}

		/// <summary>
		/// SQLiteBackupCallback that will be called by the BackupDatabase function when creating a backup of the database.
		/// </summary>
		/// <returns></returns>
		private static bool BackupLog(SQLiteConnection source, string sourceName, SQLiteConnection destination, string destinationName, int pages, int remainingPages, int totalPages, bool retry) {
			Log.Debug("Copying " + totalPages + " pages at a rate of " + pages + " per step. " + remainingPages + " pages remaining");
			return true;
		}
	}
}
