import React, { Component } from 'react';
import {BrowserRouter as Router, Switch, Route, Link, withRouter} from "react-router-dom";
import { Users, Departments, Company } from '../../../index';
import { throwStatement } from '@babel/types';

class Admin extends Component {
  constructor(props) {
    super(props)
  }

  getNavClass = (name) => {
    const path = this.props.location.pathname;
    if(name !== 'users') {
      if(path.includes(`/dashboard/Admin/${name}`)) { return `${name} active` } else { return name }
    } else {
      if(path === '/dashboard/Admin' || path.includes('dashboard/Admin/users')) { return 'users active' } else { return 'users' }
    }
  }

  getSubNavClass = (name) => {
    const path = this.props.location.pathname;
    if(path.includes(name)) { return `${name} active` } else { return name }
  }

  render() {
    const {onRedirect} = this.props;
    return (
      <React.Fragment>
        <div class="nav">
          <ul>
            <li class={this.getNavClass('users')}><Link to="/dashboard/Admin/users">Users</Link></li>
            <li class={this.getNavClass('departments')}><Link to="/dashboard/Admin/departments">Departments</Link></li>
            <li class={this.getNavClass('company')}><Link to="/dashboard/Admin/company">Company</Link></li>
          </ul>
        </div>

        <div className="shadow-sm p-3 bg-white rounded">
          <Route exact path="/dashboard/Admin"  render={props => <Users {...props} getNavClass={this.getSubNavClass} onRedirect={onRedirect} />} />
          <Route path="/dashboard/Admin/users" render={props => <Users {...props} getNavClass={this.getSubNavClass} onRedirect={onRedirect} />} />
          <Route path="/dashboard/Admin/departments" render={props => <Departments {...props} getNavClass={this.getSubNavClass} onRedirect={onRedirect} />} />
          <Route path="/dashboard/Admin/company" render={props => <Company {...props} getNavClass={this.getSubNavClass} onRedirect={onRedirect} />} />
        </div>
      </React.Fragment>
    );
  }
}

export default Admin;