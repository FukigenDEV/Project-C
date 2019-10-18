import React from 'react';
import './App.css';
import {BrowserRouter as Router, Switch, Route, Link} from "react-router-dom";
import {GegevensRegistreren} from './GegevensRegistreren.js';
import {GegevensBekijken} from './GegevensBekijken.js';
import {Notities} from './Notities.js';
import {Activiteitengeschiedenis} from './Activiteitengeschiedenis.js';
import {Backup} from './Backup.js';
import {Uitloggen} from './Uitloggen.js';

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
            <Link className="App-link Align-Bottom" to="/Uitloggen">Uitloggen</Link>
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
export default App;
