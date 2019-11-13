using Dapper.Contrib.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Webserver.Data;

namespace Webserver.API_Endpoints
{
    internal partial class NoteEndPoint : APIEndpoint
    {
        public override void DELETE()
        {
            // Get required fields
            if (!Content.TryGetValue<string>("title", out JToken title))
            {
                Send("Missing fields", HttpStatusCode.BadRequest);
                return;
            }

            //Check if the specified note exists. If it doesn't, send a 404 Not Found
            Note note = Note.GetNoteByTitle(Connection, (string)title);
            if (note == null)
            {
                Send("No such note", HttpStatusCode.NotFound);
                return;
            }

            Connection.Delete(note);
            Send("Note successfully deleted", StatusCode: HttpStatusCode.OK);
        }
    }
}
