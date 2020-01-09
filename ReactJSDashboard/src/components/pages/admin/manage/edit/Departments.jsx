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
        {fieldname: 'Beschrijving', name: 'newDescription', type: 'text', placeholder: false, check: standard},
      ],
      buttonname: "Afdeling bewerken",
      form: [],
      data: [],
      alert: {active: false, type: "", content: ""},
    }
  }

  componentDidMount() {
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
    }).then(response => {
      const alert = {...this.state.alert};
      console.log(response['status']);
      console.log(response['status'].toString()[0]);
      alert['active'] = true;
      alert['content'] = (response['status'].toString()[0] === "2") ? "De afdeling is succesvol aangepast" : "Server error";
      alert['type'] = (response['status'].toString()[0] === "2") ? "success" : "error";
      this.setState({alert});
    })
    if(this.state.form.newName !== this.props.match.params.name) {
      const link = '/dashboard/Admin/departments/manage/edit/' + this.state.form.newName;
      onRedirect(link);
    }
    // const dept = (this.state.form.newName !== this.props.match.params.name) ? this.state.form.newName : "";
    // this.getDepartment(dept);
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

  getData = (component, dept) => {
    const name = (dept) ? dept : component.props.match.params.name;
    fetch(`/department?name=${name}`) 
    .then(department => {
      return department.json();
    }).then(data => {
      const forms = [...component.state.forms];
      const list = [data.Name, data.Description];
      let count = 0;
      forms.forEach(form => {
        form['value'] = list[count];
        count++;
      })
      component.setState({data, forms, form: {newName: data.Name, newDescription: data.Description}, });
    })
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
          <h1><b>Afdeling bewerken: </b>{this.props.match.params.name}</h1><br />
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

export default EditDepartments;