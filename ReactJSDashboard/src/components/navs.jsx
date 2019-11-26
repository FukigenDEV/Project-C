import React, { Component } from 'react';
import Nav from './nav';

class Navs extends Component {
  render() {
    const { onSelect, navs } = this.props;
    return (
      <div className="col-lg-3 col-md-3 col-sm-12">
        <div className="row">
          <div className="col-md-3 shadow-sm Nav-Bar bg-white navvv">
            <nav className="flex-column">
              {navs.map(nav => (<Nav key={nav.id} onSelect={onSelect} nav={nav} />))}
            </nav>
          </div>
        </div>
      </div>
    );
  }
}

export default Navs;