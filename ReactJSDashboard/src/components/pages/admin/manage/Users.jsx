import React, { Component } from 'react';
import { Link } from "react-router-dom";
import { AdminModal} from '../../../../index';
import 'bootstrap/dist/css/bootstrap.min.css';

class ManUsers extends Component {
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
    this.getUsers();
  }

  handleDelete = async (name) => {
    await fetch(`/account`, {
      method: 'DELETE',
      body: JSON.stringify({Email: name}),
      headers: {
        'Content-Type': 'application/json'
      }
    });
    this.getUsers();
  }

  getUsers = async () => {
    await fetch('/account')
    .then(users => {
      return users.json();
    }).then(data => {
      this.setState({data});
    })
  }

  render() {
    let userid = -1;
    const userlist =
      this.state.data.map(user => (
        userid = userid + 1,
        <tr>
          <th scope="row">{user.ID}</th>
          <td>{user.Email}</td>
          <td><AdminModal id={userid} data={this.state.data} modalTitle={'Gebruiker details'} buttonLabel='Details' /></td>
          <td><Link to={`/dashboard/Admin/users/manage/edit/${user.Email}`}>Edit</Link></td>
          <td><a href onClick={() => this.handleDelete(user.Email)}>Delete</a></td>
        </tr>
      ));

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
          {userlist}
          </tbody>
        </table>

      </React.Fragment>
    );
  }
}

export default ManUsers;