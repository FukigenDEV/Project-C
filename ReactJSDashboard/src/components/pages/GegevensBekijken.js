import React, { Component } from 'react';
class GegevensBekijken extends Component {
  render() {
    return (
      <div className="shadow-sm p-3 mb-5 bg-white rounded">
        <h1>Gegevens bekijken</h1>
        <br />
        <div className="tabel">
          <table>
            <tr>
              <th>Naam</th>
              <th>Afdeling</th>
              <th>Aangemaakt op</th>
            </tr>
            <tr>
              <td>Very interesting collection of data #1</td>
              <td>HR</td>
              <td>13/10/19 14:03</td>
            </tr>
            <tr>
              <td>Very interesting collection of data #2</td>
              <td>HR</td>
              <td>13/10/19 15:34</td>
            </tr>
            <tr>
              <td>Very interesting collection of data #3</td>
              <td>IT</td>
              <td>13/10/19 15:47</td>
            </tr>
            <tr>
              <td>Very interesting collection of data #4</td>
              <td>IT</td>
              <td>13/10/19 15:52</td>
            </tr>
          </table>
        </div>
      </div>
    );
  }
}

export default GegevensBekijken;