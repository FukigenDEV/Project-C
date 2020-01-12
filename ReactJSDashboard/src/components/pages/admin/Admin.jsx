import React, { Component, useReducer } from 'react';
import {BrowserRouter as Router, Switch, Route, Link, withRouter} from "react-router-dom";
import { Users, Departments, Company, AdminWizard, Backup, Auth } from '../../../index';

class Admin extends Component {
  constructor(props) {
    super(props)
    this.handleAdminPriv = this.handleAdminPriv.bind(this);
    this.state = {
      render: false,
    }
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

  handleAdminPriv = async () => {
    const {setUser, setAdmin} = this.props;
    await setUser();
    setAdmin();
    this.setState({render: true});
  }

  render() {
    const {admin, onRedirect} = this.props;
    console.log(`admin: ${admin}`);
    if(this.state.render === false) {
      this.handleAdminPriv();
    }

    if(admin === null) {
      return (
        <div></div>
      );
    } else if(admin !== null && admin === true) {
      return (
        <React.Fragment>
          <div class="nav">
            <ul>
              <li class={this.getNavClass('users')}><Link to="/dashboard/Admin/users">Gebruikers</Link></li>
              <li class={this.getNavClass('departments')}><Link to="/dashboard/Admin/departments">Afdelingen</Link></li>
              <li class={this.getNavClass('company')}><Link to="/dashboard/Admin/company">Bedrijven</Link></li>
              <li class={this.getNavClass('wizard')}><Link to="/dashboard/Admin/wizard">Wizard</Link></li>
              <li class={this.getNavClass('backup')}><Link to="/dashboard/Admin/backup">Backup</Link></li>
            </ul>
          </div>

          <div className="shadow-sm p-3 bg-white rounded">
            <Route exact path="/dashboard/Admin"  render={props => <Users {...props} getNavClass={this.getSubNavClass} onRedirect={onRedirect} />} />
            <Route path="/dashboard/Admin/users" render={props => <Users {...props} getNavClass={this.getSubNavClass} onRedirect={onRedirect} />} />
            <Route path="/dashboard/Admin/departments" render={props => <Departments {...props} getNavClass={this.getSubNavClass} onRedirect={onRedirect} />} />
            <Route path="/dashboard/Admin/company" render={props => <Company {...props} getNavClass={this.getSubNavClass} onRedirect={onRedirect} />} />
            <Route exact path="/dashboard/Admin/wizard" render={props => <AdminWizard {...props} />} />
            <Route exact path="/dashboard/Admin/backup" render={props => <Backup {...props} />} />
          </div>
        </React.Fragment>
      );
    } else if(admin !== null && admin === false) {
      return (
        <Auth />
      );
    }
  }
}

export default Admin;