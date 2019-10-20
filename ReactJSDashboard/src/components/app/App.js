import React, { Component } from 'react';
import {BrowserRouter as Router, Switch, Route, Link} from "react-router-dom";
import Navs from '../navs';
import Main from '../main';
import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css';

class App extends Component {
  state = {
    navs: [
      {id: 1, heading: 'Gegevens bekijken', link: '', path: '/', active: true},
      {id: 2, heading: 'Gegevens Registreren', link: 'GegevensRegistreren', path: '/GegevensRegistreren', active: false},
      {id: 3, heading: 'Notities', link: 'Notities', path: '/Notities', active: false},
      {id: 4, heading: 'Activiteitengeschiedenis', link: 'Activiteitengeschiedenis', path: '/Activiteitengeschiedenis', active: false},
      {id: 5, heading: 'Back-up maken', link: 'Back-up', path: '/Back-up', active: false},
      {id: 6, heading: 'Uitloggen', link: 'Uitloggen', path: '/Uitloggen', active: false}
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

            <div className="col-9 main-window">
              <Main navs={this.state.navs} />
            </div>
          </Router>
        </div>
      </div>
    );
  }
}
export default App;
