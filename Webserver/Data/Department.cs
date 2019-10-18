using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;

namespace Webserver.Data {
	public class Department {
		
		public int ID { get; set; }
		public string Name { get; set; }

		public Department(long ID, string Name) {
			this.ID = (int)ID;
			this.Name = Name;
		}
		public Department(string Name) {
			this.Name = Name;
		}

		public static Department GetDepartmentByName(SQLiteConnection Connection, string name) => Connection.QueryFirstOrDefault<Department>("SELECT * FROM Departments WHERE Name = @Name", new { name });
	}
}
