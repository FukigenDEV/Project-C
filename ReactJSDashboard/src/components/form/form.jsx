import React, { Component } from 'react';
import { Field } from '../../index';
import { standard } from './fieldcheck';

class Form extends Component {
  constructor(props) {
    super(props)
    this.state = {
      alert: {},
      alert_type: {},
      form: {},
      compare: [],
    }
  }

  componentDidMount() {
    const { forms } = this.props;
    this.setState({forms});
    this.setAlertType().then(() => {
      this.setAlert();
    })
  }

  handleChange = async (event) => {
    event.preventDefault();
    const {setForm, setComplete} = this.props;
    const form = {...this.state.form};
    const alert = {...this.state.alert};
    const {name, value} = event.target;
    const func = this.state.alert_type[name];
    form[name] = value;
    this.setState({form});

    if(func !== undefined && func !== "compare") {
      alert[name] = func(value);
    }

    await setForm(form);

    if(func === "compare") {
      const dict = {};
      const comp_name = (name[0] === "_") ? name.slice(1, name.length) : name;
      dict[comp_name] = value;
      alert[comp_name] = this.compare(dict);
      alert[name] = this.compare(dict);
    }
    
    this.setState({alert});
    Promise.resolve(alert).then(() => {
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

  compare(dict) {
    const dictkey = Object.keys(dict)[0];
    const comparekey = `_${dictkey}`;
    const form = this.props.form;
    if(standard(form[dictkey]) !== false || standard(form[comparekey]) !== false) { return "Wachtwoord velden moeten worden ingevuld!" }
    if(form[dictkey] !== form[comparekey]) { return "Wachtwoord moet gelijk zijn aan elkaar!" }
    return false;
  }

  render() {
    const {forms} = this.props;
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
              value={form['value']}
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