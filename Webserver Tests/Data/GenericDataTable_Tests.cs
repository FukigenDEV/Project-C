using Microsoft.VisualStudio.TestTools.UnitTesting;
using Webserver.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using Dapper.Contrib.Extensions;
using Dapper;
using System.Diagnostics.CodeAnalysis;
using Configurator;
using System.IO;
using System.Reflection;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Linq;

namespace Webserver.Data.Tests {
	[TestClass()]
	public class GenericDataTable_Tests {
		/// <summary>
		/// Database connection. Will be automatically closed upon test completion.
		/// </summary>
		public SQLiteConnection Connection;
		/// <summary>
		/// Standard testing table.
		/// </summary>
		public GenericDataTable Table;

		[TestInitialize()]
		public void Init() {
			//Init config
			Config.AddConfig(new StreamReader(Assembly.LoadFrom("Webserver").GetManifestResourceStream("Webserver.DefaultConfig.json")));
			Config.SaveDefaultConfig();
			Config.LoadConfig();

			//Init database and create initial connection + table
			if ( File.Exists("Database.db") ) File.Delete("Database.db"); //Database doesn't always get wiped after debugging a failed test.
			Database.Init();
			Connection = Database.CreateConnection();
			Table = CreateTestTable(Connection);
		}

		[TestCleanup()]
		public void Cleanup() {
			Connection.Close();
			File.Delete("Database.db");
		}

		/// <summary>
		/// Create a datatable for testing purposes. Should be deleted after use.
		/// The name of the table will be "UnitTestTable", and it will have 4 columns (one for each datatype)
		/// </summary>
		/// <param name="Connection"></param>
		/// <returns></returns>
		private static GenericDataTable CreateTestTable(SQLiteConnection Connection, string Name = "Table1", Dictionary<string, DataType> Columns = null, int DepartmentID = 1, bool ReqValidation = true) {
			if ( Columns == null ) {
				Columns = new Dictionary<string, DataType>() {
					{"StringColumn", DataType.String },
					{"IntegerColumn", DataType.Integer },
				};
			}
			return new GenericDataTable(Connection, Name, Columns, DepartmentID, ReqValidation);
		}

		/// <summary>
		/// Check if GenericDataTables can be created, assuming all parameters are correct.
		/// Test is done by simply querying the table that was created during initialization. If this succeeds, the table exists.
		/// </summary>
		[TestMethod()]
		public void Constructor_ValidArgumentsTest() => Connection.Query("SELECT StringColumn, IntegerColumn FROM Table1");

		/// <summary>
		/// Check if the table name regex check is working.
		/// </summary>
		[TestMethod()]
		[ExpectedException(typeof(ArgumentException), "Argument check succeeded when it shouldn't")]
		public void Constructor_InvalidNameRegexTest() => CreateTestTable(Connection, "12345");

		/// <summary>
		/// Check if the table reserved name check is working.
		/// </summary>
		[TestMethod()]
		[ExpectedException(typeof(ArgumentException), "Argument check succeeded when it shouldn't")]
		public void Constructor_ReservedName() => CreateTestTable(Connection, "Functions");

		/// <summary>
		/// Check if the column count check is working.
		/// </summary>
		[TestMethod()]
		[ExpectedException(typeof(ArgumentException), "Argument check succeeded when it shouldn't")]
		public void Constructor_ColumnCountCheck() => CreateTestTable(Connection, Columns: new Dictionary<string, DataType>());

		/// <summary>
		/// Check if the reserved column name check is working.
		/// </summary>
		[TestMethod()]
		[ExpectedException(typeof(ArgumentException), "Argument check succeeded when it shouldn't")]
		public void Constructor_ReservedColumnNameCheck() => CreateTestTable(Connection, Columns: new Dictionary<string, DataType>() {{"Validated", DataType.Integer}});

		/// <summary>
		/// Check if the column name regex check is working.
		/// </summary>
		[TestMethod()]
		[ExpectedException(typeof(ArgumentException), "Argument check succeeded when it shouldn't")]
		public void Constructor_ColumnNameRegexCheck() => CreateTestTable(Connection, Columns: new Dictionary<string, DataType>() { { "12345", DataType.Integer } });

		/// <summary>
		/// Check if the department existence check is working.
		/// </summary>
		[TestMethod()]
		[ExpectedException(typeof(ArgumentException), "Argument check succeeded when it shouldn't")]
		public void Constructor_DepartmentCheck() => CreateTestTable(Connection, DepartmentID: 100);

		/// <summary>
		/// Check if the table existence check is working
		/// </summary>
		[TestMethod()]
		[ExpectedException(typeof(ArgumentException), "Argument check succeeded when it shouldn't")]
		public void Constructor_AlreadyExistsCheck() => CreateTestTable(Connection);

		/// <summary>
		/// Check if the AddColumn function works when given valid arguments
		/// </summary>
		[TestMethod()]
		public void AddColumn_ValidArgumentsTest() => Table.AddColumn("NewColumn", DataType.String);

		/// <summary>
		/// Check if the AddColumn function properly throws an exception when an invalid name is given
		/// </summary>
		[TestMethod()]
		[ExpectedException(typeof(ArgumentException), "Argument check succeeded when it shouldn't")]
		public void AddColumn_RegexCheck() => Table.AddColumn("12345", DataType.String);

		/// <summary>
		/// Check if the AddColumn function properly throws an exception when a reserved name is given
		/// </summary>
		[TestMethod()]
		[ExpectedException(typeof(ArgumentException), "Argument check succeeded when it shouldn't")]
		public void AddColumn_ReservedCheck() => Table.AddColumn("Validated", DataType.String);

		/// <summary>
		/// Check if the AddColumn function properly throws an exception when an existing column name is given
		/// </summary>
		[TestMethod()]
		[ExpectedException(typeof(ArgumentException), "Argument check succeeded when it shouldn't")]
		public void AddColumn_AlreadyExistsCheck() {
			Table.AddColumn("NewColumn", DataType.String);
			Table.AddColumn("NewColumn", DataType.String);
		}

		/// <summary>
		/// Check if the AddValidatedColumn function works when we only call it once.
		/// </summary>
		[TestMethod()]
		public void AddValidatedColumn_Valid() {
			Table = CreateTestTable(Connection, "Table2", ReqValidation: false);
			Table.AddValidatedColumn();
		}

		/// <summary>
		/// Check if the AddValidatedColumn function throws an exception if we call it twice
		/// </summary>
		[TestMethod()]
		[ExpectedException(typeof(ArgumentException), "Argument check succeeded when it shouldn't")]
		public void AddValidatedColumn_Invalid1() {
			Table = CreateTestTable(Connection, "Table2", ReqValidation: false);
			Table.AddValidatedColumn();
			Table.AddValidatedColumn();
		}

		/// <summary>
		/// Check if the AddValidatedColumn function throws an exception if we call it when the table already has a Validated column through its constructor
		/// </summary>
		[TestMethod()]
		[ExpectedException(typeof(ArgumentException), "Argument check succeeded when it shouldn't")]
		public void AddValidatedColumn_Invalid2() => Table.AddValidatedColumn();

		/// <summary>
		/// Check if the RenameColumn function works with valid arguments
		/// </summary>
		[TestMethod()]
		public void RenameColumn_ValidArguments() {
			Table.Insert(new Dictionary<string, dynamic>() { { "StringColumn", "Hello World!" } });
			Table.RenameColumn("StringColumn", "NewStringColumn");

			Assert.IsTrue(JToken.DeepEquals(new JObject {
				{ "Columns", new JObject {
					{"rowid", "Integer" },
					{"NewStringColumn", "String"},
					{"IntegerColumn", "Integer"},
					{"Validated", "Integer" }
				}},
				{ "Rows", new JArray(){
					new JArray(){1, "Hello World!", 0, 0}
				}}
			}, Table.GetRows()));
		}

		/// <summary>
		/// Check if the RenameColumn_DoesntExistCheck function throws an exception if a nonexistent column is specified
		/// </summary>
		[TestMethod()]
		[ExpectedException(typeof(ArgumentException), "Argument check succeeded when it shouldn't")]
		public void RenameColumn_DoesntExistCheck() => Table.RenameColumn("SomeColumn", "SomeOtherColumn");

		/// <summary>
		/// Check if the RenameColumn_DoesntExistCheck function throws an exception if a column already exists with the specified name
		/// </summary>
		[TestMethod()]
		[ExpectedException(typeof(ArgumentException), "Argument check succeeded when it shouldn't")]
		public void RenameColumn_AlreadyExiststCheck() => Table.RenameColumn("StringColumn", "IntegerColumn");

		/// <summary>
		/// Check if the RenameColumn_DoesntExistCheck function throws an exception if the user tries to rename the Validated column
		/// </summary>
		[TestMethod()]
		[ExpectedException(typeof(ArgumentException), "Argument check succeeded when it shouldn't")]
		public void RenameColumn_ValidatedColumnCheck1() => Table.RenameColumn("Validated", "SomeColumn");

		/// <summary>
		/// Check if the RenameColumn_DoesntExistCheck function throws an exception if the user tries to rename the Validated column
		/// </summary>
		[TestMethod()]
		[ExpectedException(typeof(ArgumentException), "Argument check succeeded when it shouldn't")]
		public void RenameColumn_ValidatedColumnCheck2() => Table.RenameColumn("IntegerColumn", "Validated");

		/// <summary>
		/// Check if we can drop a column using valid arguments
		/// </summary>
		[TestMethod()]
		public void DropColumn_ValidArguments() {
			Table.DropColumn("StringColumn");
			Assert.AreEqual(3, Table.GetColumns().Count);
		}

		/// <summary>
		/// Check if the column existence check is working
		/// </summary>
		[TestMethod()]
		[ExpectedException(typeof(ArgumentException), "Argument check succeeded when it shouldn't")]
		public void DropColumn_DoesntExistCheck() => Table.DropColumn("SomeColumn");


		/// <summary>
		/// Check if we can drop a table.
		/// </summary>
		[TestMethod()]
		[ExpectedException(typeof(SQLiteException), "Table not dropped")]
		public void DropTable() {
			Table.DropTable();
			Assert.IsTrue(GenericDataTable.GetTableByName(Connection, "UnitTestTable") == null);
			Connection.Query("SELECT * FROM UnitTestTable");
		}

		/// <summary>
		/// Check if we can get rows out of a datatable using valid arguments
		/// </summary>
		[TestMethod()]
		public void GetRows_ValidArgs() {
			Table.Insert(new Dictionary<string, dynamic> { { "StringColumn", "Value1" }, { "IntegerColumn", 11111 } });
			Table.Insert(new Dictionary<string, dynamic> { { "StringColumn", "Value2" }, { "IntegerColumn", 22222 } });
			Table.Insert(new Dictionary<string, dynamic> { { "StringColumn", "Value3" }, { "IntegerColumn", 33333 } });
			JObject Result = Table.GetRows();

			Assert.IsTrue(JToken.DeepEquals(new JObject {
				{ "Columns", new JObject {
					{"rowid", "Integer" },
					{"StringColumn", "String"},
					{"IntegerColumn", "Integer"},
					{"Validated", "Integer" }
				}},
				{ "Rows", new JArray(){
					new JArray() {1, "Value1", 11111, 0},
					new JArray() {2, "Value2", 22222, 0},
					new JArray() {3, "Value3", 33333, 0},
				}}
			}, Result));
		}

		/// <summary>
		/// Check if we can get rows out of an empty datatable
		/// </summary>
		[TestMethod()]
		public void GetRows_EmptyTable() {
			Assert.IsTrue(JToken.DeepEquals(new JObject {
				{ "Columns", new JObject {
					{"rowid", "Integer" },
					{"StringColumn", "String"},
					{"IntegerColumn", "Integer"},
					{"Validated", "Integer" }
				}},
				{ "Rows", new JArray(){
				}}
			}, Table.GetRows()));
		}

		/// <summary>
		/// Check if the range checks throw an exception when given a negative begin value.
		/// </summary>
		[TestMethod()]
		[ExpectedException(typeof(IndexOutOfRangeException), "Argument check succeeded when it shouldn't")]
		public void GetRows_RangeCheck1() => Table.GetRows(-1);

		/// <summary>
		/// Check if the range checks throw an exception when given a negative end value.
		/// </summary>
		[TestMethod()]
		[ExpectedException(typeof(IndexOutOfRangeException), "Argument check succeeded when it shouldn't")]
		public void GetRows_RangeCheck2() => Table.GetRows(0, -1);

		/// <summary>
		/// Check if the range checks throw an exception when begin is higher than end.
		/// </summary>
		[TestMethod()]
		[ExpectedException(typeof(ArgumentException), "Argument check succeeded when it shouldn't")]
		public void GetRows_RangeCheck3() => Table.GetRows(10, 5);

		/// <summary>
		/// Check if we can retrieve all unvalidated rows in a table
		/// </summary>
		[TestMethod()]
		public void GetUnvalidatedRows_ValidArgs() {
			Table.Insert(new Dictionary<string, dynamic> { { "StringColumn", "Value1" }, { "IntegerColumn", 11111 }, { "Validated", 1 } });
			Table.Insert(new Dictionary<string, dynamic> { { "StringColumn", "Value2" }, { "IntegerColumn", 22222 } });
			Table.Insert(new Dictionary<string, dynamic> { { "StringColumn", "Value3" }, { "IntegerColumn", 33333 } });

			Assert.IsTrue(JToken.DeepEquals(new JObject {
				{ "Columns", new JObject {
					{"rowid", "Integer" },
					{"StringColumn", "String"},
					{"IntegerColumn", "Integer"},
					{"Validated", "Integer" }
				}},
				{ "Rows", new JArray(){
					new JArray() {2, "Value2", 22222, 0},
					new JArray() {3, "Value3", 33333, 0},
				}}
			}, Table.GetUnvalidatedRows()));
		}

		/// <summary>
		/// Check if we can retrieve all unvalidated rows in an empty table
		/// </summary>
		[TestMethod()]
		public void GetUnvalidatedRows_EmptyTable() {
			Assert.IsTrue(JToken.DeepEquals(new JObject {
				{ "Columns", new JObject {
					{"rowid", "Integer" },
					{"StringColumn", "String"},
					{"IntegerColumn", "Integer"},
					{"Validated", "Integer" }
				}},
				{ "Rows", new JArray(){
				}}
			}, Table.GetUnvalidatedRows()));
		}

		/// <summary>
		/// Check if the range checks throw an exception when given a negative begin value.
		/// </summary>
		[TestMethod()]
		[ExpectedException(typeof(IndexOutOfRangeException), "Argument check succeeded when it shouldn't")]
		public void GetUnvalidatedRows_RangeCheck1() => Table.GetUnvalidatedRows(-1);

		/// <summary>
		/// Check if the range checks throw an exception when given a negative end value.
		/// </summary>
		[TestMethod()]
		[ExpectedException(typeof(IndexOutOfRangeException), "Argument check succeeded when it shouldn't")]
		public void GetUnvalidatedRows_RangeCheck2() => Table.GetUnvalidatedRows(0, -1);

		/// <summary>
		/// Check if the range checks throw an exception when begin is higher than end.
		/// </summary>
		[TestMethod()]
		[ExpectedException(typeof(ArgumentException), "Argument check succeeded when it shouldn't")]
		public void GetUnvalidatedRows_RangeCheck3() => Table.GetUnvalidatedRows(10, 5);

		/// <summary>
		/// Check if the Validated column check throws an exception when the table has no Validated column.
		/// </summary>
		[TestMethod()]
		[ExpectedException(typeof(ArgumentException), "Argument check succeeded when it shouldn't")]
		public void GetUnvalidatedRows_Columnheck() {
			Table = CreateTestTable(Connection, "Table2", ReqValidation:false);
			Table.GetUnvalidatedRows();
		}

		/// <summary>
		/// Check if we can update values in a database.
		/// </summary>
		[TestMethod()]
		public void Update_ValidArgs() {
			Table.Insert(new Dictionary<string, dynamic> { { "StringColumn", "Value1" } } );
			Table.Update(1, new Dictionary<string, dynamic>{ {"StringColumn", "Value2"} });

			Assert.IsTrue(JToken.DeepEquals(new JObject {
				{ "Columns", new JObject {
					{"rowid", "Integer" },
					{"StringColumn", "String"},
					{"IntegerColumn", "Integer" },
					{"Validated", "Integer" }
				}},
				{ "Rows", new JArray(){
					new JArray() {1, "Value2", 0, 0},
				}}
			}, Table.GetRows()));
		}

		/// <summary>
		/// Check if we can update data if we specify an invalid row
		/// </summary>
		[TestMethod()]
		[ExpectedException(typeof(ArgumentException), "Argument check succeeded when it shouldn't")]
		public void Update_RowCheck() => Table.Update(1, new Dictionary<string, dynamic> { { "SomeColumn", 1 } });

		/// <summary>
		/// Check if we can update data if we input data with an invalid type
		/// </summary>
		[TestMethod()]
		[ExpectedException(typeof(ArgumentException), "Argument check succeeded when it shouldn't")]
		public void Update_DataTypeCheck() {
			Table.Insert(new Dictionary<string, dynamic> { {"StringColumn", "Value" } });
			Table.Update(1, new Dictionary<string, dynamic> { { "SomeColumn", 1 } });
		}

		/// <summary>
		/// Check if we can update data if we specify an invalid column
		/// </summary>
		[TestMethod()]
		[ExpectedException(typeof(ArgumentException), "Argument check succeeded when it shouldn't")]
		public void Update_ColumnCheck() {
			Table.Insert(new Dictionary<string, dynamic> { { "StringColumn", "Value" } });
			Table.Update(1, new Dictionary<string, dynamic> { { "SomeColumn", "Value2" } });
		}

		/// <summary>
		/// Check if we can delete rows
		/// </summary>
		[TestMethod()]
		public void Delete_ValidArguments() {
			Table.Insert(new Dictionary<string, dynamic> { { "StringColumn", "Value" } });
			Table.Delete(1);

			Assert.IsTrue(JToken.DeepEquals(new JObject {
				{ "Columns", new JObject {
					{"rowid", "Integer" },
					{"StringColumn", "String"},
					{"IntegerColumn", "Integer" },
					{"Validated", "Integer" }
				}},
				{ "Rows", new JArray(){
				}}
			}, Table.GetRows()));
		}

		/// <summary>
		/// Check if we can delete rows if we specify an invalid ID
		/// No point in returning an exception if the column doesn't exist, because we can always assume that the data is gone anyway.
		/// </summary>
		[TestMethod()]
		public void Delete_InvalidColumn() => Table.Delete(1);

		/// <summary>
		/// 
		/// </summary>
		[TestMethod()]
		public void Insert_ValidArguments() {
			Table.Insert(new Dictionary<string, dynamic> { { "StringColumn", "Value1" } });

			Assert.IsTrue(JToken.DeepEquals(new JObject {
				{ "Columns", new JObject {
					{"rowid", "Integer" },
					{"StringColumn", "String"},
					{"IntegerColumn", "Integer" },
					{"Validated", "Integer" }
				}},
				{ "Rows", new JArray(){
					new JArray() {1, "Value1", 0, 0},
				}}
			}, Table.GetRows()));
		}

		/// <summary>
		/// Check if we can insert data if we specify invalid columns
		/// </summary>
		[TestMethod()]
		[ExpectedException(typeof(ArgumentException), "Argument check succeeded when it shouldn't")]
		public void Insert_ColumnCheck() => Table.Insert(new Dictionary<string, dynamic> { { "SomeColumn", "Value1" } });

		/// <summary>
		/// Check if we can insert data if we specify an invalid data type
		/// </summary>
		[TestMethod()]
		public void Insert_DataTypeCheck() => Table.Insert(new Dictionary<string, dynamic> { { "StringColumn", 100 } });

		/// <summary>
		/// Check if we can retrieve a list of columns
		/// </summary>
		[TestMethod()]
		public void GetColumns_Test() {
			Dictionary<string, DataType> Columns = Table.GetColumns();
			Assert.IsTrue(4 == Columns.Count && Columns.SequenceEqual(new Dictionary<string, DataType> { 
				{ "rowid", DataType.Integer }, 
				{ "StringColumn", DataType.String }, 
				{ "IntegerColumn", DataType.Integer }, 
				{"Validated", DataType.Integer } 
			}));
		}

		/// <summary>
		/// Check if Exists returns true when given a valid table
		/// </summary>
		[TestMethod()]
		public void Exists_ValidTable() => Assert.IsTrue(GenericDataTable.Exists(Connection, "Table1"));

		/// <summary>
		/// Check if Exists returns true when given a valid table
		/// </summary>
		[TestMethod()]
		public void Exists_InvalidTable() => Assert.IsFalse(GenericDataTable.Exists(Connection, "SomeTable"));

		/// <summary>
		/// Check if RowExists returns true when given a valid column
		/// </summary>
		[TestMethod()]
		public void RowExists_ValidRow() {
			Table.Insert(new Dictionary<string, dynamic> { { "StringColumn", "Value1" } });
			Assert.IsTrue(Table.RowExists(1));
		}

		/// <summary>
		/// Check if RowExists returns false when given an invalid column
		/// </summary>
		[TestMethod()]
		public void RowExists_InvalidRow() => Assert.IsFalse(Table.RowExists(1));

		/// <summary>
		/// Check if GetTableByName works when given valid arguments
		/// </summary>
		[TestMethod()]
		public void GetTableByName_Valid() => Assert.IsNotNull(GenericDataTable.GetTableByName(Connection, "Table1"));

		/// <summary>
		/// Check if GetTableByName works when given invalid arguments
		/// </summary>
		[TestMethod()]
		public void GetTableByName_Invalid() => Assert.IsNull(GenericDataTable.GetTableByName(Connection, "SomeTable"));

		/// <summary>
		/// Check if we can retrieve a list of table names if we provide valid arguments
		/// </summary>
		[TestMethod()]
		public void GetTableNames_ValidArgs() {
			//Table1 is created during init
			CreateTestTable(Connection, "Table2");
			CreateTestTable(Connection, "Table3");

			List<string> FunctionResult = GenericDataTable.GetTableNames(Connection);
			List<string> TableNames = new List<string>() { "Table1", "Table2", "Table3" };
			Assert.IsTrue(FunctionResult.SequenceEqual(TableNames));
		}

		/// <summary>
		/// Check if we can retrieve a list of table names if we provide invalid arguments
		/// </summary>
		[TestMethod()]
		[ExpectedException(typeof(ArgumentException), "Argument check passed when it shouldn't")]
		public void GetTableNames_InvalidArgs() => GenericDataTable.GetTableNames(Connection, 12345);
	}
}