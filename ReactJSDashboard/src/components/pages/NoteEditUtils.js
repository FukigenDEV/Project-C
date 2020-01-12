const patchNote = async (Note, newNote) => {
    console.log("Title: " + Note.Title + "\nText: " + newNote.editNoteText)
    await fetch('/note', {
        method: 'PATCH',
        body: noteToString(Note, newNote),
        headers: {
            'Content-Type': 'application/json'
        }
    }).then(Response => {
        console.log(Response);
        window.location.reload(true);
    }
    );
};

function noteToString(Note, newNote) {
    return JSON.stringify({ title: Note.Title, newText: newNote.editNoteText, newTitle: newNote.editNoteTitle });
}
export { patchNote };