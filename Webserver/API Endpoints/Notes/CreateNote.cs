using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json.Linq;
using Webserver.Data;

namespace Webserver.API_Endpoints {
	internal partial class NoteEndPoint : APIEndpoint {
		[RequireContentType("application/json")]
		[RequireBody]
		public override void POST() {
			// Get all required values
			if ( !JSON.TryGetValue<string>("title", out JToken title) ||
				!JSON.TryGetValue<string>("text", out JToken text) ) {
				Response.Send("Missing fields", HttpStatusCode.BadRequest);
				return;
			}

			//Check if the specified note exists. If it doesn't, send a 404 Not Found
			Note note = Note.GetNoteByTitle(Connection, (string)title);
			if ( note != null ) {
				Response.Send("Note already exists", HttpStatusCode.BadRequest);
				return;
			}

			Note newNote = new Note((string)title, (string)text);

			// Store note to database
			Connection.Insert(newNote);

			// Send success message
			Response.Send("Note succesfully added", HttpStatusCode.OK);
		}
	}
}
