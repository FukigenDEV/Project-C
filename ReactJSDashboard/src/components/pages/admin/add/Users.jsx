import React, { Component } from 'react';

class addUsers extends Component {
  constructor(props) {
    super(props)
    this.state = {
      alert: {
        type: 0,
        value: ''
      },

      form: {
        'Email': '',
        'Password': '',
        'AccountType': '',
        'MemberOf': ''
      }
    }
  }

  handleChange = (event) => {
    event.preventDefault();
    const form = {...this.state.form};
    form[event.target.name] = event.target.value;
    this.setState({form});
  }

  handleSubmit = (event) => {
    event.preventDefault();
    const obj = this.state.form;
    const data = JSON.stringify(obj);

    let xhr = new XMLHttpRequest();
    xhr.open("POST", "/account", true);
    xhr.onreadystatechange = () => {
      if(xhr.readyState === 4) {
        if(xhr.status === 200) {
          const alert = {...this.state.alert};
          alert.type = 200;
          alert.value = 'User succesfully created';
          this.setState({alert});
        } else {
          const alert = {...this.state.alert};
          alert.type = xhr.status;
          alert.value = xhr.responseText;
          this.setState({alert});
        }
      }
    }
  
    xhr.setRequestHeader('Content-Type', 'application/json');
    xhr.send(data);
  }

  render() {
    return (
      <div>
        <form onSubmit={this.handleSubmit.bind(this)}>
          <div class="form-group">
            <label for="Email">Email address</label>
            <input onChange={this.handleChange} type="text" name="Email" class="form-control" id="Email" placeholder="Enter email" />
          </div>

          <div class="form-group">
            <label for="Password">Password</label>
            <input onChange={this.handleChange} type="password" name="Password" class="form-control" id="Password" placeholder="Enter password" />
          </div>

          <div class="form-group">
            <label for="Department">Department</label>
            <input onChange={this.handleChange} type="text" name="MemberOf" class="form-control" id="Department" placeholder="Enter department" />
          </div>

          <div class="form-group">
            <label for="Type">Account type</label>
            <select onChange={this.handleChange} name="AccountType" class="form-control" id="Type">
              <option value="">Select type...</option>
              <option value="Administrator">Administrator</option>
              <option value="Manager">Manager</option>
              <option value="DeptMember">Department Member</option>
              <option value="User">User</option>
            </select>
          </div>

          <button type="submit" class="btn btn-primary">Add user</button>
        </form>
      </div>
    );
  }
}

export default addUsers;