import React, { Component } from 'react';
import { standard } from '../../../../form/fieldcheck';
import { Form } from '../../../../../index';

class EditDepartments extends Component {
  constructor(props) {
    super(props)
    this.state = {
      action: "PATCH",
      forms: [
        {fieldname: 'Naam afdeling', name: 'newName', type: 'text', placeholder: false, check: standard},
        {fieldname: 'Beschrijving', name: 'NewDescription', type: 'text', placeholder: false, check: standard},
      ],
      buttonname: "Afdeling bewerken",
      form: [],
      data: [],
    }
  }

  componentDidMount() {
    this.getDepartment();
  }

  handleChange = (event) => {
    event.preventDefault();
    const form = {...this.state.form};
    form[event.target.name] = event.target.value;
    this.setState({form});
  }

  handleSubmit = async (event) => {
    event.preventDefault();
    const {action} = this.state;
    const {onRedirect} = this.props;
    await fetch(`/department?name=${this.props.match.params.name}`, {
      method: action,
      body: JSON.stringify(this.state.form),
      headers: {
        'Content-Type': 'application/json'
      }
    });
    if(this.state.form.newName !== this.props.match.params.name) {
      const link = '/dashboard/Admin/departments/manage/edit/' + this.state.form.newName;
      onRedirect(link);
    }
    const dept = (this.state.form.newName !== this.props.match.params.name) ? this.state.form.newName : "";
    this.getDepartment(dept);
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

  getDepartment = dept => {
    const name = (dept) ? dept : this.props.match.params.name;
    fetch(`/department?name=${name}`)
    .then(department => {
      return department.json();
    }).then(data => {
      const forms = [...this.state.forms];
      const list = [data.Name, data.Description];
      let count = 0;
      forms.forEach(form => {
        form['value'] = list[count];
        count++;
      })
      this.setState({data, forms, form: {newName: data.Name, newDescription: data.Description}, });
    })
  }

  render() {
    console.log(this.state.form);
    console.log(this.state.data);
    const {action, api, forms, buttonname} = this.state;
    if(typeof(forms) === 'object') {
      return (
        <div>
          <h1><b>Afdeling bewerken: </b>{this.state.data['name']}</h1><br />
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
        </div>
      );
    } else {
      return <></>;
    }
  }
}

export default EditDepartments;