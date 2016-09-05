using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using mPassword.Shared;

namespace mPassword.iOS
{
    public partial class ComputerAccTableViewController : UITableViewController
    {
		const string CellIdentifier = "ComputerAccCell";

		ComputerAccount newComputerAcc;

		List<ComputerAccount> computerAccounts;

		bool hasInsertionRow;

		UITableViewRowAction[] editActions;

		public ComputerAccTableViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;

			NavigationItem.RightBarButtonItem = EditButtonItem;

			computerAccounts = new List<ComputerAccount>();

			computerAccounts.AddRange(ComputerAccountManager.GetComputerAccountsByUserId(appDelegate.LoginUser.ID));
			TableView.ReloadData();

			// Pull to refres
			var refreshControl = new UIRefreshControl();
			refreshControl.ValueChanged += (sender, e) =>
			{
				computerAccounts = ComputerAccountManager.GetComputerAccountsByUserId(appDelegate.LoginUser.ID) as List<ComputerAccount>;
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

			ComputerAccount computerAcc = computerAccounts[row];
			computerAccounts.RemoveAt(row);
			TableView.DeleteRows(new[] { indexPath }, UITableViewRowAnimation.Automatic);
			ComputerAccountManager.DeleteComputerAccount(computerAcc.ID);
		}

		public override nint RowsInSection(UITableView tableView, nint section)
		{
			// Adjust the index for our data to account for the "fake" row
			if (hasInsertionRow)
			{
				return computerAccounts.Count + 1;
			}

			return computerAccounts.Count;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell(CellIdentifier, indexPath);

			int row = indexPath.Row;

			if (hasInsertionRow)
			{
				if (row == 0)
				{
					cell.TextLabel.Text = "Add new a computer account";
					//cell.DetailTextLabel.Text = string.Empty
					cell.TextLabel.TextColor = UIColor.Gray;
					return cell;
				}
				row--;
			}

			var computerAcc = computerAccounts[row];

			cell.TextLabel.Text = computerAcc.Name;
			//cell.DetailTextLabel.Text = string.Empty;

			return cell;
		}

		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == "showDetail")
			{
				var detailViewController = segue.DestinationViewController as ComputerAccDetailsViewController;
				if (detailViewController != null)
				{
					var selectedAccount = newComputerAcc;

					if (selectedAccount == null)
					{
						selectedAccount = computerAccounts[TableView.IndexPathForSelectedRow.Row];
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
				var computerAcc = computerAccounts[row];

				ComputerAccountManager.DeleteComputerAccount(computerAcc.ID);

				computerAccounts.RemoveAt(indexPath.Row);
				tableView.DeleteRows(new[] { indexPath }, UITableViewRowAnimation.Automatic);

			}
			else if (editingStyle == UITableViewCellEditingStyle.Insert)
			{
				OnAddComputerAccount(this, EventArgs.Empty);
			}
		}

		public override void ViewWillAppear(bool animated)
		{
			if (newComputerAcc != null)
			{
				if (newComputerAcc.ID != 0)
				{
					computerAccounts.Add(newComputerAcc);
					TableView.ReloadData();
				}
				newComputerAcc = null;
			}
		}

		void OnAddComputerAccount(object sender, EventArgs e)
		{
			// Create a new expense
			newComputerAcc = new ComputerAccount();
			PerformSegue("showDetail", this);
		}
    }
}