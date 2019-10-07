import React, { Component } from 'react';
import '../stylesheets/login.css';

class Login extends Component {
  constructor(props) {
    super(props)
    this.state = {
      username: '',
      password: ''
    }
  }

  handleSubmit = (event) => {
    event.preventDefault()
    const data = this.state
    console.log(data)
  }

  handleChange = (event) => {
    event.preventDefault()
    this.setState({
      [event.target.name]: event.target.value
    })
  }

  render() {
    return (
      <div className="App mx-auto vertical-center">
        <form className="login-form" action="#">
          <input onChange={this.handleChange} type="text" className="form-control mx-3" placeholder="Username" name="username" autoComplete="username" />
          <input onChange={this.handleChange} type="password" className="form-control m-3" placeholder="Password" name="password" autoComplete="current-password" />
          <button className="btn btn-primary btn-lg mx-3">Login</button>
        </form>
      </div>
    );
  }
}

export default Login;