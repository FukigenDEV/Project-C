import React, { Component } from 'react';

class Field extends Component {
  constructor(props) {
    super(props)
    this.state = {
    }
  }

  render() {
    const {fieldname, name, type, placeholder, data, handleChange} = this.props;
    switch(type) {
      case 'password':
      case 'text':
        return (
          <input onChange={handleChange} type={type} name={name} class="form-control" id={name} placeholder={placeholder} />
        );
      default:
        return (
          <></>
        );
    }
  }
}

export default Field;