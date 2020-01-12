import React, { Component } from 'react';
import ReactDOM from 'react-dom'
import $ from 'jquery';
import './Style/Notities.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faTrashAlt, fas, faPencilAlt } from '@fortawesome/free-solid-svg-icons';
import { library } from '@fortawesome/fontawesome-svg-core';
import { delay } from 'q';

const { patchNote } = require('./NoteEditUtils');

library.add(
	fas,
	faTrashAlt,
	faPencilAlt
)

class Notities extends Component {
	constructor(props) {
		super(props)
		this.state = {
			editNoteTitle: '',
			editNoteText: ''
		}
	}

	componentDidMount() {
		this.setState({ editNoteTitle: this.props.note.Title });
		this.setState({ editNoteText: this.props.note.Text });
	};

	handleTitleChange = event => {
		this.setState({ editNoteTitle: event.target.value });
	};

	handleTextChange = event => {
		this.setState({ editNoteText: event.target.value });
	};

	handleReset = () => {
		this.props.reloadNote(this.props.note);
	};

	handleSubmit = () => {
		patchNote(this.props.note, this.state);
	};

	render() {
		return (
			<div class="note_edit_form" >
				<label>Titel:</label><br />
				<textarea type="text" onChange={this.handleTitleChange} style={{ width: "50%", marginBottom: "15px" }} rows={1} >{this.props.note.Title}</textarea>
				<br />
				<label>Tekst:</label><br />
				<textarea onChange={this.handleTextChange} style={{ width: "50%", height: "225px" }} >{this.props.note.Text}</textarea>
				<br />
				<button class="btn btn-primary" onClick={this.handleSubmit}>Veranderen</button>
				<button class="btn btn-secondary" onClick={this.handleReset}>Annuleren</button><br /><br />
			</div>
		);
	}
}

export default Notities;