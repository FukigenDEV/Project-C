import React, { Component } from 'react';
import $ from 'jquery';
import './Style/Notities.css';
import 'bootstrap/dist/css/bootstrap.min.css';

class Notities extends Component {
	constructor(props) {
		super(props)
		this.state = {
			allNotes: []
		}
	}
	componentDidMount() {
		$("#notitieAanmakenKnop").on("click", function (event) {
			$("#alleNotities").hide(250);
			$("#notitieAanmaken").show(250);
		});
		$("#alleNotitiesKnop").on("click", function (event) {
			$("#notitieAanmaken").hide(250);
			$("#alleNotities").show(250);
		});

		$("#add_note_form").on("submit", function (event) {
			event.preventDefault();

			var title = $("#note_title").val();
			var text = $("#note_text").val();

			var xhr = new XMLHttpRequest();

			xhr.open("POST", "/note", true);
			xhr.setRequestHeader("Content-Type", "application/json");

			xhr.onreadystatechange = function () {
				if (xhr.readyState === 4) {
					if (xhr.status === 200) {
						$("#message").html(xhr.responseText);
					}
				}
			}

			var data = JSON.stringify({ "title": title, "text": text });
			xhr.send(data);
		});
		this.getAllNotes();
	}
	getAllNotes = () => {
		fetch('/note?title=')
			.then(notes => {
				return notes.json();
			}).then(allNotes => {
				this.setState({ allNotes });
				console.log(this.state.allNotes)
			})
	}

	render() {
		return (
			<div className="shadow-sm p-3 mb-5 bg-white rounded">
				<h2>Notities</h2>
				<hr />

				<button type="button" id="alleNotitiesKnop" class="btn btn-outline-primary" onClick={this.getAllNotes} style={{ display: "inline-block" }}>
					Alle notities</button>

				<button type="button" id="notitieAanmakenKnop" class="btn btn-outline-primary" style={{ display: "inline-block" }}>
					Notitie aanmaken</button>


				<div id="alleNotities">
					<h5><b>Een overzicht van alle notities:</b></h5>
					<div className="all_notes">
						{this.state.allNotes.map((Note, index) => {
							return <div className="full_note note_border">
								<h6>{Note.Title}</h6>
								<p>
									{Note.Text}
								</p>
							</div>
						})}
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