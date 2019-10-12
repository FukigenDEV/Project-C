using Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using System.IO;
using System.Data.SQLite;
using Webserver.Data;
using Configurator;
using Dapper.Contrib.Extensions;

namespace Webserver {
	static class Database {
		public const string ConnectionString = "Data Source=Database.db;";

		/// <summary>
		/// 
		/// </summary>
		/// <param name="log"></param>
		public static void Init(Logger log) {
			log.Info("Initializing database...");

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

			Connection.Execute("CREATE TABLE IF NOT EXISTS Permissions (" +
				"User				INTEGER REFERENCES Users(ID) ON UPDATE CASCADE," +
				"Department			INTEGER REFERENCES Departments(ID) ON UPDATE CASCADE," +
				"Member				INTEGER," +
				"Manager			INTEGER," +
				"Administrator		INTEGER" +
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

			//Set the Administrator account password. Create the Administrator account first if it doesn't exist already.
			User Administrator = Connection.Get<User>(1);
			if(Administrator == null) {
				Connection.Insert(new User("Administrator", (string)Config.GetValue("AuthenticationSettings.AdministratorPassword")));
				Connection.Execute("INSERT INTO Permissions (User, Member, Manager, Administrator) VALUES (1, 0, 0, 1)");
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
