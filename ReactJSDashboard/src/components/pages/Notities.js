import React, { Component } from 'react';
import $ from 'jquery';
class Notities extends Component {
  componentDidMount() {
	$("#add_note_form").on("submit", function(event) {
		event.preventDefault();

		var title = $("#note_title").val();
		var text = $("#note_text").val();

		var xhr = new XMLHttpRequest();
		
		xhr.open("POST", "/note", true);
		xhr.setRequestHeader("Content-Type", "application/json");

		xhr.onreadystatechange = function() {
			if (xhr.readyState === 4) {
				if (xhr.status === 200) {
					$("#message").html(xhr.responseText);
				}
			}
		}
		
		var data = JSON.stringify({"title": title, "text": text});
		xhr.send(data);
	});
		
	var xhr = new XMLHttpRequest();
		
	xhr.open("GET", "/note?title=", true);
	xhr.setRequestHeader("Content-Type", "application/json");

	xhr.onreadystatechange = function() {
		if (xhr.readyState === 4) {
			$("#all_notes").html(xhr.responseText);
		}
	}

	xhr.send();
  }

  render() {
    return (
      <div className="shadow-sm p-3 mb-5 bg-white rounded">
        <h2>Notities</h2>
		<hr/>

		<p><b>Alle notities</b></p>
		<p id="all_notes"></p>

		<p><b>Notitie toevoegen</b></p>
		<form id="add_note_form" method="POST">
			<label>Titel:</label><br/>
			<input id="note_title" type="text" name="note_title" style={{width: "50%", marginBottom: "15px"}}/><br/>
			<label>Tekst:</label><br/>
			<input id="note_text" type="text" name="note_text" style={{width: "50%", height: "225px"}}/><br/>
			<br/>
			<input type="submit" value="Toevoegen" style={{width: "200px"}}/><br/>

			<p id="message"></p>
		</form>
      </div>
    );
  }
}

export default Notities;