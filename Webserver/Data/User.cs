using Dapper;
using System;
using System.Data.SQLite;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Webserver.Data {
	public class User {
		public int ID { get; set; }
		public string Email { get; set; }
		public string PasswordHash { get; set; }
		public string Firstname { get; set; }
		public string MiddleInitial { get; set; }
		public string Lastname { get; set; }
		public string Function { get; set; }
		public string WorkPhone { get; set; }
		public string MobilePhone { get; set; }
		public DateTime Birthday { get; set; }
		public string Country { get; set; }
		public string Address { get; set; }
		public string Postcode { get; set; }

		/// <summary>
		/// Creates a new user. The new user object will not be usable in the system until its inserted into the database.
		/// </summary>
		/// <param name="Email">The user's email address</param>
		/// <param name="Password">The user's password. This will be converted into a salted hash and stored in the PasswordHash field.</param>
		public User(string Email, string Password) {
			this.Email = Email;
			this.PasswordHash = CreateHash(Password, Email);
		}

		/// <summary>
		/// Constructor for deserializing database rows into User objects
		/// </summary>
		public User(
			long ID,
			string Email,
			string PasswordHash,
			string Firstname,
			string MiddleInitial,
			string Lastname,
			string Function,
			string WorkPhone,
			string MobilePhone,
			string Birthday,
			string Country,
			string Address,
			string Postcode
		) {
			this.ID = (int)ID;
			this.Email = Email;
			this.PasswordHash = PasswordHash;
			this.Firstname = Firstname;
			this.MiddleInitial = MiddleInitial;
			this.Lastname = Lastname;
			this.Function = Function;
			this.WorkPhone = WorkPhone;
			this.MobilePhone = MobilePhone;
			this.Birthday = DateTime.Parse(Birthday);
			this.Country = Country;
			this.Address = Address;
			this.Postcode = Postcode;
		}

		/// <summary>
		/// Change this user's password. The change will not be applied the object is updated in the database.
		/// </summary>
		/// <param name="Password">The new password</param>
		public void ChangePassword(string Password) => this.PasswordHash = CreateHash(Password, Email);

		/// <summary>
		/// Get a user's permission level.
		/// </summary>
		/// <param name="Connection"></param>
		/// <param name="Department"></param>
		/// <returns></returns>
		public PermLevel GetPermissionLevel(SQLiteConnection Connection, Department Department) => GetPermissionLevel(Connection, Department.ID);
		/// <summary>
		/// Get a user's permission level.
		/// </summary>
		/// <param name="Connection"></param>
		/// <param name="Department"></param>
		/// <returns></returns>
		public PermLevel GetPermissionLevel(SQLiteConnection Connection, int Department) => (PermLevel)Math.Max(
			Connection.QueryFirstOrDefault<int>("SELECT Permission FROM Permissions WHERE User = @ID AND Department = @Department", new { this.ID, Department }),
			Connection.QueryFirstOrDefault<int>("SELECT Permission FROM Permissions WHERE USER = @ID AND Permission = 3", new { this.ID })
		);

		/// <summary>
		/// Sets a user permission level.
		/// </summary>
		/// <param name="Connection"></param>
		/// <param name="Level"></param>
		/// <param name="Department"></param>
		public void SetPermissionLevel(SQLiteConnection Connection, PermLevel Level, Department Department) => SetPermissionLevel(Connection, Level, Department.ID);
		/// <summary>
		/// Sets a user permission level.
		/// </summary>
		/// <param name="Connection"></param>
		/// <param name="Level"></param>
		/// <param name="Department"></param>
		public void SetPermissionLevel(SQLiteConnection Connection, PermLevel Level, int Department) => Connection.Execute("INSERT OR REPLACE INTO Permissions (User, Permission, Department) VALUES (@User, @Permission, @Department)", new { user = this.ID, Permission = Level, Department });


		/// ########################################### Static Methods

		/// <summary>
		/// Given a password and salt, returns a salted SHA512 hash.
		/// </summary>
		/// <param name="Password"></param>
		/// <param name="Salt"></param>
		/// <returns></returns>
		public static string CreateHash(string Password, string Salt) {
			if (string.IsNullOrEmpty(Password)) {
				throw new ArgumentException("message", nameof(Password));
			}

			byte[] PassBytes = Encoding.UTF8.GetBytes(Password);
			byte[] SaltBytes = Encoding.UTF8.GetBytes(Salt);
			using SHA512 sha = SHA512.Create();
			return String.Concat(sha
				.ComputeHash(PassBytes.Concat(SaltBytes).ToArray())
				.Select(item => item.ToString("x2")));
		}

		/// <summary>
		/// Retrieve a user using their email address.
		/// </summary>
		/// <param name="Connection">The database connection</param>
		/// <param name="Email">The user's email address</param>
		/// <returns></returns>
		public static User GetUserByEmail(SQLiteConnection Connection, string Email) {
			return Connection.QueryFirstOrDefault<User>("SELECT * FROM Users WHERE Email = @Email", new { Email });
		}
	}
}
