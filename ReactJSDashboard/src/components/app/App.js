import React, { Component } from 'react';
import { createGlobalStyle, ThemeConsumer } from 'styled-components';
import { HashRouter as Router, Route, Redirect, withRouter } from "react-router-dom";
import { createHashHistory } from 'history';
import { Dashboard, Home, Admin, GegevensRegistreren, GegevensBekijken, Notities, Logout } from '../../index';
import Login from '../login';
import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css';

const Background = createGlobalStyle`body { background: rgb(2,0,36) !important; background: linear-gradient(90deg, rgba(2,0,36,1) 0%, rgba(35,35,102,1) 30%, rgba(0,142,255,1) 100%) !important;}`
const history = createHashHistory();

class App extends Component {

  constructor(props) {
    super(props);
    this.state = {
      loggedin: { value: null },
      user: null,
      navs: [
        { id: 0, heading: 'Project C', link: '/dashboard', path: '/dashboard', component: Home, active: true, icon: 'tools' },
        { id: 1, heading: 'Beheren', link: '/dashboard/Admin', path: '/dashboard/Admin', component: Admin, active: false, icon: 'user-shield' },
        { id: 2, heading: 'Gegevens bekijken', link: '/dashboard/GegevensBekijken', path: '/dashboard/GegevensBekijken', component: GegevensBekijken, active: false, icon: 'file-signature' },
        { id: 3, heading: 'Gegevens Registreren', link: '/dashboard/GegevensRegistreren', path: '/dashboard/GegevensRegistreren', component: GegevensRegistreren, active: false, icon: 'file' },
        { id: 4, heading: 'Notities', link: '/dashboard/Notities', path: '/dashboard/Notities', component: Notities, active: false, icon: 'clipboard' },
        { id: 5, heading: 'Uitloggen', link: '/dashboard/logout', path: '/dashboard/logout', component: Logout, active: false, icon: 'sign-out-alt' },
      ]
    };
  }

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
      this.handleRedirect('/dashboard');
    }
  }

  handleRedirect = location => {
    history.push(location);
  }

  setFalse = () => {
    const navs = [...this.state.navs];
    for (var i in navs) {
      navs[i].active = false;
    }
    this.setState({ navs });
  }

  setUser = async () => {
    await fetch(`/account?email=CurrentUser`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json'
      }
    })
    .then(body => {
      return body.json();
    }).then(data => {
      const user = data[0];
      this.setState({user});
    })
  }

  setLoggedin = () => {
    fetch('/login', {
      method: 'POST',
      body: '{}',
      headers: {
        'Content-Type': 'application/json'
      }
    }).then(data => {
      if(data.status === 200 || data.status === 204) {
        if(this.state.loggedin.value !== true) {const loggedin = {...this.state.loggedin}; loggedin.value = true; this.setState({loggedin}); this.setUser();}
      } else {
        if(this.state.loggedin.value !== false) {const loggedin = {...this.state.loggedin}; loggedin.value = false; this.setState({loggedin});}
      }
    })
  }

  isAdmin = async () => {
    await this.setUser();
    const user = this.state.user;
    console.log(user['Permissions']);
    if('Administrators' in user['Permissions']) {
      console.log('1');
      return (user['Permissions']['Administrators'] === 'Administrator') ? true : false;
    } else {
      console.log('2');
      return false;
    }
  }

  componentWillUnmount() {
    console.log("UNMOUNTED")
  }

  render() {
    if(this.state.loggedin.value === null) {
      this.setLoggedin();
      return(<div></div>);
    } else {
      return (
        <React.Fragment>
          <Background />
          <Router>
            <Route exact path="/" render={() => <Login onLogin={this.handleLogin} loggedin={this.state.loggedin} onMount={this.setLoggedin} onRedirect={this.handleRedirect} />} />
            <Route path="/dashboard" render={() => <Dashboard navs={this.state.navs} loggedin={this.state.loggedin} isAdmin={this.isAdmin} onSelect={this.handleSelect} onMount={this.setLoggedin} onRedirect={this.handleRedirect} onRender={this.setLoggedin} />} />
          </Router>
        </React.Fragment>
      );
    }
  }


}
export default App;