using System;
using UIKit;
using mPassword.Shared;

namespace mPassword.iOS
{
    public partial class ComputerAccDetailsViewController : UIViewController
    {

		UIBarButtonItem saveButton;
		AccountViewModel selectedAccViewModel;

		public ComputerAccount SelectedAccount { set; get; }

		public ComputerAccDetailsViewController(IntPtr handle) : base(handle)
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
			toolbar.SetItems(new UIBarButtonItem[] { flexibleSpace, doneButton }, true);

			// Tell the textbox to use the picker for input
			passwordExpiredDuration.InputView = pickerView;

			// Display the toolbar over the pickers
			passwordExpiredDuration.InputAccessoryView = toolbar;

			// Create save button
			saveButton = new UIBarButtonItem(UIBarButtonSystemItem.Save);
			saveButton.Clicked += SaveComputerAccount;
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
				updatedDate.Text = string.Format("{0:MM/dd/yyyy}", (DateTime)((UIDatePicker)sender).Date);
			};
		}

		void SaveComputerAccount(object sender, EventArgs e)
		{
			// Save the expense back to the data store.
			if (SelectedAccount != null && validateComputerAccount())
			{
				SelectedAccount.Name = accountName.Text;
				SelectedAccount.UserName = userName.Text;
				SelectedAccount.Password = SecurityUtil.Encrypt(password.Text);

				if (!string.IsNullOrWhiteSpace(passwordExpiredDuration.Text))
				{
					var duration = passwordExpiredDuration.Text.Replace(" days", string.Empty);
					SelectedAccount.PasswordDuration = int.Parse(duration);
					SelectedAccount.UpdatedDate = updatedDate.Text;
				}

				//Create an instance of our AppDelegate
				var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
				SelectedAccount.UserID = appDelegate.LoginUser.ID;

				ComputerAccountManager.SaveComputerAccount(SelectedAccount);

				SetNewAccountInfo();

				// Go back to prior screen
				NavigationController.PopViewController(true);
			}
		}

		void SetNewAccountInfo()
		{
			SelectedAccViewModel.accountId = SelectedAccount.ID;
			SelectedAccViewModel.accountName = SelectedAccount.Name;
			SelectedAccViewModel.accountType = "ComputerAcc";
			SelectedAccViewModel.isExpiredWarning = SelectedAccViewModel.IsExpiredWarning(SelectedAccount.UpdatedDate, SelectedAccount.PasswordDuration);
		}

		bool validateComputerAccount()
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
			else if (!string.IsNullOrWhiteSpace(passwordExpiredDuration.Text) && string.IsNullOrWhiteSpace(updatedDate.Text))
			{
				ShowAlertMessage(Constants.ERROR_DATA, Constants.ERROR_REQUIRED.Replace("{field_name}", "Updated date"));
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
			SelectedAccount = new ComputerAccount();
			if (!IsViewLoaded)
			{
				return;
			}

			if (selectedAccViewModel != null && selectedAccViewModel.accountId != 0)
			{
				// Get bank account
				SelectedAccount = ComputerAccountManager.GetComputerAccount(selectedAccViewModel.accountId);

				accountName.Text = SelectedAccount.Name;
				userName.Text = SelectedAccount.UserName;
				password.Text = SecurityUtil.Decrypt(SelectedAccount.Password);

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
				userName.Text = string.Empty;
				password.Text = string.Empty;
				passwordExpiredDuration.Text = string.Empty;
				updatedDate.Text = string.Empty;
			}
		}
    }
}