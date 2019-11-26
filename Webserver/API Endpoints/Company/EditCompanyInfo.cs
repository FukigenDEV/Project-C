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
            if (!Content.TryGetValue<string>("name", out JToken name))
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
            if (Content.TryGetValue<string>("newName", out JToken newName))
                company.Name = (string)newName;
            if (Content.TryGetValue<string>("newStreet", out JToken newStreet))
                company.Street = (string)newStreet;
            if (Content.TryGetValue<int>("newHouseNumber", out JToken newHouseNumber))
                company.HouseNumber = (int)newHouseNumber;
            if (Content.TryGetValue<string>("newPostCode", out JToken newPostCode))
                company.PostCode = (string)newPostCode;
            if (Content.TryGetValue<string>("newCity", out JToken newCity))
                company.City = (string)newCity;
            if (Content.TryGetValue<string>("newCountry", out JToken newCountry))
                company.Country = (string)newCountry;
            if (Content.TryGetValue<string>("newPhoneNumber", out JToken newPhoneNumber))
                company.PhoneNumber = (string)newPhoneNumber;
            if (Content.TryGetValue<string>("newEmail", out JToken newEmail))
                company.Email = (string)newEmail;

            // Update DB row
            Connection.Update<Company>(company);
            Send("Company has successfully been edited.", StatusCode: HttpStatusCode.OK);
        }
    }
}
