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
import EditUsers from './components/pages/admin/manage/edit/Users';

import Departments from './components/pages/admin/Departments';
import addDepartments from './components/pages/admin/add/Departments';
import manDepartments from './components/pages/admin/manage/Departments';
import EditDepartments from './components/pages/admin/manage/edit/Departments';

import Company from './components/pages/admin/Company';
import addCompany from './components/pages/admin/add/Company';
import manCompany from './components/pages/admin/manage/Company';
import EditCompany from './components/pages/admin/manage/edit/Company';

import Navs from './components/navs';
import GegevensRegistreren from './components/pages/GegevensRegistreren';
import GegevensBekijken from './components/pages/GegevensBekijken';
import Notities from './components/pages/Notities';
import NotitiesEditForm from './components/pages/NotitiesEditForm';
import Logs from './components/pages/Logs';
import Backup from './components/pages/Backup';
import Logout from './components/pages/Logout';
import AdminWizard from './components/pages/AdminWizard';

import Error from './components/pages/Error';
import Auth from './components/pages/Auth';

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
    EditUsers,
    Departments,
    addDepartments,
    manDepartments,
    EditDepartments,
    Company,
    addCompany,
    manCompany,
    EditCompany,
    GegevensRegistreren,
    GegevensBekijken,
    Notities,
    Logs,
    Backup,
    Logout,
    AdminWizard,
    Error,
    Auth,
    NotitiesEditForm,
}