import React, { Component } from 'react';

class EditUsers extends Component {
  constructor(props) {
    super(props)
    this.state = {
      alert: {
        type: 0,
        value: ''
      },

      form: [],
      data: []
    }
  }

  componentDidMount() {
    this.getUser();
  }

  handleChange = (event) => {
    event.preventDefault();
    const form = {...this.state.form};
    form[event.target.name] = event.target.value;
    this.setState({form});
  }

  handleSubmit = async (event) => {
    event.preventDefault();
    const {onRedirect} = this.props;
    const isNewEmail = (this.props.match.params.name !== this.state.form.Email);
    const form = this.state.form;

    if(!isNewEmail) {
      delete form["Email"];
    }

    await fetch(`/account?email=${this.state.data.map(user => (user.Email))}`, {
      method: 'PATCH',
      body: JSON.stringify(form),
      headers: {
        'Content-Type': 'application/json'
      }
    });

    if(isNewEmail) {
      const link = '/dashboard/Admin/users/manage/edit/' + this.state.form.Email;
      onRedirect(link);
    }

    const user = (isNewEmail) ? this.state.form.Email : "";
    this.getUser(user);
  }

  getUser = user => {
    const name = (user) ? user : this.props.match.params.name;
    fetch(`/account?email=${name}`)
    .then(user => {
      return user.json();
    }).then(data => {
      this.setState({data, form: {
        Email: data.map(user => (user.Email))[0],
        Function: data.map(user => (user.Function))[0],
        Firstname: data.map(user => (user.Firstname))[0],
        MiddleInitial: data.map(user => (user.MiddleInitial))[0],
        Lastname: data.map(user => (user.Lastname))[0],
        Birthday: data.map(user => (user.Birthday))[0],
        Country: data.map(user => (user.Country))[0],
        Postcode: data.map(user => (user.Postcode))[0],
        Address: data.map(user => (user.Address))[0],
        MobilePhone: data.map(user => (user.MobilePhone))[0],
        WorkPhone: data.map(user => (user.WorkPhone))[0],
      }});
    })
  }

  render() {
    console.log(this.state.form);
    console.log(this.state.data);
    return (
      <div>
        <h1><b>Edit User:</b> {this.state.data.map(user => (user.Email))}</h1><br />

        <form onSubmit={this.handleSubmit.bind(this)}>
          <div class="form-group">
            <label for="Email">Email</label>
            <input onChange={this.handleChange} type="text" name="Email" class="form-control" id="Email" value={this.state.form.Email} />

            <label for="Function">Function</label>
            <input onChange={this.handleChange} type="text" name="Function" class="form-control" id="Function" value={this.state.form.Function} />
          </div>

          <br />

          <div class="form-group">
            <label for="Firstname">First name</label>
            <input onChange={this.handleChange} type="text" name="Firstname" class="form-control" id="Firstname" value={this.state.form.Firstname} />

            <label for="MiddleInitial">Middle name</label>
            <input onChange={this.handleChange} type="text" name="MiddleInitial" class="form-control" id="MiddleInitial" value={this.state.form.MiddleInitial} />

            <label for="Lastname">Last name</label>
            <input onChange={this.handleChange} type="text" name="Lastname" class="form-control" id="Lastname" value={this.state.form.Lastname} />

            <label for="Birthday">Birthday</label>
            <input onChange={this.handleChange} type="text" name="Birthday" class="form-control" id="Birthday" value={this.state.form.Birthday} />
          </div>

          <br />

          <div class="form-group">
            <label for="Country">Country</label>
            <input onChange={this.handleChange} type="text" name="Country" class="form-control" id="Country" value={this.state.form.Country} />

            <label for="Postcode">Postcode</label>
            <input onChange={this.handleChange} type="text" name="Postcode" class="form-control" id="Postcode" value={this.state.form.Postcode} />

            <label for="Address">Address</label>
            <input onChange={this.handleChange} type="text" name="Address" class="form-control" id="Address" value={this.state.form.Address} />
          </div>

          <br />

          <div class="form-group">
            <label for="MobilePhone">Mobile phone number</label>
            <input onChange={this.handleChange} type="text" name="MobilePhone" class="form-control" id="MobilePhone" value={this.state.form.MobilePhone} />

            <label for="WorkPhone">Work phone number</label>
            <input onChange={this.handleChange} type="text" name="WorkPhone" class="form-control" id="WorkPhone" value={this.state.form.WorkPhone} />
          </div>

          <button type="submit" class="btn btn-primary">Edit user</button>
        </form>
      </div>
    );
  }
}

export default EditUsers;