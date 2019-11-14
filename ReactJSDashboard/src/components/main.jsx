import React, { Component } from 'react';
import {BrowserRouter as Router, Switch, Route, Link} from "react-router-dom";
import {Error, } from '../index';
import { faUserPlus } from '@fortawesome/free-solid-svg-icons';

class Main extends Component {
  getPointer = () => {
    const {navs} = this.props;
    let pointer = {path: '', component: null};
    for(var i in navs) {
      if(navs[i].active === true) {
        pointer.path = navs[i].path;
        pointer.component = navs[i].component;
        return pointer;
      }
    }
  }

  render() {
    const {navs} = this.props;
    return (
      <div className="col-9 col-s-12 main-window">
        <Switch>
          { navs.map(nav => (<Route exact path={nav.path} component={nav.component} />)) }
          <Route component={Error} />
        </Switch>
      </div>
    );
  }
}

export default Main;