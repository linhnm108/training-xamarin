using System;
using System.Collections.Generic;
using System.IO;

namespace mPassword
{
	public class EmailAccountRepository
	{
		readonly SQLiteDatabase database;

		protected static string dbLocation;

		protected static EmailAccountRepository emailAccountRepository;

		static EmailAccountRepository ()
		{
			emailAccountRepository = new EmailAccountRepository();
		}

		public EmailAccountRepository()
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

		public static EmailAccount GetEmailAccount(int id)
		{
			return emailAccountRepository.database.GetItem<EmailAccount>(id);
		}
		
		public static IEnumerable<EmailAccount> GetEmailAccounts ()
		{
			return emailAccountRepository.database.GetItems<EmailAccount>();
		}
		
		public static int SaveEmailAccount (User user)
		{
			return emailAccountRepository.database.SaveItem(user);
		}

		public static int DeleteEmailAccount(int id)
		{
			return emailAccountRepository.database.DeleteItem<EmailAccount>(id);
		}
	}
}