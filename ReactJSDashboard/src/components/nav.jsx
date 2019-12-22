import React, { Component } from 'react';
import { BrowserRouter as Router, Switch, Route, Link } from "react-router-dom";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faHome, faTools, faFileSignature, faFile, faClipboard, faHistory, faDownload, faSignOutAlt, fas, faUserShield, faMagic } from '@fortawesome/free-solid-svg-icons';
import { library } from '@fortawesome/fontawesome-svg-core';
library.add(
  fas,
  faHome,
  faTools,
  faFileSignature,
  faFile,
  faClipboard,
  faHistory,
  faDownload,
  faSignOutAlt
)

class Nav extends Component {
  render() {
    const heading = (this.props.nav.id === 1) ? <h1>{this.props.nav.heading}</h1> : this.props.nav.heading;
    const icon = (this.props.nav.id !== 1) ? <div className="nav-icon">{<FontAwesomeIcon icon={['fas', this.props.nav.icon]} />}</div> : <div></div>;
    var user = getLoggedInUser();
    var role = user["Permissions"]["Administrators"];
    var isAdmin = role === "Administrator";
    if (this.props.nav.id === 1) {
      if (!isAdmin) {
        return null;
	  }
	}
    if (this.props.nav.id === 9) return null;
    return (
      <Link onClick={() => this.props.onSelect(this.props.nav)} className={this.getClassesNav()} to={this.props.nav.link}>{icon} <span class="nav-text">{heading}</span></Link>
    );
  }

  getClassesNav() {
    let classes = "App-link";
    classes += (this.props.nav.id === 1) ? " header" : "";
    classes += (this.props.nav.active) ? " selected" : "";
    classes += (this.props.nav.link === "/dashboard/logout") ? " logout" : "";
    return classes;
  }
}

function getLoggedInUser() {
  var returnValue;

  var xhr = new XMLHttpRequest();
  xhr.open("GET", "/account?email=CurrentUser", false);
  xhr.setRequestHeader("Content-Type", "application/json");
  
  xhr.onreadystatechange = function() {
      returnValue = JSON.parse(xhr.responseText)[0];
  }

  xhr.send();

  return returnValue;
}

export default Nav;
