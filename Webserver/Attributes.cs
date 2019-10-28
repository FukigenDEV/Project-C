using System;

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

	/// <summary>
	/// If used on an endpoint method, any calls to this method will automatically be rejected with a 400 Bad Request.
	/// </summary>
	public sealed class RequireBody : Attribute { }
}
