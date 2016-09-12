
using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using mPassword.Shared;

namespace mPassword.Droid
{
	[Activity(Label = "Login")]
	public class LoginActivity : Activity
	{
		Button btnLogin;
		Button btnLinkToRegister;
		EditText inputUsername;
		EditText inputPassword;
		ProgressDialog pDialog;
		SessionManager session;
		User loginUser;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your application here
			SetContentView(Resource.Layout.Login);

			// Get control from view
			inputUsername = FindViewById<EditText>(Resource.Id.username);
			inputPassword = FindViewById<EditText>(Resource.Id.password);
			btnLogin = FindViewById<Button>(Resource.Id.btnLogin);
			btnLinkToRegister = FindViewById<Button>(Resource.Id.btnLinkToRegisterScreen);

			// Progress dialog
			pDialog = new ProgressDialog(this);
			pDialog.SetCancelable(false);

			// Session manager
			session = new SessionManager(Application.Context);

			// Check if user is already logged in or not
			if (session.isLoggedIn())
			{
				// User is already logged in. Take him to main activity
				var mainIntent = new Intent(this, typeof(MainActivity));
				StartActivity(mainIntent);
				Finish();
			}

			btnLinkToRegister.Click += (sender, e) =>
			{
				var signupIntent = new Intent(this, typeof(SignUpActivity));
				StartActivity(signupIntent);
				Finish();
			};

			btnLogin.Click += BtnLogin_Click;

		}

		void BtnLogin_Click(object sender, EventArgs e)
		{
			//Validate our Username & Password.
			//This is usually a web service call.
			if (IsAccountValid())
			{
				// Set login is true to session.
				session.setLogin(true);
				session.setLoginUserId(loginUser.ID);

				// User is already logged in. Take him to main activity
				var mainIntent = new Intent(this, typeof(MainActivity));
				StartActivity(mainIntent);
				Finish();
			}
			else
			{
				Toast.MakeText(this, Constants.ERROR_USER_AUTHENTICATION, ToastLength.Short).Show();
			}
		}

		bool IsAccountValid()
		{
			var userName = inputUsername.Text.Trim();
			var password = inputPassword.Text.Trim();

			loginUser = Shared.UserManager.GetUserByName(userName);

			// User not existed.
			if (loginUser == null)
			{
				return false;
			}

			// Check password.
			StringComparer compare = StringComparer.Ordinal;

			if (compare.Equals(loginUser.Password, SecurityUtil.HashSHA256(password)))
			{
				return true;
			}

			return false;
		}
	}
}