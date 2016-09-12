
using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using mPassword.Shared;

namespace mPassword.Droid
{
	[Activity(Label = "Computer Account")]
	public class ComputerDetailActivity : Activity
	{
		EditText edtAccountName;
		EditText edtUsername;
		EditText edtPassword;
		Spinner spinnerDuration;
		EditText edtUpdatedDate;

		Button btnCancel;
		Button btnSave;
		Button btnDelete;

		ArrayAdapter adapter;

		ComputerAccount selectedAccount;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your application here
			SetContentView(Resource.Layout.ComputerDetail);

			// Get control from view
			edtAccountName = FindViewById<EditText>(Resource.Id.edtAccountName);
			edtUsername = FindViewById<EditText>(Resource.Id.edtUsername);
			edtPassword = FindViewById<EditText>(Resource.Id.edtPassword);
			spinnerDuration = FindViewById<Spinner>(Resource.Id.spinnerDuration);
			edtUpdatedDate = FindViewById<EditText>(Resource.Id.edtUpdatedDate);
			btnSave = FindViewById<Button>(Resource.Id.btnSave);
			btnCancel = FindViewById<Button>(Resource.Id.btnCancel);
			btnDelete = FindViewById<Button>(Resource.Id.btnDelete);

			// Set data for spiner
			adapter = ArrayAdapter.CreateFromResource(
				this, Resource.Array.listDay, Android.Resource.Layout.SimpleSpinnerItem);

			adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
			spinnerDuration.Adapter = adapter;

			// Get data from Itent
			int account_id = Intent.GetIntExtra("accountId", 0);

			OnUpdateDetails(account_id);

			if (selectedAccount.ID == 0)
			{
				btnDelete.Visibility = ViewStates.Invisible;
			}
			else
			{
				btnDelete.Visibility = ViewStates.Visible;
			}

			if (selectedAccount.PasswordDuration == 0)
			{
				edtUpdatedDate.Visibility = ViewStates.Invisible;
			}
			else
			{
				edtUpdatedDate.Visibility = ViewStates.Visible;
			}

			spinnerDuration.ItemSelected += (sender, e) =>
			{
				if (e.Position == 0)
				{
					edtUpdatedDate.Visibility = ViewStates.Invisible;
				}
				else
				{
					edtUpdatedDate.Visibility = ViewStates.Visible;
				}

			};

			edtUpdatedDate.Click += (sender, e) =>
			{
				var imm = (InputMethodManager) GetSystemService(InputMethodService);
				imm.HideSoftInputFromWindow(CurrentFocus.WindowToken, 0);

				//To show current date in the datepicker
				DateTime today = DateTime.Today;
				var dialog = new DatePickerDialog(this, OnDateSet, today.Year, today.Month - 1, today.Day);
				dialog.Show();
			};

			btnCancel.Click += BtnCancel_Click;

			btnSave.Click += BtnSave_Click;

			btnDelete.Click += BtnDelete_Click;
		}

		void BtnDelete_Click(object sender, EventArgs e)
		{
			ComputerAccountManager.DeleteComputerAccount(selectedAccount.ID);
			GoHomeScreen();
		}

		void BtnCancel_Click(object sender, EventArgs e)
		{
			GoHomeScreen();
		}

		void BtnSave_Click(object sender, EventArgs e)
		{
			// Save the expense back to the data store.
			if (selectedAccount != null && validateComputerAccount())
			{
				selectedAccount.Name = edtAccountName.Text;
				selectedAccount.UserName = edtUsername.Text;
				selectedAccount.Password = SecurityUtil.Encrypt(edtPassword.Text);

				int position = spinnerDuration.SelectedItemPosition;
				if (position != 0)
				{
					var duration = spinnerDuration.GetItemAtPosition(position).ToString().Replace(" days", string.Empty);
					selectedAccount.PasswordDuration = int.Parse(duration);
					selectedAccount.UpdatedDate = edtUpdatedDate.Text;
				}

				// session manager
				var session = new SessionManager(Application.Context);
				selectedAccount.UserID = session.getLoginUserId();

				ComputerAccountManager.SaveComputerAccount(selectedAccount);

				GoHomeScreen();
			}
		}

		void GoHomeScreen()
		{
			var mainIntent = new Intent(this, typeof(MainActivity));
			StartActivity(mainIntent);
			Finish();
		}

		bool validateComputerAccount()
		{
			bool isValid = false;

			if (string.IsNullOrWhiteSpace(edtAccountName.Text))
			{
				Toast.MakeText(this, Constants.ERROR_REQUIRED.Replace("{field_name}", "Account name"), ToastLength.Short).Show();
			}
			else if (ValicationUtil.IsOverMaxLength(edtAccountName.Text, 50))
			{
				Toast.MakeText(this, Constants.ERROR_OVER_MAX_LENGTH.Replace("{field_name}", "Account name").Replace("{max_length}", "100"), ToastLength.Short).Show();
			}
			else if (string.IsNullOrWhiteSpace(edtUsername.Text))
			{
				Toast.MakeText(this, Constants.ERROR_REQUIRED.Replace("{field_name}", "Username"), ToastLength.Short).Show();
			}
			else if (ValicationUtil.IsOverMaxLength(edtUsername.Text, 30))
			{
				Toast.MakeText(this, Constants.ERROR_OVER_MAX_LENGTH
				               .Replace("{field_name}", "Username").Replace("{max_length}", "30"), ToastLength.Short).Show();
			}
			else if (string.IsNullOrWhiteSpace(edtPassword.Text))
			{
				Toast.MakeText(this, Constants.ERROR_REQUIRED.Replace("{field_name}", "Password"), ToastLength.Short).Show();
			}
			else if (ValicationUtil.IsOverMaxLength(edtPassword.Text, 20))
			{
				Toast.MakeText(this, Constants.ERROR_OVER_MAX_LENGTH.Replace("{field_name}", "Password").Replace("{max_length}", "20"), ToastLength.Short).Show();
			}
			else if (spinnerDuration.SelectedItemPosition != 0 && string.IsNullOrWhiteSpace(edtUpdatedDate.Text))
			{
				Toast.MakeText(this, Constants.ERROR_REQUIRED.Replace("{field_name}", "Updated date"), ToastLength.Short).Show();
			}
			else
			{
				isValid = true;
			}
			return isValid;
		}

		void OnDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
		{
			edtUpdatedDate.Text = e.Date.ToString("MM/dd/yyyy");
		}

		void OnUpdateDetails(int accountId)
		{
			selectedAccount = new ComputerAccount();

			if (accountId != 0)
			{
				// Get computer account
				selectedAccount = ComputerAccountManager.GetComputerAccount(accountId);

				edtAccountName.Text = selectedAccount.Name;
				edtUsername.Text = selectedAccount.UserName;
				edtPassword.Text = SecurityUtil.Decrypt(selectedAccount.Password);
				if (selectedAccount.PasswordDuration == 0)
				{
					spinnerDuration.SetSelection(0);
				}
				else
				{
					spinnerDuration.SetSelection(adapter.GetPosition(selectedAccount.PasswordDuration + " days"));
				}
				edtUpdatedDate.Text = selectedAccount.UpdatedDate;
			}
			else
			{
				edtAccountName.Text = string.Empty;
				edtUsername.Text = string.Empty;
				edtPassword.Text = string.Empty;
				spinnerDuration.SetSelection(0);
				edtUpdatedDate.Text = string.Empty;
			}
		}
	}
}

