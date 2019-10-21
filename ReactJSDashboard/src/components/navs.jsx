import React, { Component } from 'react';
import {BrowserRouter as Router, Switch, Route, Link} from "react-router-dom";
import Nav from './nav';

class Navs extends Component {
  render() {
    const {onSelect, navs} = this.props;

    return (
      <div className="col-lg-3 col-md-3 col-s-12">
        <div className="row">
          <div className="col-12 shadow-sm bg-white Nav-Bar d-sm-none d-md-block">
            <nav className="flex-column">
              { navs.map(nav => (<Nav key={nav.id} onSelect={onSelect} nav={nav} />)) }
            </nav>
          </div>
          
          <div className="col-12 d-none d-sm-block d-md-none bg-white">
            <nav className="flex-column">
              { navs.map(nav => (<Nav key={nav.id} onSelect={onSelect} nav={nav} />)) }
            </nav>
          </div>
        </div>
      </div>
    );
  }
}

export default Navs;