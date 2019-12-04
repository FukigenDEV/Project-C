using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;
using Configurator;
using Dapper;
using Logging;
using Webserver.Data;

namespace Webserver.Threads {
	internal class MaintenanceThread {
		public Logger Log;
		/// <summary>
		/// Run all maintenance tasks
		/// </summary>
		/// <param name="_"></param>
		public void Run(object _) {
			Log.Debug("Executing maintenance tasks.");
			DateTime Started = DateTime.Now;
			SQLiteConnection Connection = Database.CreateConnection();

			//Session cleanup
			Log.Debug("Cleaning up expired user sessions...");
			List<string> ToClean = new List<string>();
			foreach ( dynamic Entry in Connection.Query("SELECT SessionID, Token, RememberMe FROM Sessions") ) {
				if ( Session.GetRemainingTime((long)Entry.Token, (int)Entry.RememberMe != 0) < 0 ) {
					ToClean.Add(Entry.SessionID);
				}
			}
			if ( ToClean.Count > 0 ) {
				string SQL = "DELETE FROM Sessions WHERE SessionID IN ('" + string.Join("\',\'", ToClean) + "')";
				Connection.Execute(SQL);
			}
			Log.Debug("Cleaned up " + ToClean.Count + " sessions.");

			//Create backup
			BackupManager.CreateScheduledBackup();

			//Close the database connection
			Connection.Close();

			Log.Debug("Maintenance complete. Took " + (int)( DateTime.Now - Started ).TotalMilliseconds + "ms");
		}
	}
}
