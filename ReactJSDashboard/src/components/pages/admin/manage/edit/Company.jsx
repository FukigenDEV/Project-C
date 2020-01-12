import React, { Component } from 'react';
import { standard, email } from '../../../../form/fieldcheck';
import { Form } from '../../../../../index';

class EditCompany extends Component {
  constructor(props) {
    super(props)
    this.state = {
      alert: {
        type: 0,
        value: ''
      },
      forms: [
        {fieldname: 'Naam bedrijf', name: 'newName', type: 'text', placeholder: false, check: standard},
        {fieldname: 'Straat', name: 'newStreet', type: 'text', placeholder: false},
        {fieldname: 'Huisnummer', name: 'newHouseNumber', type: 'text', placeholder: false},
        {fieldname: 'Postcode', name: 'newPostCode', type: 'text', placeholder: false},
        {fieldname: 'Woonplaats', name: 'newCity', type: 'text', placeholder: false},
        {fieldname: 'Land', name: 'newCountry', type: 'text', placeholder: false},
        {fieldname: 'Telefoonnummer', name: 'newPhoneNumber', type: 'text', placeholder: false},
        {fieldname: 'E-mailadres', name: 'newEmail', type: 'text', placeholder: false, check: email},
      ],
      buttonname: "Bedrijf bewerken",
      form: [],
      data: [],
    }
  }

  componentDidMount() {
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
    }).then(response => {
      const alert = {...this.state.alert};
      console.log(response['status']);
      console.log(response['status'].toString()[0]);
      alert['active'] = true;
      alert['content'] = (response['status'].toString()[0] === "2") ? "Het bedrijf is succesvol aangepast" : "Server error";
      alert['type'] = (response['status'].toString()[0] === "2") ? "success" : "error";
      this.setState({alert});
    });
    if(this.state.form.newName !== this.props.match.params.name) {
      const link = '/dashboard/Admin/company/manage/edit/' + this.state.form.newName;
      onRedirect(link);
    }
    // const cmp = (this.state.form.newName !== this.props.match.params.name) ? this.state.form.newName : "";
    // this.getCompany(cmp);
  }

  getData = (component, cmp) => {
    const name = (cmp) ? cmp : component.props.match.params.name;
    fetch(`/Company?name=${name}`)
    .then(company => {
      return company.json();
    }).then(data => {
      component.setState({
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

  setForm = ext_form => {
    const form = {...this.state.form};
    Object.keys(ext_form).forEach(key => {
      form[key] = ext_form[key];
    });
    this.setState({form});
    console.log(this.state.form);
  }

  setComplete = ext_bool => {
    const complete = ext_bool;
    this.setState({complete});
  }

  getSubmitAlertElement = () => {
    const alert = this.state.alert;
    const alert_active = (alert['active']) ? "d-block" : "d-none";
    const alert_class = (alert['type'] === "success") ? "alert-success" : "alert-danger";
    return <div class={`alert ${alert_class} ${alert_active}`}>{alert['content']}</div>
  }

  render() {
    console.log(this.state.form);
    console.log(this.state.data);
    const {action, api, forms, buttonname} = this.state;
    if(typeof(forms) === 'object') {
      return (
        <div>
          {this.getSubmitAlertElement()}
          <h1><b>Bedrijf bewerken: </b>{this.props.match.params.name}</h1><br />
          <form onSubmit={this.handleSubmit.bind(this)}>
              <Form
                {...this.props}
                action={action}
                api={api}
                forms={forms}
                buttonname={buttonname}
                getData={this.getData}
                setForm={this.setForm}
                setComplete={this.setComplete}
              />
              <button type="submit" class="btn btn-primary" disabled={!this.state.complete}>{buttonname}</button>
          </form>
        </div>
      );
    } else {
      return <></>;
    }
  }
}

export default EditCompany;