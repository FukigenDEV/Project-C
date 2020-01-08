using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json.Linq;
using Webserver.Data;

namespace Webserver.API_Endpoints {
	internal partial class NoteEndPoint : APIEndpoint {
		[RequireContentType("application/json")]
		[RequireBody]
		public override void PATCH() {
			// Get required fields
			if ( !JSON.TryGetValue<string>("title", out JToken title) ) {
				Response.Send("Missing fields", HttpStatusCode.BadRequest);
				return;
			}

			// Check if the specified note exists. If it doesn't, send a 404 Not Found
			Note note = Note.GetNoteByTitle(Connection, (string)title);
			if ( note == null ) {
				Response.Send("No such note", HttpStatusCode.NotFound);
				return;
			}

			// Change title if necessary
			if ( JSON.TryGetValue<string>("newTitle", out JToken newTitle) ) {
				note.Title = (string)newTitle;
			}

			// Change text if necessary
			if ( JSON.TryGetValue<string>("newText", out JToken newText) ) {
				note.Text = (string)newText;
			}

			// Update DB row
			Connection.Update<Note>(note);
			Response.Send("Note has successfully been edited.", StatusCode: HttpStatusCode.OK);
		}
	}
}
