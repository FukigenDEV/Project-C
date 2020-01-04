import React, { Component } from 'react';
import { Field } from '../../index';

class Form extends Component {
  constructor(props) {
    super(props)
    this.state = {
      alert: {},
      alert_type: {},
    }
  }

  componentDidMount() {
    const { forms } = this.props;
    this.setState({forms});
    this.setAlertType().then(() => {
      this.setAlert();
    })
  }

  handleChange = (event) => {
    event.preventDefault();
    const {setForm, setComplete} = this.props;
    const form = {...this.state.form};
    const alert = {...this.state.alert};
    form[event.target.name] = event.target.value;
    if(this.state.alert_type[event.target.name] !== undefined) {
      alert[event.target.name] = this.state.alert_type[event.target.name](event.target.value);
    }
    setForm(form);
    this.setState({alert});
    return Promise.resolve(alert).then(() => {
      setComplete(this.isComplete());
    })
  }

  setAlertType = () => {
    const { forms } = this.props;
    const alert_type = {...this.state.alert_type};
    for(let i=0; i<forms.length; i++) {
      alert_type[forms[i]['name']] = forms[i]['check'];
    }
    this.setState({alert_type});
    return Promise.resolve(alert_type);
  }

  setAlert = () => {
    const { forms } = this.props;
    const alert = {...this.state.alert};
    for(let i=0; i<forms.length; i++) {
      console.log(this.state.alert_type);
      alert[forms[i]['name']] = (this.state.alert_type[forms[i]['name']] !== undefined) ? undefined : false;
    }
    this.setState({alert});
  }

  isComplete = () => {
    const alerts = Object.keys(this.state.alert_type);
    console.log(alerts);
    for(let i=0; i<alerts.length; i++) {
      if(this.state.alert[alerts[i]] !== false) {
        return false;
      }
    }
    return true;
  }

  render() {
    const {forms, buttonname} = this.props;
    console.log(forms);
    console.log(this.state.alert_type);
    console.log(this.state.alert);
    return (
      <>
        {forms.map(form => (
          <div class="form-group">
            <label for={form['name']}>{form['fieldname']}</label><span>{this.state.alert[form['name']]}</span>
            <Field
              key={form['name']}
              handleChange={this.handleChange}
              fieldname={form['fieldname']}
              name={form['name']}
              type={form['type']}
              placeholder={form['placeholder']}
              data={form['data']}
            />
          </div>
        ))}
      </>
    );
  }
}

export default Form;