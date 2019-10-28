using System;
using System.Collections.Generic;
using System.Text;

namespace Webserver.Data
{
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
        /// Constructor for deserializing database rows into Company objects.
        /// </summary>
        public Company(
            string name,
            string street,
            int houseNumber,
            string postCode,
            string city,
            string country,
            string phoneNumber,
            string email
            )
        {
            Name = name;
            Street = street;
            HouseNumber = houseNumber;
            PostCode = postCode;
            City = city;
            Country = country;
            PhoneNumber = phoneNumber;
            Email = email;
        }
    }
}
