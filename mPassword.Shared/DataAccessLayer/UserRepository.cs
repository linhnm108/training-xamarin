using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace mPassword
{
	public class UserRepository
	{
		static readonly object locker = new object();

		readonly SQLiteDatabase database;

		protected static string dbLocation;

		protected static UserRepository userRepository;

		static UserRepository ()
		{
			userRepository = new UserRepository();
		}

		protected UserRepository()
		{
			// set the db location
			dbLocation = DatabaseFilePath;
			
			// instantiate the database	
			database = new SQLiteDatabase(dbLocation);
		}

		public static string DatabaseFilePath {
			get { 
				var sqliteFilename = "MPasswordDB.db3";


#if __ANDROID__
				// Just use whatever directory SpecialFolder.Personal returns
	            string libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); ;
#else
				// we need to put in /Library/ on iOS5.1 to meet Apple's iCloud terms
				// (they don't want non-user-generated data in Documents)
				string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
				string libraryPath = Path.Combine (documentsPath, "../Library/"); // Library folder
#endif
				var path = Path.Combine (libraryPath, sqliteFilename);

				return path;	
			}
		}

		public static User GetUser(int id)
		{
			return userRepository.database.GetItem<User>(id);
		}
		
		public static IEnumerable<User> GetUsers ()
		{
			return userRepository.database.GetItems<User>();
		}
		
		public static int SaveUser (User user)
		{
			return userRepository.database.SaveItem(user);
		}

		public static int DeleteUser(int id)
		{
			return userRepository.database.DeleteItem<User>(id);
		}

		public static User GetUserByName(string userName)
		{
			lock (locker)
			{
				return userRepository.database.Table<User>().FirstOrDefault(x => x.UserName == userName);
			}
		}
	}
}