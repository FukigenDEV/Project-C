import React, { Component } from 'react';
import { Form } from '../../../../index';
import { standard } from '../../../form/fieldcheck';

class addCompany extends Component {
  constructor(props) {
    super(props)
    this.state = {
      complete: false,
      action: 'POST',
      api: '/company',
      forms: [
        {fieldname: 'Naam', name: 'name', type: 'text', placeholder: 'Naam invullen...', check: standard},
        {fieldname: 'Straatnaam', name: 'street', type: 'text', placeholder: 'Straatnaam invullen...', check: standard},
        {fieldname: 'Huisnummer', name: 'houseNumber', type: 'text', placeholder: 'Huisnummer invullen...', check: standard},
        {fieldname: 'Postcode', name: 'postCode', type: 'text', placeholder: 'Postcode invullen...', check: standard},
        {fieldname: 'Stad', name: 'city', type: 'text', placeholder: 'Stad invullen...', check: standard},
        {fieldname: 'Land', name: 'country', type: 'text', placeholder: 'Land invullen...', check: standard},
        {fieldname: 'Telefoonnummer', name: 'phoneNumber', type: 'text', placeholder: 'Telefoonnummer invullen...', check: standard},
        {fieldname: 'E-mailadres', name: 'email', type: 'text', placeholder: 'E-mailadres invullen...', check: standard},
      ],
      buttonname: 'Bedrijf toevoegen',
    }
  }

  handleSubmit = async (event) => {
    event.preventDefault();
    const {action, api} = this.state;
    const {onRedirect} = this.props;
    await fetch(api, {
      method: action,
      body: JSON.stringify(this.state.form),
      headers: {
        'Content-Type': 'application/json'
      }
    });
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

  render() {
    const {action, api, forms, buttonname} = this.state;
    return (
      <form onSubmit={this.handleSubmit.bind(this)}>
        <Form
          action={action}
          api={api}
          forms={forms}
          buttonname={buttonname}
          setForm={this.setForm}
          setComplete={this.setComplete}
        />
        <button type="submit" class="btn btn-primary" disabled={!this.state.complete}>{buttonname}</button>
      </form>
    );
  }
}

export default addCompany;