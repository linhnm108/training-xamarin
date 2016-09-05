using System.Collections.Generic;

namespace mPassword.Shared
{
	public static class ComputerAccountManager
	{
		public static ComputerAccount GetComputerAccount(int id)
		{
			return ComputerAccountRepository.GetComputerAccount(id);
		}

		public static IList<ComputerAccount> GetComputerAccounts()
		{
			return new List<ComputerAccount>(ComputerAccountRepository.GetComputerAccounts());
		}

		public static int SaveComputerAccount(ComputerAccount computerAccount)
		{
			return ComputerAccountRepository.SaveComputerAccount(computerAccount);
		}

		public static int DeleteComputerAccount(int id)
		{
			return ComputerAccountRepository.DeleteComputerAccount(id);
		}

		public static IList<ComputerAccount> GetComputerAccountsByUserId(int userId)
		{
			return new List<ComputerAccount>(ComputerAccountRepository.GetComputerAccountsByUserId(userId));
		}
	}
}