using System;
using UIKit;
using mPassword.Shared;

namespace mPassword.iOS
{
    public partial class EditUserViewController : UIViewController
    {
		AppDelegate appDelegate;
		User loginUser;



		public EditUserViewController (IntPtr handle) : base (handle)
        {
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
			loginUser = appDelegate.LoginUser;

			userName.Text = loginUser.UserName;
			userName.UserInteractionEnabled = false;
		}

		partial void BtnSave_TouchUpInside(UIButton sender)
		{
			var inputUserName = userName.Text.Trim();
			var inputOldPass = oldPassword.Text.Trim();
			var inputNewPass = newPassword.Text.Trim();
			var inputConfirmNewPass = confirmNewPassword.Text.Trim();

			if (loginUser.UserName != inputUserName || loginUser.Password != SecurityUtil.HashSHA256(inputOldPass))
			{
				ShowAlertMessage(Constants.ERROR_EDIT_USER, Constants.ERROR_OLD_PASSWORD_INCORRECT);
			}
			else if (string.IsNullOrEmpty(inputNewPass))
			{
				ShowAlertMessage(Constants.ERROR_EDIT_USER, Constants.ERROR_REQUIRED.Replace("{field_name}", "New Password"));
			}
			else if (!ValicationUtil.IsValidPassword(inputNewPass))
			{
				ShowAlertMessage(Constants.ERROR_EDIT_USER, Constants.ERROR_PASSWORD_INVALID);
			}
			else if (inputNewPass != inputConfirmNewPass)
			{
				ShowAlertMessage(Constants.ERROR_EDIT_USER, Constants.ERROR_PASSWORD_NOT_MATCH);
			}
			else
			{
				loginUser.UserName = inputUserName;
				loginUser.Password = SecurityUtil.HashSHA256(inputNewPass);
				UserManager.SaveUser(loginUser);

				// Go back to prior screen
				NavigationController.PopViewController(true);
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