import React, { Component } from 'react';
import { Link } from "react-router-dom";
import 'bootstrap/dist/css/bootstrap.min.css';

class manCompany extends Component {
  constructor(props) {
    super(props)
    this.state = {
      alert: {
        type: 0,
        value: ''
      },
      data: [],
      delete: ""
    }
  }

  componentDidMount() {
    this.getCompanies();
  }

  componentDidUpdate(prevState) {
    if(prevState.delete !== this.state.delete) {
      this.getCompanies();
    }
  }

  getCompanies = () => {
    fetch('/company?name=')
    .then(companies => {
      return companies.json();
    }).then(data => {
      this.setState({data});
      console.log(this.state.data);
    })
  }

  handleDelete = name => {
    fetch(`/company?name=${name}`, {method: 'DELETE',});
    this.setState({delete: name});
    console.log(this.state);
  }

  render() {
    const companylist = (this.state.data.length !== 0) ?
      this.state.data.map(company => (
        <tr>
          <th scope="row">{company.ID}</th>
          <td>{company.Name}</td>
          <td><Link to={`/dashboard/Admin/company/manage/details/${company.ID}`}>Details</Link></td>
          <td><Link to={`/dashboard/Admin/company/manage/edit/${company.ID}`}>Edit</Link></td>
          <td><a href onClick={() => this.handleDelete(company.Name)}>Delete</a></td>
        </tr>
      ))
      :
        <tr>
          <th scope="row">&nbsp;</th>
          <td>Geen bedrijven beschikbaar!</td>
          <td>&nbsp;</td>
          <td>&nbsp;</td>
          <td>&nbsp;</td>
        </tr>
      ;

    return (
      <React.Fragment>
        <table className="table table-striped table-dark">
        <thead>
          <tr>
            <th scope="col">#</th>
            <th scope="col">Name</th>
            <th scope="col">Details</th>
            <th scope="col">Edit</th>
            <th scope="col">Delete</th>
          </tr>
        </thead>
        <tbody>
          {companylist}
          </tbody>
        </table>
      </React.Fragment>
    );
  }
}

export default manCompany;