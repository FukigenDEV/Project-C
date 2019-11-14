﻿using Configurator;
using Dapper;
using Dapper.Contrib.Extensions;
using Logging;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using Webserver.Data;

namespace Webserver {
	static class Database {
		public const string ConnectionString = "Data Source=Database.db;foreign keys=true";
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

            Connection.Execute("CREATE TABLE IF NOT EXISTS Companies (" +
            "ID					INTEGER PRIMARY KEY," +
            "Name				STRING NOT NULL," +
            "Street             STRING," +
            "HouseNumber        INTEGER," +
            "PostCode           STRING," +
            "City               STRING," +
            "Country            STRING," +
            "PhoneNumber        STRING," +
            "Email              STRING" +
            ")");

            Connection.Execute("CREATE TABLE IF NOT EXISTS Departments (" +
            "ID					INTEGER PRIMARY KEY," +
            "Name				STRING NOT NULL," +
            "Description        STRING" +
            ")");

            Connection.Execute("CREATE TABLE IF NOT EXISTS Notes (" +
            "ID                 INTEGER PRIMARY KEY," +
            "Title              STRING NOT NULL," +
            "Text               STRING," +
            "Author             INTEGER NOT NULL," +
            "FOREIGN KEY(Author) REFERENCES Users(ID) ON DELETE CASCADE" +
            ")");

            Connection.Execute("CREATE TABLE IF NOT EXISTS Permissions (" +
				"User				INTEGER NOT NULL," +
				"Permission			INTEGER NOT NULL," +
				"Department			INTEGER," +
				"PRIMARY KEY (User, Department)," +
				"FOREIGN KEY(User) REFERENCES Users(ID) ON DELETE CASCADE," +
				"FOREIGN KEY(Department) REFERENCES Departments(ID) ON DELETE CASCADE" +
			")");

			Connection.Execute("CREATE TABLE IF NOT EXISTS Users (" +
				"ID					INTEGER PRIMARY KEY, " +
				"Email				STRING NOT NULL, " +
				"PasswordHash		STRING NOT NULL," +
				"Firstname			STRING," +
				 "MiddleInitial		STRING," +
				 "Lastname			STRING," +
				 "Function			STRING," +
				 "WorkPhone			STRING," +
				 "MobilePhone		STRING," +
				 "Birthday			STRING," +
				 "Country			STRING," +
				 "Address			STRING," +
				 "Postcode			STRING," +
				 "FOREIGN KEY(Function) REFERENCES Functions(Name) ON UPDATE CASCADE" +
			")");

			Connection.Execute("CREATE TABLE IF NOT EXISTS Sessions (" +
				"ID					INTEGER PRIMARY KEY," +
				"User				INTEGER NOT NULL," +
				"SessionID			STRING NOT NULL," +
				"Token				INTEGER NOT NULL," +
				"RememberMe			INTEGER NOT NULL," +
				"FOREIGN KEY(User)	REFERENCES Users(ID) ON DELETE CASCADE" +
			")");

			Connection.Execute("CREATE TABLE IF NOT EXISTS GenericTableConfiguration (" +
				"TableName			STRING PRIMARY KEY," +
				"ReqValidation		INTEGER," +
				"Department			INTEGER NOT NULL," +
				"FOREIGN KEY(Department) REFERENCES Departments(ID) ON UPDATE CASCADE" +
			")");

			Department AdministratorDept = Connection.Get<Department>(1);
			if(AdministratorDept == null) {
				Connection.Insert(new Department("Administrators", "Department for Administrators"));
			}

			//Set the Administrator account password. Create the Administrator account first if it doesn't exist already.
			User Administrator = Connection.Get<User>(1);
			if (Administrator == null) {
				Administrator = new User("Administrator", (string)Config.GetValue("AuthenticationSettings.AdministratorPassword"));
				Connection.Insert(Administrator);
				Administrator.SetPermissionLevel(Connection, PermLevel.Administrator, 1);
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
