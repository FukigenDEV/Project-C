import React, { Component } from 'react';

class addCompany extends Component {
  constructor(props) {
    super(props)
    this.state = {
      alert: {
        type: 0,
        value: ''
      },

      form: {
        'name': '',
        'street': '',
        'houseNumber': '',
        'postCode': '',
        'city': '',
        'country': '',
        'phoneNumber': '',
        'email': ''
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
    xhr.open("POST", "/company", true);
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
    console.log(this.state.form);
    console.log(this.state.alert);
    return (
      <div>
        <form onSubmit={this.handleSubmit.bind(this)}>
          <div class="form-group">
            <label for="Name">Name</label>
            <input onChange={this.handleChange} type="text" name="name" class="form-control" id="Name" placeholder="Enter name" />
          </div>

          <div class="form-group">
            <label for="street">Street</label>
            <input onChange={this.handleChange} type="text" name="street" class="form-control" id="street" placeholder="Enter street" />
          </div>

          <div class="form-group">
            <label for="houseNumber">House number</label>
            <input onChange={this.handleChange} type="text" name="houseNumber" class="form-control" id="houseNumber" placeholder="Enter house number" />
          </div>

          <div class="form-group">
            <label for="postCode">Postcode</label>
            <input onChange={this.handleChange} type="text" name="postCode" class="form-control" id="postCode" placeholder="Enter postcode" />
          </div>

          <div class="form-group">
            <label for="city">City</label>
            <input onChange={this.handleChange} type="text" name="city" class="form-control" id="city" placeholder="Enter city" />
          </div>

          <div class="form-group">
            <label for="country">Country</label>
            <input onChange={this.handleChange} type="text" name="country" class="form-control" id="country" placeholder="Enter country" />
          </div>

          <div class="form-group">
            <label for="phoneNumber">Phone number</label>
            <input onChange={this.handleChange} type="text" name="phoneNumber" class="form-control" id="phoneNumber" placeholder="Enter phone number" />
          </div>

          <div class="form-group">
            <label for="email">E-mail</label>
            <input onChange={this.handleChange} type="text" name="email" class="form-control" id="email" placeholder="Enter e-mail" />
          </div>

          <button type="submit" class="btn btn-primary">Add company</button>
        </form>
      </div>
    );
  }
}

export default addCompany;