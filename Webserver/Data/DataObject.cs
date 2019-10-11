using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Reflection;
using System.Text;

namespace Webserver.Data {
	/// <summary>
	/// Interface for all database objects
	/// </summary>
	interface DataObject {
		int ID { get; set; }
	}
}
