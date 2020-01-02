import React, { Component } from 'react';
import $ from 'jquery';

function getDepartmentOfTable(table) {
  var returnValue;

  var xhr = new XMLHttpRequest();
  xhr.open("GET", "/datatable?name=" + table, false);
  xhr.setRequestHeader("Content-Type", "application/json");

  xhr.onreadystatechange = function() {
	var json = JSON.parse(xhr.responseText);

	returnValue = json["Department"];
  };

  xhr.send();

  return returnValue;
}

function getLoggedInUser() {
  var returnValue;

  var xhr = new XMLHttpRequest();
  xhr.open("GET", "/account?email=CurrentUser", false);
  xhr.setRequestHeader("Content-Type", "application/json");
  
  xhr.onreadystatechange = function() {
	returnValue = JSON.parse(xhr.responseText)[0];
  }

  xhr.send();

  return returnValue;
}

function getRoleInDepartment(user, department) {
  return user["Permissions"][department];
}

function addOptionEvents() {
	$(".validate_row").each(function (i, obj) {
		$(this).on("click", function() {
			var element = $(this)[0];

			var rowId = element.parentElement.parentElement.getAttribute("data-rowid");
			var tableName = $("#table").attr("data-name");
		
			var xhr = new XMLHttpRequest();
			xhr.open("PATCH", "/data?table=" + tableName, false);
			xhr.setRequestHeader("Content-Type", "application/json");
		
			var json = "{" + rowId + ": {\"Validated\": 1}}";
			xhr.send(json);

			$("#tables_dropdown").change();
		});
	});
	
	$('.delete_row').each(function() {
		$(this).on("click", function() {
			var element = $(this)[0];

			var rowId = element.parentElement.parentElement.getAttribute("data-rowid");
			var tableName = $("#table").attr("data-name");
		
			var xhr = new XMLHttpRequest();
			xhr.open("DELETE", "/data?table=" + tableName, false);
			xhr.setRequestHeader("Content-Type", "application/json");
		
			var json = "{ RowIDs: [ " + rowId + " ] }";
			
			xhr.send(json);
	  
			$("#tables_dropdown").change();
		});
	});
}

class Gegevens extends Component {
  componentDidMount() {
	var xhr = new XMLHttpRequest();
	var url = "/datatable";
	xhr.open("GET", url, true);

	xhr.setRequestHeader("Content-Type", "application/json");

	xhr.onreadystatechange = function() {
		if (xhr.readyState === 4 && xhr.status >= 200 && xhr.status < 300) {
			var tables = JSON.parse(xhr.responseText);

			if (tables.length === 0) {
				$("#add_row").hide();
				$("#table_elements").hide();

				$("#no_table_message").text("Er zijn nog geen tabellen.");

				var user = getLoggedInUser();
				var role = user["Permissions"]["Administrators"];

				console.log(user);
				console.log(role);
				console.log(role === "Administrator");

				if (role === "Administrator") {
					$("#new_table").show();
				}
			}

			// Populate the tables drop down
			for (var i = 0; i < tables.length; i++) {
				$("#tables_dropdown select").append("<option value=" + i + ">" + tables[i].Name + "</option>");
			}

			$("#tables_dropdown").change();
		}
	}

	xhr.send();

	$("#delete_table").on("click", function() {
		var tableName = $("#tables_dropdown").find(":selected").text();

		if (window.confirm("Weet u zeker dat de tabel " + tableName + " wilt verwijderen?")) {
			var xhr = new XMLHttpRequest();
			xhr.open("DELETE", "/datatable?table=" + tableName, true);
			xhr.setRequestHeader("Content-Type", "application/json");
				
			xhr.onreadystatechange = function() {
				if (xhr.readyState === 4) {
					// Hard refresh the page
					window.location.reload(true);
				}
			}

			xhr.send();
		}
	});

	$("#tables_dropdown").on("change", function() {
		var tableName = $(this).find(":selected").text();

		if (tableName === "") return;

		$("#table").empty();

		$("#add_row").hide();
		$("#new_table").hide();
		$("#delete_table").hide();

		$("#table").attr("data-name", tableName);

		var xhr = new XMLHttpRequest();
		xhr.open("GET", "/data?table=" + tableName, true);
		xhr.setRequestHeader("Content-Type", "application/json");

		xhr.onreadystatechange = function() {
			if (xhr.readyState === 4) {
				var json = JSON.parse(xhr.responseText);

				var user = getLoggedInUser();
				var tableDepartment = getDepartmentOfTable(tableName);
				var userRole = getRoleInDepartment(user, tableDepartment);

				if (userRole === "User") {
				  $("#permissionInfo").text("U hebt niet de permissie om deze tabel te bekijken.");
				  return;
				} else if (userRole === "DeptMember") {
				  $("#permissionInfo").text("U kunt deze tabel zien en aanpassen.");
				} else if (userRole === "Manager") {
				  $("#permissionInfo").text("U kunt deze tabel zien, aanpassen en valideren.");
				} else if (userRole === "Administrator" || user["Email"] === "Administrator") {
				  $("#permissionInfo").text("U kunt deze tabel zien, aanpassen, valideren en verwijderen.");
				  $("#new_table").show();
				  $("#delete_table").show();
				}

				$("#add_row").show();

				// Build the first row with the column names
				var columnElement = "<tr style=\"background-color: #DDD;\">";
				for (var columnName in json[tableName]["Columns"]) {
					// The reserved column "rowid" should not be visible
					if (columnName === "rowid") {
						continue;
					}

					// Translate "Validated" to "Gevalideerd"
					if (columnName === "Validated") { columnName = "Gevalideerd"; }

					columnElement += "<th>" + columnName + "</th>";
				}
				
				// If user is admin or manager, show the options column
				if ((userRole === "Administrator" || user["Email"] === "Administrator") || userRole === "Manager") {
					columnElement += "<th style=\"width: 200px;\">Opties</th>";
				}

				columnElement += "</tr>";
				
				$("#table").append(columnElement);

				// Build the rows with the entries
				for (var r = 0; r < json[tableName]["Rows"].length; r++) {
					var validated = false;

					var row = json[tableName]["Rows"][r];

					var rowElement = "<tr data-rowid=\"" + row[0] + "\">";
					// Start at index 1 (index 0 is the rowid that should not be visible)
					for (var el = 1; el < row.length; el++) {
						var text = row[el];

						// Change 0/1 to Nee/Ja
						if (el === row.length - 1) {
						  if (text === 0) {
						  	  text = "Nee";
						  } else {
						  	  text = "Ja";
							  validated = true;
						  }
						}

						rowElement += "<td>" + text + "</td>";
					}
					
					// If user is admin or manager, show the buttons in the options rows
					if ((userRole === "Administrator" || user["Email"] === "Administrator") || userRole === "Manager") {
						var validatePart = "<span class=\"validate_row\" style=\"text-decoration: underline; color: #007BFF; cursor: pointer;\">Valideren</span>";
						var deletePart = "<span class=\"delete_row\" style=\"text-decoration: underline; color: #007BFF; cursor: pointer;\">Verwijderen</span>";

						if (validated) {
							rowElement += "<td>" + deletePart + "</td>";
						} else {
							rowElement += "<td>" + validatePart + " | " + deletePart + "</td>";
						}
					}

					rowElement += "</tr>";

					$("#table").append(rowElement);
				}

				// Build the last row with the input fields (for adding new data)
				var lastRowElement = "<tr id=\"new_row\" style=\"background-color: grey;\">";
				for (var lastColumnName in json[tableName]["Columns"]) {
					// "rowid" is auto-incremented and not visible
					if (lastColumnName === "rowid") {
						continue;
					}

					// "Validated" always starts with its default value 0
					if (lastColumnName === "Validated") {
						lastRowElement += "<td style=\"border: 1px solid transparent;\"></td>";
					} else {
						switch (json[tableName]["Columns"][lastColumnName]) {
							case "Integer":
								lastRowElement += "<td style=\"border: 1px solid transparent;\"><input type=\"number\" min=\"0\" id=\"" + lastColumnName + "\" placeholder=\"" + lastColumnName + "\"></td>";
								break;
							case "String":
								lastRowElement += "<td style=\"border: 1px solid transparent;\"><input type=\"text\" id=\"" + lastColumnName + "\" placeholder=\"" + lastColumnName + "\"></td>";
								break;
							case "Real":
								lastRowElement += "<td style=\"border: 1px solid transparent;\"><input type=\"number\" min=\"0\" step=\"any\" id=\"" + lastColumnName + "\" placeholder=\"" + lastColumnName + "\"></td>";
								break;
							case "Blob":
								lastRowElement += "<td style=\"border: 1px solid transparent;\"><input type=\"file\" id=\"" + lastColumnName + "\" placeholder=\"" + lastColumnName + "\"></td>";
								break;
						}
					}
				}

				// If user is admin or manager, show one more row for the options column
				if ((userRole === "Administrator" || user["Email"] === "Administrator") || userRole === "Manager") {
					lastRowElement += "<td style=\"border: 1px solid transparent;\"></td>";
				}

				lastRowElement += "<tr/>";
				
				$("#table").append(lastRowElement);

				addOptionEvents();
			}
		}

		xhr.send();
	});

	$("#add_row").on("click", function() {
		var valid = true;

		$("#new_row").find("input").each(function() {
			if ($(this).val() === "") {
				valid = false;
				return false;
			}
		});

		if (!valid) {
			$("#error_message").text("Vul alle velden in.");

			return;
		}

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
				if (xhr.status >= 200 && xhr.status < 300) {
					// Update the table
					$("#error_message").text("");
					$("#tables_dropdown").change();
				} else {
					$("#error_message").text("De row kon niet worden aangemaakt.");
				}
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

		<p id="no_table_message"></p>

		<div id="new_table" style={{"border-bottom":"15px","display":"none"}}>
			<a href="/index.html?#/dashboard/NewTable" style={{"text-decoration":"underline"}}>Nieuwe tabel aanmaken</a><br/>
			<br/>
		</div>

		<div id="table_elements">
			Tabel:<br/>
			<div id="tables_dropdown">
				<select>
				</select>
				<button id="delete_table" style={{"float":"right","display":"none","width":"200px"}}>Verwijder tabel</button>
			</div>

			<br/>

			<p id="permissionInfo"></p>
		
			<table id="table" style={{width: '100%'}} border="1" data-name=""></table>

			<br/>

			<button id="add_row" style={{'display':'none','width':'300px','height':'50px'}}>Rij toevoegen</button>

			<br/>
			<br/>

			<p id="error_message"></p>
		</div>
      </div>
    );
  }
}

export default Gegevens;