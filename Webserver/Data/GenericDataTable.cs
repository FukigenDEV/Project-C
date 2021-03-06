﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Dapper;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json.Linq;

namespace Webserver.Data {
	[Table("GenericTableConfigurations")]
	public class GenericDataTable {
		private SQLiteConnection Connection;

		[Key]
		public string Name { get; set; }
		public bool ReqValidation { get; set; }
		public int Department { get; set; }

		public const string RX = "^[A-z]{1}[0-9A-Za-z_]*$";

		/// <summary>
		/// List of reserved table names
		/// </summary>
		public static readonly ReadOnlyCollection<string> ReservedTables = new ReadOnlyCollection<string>(new List<string>() { "Functions", "Companies", "Departments", "Permissions", "Users", "Sessions", "GenericTableConfigurations" });
		/// <summary>
		/// List of reserved column names
		/// </summary>
		public static readonly ReadOnlyCollection<string> ReservedColumns = new ReadOnlyCollection<string>(new List<string>() { "Validated", "rowid" });

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
		public GenericDataTable(SQLiteConnection Connection, string Name, Dictionary<string, DataType> Columns, int DepartmentID = 2, bool RequireValidation = false) {
			//Validate args
			if ( !Regex.IsMatch(Name, RX) || ReservedTables.Contains(Name) ) {
				throw new ArgumentException("Invalid table name. Table names can only contain letters, numbers, and underscores. Also note that some names are reserved.");
			}
			if ( Columns.Count == 0 ) {
				throw new ArgumentException("At least one column must be specified");
			}
			if ( Columns.Keys.Intersect(ReservedColumns).Count() > 0 ) {
				throw new ArgumentException("Cannot use reserved column names");
			}
			foreach ( string Key in Columns.Keys ) {
				if ( !Regex.IsMatch(Key, RX) ) {
					throw new ArgumentException("Invalid column name.");
				}
			}
			if ( !Data.Department.Exists(Connection, DepartmentID) && DepartmentID != 2 ) {
				throw new ArgumentException("Missing or invalid Department ID");
			}
			if ( Exists(Connection, Name) ) {
				throw new ArgumentException("Table already exists");
			}


			//Set fields
			this.Connection = Connection;
			this.Name = Name;
			this.Department = DepartmentID;
			this.ReqValidation = RequireValidation;

			//Create table
			string SQL = "CREATE TABLE `" + Name + "` (rowid Integer PRIMARY KEY,";
			List<string> SQLColumns = new List<string>();
			foreach ( string Key in Columns.Keys ) {
				SQLColumns.Add("`" + Key + "` " + Columns[Key]);
			}
			SQL += string.Join(',', SQLColumns) + ")";

			Connection.Execute(SQL);
			Connection.Execute("INSERT INTO GenericTableConfigurations (Name, ReqValidation, Department) VALUES ('" + Name + "', " + ( RequireValidation ? 1 : 0 ) + ", " + DepartmentID + ")");

			if ( RequireValidation ) {
				AddValidatedColumn();
			}
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
			foreach ( string Name in Columns.Keys ) {
				AddColumn(Name, Columns[Name]);
			}
		}

		/// <summary>
		/// Add a new column to the table.
		/// </summary>
		/// <param name="Name"></param>
		/// <param name="DT"></param>
		public void AddColumn(string Name, DataType DT = DataType.String) {
			if ( !Regex.IsMatch(Name, RX) ) {
				throw new ArgumentException("Invalid column name. Column name must only contain letters, numbers, and underscores");
			}
			if ( ReservedColumns.Contains(Name) ) {
				throw new ArgumentException("Invalid column name. This name is reserved");
			}
			if ( GetColumns().Keys.Contains(Name) ) {
				throw new ArgumentException("Column already exists");
			}
			Connection.Execute("ALTER TABLE `" + this.Name + "` ADD COLUMN `" + Name + "` " + DT.ToString());
		}

		/// <summary>
		/// Add a new validation column to the table.
		/// </summary>
		public void AddValidatedColumn() {
			if ( GetColumns().Keys.Contains("Validated") ) {
				throw new ArgumentException("Column already exists");
			}
			this.ReqValidation = true;
			Connection.Update<GenericDataTable>(this);
			Connection.Execute("ALTER TABLE `" + this.Name + "` ADD COLUMN Validated Integer DEFAULT 0");
		}

		/// <summary>
		/// Rename multiple columns from the table.
		/// </summary>
		/// <param name="Columns"></param>
		public void RenameColumn(Dictionary<string, string> Columns) {
			foreach ( var Entry in Columns ) {
				RenameColumn(Entry.Key, Entry.Value);
			}
		}

		/// <summary>
		/// Renames a column.
		/// </summary>
		/// <param name="OldName">The column's current name.</param>
		/// <param name="NewName">The column's new name.</param>
		public void RenameColumn(string OldName, string NewName) {
			Dictionary<string, DataType> Columns = GetColumns();
			if ( !Columns.ContainsKey(OldName) ) {
				throw new ArgumentException("Column does not exist");
			}
			if ( Columns.ContainsKey(NewName) ) {
				throw new ArgumentException("Column already exists");
			}
			if ( OldName == "Validated" || NewName == "Validated" ) {
				throw new ArgumentException("Can't rename Validated column");
			}

			//Retrieve all data from the table.
			string aaa = "SELECT `" + string.Join("`,`", Columns.Keys) + "` FROM `" + this.Name + "`";
			List<dynamic> Rows = Connection.Query(aaa).ToList();
			List<IDictionary<string, object>> Data = ( Rows.Select(Row => (IDictionary<string, object>)Row) ).ToList();

			//Update columns list
			DataType DT = Columns[OldName];
			Columns.Remove(OldName);
			Columns.Add(NewName, DT);

			//Recreate the table.
			Connection.Execute("DROP TABLE `" + this.Name + "`");
			string SQL = "CREATE TABLE `" + this.Name + "` (";
			List<string> SQLColumns = new List<string>();
			foreach ( string Key in Columns.Keys ) {
				SQLColumns.Add("`" + Key + "` " + Columns[Key]);
			}
			SQL += string.Join(',', SQLColumns) + ")";
			Connection.Execute(SQL);

			RestoreData(Data);
		}

		/// <summary>
		/// Inserts the specified data into the table.
		/// </summary>
		/// <param name="Data"></param>
		private void RestoreData(List<IDictionary<string, object>> Data) {
			//Restore all data if necessary
			if ( Data.Count > 0 ) {
				Dictionary<string, DataType> Columns = GetColumns();
				List<string> ColumnNames = Columns.Keys.AsList();
				List<string> Values = new List<string>();
				foreach ( IDictionary<string, object> Row in Data ) {
					List<string> Cells = new List<string>();
					foreach ( string Key in Row.Keys ) {
						Cells.Add("'" + Row[Key].ToString() + "'");
					}
					Values.Add("(" + string.Join(',', Cells) + ")");
				}
				Connection.Execute("INSERT INTO `" + this.Name + "` (" + string.Join(',', ColumnNames) + ") VALUES " + string.Join(',', Values));
			}
		}

		/// <summary>
		/// Remove multiple columns from the table.
		/// </summary>
		/// <param name="Columns"></param>
		public void DropColumn(List<string> Columns) => Columns.ForEach(DropColumn);

		/// <summary>
		/// Remove a column from the table
		/// </summary>
		/// <param name="Name"></param>
		public void DropColumn(string Name) {
			Dictionary<string, DataType> Columns = GetColumns();
			if ( !Columns.ContainsKey(Name) ) {
				throw new ArgumentException("Column does not exist");
			}
			Columns.Remove(Name);
			if ( ( ReqValidation && Columns.Count == 2 ) || Columns.Count == 1 ) {
				throw new ArgumentException("Table must have at least 1 column, excluding Validated and rowid");
			}

			//Additional handling for system columns
			if ( Name == "Validated" ) {
				this.ReqValidation = false;
				Connection.Update<GenericDataTable>(this);
			}

			//Retrieve all data from the table.
			List<dynamic> Rows = Connection.Query("SELECT " + string.Join(',', Columns.Keys) + " FROM `" + this.Name + "`").ToList();
			List<IDictionary<string, object>> Data = ( Rows.Select(Row => (IDictionary<string, object>)Row) ).ToList();

			//Recreate the table.
			Connection.Execute("DROP TABLE `" + this.Name + "`");
			string SQL = "CREATE TABLE `" + this.Name + "` (";
			List<string> SQLColumns = new List<string>();
			foreach ( string Key in Columns.Keys ) {
				SQLColumns.Add(Key + " " + Columns[Key]);
			}
			SQL += string.Join(',', SQLColumns) + ")";
			Connection.Execute(SQL);

			//Restore all data if necessary
			RestoreData(Data);
		}

		/// <summary>
		/// Deletes the table and all the data that it contains. Note that the current GenericDataTable object will become unusable after calling this method.
		/// </summary>
		public void DropTable() {
			Connection.Execute("DROP TABLE `" + this.Name + "`");
			Connection.Delete(this);
		}

		/// <summary>
		/// Returns a JObject containing JObjects that represent a table's rows.
		/// </summary>
		/// <param name="Connection"></param>
		/// <param name="Table"></param>
		/// <param name="Begin"></param>
		/// <param name="End"></param>
		/// <returns></returns>
		public JObject GetRows(int Begin = 0, int End = 25) {
			Dictionary<string, DataType> Columns = GetColumns(Connection, this.Name);
			if ( Begin < 0 || End < 1 ) throw new ArgumentException("Begin/End too low");
			if ( Begin > End ) throw new ArgumentException("Begin must be lower than End");
			
			List<dynamic> Rows = Connection.Query("SELECT " + string.Join(',', Columns.Keys) + " FROM `" + this.Name + "` WHERE rowid BETWEEN " + Begin + " AND " + End).ToList();
			return RowsToJObject(Rows, Columns);
		}

		/// <summary>
		/// Returns a JObject containing JObjects that represent a table's unvalidated rows.
		/// </summary>
		/// <param name="Begin"></param>
		/// <param name="End"></param>
		/// <returns></returns>
		public JObject GetUnvalidatedRows(int Begin = 0, int End = 25) {
			Dictionary<string, DataType> Columns = GetColumns(Connection, this.Name);
			if ( !Columns.ContainsKey("Validated") ) throw new ArgumentException("Table contains no Validated column");
			if ( Begin < 0 || End < 1 ) throw new ArgumentException("Begin/End too low");
			if ( Begin > End ) throw new ArgumentException("Begin must be lower than End");

			string SQL = "SELECT " + string.Join(',', Columns.Keys) + " FROM `" + this.Name + "` WHERE Validated = 0 AND rowid >= " + Begin + " LIMIT " + End;
			List<dynamic> Rows = Connection.Query(SQL).ToList();
			return RowsToJObject(Rows, Columns);
		}

		/// <summary>
		/// Converts rows to a JObject.
		/// </summary>
		/// <param name="Data">A list of rows, as provided by Connection.Query</param>
		/// <param name="Columns">A column dictionary, as provided by GetColumns</param>
		/// <returns></returns>
		private static JObject RowsToJObject(List<dynamic> Rows, Dictionary<string, DataType> Columns) {
			//Create Columns list
			JObject JColumns = new JObject();
			foreach ( KeyValuePair<string, DataType> Entry in Columns ) {
				JColumns.Add(Entry.Key, Entry.Value.ToString());
			}

			//Create Rows list
			JArray JRows = new JArray();
			foreach ( IDictionary<string, object> Row in Rows ) {
				JArray Entry = new JArray();
				foreach ( string Key in Row.Keys ) {
					Entry.Add(Row[Key]);
				}
				JRows.Add(Entry);
			}

			return new JObject {
				{ "Columns", JColumns },
				{ "Rows", JRows }
			}; ;
		}

		public static dynamic CastValue(object Value, DataType DT) => DT == DataType.Integer ? (int)(long)Value : (dynamic)Value.ToString();

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
			if ( Data == null || Data.Count == 0 ) throw new ArgumentNullException(nameof(Data));

			//Type-checking
			Dictionary<string, DataType> Columns = GetColumns();
			foreach ( Dictionary<string, dynamic> Row in Data.Values ) {
				if ( Row.Count == 0 ) throw new ArgumentException("No updates specified");
				foreach ( string Column in Row.Keys ) {
					if ( !Columns.ContainsKey(Column) ) throw new ArgumentException("Invalid column: " + Column);
					if ( Columns[Column] == DataType.Integer && Row[Column].GetType() != typeof(int) ) throw new ArgumentException("Column " + Column + " can only contain integers");
				}
			}

			//Build and execute SQL
			string QueryList = "";
			foreach ( int Row in Data.Keys ) {
				List<string> ColumnUpdates = new List<string>();
				foreach ( string Column in Data[Row].Keys ) {
					ColumnUpdates.Add(Column + " = '" + Data[Row][Column].ToString() + "'");
				}

				QueryList += "UPDATE `" + this.Name + "` SET " + string.Join(',', ColumnUpdates) + " WHERE rowid = " + Row + ";\n";
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
			if ( Data == null || Data.Count == 0 ) throw new ArgumentNullException(nameof(Data));

			//Type-checking
			Dictionary<string, DataType> Columns = GetColumns();
			foreach ( Dictionary<string, dynamic> Row in Data ) {
				if ( Row.Count == 0 ) throw new ArgumentException("No updates specified");
				foreach ( string Column in Row.Keys ) {
					if ( !Columns.ContainsKey(Column) ) throw new ArgumentException("Invalid column: " + Column);
					if ( Columns[Column] == DataType.Integer && Row[Column].GetType() != typeof(int) ) throw new ArgumentException("Column " + Column + " can only contain integers");
				}
			}

			//Check columns
			List<string> ColumNames = Columns.Keys.ToList();
			foreach ( var Key in Data.SelectMany(Row => Row.Keys.Where(Key => !ColumNames.Contains(Key)).Select(Key => Key)) ) {
				throw new ArgumentException("No such column: " + Key);
			}
			ColumNames.Remove("rowid");

			//Create query
			List<string> Values = new List<string>();
			foreach ( Dictionary<string, dynamic> Row in Data ) {
				List<string> Value = new List<string>();
				foreach ( string Column in ColumNames ) {
					if ( Row.ContainsKey(Column) ) {
						Value.Add("'" + Row[Column] + "'");
					} else {
						if ( Columns[Column] == DataType.Integer ) {
							Value.Add("0");
						} else {
							Value.Add("'null'");
						}
					}
				}
				Values.Add("(" + string.Join(',', Value) + ")");
			}
			string SQL = "INSERT INTO `" + this.Name + "`(" + string.Join(',', ColumNames) + ") VALUES " + string.Join(',', Values);
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
			foreach ( dynamic val in Connection.Query("PRAGMA table_info(`" + Name + "`)").AsList() ) {
				Columns.Add(val.name, Enum.Parse(typeof(DataType), val.type));
			}
			return Columns;
		}

		/// <summary>
		/// Returns true if the specified table exists.
		/// </summary>
		/// <param name="Connection"></param>
		/// <param name="Name"></param>
		/// <returns></returns>
		public static bool Exists(SQLiteConnection Connection, string Name) => Exists(Connection, new List<string>() { Name });

		/// <summary>
		/// Returns true if all specified tables exist.
		/// </summary>
		/// <param name="Connection"></param>
		/// <param name="Name"></param>
		/// <returns></returns>
		public static bool Exists(SQLiteConnection Connection, List<string> Names) => Names.Intersect(GetTableNames(Connection)).Count() == Names.Count();

		/// <summary>
		/// Returns true if the specified row exists
		/// </summary>
		/// <param name="Connection"></param>
		/// <param name="ID"></param>
		/// <returns></returns>
		public bool RowExists(int ID) => RowExists(new List<int>() { ID });
		/// <summary>
		/// Returns true if the specified row exists.
		/// </summary>
		/// <param name="Connection"></param>
		/// <param name="IDs"></param>
		/// <returns></returns>
		public bool RowExists(List<int> IDs) => IDs.Intersect(Connection.Query<int>("SELECT rowid FROM `" + Name + "` WHERE rowid IN (" + string.Join(',', IDs) + ")").ToList()).Count() == IDs.Count();

		/// <summary>
		/// Retrieves a GenericDataTable from the database, using its name.
		/// </summary>
		/// <param name="Connection"></param>
		/// <param name="Name"></param>
		/// <returns></returns>
		public static GenericDataTable GetTableByName(SQLiteConnection Connection, string Name) {
			GenericDataTable Table = Connection.QueryFirstOrDefault<GenericDataTable>("SELECT * FROM GenericTableConfigurations WHERE Name = @Name", new { Name });
			if ( Table != null ) Table.Connection = Connection;
			return Table;
		}

		/// <summary>
		/// Returns a list of all generic data tables.
		/// </summary>
		/// <param name="Connection"></param>
		/// <param name="DepartmentID"></param>
		/// <returns></returns>
		public static List<GenericDataTable> GetTables(SQLiteConnection Connection, int DepartmentID = 0) {
			if ( DepartmentID != 0 && !Data.Department.Exists(Connection, DepartmentID) ) {
				throw new ArgumentException("No such department");
			}
			return DepartmentID == 0
				? Connection.Query<GenericDataTable>("SELECT * FROM GenericTableConfigurations").ToList()
				: Connection.Query<GenericDataTable>("SELECT * FROM GenericTableConfigurations WHERE Department = @DepartmentID", new { DepartmentID }).ToList();
		}

		/// <summary>
		/// Returns a list of all generic data table names.
		/// </summary>
		/// <param name="Connection"></param>
		/// <param name="DepartmentID"></param>
		/// <returns></returns>
		public static List<string> GetTableNames(SQLiteConnection Connection, int DepartmentID = 0) {
			if (DepartmentID != 0 && !Data.Department.Exists(Connection, DepartmentID)) {
				throw new ArgumentException("No such department");
			}
			return DepartmentID == 0
				? Connection.Query<string>("SELECT Name FROM GenericTableConfigurations").ToList()
				: Connection.Query<string>("SELECT Name FROM GenericTableConfigurations WHERE Department = @DepartmentID", new { DepartmentID }).ToList();

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
