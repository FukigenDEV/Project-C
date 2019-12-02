using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Webserver.Data;

namespace Webserver.API_Endpoints {
	[EndpointURL("/note")]
	internal partial class NoteEndPoint : APIEndpoint {
		/// <summary>
		/// Endpoint for retrieving user notes
		/// </summary>
		public override void GET() {
			// Get required fields
			if ( !RequestParams.ContainsKey("title") ) {
				Send("Missing params", HttpStatusCode.BadRequest);
				return;
			}

			//Check if the title parameter exists. If it doesn't, return all notes.
			if ( RequestParams["title"][0].Length == 0 ) {
				List<Note> notes = Note.GetAllNotes(Connection);
				Send(JsonConvert.SerializeObject(notes), HttpStatusCode.OK);
				return;
			}

			// Check if the specified note exists. If it doesn't, send a 404 Not Found
			Note note = Note.GetNoteByTitle(Connection, RequestParams["title"][0]);
			if ( note == null ) {
				Send("No such note", HttpStatusCode.NotFound);
				return;
			}

			// Build and send response
			JObject JSON = JObject.FromObject(note);
			Send(JSON.ToString(Formatting.None), HttpStatusCode.OK);
		}
	}
}
