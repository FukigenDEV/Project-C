import React, { Component } from 'react';
import $ from 'jquery';

class Backup extends Component {
  componentDidMount() {
	var xhr = new XMLHttpRequest();
	xhr.open("GET", "/backup", true);
	xhr.setRequestHeader("Content-Type", "application/json");
		
	xhr.onreadystatechange = function() {
		if (xhr.readyState === 4) {
			var backupList = JSON.parse(xhr.responseText);

			if (backupList.length === 0) {
				$("#no_backups").show();
				$("#no_backups").text("Er zijn momenteel geen back-ups beschikbaar.");

				return;
			}

			for (var i = 0; i < backupList.length; i++) {
				$("#backup_list").append("<p>" + backupList[0] + "</p><a href =\"backup?name=" + backupList[0] + "\">Downloaden</a><hr/>");
			}
		}
	}

	xhr.send();
  }

  render() {
    return (
      <div className="shadow-sm p-3 mb-5 bg-white rounded">
		<h2>Back-up</h2>

		<hr/>

		<p id="no_backups" style={{"display":"none"}}></p>

		<div id="backup_list">
		</div>
      </div>
    );
  }
}

export default Backup;