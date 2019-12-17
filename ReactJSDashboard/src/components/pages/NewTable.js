import React, { Component } from 'react';
import $ from 'jquery';
class NewTable extends Component {
  componentDidMount() {
	$("#add_gdt_form").on("submit", function(event) {
		event.preventDefault();

		var name = $("#gdt_name").val();
		var department = $("#departments_dropdown_gdt").find("option:selected").text();
		var requireValidation = $("#gdt_require_validation").is(":checked") ? true : false;

		var xhr = new XMLHttpRequest();
		xhr.open("POST", "/datatable", true);
		xhr.setRequestHeader("Content-Type", "application/json");

		xhr.onreadystatechange = function() {
			if (xhr.readyState === 4) {
				if (xhr.status >= 200 && xhr.status < 300) {
					$("#gdt_message").text("De tabel is succesvol aangemaakt");
				} else {
					$("#gdt_message").text(xhr.responseText);
				}
			}
		}

		var jsonString = '{ ';
		$("#columns").find("div").each(function(){
			var innerDiv = $(this);

			var name = innerDiv.children("input").val();
			var dataType = innerDiv.children("select").find(":selected").text();

			var pair = '"' + name + '":"' + dataType + '"';
			jsonString += pair;

			// If the last element is not reached yet, add a comma.
			if (innerDiv.index() !== $("#columns").find("div").length - 1) {
				jsonString += ', ';
			}
		});
		jsonString += '}';
		
		var data = JSON.stringify({"Name": name, "Columns": JSON.parse(jsonString), "Department": department, "RequireValidation": requireValidation });
		xhr.send(data);
	});

	$("#add_column").on("click", function(event) {
		event.preventDefault();

		var childString = "<div>" +
			"<select>" +
				"<option value=\"integer\">Integer</option>" +
				"<option value=\"string\">String</option>" +
				"<option value=\"real\">Real</option>" +
				"<option value=\"blob\">Blob</option>" +
			"</select>" +
			"<input type=\"text\" name=\"gdt_column_name\">" +
			"<button class=\"remove_column\" style=\"width: 30px;\">-</button>" +
		"</div>";

		$("#columns").append(childString);

		$(".remove_column").on("click", function(event) {
			event.preventDefault();

			$(this).parent().remove();
		});
	});

	var xhr = new XMLHttpRequest();
	var url = "/department?name=";
	xhr.open("GET", url, true);

	xhr.setRequestHeader("Content-Type", "application/json");

	xhr.onreadystatechange = function() {
		if (xhr.readyState === 4 && xhr.status >= 200 && xhr.status < 300) {
			var departments = JSON.parse(xhr.responseText);

			for (var i = 0; i < departments.length; i++) {
				$("#departments_dropdown_gdt select").append("<option value=" + i + ">" + departments[i].Name + "</option>");
				$("#departments_dropdown_user select").append("<option value=" + i + ">" + departments[i].Name + "</option>");
			}
		}
	}

	xhr.send();
  }

  render() {
    return (
      <div className="shadow-sm p-3 mb-5 bg-white rounded">
		<h2>Nieuwe tabel</h2>

		<div id="add_gdt">
			<form id="add_gdt_form" method="POST">
				Name: <input id="gdt_name" type="text" name="gdt_name"/><br/>

				<br/>

				Columns:

				<template id="column_template">
					<div>
						<select>
							<option value="integer">Integer</option>
							<option value="string">String</option>
							<option value="real">Real</option>
							<option value="blob">Blob</option>
						</select>
						<input type="text" name="gdt_column_name"/>
						<button id="remove_column" onclick="removeColumn(event, this);">-</button>
					</div>
				</template>
		
				<div id="columns">
					<div>
						<select>
							<option value="integer">Integer</option>
							<option value="string">String</option>
							<option value="real">Real</option>
							<option value="blob">Blob</option>
						</select>
						<input type="text" name="gdt_column_name"/>
					</div>
				</div>

				<br/>

				<button id="add_column" style={{width: "200px"}}>+ Add column</button><br/>

				<br/>

				Department:<br/>
				<div id="departments_dropdown_gdt">
					<select>
					</select>
				</div>
		
				<br/>

				Require validation: <input id="gdt_require_validation" type="checkbox" name="gdt_require_validation"/><br/>

				<br/>
				<input type="submit" value="Add"/><br/>

				<br/>

				<p id="gdt_message"></p>
			</form>
		</div>
      </div>
    );
  }
}

export default NewTable;