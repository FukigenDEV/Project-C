using System;
using System.Collections.Generic;
using System.Text;
using static Webserver.Utils;

namespace Webserver {
	
	/// <summary>
	/// Stores information about an API endpoint;
	/// ContentType		-	The ContentType requests to this EndPoint should have.
	/// URL				-	The URL used to reach this endpoint. The wwwroot folder is automatically added.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class EndpointInfo : Attribute {
		public string ContentType;
		public string URL;

		public EndpointInfo(string ContentType, string URL) {
			this.ContentType = ContentType;
			this.URL = URL;
		}
	}

	/// <summary>
	/// If used on an endpoint method, the user session will not be checked for validity.
	/// This means that the method is usable even if you don't supply a session cookie (though the method may require it anyway)
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class SkipSessionCheck : Attribute {}

	/// <summary>
	/// If used on an endpoint method, the method will only be usable by users with the specified permission level or above
	/// All users can use the method if this attribute isn't attached to it.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class PermissionLevel : Attribute {
		public PermLevel Level;
		public PermissionLevel(PermLevel Level) {
			this.Level = Level;
		}
	}
}
