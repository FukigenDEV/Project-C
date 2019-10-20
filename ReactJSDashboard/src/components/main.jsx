import React, { Component } from 'react';
import {BrowserRouter as Router, Switch, Route, Link} from "react-router-dom";
import {GegevensRegistreren, GegevensBekijken, Notities, Activiteitengeschiedenis, Backup, Uitloggen} from '../index';

class Main extends Component {
  getPath = id => {
    const {navs} = this.props;
    for(var i in navs) {
      if(navs[i].id === id) {
        return navs[i].path;
      }
    }
  }

  render() {
    return (
      <div className="col-9 main-window">
        <Switch>
          <Route exact path={this.getPath(1)}>
            <GegevensBekijken />
          </Route>
          <Route exact path={this.getPath(2)}>
            <GegevensRegistreren />
          </Route>
          <Route exact path={this.getPath(3)}>
            <Notities />
          </Route>
          <Route exact path={this.getPath(4)}>
            <Activiteitengeschiedenis />
          </Route>
          <Route exact path={this.getPath(5)}>
            <Backup />
          </Route>
          <Route exact path={this.getPath(6)}>
            <Uitloggen />
          </Route>
        </Switch>
      </div>
    );
  }
}

export default Main;