using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;
using Webserver;
using Webserver.Data;

namespace Webserver_Tests.Data
{
    [TestClass()]
    public class Note_Tests
    {
        public static SQLiteConnection connection;
        public SQLiteTransaction transaction;

        public TestContext TestContext { get; set; }
        [ClassInitialize]
        public static void ClassInit(TestContext _) => connection = Database.Init(true);
        [TestInitialize()]
        public void Init() => transaction = connection.BeginTransaction();
        [TestCleanup()]
        public void Cleanup() => transaction.Rollback();
        [ClassCleanup]
        public static void ClassCleanup() => connection.Close();

        [TestMethod]
        public void Constructor()
        {
            new Note(connection, "Note Title", "Note Text");

            Assert.IsNotNull(Note.GetNoteByTitle(connection, "Note Title"));
        }

        [TestMethod]
        public void ChangeNameTest()
        {
            Note note = new Note(connection, "Some Note", "Some Note Text");

            string oldTitle = note.Title;
            note.Title = "Some Cool Note";

            Assert.IsTrue(oldTitle != note.Title);
        }

        [TestMethod]
        public void GetNoteByTitleTest()
        {
            new Note(connection, "Some Note", "Some Note Text");

            Note noteByTitle = Note.GetNoteByTitle(connection, "Some Note");

            Assert.IsNotNull(noteByTitle);
        }

        [TestMethod]
        public void GetAllNotesTest()
        {
            new Note(connection, "Some Note 1", "Some Note Text 1");
            new Note(connection, "Some Note 2", "Some Note Text 2");
            new Note(connection, "Some Note 3", "Some Note Text 3");

            List<Note> allNotes = Note.GetAllNotes(connection);

            // We added 3 notes, so we expect the list count to be 3.
            Assert.IsTrue(allNotes.Count == 3);
        }
    }
}
