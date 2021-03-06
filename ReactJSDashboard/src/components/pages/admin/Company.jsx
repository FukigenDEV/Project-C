import React, { Component } from 'react';
import { BrowserRouter as Switch, Route, Link } from "react-router-dom";
import { EditCompany, manCompany, addCompany } from '../../../index';

class Company extends Component {
  render() {
    const {getNavClass, onRedirect} = this.props;
    return (
      <React.Fragment>
        <div class="nav">
          <ul>
          <li className={getNavClass('add')}><Link to="/dashboard/Admin/company/add">Toevoegen</Link></li>
            <li className={getNavClass('manage')}><Link to="/dashboard/Admin/company/manage">Beheren</Link></li>
          </ul>
        </div>

        <div className="shadow-sm p-3 bg-white rounded">
          <Route exact path="/dashboard/Admin/company" component={addCompany} />
          <Route exact path="/dashboard/Admin/company/add" component={addCompany} />
          <Route exact path="/dashboard/Admin/company/manage" component={manCompany} />
          <Route path="/dashboard/Admin/company/manage/details/:id" component={manCompany} />
          <Route path="/dashboard/Admin/company/manage/edit/:name" render={props => <EditCompany {...props} onRedirect={onRedirect} />} />
        </div>
      </React.Fragment>
    );
  }
}

export default Company;