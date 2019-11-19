import React, { Component } from 'react';
import {BrowserRouter as Router, Switch, Route, Link} from "react-router-dom";

class Uitloggen extends Component {
    constructor(props) {
      super(props)
      this.state = {
        alert: {
          type: 0,
          value: ''
        }
    }
  }

  handleLogout = (event) => {
    event.preventDefault();
    
    let xhr = new XMLHttpRequest();
    xhr.open("DELETE", "/login", true);
    console.log("HandleLogout")
    xhr.onreadystatechange = () => {
      // console.log('onreadystatechange %i', xhr.readyState)
      if(xhr.readyState === 4) {
        // console.log("Response")
        const alert = {...this.state.alert};
        alert.type = xhr.status;
        alert.value = xhr.responseText;
        this.setState({alert});
        if(xhr.status === 200) {
          this.props.onRedirect('/');
        }
        // this.props.history.push('/index.html#')
        this.props.loggedin.value = false;
      }
    }
    xhr.setRequestHeader('Content-Type', 'application/json');
    xhr.send("{}"); 
    // console.log(this.state.alert);
  }

  getBadgeClasses = () => {
    let classes = 'alert mr-3 ml-3 ';
    classes += (this.state.alert.type === 200) ? "alert-success" : "alert-danger";
    classes += (this.state.alert.type === 0) ? " d-none" : " d-block";
    return classes;
  }

  render() {
    const {onRedirect, loggedin} = this.props;
    console.log(loggedin.value);

    if(loggedin.value === false) {
      onRedirect('/');
    }

    return (
      <div className="col-sm-12 shadow-sm p-3 mb-5 bg-white rounded">
        Weet je zeker dat je wilt uitloggen?
        <br/><br/><br/>
        <div className={this.getBadgeClasses()}>{(this.state.alert.type === 401) ? "An error occured, couldn't log out" : "Logout was succesful"}</div>
        <button onClick={this.handleLogout.bind(this)} className="logout-button">Log uit</button>        
      </div>
    );
  }
}
export default Uitloggen;