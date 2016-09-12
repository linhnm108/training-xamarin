using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using mPassword.Shared;

namespace mPassword.Droid
{
	[Activity(Label = "Edit Profile")]
	public class EditProfileActivity : Activity
	{
		EditText edtOldPass;
		EditText edtNewPass;
		EditText edtConfirmNewPass;
		Button btnSave;
		Button btnCancel;

		User loginUser;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your application here
			SetContentView(Resource.Layout.EditProfile);

			var session = new SessionManager(Application.Context);

			loginUser = Shared.UserManager.GetUser(session.getLoginUserId());

			// Get control from view
			edtOldPass = FindViewById<EditText>(Resource.Id.edtOldPass);
			edtNewPass = FindViewById<EditText>(Resource.Id.edtNewPass);
			edtConfirmNewPass = FindViewById<EditText>(Resource.Id.edtConfirmNewPass);
			btnSave = FindViewById<Button>(Resource.Id.btnSave);
			btnCancel = FindViewById<Button>(Resource.Id.btnCancel);

			btnCancel.Click += BtnCancel_Click;

			btnSave.Click += BtnSave_Click;
		}

		void BtnCancel_Click(object sender, EventArgs e)
		{
			GoHomeScreen();
		}

		void BtnSave_Click(object sender, EventArgs e)
		{
			var inputOldPass = edtOldPass.Text;
			var inputNewPass = edtNewPass.Text;
			var inputConfirmNewPass = edtConfirmNewPass.Text;

			if (loginUser.Password != SecurityUtil.HashSHA256(inputOldPass))
			{
				Toast.MakeText(this, Constants.ERROR_OLD_PASSWORD_INCORRECT, ToastLength.Short).Show();
			}
			else if (string.IsNullOrEmpty(inputNewPass))
			{
				Toast.MakeText(this, Constants.ERROR_REQUIRED.Replace("{field_name}", "New Password"), ToastLength.Short).Show();
			}
			else if (!ValicationUtil.IsValidPassword(inputNewPass))
			{
				Toast.MakeText(this, Constants.ERROR_PASSWORD_INVALID, ToastLength.Short).Show();
			}
			else if (inputNewPass != inputConfirmNewPass)
			{
				Toast.MakeText(this, Constants.ERROR_PASSWORD_NOT_MATCH, ToastLength.Short).Show();
			}
			else
			{
				loginUser.Password = SecurityUtil.HashSHA256(inputNewPass);
				Shared.UserManager.SaveUser(loginUser);

				// Go back to prior screen
				Toast.MakeText(this, "Update profile successfully.", ToastLength.Short).Show();

				GoHomeScreen();
			}
		}

		void GoHomeScreen()
		{
			var mainIntent = new Intent(this, typeof(MainActivity));
			StartActivity(mainIntent);
			Finish();
		}
	}
}

