﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Webserver {
	
	/// <summary>
	/// Stores information about an API endpoint;
	/// ContentType		-	The ContentType requests to this EndPoint should have.
	/// URL				-	The URL used to reach this endpoint. The wwwroot folder is automatically added.
	/// </summary>
	public sealed class EndpointInfo : Attribute {
		public string ContentType;
		public string URL;

		public EndpointInfo(string ContentType, string URL) {
			this.ContentType = ContentType;
			this.URL = URL;
		}
	}
}
