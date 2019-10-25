import React, { Component } from 'react';
import {BrowserRouter as Router, Switch, Route} from "react-router-dom";

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
        </Switch>
      </div>
    );
  }
}

export default Main;