using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;
using Webserver;
using Webserver.Data;

namespace Webserver_Tests.Data
{
    [TestClass()]
    public class Department_Tests
    {
        public static SQLiteConnection connection;
        public SQLiteTransaction transaction;

        public TestContext TestContext { get; set; }
        [ClassInitialize]
        public static void ClassInit(TestContext _) => connection = Database.Init(true);
        [TestInitialize()]
        public void Init() => transaction = connection.BeginTransaction();
        [TestCleanup()]
        public void Cleanup() => transaction.Rollback();
        [ClassCleanup]
        public static void ClassCleanup() => connection.Close();

        [TestMethod]
        public void Constructor()
        {
            new Department(connection, "Some Department", "A department that was added to test the application.");

            Assert.IsNotNull(Department.GetByName(connection, "Some Department"));
        }

        [TestMethod]
        public void ChangeNameTest()
        {
            Department department = new Department(connection, "Some Department", "A department that was added to test the application.");

            string oldName = department.Name;
            department.Name = "Some Cool Department";

            Assert.IsTrue(oldName != department.Name);
        }

        [TestMethod]
        public void GetDepartmentByNameTest()
        {
            new Department(connection, "Some Department", "A department that was added to test the application.");

            Department departmentByName = Department.GetByName(connection, "Some Department");

            Assert.IsNotNull(departmentByName);
        }

        [TestMethod]
        public void GetAllDepartmentsTest()
        {
            new Department(connection, "Some Department", "A department that was added to test the application.");
            List<Department> allDepartments = Department.GetAllDepartments(connection);

            // The departments Administrators and All Users are there by default, so we should now have a total of 3.
            Assert.IsTrue(allDepartments.Count == 3);
        }
    }
}
