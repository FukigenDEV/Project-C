import React, { Component } from 'react';

class AddUsers extends Component {
  constructor(props) {
    super(props)
    this.state = {
      alert: {
        type: 0,
        value: ''
      },

      form: {
        'Email': '',
        'Password': '',
        'MemberDepartments': {"_d1" : "User"},
      },
      depts: {
        "_d1" : "_d1",
      },
      data: [],
    }
  }

  componentDidMount() {
    this.getDepartments();
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

  handleSubmit = (event) => {
    event.preventDefault();
    const obj = this.state.form;
    const data = JSON.stringify(obj);

    let xhr = new XMLHttpRequest();
    xhr.open("POST", "/account", true);
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

  render() {
    console.log(this.state.form);
    console.log(this.state.depts);
    console.log(Object.keys(this.state.form["MemberDepartments"]));
    return (
      <div>
        <form onSubmit={this.handleSubmit.bind(this)}>
          <div class="form-group">
            <label for="Email">E-mailadres</label>
            <input onChange={this.handleChange} type="text" name="Email" class="form-control" id="Email" placeholder="E-mailadres" />
          </div>

          <div class="form-group">
            <label for="Password">Wachtwoord</label>
            <input onChange={this.handleChange} type="password" name="Password" class="form-control" id="Password" placeholder="Wachtwoord" />
          </div>

          <div class="form-group">
            <a href onClick={this.addDept.bind(this)}>Meer departments toevoegen</a>
          </div>

          {Object.keys(this.state.depts).map(dept => (
            <React.Fragment>
              <div class="form-group float-left dept">
                <label for="Type">Afdeling</label>
                <select onChange={this.handleChange} name={dept} class="form-control" id="Department">
                  <option value="">Selecteer afdeling...</option>
                  {this.state.data.map(department => (<option value={department.Name}>{department.Name}</option>))}
                </select>
              </div>

              <div class="form-group float-right dept">
                <label for="Type">Autorisatie</label>
                <select onChange={this.handleChange} name={`_a${dept[2]}`} class="form-control" id="Type">
                  <option value="">Selecteer autorisatie...</option>
                  <option value="Administrator">Administrator</option>
                  <option value="Manager">Manager</option>
                  <option value="DeptMember">Department Member</option>
                  <option value="User">User</option>
                </select>
              </div>
            </React.Fragment>
          ))}

          <button type="submit" class="btn btn-primary">Add user</button>
        </form>
      </div>
    );
  }
}

export default AddUsers;