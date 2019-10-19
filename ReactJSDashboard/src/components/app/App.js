import React, { Component } from 'react';
import {BrowserRouter as Router, Switch, Route, Link} from "react-router-dom";
import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import Navs from '../navs';
import GegevensRegistreren from '../pages/GegevensRegistreren';
import GegevensBekijken from '../pages/GegevensBekijken';
import Notities from '../pages/Notities';
import Activiteitengeschiedenis from '../pages/Activiteitengeschiedenis';
import Backup from '../pages/Backup';
import Uitloggen from '../pages/Uitloggen';

class App extends Component {
  state = {
    navs: [
      {id: 1, heading: 'Gegevens bekijken', link: '', active: true},
      {id: 2, heading: 'Gegevens Registreren', link: 'GegevensRegistreren', active: false},
      {id: 3, heading: 'Notities', link: 'Notities', active: false},
      {id: 4, heading: 'Activiteitengeschiedenis', link: 'Activiteitengeschiedenis', active: false},
      {id: 5, heading: 'Back-up maken', link: 'Back-up', active: false}
    ]
  };

  handleSelect = nav => {
    this.setFalse();
    const navs = [...this.state.navs];
    const index = navs.indexOf(nav);
    navs[index] = {...nav};
    navs[index].active = true;
    this.setState({navs});
  }

  setFalse = () => {
    const navs = [...this.state.navs];
    for (var i in navs) {
      navs[i].active = false;
    }
    console.log(navs);
    this.setState({navs});
  }

  render() {
    return (
      <div className="container-fluid">
        <div className="row">
          <Router>
            <div className="col Nav-Bar">
              <nav className="nav navbar-light bg-light flex-column">
                <Link className="App-link header" to="/">
                  <h1>PROJECT C</h1>
                </Link>

                <Navs navs={this.state.navs} onSelect={this.handleSelect} />
              </nav>
            </div>

            <div className="col-9 main-window">
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
            </div>
          </Router>
        </div>
      </div>
    );
  }
}
export default App;
