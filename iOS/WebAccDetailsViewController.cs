using System;
using UIKit;
using mPassword.Shared;

namespace mPassword.iOS
{
    public partial class WebAccDetailsViewController : UIViewController
    {
		UIBarButtonItem saveBarButton;
		AccountViewModel selectedAccViewModel;

		public WebAccount SelectedAccount { set; get; }

		public WebAccDetailsViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			// Create save button
			saveBarButton = new UIBarButtonItem(UIBarButtonSystemItem.Save);
			saveBarButton.Clicked += SaveWebAccount;
			NavigationItem.RightBarButtonItem = saveBarButton;

			OnUpdateDetails();
		}

		partial void BtnSave_TouchUpInside(UIButton sender)
		{
			SaveWebAccount(sender, null);
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

				SetNewAccountInfo();

				// Go back to prior scree
				NavigationController.PopViewController(true);
			}
		}

		void SetNewAccountInfo()
		{
			SelectedAccViewModel.accountId = SelectedAccount.ID;
			SelectedAccViewModel.accountName = SelectedAccount.Name;
			SelectedAccViewModel.accountType = "WebAcc";
			SelectedAccViewModel.isExpiredWarning = false;
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
			SelectedAccount = new WebAccount();

			if (!IsViewLoaded)
			{
				return;
			}

			if (selectedAccViewModel != null && selectedAccViewModel.accountId != 0)
			{
				// Get Selected Email Account
				SelectedAccount = WebAccountManager.GetWebAccount(selectedAccViewModel.accountId);

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