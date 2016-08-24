using SQLite;
using SQLiteNetExtensions.Attributes;

namespace mPassword
{
	public class BankAccount : IModel
	{

		[PrimaryKey, AutoIncrement]
		public int ID { get; set; }

		[MaxLength(100), NotNull]
		public string Name { get; set; }

		[MaxLength(30), NotNull]
		public string AccountNumber { get; set; }

		[MaxLength(20), NotNull]
		public string AtmPassword { get; set; }

		[MaxLength(30)]
		public string EBUserName { get; set; }

		[MaxLength(20)]
		public string EBPassword { get; set; }

		public string SecurityQuestion { get; set; }

		public string SecurityAnswer { get; set; }

		public int PasswordDuration { get; set; }

		public string UpdatedDate { get; set; }

		public string Note { get; set; }

		[ForeignKey(typeof(User))]
		public int UserID { get; set; }
	}
}

