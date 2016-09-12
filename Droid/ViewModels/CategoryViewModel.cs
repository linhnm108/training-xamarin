using System.Collections.Generic;

namespace mPassword
{
	public class CategoryViewModel
	{
		public string categoryName;
		public int quantity;

		public CategoryViewModel(IList<BankAccount> accounts)
		{
			categoryName = "Bank Accounts";
			quantity = accounts.Count;
		}

		public CategoryViewModel(IList<ComputerAccount> accounts)
		{
			categoryName = "Computer Accounts";
			quantity = accounts.Count;
		}

		public CategoryViewModel(IList<EmailAccount> accounts)
		{
			categoryName = "Email Accounts";
			quantity = accounts.Count;
		}

		public CategoryViewModel(IList<WebAccount> accounts)
		{
			categoryName = "Web Accounts";
			quantity = accounts.Count;
		}
	}
}