using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using Configurator;
using Dapper;
using Dapper.Contrib.Extensions;
using Logging;
using Newtonsoft.Json.Linq;
using Webserver.Data;
using static Dapper.SqlMapper;

namespace Webserver {
	public static class Database {
		private static readonly Logger Log = Program.Log;

		/// <summary>
		/// Initialize the database, inserting all data required for first start-up
		/// </summary>
		/// <param name="log"></param>
		public static void Init() {
			Log?.Info("Initializing database...");

			//Create the database if it doesn't exist already.
			if ( !File.Exists("Database.db") ) {
				SQLiteConnection.CreateFile("Database.db");
			}

			//Connect to the database
			using SQLiteConnection Connection = CreateConnection();

			//Create tables if they don't already exist.
			//Functions table
			Connection.Execute("CREATE TABLE IF NOT EXISTS Functions (" +
				"Name				STRING PRIMARY KEY" +
			")");

			//Company table
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

			//Department table
			Connection.Execute("CREATE TABLE IF NOT EXISTS Departments (" +
			"ID					INTEGER PRIMARY KEY," +
			"Name				STRING NOT NULL," +
			"Description        STRING" +
			")");

			//Notes table
			Connection.Execute("CREATE TABLE IF NOT EXISTS Notes (" +
			"ID                 INTEGER PRIMARY KEY," +
			"Title              STRING NOT NULL," +
			"Text               STRING" +
			")");

			//Permissions table
			Connection.Execute("CREATE TABLE IF NOT EXISTS Permissions (" +
				"User				INTEGER NOT NULL," +
				"Permission			INTEGER NOT NULL," +
				"Department			INTEGER," +
				"PRIMARY KEY (User, Department)," +
				"FOREIGN KEY(User) REFERENCES Users(ID) ON DELETE CASCADE," +
				"FOREIGN KEY(Department) REFERENCES Departments(ID) ON DELETE CASCADE" +
			")");

			//Users table
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

			//Sessions table
			Connection.Execute("CREATE TABLE IF NOT EXISTS Sessions (" +
				"ID					INTEGER PRIMARY KEY," +
				"User				INTEGER NOT NULL," +
				"SessionID			STRING NOT NULL," +
				"Token				INTEGER NOT NULL," +
				"RememberMe			INTEGER NOT NULL," +
				"FOREIGN KEY(User)	REFERENCES Users(ID) ON DELETE CASCADE" +
			")");

			//GenericTableConfigurations table
			Connection.Execute("CREATE TABLE IF NOT EXISTS GenericTableConfigurations (" +
				"Name				STRING PRIMARY KEY," +
				"ReqValidation		INTEGER," +
				"Department			INTEGER NOT NULL," +
				"FOREIGN KEY(Department) REFERENCES Departments(ID) ON UPDATE CASCADE" +
			")");

			//If the Administrators department doesn't exist yet, create it.
			Department AdministratorDept = Connection.Get<Department>(1);
			if ( AdministratorDept == null ) {
				Connection.Insert(new Department("Administrators", "Department for Administrators"));
			}

			//If the All Users doesn't exist yet, create it
			Department UncategorizedDept = Connection.Get<Department>(2);
			if ( UncategorizedDept == null ) {
				Connection.Insert(new Department("All Users", "Default Department"));
			}

			//If the built-in Administrator account doesn't exist yet, create it. If it does exist, update its password to the
			//one specified in the server configuration file.
			User Administrator = Connection.Get<User>(1);
			if ( Administrator == null ) {
				Administrator = new User("Administrator", (string)Config.GetValue("AuthenticationSettings.AdministratorPassword"));
				Connection.Insert(Administrator);
				Administrator.SetPermissionLevel(Connection, PermLevel.Administrator, 1);
			} else {
				Administrator.ChangePassword((string)Config.GetValue("AuthenticationSettings.AdministratorPassword"));
				Connection.Update<User>(Administrator);
			}

			GenericDataTable.GetTableNames(Connection, 2).ForEach(Console.WriteLine);
		}

		/// <summary>
		/// Creates a new database connection and opens it immediately.
		/// </summary>
		/// <returns></returns>
		public static SQLiteConnection CreateConnection() {
			SQLiteConnection Connection = new SQLiteConnection("Data Source=Database.db;foreign keys=true");
			Connection.Open();
			return ( Connection );
		}
	}
}
