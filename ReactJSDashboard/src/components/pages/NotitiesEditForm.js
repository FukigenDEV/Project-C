import React, { Component } from 'react';
import ReactDOM from 'react-dom'
import $ from 'jquery';
import './Style/Notities.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faTrashAlt, fas, faPencilAlt } from '@fortawesome/free-solid-svg-icons';
import { library } from '@fortawesome/fontawesome-svg-core';
import { delay } from 'q';
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
	componentWillUnmount(){
		console.log("Form Unmounted");
	};
	handleTitleChange = event => {
		this.setState({ editNoteTitle: event.target.value });
	};

	handleTextChange = event => {
		this.setState({ editNoteText: event.target.value });
	};
	handleSubmit = async () => {
		console.log("Title: " + this.props.note.Title + "\nText: " + this.state.editNoteText)
		await fetch('/note', {
			method: 'PATCH',
			body: JSON.stringify({ title: this.props.note.Title, newText: this.state.editNoteText, newTitle: this.state.editNoteTitle }),
			headers: {
				'Content-Type': 'application/json'
			}
		}).then(Response =>	{
			console.log(Response.body);
			console.log(Response.statusText);
			var test = Response;
			this.props.reloadNotes(test);
		}
		);
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
				<button class="btn btn-secondary" type="button">Annuleren</button><br /><br />
			</div>
		);
	}
}

export default Notities;