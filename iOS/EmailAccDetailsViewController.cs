using System;
using UIKit;
using mPassword.Shared;

namespace mPassword.iOS
{
    public partial class EmailAccDetailsViewController : UIViewController
    {
		UIBarButtonItem saveButton;
		AccountViewModel selectedAccViewModel;

		public EmailAccount SelectedAccount { set; get; }

		public EmailAccDetailsViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			// Create save button
			saveButton = new UIBarButtonItem(UIBarButtonSystemItem.Save);
			saveButton.Clicked += SaveEmailAccount;
			NavigationItem.RightBarButtonItem = saveButton;

			OnUpdateDetails();
		}

		void SaveEmailAccount(object sender, EventArgs e)
		{
			// Save the expense back to the data store.
			if (SelectedAccount != null && validateEmailAccount())
			{
				SelectedAccount.Name = accountName.Text;
				SelectedAccount.UserName = userName.Text;
				SelectedAccount.Password = SecurityUtil.Encrypt(password.Text);

				//Create an instance of our AppDelegate
				var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
				SelectedAccount.UserID = appDelegate.LoginUser.ID;

				EmailAccountManager.SaveEmailAccount(SelectedAccount);

				SetNewAccountInfo();

				// Go back to prior screen
				NavigationController.PopViewController(true);
			}
		}

		void SetNewAccountInfo()
		{
			SelectedAccViewModel.accountId = SelectedAccount.ID;
			SelectedAccViewModel.accountName = SelectedAccount.Name;
			SelectedAccViewModel.accountType = "EmailAcc";
			SelectedAccViewModel.isExpiredWarning = false;
		}

		bool validateEmailAccount()
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

		public AccountViewModel SelectedAccViewModel
		{
			get
			{
				return selectedAccViewModel;
			}
			set
			{
				if (selectedAccViewModel != value)
				{
					selectedAccViewModel = value;
					OnUpdateDetails();
				}
			}
		}

		void OnUpdateDetails()
		{
			SelectedAccount = new EmailAccount();

			if (!IsViewLoaded)
			{
				return;
			}

			if (selectedAccViewModel != null && selectedAccViewModel.accountId != 0)
			{
				// Get Selected Email Account
				SelectedAccount = EmailAccountManager.GetEmailAccount(selectedAccViewModel.accountId);
				
				accountName.Text = SelectedAccount.Name;
				userName.Text = SelectedAccount.UserName;
				password.Text = SecurityUtil.Decrypt(SelectedAccount.Password);
			}
			else
			{
				accountName.Text = string.Empty;
				userName.Text = string.Empty;
				password.Text = string.Empty;
			}
		}
    }
}