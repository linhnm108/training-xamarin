using SQLite;

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
	}
}