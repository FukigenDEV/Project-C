using System;

namespace Webserver {

	/// <summary>
	/// Stores the URL that this endpoint can be reached at.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class EndpointURL : Attribute {
		public string URL;

		public EndpointURL(string URL) {
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
	/// If used on an endpoint method, any calls to this method will automatically be rejected with a 400 Bad Request if the request body is empty.
	/// </summary>
	public sealed class RequireBody : Attribute { }

	/// <summary>
	/// If used on an endpoint method, any requests to it will only be accepted if the content type matches.
	/// Will also be used to set the content type for the response sent using APIEndpoint.Send, unless manually overriden.
	/// </summary>
	public sealed class RequireContentType : Attribute {
		public string ContentType;
		public RequireContentType(string ContentType) {
			this.ContentType = ContentType;
		}
	}
}
