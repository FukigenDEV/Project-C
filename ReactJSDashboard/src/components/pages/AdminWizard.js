import React, { Component } from 'react';
import $ from 'jquery';
class AdminWizard extends Component {
  componentDidMount() {
	$("#begin_button").on("click", function(event) {
		$("#intro").hide(250);
		$("#add_company").show(250);
	});
	
	///////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////// Begin of Company code ////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////
	$("#add_company_form").on("submit", function(event) {
		event.preventDefault();

		var name = $("#company_name").val();
		var street = $("#company_street").val();
		var houseNumber = $("#company_house_number").val();
		var postCode = $("#company_post_code").val();
		var city = $("#company_city").val();
		var country = $("#company_country").val();
		var phoneNumber = $("#company_phone_number").val();
		var email = $("#company_email").val();

		var xhr = new XMLHttpRequest();
		xhr.open("POST", "/company", true);
		xhr.setRequestHeader("Content-Type", "application/json");

		xhr.onreadystatechange = function() {
			if (xhr.readyState === 4) {
				if (xhr.status >= 200 && xhr.status < 300) {
					$("#add_company").hide(250);
					$("#add_department").show(250);
				} else {
					$("#company_message").html(xhr.responseText);
				}
			}
		}
		
		var data = JSON.stringify({"name": name, "street": street, "houseNumber": houseNumber, "postCode": postCode, "city": city, "country": country, "phoneNumber": phoneNumber, "email": email });
		xhr.send(data);
	});
	///////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////// End of Company code //////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////

	
	///////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////// Begin of Department code /////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////
	$("#add_department_form").on("submit", function(event) {
		event.preventDefault();

		var name = $("#department_name").val();
		var description = $("#department_description").val();

		var xhr = new XMLHttpRequest();
		xhr.open("POST", "/department", true);
		xhr.setRequestHeader("Content-Type", "application/json");

		xhr.onreadystatechange = function() {
			if (xhr.readyState === 4) {
				if (xhr.status >= 200 && xhr.status < 300) {
					$("#department_message").html("Afdeling is succesvol aangemaakt.");
				} else {
					$("#department_message").html("De afdeling kon niet worden aangemaakt.");
				}

				$('#department_message').css({"display": "block"});
				$('#department_message').delay(2000).fadeOut(300);

				if (xhr.status >= 200 && xhr.status < 300) {
					$("#add_department_form").trigger("reset");
				}
			}
		}
		
		var data = JSON.stringify({"name": name, "description": description});
		xhr.send(data);
	});

	$("#department_next_form").on("submit", function(event) {
		$("#add_department").hide(250);
		$("#add_user").show(250);
	});
	///////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////// End of Department code ///////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////
	
	///////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////// Begin of User code ///////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////
	$("#add_user_form").on("submit", function(event) {
		event.preventDefault();

		var email = $("#user_email").val();
		var password = $("#user_password").val();
		var firstName = $("#user_first_name").val();
		var middleInitial = $("#user_middle_initial").val();
		var lastName = $("#user_last_name").val();
		var function_ = $("#user_function").val();
		var phoneNumberWork = $("#user_phone_number_work").val();
		var phoneNumberPrivate = $("#user_phone_number_private").val();
		var dateOfBirth = $("#user_date_of_birth").val();
		var country = $("#user_country").val();
		var address = $("#user_address").val();
		var postCode = $("#user_post_code").val();
		var accountType = $("input[name=user_account_type]:checked").val();
		var department = $("#departments_dropdown_user option:selected").text();

		var xhr = new XMLHttpRequest();
		xhr.open("POST", "/account", true);
		xhr.setRequestHeader("Content-Type", "application/json");

		xhr.onreadystatechange = function() {
			if (xhr.readyState === 4) {
				if (xhr.status >= 200 && xhr.status < 300) {
					$("#user_message").html("De gebruiker is succesvol aangemaakt.");
				} else {
					$("#user_message").html("De gebruiker kon niet worden aangemaakt.");
				}

				$('#user_message').css({"display": "block"});
				$('#user_message').delay(2000).fadeOut(300);

				if (xhr.status >= 200 && xhr.status < 300) {
					$("#add_user_form").trigger("reset");
				}
			}
		}
		
		var data = JSON.stringify({"Email": email, "Password": password, "AccountType": accountType, "MemberOf": department });
		xhr.send(data);
	});

	$("#user_next_form").on("submit", function(event) {
		$("#add_user").hide(250);
		$("#add_gdt").show(250);
	});
	///////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////// End of User code /////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////
	
	///////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////// Begin of Generic data table code /////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////
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
					$("#gdt_message").text("De tabel is succesvol aangemaakt.");	
				} else {
					$("#gdt_message").text("De tabel kon niet worden aangemaakt.");
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

	$("#gdt_next_form").on("submit", function(event) {
		$("#add_gdt").hide(250);
		$("#finished").show(250);

		// Local storage should remember that de admin wizard has been completed
		window.localStorage.setItem("adminWizardDone", "true");
	});

	///////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////// End of Generic data table code ///////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////
  }

  render() {
    return (
      <div className="shadow-sm p-3 mb-5 bg-white rounded">
        <h2>Admin wizard</h2>

		<hr/>

		<div id="intro" style={{display: "block"}}>
			<p><strong>De admin wizard zal u door het volgende heen leiden:</strong><br/>
			- Uw bedrijf toevoegen<br/>
			- Afdelingen toevoegen<br/>
			- Gebruikers toevoegen<br/>
			- Tabellen met kolommen toevoegen</p>

			<p>De admin wizard zal een paar minuten duren. U kunt meteen de applicatie gebruiken als uw het heeft afgerond.</p>

			<button id="begin_button">Beginnen</button>
		</div>
		
		<div id="add_company" style={{display: "none"}}>
			<p><b>Stap 1: Uw bedrijf toevoegen</b><br/>
			Voeg uw bedrijf toe aan de applicatie. U kunt slechts één bedrijf toevoegen.</p>

			<form id="add_company_form" method="POST">
				Naam: <input id="company_name" type="text" name="company_name"/><br/>
				Straatnaam: <input id="company_street" type="text" name="company_street"/><br/>
				Huisnummer: <input id="company_house_number" type="number" name="company_house_number"/><br/>
				Postcode: <input id="company_post_code" type="text" name="company_post_code"/><br/>
				Woonplaats: <input id="company_city" type="text" name="company_city"/><br/>
				Land: <input id="company_country" type="text" name="company_country"/><br/>
				Telefoonnummer: <input id="company_phone_number" type="text" name="company_phone_number"/><br/>
				E-mailadres: <input id="company_email" type="email" name="company_email"/><br/>
				<br/>
				<input type="submit" value="Toevoegen"/><br/>

				<p id="company_message"></p>
			</form>
		</div>

		<div id="add_department" style={{display: "none"}}>
			<p><b>Stap 2: Afdelingen toevoegen</b><br/>
			Voeg afdelingen toe aan uw bedrijf.</p>

			<form id="add_department_form" method="POST">
				Naam van afdeling: <input id="department_name" type="text" name="department_name"/><br/>
				Omschrijving van afdeling: <input id="department_description" type="text" name="department_description"/><br/>
				<br/>
				<input type="submit" value="Toevoegen"/><br/>

				<p id="department_message"></p>
			</form>

			<hr/>

			<form id="department_next_form">
				<input type="submit" value="Volgende"/><br/>
			</form>
		</div>

		<div id="add_user" style={{display: "none"}}>
			<p><b>Stap 3: Gebruikers toevoegen</b><br/>
			Voeg gebruikers toe aan uw bedrijf.</p>

			<form id="add_user_form" method="POST">
				E-mailadres: <input id="user_email" type="email" name="user_email"/><br/>
				Wachtwoord: <input id="user_password" type="password" name="user_password"/><br/>
				Voornaam: <input id="user_first_name" type="text" name="user_first_name"/><br/>
				Tussenvoegsel: <input id="user_middle_initial" type="text" name="user_middle_initial"/><br/>
				Achternaam: <input id="user_last_name" type="text" name="user_last_name"/><br/>
				Functie: <input id="user_function" type="text" name="user_function"/><br/>
				Telefoonnummer (werk): <input id="user_phone_number_work" type="number" name="user_phone_number_work"/><br/>
				Telefoonnummer (privé): <input id="user_phone_number_private" type="number" name="user_phone_number_private"/><br/>
				Geboortedatum: <input id="user_date_of_birth" type="date" name="user_date_of_birth"/><br/>
				Land: <input id="user_country" type="text" name="user_country"/><br/>
				Adres: <input id="user_address" type="text" name="user_address"/><br/>
				Postcode: <input id="user_post_code" type="text" name="user_post_code"/><br/>

				Accounttype:<br/>
				<input id="user_account_type_administrator" type="radio" name="user_account_type" value="Administrator"/>Administrator<br/>
				<input id="user_account_type_manager" type="radio" name="user_account_type" value="Manager"/>Manager<br/>
				<input id="user_account_type_department_member" type="radio" name="user_account_type" value="Lid van Afdeling"/>Department member<br/>
				<input id="user_account_type_user" type="radio" name="user_account_type"  value="Gebuiker" checked/>User<br/>

				Afdeling:<br/>
				<div id="departments_dropdown_user">
					<select>
					</select>
				</div>

				<br/>
				<input type="submit" value="Toevoegen"/><br/>

				<p id="user_message"></p>
			</form>

			<hr/>

			<form id="user_next_form">
				<input type="submit" value="Volgende"/><br/>
			</form>
		</div>

		<div id="add_gdt" style={{display: "none"}}>
			<p><b>Stap 4: Tabellen met kolommen toevoegen</b><br/>
			Door het toevoegen van tabellen kunnen de gebruikers van de applicatie gegevens registreren.</p>

			<form id="add_gdt_form" method="POST">
				Naam: <input id="gdt_name" type="text" name="gdt_name"/><br/>

				<br/>

				Kolommen:

				<template id="column_template">
					<div>
						<select>
							<option value="integer">Getal</option>
							<option value="string">Woord</option>
							<option value="real">Decimaal</option>
							<option value="blob">Bestand</option>
						</select>
						<input type="text" name="gdt_column_name"/>
						<button id="remove_column" onclick="removeColumn(event, this);">-</button>
					</div>
				</template>
		
				<div id="columns">
					<div>
						<select>
							<option value="integer">Getal</option>
							<option value="string">Woord</option>
							<option value="real">Decimaal</option>
							<option value="blob">Bestand</option>
						</select>
						<input type="text" name="gdt_column_name"/>
					</div>
				</div>

				<br/>

				<button id="add_column" style={{width: "200px"}}>+ Kolom toevoegen</button><br/>

				<br/>

				Afdeling:<br/>
				<div id="departments_dropdown_gdt">
					<select>
					</select>
				</div>
		
				<br/>

				Goedkeuring vereist: <input id="gdt_require_validation" type="checkbox" name="gdt_require_validation"/><br/>

				<br/>
				<input type="submit" value="Toevoegen"/><br/>

				<br/>

				<p id="gdt_message"></p>
			</form>

			<hr/>

			<form id="gdt_next_form">
				<input type="submit" value="Volgende"/><br/>
			</form>
		</div>

		<div id="finished" style={{display: "none"}}>
			<p><b>U hebt de admin wizard afgerond.</b></p>
		</div>
      </div>
    );
  }
}

export default AdminWizard;