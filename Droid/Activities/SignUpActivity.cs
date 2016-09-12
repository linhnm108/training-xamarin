
using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using mPassword.Shared;

namespace mPassword.Droid
{
	[Activity(Label = "Register")]
	public class SignUpActivity : Activity
	{
		Button btnRegister;
		Button btnLinkToLogin;
		EditText inputUserName;
		EditText inputConfirmPassword;
		EditText inputPassword;
		ProgressDialog pDialog;
		SessionManager session;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your application here
			SetContentView(Resource.Layout.SignUp);

			// Get control from view
			inputUserName = FindViewById<EditText>(Resource.Id.username);
			inputPassword = FindViewById<EditText>(Resource.Id.password);
			inputConfirmPassword = FindViewById<EditText>(Resource.Id.confirm_password);
			btnRegister = FindViewById<Button>(Resource.Id.btnRegister);
			btnLinkToLogin = FindViewById<Button>(Resource.Id.btnLinkToLoginScreen);

			// Progress dialog
			pDialog = new ProgressDialog(this);
			pDialog.SetCancelable(false);

			session = new SessionManager(Application.Context);

			// Check if user is already logged in or not
			if (session.isLoggedIn())
			{
				// User is already logged in. Take him to main activity
				var maintIntent = new Intent(this, typeof(MainActivity));
				StartActivity(maintIntent);
				Finish();
			}

			btnLinkToLogin.Click += (sender, e) =>
			{
				var loginIntent = new Intent(this, typeof(LoginActivity));
				StartActivity(loginIntent);
				Finish();
			};

			btnRegister.Click += BtnRegister_Click;

		}

		void BtnRegister_Click(object sender, EventArgs e)
		{
			var userName = inputUserName.Text.Trim();
			var password = inputPassword.Text.Trim();
			var confirmPassword = inputConfirmPassword.Text.Trim();

			if (string.IsNullOrEmpty(userName))
			{
				Toast.MakeText(this, Constants.ERROR_REQUIRED.Replace("{field_name}", "Username"), ToastLength.Short).Show();
			}
			else if (string.IsNullOrEmpty(password))
			{
				Toast.MakeText(this, Constants.ERROR_REQUIRED.Replace("{field_name}", "Password"), ToastLength.Short).Show();
			}
			else if (ValicationUtil.IsOverMaxLength(userName, 12))
			{
				Toast.MakeText(this, Constants.ERROR_OVER_MAX_LENGTH.Replace("{field_name}", "Username")
				               .Replace("{max_length}", "12"), ToastLength.Short).Show();
			}
			else if (!ValicationUtil.IsValidPassword(password))
			{
				Toast.MakeText(this, Constants.ERROR_PASSWORD_INVALID, ToastLength.Short).Show();
			}
			else if (password != confirmPassword)
			{
				Toast.MakeText(this, Constants.ERROR_PASSWORD_NOT_MATCH, ToastLength.Short).Show();
			}
			else if (Shared.UserManager.GetUserByName(userName) != null)
			{
				Toast.MakeText(this, Constants.ERROR_USER_EXISTED, ToastLength.Short).Show();
			}
			else
			{
				// Save user info to database
				var user = new User();
				user.UserName = userName;
				user.Password = SecurityUtil.HashSHA256(password);
				Shared.UserManager.SaveUser(user);

				// Set login is true to session.
				session.setLogin(true);
				session.setLoginUserId(user.ID);

				// User is already logged in. Take him to main activity
				var mainIntent = new Intent(this, typeof(MainActivity));
				StartActivity(mainIntent);
				Finish();
			}
		}
	}
}

