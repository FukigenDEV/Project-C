using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;

namespace Webserver.Data
{
	[Table("Companies")]
    public class Company
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string Street { get; set; }
        public int HouseNumber { get; set; }
        public string PostCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

		/// <summary>
		/// Creates a new company
		/// </summary>
		/// <param name="Name">The company name</param>
		/// <param name="Street">The company address</param>
		/// <param name="HouseNumber">The company address</param>
		/// <param name="PostCode">The company address</param>
		/// <param name="City">The company address</param>
		/// <param name="Country">The company address</param>
		/// <param name="PhoneNumber"The company's phone number></param>
		/// <param name="Email">The company's Email address</param>
		public Company(
			string Name,
			string Street,
			long HouseNumber,
			string PostCode,
			string City,
			string Country,
			string PhoneNumber,
			string Email
		) {
			this.Name = Name;
			this.Street = Street;
			this.HouseNumber = (int)HouseNumber;
			this.PostCode = PostCode;
			this.City = City;
			this.Country = Country;
			this.PhoneNumber = PhoneNumber;
			this.Email = Email;
		}

		/// <summary>
		/// Constructor for deserializing database rows into Company objects.
		/// </summary>
		public Company(
			long ID,
            string Name,
            string Street,
            long HouseNumber,
            string PostCode,
            string City,
            string Country,
            string PhoneNumber,
            string Email
            )
        {
			this.ID = (int)ID;
            this.Name = Name;
            this.Street = Street;
            this.HouseNumber = (int)HouseNumber;
            this.PostCode = PostCode;
            this.City = City;
            this.Country = Country;
            this.PhoneNumber = PhoneNumber;
            this.Email = Email;
        }

        /// <summary>
        /// Lists all companies.
        /// </summary>
        /// <param name="Connection"></param>
        /// <returns></returns>
        public static List<Company> GetAllCompanies(SQLiteConnection Connection) => Connection.Query<Company>("SELECT * FROM Companies").AsList();

        /// <summary>
        /// Get a company's information using its name. Returns null if the company doesn't exist.
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Company GetCompanyByName(SQLiteConnection Connection, string name) => Connection.QueryFirstOrDefault<Company>("SELECT * FROM Companies WHERE Name = @Name", new { name });
    }
}
