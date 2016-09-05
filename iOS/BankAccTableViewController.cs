using System;
using UIKit;
using System.Collections.Generic;
using mPassword.Shared;
using Foundation;

namespace mPassword.iOS
{
    public partial class BankAccTableViewController : UITableViewController
    {
		const string CellIdentifier = "BankAccCell";

		BankAccount newBankAcc;

		List<BankAccount> bankAccounts;

		bool hasInsertionRow;

		UITableViewRowAction[] editActions;

		public BankAccTableViewController (IntPtr handle) : base (handle)
        {
        }

		public override void ViewDidLoad()
		{
			var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;

			NavigationItem.RightBarButtonItem = EditButtonItem;

			bankAccounts = new List<BankAccount>();

			bankAccounts.AddRange(BankAccountManager.GetBankAccountsByUserId(appDelegate.LoginUser.ID));
			TableView.ReloadData();

			// Pull to refresh
			var refreshControl = new UIRefreshControl();
			refreshControl.ValueChanged += (sender, e) =>
			{
				bankAccounts = BankAccountManager.GetBankAccountsByUserId(appDelegate.LoginUser.ID) as List<BankAccount>;
				BeginInvokeOnMainThread(() =>
				{
					TableView.ReloadData();
					refreshControl.EndRefreshing();
				});
			};
			RefreshControl = refreshControl;
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

			BankAccount bankAcc = bankAccounts[row];
			bankAccounts.RemoveAt(row);
			TableView.DeleteRows(new[] { indexPath }, UITableViewRowAnimation.Automatic);
			BankAccountManager.DeleteBankAccount(bankAcc.ID);
		}

		public override nint RowsInSection(UITableView tableView, nint section)
		{
			// Adjust the index for our data to account for the "fake" row.
			if (hasInsertionRow)
			{
				return bankAccounts.Count + 1;
			}

			return bankAccounts.Count;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell(CellIdentifier, indexPath);

			int row = indexPath.Row;

			if (hasInsertionRow)
			{
				if (row == 0)
				{
					cell.TextLabel.Text = "Add new a bank account";
					//cell.DetailTextLabel.Text = string.Empty;
					cell.TextLabel.TextColor = UIColor.Gray;
					return cell;
				}
				row--;
			}

			var bankAcc = bankAccounts[row];

			cell.TextLabel.Text = bankAcc.Name;
			//cell.DetailTextLabel.Text = string.Empty;

			return cell;
		}

		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == "showDetail")
			{
				var detailViewController = segue.DestinationViewController as BankAccDetailsViewController;
				if (detailViewController != null)
				{
					var selectedAccount = newBankAcc;

					if (selectedAccount == null)
					{
						selectedAccount = bankAccounts[TableView.IndexPathForSelectedRow.Row];
					}
					detailViewController.SelectedAccount = selectedAccount;
				}
			}
		}

		public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
		{
			return true;
		}

		public override UITableViewCellEditingStyle EditingStyleForRow(
		   UITableView tableView, NSIndexPath indexPath)
		{
			if (hasInsertionRow && indexPath.Row == 0)
			{
				return UITableViewCellEditingStyle.Insert;
			}
			return UITableViewCellEditingStyle.Delete;
		}

		public override void WillBeginEditing(UITableView tableView, NSIndexPath indexPath)
		{
			base.WillBeginEditing(tableView, indexPath);

			if (hasInsertionRow)
			{
				hasInsertionRow = false;
				using (indexPath = NSIndexPath.FromRowSection(0, 0))
				{
					TableView.DeleteRows(new[] { indexPath }, UITableViewRowAnimation.Automatic);
				}
			}
		}

		public override void SetEditing(bool editing, bool animated)
		{
			base.SetEditing(editing, animated);

			using (var indexPath = NSIndexPath.FromRowSection(0, 0))
			{
				if (editing)
				{
					hasInsertionRow = true;
					TableView.InsertRows(new[] { indexPath }, UITableViewRowAnimation.Automatic);

				}
				else if (hasInsertionRow)
				{
					hasInsertionRow = false;
					TableView.DeleteRows(new[] { indexPath }, UITableViewRowAnimation.Automatic);
				}
			}
		}

		public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
		{
			int row = hasInsertionRow ? indexPath.Row - 1 : indexPath.Row;
			if (editingStyle == UITableViewCellEditingStyle.Delete)
			{
				var bankAcc = bankAccounts[row];

				BankAccountManager.DeleteBankAccount(bankAcc.ID);

				bankAccounts.RemoveAt(indexPath.Row);
				tableView.DeleteRows(new[] { indexPath }, UITableViewRowAnimation.Automatic);

			}
			else if (editingStyle == UITableViewCellEditingStyle.Insert)
			{
				OnAddBankAccount(this, EventArgs.Empty);
			}
		}

		public override void ViewWillAppear(bool animated)
		{
			if (newBankAcc != null)
			{
				if (newBankAcc.ID != 0)
				{
					bankAccounts.Add(newBankAcc);
					TableView.ReloadData();
				}
				newBankAcc = null;
			}
		}

		void OnAddBankAccount(object sender, EventArgs e)
		{
			// Create a new expense.
			newBankAcc = new BankAccount();
			PerformSegue("showDetail", this);
		}
    }
}