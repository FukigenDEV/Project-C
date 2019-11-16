import React, { Component } from 'react';
import {BrowserRouter as Router, Switch, Route, Link, withRouter} from "react-router-dom";
import { createHashHistory } from 'history';
import {Error, Navs} from '../index';
import { faUserPlus } from '@fortawesome/free-solid-svg-icons';

const history = createHashHistory();

class Dashboard extends Component {
  componentDidMount() {
    this.props.onMount();
  }

  render() {
    const {navs, loggedin, onSelect, onRedirect, onRender} = this.props;
    onRender();

    if(loggedin.value === true) {
      return (
        <div className="container-fluid">
          <div className="row">
            <Navs navs={navs} onSelect={onSelect} />
            <div className="col-9 col-s-12 main-window">
              <Switch>
                { navs.map(nav => (<Route exact path={nav.path} component={nav.component} />)) }
                <Route component={Error} />
              </Switch>
            </div>
          </div>
        </div>
      );
    } else {
      onRedirect('/');
      return (<div></div>);
    }
  }
}

export default Dashboard;