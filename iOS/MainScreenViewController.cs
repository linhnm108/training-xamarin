using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using mPassword.Shared;

namespace mPassword.iOS
{
    public partial class MainScreenViewController : UITableViewController
    {
		List<AccountCategoryModel> accountCategories;

		public MainScreenViewController (IntPtr handle) : base (handle)
        {
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;

			accountCategories = new List<AccountCategoryModel>();
			accountCategories.Add(new AccountCategoryModel(BankAccountManager.GetBankAccountsByUserId(appDelegate.LoginUser.ID), false)); 
			accountCategories.Add(new AccountCategoryModel(ComputerAccountManager.GetComputerAccountsByUserId(appDelegate.LoginUser.ID), false));
			accountCategories.Add(new AccountCategoryModel(EmailAccountManager.GetEmailAccountsByUserId(appDelegate.LoginUser.ID), false));
			accountCategories.Add(new AccountCategoryModel(WebAccountManager.GetWebAccountsByUserId(appDelegate.LoginUser.ID), false));

			NavigationItem.RightBarButtonItem = EditButtonItem;
		}

		[Export("tableView:titleForHeaderInSection:")]
		public override string TitleForHeader(UITableView tableView, nint section)
		{
			return accountCategories[(int)section].categoryName;
		}

		[Export("tableView:viewForHeaderInSection:")]
		public override UIView GetViewForHeader(UITableView tableView, nint section)
		{
			var header = tableView.DequeueReusableCell("header") as AccountCategoryCell;

			header.toggleButton.Tag = section;
			header.titleLabel.Text = accountCategories[(int)section].categoryName;
			header.quantityLabel.Text = accountCategories[(int)section].quantity.ToString(); 

			header.toggleButton.TouchUpInside += ToggleButton_TouchUpInside;

			return header.ContentView;
		}

		void ToggleButton_TouchUpInside(object sender, EventArgs e)
		{
			var toggleButton = (UIButton)sender;
			var section = (int)toggleButton.Tag;

			var collapsed = accountCategories[section].collapsed;

			// Toggle collapse
			accountCategories[section].collapsed = !collapsed;

			// Reload section
			TableView.ReloadSections(new NSIndexSet((nuint)section), UITableViewRowAnimation.Automatic);
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell("cell", indexPath) as UITableViewCell;
			cell.TextLabel.Text = accountCategories[indexPath.Section].accounts[indexPath.Row].accountName;
			return cell;
		}

		public override nint RowsInSection(UITableView tableView, nint section)
		{
			var position = (int)section;
			return accountCategories[position].collapsed ? 0 : accountCategories[position].accounts.Count;
		}

		public override nint NumberOfSections(UITableView tableView)
		{
			return accountCategories.Count;
		}
    }
}