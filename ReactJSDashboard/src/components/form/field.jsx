import React, { Component } from 'react';

class Field extends Component {
  constructor(props) {
    super(props)
    this.state = {
    }
  }

  render() {
    const {name, type, value, placeholder, handleChange} = this.props;
    switch(type) {
      case 'password':
      case 'text':
        return (
          <input onChange={handleChange} type={type} name={name} class="form-control" id={name} value={value} placeholder={placeholder} />
        );
      default:
        return (
          <></>
        );
    }
  }
}

export default Field;