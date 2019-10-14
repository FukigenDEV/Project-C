import React from 'react';
import './App.css';
import {BrowserRouter as Router, Switch, Route, Link} from "react-router-dom";


function App() {
  return (
    <div className="App">
        <Router>
          <div className="Nav-Bar">
            <div className="logo">
            <img src="http://placekitten.com/g/190/80" alt="Logo"></img>
            </div>
            <Link className="App-link" to="/">Gegevens bekijken</Link>
            <Link className="App-link" to="/GegevensRegistreren">Gegevens registreren</Link>
            <Link className="App-link" to="/Notities">Notities</Link>
            <Link className="App-link" to="/Activiteitengeschiedenis">Activiteitengeschiedenis</Link>
            <Link className="App-link" to="/Back-up">Back-up maken</Link>
            <Link className="App-link" to="/Uitloggen">Uitloggen</Link>
          </div>
        <Switch>
          <Route exact path="/">
            <GegevensBekijken />
          </Route>
          <Route exact path="/GegevensRegistreren">
            <GegevensRegistreren />
          </Route>
          <Route exact path="/Notities">
            <Notities />
          </Route>
          <Route exact path="/Activiteitengeschiedenis">
            <Activiteitengeschiedenis />
          </Route>
          <Route exact path="/Back-up">
            <Backup />
          </Route>
          <Route exact path="/Uitloggen">
            <Uitloggen />
          </Route>
        </Switch>
        </Router>
    </div>
  );
}

function GegevensBekijken() {
  return(
    <div className="main-window">

    <h1>Gegevens bekijken</h1>
      <div className="tabel">
        <table>
          <tr>
            <th>Naam</th>
            <th>Afdeling</th>
            <th>Aangemaakt op</th>
          </tr>
          <tr>
            <td>Very interesting collection of data #1</td>
            <td>HR</td>
            <td>13/10/19 14:03</td>
          </tr>
          <tr>
            <td>Very interesting collection of data #2</td>
            <td>HR</td>
            <td>13/10/19 15:34</td>
          </tr>
          <tr>
            <td>Very interesting collection of data #3</td>
            <td>IT</td>
            <td>13/10/19 15:47</td>
          </tr>
          <tr>
            <td>Very interesting collection of data #4</td>
            <td>IT</td>
            <td>13/10/19 15:52</td>
          </tr>
        </table>
      </div>
    </div>
  );
}
function GegevensRegistreren() {
  return(
    <div className="main-window">
      
    </div>
  );
}
function Notities() {
  return(
    <div className="main-window">
      
    </div>
  );
}
function Activiteitengeschiedenis() {
  return(
    <div className="main-window">
      
    </div>
  );
}
function Backup() {
  return(
    <div>
      
    </div>
  );
}
function Uitloggen() {
  return(
    <div>
      
    </div>
  );
}
export default App;
