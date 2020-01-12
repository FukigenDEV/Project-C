jest.mock('./NoteEditUtils');

const { noteToString } = require('./NoteEditUtils');

const Note = {
    ID: 1,
    Title: "TestNote",
    Text: "This a test"
}

const editNote = {
    editNoteText: "A new note",
    editNoteTitle: "That's very fanceh"
}

test('Should give back the edited and stringified note', () => {
    var expectedResponse = "{\"title\":\"TestNote\",\"newText\":\"A new note\",\"newTitle\":\"That's very fanceh\"}";
    expect(noteToString(Note, editNote)).toBe(expectedResponse);

});