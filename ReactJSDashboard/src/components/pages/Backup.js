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
				$("#message").text("Back up successfully performed.");
			}
		}
		
		xhr.send();
	});
  }

  render() {
    return (
		<React.Fragment>
			<p>Backup</p>

			<button id="backup_button">Create a back up.</button><br/>

			<br/>

			<p id="message"></p>
		</React.Fragment>
    );
  }
}

export default Backup;