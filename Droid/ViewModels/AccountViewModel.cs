using System;
using System.Globalization;

namespace mPassword
{
	public class AccountViewModel
	{
		public int accountId;
		public string accountName;
		public bool isExpiredWarning;
		public string expiredDate;

		public AccountViewModel()
		{
		}

		public AccountViewModel(BankAccount bankAccount)
		{
			accountId = bankAccount.ID;
			accountName = bankAccount.Name;
			isExpiredWarning = IsExpiredWarning(bankAccount.UpdatedDate, bankAccount.PasswordDuration);
			expiredDate = CalculateExpiredDate(bankAccount.UpdatedDate, bankAccount.PasswordDuration);
		}

		public AccountViewModel(ComputerAccount computerAccount)
		{
			accountId = computerAccount.ID;
			accountName = computerAccount.Name;
			isExpiredWarning = IsExpiredWarning(computerAccount.UpdatedDate, computerAccount.PasswordDuration);
			expiredDate = CalculateExpiredDate(computerAccount.UpdatedDate, computerAccount.PasswordDuration);
		}

		public AccountViewModel(WebAccount webAccount)
		{
			accountId = webAccount.ID;
			accountName = webAccount.Name;
			isExpiredWarning = false;
			expiredDate = string.Empty;
		}

		public AccountViewModel(EmailAccount emailAccount)
		{
			accountId = emailAccount.ID;
			accountName = emailAccount.Name;
			isExpiredWarning = false;
			expiredDate = string.Empty;
		}

		public bool IsExpiredWarning(string strUpdatedDate, int duration)
		{
			if (string.IsNullOrWhiteSpace(strUpdatedDate))
			{
				return false;
			}

			DateTime updatedDate = DateTime.ParseExact(strUpdatedDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
			DateTime warningExpiredDate = updatedDate.AddDays(duration - 5);

			if (warningExpiredDate > DateTime.Today)
			{
				return false;
			}
			return true;
		}

		public string CalculateExpiredDate(string strUpdatedDate, int duration)
		{
			if (string.IsNullOrWhiteSpace(strUpdatedDate))
			{
				return strUpdatedDate;
			}

			DateTime updatedDate = DateTime.ParseExact(strUpdatedDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
			return updatedDate.AddDays(duration).ToString("MM/dd/yyyy");
		}

	}
}