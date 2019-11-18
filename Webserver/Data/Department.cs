﻿using Dapper;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Webserver.Data {
	public class Department {

		public int ID { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }

        public Department() { }

		/// <summary>
		/// Dapper constructor. Do not use.
		/// </summary>
		/// <param name="ID"></param>
		/// <param name="Name"></param>
		/// <param name="Description"></param>
		public Department(long ID, string Name, string Description) {
			this.ID = (int)ID;
			this.Name = Name;
			this.Description = Description;
		}

		/// <summary>
		/// Creates a new department
		/// </summary>
		/// <param name="Name"></param>
		/// <param name="Description"></param>
        public Department(string Name, string Description = null) {
            this.Name = Name;
            this.Description = Description;
        }

		/// <summary>
		/// Lists all departments.
		/// </summary>
		/// <param name="Connection"></param>
		/// <returns></returns>
        public static List<Department> GetAllDepartments(SQLiteConnection Connection) => Connection.Query<Department>("SELECT * FROM Departments").AsList();

		/// <summary>
		/// Get a department's information using its name. Returns null if the department doesn't exist.
		/// </summary>
		/// <param name="Connection"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static Department GetDepartmentByName(SQLiteConnection Connection, string name) => Connection.QueryFirstOrDefault<Department>("SELECT * FROM Departments WHERE Name = @Name", new { name });
	}
}
