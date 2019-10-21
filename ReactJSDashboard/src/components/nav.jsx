import React, { Component } from 'react';
import {BrowserRouter as Router, Switch, Route, Link} from "react-router-dom";

class Nav extends Component {
  render() {
    const heading = (this.props.nav.id === 0) ? <h1>{this.props.nav.heading}</h1> : this.props.nav.heading;
    return (
      <Link onClick={() => this.props.onSelect(this.props.nav)} className={this.getClassesNav()} to={this.props.nav.link}>{heading}</Link>
    );
  }

  getClassesNav() {
    let classes = "App-link";
    classes += (this.props.nav.id === 0) ? " header" : "";
    classes += (this.props.nav.active) ? " selected" : "";
    return classes;
  }
}

export default Nav;