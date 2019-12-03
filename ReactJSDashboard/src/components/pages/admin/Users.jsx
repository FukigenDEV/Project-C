import React, { Component } from 'react';
import { BrowserRouter as Switch, Route, Link } from "react-router-dom";
import { EditCompany, ManUsers, AddUsers } from '../../../index';

class Users extends Component {
  render() {
    const {onRedirect} = this.props;
    return (
      <React.Fragment>
        <div class="nav">
          <ul>
            <li class="company"><Link to="/dashboard/Admin/users/add">Add</Link></li>
            <li class="company"><Link to="/dashboard/Admin/users/manage">Manage</Link></li>
          </ul>
        </div>

        <div className="shadow-sm p-3 mb-5 bg-white rounded">
          <Route exact path="/dashboard/Admin" component={AddUsers} />
          <Route exact path="/dashboard/Admin/users" component={AddUsers} />
          <Route exact path="/dashboard/Admin/users/add" component={AddUsers} />
          <Route exact path="/dashboard/Admin/users/manage" component={ManUsers} />
          <Route path="/dashboard/Admin/users/manage/details/:id" component={ManUsers} />
          <Route path="/dashboard/Admin/users/manage/edit/:name" render={props => <EditCompany {...props} onRedirect={onRedirect} />} />
        </div>
      </React.Fragment>
    );
  }
}

export default Users;