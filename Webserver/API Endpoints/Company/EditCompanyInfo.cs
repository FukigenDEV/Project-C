﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json.Linq;
using Webserver.Data;

namespace Webserver.API_Endpoints {
	internal partial class CompanyEndpoint : APIEndpoint {
		[RequireBody]
		[RequireContentType("application/json")]
		public override void PATCH() {
			// Get required fields
			if ( !Params.ContainsKey("name") ) {
				Response.Send("Missing params", HttpStatusCode.BadRequest);
				return;
			}

			//Check if the specified company exists. If it doesn't, send a 404 Not Found
			Company company = Company.GetCompanyByName(Connection, Params["name"][0]);
			if ( company == null ) {
				Response.Send("No such company", HttpStatusCode.NotFound);
				return;
			}

			// Change necessary fields
			if ( JSON.TryGetValue<string>("newName", out JToken newName) )
				company.Name = (string)newName;
			if ( JSON.TryGetValue<string>("newStreet", out JToken newStreet) )
				company.Street = (string)newStreet;
			if ( JSON.TryGetValue<int>("newHouseNumber", out JToken newHouseNumber) )
				company.HouseNumber = (int)newHouseNumber;
			if ( JSON.TryGetValue<string>("newPostCode", out JToken newPostCode) )
				company.PostCode = (string)newPostCode;
			if ( JSON.TryGetValue<string>("newCity", out JToken newCity) )
				company.City = (string)newCity;
			if ( JSON.TryGetValue<string>("newCountry", out JToken newCountry) )
				company.Country = (string)newCountry;
			if ( JSON.TryGetValue<string>("newPhoneNumber", out JToken newPhoneNumber) )
				company.PhoneNumber = (string)newPhoneNumber;
			if ( JSON.TryGetValue<string>("newEmail", out JToken newEmail) )
				company.Email = (string)newEmail;

			// Update DB row
			Connection.Update<Company>(company);
			Response.Send("Company has successfully been edited.", StatusCode: HttpStatusCode.OK);
		}
	}
}
