import React, { Component } from 'react';
import ReactDOM from 'react-dom'
import $ from 'jquery';
import './Style/Notities.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faTrashAlt, fas, faPencilAlt } from '@fortawesome/free-solid-svg-icons';
import { library } from '@fortawesome/fontawesome-svg-core';
import { delay } from 'q';
import { NotitiesEditForm } from '../../index';
library.add(
	fas,
	faTrashAlt,
	faPencilAlt
)


class Notities extends Component {
	constructor(props) {
		super(props)
		this.state = {
			editNote: {},
			editNoteIndex: 99,
			allNotes: [],
			alert: {
				type: 0,
				value: ''
			}
		}
		// this.forceUpdateHandler = this.forceUpdateHandler.bind(this);
	}
	// nadat het component aangemaakt is, wordt de componentDidMount altijd als eerste aangeroepen
	componentDidMount() {
		$("#notitieAanmakenKnop").on("click", function () {
			$("#alleNotities").hide(250);
			$("#notitieAanmaken").show(250);
		});
		$("#alleNotitiesKnop").on("click", function () {
			$("#notitieAanmaken").hide(250);
			$("#alleNotities").show(250);
		});
		// Wanneer de gebruiker zijn tekst heeft ingevoerd en op de submit knop drukt...
		$("#add_note_form").on("submit", function (event) {
			// Wordt de pagina NIET herladen...
			event.preventDefault();
			// En worden de ingetypte waardes in een variabel gezet
			var title = $("#note_title").val();
			var text = $("#note_text").val();

			// Maak een nieuwe XML request naar de server
			var xhr = new XMLHttpRequest();

			// de open method initialiseerd de request, in de backend zoekt hij naar de filename "note", en daarin wordt data gePOST (gegeven aan server), true voor async uitvoeren
			xhr.open("POST", "/note", true);
			// Geeft aan de XHR door dat hij json data moet verwachten
			xhr.setRequestHeader("Content-Type", "application/json");

			// als hij klaar is (4) en als dan de status code 200 is (goed) dan geeft hij een response tekst vanuit de server, dit bevestigd dat je notitie is toegevoegd
			xhr.onreadystatechange = function () {
				if (xhr.readyState === 4) {
					if (xhr.status === 200) {
						$("#message").html(xhr.responseText);
					}
				}
			}
			// Omdat de request asynchroon wordt uitgevoerd, wordt de data hieronder omgezet in JSON data, en daarna opgestuurd
			var data = JSON.stringify({ "title": title, "text": text });
			xhr.send(data);
		});
		// Aan het einde wordt deze functie aangeroepen omdat anders de pagina herladen moest worden om de nieuwe notitie te laten zien
		this.getAllNotes();
	}
	// Deze functie haalt alle gemaakte notities op van de server
	getAllNotes = async () => {
		// in de /note (endpoint URL) in de backend gaat hij zoeken naar alle notes die gemaakt zijn, dit doet hij aan de hand van de titel omdat geen note dezelfde titel mag hebben dus werkt als ID
		await fetch('/note?title=')
			.then(notes => {
				return notes.json();
				// Als er eventueel een note bij is gekomen (of afgegaan) dan veranderd hij dit in de state en dit wordt dan weer gerendered op je scherm
			}).then(allNotes => {
				this.setState({ allNotes });
				console.log(this.state.allNotes)
			})
	}
	// notitie verwijderen
	deleteNote = async (noteName) => {
		// omdat hij async is kan de fetch met await worden gebruikt zodat hij tussendoor eventueel doorkan met een andere functie
		await fetch('/note', {
			// in de note endpoint zoekt hij naar de DELETE functie
			method: 'DELETE',
			// zet de titel in json formaat
			body: JSON.stringify({ title: noteName }),
			// Geeft door dat hij json data moet verwachten 
			headers: {
				'Content-Type': 'application/json'
			}
		});
		// en weer na verwijderen de notities opnieuw ophalen zodat de verandering op je scherm verschijnt
		this.getAllNotes();
	}
	handleTitleChange = event => {
		this.setState({ editNoteTitle: event.target.value });
	};

	handleTextChange = event => {
		this.setState({ editNoteText: event.target.value });
	};

	handleSubmit = () => {
		console.log(this.state.editNoteTitle);
		console.log(this.state.editNoteText);
	};

	// Ik heb een klein pop up scherm die standaard in je browser zit gebruikt om te bevestigen dat men daadwerkelijk de notitie verwijderd
	deleteConfirm = (title) => {
		if (window.confirm("Weet u zeker dat u deze notitie wilt verwijderen?")) {
			this.deleteNote(title)
		}
	}
	getBadgeClasses = () => {
		let classes = 'alert mr-3 ml-3 ';
		classes += (this.state.alert.type === 200) ? "alert-success" : "alert-danger";
		classes += (this.state.alert.type === 0) ? " d-none" : " d-block";
		return classes;
	}
	renderForm = (Note, noteId) => {
		console.log(noteId);
		console.log(Note);
		ReactDOM.render(<NotitiesEditForm note={Note} reloadNote={this.reRenderNote} />, document.getElementById(noteId));
	}
	reRenderNote = (Note) => {
		var noteIdString = "note_" + Note.ID.toString();
		ReactDOM.render(
			<div className="note_border">
				<h6>{Note.Title}</h6>
				<a class="note_edit" onClick={() => { this.renderForm(Note, noteIdString) }}><FontAwesomeIcon icon={['fas', 'pencil-alt']} />
				</a>
				<a class="note_delete" onClick={() => this.deleteConfirm(Note.Title)}><FontAwesomeIcon icon={['fas', 'trash-alt']} /></a>
				<p>
					{Note.Text}
				</p>
			</div>
			, document.getElementById(noteIdString));
	}

	render() {
		return (
			<div className="shadow-sm p-3 bg-white rounded">
				<h2>Notities</h2>
				<hr />

				<button type="button" id="alleNotitiesKnop" class="btn btn-outline-primary" onClick={this.getAllNotes} style={{ display: "inline-block" }}>
					Alle notities</button>

				<button type="button" id="notitieAanmakenKnop" class="btn btn-outline-primary" style={{ display: "inline-block" }}>
					Notitie aanmaken</button>


				<div id="alleNotities">
					<h5><b>Een overzicht van alle notities:</b></h5>
					<div id="all_notes">
						{this.state.allNotes.map(Note => {
							return <div className="full_note" id={"note_" + Note.ID.toString()}>
								<div className="note_border">
									<h6>{Note.Title}</h6>
									<a class="note_edit" onClick={() => { this.renderForm(Note, "note_" + Note.ID.toString()) }}><FontAwesomeIcon icon={['fas', 'pencil-alt']} />
									</a>

									<a class="note_delete" onClick={() => this.deleteConfirm(Note.Title)}><FontAwesomeIcon icon={['fas', 'trash-alt']} /></a>
									<p>
										{Note.Text}
									</p>
								</div>
							</div>
						})}
						<div className={this.getBadgeClasses()}>{this.state.alert.value}</div>
					</div>

				</div>

				<div id="notitieAanmaken" style={{ display: "none" }}>
					<h5><b>Notitie toevoegen:</b></h5>
					<form id="add_note_form" method="POST">
						<label>Titel:</label><br />
						<input id="note_title" type="text" name="note_title" style={{ width: "50%", marginBottom: "15px" }} /><br />
						<label>Tekst:</label><br />
						<textarea id="note_text" type="text" name="note_text" style={{ width: "50%", height: "225px" }} > </textarea> <br />
						<br />
						<input type="submit" value="Toevoegen" style={{ width: "200px" }} /><br /><br />

						<p id="message"></p>
					</form>
				</div>

			</div>
		);
	}
}
export default Notities;