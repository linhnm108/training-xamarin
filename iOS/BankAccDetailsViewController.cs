using System;
using mPassword.Shared;
using UIKit;

namespace mPassword.iOS
{
    public partial class BankAccDetailsViewController : UIViewController
    {
		UIBarButtonItem saveButton;
		AccountViewModel selectedAccViewModel;

		public BankAccount SelectedAccount { set; get; }

		public BankAccDetailsViewController (IntPtr handle) : base (handle)
        {
        }

		public override void ViewDidLoad()
		{
			OnUpdateDetails();

			// Create Password Expired Durtion Picker View
			var expiredDurationPickerModel = new PasswordExpiredDurationModelView();
			var pickerView = new UIPickerView();
			pickerView.Model = expiredDurationPickerModel;
			pickerView.ShowSelectionIndicator = true;

			expiredDurationPickerModel.ValueChanged += (sender, e) =>
			{
				passwordExpiredDuration.Text = expiredDurationPickerModel.SelectedItem;
				if (string.Equals(passwordExpiredDuration.Text, "---Select---"))
				{
					passwordExpiredDuration.Text = string.Empty;
					updatedDate.Text = string.Empty;
					updatedDate.Hidden = true;
				}
				else
				{
					updatedDate.Hidden = false;
					updatedDate.Text = string.Format("{0:MM/dd/yyyy}", DateTime.Today);
				}

			};

 			// Use a Toolbar to add a "Done" button to dismiss the picke..
			var toolbar = new UIToolbar();
			toolbar.BarStyle = UIBarStyle.Default;
			toolbar.Translucent = true;
			toolbar.SizeToFit();
			var flexibleSpace = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace, null);

			var doneButton = new UIBarButtonItem("Done", UIBarButtonItemStyle.Done, (s, e) =>
			{
				updatedDate.ResignFirstResponder();
				passwordExpiredDuration.ResignFirstResponder();
			});
			toolbar.SetItems(new UIBarButtonItem[] { flexibleSpace,doneButton }, true);

			// Tell the textbox to use the picker for input
			passwordExpiredDuration.InputView = pickerView;

			// Display the toolbar over the pickers
			passwordExpiredDuration.InputAccessoryView = toolbar;

			// Create save button
			saveButton = new UIBarButtonItem(UIBarButtonSystemItem.Save);
			saveButton.Clicked += SaveBankAccount;
			NavigationItem.RightBarButtonItem = saveButton;

			if (SelectedAccount.PasswordDuration == 0)
			{
				updatedDate.Hidden = true;
			}
			else
			{
				updatedDate.Hidden = false;
			}

			// Create updated date picker
			var datePicker = new UIDatePicker();
			datePicker.Mode = UIDatePickerMode.Date;
			updatedDate.InputView = datePicker;
			updatedDate.InputAccessoryView = toolbar;
			datePicker.ValueChanged += (sender, e) =>
			{
				updatedDate.Text = string.Format("{0:MM/dd/yyyy}", (DateTime) ((UIDatePicker)sender).Date);
			};
		}

		void SaveBankAccount(object sender, EventArgs e)
		{
			// Save the expense back to the data store.
			if (SelectedAccount != null && validateBankAccount())
			{
				SelectedAccount.Name = accountName.Text;
				SelectedAccount.AccountNumber = accountNumber.Text;
				SelectedAccount.AtmPassword = SecurityUtil.Encrypt(atmPassword.Text);
				SelectedAccount.EBUserName = iBankUser.Text;
				SelectedAccount.EBPassword = SecurityUtil.Encrypt(iBankPassword.Text);
				SelectedAccount.SecurityQuestion = securityQuestion.Text;
				SelectedAccount.SecurityAnswer = securityAnswer.Text;
				SelectedAccount.Note = note.Text;

				if (!string.IsNullOrWhiteSpace(passwordExpiredDuration.Text))
				{
					var duration = passwordExpiredDuration.Text.Replace(" days", string.Empty);
					SelectedAccount.PasswordDuration = int.Parse(duration);
					SelectedAccount.UpdatedDate = updatedDate.Text;
				}

				//Create an instance of our AppDelegate
				var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
				SelectedAccount.UserID = appDelegate.LoginUser.ID;

				BankAccountManager.SaveBankAccount(SelectedAccount);

				SetNewAccountInfo();

				// Go back to prior screen
				NavigationController.PopViewController(true);
			}
		}

		void SetNewAccountInfo()
		{
			SelectedAccViewModel.accountId = SelectedAccount.ID;
			SelectedAccViewModel.accountName = SelectedAccount.Name;
			SelectedAccViewModel.accountType = "BankAcc";
			SelectedAccViewModel.isExpiredWarning = SelectedAccViewModel.IsExpiredWarning(SelectedAccount.UpdatedDate, SelectedAccount.PasswordDuration);
		}

		bool validateBankAccount()
		{
			bool isValid = false;

			if (string.IsNullOrWhiteSpace(accountName.Text))
			{
				ShowAlertMessage(Constants.ERROR_DATA, Constants.ERROR_REQUIRED.Replace("{field_name}", "Account name"));
			}
			else if (ValicationUtil.IsOverMaxLength(accountName.Text, 100))
			{
				ShowAlertMessage(Constants.ERROR_DATA, Constants.ERROR_OVER_MAX_LENGTH.Replace("{field_name}", "Account name").Replace("{max_length}", "100"));
			}
			else if (string.IsNullOrWhiteSpace(accountNumber.Text))
			{
				ShowAlertMessage(Constants.ERROR_DATA, Constants.ERROR_REQUIRED.Replace("{field_name}", "Account number"));
			}
			else if (ValicationUtil.IsOverMaxLength(accountNumber.Text, 30))
			{
				ShowAlertMessage(Constants.ERROR_DATA, Constants.ERROR_OVER_MAX_LENGTH.Replace("{field_name}", "Account number").Replace("{max_length}", "30"));
			}
			else if (string.IsNullOrWhiteSpace(atmPassword.Text))
			{
				ShowAlertMessage(Constants.ERROR_DATA, Constants.ERROR_REQUIRED.Replace("{field_name}", "ATM password"));
			}
			else if (ValicationUtil.IsOverMaxLength(atmPassword.Text, 20))
			{
				ShowAlertMessage(Constants.ERROR_DATA, Constants.ERROR_OVER_MAX_LENGTH.Replace("{field_name}", "ATM password").Replace("{max_length}", "20"));
			}
			else if (!string.IsNullOrWhiteSpace(passwordExpiredDuration.Text) && string.IsNullOrWhiteSpace(updatedDate.Text))
			{
				ShowAlertMessage(Constants.ERROR_DATA, Constants.ERROR_REQUIRED.Replace("{field_name}", "Updated date"));
			}
			else if (!string.IsNullOrWhiteSpace(iBankUser.Text) && ValicationUtil.IsOverMaxLength(iBankUser.Text, 30))
			{
				ShowAlertMessage(Constants.ERROR_DATA, Constants.ERROR_OVER_MAX_LENGTH.Replace("{field_name}", "Internet banking user").Replace("{max_length}", "30"));
			}
			else if (!string.IsNullOrWhiteSpace(iBankPassword.Text) && ValicationUtil.IsOverMaxLength(iBankPassword.Text, 20))
			{
				ShowAlertMessage(Constants.ERROR_DATA, Constants.ERROR_OVER_MAX_LENGTH.Replace("{field_name}", "Internet banking password").Replace("{max_length}", "30"));
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
			SelectedAccount = new BankAccount();

			if (!IsViewLoaded)
			{
				return;
			}

			if (selectedAccViewModel != null && selectedAccViewModel.accountId != 0)
			{
				// Get bank account
				SelectedAccount = BankAccountManager.GetBankAccount(selectedAccViewModel.accountId);
				
				accountName.Text = SelectedAccount.Name;
				accountNumber.Text = SelectedAccount.AccountNumber;
				atmPassword.Text = SecurityUtil.Decrypt(SelectedAccount.AtmPassword);
				iBankUser.Text = SelectedAccount.EBUserName;
				iBankPassword.Text = SecurityUtil.Decrypt(SelectedAccount.EBPassword);
				securityQuestion.Text = SelectedAccount.SecurityQuestion;
				securityAnswer.Text = SelectedAccount.SecurityAnswer;
				note.Text = SelectedAccount.Note;
				if (SelectedAccount.PasswordDuration == 0)
				{
					passwordExpiredDuration.Text = string.Empty;
				}
				else
				{
					passwordExpiredDuration.Text = SelectedAccount.PasswordDuration + " days";
				}
				updatedDate.Text = SelectedAccount.UpdatedDate;
			}
			else 
			{
				accountName.Text = string.Empty;
				accountNumber.Text = string.Empty;
				atmPassword.Text = string.Empty;
				iBankUser.Text = string.Empty;
				iBankPassword.Text = string.Empty;
				securityQuestion.Text = string.Empty;
				securityAnswer.Text = string.Empty;
				note.Text = string.Empty;
				passwordExpiredDuration.Text = string.Empty;
				updatedDate.Text = string.Empty;
			}
		}
    }
}