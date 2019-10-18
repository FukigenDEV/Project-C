using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Net;
using System.Text;

namespace Webserver.API_Endpoints {

	/// <summary>
	/// API endpoint example. Simply create a new class, inherit APIEndpoint, give it the right constructor, and override the HTTP methods you need.
	/// </summary>
	[EndpointInfo("application/json", "/example")]
	class Example : APIEndpoint {
		[PermissionLevel(PermLevel.Manager)]
		public override void GET() {
			Send("Hello World!");
		}
	}
}
