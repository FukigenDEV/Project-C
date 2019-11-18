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
  const obj = this.state.login;
  const data = JSON.stringify(obj);

  let xhr = new XMLHttpRequest();
  xhr.open("DELETE", "/login", true);
  xhr.onreadystatechange = () => {
    if(xhr.readyState === 4) {
      const alert = {...this.state.alert};
      alert.type = xhr.status;
      alert.value = xhr.responseText;
      this.setState({alert});
      this.props.history.push('/index.html')
    }
  }
  xhr.setRequestHeader('Content-Type', 'application/json');
  xhr.send(data);
}

getBadgeClasses = () => {
  let classes = 'alert mr-3 ml-3 ';
  classes += (this.state.alert.type === 200) ? "alert-success" : "alert-danger";
  classes += (this.state.alert.type === 0) ? " d-none" : " d-block";
  return classes;
}

render() {
  return (
    <div className="col-sm-12 shadow-sm p-3 mb-5 bg-white rounded">
      Weet je zeker dat je wilt uitloggen?
      <br/><br/><br/>
      <div className={this.getBadgeClasses()}>{(this.state.alert.type === 401) ? "An error occured, couldn't log out" : this.state.alert.value}</div>
      <button onClick={this.handleLogout} className="logout-button">Log uit</button>        
    </div>
  );
}
}
export default Uitloggen;