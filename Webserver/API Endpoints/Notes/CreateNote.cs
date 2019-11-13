﻿using Dapper.Contrib.Extensions;
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
                !Content.TryGetValue<string>("text", out JToken text))
            {
                Send("Missing fields", HttpStatusCode.BadRequest);
                return;
            }

			//Check if the specified note exists. If it doesn't, send a 404 Not Found
			Note note = Note.GetNoteByTitle(Connection, (string)title);
			if (note != null) {
				Send("Note already exists", HttpStatusCode.BadRequest);
				return;
			}

			Note newNote = new Note((string)title, (string)text);

            // Store note to database
            Connection.Insert(newNote);

            // Send success message
            Send("Note successfully created", HttpStatusCode.OK);
        }
    }
}
