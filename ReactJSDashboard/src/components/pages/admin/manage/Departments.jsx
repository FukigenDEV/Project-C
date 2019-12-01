import React, { Component } from 'react';
import { Link } from "react-router-dom";
import 'bootstrap/dist/css/bootstrap.min.css';

class manDepartments extends Component {
  constructor(props) {
    super(props)
    this.state = {
      alert: {
        type: 0,
        value: ''
      },
      data: [],
    }
  }

  componentDidMount() {
    this.getDepartments();
  }

  handleDelete = async (name) => {
    await fetch(`/department?name=${name}`, {method: 'DELETE',});
    this.getDepartments();
  }

  getDepartments = () => {
    fetch('/department?name=')
    .then(departments => {
      return departments.json();
    }).then(data => {
      this.setState({data});
    })
  }

  render() {
    return (
      <React.Fragment>
        <table className="table table-striped table-dark">
        <thead>
          <tr>
            <th scope="col">#</th>
            <th scope="col">Name</th>
            <th scope="col">Description</th>
            <th scope="col">Edit</th>
            <th scope="col">Delete</th>
          </tr>
        </thead>
        <tbody>
          {this.state.data.map(department => (
            <tr>
              <th scope="row">{department.ID}</th>
              <td>{department.Name}</td>
              <td>{department.Description}</td>
              <td><Link to={`/dashboard/Admin/departments/manage/edit/${department.Name}`}>Edit</Link></td>
              <td><a href onClick={() => this.handleDelete(department.Name)}>Delete</a></td>
            </tr>
          ))}
          </tbody>
        </table>
      </React.Fragment>
    );
  }
}

export default manDepartments;