using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Webserver.Data;

namespace Webserver.API_Endpoints {
	internal partial class CompanyEndpoint : APIEndpoint {
		public override void GET() {
			// Get required fields
			if ( !Params.ContainsKey("name") ) {
				Response.Send("Missing params", HttpStatusCode.BadRequest);
				return;
			}

			if ( string.IsNullOrEmpty(Params["name"][0]) ) {
				List<Company> companies = Company.GetAllCompanies(Connection);
				Response.Send(JsonConvert.SerializeObject(companies), HttpStatusCode.OK);

				return;
			}

			// Check if the specified company exists. If it doesn't, send a 404 Not Found
			Company company = Company.GetCompanyByName(Connection, Params["name"][0]);
			if ( company == null ) {
				Response.Send("No such company", HttpStatusCode.NotFound);
				return;
			}

			// Build and send response
			JObject JSON = JObject.FromObject(company);
			Response.Send(JSON.ToString(Formatting.None), HttpStatusCode.OK);
		}
	}
}
