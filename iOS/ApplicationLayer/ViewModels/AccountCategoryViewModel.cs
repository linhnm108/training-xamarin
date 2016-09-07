using System.Collections.Generic;

namespace mPassword.iOS
{
	public class AccountCategoryModel
	{
		public string categoryName;
		public List<AccountViewModel> accounts;
		public bool collapsed;
		public int quantity;

		public AccountCategoryModel(IList<BankAccount> accounts, bool collapsed)
		{
			categoryName = "Bank Accounts";
			quantity = accounts.Count;
			this.collapsed = collapsed;
			this.accounts = new List<AccountViewModel>();

			foreach (BankAccount bankAcc in accounts)
			{
				this.accounts.Add(new AccountViewModel(bankAcc));
			}
		}

		public AccountCategoryModel(IList<ComputerAccount> accounts, bool collapsed)
		{
			categoryName = "Computer Accounts";
			quantity = accounts.Count;
			this.collapsed = collapsed;
			this.accounts = new List<AccountViewModel>();

			foreach (ComputerAccount computerAcc in accounts)
			{
				this.accounts.Add(new AccountViewModel(computerAcc));
			}
		}

		public AccountCategoryModel(IList<EmailAccount> accounts, bool collapsed)
		{
			categoryName = "Email Accounts";
			quantity = accounts.Count;
			this.collapsed = collapsed;

			this.accounts = new List<AccountViewModel>();

			foreach (EmailAccount emailAcc in accounts)
			{
				this.accounts.Add(new AccountViewModel(emailAcc));
			}
		}

		public AccountCategoryModel(IList<WebAccount> accounts, bool collapsed)
		{
			categoryName = "Web Accounts";
			quantity = accounts.Count;
			this.collapsed = collapsed;

			this.accounts = new List<AccountViewModel>();

			foreach (WebAccount webAcc in accounts)
			{
				this.accounts.Add(new AccountViewModel(webAcc));
			}
		}
	}
}