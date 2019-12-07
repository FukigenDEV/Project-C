import React, { Component } from 'react';
import { BrowserRouter as Switch, Route, Link } from "react-router-dom";
import { EditDepartments, manDepartments, addDepartments } from '../../../index';

class Departments extends Component {
  render() {
    const {onRedirect} = this.props;
    return (
      <React.Fragment>
        <div class="nav">
          <ul>
            <li class="users"><Link to="/dashboard/Admin/departments/add">Add</Link></li>
            <li class="departments"><Link to="/dashboard/Admin/departments/manage">Manage</Link></li>
          </ul>
        </div>

        <div className="shadow-sm p-3 mb-5 bg-white rounded">
          <Route exact path="/dashboard/Admin/departments" component={addDepartments} />
          <Route exact path="/dashboard/Admin/departments/add" component={addDepartments} />
          <Route exact path="/dashboard/Admin/departments/manage" component={manDepartments} />
          <Route path="/dashboard/Admin/departments/manage/edit/:name" render={props => <EditDepartments {...props} onRedirect={onRedirect} />} />
        </div>
      </React.Fragment>
    );
  }
}

export default Departments;