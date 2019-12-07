import React, { Component } from 'react';
import $ from 'jquery';
class Gegevens extends Component {
  componentDidMount() {
	var xhr = new XMLHttpRequest();
	var url = "/datatable?department=";
	xhr.open("GET", url, true);

	xhr.setRequestHeader("Content-Type", "application/json");

	xhr.onreadystatechange = function() {
		if (xhr.readyState === 4 && xhr.status >= 200 && xhr.status < 300) {
			var tables = JSON.parse(xhr.responseText);

			if (tables.length === 0) {
				$("#add_row").hide();
			}

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
					// The reversed column "rowid" should not be visible
					if (columnName === "rowid") {
						continue;
					}

					columnElement += "<th>" + columnName + "</th>";
				}
				columnElement += "</tr>"
				
				$("#table").append(columnElement);

				// Build the rows with the entries
				for (var r = 0; r < json[tableName]["Rows"].length; r++) {
					var row = json[tableName]["Rows"][r];

					var rowElement = "<tr>";
					// Start at index 1 (index 0 is the rowid that should not be visible)
					for (var el = 1; el < row.length; el++) {
						rowElement += "<td>" + row[el] + "</td>";
					}
					rowElement += "</tr>"

					$("#table").append(rowElement);
				}

				// Build the last row with the input fields (for adding new data)
				var lastRowElement = "<tr id=\"new_row\">";
				for (var lastColumnName in json[tableName]["Columns"]) {
					// The reversed column "rowid" should not be visible
					if (lastColumnName === "rowid") {
						continue;
					}

					lastRowElement += "<td style=\"border: 1px solid transparent;\"><input type=\"text\" id=\"" + lastColumnName + "\" placeholder=\"" + lastColumnName + "\"></td>";
				}
				lastRowElement += "<tr/>";
				
				$("#table").append(lastRowElement);
			}
		}

		xhr.send();
	});

	$("#add_row").on("click", function() {
		var tableName = $("#tables_dropdown").find(":selected").text();

		// Build the json string that will be sent
		var jsonString = '{ ';
		$("#new_row").find("input").each(function() {
			var key = $(this).attr("id");
			var value = $(this).val();

			if (key === "rowid" || key === "Validated") return;
			
			jsonString += '"' + key + '": "' + value + '"';

			// If the last element is not reached yet, add a comma.
			if ($(this).index("input") !== $("#rows").find("input").length - 1) {
				jsonString += ', '
			}
		});
		jsonString += '}';

		var xhr = new XMLHttpRequest();
		xhr.open("POST", "/data?table=" + tableName, true);
		xhr.setRequestHeader("Content-Type", "application/json");

		xhr.onreadystatechange = function() {
			if (xhr.readyState === 4) {
				// Update the table
				$("#tables_dropdown").change();
			}
		}

		xhr.send(jsonString);
	});
  }

  render() {
    return (
      <div className="shadow-sm p-3 mb-5 bg-white rounded">
		<h2>Gegevens</h2>

		<hr/>

		Table:<br/>
		<div id="tables_dropdown">
			<select>
			</select>
		</div>

		<br/>
		
		<table id="table" style={{width: '100%'}} border="1"></table>

		<br/>

		<button id="add_row">Add row</button>

		<p id="message"></p>
      </div>
    );
  }
}

export default Gegevens;