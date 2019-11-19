import React, { Component } from 'react';
import { BrowserRouter as Switch, Route, Link } from "react-router-dom";
import { Users, Departments, Company } from '../../../index';

class Admin extends Component {
  render() {
    return (
      <React.Fragment>
        <div class="nav">
          <ul>
            <li class="users"><Link to="/dashboard/Admin/users">Users</Link></li>
            <li class="departments"><Link to="/dashboard/Admin/departments">Departments</Link></li>
            <li class="company"><Link to="/dashboard/Admin/company">Company</Link></li>
          </ul>
        </div>

        <div className="shadow-sm p-3 mb-5 bg-white rounded">
          <Route exact path="/dashboard/Admin/users" component={Users} />
          <Route exact path="/dashboard/Admin/departments" component={Departments} />
          <Route exact path="/dashboard/Admin/company" component={Company} />
        </div>
      </React.Fragment>
    );
  }
}

export default Admin;