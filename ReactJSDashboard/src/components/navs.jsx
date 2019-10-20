import React, { Component } from 'react';
import {BrowserRouter as Router, Switch, Route, Link} from "react-router-dom";
import Nav from './nav';

class Navs extends Component {
  render() {
    const {onSelect, navs} = this.props;

    return (
      <div className="col Nav-Bar">
        <nav className="nav navbar-light bg-light flex-column">
          <Link className="App-link header" to="/">
            <h1>PROJECT C</h1>
          </Link>

          { navs.map(nav => (<Nav key={nav.id} onSelect={onSelect} nav={nav} />)) }
        </nav>
      </div>
    );
  }
}

export default Navs;