using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;

namespace Webserver.Data
{
    public class Note
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }

        public Note()
        {

        }

        /// <summary>
        /// Creates a new note.
        /// </summary>
        /// <param name="title">The title of the note.</param>
        /// <param name="text">The text of the note.</param>
        public Note(string title, string text)
        {
            Title = title;
            Text = text;
        }

        /// <summary>
        /// Get all notes.
        /// </summary>
        /// <param name="connection">The SQLite connection.</param>
        /// <returns>A list of all the notes.</returns>
        public static List<Note> GetAllNotes(SQLiteConnection connection) => connection.Query<Note>("SELECT * FROM Notes").AsList();

        /// <summary>
        /// Gets a noe by its title. Returns null if the note doesn't exist.
        /// </summary>
        /// <param name="connection">The SQLite connection.</param>
        /// <param name="title">The note's title.</param>
        /// <returns>A note. Null if the note doesn't exist.</returns>
        public static Note GetNoteByTitle(SQLiteConnection connection, string title) => connection.QueryFirstOrDefault<Note>("SELECT * FROM Notes WHERE Title = @Title", new { title });
    }
}
