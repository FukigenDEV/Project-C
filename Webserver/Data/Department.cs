using Dapper;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Webserver.Data {
	public class Department {

		public int ID { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }

        public Department() { }

		public Department(long ID, string Name) {
			this.ID = (int)ID;
			this.Name = Name;
		}

		public Department(string Name) {
			this.Name = Name;
		}

        public Department(string Name, string Description) {
            this.Name = Name;
            this.Description = Description;
        }

		public static Department GetDepartmentByName(SQLiteConnection Connection, string name) => Connection.QueryFirstOrDefault<Department>("SELECT * FROM Departments WHERE Name = @Name", new { name });
	}
}
