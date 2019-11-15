import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';
import App from "./components/app/App";
import Dashboard from './components/dashboard';
import Home from './components/pages/Home'
import Navs from './components/navs';
import GegevensRegistreren from './components/pages/GegevensRegistreren';
import GegevensBekijken from './components/pages/GegevensBekijken';
import Notities from './components/pages/Notities';
import Activiteitengeschiedenis from './components/pages/Activiteitengeschiedenis';
import Backup from './components/pages/Backup';
import Uitloggen from './components/pages/Uitloggen';
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
    GegevensRegistreren,
    GegevensBekijken,
    Notities,
    Activiteitengeschiedenis,
    Backup,
    Uitloggen,
    Error
}