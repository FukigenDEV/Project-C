using Microsoft.VisualStudio.TestTools.UnitTesting;
using Webserver.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using Configurator;

namespace Webserver.Data.Tests {
	[TestClass()]
	public class User_Tests{

		public static SQLiteConnection Connection;
		public static GenericDataTable Table;
		public SQLiteTransaction Transaction;
		public TestContext TestContext { get; set; }
		[ClassInitialize]
		public static void ClassInit(TestContext _) => Connection = Database.Init(true);
		[TestInitialize()]
		public void Init() => Transaction = Connection.BeginTransaction();
		[TestCleanup()]
		public void Cleanup() => Transaction.Rollback();
		[ClassCleanup]
		public static void ClassCleanup() => Connection.Close();

		/// <summary>
		/// Check if we can create a new user using the constructor
		/// </summary>
		[TestMethod]
		public void Constructor() {
			new User("user@example.com", "W@chtw00rd", Connection);
			Assert.IsNotNull(User.GetUserByEmail(Connection, "user@example.com"));
		}

		/// <summary>
		/// Check if we can change a user's password
		/// </summary>
		[TestMethod]
		public void ChangePasswordTest() {
			User Acc = new User("user@example.com", "W@chtw00rd", Connection);
			string OldHash = Acc.PasswordHash;
			Acc.ChangePassword(Connection, "SomePassword");
			Assert.IsTrue(OldHash != Acc.PasswordHash);
		}

		/// <summary>
		/// Check if the Administrator permission level takes priority over all others. Admin users should always show up as admin regardless
		/// of what department you specify
		/// </summary>
		[TestMethod]
		public void GetPermissionLevelAdmin() {
			new Department(Connection, "SomeDepartment");
			User Admin = User.GetUserByEmail(Connection, "Administrator");
			Assert.IsTrue(Admin.GetPermissionLevel(Connection, 1) == PermLevel.Administrator);
			Assert.IsTrue(Admin.GetPermissionLevel(Connection, 3) == PermLevel.Administrator);
		}

		/// <summary>
		/// Check if we can retrieve a user's permission level
		/// </summary>
		[TestMethod]
		public void GetPermissionLevelUser() {
			new Department(Connection, "SomeDepartment");
			User SomeUser = new User("user@example.com", "W@chtw00rd", Connection);
			SomeUser.SetPermissionLevel(Connection, PermLevel.Manager, 3);
			Assert.IsTrue(SomeUser.GetPermissionLevel(Connection, 1) == PermLevel.User);
			Assert.IsTrue(SomeUser.GetPermissionLevel(Connection, 3) == PermLevel.Manager);
		}

		/// <summary>
		/// Check if IsAdmin properly returns True for admin users
		/// </summary>
		[TestMethod]
		public void IsAdmin() => Assert.IsTrue(User.GetUserByEmail(Connection, "Administrator").IsAdmin(Connection));

		/// <summary>
		/// Check if IsAdmin properly returns False for non-admin users
		/// </summary>
		[TestMethod]
		public void IsAdmin2() {
			new User("user@example.com", "SomePassword123", Connection);
			Assert.IsFalse(User.GetUserByEmail(Connection, "user@example.com").IsAdmin(Connection));
		}

		/// <summary>
		/// Check if we can change a user's permission level
		/// </summary>
		[TestMethod]
		public void SetPermissionLevelUser() {
			new Department(Connection, "SomeDepartment");
			User SomeUser = new User("user@example.com", "W@chtw00rd", Connection);
			Assert.IsTrue(SomeUser.GetPermissionLevel(Connection, 3) == PermLevel.User);
			SomeUser.SetPermissionLevel(Connection, PermLevel.Manager, 3);
			Assert.IsTrue(SomeUser.GetPermissionLevel(Connection, 3) == PermLevel.Manager);
		}

		/// <summary>
		/// Check if we can set a user's permission level to Admin in the Administrators department and have it override all other permissions
		/// </summary>
		[TestMethod]
		public void SetPermissionLevelTest1() {
			new Department(Connection, "SomeDepartment");
			User SomeUser = new User("user@example.com", "W@chtw00rd", Connection);
			Assert.IsTrue(SomeUser.GetPermissionLevel(Connection, 3) == PermLevel.User);
			SomeUser.SetPermissionLevel(Connection, PermLevel.Administrator, 1);
			Assert.IsTrue(SomeUser.GetPermissionLevel(Connection, 3) == PermLevel.Administrator);
		}

		/// <summary>
		/// Check if CreateHash works. If two calls with the same input both return the same output, the function works.
		/// </summary>
		[TestMethod]
		public void CreateHashTest() => Assert.IsTrue(User.CreateHash("user@example.com", "SomePassword123") == User.CreateHash("user@example.com", "SomePassword123"));

		/// <summary>
		/// Check if we can retrieve user objects using their email address
		/// </summary>
		[TestMethod]
		public void GetUserByEmailTest() {
			User Acc = User.GetUserByEmail(Connection, "Administrator");
			Assert.IsNotNull(Acc);
			Assert.IsTrue(Acc.Email == "Administrator");
		}

		/// <summary>
		/// Check if we can retrieve a list of all existing email addresses
		/// </summary>
		[TestMethod]
		public void GetAllEmailsTest() {
			new User("user@example.com", "W@chtw00rd", Connection);
			List<string> Emails = User.GetAllEmails(Connection);
			Assert.IsTrue(Emails.Count == 2);
			Assert.IsTrue(Emails.Contains("Administrator") && Emails.Contains("user@example.com"));
		}

		/// <summary>
		/// Check if we can get a list of all users
		/// </summary>
		[TestMethod]
		public void GetAllUsersTest() {
			new User("user@example.com", "W@chtw00rd", Connection);
			List<User> Emails = User.GetAllUsers(Connection);
			Assert.IsTrue(Emails.Count == 2);
		}
	}
}