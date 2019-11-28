import React, { Component } from 'react';

class addDepartments extends Component {
  constructor(props) {
    super(props)
    this.state = {
      alert: {
        type: 0,
        value: ''
      },

      form: {
        'name': '',
        'description': ''
      }
    }
  }

  handleChange = (event) => {
    event.preventDefault();
    const form = {...this.state.form};
    form[event.target.name] = event.target.value;
    this.setState({form});
  }

  handleSubmit = (event) => {
    event.preventDefault();
    const obj = this.state.form;
    const data = JSON.stringify(obj);

    let xhr = new XMLHttpRequest();
    xhr.open("POST", "/department", true);
    xhr.onreadystatechange = () => {
      if(xhr.readyState === 4) {
        if(xhr.status === 200) {
          const alert = {...this.state.alert};
          alert.type = 200;
          alert.value = 'Department succesfully added';
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

  getBadgeClasses = () => {
    let classes = 'alert mr-3 ml-3 ';
    classes += (this.state.alert.type === 200) ? "alert-success" : "alert-danger";
    classes += (this.state.alert.type === 0) ? " d-none" : " d-block";
    return classes;
  }

  render() {
    console.log(this.state.form);
    console.log(this.state.alert);
    return (
      <div>
        <form onSubmit={this.handleSubmit.bind(this)}>
          <div class="form-group">
            <label for="department">Department name</label>
            <input onChange={this.handleChange} type="text" name="name" class="form-control" id="department" placeholder="Enter department name" />
          </div>

          <div class="form-group">
            <label for="description">Description</label>
            <input onChange={this.handleChange} type="text" name="description" class="form-control" id="description" placeholder="Enter description" />
          </div>

          <div className={this.getBadgeClasses()}>{this.state.alert.value}</div>

          <button type="submit" class="btn btn-primary">Add department</button>
        </form>
      </div>
    );
  }
}

export default addDepartments;