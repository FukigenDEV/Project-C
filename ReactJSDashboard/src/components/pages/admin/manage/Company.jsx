import React, { Component } from 'react';
import { Link } from "react-router-dom";
import { AdminModal} from '../../../../index';
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
    }
  }

  componentDidMount() {
    this.getCompanies();
  }

  handleDelete = async (name) => {
    await fetch(`/company?name=${name}`, {method: 'DELETE',});
    this.getCompanies();
  }

  getCompanies = async () => {
    await fetch('/company?name=')
    .then(companies => {
      return companies.json();
    }).then(data => {
      this.setState({data});
      console.log(this.state.data);
    })
  }

  render() {
    let id = -1;
    const companylist = (this.state.data.length !== 0) ?
      this.state.data.map(company => (
        id = id + 1,
        <tr>
          <th scope="row">{company.ID}</th>
          <td>{company.Name}</td>
          <td><AdminModal id={id} data={this.state.data} modalTitle={'Bedrijf details'} buttonLabel='Details' /></td>
          <td><Link to={`/dashboard/Admin/company/manage/edit/${company.Name}`}>Edit</Link></td>
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