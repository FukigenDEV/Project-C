﻿using Configurator;
using Dapper;
using Dapper.Contrib.Extensions;
using Logging;
using System.Data.SQLite;
using System.IO;
using Webserver.Data;

namespace Webserver {
	static class Database {
		public const string ConnectionString = "Data Source=Database.db;";
		private static readonly Logger Log = Program.Log;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="log"></param>
		public static void Init() {
			Log.Info("Initializing database...");

			//Create the database if it doesn't exist already.
			if (!File.Exists("Database.db")) {
				SQLiteConnection.CreateFile("Database.db");
			}

			//Connect to it
			using SQLiteConnection Connection = createConnection();

			//Create tables if they don't already exist.
			Connection.Execute("CREATE TABLE IF NOT EXISTS Functions (" +
				"Name				STRING PRIMARY KEY" +
			")");

			Connection.Execute("CREATE TABLE IF NOT EXISTS Departments (" +
				"ID					INTEGER PRIMARY KEY," +
				"Name				STRING NOT NULL" +
			")");

			//TODO: Perms aren't deleted after user account or dept is deleted
			Connection.Execute("CREATE TABLE IF NOT EXISTS Permissions (" +
				"User				INTEGER REFERENCES Users(ID) ON UPDATE CASCADE," +
				"Permission			INTEGER," +
				"Department			INTEGER REFERENCES Departments(ID) ON UPDATE CASCADE," +
				"PRIMARY KEY (User, Department)" +
			")");

			Connection.Execute("CREATE TABLE IF NOT EXISTS Users (" +
				"ID					INTEGER PRIMARY KEY, " +
				"Email				STRING NOT NULL, " +
				"PasswordHash		STRING NOT NULL," +
				"Firstname			STRING," +
				 "MiddleInitial		STRING," +
				 "Lastname			STRING," +
				 "Function			STRING REFERENCES Function(Name) ON UPDATE CASCADE," +
				 "WorkPhone			STRING," +
				 "MobilePhone		STRING," +
				 "Birthday			STRING," +
				 "Country			STRING," +
				 "Address			STRING," +
				 "Postcode			STRING" +
			")");

			//TODO: Sessions aren't deleted after user account is deleted
			Connection.Execute("CREATE TABLE IF NOT EXISTS Sessions (" +
				"ID					INTEGER PRIMARY KEY," +
				"SessionID			STRING NOT NULL," +
				"User				INTEGER REFERENCES Users(ID) NOT NULL," +
				"Token				INTEGER NOT NULL," +
				"RememberMe			INTEGER NOT NULL" +
			")");

			//Set the Administrator account password. Create the Administrator account first if it doesn't exist already.
			User Administrator = Connection.Get<User>(1);
			if (Administrator == null) {
				Administrator = new User("Administrator", (string)Config.GetValue("AuthenticationSettings.AdministratorPassword"));
				Connection.Insert(Administrator);
				Administrator.SetPermissionLevel(Connection, PermLevel.Administrator, 0);
			} else {
				Administrator.ChangePassword((string)Config.GetValue("AuthenticationSettings.AdministratorPassword"));
				Connection.Update<User>(Administrator);
			}
		}

		public static SQLiteConnection createConnection() {
			SQLiteConnection Connection = new SQLiteConnection(ConnectionString);
			Connection.Open();
			return (Connection);
		}
	}
}
