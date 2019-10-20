import React, { Component } from 'react';
import {BrowserRouter as Router, Switch, Route, Link} from "react-router-dom";
import {GegevensRegistreren, GegevensBekijken, Notities, Activiteitengeschiedenis, Backup, Uitloggen} from '../../index';
import Navs from '../navs';
import Main from '../main';
import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css';

class App extends Component {
  state = {
    navs: [
      {id: 1, heading: 'Gegevens bekijken', link: '', path: '/', component: GegevensBekijken, active: true},
      {id: 2, heading: 'Gegevens Registreren', link: 'GegevensRegistreren', path: '/GegevensRegistreren', component: GegevensRegistreren, active: false},
      {id: 3, heading: 'Notities', link: 'Notities', path: '/Notities', component: Notities, active: false},
      {id: 4, heading: 'Activiteitengeschiedenis', link: 'Activiteitengeschiedenis', path: '/Activiteitengeschiedenis', component: Activiteitengeschiedenis, active: false},
      {id: 5, heading: 'Back-up maken', link: 'Back-up', path: '/Back-up', component: Backup, active: false},
      {id: 6, heading: 'Uitloggen', link: 'Uitloggen', path: '/Uitloggen', component: Uitloggen, active: false}
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
    this.setState({navs});
  }

  render() {
    return (
      <div className="container-fluid">
        <div className="row">
          <Router>
            <Navs navs={this.state.navs} onSelect={this.handleSelect} />
            <Main navs={this.state.navs} />
          </Router>
        </div>
      </div>
    );
  }
}
export default App;
