import React, { Component } from 'react';
import $ from 'jquery';
class AdminWizard extends Component {
  componentDidMount() {
	$("#begin_button").on("click", function(event) {
		$("#intro").hide(250);
		$("#add_company").show(250);
	});

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
				if (xhr.status === 200) {
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

	$("#add_department_form").on("submit", function(event) {
		event.preventDefault();

		var name = $("#department_name").val();
		var description = $("#department_description").val();

		var xhr = new XMLHttpRequest();
		
		xhr.open("POST", "/department", true);
		xhr.setRequestHeader("Content-Type", "application/json");

		xhr.onreadystatechange = function() {
			if (xhr.readyState === 4) {
				$("#department_message").html(xhr.responseText);
				$('#department_message').css({"display": "block"});
				$('#department_message').delay(2000).fadeOut(300);
			}
		}
		
		var data = JSON.stringify({"name": name, "description": description});
		xhr.send(data);
	});

	$("#department_next_form").on("submit", function(event) {
		$("#add_department").hide(250);
		$("#add_user").show(250);
	});

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
		var department = $("#departments_dropdown option:selected").text();

		var xhr = new XMLHttpRequest();
		
		xhr.open("POST", "/account", true);
		xhr.setRequestHeader("Content-Type", "application/json");

		xhr.onreadystatechange = function() {
			if (xhr.readyState === 4) {
				$("#user_message").html(xhr.responseText);
				$('#user_message').css({"display": "block"});
				$('#user_message').delay(2000).fadeOut(300);
			}
		}
		
		var data = JSON.stringify({"Email": email, "Password": password, "AccountType": accountType, "MemberOf": department });
		xhr.send(data);
	});

	$("#user_next_form").on("submit", function(event) {
		$("#add_user").hide(250);
		$("#finished").show(250);
	});

	var xhr = new XMLHttpRequest();
	var url = "/department?name=";
	xhr.open("GET", url, true);

	xhr.setRequestHeader("Content-Type", "application/json");

	xhr.onreadystatechange = function() {
		if (xhr.readyState === 4 && xhr.status === 200) {
			var departments = JSON.parse(xhr.responseText);

			for (var i = 0; i < departments.length; i++) {
				$("#departments_dropdown select").append("<option value=" + i + ">" + departments[i].Name + "</option>");
			}
		}
	}

	xhr.send();
  }

  render() {
    return (
      <div className="shadow-sm p-3 mb-5 bg-white rounded">
        <h2>Admin wizard</h2>
		<hr/>

		<div id="intro">
			<p><b>The admin wizard will walk you through the most import things in the application.</b></p>

			<button id="begin_button">Begin</button>
		</div>
		
		<div id="add_company" style={{display: "none"}}>
			<p><b>Company data</b></p>
			<form id="add_company_form" method="POST">
				Name: <input id="company_name" type="text" name="company_name"/><br/>
				Street: <input id="company_street" type="text" name="company_street"/><br/>
				House number: <input id="company_house_number" type="number" name="company_house_number"/><br/>
				Post code: <input id="company_post_code" type="text" name="company_post_code"/><br/>
				City: <input id="company_city" type="text" name="company_city"/><br/>
				Country: <input id="company_country" type="text" name="company_country"/><br/>
				Phone number: <input id="company_phone_number" type="text" name="company_phone_number"/><br/>
				Email: <input id="company_email" type="email" name="company_email"/><br/>
				<br/>
				<input type="submit" value="Add"/><br/>

				<p id="company_message"></p>
			</form>
		</div>

		<div id="add_department" style={{display: "none"}}>
			<p><b>Add a Department</b></p>
			<form id="add_department_form" method="POST">
				Department name: <input id="department_name" type="text" name="department_name"/><br/>
				Department description: <input id="department_description" type="text" name="department_description"/><br/>
				<br/>
				<input type="submit" value="Add"/><br/>

				<p id="department_message"></p>
			</form>

			<hr/>

			<form id="department_next_form">
				<input type="submit" value="Next"/><br/>
			</form>
		</div>

		<div id="add_user" style={{display: "none"}}>
			<p><b>Add a User</b></p>
			<form id="add_user_form" method="POST">
				Email: <input id="user_email" type="email" name="user_email"/><br/>
				Password: <input id="user_password" type="password" name="user_password"/><br/>
				First name: <input id="user_first_name" type="text" name="user_first_name"/><br/>
				Middle initial: <input id="user_middle_initial" type="text" name="user_middle_initial"/><br/>
				Last name: <input id="user_last_name" type="text" name="user_last_name"/><br/>
				Function: <input id="user_function" type="text" name="user_function"/><br/>
				Phone number (work): <input id="user_phone_number_work" type="number" name="user_phone_number_work"/><br/>
				Phone number (private): <input id="user_phone_number_private" type="number" name="user_phone_number_private"/><br/>
				Date of birth: <input id="user_date_of_birth" type="date" name="user_date_of_birth"/><br/>
				Country: <input id="user_country" type="text" name="user_country"/><br/>
				Address: <input id="user_address" type="text" name="user_address"/><br/>
				Post code: <input id="user_post_code" type="text" name="user_post_code"/><br/>

				Account type:<br/>
				<input id="user_account_type_administrator" type="radio" name="user_account_type" value="Administrator"/>Administrator<br/>
				<input id="user_account_type_manager" type="radio" name="user_account_type" value="Manager"/>Manager<br/>
				<input id="user_account_type_department_member" type="radio" name="user_account_type" value="DeptMember"/>Department member<br/>
				<input id="user_account_type_user" type="radio" name="user_account_type"  value="User" checked/>User<br/>

				Department:<br/>
				<div id="departments_dropdown">
					<select>
					</select>
				</div>

				<br/>
				<input type="submit" value="Add"/><br/>

				<p id="user_message"></p>
			</form>

			<hr/>

			<form id="user_next_form">
				<input type="submit" value="Next"/><br/>
			</form>
		</div>

		<div id="finished" style={{display: "none"}}>
			<p><b>You finished the admin wizard</b></p>
		</div>
      </div>
    );
  }
}

export default AdminWizard;