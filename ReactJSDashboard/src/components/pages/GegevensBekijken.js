import React, { Component } from 'react';
import $ from 'jquery';
class GegevensBekijken extends Component {
  componentDidMount() {
	var xhr = new XMLHttpRequest();
	var url = "/datatable?department=";
	xhr.open("GET", url, true);

	xhr.setRequestHeader("Content-Type", "application/json");

	xhr.onreadystatechange = function() {
		if (xhr.readyState === 4 && xhr.status === 200) {
			var tables = JSON.parse(xhr.responseText);

			for (var i = 0; i < tables.length; i++) {
				$("#tables_dropdown select").append("<option value=" + i + ">" + tables[i].Name + "</option>");
			}

			$("#tables_dropdown").change();
		}
	}

	xhr.send();

	$("#tables_dropdown").on("change", function() {
		$("#table").empty();
		$("#message").empty();

		var tableName = $(this).find(":selected").text();

		var xhr = new XMLHttpRequest();
		xhr.open("GET", "/data?table=" + tableName, true);
		xhr.setRequestHeader("Content-Type", "application/json");

		xhr.onreadystatechange = function() {
			if (xhr.readyState === 4) {
				var json = JSON.parse(xhr.responseText);

				// Build the first row with the column names
				var columnElement = "<tr style=\"background-color: #DDD;\">";
				for (var columnName in json[tableName]["Columns"]) {
					columnElement += "<th>" + columnName + "</th>";
				}
				columnElement += "</tr>"
				
				$("#table").append(columnElement);

				// Return if there are no rows
				if (json[tableName]["Rows"].length === 0) {
					$("#message").html("<br/>Table <em>" + tableName + "</em> doesn't contain any rows.");

					return;
				}

				// Build the rows with the entries
				for (var r = 0; r < json[tableName]["Rows"].length; r++) {
					var row = json[tableName]["Rows"][r];

					var rowElement = "<tr>";

					for (var el = 0; el < row.length; el++) {
						rowElement += "<td>" + row[el] + "</td>";
					}

					rowElement += "</tr>"

					$("#table").append(rowElement);
				}
			}
		}

		xhr.send();
	});
  }

  render() {
    return (
      <div className="shadow-sm p-3 bg-white rounded">
		Table:<br/>
		<div id="tables_dropdown">
			<select>
			</select>
		</div>

		<br/>
		
		<table id="table" style={{width: '100%'}} border="1"></table>

		<p id="message"></p>
      </div>
    );
  }
}

export default GegevensBekijken;