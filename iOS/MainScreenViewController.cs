using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using mPassword.Shared;
using CoreAnimation;
using System.Drawing;

namespace mPassword.iOS
{
    public partial class MainScreenViewController : UITableViewController
    {
		const string AccCellIdentifier = "AccCell";
		const string HeaderCellIdentifier = "HeaderCell";

		List<AccountCategoryModel> accountCategories;

		bool hasInsertionRow;

		UITableViewRowAction[] editActions;

		AppDelegate appDelegate;

		AccountViewModel newAccount;

		UIBarButtonItem settingButton;

		FlyMenuView flyMenuView;

		public MainScreenViewController (IntPtr handle) : base (handle)
        {
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;

			// Init data
			GetAllUserAccounts();
			TableView.ReloadData();

			// Edit button
			NavigationItem.RightBarButtonItem = EditButtonItem;

			// Settings button
			settingButton = new UIBarButtonItem(UIImage.FromBundle("Menu"), UIBarButtonItemStyle.Plain, (sender, args) =>
			{
				// button was clicked
				PerformFlyMenu();
			});
			NavigationItem.LeftBarButtonItem = settingButton;

			// Create Fly Menu View
			flyMenuView = FlyMenuView.Create();
			flyMenuView.Hidden = true;
			flyMenuView.Frame = new RectangleF(0f, 0f, 240f, 140f);

			flyMenuView.EditUserButton.TouchUpInside += (sender, e) =>
			{
				PerformFlyMenu();
				PerformSegue("editUser", this);
			};

			// Pull to refresh
			var refreshControl = new UIRefreshControl();
			refreshControl.ValueChanged += (sender, e) =>
			{
				GetAllUserAccounts();
				BeginInvokeOnMainThread(() =>
				{
					TableView.ReloadData();
					refreshControl.EndRefreshing();
				});
			};
			RefreshControl = refreshControl;
		}

		void PerformFlyMenu()
		{
			flyMenuView.Hidden = !flyMenuView.Hidden;
			var transition = new CATransition();
			transition.Duration = 0.25f;
			transition.Type = CAAnimation.TransitionPush;
			if (flyMenuView.Hidden)
			{
				transition.TimingFunction = CAMediaTimingFunction.FromName(new NSString("easeOut"));
				transition.Subtype = CAAnimation.TransitionFromRight;
			}
			else
			{
				transition.TimingFunction = CAMediaTimingFunction.FromName(new NSString("easeIn"));
				transition.Subtype = CAAnimation.TransitionFromLeft;
			}
			flyMenuView.Layer.AddAnimation(transition, null);

			View.AddSubview(flyMenuView);
		}

		public override UITableViewRowAction[] EditActionsForRow(UITableView tableView, NSIndexPath indexPath)
		{
			if (editActions == null)
			{
				editActions = new[]
				{
					UITableViewRowAction.Create(UITableViewRowActionStyle.Destructive, "Delete", OnDelete)
				};
			}
			return editActions;
		}

		void OnDelete(UITableViewRowAction rowAction, NSIndexPath indexPath)
		{
			int row = hasInsertionRow ? indexPath.Row - 1 : indexPath.Row;

			AccountViewModel accountModel = accountCategories[indexPath.Section].accounts[row];

			if (string.Equals(accountModel.accountType, "BankAcc"))
			{
				BankAccountManager.DeleteBankAccount(accountModel.accountId);
			}
			else if (string.Equals(accountModel.accountType, "ComputerAcc"))
			{
				ComputerAccountManager.DeleteComputerAccount(accountModel.accountId);
			}
			else if (string.Equals(accountModel.accountType, "EmailAcc"))
			{
				EmailAccountManager.DeleteEmailAccount(accountModel.accountId);
			}
			else if (string.Equals(accountModel.accountType, "WebAcc"))
			{
				WebAccountManager.DeleteWebAccount(accountModel.accountId);
			}

			accountCategories[indexPath.Section].accounts.RemoveAt(row);
			accountCategories[indexPath.Section].quantity = accountCategories[indexPath.Section].accounts.Count;
			TableView.DeleteRows(new[] { indexPath }, UITableViewRowAnimation.Automatic);
			TableView.ReloadData();
		}

		public void GetAllUserAccounts()
		{
			accountCategories = new List<AccountCategoryModel>();
			accountCategories.Add(new AccountCategoryModel(BankAccountManager.GetBankAccountsByUserId(appDelegate.LoginUser.ID), false));
			accountCategories.Add(new AccountCategoryModel(ComputerAccountManager.GetComputerAccountsByUserId(appDelegate.LoginUser.ID), false));
			accountCategories.Add(new AccountCategoryModel(EmailAccountManager.GetEmailAccountsByUserId(appDelegate.LoginUser.ID), false));
			accountCategories.Add(new AccountCategoryModel(WebAccountManager.GetWebAccountsByUserId(appDelegate.LoginUser.ID), false)); 
		}

		[Export("tableView:titleForHeaderInSection:")]
		public override string TitleForHeader(UITableView tableView, nint section)
		{
			return accountCategories[(int)section].categoryName;
		}

		[Export("tableView:viewForHeaderInSection:")]
		public override UIView GetViewForHeader(UITableView tableView, nint section)
		{
			var header = tableView.DequeueReusableCell(HeaderCellIdentifier) as AccountCategoryCell;

			header.ToggleButton.Tag = section;
			header.TitleLabel.Text = accountCategories[(int)section].categoryName;
			header.QuantityLabel.Text = accountCategories[(int)section].quantity.ToString(); 

			header.ToggleButton.TouchUpInside += ToggleButton_TouchUpInside;

			return header.ContentView;
		}

		void ToggleButton_TouchUpInside(object sender, EventArgs e)
		{
			var toggleButton = (UIButton)sender;
			var section = (int)toggleButton.Tag;

			if (accountCategories[section].accounts.Count > 0)
			{
				var collapsed = accountCategories[section].collapsed;

				// Toggle collapse
				accountCategories[section].collapsed = !collapsed;

				// Reload section
				TableView.ReloadSections(new NSIndexSet((nuint)section), UITableViewRowAnimation.Automatic);
			}
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell(AccCellIdentifier, indexPath) as UITableViewCell;
			cell.TextLabel.TextColor = UIColor.Gray;
			int row = indexPath.Row;

			if (hasInsertionRow)
			{
				if (row == 0)
				{
					var category = accountCategories[indexPath.Section];

					if (string.Equals(category.categoryName, "Bank Accounts"))
					{
						cell.TextLabel.Text = "Add new a bank account";
					}
					else if (string.Equals(category.categoryName, "Computer Accounts"))
					{
						cell.TextLabel.Text = "Add new a computer account";
					}
					else if (string.Equals(category.categoryName, "Email Accounts"))
					{
						cell.TextLabel.Text = "Add new an email account";
					}
					else if (string.Equals(category.categoryName, "Web Accounts"))
					{
						cell.TextLabel.Text = "Add new a web account";
					}

					//cell.DetailTextLabel.Text = string.Empty;
					return cell;
				}
				row--;
			}
			cell.TextLabel.Text = accountCategories[indexPath.Section].accounts[row].accountName;

			return cell;
		}

		public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
		{
			return true;
		}

		public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
		{
			if (hasInsertionRow && indexPath.Row == 0)
			{
				return UITableViewCellEditingStyle.Insert;
			}
			return UITableViewCellEditingStyle.Delete;
		}

		public override void WillBeginEditing(UITableView tableView, NSIndexPath indexPath)
		{
			if (hasInsertionRow && accountCategories[indexPath.Section].collapsed)
			{
				hasInsertionRow = false;
				var selectedSection = indexPath.Section;
				var selectedIndexPath = NSIndexPath.FromRowSection(0, selectedSection);
				TableView.DeleteRows(new[] { selectedIndexPath }, UITableViewRowAnimation.Automatic);
			}
		}

		public override void SetEditing(bool editing, bool animated)
		{
			base.SetEditing(editing, animated);

			var atIndexPaths = new List<NSIndexPath>();
			for (int i = 0; i < accountCategories.Count; i++)
			{
				NSIndexPath indexPath = NSIndexPath.FromRowSection(0, i);
				if (accountCategories[indexPath.Section].collapsed)
				{
					continue;
				}
				atIndexPaths.Add(NSIndexPath.FromRowSection(0, i));
			}

			if (editing)
			{
				hasInsertionRow = true;
				TableView.InsertRows(atIndexPaths.ToArray(), UITableViewRowAnimation.Automatic);
			}
			else if (hasInsertionRow)
			{
				hasInsertionRow = false;
				TableView.DeleteRows(atIndexPaths.ToArray(), UITableViewRowAnimation.Automatic);
			}
		}

		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			switch (segue.Identifier)
			{
				case "showBankAccDetails":
					var bankAccDetailViewController = segue.DestinationViewController as BankAccDetailsViewController;

					if (bankAccDetailViewController != null)
					{
						var selectedAccount = newAccount;

						if (selectedAccount == null)
						{
							selectedAccount = accountCategories[TableView.IndexPathForSelectedRow.Section].accounts[TableView.IndexPathForSelectedRow.Row];
						}
						bankAccDetailViewController.SelectedAccViewModel = selectedAccount;
					}
					break;

				case "showComputerAccDetails":
					var computerAccDetailViewController = segue.DestinationViewController as ComputerAccDetailsViewController;
					if (computerAccDetailViewController != null)
					{
						var selectedAccount = newAccount;

						if (selectedAccount == null)
						{
							selectedAccount = accountCategories[TableView.IndexPathForSelectedRow.Section].accounts[TableView.IndexPathForSelectedRow.Row];
						}
						computerAccDetailViewController.SelectedAccViewModel = selectedAccount;
					}

					break;
					
				case "showEmailAccDetails":
					var emailAccDetailViewController = segue.DestinationViewController as EmailAccDetailsViewController;

					if (emailAccDetailViewController != null)
					{
						var selectedAccount = newAccount;

						if (selectedAccount == null)
						{
							selectedAccount = accountCategories[TableView.IndexPathForSelectedRow.Section].accounts[TableView.IndexPathForSelectedRow.Row];
						}
						emailAccDetailViewController.SelectedAccViewModel = selectedAccount;
					}
					break;
					
				case "showWebAccDetails":
					var webAccDetailViewController = segue.DestinationViewController as WebAccDetailsViewController;
					if (webAccDetailViewController != null)
					{
						var selectedAccount = newAccount;

						if (selectedAccount == null)
						{
							selectedAccount = accountCategories[TableView.IndexPathForSelectedRow.Section].accounts[TableView.IndexPathForSelectedRow.Row];
						}
						webAccDetailViewController.SelectedAccViewModel = selectedAccount;
					}
					break;
			}
		}

		public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
		{
			int row = hasInsertionRow ? indexPath.Row - 1 : indexPath.Row;
			if (editingStyle == UITableViewCellEditingStyle.Delete)
			{
				AccountViewModel accountModel = accountCategories[indexPath.Section].accounts[row];

				if (string.Equals(accountModel.accountType, "BankAcc"))
				{
					BankAccountManager.DeleteBankAccount(accountModel.accountId);
				}
				else if (string.Equals(accountModel.accountType, "ComputerAcc"))
				{
					ComputerAccountManager.DeleteComputerAccount(accountModel.accountId);
				}
				else if (string.Equals(accountModel.accountType, "EmailAcc"))
				{
					EmailAccountManager.DeleteEmailAccount(accountModel.accountId);
				}
				else if (string.Equals(accountModel.accountType, "WebAcc"))
				{
					WebAccountManager.DeleteWebAccount(accountModel.accountId);
				}

				accountCategories[indexPath.Section].accounts.RemoveAt(indexPath.Row);
				tableView.DeleteRows(new[] { indexPath }, UITableViewRowAnimation.Automatic);

			}
			else if (editingStyle == UITableViewCellEditingStyle.Insert)
			{
				OnAddNewAccount(this, EventArgs.Empty, indexPath.Section);
			}
		}

		void OnAddNewAccount(object sender, EventArgs e, int section)
		{
			var category = accountCategories[section];
			newAccount = new AccountViewModel();

			if (string.Equals(category.categoryName, "Bank Accounts"))
			{
				PerformSegue("showBankAccDetails", this);
			}
			else if (string.Equals(category.categoryName, "Computer Accounts"))
			{
				PerformSegue("showComputerAccDetails", this);
			}
			else if (string.Equals(category.categoryName, "Email Accounts"))
			{
				PerformSegue("showEmailAccDetails", this);
			}
			else if (string.Equals(category.categoryName, "Web Accounts"))
			{
				PerformSegue("showWebAccDetails", this);
			}
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			AccountViewModel accountModel = accountCategories[indexPath.Section].accounts[indexPath.Row];

			if (string.Equals(accountModel.accountType, "BankAcc"))
			{
				PerformSegue("showBankAccDetails", this);
			}
			else if (string.Equals(accountModel.accountType, "ComputerAcc"))
			{
				PerformSegue("showComputerAccDetails", this);
			}
			else if (string.Equals(accountModel.accountType, "EmailAcc"))
			{
				PerformSegue("showEmailAccDetails", this);
			}
			else if (string.Equals(accountModel.accountType, "WebAcc"))
			{
				PerformSegue("showWebAccDetails", this);
			}
		}

		public override nint RowsInSection(UITableView tableView, nint section)
		{
			var position = (int)section;

			// Adjust the index for our data to account for the "fake" row.
			if (hasInsertionRow)
			{
				return accountCategories[position].collapsed ? 0 : accountCategories[position].accounts.Count + 1;
			}

			return accountCategories[position].collapsed ? 0 : accountCategories[position].accounts.Count;
		}

		public override nint NumberOfSections(UITableView tableView)
		{
			return accountCategories.Count;
		}

		public override void ViewWillAppear(bool animated)
		{
			if (newAccount != null)
			{
				if (newAccount.accountId != 0)
				{
					if (string.Equals(newAccount.accountType, "BankAcc"))
					{
						AddNewAccount("Bank Accounts", newAccount);
					}
					else if (string.Equals(newAccount.accountType, "ComputerAcc"))
					{
						AddNewAccount("Computer Accounts", newAccount);	
					}
					else if (string.Equals(newAccount.accountType, "EmailAcc"))
					{
						AddNewAccount("Email Accounts", newAccount);
					}
					else if (string.Equals(newAccount.accountType, "WebAcc"))
					{
						AddNewAccount("Web Accounts", newAccount);
					} 
				}
				newAccount = null;
			}
			TableView.ReloadData();
		}

		void AddNewAccount(string categoryName, AccountViewModel newAcc)
		{
			foreach (AccountCategoryModel category in accountCategories)
			{
				if (category.categoryName == categoryName)
				{
					category.accounts.Add(newAcc);
					category.quantity = category.accounts.Count;
				}
			}
		}
    }
}