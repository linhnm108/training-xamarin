using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using mPassword.Shared;

namespace mPassword.iOS
{
    public partial class WebAccTableViewController : UITableViewController
    {

		const string CellIdentifier = "WebAccCell";

		WebAccount newWebAcc;

		List<WebAccount> webAccounts;

		bool hasInsertionRow;

		UITableViewRowAction[] editActions;

		public WebAccTableViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;

			NavigationItem.RightBarButtonItem = EditButtonItem;

			webAccounts = new List<WebAccount>();

			webAccounts.AddRange(WebAccountManager.GetWebAccountsByUserId(appDelegate.LoginUser.ID));
			TableView.ReloadData();

			// Pull to refre
			var refreshControl = new UIRefreshControl();
			refreshControl.ValueChanged += (sender, e) =>
			{
				webAccounts = WebAccountManager.GetWebAccountsByUserId(appDelegate.LoginUser.ID) as List<WebAccount>;
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

			WebAccount webAcc = webAccounts[row];
			webAccounts.RemoveAt(row);
			TableView.DeleteRows(new[] { indexPath }, UITableViewRowAnimation.Automatic);
			WebAccountManager.DeleteWebAccount(webAcc.ID);
		}

		public override nint RowsInSection(UITableView tableView, nint section)
		{
			// Adjust the index for our data to account for the "fake" ro
			if (hasInsertionRow)
			{
				return webAccounts.Count + 1;
			}

			return webAccounts.Count;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell(CellIdentifier, indexPath);

			int row = indexPath.Row;

			if (hasInsertionRow)
			{
				if (row == 0)
				{
					cell.TextLabel.Text = "Add new a web account";
					//cell.DetailTextLabel.Text = string.Empt
					cell.TextLabel.TextColor = UIColor.Gray;
					return cell;
				}
				row--;
			}

			var webAcc = webAccounts[row];

			cell.TextLabel.Text = webAcc.Name;
			//cell.DetailTextLabel.Text = string.Empty;

			return cell;
		}

		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == "showDetail")
			{
				var detailViewController = segue.DestinationViewController as WebAccDetailsViewController;
				if (detailViewController != null)
				{
					var selectedAccount = newWebAcc;

					if (selectedAccount == null)
					{
						selectedAccount = webAccounts[TableView.IndexPathForSelectedRow.Row];
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
				var webAcc = webAccounts[row];

				WebAccountManager.DeleteWebAccount(webAcc.ID);

				webAccounts.RemoveAt(indexPath.Row);
				tableView.DeleteRows(new[] { indexPath }, UITableViewRowAnimation.Automatic);

			}
			else if (editingStyle == UITableViewCellEditingStyle.Insert)
			{
				OnAddWebAccount(this, EventArgs.Empty);
			}
		}

		public override void ViewWillAppear(bool animated)
		{
			if (newWebAcc != null)
			{
				if (newWebAcc.ID != 0)
				{
					webAccounts.Add(newWebAcc);
					TableView.ReloadData();
				}
				newWebAcc = null;
			}
		}

		void OnAddWebAccount(object sender, EventArgs e)
		{
			// Create a new expens
			newWebAcc = new WebAccount();
			PerformSegue("showDetail", this);
		}
    }
}