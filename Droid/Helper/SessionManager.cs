using Android.Content;
using Android.Preferences;

namespace mPassword.Droid
{
	public class SessionManager
	{
		public SessionManager()
		{
		}

		// Shared Preferences
		readonly ISharedPreferences pref;
		ISharedPreferencesEditor editor;

		static string KEY_IS_LOGGED_IN = "isLoggedIn";

		static string KEY_LOGIN_USER_ID = "loginUserId";

		public SessionManager(Context context)
		{
			pref = PreferenceManager.GetDefaultSharedPreferences(context);
			editor = pref.Edit();
		}

		public void setLogin(bool isLoggedIn)
		{
			editor.PutBoolean(KEY_IS_LOGGED_IN, isLoggedIn);

			// commit changes
			editor.Commit();
		}

		public void setLoginUserId(int loginUserId)
		{
			editor.PutInt(KEY_LOGIN_USER_ID, loginUserId);

			// commit changes
			editor.Commit();
		}

		public bool isLoggedIn()
		{
			return pref.GetBoolean(KEY_IS_LOGGED_IN, false);
		}

		public int getLoginUserId()
		{
			return pref.GetInt(KEY_LOGIN_USER_ID, 0);
		}
	}
}