import React, { Component } from 'react';
import { NONAME } from 'dns';

class Login extends Component {
  constructor(props) {
    super(props)
    this.state = {
      alert: {
        type: 0,
        value: ''
      },

      login: {
        'Email': '',
        'Password': '',
        'RememberMe': true
      }
    }
  }

  componentDidMount() {
    if(this.props.loggedin === true) {
      this.props.onLogin(200);
    }
  }

  handleSubmit = (event) => {
    event.preventDefault();
    const obj = this.state.login;
    const data = JSON.stringify(obj);

    let xhr = new XMLHttpRequest();
    xhr.open("POST", "/login", true);
    xhr.onreadystatechange = () => {
      if(xhr.readyState === 4) {
        const alert = {...this.state.alert};
        alert.type = xhr.status;
        alert.value = xhr.responseText;
        this.props.onLogin(xhr.status);
        this.setState({alert});
      }
    }
    xhr.setRequestHeader('Content-Type', 'application/json');
    xhr.send(data);
  }

  handleChange = (event) => {
    event.preventDefault();
    const login = {...this.state.login};
    login[event.target.name] = event.target.value;
    this.setState({login});
  }

  getBadgeClasses = () => {
    let classes = 'alert mr-3 ml-3 ';
    classes += (this.state.alert.type === 204 || this.state.alert.type === 200) ? "alert-success" : "alert-danger";
    classes += (this.state.alert.type === 0) ? " d-none" : " d-block";
    return classes;
  }

  render() {
    return (
      <div className="container mx-auto vertical-center">
        <div className="mx-auto vertical-center">
          <form className="login-form" onSubmit={this.handleSubmit.bind(this)}>
            <div className={this.getBadgeClasses()}>{(this.state.alert.type === 401) ? "Wrong e-mail and/or password!" : this.state.alert.value}</div>
            <input onChange={this.handleChange} type="text" className="form-control m-3" placeholder="E-mail" name="Email" autoComplete="username" />
            <input onChange={this.handleChange} type="password" className="form-control m-3" placeholder="Password" name="Password" autoComplete="current-password" />
            <button className="btn btn-primary btn-lg mr-3 ml-3">Login</button>
          </form>
        </div>
      </div>
    );
  }
}

export default Login;