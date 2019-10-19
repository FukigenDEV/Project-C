import React, { Component } from 'react';
import Nav from './nav';

class Navs extends Component {
  render() {
    const {onSelect, navs} = this.props;

    return (navs.map(nav => (<Nav key={nav.id} onSelect={onSelect} nav={nav} />)));
  }
}

export default Navs;