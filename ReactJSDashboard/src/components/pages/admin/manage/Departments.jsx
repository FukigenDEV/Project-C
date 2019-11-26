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
      delete: ''
    }
  }

  componentDidMount() {
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

  handleDelete = name => {
    fetch(`/department?name=${name}`, {method: 'DELETE'});
    this.setState({delete: name});
    console.log(this.state);
  }

  render() {
    return (
      <React.Fragment>
        <div className="d-none">{this.state.delete}</div>
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
              <td><Link>Edit</Link></td>
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