using Foundation;
using System;
using UIKit;
using mPassword.Shared;

namespace mPassword.iOS
{
    public partial class WebAccDetailsViewController : UIViewController
    {
		UIBarButtonItem saveButton;
		WebAccount selectedAccount;

		public WebAccDetailsViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			// Create save button
			saveButton = new UIBarButtonItem(UIBarButtonSystemItem.Save);
			saveButton.Clicked += SaveWebAccount;
			NavigationItem.RightBarButtonItem = saveButton;

			OnUpdateDetails();
		}

		void SaveWebAccount(object sender, EventArgs e)
		{
			// Save the expense back to the data store.
			if (SelectedAccount != null && validateWebAccount())
			{
				SelectedAccount.Name = accountName.Text;
				SelectedAccount.URL = url.Text;
				SelectedAccount.UserName = userName.Text;
				SelectedAccount.Password = SecurityUtil.Encrypt(password.Text);
				SelectedAccount.Note = note.Text;

				//Create an instance of our AppDelegate
				var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
				SelectedAccount.UserID = appDelegate.LoginUser.ID;

				WebAccountManager.SaveWebAccount(SelectedAccount);

				// Go back to prior screen
				NavigationController.PopViewController(true);
			}
		}

		bool validateWebAccount()
		{
			bool isValid = false;

			if (string.IsNullOrWhiteSpace(accountName.Text))
			{
				ShowAlertMessage(Constants.ERROR_DATA, Constants.ERROR_REQUIRED.Replace("{field_name}", "Account name"));
			}
			else if (ValicationUtil.IsOverMaxLength(accountName.Text, 50))
			{
				ShowAlertMessage(Constants.ERROR_DATA, Constants.ERROR_OVER_MAX_LENGTH.Replace("{field_name}", "Account name").Replace("{max_length}", "100"));
			}
			else if (string.IsNullOrWhiteSpace(url.Text))
			{
				ShowAlertMessage(Constants.ERROR_DATA, Constants.ERROR_REQUIRED.Replace("{field_name}", "URL"));
			}
			else if (ValicationUtil.IsOverMaxLength(url.Text, 255))
			{
				ShowAlertMessage(Constants.ERROR_DATA, Constants.ERROR_OVER_MAX_LENGTH.Replace("{field_name}", "URL").Replace("{max_length}", "255"));
			}
			else if (string.IsNullOrWhiteSpace(userName.Text))
			{
				ShowAlertMessage(Constants.ERROR_DATA, Constants.ERROR_REQUIRED.Replace("{field_name}", "Username"));
			}
			else if (ValicationUtil.IsOverMaxLength(userName.Text, 30))
			{
				ShowAlertMessage(Constants.ERROR_DATA, Constants.ERROR_OVER_MAX_LENGTH.Replace("{field_name}", "Username").Replace("{max_length}", "30"));
			}
			else if (string.IsNullOrWhiteSpace(password.Text))
			{
				ShowAlertMessage(Constants.ERROR_DATA, Constants.ERROR_REQUIRED.Replace("{field_name}", "Password"));
			}
			else if (ValicationUtil.IsOverMaxLength(password.Text, 20))
			{
				ShowAlertMessage(Constants.ERROR_DATA, Constants.ERROR_OVER_MAX_LENGTH.Replace("{field_name}", "Password").Replace("{max_length}", "20"));
			}
			else
			{
				isValid = true;
			}
			return isValid;
		}

		void ShowAlertMessage(string errorHeader, string errorContent)
		{
			var okAlertController = UIAlertController.Create(errorHeader, errorContent, UIAlertControllerStyle.Alert);
			okAlertController.AddAction(UIAlertAction.Create(Constants.OK_BUTTON_LABEL, UIAlertActionStyle.Default, null));
			PresentViewController(okAlertController, true, null);
		}

		public WebAccount SelectedAccount
		{
			get
			{
				return selectedAccount;
			}
			set
			{
				if (selectedAccount != value)
				{
					selectedAccount = value;
					OnUpdateDetails();
				}
			}
		}

		void OnUpdateDetails()
		{
			if (!IsViewLoaded)
			{
				return;
			}

			if (SelectedAccount != null && SelectedAccount.ID != 0)
			{
				accountName.Text = SelectedAccount.Name;
				url.Text = SelectedAccount.URL;
				userName.Text = SelectedAccount.UserName;
				password.Text = SecurityUtil.Decrypt(SelectedAccount.Password);
				note.Text = SelectedAccount.Note;
			}
			else
			{
				accountName.Text = string.Empty;
				url.Text = string.Empty;
				userName.Text = string.Empty;
				password.Text = string.Empty;
				note.Text = string.Empty;
			}
		}
    }
}