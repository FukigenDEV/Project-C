import React, { Component } from 'react';

class EditDepartments extends Component {
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
    const {onRedirect} = this.props;
    await fetch(`/department?name=${this.props.match.params.name}`, {
      method: 'PATCH',
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

  getDepartment = dept => {
    const name = (dept) ? dept : this.props.match.params.name;
    fetch(`/department?name=${name}`)
    .then(department => {
      return department.json();
    }).then(data => {
      this.setState({data, form: {newName: data.Name, newDescription: data.Description}});
    })
  }

  render() {
    console.log(this.state.form);
    console.log(this.state.data);
    return (
      <div>
        <h1><b>Edit Department:</b> {this.state.data.Name}</h1><br />

        <form onSubmit={this.handleSubmit.bind(this)}>
          <div class="form-group">
            <label for="newDepartment">Department name</label>
            <input onChange={this.handleChange} type="text" name="newName" class="form-control" id="newDepartment" value={this.state.form.newName} />
          </div>

          <div class="form-group">
            <label for="newDescription">Description</label>
            <input onChange={this.handleChange} type="text" name="newDescription" class="form-control" id="newDescription" value={this.state.form.newDescription} />
          </div>

          <button type="submit" class="btn btn-primary">Edit department</button>
        </form>
      </div>
    );
  }
}

export default EditDepartments;