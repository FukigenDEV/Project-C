using Configurator;
using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;

namespace Webserver.Data {
	class Session {
		public int ID { get; set; }
		public int User { get; set; }
		public int Token { get; set; }
		public string SessionID { get; set; }
		public bool RememberMe { get; set; }

		/// <summary>
		/// Creates a new user Session
		/// </summary>
		/// <param name="User">The user this session belongs to</param>
		/// <param name="RememberMe"></param>
		public Session(long User, bool RememberMe) {
			this.SessionID = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
			this.User = (int)User;
			this.RememberMe = RememberMe;
			this.Token = (int)DateTime.UtcNow.Ticks;
		}

		/// <summary>
		/// Constructor for deserializing database rows into Session objects
		/// </summary>
		public Session(long ID, string SessionID, long User, long Token, long RememberMe) {
			this.ID = (int)ID;
			this.User = (int)User;
			this.Token = (int)Token;
			this.SessionID = SessionID;
			this.RememberMe = ((int)RememberMe == 1) ? true : false;
		}

		private static DateTime Epoch { get; } = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

		/// <summary>
		/// Gets a user session. If the session doesn't exist or is out of date, null will be returned.
		/// </summary>
		/// <param name="Connection"></param>
		/// <returns></returns>
		public static Session GetUserSession(SQLiteConnection Connection, string SessionID) {
			Session s = Connection.QueryFirst<Session>("SELECT * FROM Sessions WHERE SessionID = @SessionID", new { SessionID });
			if (s == null) return null;

			long Timeout;
			if (s.RememberMe) {
				Timeout = (long)Config.GetValue("AuthenticationSettings.SessionTimeoutLong");
			} else {
				Timeout = (long)Config.GetValue("AuthenticationSettings.SessionTimeoutShort");
			}
			long TokenAge = (long)(DateTime.UtcNow - Epoch.AddSeconds(s.Token).ToLocalTime()).TotalSeconds;

			if(TokenAge > Timeout) {
				Connection.Delete(s);
				return null;
			} else {
				return s;
			}
		}
	}
}
