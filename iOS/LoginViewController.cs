using Foundation;
using System;
using UIKit;
using mPassword.Shared;

namespace mPassword.iOS
{
    public partial class LoginViewController : UIViewController
    {
		//Create an event when a authentication is successful
		public event EventHandler OnLoginSuccess;

		public LoginViewController (IntPtr handle) : base (handle)
        {
			
        }

		partial void Login(UIButton sender)
		{
			//Validate our Username & Password.
			//This is usually a web service call.
			if (IsAccountValid())
			{
				//We have successfully authenticated a the user,
				//Now fire our OnLoginSuccess Event.
				if (OnLoginSuccess != null)
				{
					OnLoginSuccess(sender, new EventArgs());
				}
			}
			else
			{
				ShowAlertMessage(Constants.ERROR_LOGIN, Constants.ERROR_USER_AUTHENTICATION);
			}

		}

		bool IsAccountValid()
		{
			var userName = UserNameTextView.Text.Trim();
			var password = PasswordTextView.Text.Trim();

			User user = UserManager.GetUserByName(userName);

			// User not existed.
			if (user == null)
			{
				return false;
			}

			// Check password.
			StringComparer compare = StringComparer.Ordinal;

			if (compare.Equals(user.Password, SecurityUtil.HashSHA256(password)))
			{
				return true;
			}

			return false;
		}

		void ShowAlertMessage(string errorHeader, string errorContent)
		{
			var okAlertController = UIAlertController.Create(errorHeader, errorContent, UIAlertControllerStyle.Alert);
			okAlertController.AddAction(UIAlertAction.Create(Constants.OK_BUTTON_LABEL, UIAlertActionStyle.Default, null));
			PresentViewController(okAlertController, true, null);
		}

	}
}