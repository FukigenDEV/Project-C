using Configurator;
using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Data.SQLite;

namespace Webserver.Data {
	/// <summary>
	/// A user login session
	/// </summary>
	public class Session {
		public int ID { get; set; }
		public int User { get; set; }
		public long Token { get; set; }
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
			this.Token = Utils.GetUnixTimestamp();
		}

		/// <summary>
		/// Constructor for deserializing database rows into Session objects
		/// </summary>
		public Session(long ID, long User, string SessionID, long Token, long RememberMe) {
			this.ID = (int)ID;
			this.User = (int)User;
			this.Token = Token;
			this.SessionID = SessionID;
			this.RememberMe = ((int)RememberMe == 1) ? true : false;
		}

		/// <summary>
		/// Renews the token
		/// </summary>
		public void Renew(SQLiteConnection Connection) {
			this.Token = Utils.GetUnixTimestamp();
			Connection.Update<Session>(this);
		}

		/// <summary>
		/// Get the amount of seconds remaining until this session expires.
		/// The number will be negative if the session has already expired.
		/// </summary>
		/// <returns></returns>
		public long GetRemainingTime() => GetRemainingTime(this.Token, this.RememberMe);
		/// <summary>
		/// Get the amount of seconds remaining until this session expires.
		/// The number will be negative if the session has already expired.
		/// </summary>
		public static long GetRemainingTime(long Token, bool RememberMe) {
			long Timeout = RememberMe
				? (long)Config.GetValue("AuthenticationSettings.SessionTimeoutLong")
				: (long)Config.GetValue("AuthenticationSettings.SessionTimeoutShort");
			long TokenAge = Utils.GetUnixTimestamp() - Token;
			return Timeout - TokenAge;
		}

		/// <summary>
		/// Gets a user session. If the session doesn't exist or is out of date, null will be returned.
		/// </summary>
		/// <param name="Connection"></param>
		/// <returns></returns>
		public static Session GetUserSession(SQLiteConnection Connection, string SessionID) {
			//Get the session
			Session s = Connection.QueryFirstOrDefault<Session>("SELECT * FROM Sessions WHERE SessionID = @SessionID", new { SessionID });
			if (s == null) {
				return null;
			}

			//Check if this session is still valid. If it isn't, delete it and return null.
			if (GetRemainingTime(s.Token, s.RememberMe) < 0) {
				Connection.Delete(s);
				return null;
			} else {
				return s;
			}
		}
	}
}
