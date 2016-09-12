
using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using mPassword.Shared;

namespace mPassword.Droid
{
	[Activity(Label = "Web Account")]
	public class WebDetailActivity : Activity
	{
		EditText edtAccountName;
		EditText edtUrl;
		EditText edtUsername;
		EditText edtPassword;
		EditText edtNote;

		Button btnCancel;
		Button btnSave;
		Button btnDelete;

		WebAccount selectedAccount;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your application here
			SetContentView(Resource.Layout.WebDetail);

			// Get control from view
			edtAccountName = FindViewById<EditText>(Resource.Id.edtAccountName);
			edtUrl = FindViewById<EditText>(Resource.Id.edtUrl);
			edtUsername = FindViewById<EditText>(Resource.Id.edtUsername);
			edtPassword = FindViewById<EditText>(Resource.Id.edtPassword);
			edtNote = FindViewById<EditText>(Resource.Id.edtNote);
			btnSave = FindViewById<Button>(Resource.Id.btnSave);
			btnCancel = FindViewById<Button>(Resource.Id.btnCancel);
			btnDelete = FindViewById<Button>(Resource.Id.btnDelete);

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

			btnCancel.Click += BtnCancel_Click;

			btnSave.Click += BtnSave_Click;

			btnDelete.Click += BtnDelete_Click;
		}

		void BtnDelete_Click(object sender, EventArgs e)
		{
			WebAccountManager.DeleteWebAccount(selectedAccount.ID);
			GoHomeScreen();
		}

		void BtnCancel_Click(object sender, EventArgs e)
		{
			GoHomeScreen();
		}

		void BtnSave_Click(object sender, EventArgs e)
		{
			// Save the expense back to the data store.
			if (selectedAccount != null && validateWebAccount())
			{
				selectedAccount.Name = edtAccountName.Text;
				selectedAccount.URL = edtUrl.Text;
				selectedAccount.UserName = edtUsername.Text;
				selectedAccount.Password = SecurityUtil.Encrypt(edtPassword.Text);
				selectedAccount.Note = edtNote.Text;

				// session manager
				var session = new SessionManager(Application.Context);
				selectedAccount.UserID = session.getLoginUserId();

				WebAccountManager.SaveWebAccount(selectedAccount);

				GoHomeScreen();
			}
		}

		void GoHomeScreen()
		{
			var mainIntent = new Intent(this, typeof(MainActivity));
			StartActivity(mainIntent);
			Finish();
		}

		bool validateWebAccount()
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
			else if (string.IsNullOrWhiteSpace(edtUrl.Text))
			{
				Toast.MakeText(this, Constants.ERROR_REQUIRED.Replace("{field_name}", "URL"), ToastLength.Short).Show();
			}
			else if (ValicationUtil.IsOverMaxLength(edtUrl.Text, 255))
			{
				Toast.MakeText(this, Constants.ERROR_OVER_MAX_LENGTH.Replace("{field_name}", "URL").Replace("{max_length}", "255"), ToastLength.Short).Show();
			}
			else if (string.IsNullOrWhiteSpace(edtUsername.Text))
			{
				Toast.MakeText(this, Constants.ERROR_REQUIRED.Replace("{field_name}", "Username"), ToastLength.Short).Show();
			}
			else if (ValicationUtil.IsOverMaxLength(edtUsername.Text, 30))
			{
				Toast.MakeText(this, Constants.ERROR_OVER_MAX_LENGTH.Replace("{field_name}", "Username").Replace("{max_length}", "30"), ToastLength.Short).Show();
			}
			else if (string.IsNullOrWhiteSpace(edtPassword.Text))
			{
				Toast.MakeText(this, Constants.ERROR_REQUIRED.Replace("{field_name}", "Password"), ToastLength.Short).Show();
			}
			else if (ValicationUtil.IsOverMaxLength(edtPassword.Text, 20))
			{
				Toast.MakeText(this, Constants.ERROR_OVER_MAX_LENGTH.Replace("{field_name}", "Password").Replace("{max_length}", "20"), ToastLength.Short).Show();
			}
			else
			{
				isValid = true;
			}
			return isValid;
		}

		void OnUpdateDetails(int accountId)
		{
			selectedAccount = new WebAccount();

			if (accountId != 0)
			{
				// Get computer account
				selectedAccount = WebAccountManager.GetWebAccount(accountId);

				edtAccountName.Text = selectedAccount.Name;
				edtUrl.Text = selectedAccount.URL;
				edtUsername.Text = selectedAccount.UserName;
				edtPassword.Text = SecurityUtil.Decrypt(selectedAccount.Password);
				edtNote.Text = selectedAccount.Note;
			}
			else
			{
				edtAccountName.Text = string.Empty;
				edtUrl.Text = string.Empty;
				edtUsername.Text = string.Empty;
				edtPassword.Text = string.Empty;
				edtNote.Text = string.Empty;
			}
		}
	}
}

