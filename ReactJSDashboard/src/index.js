import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';
import App from "./components/app/App";
import Dashboard from './components/dashboard';
import Home from './components/pages/Home'

import Admin from './components/pages/admin/Admin';

import Users from './components/pages/admin/Users';
import AddUsers from './components/pages/admin/add/Users';
import ManUsers from './components/pages/admin/manage/Users';

import Departments from './components/pages/admin/Departments';
import addDepartments from './components/pages/admin/add/Departments';
import manDepartments from './components/pages/admin/manage/Departments';
import EditDepartments from './components/pages/admin/manage/edit/Departments';

import Company from './components/pages/admin/Company';
import addCompany from './components/pages/admin/add/Company';
import manCompany from './components/pages/admin/manage/Company';
import EditCompany from './components/pages/admin/manage/edit/Company';

import Navs from './components/navs';
import Gegevens from './components/pages/Gegevens';
import Notities from './components/pages/Notities';
import Activiteitengeschiedenis from './components/pages/Activiteitengeschiedenis';
import Backup from './components/pages/Backup';
import Uitloggen from './components/pages/Uitloggen';
import AdminWizard from './components/pages/AdminWizard';
import Error from './components/pages/Error';

import * as serviceWorker from './serviceWorker';

ReactDOM.render(<App />, document.getElementById('root'));

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.unregister();

export {
    Dashboard,
    Navs,
    Home,
    Admin,
    Users,
    AddUsers,
    ManUsers,
    Departments,
    addDepartments,
    manDepartments,
    EditDepartments,
    Company,
    addCompany,
    manCompany,
    EditCompany,
    Gegevens,
    Notities,
    Activiteitengeschiedenis,
    Backup,
    Uitloggen,
    Error,
	AdminWizard
}