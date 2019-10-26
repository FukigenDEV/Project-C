using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Webserver.API_Endpoints {

	/// <summary>
	/// API endpoint example. Simply create a new class, inherit APIEndpoint, give it the right constructor, and override the HTTP methods you need.
	/// </summary>
	[EndpointInfo("application/json", "/example")]
	class Example : APIEndpoint {
		[PermissionLevel(PermLevel.Manager)]
		public override void GET() {
			Console.WriteLine(RequestParams["null"][0]);

			Send();
		}
	}
}
