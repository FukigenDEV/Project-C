using Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using System.IO;
using System.Data.SQLite;
using Webserver.Data;

namespace Webserver {
	class Database {
		public const string ConnectionString = "Data Source=Database.db;";

		public static void Init(Logger log) {
			SQLiteConnection.CreateFile("Database.db");
			using SQLiteConnection Connection = new SQLiteConnection(ConnectionString);
			Connection.Open();
			   
			Connection.Execute("CREATE TABLE Users (Username STRING, PasswordHash STRING)");

			Connection.Execute("INSERT INTO Users (Username, PasswordHash) VALUES ('Administrator', 'yeet')");
			Connection.Execute("INSERT INTO Users (Username, PasswordHash) VALUES ('Guest', 'yeet')");

			User usr = Connection.QueryFirst<User>("SELECT * FROM Users LIMIT 1");
			Console.WriteLine(usr.Username);
			Console.WriteLine(usr.PasswordHash);
		}
	}
}
