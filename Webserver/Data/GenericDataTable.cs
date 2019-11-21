using Dapper;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Webserver.Data {
	[Table("GenericTableConfigurations")]
	class GenericDataTable {
		private SQLiteConnection Connection;

		[Key]
		public string Name { get; set; }
		public bool ReqValidation { get; set; }
		public int Department { get; set; }

		/// <summary>
		/// List of reserved table names
		/// </summary>
		public static readonly ReadOnlyCollection<string> ReservedTables = new ReadOnlyCollection<string>(new List<string>() { "Functions", "Companies", "Departments", "Permissions", "Users", "Sessions", "GenericTableConfigurations" });
		/// <summary>
		/// List of reserved column names
		/// </summary>
		public static readonly ReadOnlyCollection<string> ReservedColumns = new ReadOnlyCollection<string>(new List<string>() { "Validated", "rowid"});

		/// <summary>
		/// Create a new datatable
		/// </summary>
		/// <param name="Connection">A database connection</param>
		/// <param name="Name">The table name</param>
		/// <param name="Columns">The columns this table will have. Must contain at least 1 column.</param>
		/// <param name="DepartmentID">The department that this table will be associated with.</param>
		/// <param name="ReqValidation">Whether or not entries in the table must be validated by a manager. If true, a hidden "Validated" column will be added.</param>
		public GenericDataTable(SQLiteConnection Connection, string Name, Dictionary<string, DataType> Columns, Department Dept, bool ReqValidation = false) : this(Connection, Name, Columns, Dept.ID, ReqValidation) { }
		
		/// <summary>
		/// Create a new datatable
		/// </summary>
		/// <param name="Connection">A database connection</param>
		/// <param name="Name">The table name</param>
		/// <param name="Columns">The columns this table will have. Must contain at least 1 column.</param>
		/// <param name="DepartmentID">The ID of the department that this table will be associated with.</param>
		/// <param name="RequireValidation">Whether or not entries in the table must be validated by a manager. If true, a hidden "Validated" column will be added.</param>
		public GenericDataTable(SQLiteConnection Connection, string Name, Dictionary<string, DataType> Columns, int DepartmentID = 2, bool RequireValidation = false){
			//Validate args
			if (!Regex.IsMatch(Name, "[0-9A-Za-z_]") || ReservedTables.Contains(Name)) {
				throw new ArgumentException("Invalid table name. Table names can only contain letters, numbers, and underscores. Also note that some names are reserved.");
			}
			if(Columns.Count == 0) {
				throw new ArgumentException("At least one column must be specified");
			}
			if(Columns.Keys.Intersect(ReservedColumns).Count() > 0) {
				throw new ArgumentException("Cannot use reserved column names");
			}
			foreach (var _ in Columns.Keys.Where(Column => !Regex.IsMatch(Column, "[0-9A-Za-z_]")).Select(Column => new { })) {
				throw new ArgumentException("Invalid column name. Column name must only contain letters, numbers, and underscores");
			}
			if (!Data.Department.Exists(Connection, DepartmentID) && DepartmentID != 2) {
				throw new ArgumentException("Missing or invalid Department ID");
			}
			if(Exists(Connection, Name)) {
				throw new ArgumentException("Table already exists");
			}


			//Set fields
			this.Connection = Connection;
			this.Name = Name;
			this.Department = DepartmentID;
			this.ReqValidation = RequireValidation;

			//Add Validated column if necessary
			if (RequireValidation) {
				Columns.Add("Validated", DataType.Integer);
			}

			//Create table
			string SQL = "CREATE TABLE " + Name + " (rowid Integer PRIMARY KEY,";
			List<string> SQLColumns = new List<string>();
			foreach(string Key in Columns.Keys) {
				SQLColumns.Add(Key + " " + Columns[Key]);
			}
			SQL += string.Join(',', SQLColumns) + ")";

			Connection.Execute(SQL);
			Connection.Execute("INSERT INTO GenericTableConfigurations (Name, ReqValidation, Department) VALUES ('" + Name + "', " + (RequireValidation ? 1 : 0) + ", " + DepartmentID + ")");
		}
				
		/// <summary>
		/// Dapper-only constructor. Do not use.
		/// </summary>
		/// <param name="Name"></param>
		/// <param name="ReqValidation"></param>
		/// <param name="Department"></param>
		public GenericDataTable(string Name, long ReqValidation, long Department) {
			this.Name = Name;
			this.ReqValidation = Convert.ToBoolean(ReqValidation);
			this.Department = (int)Department;
		}

		/// <summary>
		/// Add multiple columns to the table.
		/// </summary>
		/// <param name="Columns"></param>
		public void AddColumn(Dictionary<string, DataType> Columns) {
			foreach(string Name in Columns.Keys) {
				AddColumn(Name, Columns[Name]);
			}
		}

		/// <summary>
		/// Add a new column to the table.
		/// </summary>
		/// <param name="Name"></param>
		/// <param name="DT"></param>
		public void AddColumn(string Name, DataType DT = DataType.String) {
			if(!Regex.IsMatch(Name, "[0-9A-Za-z_]")) {
				throw new ArgumentException("Invalid column name. Column name must only contain letters, numbers, and underscores");
			}
			if (ReservedColumns.Contains(Name)) {
				throw new ArgumentException("Invalid column name. This name is reserved");
			}
			if (GetColumns().Keys.Contains(Name)) {
				throw new ArgumentException("Column already exists");
			}
			Connection.Execute("ALTER TABLE " + this.Name + " ADD COLUMN " + Name + " " + DT.ToString());
		}

		public void AddValidatedColumn() {
			if (GetColumns().Keys.Contains(Name)) {
				throw new ArgumentException("Column already exists");
			}
			this.ReqValidation = true;
			Connection.Update<GenericDataTable>(this);
			Connection.Execute("ALTER TABLE " + this.Name + " ADD COLUMN Validated Integer DEFAULT 0");
		}

		/// <summary>
		/// Renames a column.
		/// </summary>
		/// <param name="OldName">The column's current name.</param>
		/// <param name="NewName">The column's new name.</param>
		public void RenameColumn(string OldName, string NewName) {
			Dictionary<string, DataType> Columns = GetColumns();
			if (!Columns.ContainsKey(OldName)) {
				throw new ArgumentException("Column does not exist");
			}

			//Retrieve all data from the table.
			List<dynamic> Rows = Connection.Query("SELECT " + string.Join(',', Columns.Keys) + " FROM " + this.Name).ToList();
			List<IDictionary<string, object>> Data = (Rows.Select(Row => (IDictionary<string, object>)Row)).ToList();

			//Update columns list
			DataType DT = Columns[OldName];
			Columns.Remove(OldName);
			Columns.Add(NewName, DT);

			//Recreate the table.
			Connection.Execute("DROP TABLE " + this.Name);
			string SQL = "CREATE TABLE " + this.Name + " (";
			List<string> SQLColumns = new List<string>();
			foreach (string Key in Columns.Keys) {
				SQLColumns.Add(Key + " " + Columns[Key]);
			}
			SQL += string.Join(',', SQLColumns) + ")";
			Connection.Execute(SQL);

			//Restore all data
			List<string> ColumnNames = Columns.Keys.AsList();
			SQL = "INSERT INTO " + this.Name + "(" + string.Join(',', ColumnNames) + ") VALUES ";
			List<string> Values = new List<string>();
			foreach (IDictionary<string, object> Row in Data) {
				List<string> Cells = new List<string>();
				foreach (string Key in Row.Keys) {
					Cells.Add("'"+Row[Key].ToString()+"'");
				}
				Values.Add("(" + string.Join(',', Cells) + ")");
			}
			SQL += string.Join(',', Values);
			Connection.Execute(SQL);
		}

		/// <summary>
		/// Remove multiple columns from the table.
		/// </summary>
		/// <param name="Columns"></param>
		public void DropColumns(List<string> Columns) => Columns.ForEach(DropColumn);

		/// <summary>
		/// Remove a column from the table
		/// </summary>
		/// <param name="Name"></param>
		public void DropColumn(string Name) {
			Dictionary<string, DataType> Columns = GetColumns();
			if (!Columns.ContainsKey(Name)) {
				throw new ArgumentException("Column does not exist");
			}
			Columns.Remove(Name);
			if(Columns.Count == 0) {
				throw new ArgumentException("Table must have at least 1 column");
			}

			//Additional handling for system columns
			if(Name == "Validated") {
				this.ReqValidation = false;
				Connection.Update<GenericDataTable>(this);
			}

			//Retrieve all data from the table.
			List<dynamic> Rows = Connection.Query("SELECT "+string.Join(',', Columns.Keys)+" FROM "+this.Name).ToList();
			List<IDictionary<string, object>> Data = (Rows.Select(Row => (IDictionary<string, object>)Row)).ToList();

			//Recreate the table.
			Connection.Execute("DROP TABLE " + this.Name);
			string SQL = "CREATE TABLE " + this.Name + " (";
			List<string> SQLColumns = new List<string>();
			foreach (string Key in Columns.Keys) {
				SQLColumns.Add(Key + " " + Columns[Key]);
			}
			SQL += string.Join(',', SQLColumns) + ")";
			Connection.Execute(SQL);

			//Restore all data
			List<string> ColumnNames = Columns.Keys.AsList();
			SQL = "INSERT INTO " + this.Name + "(" + string.Join(',', ColumnNames) + ") VALUES ";
			List<string> Values = new List<string>();
			foreach(IDictionary<string, object> Row in Data) {
				List<string> Cells = new List<string>();
				foreach(string Key in Row.Keys) {
					Cells.Add("'" + Row[Key].ToString() + "'");
				}
				Values.Add("(" +string.Join(',',Cells) + ")");
			}
			SQL += string.Join(',', Values);
			Connection.Execute(SQL);
		}

		/// <summary>
		/// Deletes the table and all the data that it contains. Note that the current GenericDataTable object will become unusable after calling this method.
		/// </summary>
		public void DropTable() {
			Connection.Execute("DROP TABLE " + this.Name);
			Connection.Delete(this);
		}

		/// <summary>
		/// Returns a JArray containing JObjects that represent a table's rows.
		/// </summary>
		/// <param name="Connection"></param>
		/// <param name="Table"></param>
		/// <param name="Begin"></param>
		/// <param name="End"></param>
		/// <returns></returns>
		public JObject GetRows(int Begin = 0, int End = 25) {
			Dictionary<string, DataType> Columns = GetColumns(Connection, this.Name);
			List<dynamic> Rows = Connection.Query("SELECT " + string.Join(',', Columns.Keys) + " FROM " + this.Name + " WHERE rowid BETWEEN "+Begin+" AND "+End).ToList();
			return RowsToJObject(Rows, Columns);
		}

		public JObject GetUnvalidatedRows(int Begin = 0, int End = 25) {
			Dictionary<string, DataType> Columns = GetColumns(Connection, this.Name);
			if (!Columns.ContainsKey("Validated")) {
				throw new ArgumentException("Table contains no Validated column");
			}
			List<dynamic> Rows = Connection.Query("SELECT " + string.Join(',', Columns.Keys) + " FROM " + this.Name + " WHERE Validated = 0 AND rowid > " + Begin + " LIMIT " + End).ToList();
			return RowsToJObject(Rows, Columns);
		}

		/// <summary>
		/// Converts rows to a JObject.
		/// </summary>
		/// <param name="Data">A list of rows, as provided by Connection.Query</param>
		/// <param name="Columns">A column dictionary, as provided by GetColumns</param>
		/// <returns></returns>
		private static JObject RowsToJObject(List<dynamic> Rows, Dictionary<string, DataType> Columns) {
			List<IDictionary<string, object>> Data = (Rows.Select(Row => (IDictionary<string, object>)Row)).ToList();
			JObject Result = new JObject();
			foreach (IDictionary<string, object> Row in Data) {
				JObject JSONRow = new JObject();
				foreach (string Key in Row.Keys) {
					if (Key == "rowid") {
						continue;
					}
					JSONRow.Add(Key, CastValue(Row[Key], Columns[Key]));
				}
				Result.Add(Row["rowid"].ToString(), JSONRow);
			}

			return Result;
		}

		private static dynamic CastValue(object Value, DataType DT) => DT == DataType.Integer ? (int)(long)Value : (dynamic)(string)Value;

		/// <summary>
		/// Updates a row
		/// </summary>
		/// <param name="RowID">The row's unique ID</param>
		/// <param name="NewData">A dictionary containing keys (column names) and data</param>
		public void Update(int RowID, Dictionary<string, dynamic> NewData) => Update(new Dictionary<int, Dictionary<string, dynamic>>() { { RowID, NewData } });
		/// <summary>
		/// Updates a row
		/// </summary>
		/// <param name="Data">A dictionary containing row IDs and dictionaries that contain keys (column names) and data</param>	
		public void Update(Dictionary<int, Dictionary<string, dynamic>> Data) {
			string QueryList = "";
			foreach(int Row in Data.Keys) {
				List<string> ColumnUpdates = new List<string>();
				foreach(string Column in Data[Row].Keys) {
					ColumnUpdates.Add(Column + " = '" + Data[Row][Column]+"'");
				}

				QueryList += "UPDATE " + this.Name + " SET " + string.Join(',', ColumnUpdates) + " WHERE rowid = " + Row + ";\n";
			}
			Connection.Execute(QueryList);
		}

		/// <summary>
		/// Delete a row from the database.
		/// </summary>
		/// <param name="RowID">The row's unique ID</param>
		public void Delete(int RowID) => Delete(new List<int>() { RowID });
		/// <summary>
		/// Delete multiple rows from the database.
		/// </summary>
		/// <param name="RowIDs">A list containing row IDs</param>
		public void Delete(List<int> RowIDs) => this.Connection.Execute("DELETE FROM " + this.Name + " WHERE rowid IN (" + string.Join(',', RowIDs) + ")");

		/// <summary>
		/// Insert a row into the database. If a column's value is not specified, it will be null.
		/// </summary>
		/// <param name="Data">A dictionary containing keys (column names) and data</param>
		public void Insert(Dictionary<string, dynamic> Data) => Insert(new List<Dictionary<string, dynamic>>() { Data });
		/// <summary>
		/// Insert multiple rows into the database. If a column's value is not specified, it will be null.
		/// </summary>
		/// <param name="Data">A list of dictionaries containing keys (column names) and data</param>
		public void Insert(List<Dictionary<string, dynamic>> Data) {
			//Check columns
			List<string> Columns = GetColumns().Keys.ToList();
			foreach (var Key in Data.SelectMany(Row => Row.Keys.Where(Key => !Columns.Contains(Key)).Select(Key => Key))) {
				throw new ArgumentException("No such column: " + Key);
			}
			Columns.Remove("rowid");

			//Create query
			List<string> Values = new List<string>();
			foreach(Dictionary<string, dynamic> Row in Data) {
				List<string> Value = new List<string>();
				foreach (string Column in Columns) {
					if (Row.ContainsKey(Column)) {
						Value.Add("'"+Row[Column]+"'");
					} else {
						Value.Add("'null'");
					}
				}
				Values.Add("(" + string.Join(',', Value) + ")");
			}
			string SQL = "INSERT INTO " + this.Name + "(" + string.Join(',', Columns) + ") VALUES " + string.Join(',', Values);
			Connection.Execute(SQL);
		}

		/// <summary>
		/// Returns a list containing the names of this table's columns
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, DataType> GetColumns() => GetColumns(Connection, Name);

		/// <summary>
		/// Returns a list containing the names of a table's columns.
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, DataType> GetColumns(string Name) => GetColumns(Connection, Name);

		/// <summary>
		/// Returns a list containing the names of a table's columns.
		/// The table must already be in the database for this to work.
		/// </summary>
		/// <returns></returns>
		public static Dictionary<string, DataType> GetColumns(SQLiteConnection Connection, string Name) {
			Dictionary<string, DataType> Columns = new Dictionary<string, DataType>();
			foreach (dynamic val in Connection.Query("PRAGMA table_info(" + Name + ")").AsList()) {
				Columns.Add(val.name, Enum.Parse(typeof(DataType), val.type));
			}
			return Columns;
		}

		/// <summary>
		/// Returns true if a table exists with the specified name.
		/// </summary>
		/// <param name="Connection"></param>
		/// <param name="Name"></param>
		/// <returns></returns>
		public static bool Exists(SQLiteConnection Connection, string Name) {
			using SQLiteCommand CMD = new SQLiteCommand("SELECT name FROM sqlite_master", Connection);
			using SQLiteDataReader Reader = CMD.ExecuteReader();
			while (Reader.Read()) {
				NameValueCollection Row = Reader.GetValues();
				foreach (string Column in new List<string>(Row.AllKeys)) {
					if (Row[Column] == Name) {
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Retrieves a GenericDataTable from the database, using its name.
		/// </summary>
		/// <param name="Connection"></param>
		/// <param name="Name"></param>
		/// <returns></returns>
		public static GenericDataTable GetTableByName(SQLiteConnection Connection, string Name) {
			GenericDataTable Table = Connection.QueryFirstOrDefault<GenericDataTable>("SELECT * FROM GenericTableConfigurations WHERE Name = @Name", new { Name });
			Table.Connection = Connection;
			return Table;
		}
	}

	/// <summary>
	/// Enum for column datatypes.
	/// Booleans will automatically be converted to integers by Dapper
	/// Integers will be Int64 (long) when retrieved from the database.
	/// </summary>
	public enum DataType {
		Integer,
		Real,
		String,
		Blob
	}
}
