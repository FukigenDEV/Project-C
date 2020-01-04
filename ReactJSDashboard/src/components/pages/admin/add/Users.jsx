import React, { Component } from 'react';
import Form from '../../../form/form';
import { standard, email } from '../../../form/fieldcheck';

class AddUsers extends Component {
  constructor(props) {
    super(props)
    this.state = {
      complete: false,
      form: {
        'MemberDepartments': {"_d1" : "User"},
      },
      depts: {
        "_d1" : "_d1",
      },
      data: [],
      forms: [
        {fieldname: 'E-mailadres', name: 'Email', type: 'text', placeholder: 'E-mailadres invullen...', check: email},
        {fieldname: 'Wachtwoord', name: 'Password', type: 'password', placeholder: 'Wachtwoord invullen...', check: standard},
      ],
      action: 'POST',
      api: '/account',
      buttonname: 'Gebruiker toevoegen',
    }
  }

  componentDidMount() {
    this.getDepartments();
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

  handleChange = (event) => {
    event.preventDefault();
    const form = {...this.state.form};
    const depts = {...this.state.depts};
    if(event.target.name.startsWith("_d")) {
      const item_key = depts[event.target.name];
      form["MemberDepartments"][event.target.value] = form["MemberDepartments"][item_key];
      depts[event.target.name] = event.target.value;
      delete form["MemberDepartments"][item_key];
    } else if(event.target.name.startsWith("_a")) {
      const item_key = depts[`_d${event.target.name[2]}`];
      form["MemberDepartments"][item_key] = event.target.value;
    } else {
      form[event.target.name] = event.target.value;
    }
    this.setState({form, depts});
  }

  getDepartments = () => {
    fetch('/department?name=')
    .then(departments => {
      return departments.json();
    }).then(data => {
      this.setState({data});
    })
  }

  addDept = (event) => {
    event.preventDefault();
    const depts = {...this.state.depts};
    const form = {...this.state.form};
    const dept_amount = Object.keys(depts).length + 1;
    const name = "_d" + dept_amount;
    depts[name] = name;
    form["MemberDepartments"][name] = "User";
    this.setState({form, depts});
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
    console.log(this.state.form);
    const {action, api, form, forms, buttonname} = this.state;

    if (typeof(forms) === 'object') {
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

          {Object.keys(this.state.depts).map(dept => (
            <>
              <div class="form-group float-left dept">
                <label for="Type">Afdeling</label>
                <select onChange={this.handleChange} name={dept} class="form-control" id="Department">
                  <option value="">Selecteer afdeling...</option>
                  {this.state.data.map(department => (<option value={department.Name}>{department.Name}</option>))}
                </select>
              </div>

              <div class="form-group float-right dept">
                <label for="Type">Autorisatie</label><a href onClick={this.addDept.bind(this)} class="float-right">+</a>
                <select onChange={this.handleChange} name={`_a${dept[2]}`} class="form-control" id="Type">
                  <option value="">Selecteer autorisatie...</option>
                  <option value="Administrator">Administrator</option>
                  <option value="Manager">Manager</option>
                  <option value="DeptMember">Department Member</option>
                  <option value="User">User</option>
                </select>
              </div>
            </>
          ))}

          <button type="submit" class="btn btn-primary" disabled={!this.state.complete}>{buttonname}</button>
        </form>
      );
    } else {
      return <></>;
    }
  }
}

export default AddUsers;