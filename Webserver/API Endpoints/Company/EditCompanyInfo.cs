using Dapper.Contrib.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Webserver.Data;

namespace Webserver.API_Endpoints
{
    internal partial class CompanyEndpoint : APIEndpoint
    {
        public override void PATCH()
        {
            // Get required fields
            if (!JSON.TryGetValue<string>("name", out JToken name))
            {
                Send("Missing fields", HttpStatusCode.BadRequest);
                return;
            }

            // Check if the specified company exists. If it doesn't, send a 404 Not Found
            Company company = Company.GetCompanyByName(Connection, (string)name);
            if (company == null)
            {
                Send("No such company", HttpStatusCode.NotFound);
                return;
            }

            // Change necessary fields
            if (JSON.TryGetValue<string>("newName", out JToken newName))
                company.Name = (string)newName;
            if (JSON.TryGetValue<string>("newStreet", out JToken newStreet))
                company.Street = (string)newStreet;
            if (JSON.TryGetValue<int>("newHouseNumber", out JToken newHouseNumber))
                company.HouseNumber = (int)newHouseNumber;
            if (JSON.TryGetValue<string>("newPostCode", out JToken newPostCode))
                company.PostCode = (string)newPostCode;
            if (JSON.TryGetValue<string>("newCity", out JToken newCity))
                company.City = (string)newCity;
            if (JSON.TryGetValue<string>("newCountry", out JToken newCountry))
                company.Country = (string)newCountry;
            if (JSON.TryGetValue<string>("newPhoneNumber", out JToken newPhoneNumber))
                company.PhoneNumber = (string)newPhoneNumber;
            if (JSON.TryGetValue<string>("newEmail", out JToken newEmail))
                company.Email = (string)newEmail;

            // Update DB row
            Connection.Update<Company>(company);
            Send("Company has successfully been edited.", StatusCode: HttpStatusCode.OK);
        }
    }
}
