	using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SQLite;

namespace mPassword
{
	public class SQLiteDatabase : SQLiteConnection
	{
		static readonly object locker = new object();

		public SQLiteDatabase(string path) : base(path)
		{
			// create the tables
			CreateTable<Task>();
		}

		public IEnumerable<T> GetItems<T>() where T : IModel, new()
		{
			lock (locker)
			{
				return (from i in Table<T>() select i).ToList();
			}
		}

		public T GetItem<T>(int id) where T : IModel, new()
		{
			lock (locker)
			{
				return Table<T>().FirstOrDefault(x => x.ID == id);
			}
		}

		public int SaveItem<T>(T item) where T : IModel
		{
			lock (locker)
			{
				if (item.ID != 0)
				{
					Update(item);
					return item.ID;
				}
				return Insert(item);
			}
		}

		public int DeleteItem<T>(int id) where T : IModel, new()
		{
			lock (locker)
			{
				return Delete(new T { ID = id });
			}
		}
	}
}