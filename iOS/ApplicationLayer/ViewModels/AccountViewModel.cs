using System;
using System.Globalization;

namespace mPassword.iOS
{
	public class AccountViewModel
	{
		public int accountId;
		public string accountName;
		public string accountType;
		public bool isExpiredWarning;

		public AccountViewModel(BankAccount bankAccount)
		{
			accountId = bankAccount.ID;
			accountName = bankAccount.Name;
			accountType = "BankAcc";

			DateTime updatedDate = DateTime.ParseExact(bankAccount.UpdatedDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
			DateTime expiredDate = updatedDate.AddDays(bankAccount.PasswordDuration);

			long diffDays = (expiredDate - DateTime.Today).Ticks;

			if (diffDays > 5)
			{
				isExpiredWarning = false;
			}
			else
			{
				isExpiredWarning = true;	
			}
		}

		public AccountViewModel(ComputerAccount computerAccount)
		{
			accountId = computerAccount.ID;
			accountName = computerAccount.Name;
			accountType = "ComputerAcc";

			DateTime updatedDate = DateTime.ParseExact(computerAccount.UpdatedDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
			DateTime expiredDate = updatedDate.AddDays(computerAccount.PasswordDuration);

			long diffDays = (expiredDate - DateTime.Today).Ticks;

			if (diffDays > 5)
			{
				isExpiredWarning = false;
			}
			else
			{
				isExpiredWarning = true;
			}
		}

		public AccountViewModel(WebAccount webAccount)
		{
			accountId = webAccount.ID;
			accountName = webAccount.Name;
			accountType = "WebAcc";
			isExpiredWarning = false;
		}

		public AccountViewModel(EmailAccount emailAccount)
		{
			accountId = emailAccount.ID;
			accountName = emailAccount.Name;
			accountType = "EmailAcc";
			isExpiredWarning = false;
		}
	}
}