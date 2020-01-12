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
    public class Company_Tests
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
            new Company(connection, "Company name", "Street", 123, "Post code", "City", "Country", "Phone number", "Email");

            Assert.IsNotNull(Company.GetCompanyByName(connection, "Company name"));
        }

        [TestMethod]
        public void ChangeNameTest()
        {
            Company company = new Company(connection, "Company name", "Street", 123, "Post code", "City", "Country", "Phone number", "Email");

            string oldName = company.Name;
            company.Name = "New company name";

            Assert.IsTrue(oldName != company.Name);
        }

        [TestMethod]
        public void GetCompanyByNameTest()
        {
            new Company(connection, "Company name", "Street", 123, "Post code", "City", "Country", "Phone number", "Email");

            Company companyByName = Company.GetCompanyByName(connection, "Company name");

            Assert.IsNotNull(companyByName);
        }

        [TestMethod]
        public void GetAllCompaniesTest()
        {
            new Company(connection, "Company name 1", "Street", 123, "Post code", "City", "Country", "Phone number", "Email");
            new Company(connection, "Company name 2", "Street", 123, "Post code", "City", "Country", "Phone number", "Email");
            new Company(connection, "Company name 3", "Street", 123, "Post code", "City", "Country", "Phone number", "Email");

            List<Company> allCompanies = Company.GetAllCompanies(connection);

            System.Diagnostics.Debug.WriteLine(allCompanies.Count);

            // We added 3 companies, so we expect the list count to be 3.
            Assert.IsTrue(allCompanies.Count == 3);
        }
    }
}
