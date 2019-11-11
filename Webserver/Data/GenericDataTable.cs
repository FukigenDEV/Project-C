using Dapper;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SQLite;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

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
		public GenericDataTable(SQLiteConnection Connection, string Name, bool RequireValidation = false, int DepartmentID = 2) {
			Debug.Assert(Regex.IsMatch(Name, "[0-9A-Za-z_]"), "Invalid table name", "Table names can only contain letters, numbers, and underscores");
			this.Connection = Connection;
			this.DepartmentID = DepartmentID;
			this.Name = Name;
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
		public GenericDataTable(string TableName, long ReqValidation, long Department) {
			this.Uploaded = true;

			this.Name = TableName;
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
			if (SQL1[^1] != '(') {
				SQL1 += ", ";
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
			if (SQL2.Length == 0 || SQL2[^1] != ' ') {
				SQL2 += ", ";
			}
			SQL2 += "FOREIGN KEY(" + Name + ") REFERENCES " + ReferenceTableName + "(" + ReferenceColumnName + ") ON UPDATE CASCADE";
		}

		/// <summary>
		/// Add this table to the database.
		/// </summary>
		public void Upload() {
			Debug.Assert(!Uploaded, "Table already uploaded.");
			Connection.Execute(SQL1 + SQL2 + ")");
			string SQL = "INSERT INTO GenericTableConfiguration (TableName, ReqValidation, Department) VALUES ('" + Name + "', " + (RequireValidation ? 1 : 0) + ", " + this.DepartmentID + ")";
			Connection.Execute(SQL);

			Uploaded = true;
		}

		/// <summary>
		/// Returns a JArray containing JObjects that represent this table's rows.
		/// </summary>
		/// <param name="Begin"></param>
		/// <param name="End"></param>
		/// <returns></returns>
		public JArray GetRowsAsJSON(SQLiteConnection Connection, int Begin = 0, int End = 25) => GetRowsAsJSON(Connection, this, Begin, End);

		/// <summary>
		/// Returns a JArray containing JObjects that represent a table's rows.
		/// </summary>
		/// <param name="Connection"></param>
		/// <param name="Table"></param>
		/// <param name="Begin"></param>
		/// <param name="End"></param>
		/// <returns></returns>
		public static JArray GetRowsAsJSON(SQLiteConnection Connection, GenericDataTable Table, int Begin = 0, int End = 25) {
			using SQLiteDataReader Reader = new SQLiteCommand("SELECT * FROM " + Table.Name + " WHERE rowid BETWEEN " + Begin + " AND " + End, Connection).ExecuteReader();
			JArray Result = new JArray();
			while (Reader.Read()) {
				NameValueCollection Row = Reader.GetValues();
				JObject JSONRow = new JObject();
				foreach (string Column in new List<string>(Row.AllKeys)) {
					JSONRow.Add(Column, Row[Column]);
				}
				Result.Add(JSONRow);
			}
			return Result;
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
			foreach (dynamic val in Connection.Query("PRAGMA table_info(" + Name + ")").AsList()) {
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
