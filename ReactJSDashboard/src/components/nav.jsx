import React, { Component } from 'react';
import {BrowserRouter as Router, Switch, Route, Link} from "react-router-dom";

class Nav extends Component {
  render() {
    const {heading} = this.props.nav;
    return (
      <Link onClick={() => this.props.onSelect(this.props.nav)} className={this.getBadgeNav()} to={this.props.nav.link}>{heading}</Link>
    );
  }

  getBadgeNav() {
    let classes = "App-link";
    classes += (this.props.nav.active) ? " selected" : "";
    return classes;
  }
}

export default Nav;