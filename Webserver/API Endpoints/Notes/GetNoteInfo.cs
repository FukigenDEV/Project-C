using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Collections.Generic;
using System.Text;
using Webserver.Data;

namespace Webserver.API_Endpoints
{
    [EndpointInfo("application/json", "/note")]
    internal partial class NoteEndPoint : APIEndpoint
    {
        public override void GET()
        {
            // Get required fields
            if (!RequestParams.ContainsKey("title"))
            {
                Send("Missing fields", HttpStatusCode.BadRequest);
                return;
            }

            if (RequestParams["title"][0] == "")
            {
                List<Note> notes = Note.GetAllNotes(Connection);
                Send(JsonConvert.SerializeObject(notes), HttpStatusCode.OK);

                return;
            }

            // Check if the specified note exists. If it doesn't, send a 404 Not Found
            Note note = Note.GetNoteByTitle(Connection, RequestParams["title"][0]);
            if (note == null)
            {
                Send("No such note", HttpStatusCode.NotFound);
                return;
            }

            // Build and send response
            JObject JSON = JObject.FromObject(note);
            Send(JSON.ToString(Formatting.None), HttpStatusCode.OK);
        }
    }
}
