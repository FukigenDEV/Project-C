import React, { Component } from 'react';
import { BrowserRouter as Router, Switch, Route, Link } from "react-router-dom";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faHome, faFileSignature, faFile, faClipboard, faHistory, faDownload, faSignOutAlt, fas } from '@fortawesome/free-solid-svg-icons';
import { library } from '@fortawesome/fontawesome-svg-core';
library.add(
  fas,
  faHome,
  faFileSignature,
  faFile,
  faClipboard,
  faHistory,
  faDownload,
  faSignOutAlt
)

class Nav extends Component {
  render() {
    const heading = (this.props.nav.id === 0) ? <h1>{this.props.nav.heading}</h1> : this.props.nav.heading;
    const icon = <div className="nav-icon">{<FontAwesomeIcon icon={['fas', this.props.nav.icon]} />}</div>;
    return (
      <Link onClick={() => this.props.onSelect(this.props.nav)} className={this.getClassesNav()} to={this.props.nav.link}>{icon} {heading}</Link>
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