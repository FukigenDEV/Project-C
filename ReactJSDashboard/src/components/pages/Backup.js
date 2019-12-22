import React, { Component } from 'react';
import $ from 'jquery';
class Backup extends Component {
  componentDidMount() {
	$("#backup_button").on("click", function() {
		var xhr = new XMLHttpRequest();
		xhr.open("GET", "/backup", true);
		xhr.setRequestHeader("Content-Type", "application/json");
		
		xhr.onreadystatechange = function() {
			if (xhr.readyState === 4) {
				$("#message").text("De back-up is succesvol uitgevoerd.");
			}
		}
		
		xhr.send();
	});
  }

  render() {
    return (
      <div className="shadow-sm p-3 mb-5 bg-white rounded">
		<h2>Back-up</h2>

		<hr/>

		<button id="backup_button">Back-up maken</button><br/>

		<br/>

		<p id="message"></p>
      </div>
    );
  }
}

export default Backup;