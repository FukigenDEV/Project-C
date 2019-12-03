import React, { Component } from 'react';
import { Link } from "react-router-dom";
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
    await fetch(`/account?name=${name}`, {method: 'DELETE',});
    this.getUsers();
  }

  getUsers = () => {
    fetch('/account')
    .then(users => {
      return users.json();
    }).then(data => {
      this.setState({data});
      console.log(this.state.data);
    })
  }

  render() {
    // const userlist =
    //   this.state.data.map(user => (
    //     <tr>
    //       <th scope="row">{user.ID}</th>
    //       <td>{user}</td>
    //       <td><Link to={`/dashboard/Admin/users/manage/details/${user.ID}`}>Details</Link></td>
    //       <td><Link to={`/dashboard/Admin/users/manage/edit/${user}`}>Edit</Link></td>
    //       <td><a href onClick={() => this.handleDelete(user)}>Delete</a></td>
    //     </tr>
    //   ));

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
          {/* {userlist} */}
          </tbody>
        </table>
      </React.Fragment>
    );
  }
}

export default ManUsers;