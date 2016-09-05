using System.Collections.Generic;

namespace mPassword.Shared
{
	public static class WebAccountManager
	{
		public static WebAccount GetWebAccount(int id)
		{
			return WebAccountRepository.GetWebAccount(id);
		}

		public static IList<WebAccount> GetWebAccounts()
		{
			return new List<WebAccount>(WebAccountRepository.GetWebAccounts());
		}

		public static int SaveWebAccount(WebAccount webAccount)
		{
			return WebAccountRepository.SaveWebAccount(webAccount);
		}

		public static int DeleteWebAccount(int id)
		{
			return WebAccountRepository.DeleteWebAccount(id);
		}

		public static IList<WebAccount> GetWebAccountsByUserId(int userId)
		{
			return new List<WebAccount>(WebAccountRepository.GetWebAccountsByUserId(userId));
		}
	}
}