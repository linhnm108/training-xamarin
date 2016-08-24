using System;
using System.Collections.Generic;
using System.IO;

namespace mPassword
{
	public class ComputerAccountRepository
	{
		readonly SQLiteDatabase database;

		protected static string dbLocation;

		protected static ComputerAccountRepository computerAccountRepository;

		static ComputerAccountRepository ()
		{
			computerAccountRepository = new ComputerAccountRepository();
		}

		public ComputerAccountRepository()
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

		public static ComputerAccount GetComputerAccount(int id)
		{
			return computerAccountRepository.database.GetItem<ComputerAccount>(id);
		}
		
		public static IEnumerable<ComputerAccount> GetComputerAccounts ()
		{
			return computerAccountRepository.database.GetItems<ComputerAccount>();
		}
		
		public static int SaveComputerAccount (ComputerAccount computerAccount)
		{
			return computerAccountRepository.database.SaveItem(computerAccount);
		}

		public static int DeleteComputerAccount(int id)
		{
			return computerAccountRepository.database.DeleteItem<ComputerAccount>(id);
		}
	}
}