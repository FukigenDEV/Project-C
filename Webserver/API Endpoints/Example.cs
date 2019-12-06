﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Webserver.API_Endpoints {

	/// <summary>
	/// API endpoint example. Simply create a new class, inherit APIEndpoint, give it the right constructor, and override the HTTP methods you need.
	/// Also used for testing purposes.
	/// </summary>
	[EndpointURL("/example")]
	internal class Example : APIEndpoint {
		[PermissionLevel(PermLevel.Manager)]
		[RequireContentType("application/json")]
		public override void GET() {
			Console.WriteLine(Params["null"][0]);

			Response.Send();
		}
	}
}
