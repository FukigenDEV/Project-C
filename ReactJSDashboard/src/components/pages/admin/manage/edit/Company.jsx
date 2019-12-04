import React, { Component } from 'react';

class EditCompany extends Component {
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
    this.getCompany();
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
    await fetch(`/company?name=${this.props.match.params.name}`, {
      method: 'PATCH',
      body: JSON.stringify(this.state.form),
      headers: {
        'Content-Type': 'application/json'
      }
    });
    if(this.state.form.newName !== this.props.match.params.name) {
      const link = '/dashboard/Admin/company/manage/edit/' + this.state.form.newName;
      onRedirect(link);
    }
    const cmp = (this.state.form.newName !== this.props.match.params.name) ? this.state.form.newName : "";
    this.getCompany(cmp);
  }

  getCompany = cmp => {
    const name = (cmp) ? cmp : this.props.match.params.name;
    fetch(`/Company?name=${name}`)
    .then(company => {
      return company.json();
    }).then(data => {
      this.setState({
        data,
        form: {
          newName: data.Name,
          newStreet: data.Street,
          newHouseNumber: data.HouseNumber,
          newPostCode: data.PostCode,
          newCity: data.City,
          newCountry: data.Country,
          newPhoneNumber: data.PhoneNumber,
          newEmail: data.Email
        }
      });
    })
  }

  render() {
    console.log(this.state.form);
    console.log(this.state.data);
    return (
      <div>
        <h1><b>Edit Company:</b> {this.state.data.Name}</h1><br />

        <form onSubmit={this.handleSubmit.bind(this)}>
          <div class="form-group">
            <label for="Name">Name</label>
            <input onChange={this.handleChange} type="text" name="newName" class="form-control" id="Name" value={this.state.form.newName} />
          </div>

          <div class="form-group">
            <label for="street">Street</label>
            <input onChange={this.handleChange} type="text" name="newStreet" class="form-control" id="street" value={this.state.form.newStreet} />
          </div>

          <div class="form-group">
            <label for="houseNumber">House number</label>
            <input onChange={this.handleChange} type="text" name="newHouseNumber" class="form-control" id="houseNumber" value={this.state.form.newHouseNumber} />
          </div>

          <div class="form-group">
            <label for="postCode">Postcode</label>
            <input onChange={this.handleChange} type="text" name="newPostCode" class="form-control" id="postCode" value={this.state.form.newPostCode} />
          </div>

          <div class="form-group">
            <label for="city">City</label>
            <input onChange={this.handleChange} type="text" name="newCity" class="form-control" id="city" value={this.state.form.newCity} />
          </div>

          <div class="form-group">
            <label for="country">Country</label>
            <input onChange={this.handleChange} type="text" name="newCountry" class="form-control" id="country" value={this.state.form.newCountry} />
          </div>

          <div class="form-group">
            <label for="phoneNumber">Phone number</label>
            <input onChange={this.handleChange} type="text" name="newPhoneNumber" class="form-control" id="phoneNumber" value={this.state.form.newPhoneNumber} />
          </div>

          <div class="form-group">
            <label for="email">E-mail</label>
            <input onChange={this.handleChange} type="text" name="newEmail" class="form-control" id="email" value={this.state.form.newEmail} />
          </div>

          <button type="submit" class="btn btn-primary">Edit Company</button>
        </form>
      </div>
    );
  }
}

export default EditCompany;