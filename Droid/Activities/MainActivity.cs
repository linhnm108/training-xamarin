using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using System.Collections.Generic;
using mPassword.Shared;
using Android.Views;

namespace mPassword.Droid
{

	[Activity(Label = "mPassword", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		SessionManager session;
		AccountListAdapter adapter;
		ExpandableListView expandListView;

		List<CategoryViewModel> listCategory;
		Dictionary<string, List<AccountViewModel>> listAccount;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			// session manager
			session = new SessionManager(Application.Context);

			if (!session.isLoggedIn())
			{
				LogoutUser();
			}

			// get the listview
			expandListView = FindViewById<ExpandableListView>(Resource.Id.expandListView);

			// preparing list data
			prepareListData();

			adapter = new AccountListAdapter(this, listCategory, listAccount);

			// setting list adapter
			expandListView.SetAdapter(adapter);

			// Event Handle
			expandListView.ChildClick += ExpandListView_ChildClick;
		}

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.MenuItems, menu);
			return base.OnCreateOptionsMenu(menu);
		}


		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.TitleFormatted.ToString())
			{
				case "Edit profile":
					StartActivity(typeof(EditProfileActivity));
					break;
				case "Logout":
					LogoutUser();
					break;
			}
			return base.OnOptionsItemSelected(item);
		}

		void ExpandListView_ChildClick(object sender, ExpandableListView.ChildClickEventArgs e)
		{
			var categoryPosition = e.GroupPosition;
			var accountPosition = e.ChildPosition;

			AccountViewModel account = adapter.GetAccountByPosition(categoryPosition, accountPosition);

			switch (categoryPosition)
			{
				case 0:
					var bankIntent = new Intent(this, typeof(BankDetailActivity));
					bankIntent.PutExtra("accountId", account.accountId);

					StartActivity(bankIntent);
					break;
				case 1:
					var computerIntent = new Intent(this, typeof(ComputerDetailActivity));
					computerIntent.PutExtra("accountId", account.accountId);

					StartActivity(computerIntent);
					break;
				case 2:
					var emailIntent = new Intent(this, typeof(EmailDetailActivity));
					emailIntent.PutExtra("accountId", account.accountId);

					StartActivity(emailIntent);
					break;
				case 3:
					var webIntent = new Intent(this, typeof(WebDetailActivity));
					webIntent.PutExtra("accountId", account.accountId);

					StartActivity(webIntent);
					break;
			}
		}

		void prepareListData()
		{
			listCategory = new List<CategoryViewModel>();

			IList<BankAccount> bankAccounts = BankAccountManager.GetBankAccountsByUserId(session.getLoginUserId());
			listCategory.Add(new CategoryViewModel(bankAccounts));

			IList<ComputerAccount> computerAccounts = ComputerAccountManager.GetComputerAccountsByUserId(session.getLoginUserId());
			listCategory.Add(new CategoryViewModel(computerAccounts));

			IList<EmailAccount> emailAccounts = EmailAccountManager.GetEmailAccountsByUserId(session.getLoginUserId());
			listCategory.Add(new CategoryViewModel(emailAccounts));

			IList<WebAccount> webAccounts = WebAccountManager.GetWebAccountsByUserId(session.getLoginUserId());
			listCategory.Add(new CategoryViewModel(webAccounts));

			listAccount = new Dictionary<string, List<AccountViewModel>>();

			// Adding child data to Bank Section
			var listBankAccount = new List<AccountViewModel>();

			var newBankAcc = new AccountViewModel();
			newBankAcc.accountName = "(Add new a bank account)";
			listBankAccount.Add(newBankAcc);
			               
			foreach (BankAccount acc in bankAccounts)
			{
				listBankAccount.Add(new AccountViewModel(acc));
			}

			// Add child data to Computer Section
			var listComputerAccount = new List<AccountViewModel>();

			var newComputerAcc = new AccountViewModel();
			newComputerAcc.accountName = "(Add new a computer account)";
			listComputerAccount.Add(newComputerAcc);

			foreach (ComputerAccount acc in computerAccounts)
			{
				listComputerAccount.Add(new AccountViewModel(acc));
			}

			// Add child data to Email Section
			var listEmailAccount = new List<AccountViewModel>();

			var newEmailAcc = new AccountViewModel();
			newEmailAcc.accountName = "(Add new an email account)";
			listEmailAccount.Add(newEmailAcc);

			foreach (EmailAccount acc in emailAccounts)
			{
				listEmailAccount.Add(new AccountViewModel(acc));
			}

			// Add child data to Web Section
			var listWebAccount = new List<AccountViewModel>();

			var newWebAcc = new AccountViewModel();
			newWebAcc.accountName = "(Add new a web account)";
			listWebAccount.Add(newWebAcc);

			foreach (WebAccount acc in webAccounts)
			{
				listWebAccount.Add(new AccountViewModel(acc));
			}

			// Add all to list account
			listAccount.Add(listCategory[0].categoryName, listBankAccount);
			listAccount.Add(listCategory[1].categoryName, listComputerAccount);
			listAccount.Add(listCategory[2].categoryName, listEmailAccount);
			listAccount.Add(listCategory[3].categoryName, listWebAccount);
		}

		void LogoutUser()
		{
			session.setLogin(false);
			session.setLoginUserId(0);

			// Launching the login activity
			var loginIntent = new Intent(this, typeof(LoginActivity));
			StartActivity(loginIntent);
			Finish();
		}
	}
}


