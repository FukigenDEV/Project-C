import React, { Component } from 'react';
import { BrowserRouter as Switch, Route, Link } from "react-router-dom";
import { manCompany, addCompany } from '../../../index';

class Users extends Component {
  render() {
    return (
      <React.Fragment>
        <div class="nav">
          <ul>
            <li class="company"><Link to="/dashboard/Admin/company/add">Add</Link></li>
            <li class="company"><Link to="/dashboard/Admin/company/manage">Manage</Link></li>
          </ul>
        </div>

        <div className="shadow-sm p-3 mb-5 bg-white rounded">
          <Route exact path="/dashboard/Admin/company/add" component={addCompany} />
          <Route exact path="/dashboard/Admin/company/manage" component={manCompany} />
        </div>
      </React.Fragment>
    );
  }
}

export default Users;