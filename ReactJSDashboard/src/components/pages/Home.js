import React, { Component } from 'react';
import $ from 'jquery';

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

class Home extends Component {
  componentDidMount() {
	var user = getLoggedInUser();

	$("#greeting").html("<b>Goedendag, " + user["Firstname"] + "!</b>");
	$("#user_info").html("E-mailadres: " + user["Email"] + "<br/>");

	$("#add_note_form").on("submit", function (event) {
		event.preventDefault();

		var title = $("#note_title").val();
		var text = $("#note_text").val();

		var xhr = new XMLHttpRequest();

		xhr.open("POST", "/note", true);
		xhr.setRequestHeader("Content-Type", "application/json");

		xhr.onreadystatechange = function () {
			if (xhr.readyState === 4) {
				if (xhr.status >= 200 && xhr.status < 300) {
					$("#message").html("De notitie is succesvol aangemaakt.");
                    $("#message").show().delay(2000).fadeOut();
                    
                    $("#note_title").val("");
                    $("#note_text").val("");
				}
			}
		}

		var data = JSON.stringify({ "title": title, "text": text });
		xhr.send(data);
	});
  }

  render() {
    return (
      <div className="shadow-sm p-3 mb-5 bg-white rounded">
		<h2>Home</h2>

		<hr/>

		<div class="row" style={{"padding":"0px 15px"}}>
		  <div class="col-sm" style={{"background-color":"#E6E6E6","padding":"15px","border":"1px solid #5A5A5A"}}>
		    <p id="greeting" style={{"font-size":"18px"}}></p>
		    <p id="user_info"></p>
		  </div>

		  <div class="col-sm" style={{"background-color":"#D2D2D2","padding":"15px","border":"1px solid #5A5A5A"}}>
            <p style={{"font-size":"18px"}}><b>Al uw bestanden veilig opslaan?</b></p>
            <a href="/index.html#/dashboard/Back-up" style={{"color":"#000000","text-decoration":"underline"}}>Back-up maken →</a>
		  </div>
		</div>

        <div class="row" style={{"padding":"0px 15px"}}>
		  <div class="col-sm" style={{"background-color":"#DCDCDC","padding":"15px","border":"1px solid #5A5A5A"}}>
            <p style={{"font-size":"18px"}}><b>Gegevens bekijken</b></p>
            <p>Geregistreerde gegevens kunt u van bepaalde tabellen inzien. Afhankelijk van uw rol binnen de afdeling kunnen gegevens ook worden gevalideerd.</p>
            <a href="/index.html#/dashboard/Gegevens" style={{"color":"#000000","text-decoration":"underline"}}>Naar gegevens →</a>
		  </div>

		  <div class="col-sm" style={{"background-color":"#F0F0F0","padding":"15px","border":"1px solid #5A5A5A"}}>
            <div id="notitieAanmaken">
              <p style={{"font-size":"18px"}}>
                <b>Snel een notitie aanmaken</b>
                <br/>
                <a href="/index.html#/dashboard/Notities" style={{"font-size":"12px","text-decoration":"underline","color":"black"}}>Alle notities →</a>
              </p>

              <form id="add_note_form" method="POST">
                <label>Titel:</label><br/>
                <input id="note_title" type="text" name="note_title" style={{"width":"50%","margin-bottom":"15px"}}/><br/>
                <label>Tekst:</label><br/>
                <textarea id="note_text" type="text" name="note_text" style={{"width":"50%","height":"100px"}}></textarea><br/>
                
                <br/>
                
                <input type="submit" value="Toevoegen" style={{"width":"200px"}}/><br/>
                
                <br/>
                
                <p id="message" style={{"display":"none"}}></p>
                
              </form>
            </div>
		  </div>
		</div>

      </div>
    );
  }
}

export default Home;