using Newtonsoft.Json.Linq;
using System;

namespace Webserver.API_Endpoints {

	/// <summary>
	/// API endpoint example. Simply create a new class, inherit APIEndpoint, give it the right constructor, and override the HTTP methods you need.
	/// </summary>
	[EndpointInfo("application/json", "/example")]
	class Example : APIEndpoint {
		[PermissionLevel(PermLevel.Manager)]
		public override void GET() {
			Console.WriteLine(Content.TryGetValue("test", out JToken Value));
			Console.WriteLine(Content.TryGetValue<string>("test", out JToken Value2));

			Send();
		}
	}
}
