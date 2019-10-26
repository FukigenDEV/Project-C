using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Dapper;

namespace Webserver.Data {
	class GenericDataTable {
		private readonly SQLiteConnection Connection;
		public readonly string Name;
		public readonly bool RequireValidation;
		public readonly int DepartmentID;
		private string SQL1 = "CREATE TABLE %NAME% (";
		private string SQL2 = "";
		private bool Uploaded = false;

		/// <summary>
		/// Create a new generic data table.
		/// </summary>
		/// <param name="Connection">A SQLiteConnection</param>
		/// <param name="Name">The table name</param>
		public GenericDataTable(SQLiteConnection Connection, string Name, bool RequireValidation = false) {
			Debug.Assert(Regex.IsMatch(Name, "[0-9A-Za-z_]"), "Invalid table name", "Table names can only contain letters, numbers, and underscores");
			this.Connection = Connection;
			SQL1 = SQL1.Replace("%NAME%", Name);

			if (RequireValidation) {
				AddColumn("Validated", DataType.Integer);
			}
		}

		/// <summary>
		/// Dapper-only constructor. Do not use.
		/// </summary>
		/// <param name="Name"></param>
		/// <param name="ReqValidation"></param>
		/// <param name="Department"></param>
		public GenericDataTable(string Name, long ReqValidation, long Department) {
			this.Uploaded = true;
			
			this.Name = Name;
			this.RequireValidation = Convert.ToBoolean(ReqValidation);
			this.DepartmentID = (int)Department;
		}

		/// <summary>
		/// Add a new 
		/// </summary>
		/// <param name="Name"></param>
		/// <param name="DT"></param>
		public void AddColumn(string Name, DataType DT = DataType.String, bool NotNull = false) {
			Debug.Assert(!Uploaded, "Table already uploaded.");
			Debug.Assert(Regex.IsMatch(Name, "[0-9A-Za-z_]"), "Invalid column name", "Column names can only contain letters, numbers, and underscores");
			if(SQL1[^1] != '(') {
				SQL1+= ", ";
			}
			string ColumnText = Name + " " + DT.ToString().ToUpper();
			if (NotNull) {
				ColumnText += " NOT NULL";
			}
			SQL1 += ColumnText;
		}

		public void AddReferenceColumn(string Name, DataType DT, string ReferenceTableName, string ReferenceColumnName) {
			//Check if the referenced table exists
			if (ReferenceTableName != "Users" && ReferenceTableName != "Departments" && ReferenceTableName != "Functions" && GetTableByName(this.Connection, ReferenceTableName) == null) {
				throw new ArgumentException("Reference table does not exist");
			}

			//Check if the referenced column exists
			if (!GetColumns(ReferenceTableName).Contains(ReferenceColumnName)) {
				throw new ArgumentException("Reference column does not exist");
			}

			AddColumn(Name, DT);
			if (SQL2.Length == 0 || SQL2[^1] != ' '){
				SQL2 += ", ";
			}
			SQL2 += "FOREIGN KEY(" + Name + ") REFERENCES " + ReferenceTableName + "(" + ReferenceColumnName + ") ON UPDATE CASCADE";
		}

		/// <summary>
		/// Add this table to the database.
		/// </summary>
		public void Upload() {
			Debug.Assert(!Uploaded, "Table already uploaded.");
			Connection.Execute(SQL1+SQL2+")");
			Uploaded = true;
		}

		/// <summary>
		/// The SQL used to create this table.
		/// </summary>
		public string SQL {
			get {
				if (Uploaded) {
					return Connection.QueryFirstOrDefault<string>("SELECT SQL FROM sqlite_master WHERE name = @name", new { name = this.Name });
				} else {
					return SQL1 + SQL2 + ")";
				}
			}
		}

		/// <summary>
		/// Returns a list containing the names of this table's columns
		/// The table must already be in the database for this to work.
		/// </summary>
		/// <returns></returns>
		public List<string> GetColumns() => GetColumns(Connection, Name);

		/// <summary>
		/// Returns a list containing the names of a table's columns.
		/// The table must already be in the database for this to work.
		/// </summary>
		/// <returns></returns>
		public List<string> GetColumns(string Name) => GetColumns(Connection, Name);

		/// <summary>
		/// Returns a list containing the names of a table's columns.
		/// The table must already be in the database for this to work.
		/// </summary>
		/// <returns></returns>
		public static List<string> GetColumns(SQLiteConnection Connection, string Name) {
			List<string> Columns = new List<string>();
			foreach (dynamic val in Connection.Query("PRAGMA table_info("+Name+")").AsList()) {
				Columns.Add(val.name);
			}
			return Columns;
		}

		public static GenericDataTable GetTableByName(SQLiteConnection Connection, string Name) => Connection.QueryFirstOrDefault<GenericDataTable>("SELECT * FROM GenericTableConfiguration WHERE TableName = @Name", new { Name });
	}

	public enum DataType {
		Integer,
		Real,
		String,
		Blob
	}
}
