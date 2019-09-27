using Logging;
using System;
using System.Data.SQLite;
using System.IO;

namespace DBManager{
	public static class DatabaseManager{
		private static SQLiteConnection Connection;
		private const string ConnectionString = "Data Source=Database.sqlite;Version=3;";

		public static void Init(Logger Log) {
			//Create DB if it doesn't already exist.
			if (!File.Exists("Database.sqlite")) {
				//Create a database and open it
				Log.Info("No existing database was found. A new database will be created.");
				SQLiteConnection.CreateFile("Database.sqlite");
				Connection = new SQLiteConnection(ConnectionString);
				Connection.Open();

				//Create user table and default admin account
				string SQL = @"CREATE TABLE Users (
					username VARCHAR(255),
					password VARCHAR(255)
				)";
				using (SQLiteCommand Command = new SQLiteCommand(SQL, Connection)) {
					Command.ExecuteNonQuery();
				}

				SQL = @"INSERT INTO Users (username, password) values ('Administrator', 'Password')";
				using (SQLiteCommand Command = new SQLiteCommand(SQL, Connection)) {
					Command.ExecuteNonQuery();
				}


			} else {
				//Open the database
				Connection = new SQLiteConnection(ConnectionString);
				Connection.Open();

				string SQL = @"SELECT * FROM USERS";
				using SQLiteCommand Command = new SQLiteCommand(SQL, Connection);
				using SQLiteDataReader Reader = Command.ExecuteReader();
				while (Reader.Read()) {
					Console.WriteLine(Reader["username"] + " " + Reader["password"]);
				}
			}
		}
	}
}
