using System;

namespace Webserver {

	/// <summary>
	/// Stores the URL that this endpoint can be reached at.
	/// If an endpoint is missing this attribute, it will be inaccessible and a warning will be shown in the console.
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
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class RequireBody : Attribute { }

	/// <summary>
	/// If used on an endpoint method, any requests to it will only be accepted if the content type matches.
	/// Any JSON in request content bodies will not be converted to a JObject unless this attribute is set to application/json. If the
	/// JSON cannot be parsed and the RequireBody attribute is present, a 400 Bad Request will be sent to the client.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class RequireContentType : Attribute {
		public string ContentType;
		public RequireContentType(string ContentType) {
			this.ContentType = ContentType;
		}
	}
}
