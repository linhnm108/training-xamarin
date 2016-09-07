using System;
using UIKit;
using mPassword.Shared;

namespace mPassword.iOS
{
    public partial class SignUpViewController : UIViewController
    {
        public SignUpViewController (IntPtr handle) : base (handle)
        {
			
        }

		partial void Cancel(UIButton sender)
		{
			DismissViewController(true, null);
		}

		partial void SignUp(UIButton sender)
		{
			var userName = UserNameTextView.Text.Trim();
			var password = PasswordTextView.Text.Trim();
			var confirmPassword = ConfirmPasswordTextView.Text.Trim();

			if (string.IsNullOrEmpty(userName))
			{
				ShowAlertMessage(Constants.ERROR_SIGNUP, Constants.ERROR_REQUIRED.Replace("{field_name}", "Username"));
			}
			else if (string.IsNullOrEmpty(password))
			{
				ShowAlertMessage(Constants.ERROR_SIGNUP, Constants.ERROR_REQUIRED.Replace("{field_name}", "Password"));
			}
			else if (ValicationUtil.IsOverMaxLength(userName, 12))
			{
				ShowAlertMessage(Constants.ERROR_SIGNUP, Constants.ERROR_OVER_MAX_LENGTH.Replace("{field_name}", "Username").Replace("{max_length}","12"));
			}
			else if (!ValicationUtil.IsValidPassword(password))
			{
				ShowAlertMessage(Constants.ERROR_SIGNUP, Constants.ERROR_PASSWORD_INVALID);
			}
			else if (password != confirmPassword)
			{
				ShowAlertMessage(Constants.ERROR_SIGNUP, Constants.ERROR_PASSWORD_NOT_MATCH);
			}
			else if (UserManager.GetUserByName(userName) != null)
			{
				ShowAlertMessage(Constants.ERROR_SIGNUP, Constants.ERROR_USER_EXISTED);
			}
			else
			{
				// Save user info to database
				var user = new User();
				user.UserName = userName;
				user.Password = SecurityUtil.HashSHA256(password);
				UserManager.SaveUser(user);

				//Create an instance of our AppDelegate
				var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;

				//Get an instance of our MainStoryboard.storyboard
				var mainStoryboard = appDelegate.MainStoryboard;

				//Get an instance of our MainTabBarViewController
				//var mainTabBarViewController = appDelegate.GetViewController(mainStoryboard, "MainTabBarViewController");

				//Get an instance of our MainScreenr
				var mainScreenViewController = appDelegate.GetViewController(mainStoryboard, "MainScreenViewController");

				//Set the MainTabBarController as our RootViewController
				appDelegate.SetRootViewController(mainScreenViewController, true);
				appDelegate.LoginUser = UserManager.GetUserByName(userName);
			}
		}

		void ShowAlertMessage(string errorHeader, string errorContent)
		{
			var okAlertController = UIAlertController.Create(errorHeader, errorContent, UIAlertControllerStyle.Alert);
			okAlertController.AddAction(UIAlertAction.Create(Constants.OK_BUTTON_LABEL, UIAlertActionStyle.Default, null));
			PresentViewController(okAlertController, true, null);
		}
	}
}