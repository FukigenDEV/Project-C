﻿using System;
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
		public override void DELETE() {
			// Get required fields
			if ( !JSON.TryGetValue<string>("title", out JToken title) ) {
				Response.Send("Missing fields", HttpStatusCode.BadRequest);
				return;
			}

			//Check if the specified note exists. If it doesn't, send a 404 Not Found
			Note note = Note.GetNoteByTitle(Connection, (string)title);
			if ( note == null ) {
				Response.Send("No such note", HttpStatusCode.NotFound);
				return;
			}

			Connection.Delete(note);
			Response.Send("Note successfully deleted", StatusCode: HttpStatusCode.OK);
		}
	}
}
