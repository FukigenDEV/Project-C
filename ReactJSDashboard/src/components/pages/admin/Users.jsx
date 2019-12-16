import React, { Component } from 'react';
import { BrowserRouter as Switch, Route, Link } from "react-router-dom";
import { EditUsers, ManUsers, AddUsers } from '../../../index';

class Users extends Component {
  render() {
    const {getNavClass, onRedirect} = this.props;
    return (
      <React.Fragment>
        <div class="nav">
          <ul>
            <li className={getNavClass('add')}><Link to="/dashboard/Admin/users/add">Toevoegen</Link></li>
            <li className={getNavClass('manage')}><Link to="/dashboard/Admin/users/manage">Beheren</Link></li>
          </ul>
        </div>

        <div className="shadow-sm p-3 bg-white rounded">
          <Route exact path="/dashboard/Admin" component={AddUsers} />
          <Route exact path="/dashboard/Admin/users" component={AddUsers} />
          <Route exact path="/dashboard/Admin/users/add" component={AddUsers} />
          <Route exact path="/dashboard/Admin/users/manage" component={ManUsers} />
          <Route path="/dashboard/Admin/users/manage/details/:id" component={ManUsers} />
          <Route path="/dashboard/Admin/users/manage/edit/:name" render={props => <EditUsers {...props} onRedirect={onRedirect} />} />
        </div>
      </React.Fragment>
    );
  }
}

export default Users;