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
	[Activity(Label = "Bank Account")]
	public class BankDetailActivity : Activity
	{
		EditText edtBankName;
		EditText edtAccountNumber;
		EditText edtAtmPassword;
		EditText edtEBankUsername;
		EditText edtEBankPassword;
		EditText edtSecurityQuestion;
		EditText edtSecurityAnswer;
		Spinner spinnerDuration;
		EditText edtUpdatedDate;
		EditText edtNote;

		Button btnCancel;
		Button btnSave;
		Button btnDelete;

		ArrayAdapter adapter;

		BankAccount selectedAccount;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your application here
			SetContentView(Resource.Layout.BankDetail);

			// Get control from view
			edtBankName = FindViewById<EditText>(Resource.Id.edtBankName);
			edtAccountNumber = FindViewById<EditText>(Resource.Id.edtAccountNumber);
			edtAtmPassword = FindViewById<EditText>(Resource.Id.edtATMPassword);
			edtEBankUsername = FindViewById<EditText>(Resource.Id.edtEbankUsername);
			edtEBankPassword = FindViewById<EditText>(Resource.Id.edtEbankPassword);
			edtSecurityQuestion = FindViewById<EditText>(Resource.Id.edtSecurityQuestion);
			edtSecurityAnswer = FindViewById<EditText>(Resource.Id.edtSecurityAnswer);
			spinnerDuration = FindViewById<Spinner>(Resource.Id.spinnerDuration);
			edtUpdatedDate = FindViewById<EditText>(Resource.Id.edtUpdatedDate);
			edtNote = FindViewById<EditText>(Resource.Id.edtNote);
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

			if (selectedAccount.PasswordDuration == 0)
			{
				edtUpdatedDate.Visibility = ViewStates.Invisible;
			}
			else
			{
				edtUpdatedDate.Visibility = ViewStates.Visible;
			}

			if (selectedAccount.ID == 0)
			{
				btnDelete.Visibility = ViewStates.Invisible;
			}
			else
			{
				btnDelete.Visibility = ViewStates.Visible;
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
				var imm = (InputMethodManager)GetSystemService(InputMethodService);
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

		void BtnCancel_Click(object sender, EventArgs e)
		{
			GoHomeScreen();
		}

		void BtnSave_Click(object sender, EventArgs e)
		{
			// Save the expense back to the data store.
			if (selectedAccount != null && validateBankAccount())
			{
				selectedAccount.Name = edtBankName.Text;
				selectedAccount.AccountNumber = edtAccountNumber.Text;
				selectedAccount.AtmPassword = SecurityUtil.Encrypt(edtAtmPassword.Text);
				selectedAccount.EBUserName = edtEBankUsername.Text;
				selectedAccount.EBPassword = SecurityUtil.Encrypt(edtEBankPassword.Text);
				selectedAccount.SecurityQuestion = edtSecurityQuestion.Text;
				selectedAccount.SecurityAnswer = edtSecurityAnswer.Text;
				selectedAccount.Note = edtNote.Text;

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

				BankAccountManager.SaveBankAccount(selectedAccount);

				GoHomeScreen();
			}
		}

		void BtnDelete_Click(object sender, EventArgs e)
		{
			BankAccountManager.DeleteBankAccount(selectedAccount.ID);
			GoHomeScreen();
		}

		void GoHomeScreen()
		{
			var mainIntent = new Intent(this, typeof(MainActivity));
			StartActivity(mainIntent);
			Finish();
		}

		bool validateBankAccount()
		{
			bool isValid = false;

			if (string.IsNullOrWhiteSpace(edtBankName.Text))
			{
				Toast.MakeText(this, Constants.ERROR_REQUIRED.Replace("{field_name}", "Account name"), ToastLength.Short).Show();
			}
			else if (ValicationUtil.IsOverMaxLength(edtBankName.Text, 100))
			{
				Toast.MakeText(this, Constants.ERROR_OVER_MAX_LENGTH
				               .Replace("{field_name}", "Account name").Replace("{max_length}", "100"), ToastLength.Short).Show();
			}
			else if (string.IsNullOrWhiteSpace(edtAccountNumber.Text))
			{
				Toast.MakeText(this, Constants.ERROR_REQUIRED.Replace("{field_name}", "Account number"), ToastLength.Short).Show();
			}
			else if (ValicationUtil.IsOverMaxLength(edtAccountNumber.Text, 30))
			{
				Toast.MakeText(this, Constants.ERROR_OVER_MAX_LENGTH
				               .Replace("{field_name}", "Account number").Replace("{max_length}", "30"), ToastLength.Short).Show();
			}
			else if (string.IsNullOrWhiteSpace(edtAtmPassword.Text))
			{
				Toast.MakeText(this, Constants.ERROR_REQUIRED.Replace("{field_name}", "ATM password"), ToastLength.Short).Show();
			}
			else if (ValicationUtil.IsOverMaxLength(edtAtmPassword.Text, 20))
			{
				Toast.MakeText(this, Constants.ERROR_OVER_MAX_LENGTH
				               .Replace("{field_name}", "ATM password").Replace("{max_length}", "20"), ToastLength.Short).Show();
			}
			else if (spinnerDuration.SelectedItemPosition != 0 && string.IsNullOrWhiteSpace(edtUpdatedDate.Text))
			{
				Toast.MakeText(this, Constants.ERROR_REQUIRED.Replace("{field_name}", "Updated date"), ToastLength.Short).Show();
			}
			else if (!string.IsNullOrWhiteSpace(edtEBankUsername.Text) && ValicationUtil.IsOverMaxLength(edtEBankUsername.Text, 30))
			{
				Toast.MakeText(this, Constants.ERROR_OVER_MAX_LENGTH
				               .Replace("{field_name}", "Internet banking user").Replace("{max_length}", "30"), ToastLength.Short).Show();
			}
			else if (!string.IsNullOrWhiteSpace(edtEBankPassword.Text) && ValicationUtil.IsOverMaxLength(edtEBankPassword.Text, 20))
			{
				Toast.MakeText(this, Constants.ERROR_OVER_MAX_LENGTH
				               .Replace("{field_name}", "Internet banking password").Replace("{max_length}", "30"), ToastLength.Short).Show();
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
			selectedAccount = new BankAccount();

			if (accountId != 0)
			{
				// Get bank account
				selectedAccount = BankAccountManager.GetBankAccount(accountId);

				edtBankName.Text = selectedAccount.Name;
				edtAccountNumber.Text = selectedAccount.AccountNumber;
				edtAtmPassword.Text = SecurityUtil.Decrypt(selectedAccount.AtmPassword);
				edtEBankUsername.Text = selectedAccount.EBUserName;
				edtEBankPassword.Text = SecurityUtil.Decrypt(selectedAccount.EBPassword);
				edtSecurityQuestion.Text = selectedAccount.SecurityQuestion;
				edtSecurityAnswer.Text = selectedAccount.SecurityAnswer;
				edtNote.Text = selectedAccount.Note;
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
				edtBankName.Text = string.Empty;
				edtAccountNumber.Text = string.Empty;
				edtAtmPassword.Text = string.Empty;
				edtEBankUsername.Text = string.Empty;
				edtEBankPassword.Text = string.Empty;
				edtSecurityQuestion.Text = string.Empty;
				edtSecurityAnswer.Text = string.Empty;
				edtNote.Text = string.Empty;
				spinnerDuration.SetSelection(0);
				edtUpdatedDate.Text = string.Empty;
			}
		}
	}
}

