import React, { Component } from 'react';
import {BrowserRouter as Router, Switch, Route, Link} from "react-router-dom";

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
    console.log(this.getPointer().component)
    return (
      <div className="col-9 main-window">
        <Switch>
          <Route exact path={this.getPointer().path} component={this.getPointer().component} />
        </Switch>
      </div>
    );
  }
}

export default Main;