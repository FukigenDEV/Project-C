import React, { Component } from 'react';
import {BrowserRouter as Router, Switch, Route, Link} from "react-router-dom";

class Uitloggen extends Component {
  // initialiseren van de class Uitloggen 
  constructor(props) {
    // Hier zet hij de benodigde properties van de class Component erin
    super(props)
    // Dit is het begin van de states, dus bijvoorbeeld het type wordt hier op 0 gezet, later kan dit veranderd worden. Zelfde geld voor value.
    this.state = {
      alert: {
        type: 0,
        value: ''
      }
    }
  }

  handleLogout = (event) => {
    // prevent default zorgt ervoor dat de pagina niet herladen wordt
    event.preventDefault();
    // Maakt een XHR aan, om met de backend te praten
    let xhr = new XMLHttpRequest();
    // Dit roept de DELETE functie op die in de backend is gemaakt, de true zorgt ervoor dat de funtie asynchroon uitgevoerd wordt
    xhr.open("DELETE", "/login", true);
    // Wanneer er iets in de state veranderd van de xhr, gaat hij dit doorlopen
    xhr.onreadystatechange = () => {
      // de 4 staat voor "done", 0 is unsent, 1 is opened, 2 is headers received, 3 is loading
      if(xhr.readyState === 4) {
        // Hier veranderen de type en value in naar wat hij van de xhr krijgt
        const alert = {...this.state.alert};
        alert.type = xhr.status;
        alert.value = xhr.responseText;
        // Als de status 200 is, dan is er succesvol uitgelogd en dan met de OnLogout wordt terug verwezen naar de index pagina, oftewel de inlog pagina
        this.setState({alert});
        if(xhr.status === 200) {
          this.props.onLogout('/');
        }
        // dat betekend dat de logged in value uiteraard naar false moet worden gezet
        this.props.loggedin.value = false;
      }
    }
    // Geeft aan de backend door wat voor soort data eraan komt, Json in dit geval
    xhr.setRequestHeader('Content-Type', 'application/json');
    // verstuurt data naar de backend, een leeg json object in dit geval. Maar anders deed hij het niet, backend verwacht een send.
    xhr.send("{}"); 
  }
// Dit is om aan te geven welke response de front end van de backend krijgt nadat de front end iets daarnaar heeft verstuurd
  getBadgeClasses = () => {
    // Bootstrap, maakt een alert message box met een bepaalde styling
    let classes = 'alert mr-3 ml-3 ';
    // als de status 401 is dan is er iets fout gegaan, beneden in de render geven we hier een melding aan
    classes += (this.state.alert.type === 401) ? "alert-danger" : "";
    // als de status 0 is laat hij geen "block" ofwel balk zien, als de status anders is dan 0 laat hij wel een balk zien
    classes += (this.state.alert.type === 0) ? " d-none" : " d-block";
    return classes;
  }

  render() {

    return (
      <div className="col-sm-12 shadow-sm p-3 mb-5 bg-white rounded">
        Weet je zeker dat je wilt uitloggen?
        <br/><br/><br/>
        {/*Wanneer er iets foutgaat bij uitloggen geeft hij deze melding*/}
        <div className={this.getBadgeClasses()}>{"An error occured, couldn't log out"}</div>
        {/*Maakt de knop aan die het daadwerkelijke uitloggen in gang zet*/}
        <button onClick={this.handleLogout.bind(this)} className="logout-button">Log uit</button>        
      </div>
    );
  }
}
export default Uitloggen;