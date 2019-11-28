using Dapper.Contrib.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Collections.Generic;
using System.Text;
using Webserver.Data;

namespace Webserver.API_Endpoints
{
    internal partial class NoteEndPoint : APIEndpoint
    {
        public override void POST()
        {
            // Get all required values
            if (!Content.TryGetValue<string>("title", out JToken title) ||
                !Content.TryGetValue<string>("text", out JToken text) ||
                !Content.TryGetValue<string>("author", out JToken author))
            {
                Send("Missing fields", HttpStatusCode.BadRequest);
                return;
            }

            // Check if the author is valid
            User user = User.GetUserByEmail(Connection, (string)author);
            if (user == null)
            {
                Send("No such user", HttpStatusCode.BadRequest);
                return;
            }

			//Check if the specified note exists. If it doesn't, send a 404 Not Found
			Note note = Note.GetNoteByTitle(Connection, (string)title);
			if (note != null) {
				Send("Note already exists", HttpStatusCode.BadRequest);
				return;
			}

			Note newNote = new Note((string)title, (string)text, user.ID);

            // Store note to database
            Connection.Insert(newNote);

            // Send success message
            Send("Note successfully created", HttpStatusCode.OK);
        }
    }
}
