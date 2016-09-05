using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using mPassword.Shared;

namespace mPassword.iOS
{
    public partial class EmailAccTableViewController : UITableViewController
    {

		const string CellIdentifier = "EmailAccCell";

		EmailAccount newEmailAcc;

		List<EmailAccount> emailAccounts;

		bool hasInsertionRow;

		UITableViewRowAction[] editActions;

		public EmailAccTableViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;

			NavigationItem.RightBarButtonItem = EditButtonItem;

			emailAccounts = new List<EmailAccount>();

			emailAccounts.AddRange(EmailAccountManager.GetEmailAccountsByUserId(appDelegate.LoginUser.ID));
			TableView.ReloadData();

			// Pull to refres
			var refreshControl = new UIRefreshControl();
			refreshControl.ValueChanged += (sender, e) =>
			{
				emailAccounts = EmailAccountManager.GetEmailAccountsByUserId(appDelegate.LoginUser.ID) as List<EmailAccount>;
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

			EmailAccount emailAcc = emailAccounts[row];
			emailAccounts.RemoveAt(row);
			TableView.DeleteRows(new[] { indexPath }, UITableViewRowAnimation.Automatic);
			EmailAccountManager.DeleteEmailAccount(emailAcc.ID);
		}

		public override nint RowsInSection(UITableView tableView, nint section)
		{
			// Adjust the index for our data to account for the "fake" row
			if (hasInsertionRow)
			{
				return emailAccounts.Count + 1;
			}

			return emailAccounts.Count;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell(CellIdentifier, indexPath);

			int row = indexPath.Row;

			if (hasInsertionRow)
			{
				if (row == 0)
				{
					cell.TextLabel.Text = "Add new an email account";
					//cell.DetailTextLabel.Text = string.Empty
					cell.TextLabel.TextColor = UIColor.Gray;
					return cell;
				}
				row--;
			}

			var emailAcc = emailAccounts[row];

			cell.TextLabel.Text = emailAcc.Name;
			//cell.DetailTextLabel.Text = string.Empty;

			return cell;
		}

		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == "showDetail")
			{
				var detailViewController = segue.DestinationViewController as EmailAccDetailsViewController;
				if (detailViewController != null)
				{
					var selectedAccount = newEmailAcc;

					if (selectedAccount == null)
					{
						selectedAccount = emailAccounts[TableView.IndexPathForSelectedRow.Row];
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
				var emailAcc = emailAccounts[row];

				EmailAccountManager.DeleteEmailAccount(emailAcc.ID);

				emailAccounts.RemoveAt(indexPath.Row);
				tableView.DeleteRows(new[] { indexPath }, UITableViewRowAnimation.Automatic);

			}
			else if (editingStyle == UITableViewCellEditingStyle.Insert)
			{
				OnAddEmailAccount(this, EventArgs.Empty);
			}
		}

		public override void ViewWillAppear(bool animated)
		{
			if (newEmailAcc != null)
			{
				if (newEmailAcc.ID != 0)
				{
					emailAccounts.Add(newEmailAcc);
					TableView.ReloadData();
				}
				newEmailAcc = null;
			}
		}

		void OnAddEmailAccount(object sender, EventArgs e)
		{
			// Create a new expense
			newEmailAcc = new EmailAccount();
			PerformSegue("showDetail", this);
		}
    }
}