using System;
using System.Collections.Generic;
using System.IO;

namespace mPassword
{
	public class WebAccountRepository
	{
		static readonly object locker = new object();

		readonly SQLiteDatabase database;

		protected static string dbLocation;

		protected static WebAccountRepository webAccountRepository;

		static WebAccountRepository ()
		{
			webAccountRepository = new WebAccountRepository();
		}

		public WebAccountRepository()
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
	            string libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
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

		public static WebAccount GetWebAccount(int id)
		{
			return webAccountRepository.database.GetItem<WebAccount>(id);
		}
		
		public static IEnumerable<WebAccount> GetWebAccounts ()
		{
			return webAccountRepository.database.GetItems<WebAccount>();
		}
		
		public static int SaveWebAccount (WebAccount webAccount)
		{
			return webAccountRepository.database.SaveItem(webAccount);
		}

		public static int DeleteWebAccount(int id)
		{
			return webAccountRepository.database.DeleteItem<WebAccount>(id);
		}

		public static IEnumerable<WebAccount> GetWebAccountsByUserId(int userId)
		{
			lock (locker)
			{
				return webAccountRepository.database.Table<WebAccount>().Where(x => x.UserID == userId);
			}
		}
	}
}