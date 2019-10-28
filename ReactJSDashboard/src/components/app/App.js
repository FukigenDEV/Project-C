import React, { Component } from 'react';
import { createGlobalStyle } from 'styled-components';
import { BrowserRouter as Router } from "react-router-dom";
import { Dashboard, GegevensRegistreren, GegevensBekijken, Notities, Activiteitengeschiedenis, Backup, Uitloggen } from '../../index';
import Login from '../login';
import Navs from '../navs';
import Main from '../main';
import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import { bool } from 'prop-types';

const GlobalStyle = createGlobalStyle`body { background: rgb(2,0,36) !important; background: linear-gradient(90deg, rgba(2,0,36,1) 0%, rgba(35,35,102,1) 30%, rgba(0,142,255,1) 100%) !important;}`

class App extends Component {
  state = {
    loggedin: { value: false },
    navs: [
      { id: 0, heading: 'Project C', link: '', path: '/', component: Dashboard, active: true, icon: 'home' },
      { id: 1, heading: 'Gegevens bekijken', link: 'GegevensBekijken', path: '/GegevensBekijken', component: GegevensBekijken, active: false, icon: 'file-signature' },
      { id: 2, heading: 'Gegevens Registreren', link: 'GegevensRegistreren', path: '/GegevensRegistreren', component: GegevensRegistreren, active: false, icon: 'file' },
      { id: 3, heading: 'Notities', link: 'Notities', path: '/Notities', component: Notities, active: false, icon: 'clipboard' },
      { id: 4, heading: 'Activiteiten geschiedenis', link: 'Activiteitengeschiedenis', path: '/Activiteitengeschiedenis', component: Activiteitengeschiedenis, active: false, icon: 'history' },
      { id: 5, heading: 'Back-up maken', link: 'Back-up', path: '/Back-up', component: Backup, active: false, icon: 'download' },
      { id: 6, heading: 'Uitloggen', link: 'Uitloggen', path: '/Uitloggen', component: Uitloggen, active: false, icon: 'sign-out-alt' }
    ]
  };

  handleSelect = nav => {
    this.setFalse();
    const navs = [...this.state.navs];
    const index = navs.indexOf(nav);
    navs[index] = { ...nav };
    navs[index].active = true;
    this.setState({ navs });
  }

  handleLogin = status => {
    if(status === 200 || status === 204) {
      const loggedin = {...this.state.loggedin};
      loggedin.value = true;
      this.setState({loggedin});
    }
  }

  setFalse = () => {
    const navs = [...this.state.navs];
    for (var i in navs) {
      navs[i].active = false;
    }
    this.setState({ navs });
  }

  componentWillMount() {
    let xhr = new XMLHttpRequest();
    xhr.open("POST", "/login", true);
    xhr.onreadystatechange = () => {
      if(xhr.status === 200 || xhr.status === 204) {
        const loggedin = {...this.state.loggedin};
        loggedin.value = true;
        this.setState({loggedin});
      }
    }
    xhr.setRequestHeader('Content-Type', 'application/json');
    xhr.send("{}");
  }

  render() {
    console.log('loggedin: ', this.state.loggedin.value);
    if(this.state.loggedin.value) {
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
    } else {
      return (
        <React.Fragment>
          <GlobalStyle />
          <Login onLogin={this.handleLogin} />
        </React.Fragment>
      );
    }
  }
}
export default App;