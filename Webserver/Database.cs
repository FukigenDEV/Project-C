using Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using System.IO;
using System.Data.SQLite;
using Webserver.Data;

namespace Webserver {
	static class Database {
		public const string ConnectionString = "Data Source=Database.db;";

		public static void Init(Logger log) {
			SQLiteConnection.CreateFile("Database.db");
			SQLiteConnection Connection = createConnection();
			   
			Connection.Execute("CREATE TABLE Users (Username STRING, PasswordHash STRING)");
		}

		public static SQLiteConnection createConnection() {
			SQLiteConnection Connection = new SQLiteConnection(ConnectionString);
			Connection.Open();
			return (Connection);
		}
	}
}
