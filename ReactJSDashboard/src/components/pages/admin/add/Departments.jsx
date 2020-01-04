import React, { Component } from 'react';
import { Form } from '../../../../index';
import { standard } from '../../../form/fieldcheck';

class addDepartments extends Component {
  constructor(props) {
    super(props)
    this.state = {
      complete: false,
      action: 'POST',
      api: '/department',
      forms: [
      {fieldname: 'Naam van afdeling', name: 'name', type: 'text', placeholder: 'Naam invullen...', check: standard},
      {fieldname: 'Beschrijving', name: 'description', type: 'text', placeholder: 'Beschrijving invullen...', check: standard}
      ],
      buttonname: 'Afdeling toevoegen',
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
    if(typeof(forms) === 'object') {
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
    } else {
      return <></>;
    }
  }
}

export default addDepartments;