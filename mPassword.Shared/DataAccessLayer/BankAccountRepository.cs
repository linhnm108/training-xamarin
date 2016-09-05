using System;
using System.Collections.Generic;
using System.IO;

namespace mPassword
{
	public class BankAccountRepository
	{
		static readonly object locker = new object();

		readonly SQLiteDatabase database;

		protected static string dbLocation;

		protected static BankAccountRepository bankAccountRepository;

		static BankAccountRepository ()
		{
			bankAccountRepository = new BankAccountRepository();
		}

		public BankAccountRepository()
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

		public static BankAccount GetBankAccount(int id)
		{
			return bankAccountRepository.database.GetItem<BankAccount>(id);
		}
		
		public static IEnumerable<BankAccount> GetBankAccounts ()
		{
			return bankAccountRepository.database.GetItems<BankAccount>();
		}

		public static IEnumerable<BankAccount> GetBankAccountsByUserId(int userId)
		{
			lock (locker)
			{
				return bankAccountRepository.database.Table<BankAccount>().Where(x => x.UserID == userId);
			}
		}
		
		public static int SaveBankAccount (BankAccount bankAccount)
		{
			return bankAccountRepository.database.SaveItem(bankAccount);
		}

		public static int DeleteBankAccount(int id)
		{
			return bankAccountRepository.database.DeleteItem<BankAccount>(id);
		}
	}
}