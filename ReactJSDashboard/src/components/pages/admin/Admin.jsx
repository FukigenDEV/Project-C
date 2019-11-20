import React, { Component } from 'react';
import { BrowserRouter as Switch, Route, Link } from "react-router-dom";
import { addUsers, addDepartments, addCompany } from '../../../index';

class Admin extends Component {
  render() {
    return (
      <React.Fragment>
        <div class="nav">
          <ul>
            <li class="users"><Link to="/dashboard/Admin/users/add">Users</Link></li>
            <li class="departments"><Link to="/dashboard/Admin/departments/add">Departments</Link></li>
            <li class="company"><Link to="/dashboard/Admin/company/add">Company</Link></li>
          </ul>
        </div>

        <div className="shadow-sm p-3 mb-5 bg-white rounded">
          <Route exact path="/dashboard/Admin/users/add" component={addUsers} />
          <Route exact path="/dashboard/Admin/departments/add" component={addDepartments} />
          <Route exact path="/dashboard/Admin/company/add" component={addCompany} />
        </div>
      </React.Fragment>
    );
  }
}

export default Admin;