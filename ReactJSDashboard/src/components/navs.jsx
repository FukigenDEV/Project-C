import React, { Component } from 'react';
import Nav from './nav';

class Navs extends Component {
  render() {
    const { onSelect, admin, navs } = this.props;
    return (
      <div className="nav-container">
          <div className="Nav-Bar bg-white navvv">
            <nav className="flex-column">
              {navs.map(nav => (<Nav key={nav.id} onSelect={onSelect} nav={nav} admin={admin} />))}
            </nav>
          </div>
      </div>
    );
  }
}

export default Navs;