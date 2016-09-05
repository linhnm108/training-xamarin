using System.Collections.Generic;

namespace mPassword.Shared
{
	public static class BankAccountManager
	{
		public static BankAccount GetBankAccount(int id)
		{
			return BankAccountRepository.GetBankAccount(id);
		}

		public static IList<BankAccount> GetBankAccounts()
		{
			return new List<BankAccount>(BankAccountRepository.GetBankAccounts());
		}

		public static IList<BankAccount> GetBankAccountsByUserId(int userId)
		{
			return new List<BankAccount>(BankAccountRepository.GetBankAccountsByUserId(userId));
		}

		public static int SaveBankAccount(BankAccount bankAccount)
		{
			return BankAccountRepository.SaveBankAccount(bankAccount);
		}

		public static int DeleteBankAccount(int id)
		{
			return BankAccountRepository.DeleteBankAccount(id);
		}
	}
}