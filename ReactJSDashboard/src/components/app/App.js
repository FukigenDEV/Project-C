import React, { Component } from 'react';
import { createGlobalStyle } from 'styled-components';
import {BrowserRouter as Router, Switch, Route, Link} from "react-router-dom";
import {Dashboard, GegevensRegistreren, GegevensBekijken, Notities, Activiteitengeschiedenis, Backup, Uitloggen} from '../../index';
import Navs from '../navs';
import Main from '../main';
import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css';

const GlobalStyle = createGlobalStyle`body { background: rgb(2,0,36) !important; background: linear-gradient(90deg, rgba(2,0,36,1) 0%, rgba(35,35,102,1) 30%, rgba(0,142,255,1) 100%) !important;}`

class App extends Component {
  state = {
    navs: [
      {id: 0, heading: 'Project C', link: '', path: '/', component: Dashboard, active: true},
      {id: 1, heading: 'Gegevens bekijken', link: 'GegevensBekijken', path: '/GegevensBekijken', component: GegevensBekijken, active: false},
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
      <React.Fragment>
        <GlobalStyle />
        <div className="container-fluid">
          <div className="row">
            <Router>
              <Navs navs={this.state.navs} onSelect={this.handleSelect} />
              <Main navs={this.state.navs} />
            </Router>
          </div>
        </div>
      </React.Fragment>
    );
  }
}
export default App;