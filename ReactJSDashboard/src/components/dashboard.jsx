import React, { Component } from 'react';
import {BrowserRouter as Router, Switch, Route, Link, withRouter} from "react-router-dom";
import { createHashHistory } from 'history';
import { Admin, Uitloggen, Error, Navs} from '../index';

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
                <Route path="/dashboard/Admin" render={() => <Admin onRedirect={onRedirect} />} />
                {/*dit geeft de value "loggedin" en de method "onRedirect" door aan de uitlogpagina zodat deze daar gebruikt kunnen worden*/}
                <Route exact path="/dashboard/Uitloggen" render={() => <Uitloggen loggedin={loggedin} onLogout={onRedirect} />} />
                { navs.filter(nav => (nav.heading !== "Admin" && nav.heading !== "Uitloggen")).map(nav => (<Route exact path={nav.path} component={nav.component} />)) }
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
//   render() {
//     const {navs, loggedin, onSelect, onRedirect, onRender} = this.props;
//     onRender();

//     if(loggedin.value === true) {
//       return (
//         <div className="container-fluid">
//           <div className="row">
//             <Navs navs={navs} onSelect={onSelect} />
//             <div className="col-9 col-s-12 main-window">
//               <Switch>
//                 { navs.map(nav => (<Route exact path={nav.path} component={nav.component} />)) }
//                 <Route component={Error} />
//               </Switch>
//             </div>
//           </div>
//         </div>
//       );
//     } else {
//       onRedirect('/');
//       return (<div></div>);
//     }
//   }
// }

