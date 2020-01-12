function noteToString(Note, newNote) {
    var string = JSON.stringify({
        title: Note.Title,
        newText: newNote.editNoteText,
        newTitle: newNote.editNoteTitle
    });
    console.log(string);
    return string;
};
export { noteToString };