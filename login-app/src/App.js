import React, { Component } from 'react';
import styled from 'styled-components';
import { createGlobalStyle } from 'styled-components'
import Login from './components/login';
import './stylesheets/App.css';

const GlobalStyle = createGlobalStyle`
  body {
    background: rgb(2,0,36) !important;
    background: linear-gradient(90deg, rgba(2,0,36,1) 0%, rgba(35,35,102,1) 30%, rgba(0,142,255,1) 100%) !important;
  }
`

class App extends Component {
  render() {
    return (
      <React.Fragment>
        <GlobalStyle />
        <div className="container mx-auto vertical-center">
            <Login />
        </div>
      </React.Fragment>
    );
  }
}

export default App;
