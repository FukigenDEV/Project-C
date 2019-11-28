import React, { Component } from 'react';
import 'bootstrap/dist/css/bootstrap.min.css';

class manCompany extends Component {
  constructor(props) {
    super(props)
    this.state = {
      alert: {
        type: 0,
        value: ''
      },
      data: []
    }
  }

  componentDidMount() {
    fetch('/company?name=')
    .then(companies => {
      return companies.json();
    }).then(data => {
      this.setState({data});
      console.log(this.state.data);
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
          </tr>
        </thead>
        <tbody>
          {/* {this.state.data.map(company => (
            <tr>
              <th scope="row">{company.ID}</th>
              <td>{company.Name}</td>
              <td>{company.Description}</td>
            </tr>
          ))} */}
          </tbody>
        </table>
      </React.Fragment>
    );
  }
}

export default manCompany;