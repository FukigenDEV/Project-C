import React, { Component } from 'react';
import {BrowserRouter as Router, Switch, Route, Link} from "react-router-dom";
import Nav from './nav';

class Navs extends Component {
  render() {
    const {onSelect, navs} = this.props;

    return (
      <div className="col shadow-sm bg-white Nav-Bar">
        <nav className="flex-column">
          { navs.map(nav => (<Nav key={nav.id} onSelect={onSelect} nav={nav} />)) }
        </nav>
      </div>
    );
  }
}

export default Navs;