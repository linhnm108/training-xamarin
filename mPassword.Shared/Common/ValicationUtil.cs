using System.Text.RegularExpressions;

namespace mPassword.Shared
{
	public static class ValicationUtil
	{
		public static bool IsOverMaxLength(string strNeedCheck, int maxLength)
		{
			if (strNeedCheck.Length > maxLength)
			{
				return true;
			}
			return false;
		}

		public static bool IsValidPassword(string password)
		{
			var regex = new Regex(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,}$");
			var match = regex.Match(password);
			if (match.Success)
			{
				return true;
			}
			return false;
		}

	}
}