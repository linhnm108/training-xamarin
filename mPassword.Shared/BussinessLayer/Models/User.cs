using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace mPassword
{
	public class User : IModel
	{
		[PrimaryKey, AutoIncrement]
		public int ID { get; set;}

		[MaxLength(12), NotNull]
		public string UserName { get; set; }

		[NotNull]
		public string Password { get; set; }

		[OneToMany("UserID")]
		public List<BankAccount> BankAccounts { get; set; }

		[OneToMany("UserID")]
		public List<ComputerAccount> ComputerAccounts { get; set; }

		[OneToMany("UserID")]
		public List<EmailAccount> EmailAccounts { get; set; }

		[OneToMany("UserID")]
		public List<WebAccount> WebAccounts { get; set; }
	}
}