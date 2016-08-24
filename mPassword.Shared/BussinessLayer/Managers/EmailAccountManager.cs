using System.Collections.Generic;

namespace mPassword.Shared
{
	public static class EmailAccountManager
	{
		public static EmailAccount GetEmailAccount(int id)
		{
			return EmailAccountRepository.GetEmailAccount(id);
		}

		public static IList<EmailAccount> GetEmailAccounts()
		{
			return new List<EmailAccount>(EmailAccountRepository.GetEmailAccounts());
		}

		public static int SaveEmailAccount(EmailAccount emailAccount)
		{
			return EmailAccountRepository.SaveEmailAccount(emailAccount);
		}

		public static int DeleteEmailAccount(int id)
		{
			return EmailAccountRepository.DeleteEmailAccount(id);
		}
	}
}