import React, { Component } from 'react';
import { email } from '../../../../form/fieldcheck';
import { Form } from '../../../../../index';

class EditUsers extends Component {
  constructor(props) {
    super(props)
    this.state = {
      alert: {
        type: 0,
        value: ''
      },
      forms: [
        {fieldname: 'E-mailadres gebruiker', name: 'Email', type: 'text', placeholder: false, check: email},
        {fieldname: 'Functie', name: 'Function', type: 'text', placeholder: false},
        {fieldname: 'Voornaam', name: 'Firstname', type: 'text', placeholder: false},
        {fieldname: 'Tussenvoegsel', name: 'MiddleInitial', type: 'text', placeholder: false},
        {fieldname: 'Achternaam', name: 'Lastname', type: 'text', placeholder: false},
        {fieldname: 'Geboortedatum', name: 'Birthday', type: 'text', placeholder: false},
        {fieldname: 'Land', name: 'Country', type: 'text', placeholder: false},
        {fieldname: 'Postcode', name: 'Postcode', type: 'text', placeholder: false},
        {fieldname: 'Adres', name: 'Address', type: 'text', placeholder: false},
        {fieldname: 'Telefoonnummer mobiel', name: 'MobilePhone', type: 'text', placeholder: false},
        {fieldname: 'Telefoonnummer werk', name: 'WorkPhone', type: 'text', placeholder: false},
      ],
      
      buttonname: "Bedrijf bewerken",
      form: [],
      data: []
    }
  }

  componentDidMount() {
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
    }).then(response => {
      const alert = {...this.state.alert};
      console.log(response['status']);
      console.log(response['status'].toString()[0]);
      alert['active'] = true;
      alert['content'] = (response['status'].toString()[0] === "2") ? "De gebruiker is succesvol aangepast" : "Server error";
      alert['type'] = (response['status'].toString()[0] === "2") ? "success" : "error";
      this.setState({alert});
    });

    if(isNewEmail) {
      const link = '/dashboard/Admin/users/manage/edit/' + this.state.form.Email;
      onRedirect(link);
    }

    // const user = (isNewEmail) ? this.state.form.Email : "";
    // this.getUser(user);
  }

  getData = (component, user) => {
    const name = (user) ? user : component.props.match.params.name;
    fetch(`/account?email=${name}`)
    .then(user => {
      return user.json();
    }).then(data => {
      component.setState({data, form: {
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

export default EditUsers;