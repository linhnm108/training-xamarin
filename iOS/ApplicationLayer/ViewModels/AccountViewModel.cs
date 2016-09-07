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

		public AccountViewModel()
		{
		}

		public AccountViewModel(string accountType)
		{
			this.accountType = accountType;
		}

		public AccountViewModel(BankAccount bankAccount)
		{
			accountId = bankAccount.ID;
			accountName = bankAccount.Name;
			accountType = "BankAcc";
			isExpiredWarning = IsExpiredWarning(bankAccount.UpdatedDate, bankAccount.PasswordDuration);
		}

		public AccountViewModel(ComputerAccount computerAccount)
		{
			accountId = computerAccount.ID;
			accountName = computerAccount.Name;
			accountType = "ComputerAcc";
			isExpiredWarning = IsExpiredWarning(computerAccount.UpdatedDate, computerAccount.PasswordDuration);
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

		public bool IsExpiredWarning(string strUpdatedDate, int duration)
		{
			if (string.IsNullOrWhiteSpace(strUpdatedDate))
			{
				return false;
			}

			DateTime updatedDate = DateTime.ParseExact(strUpdatedDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
			DateTime expiredDate = updatedDate.AddDays(duration);

			long diffDays = (expiredDate - DateTime.Today).Ticks;

			if (diffDays > 5)
			{
				return false;
			}
			return true;
		}
	}
}