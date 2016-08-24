using SQLite;
using SQLiteNetExtensions.Attributes;

namespace mPassword
{
	public class WebAccount : IModel
	{

		[PrimaryKey, AutoIncrement]
		public int ID { get; set; }

		[MaxLength(50), NotNull]
		public string Name { get; set; }

		[MaxLength(255), NotNull]
		public string URL { get; set; }

		[MaxLength(30), NotNull]
		public string UserName { get; set; }

		[MaxLength(20), NotNull]
		public string Password { get; set; }

		public string Note { get; set; }

		[ForeignKey(typeof(User))]
		public int UserID { get; set; }
	}
}