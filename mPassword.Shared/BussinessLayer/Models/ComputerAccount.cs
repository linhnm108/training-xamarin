using SQLite;
using SQLiteNetExtensions.Attributes;

namespace mPassword
{
	public class ComputerAccount : IModel
	{

		[PrimaryKey, AutoIncrement]
		public int ID { get; set; }

		[MaxLength(50), NotNull]
		public string Name { get; set; }

		[MaxLength(30), NotNull]
		public string Password { get; set; }

		[MaxLength(20), NotNull]
		public string UserName { get; set; }

		public int PasswordDuration { get; set; }

		public string UpdatedDate { get; set; }

		[ForeignKey(typeof(User))]
		public int UserID { get; set; }
	}
}

