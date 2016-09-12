using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using mPassword.Shared;

namespace mPassword.Droid
{
	public class AccountListAdapter : BaseExpandableListAdapter
	{
		Context context;
		readonly List<CategoryViewModel> listCategory;
		Dictionary<string, List<AccountViewModel>> listAccount;


		public AccountListAdapter(Context context, List<CategoryViewModel> listCategory, Dictionary<string, List<AccountViewModel>> listAccount)
		{
			this.context = context;
			this.listCategory = listCategory;
			this.listAccount = listAccount;
		}

		public override int GroupCount
		{
			get
			{
				return listCategory.Count;
			}
		}

		public override bool HasStableIds
		{
			get
			{
				return false;
			}
		}

		public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
		{
			//return  listAccount[listCategory[groupPosition]];
			return null;
		}

		public override long GetChildId(int groupPosition, int childPosition)
		{
			return childPosition;
		}

		public override int GetChildrenCount(int groupPosition)
		{
			return listAccount[listCategory[groupPosition].categoryName].Count;
		}

		public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
		{
			//string childText = (String) getChild(groupPosition, childPosition);

			if (convertView == null)
			{
				var infalInflater = (LayoutInflater) context.GetSystemService(Context.LayoutInflaterService);
				convertView = infalInflater.Inflate(Resource.Layout.AccountRow, null);
			}

			var txtListChild = convertView.FindViewById<TextView>(Resource.Id.lblListItem);
			var warningButton = convertView.FindViewById<ImageButton>(Resource.Id.imgButtonWarning);

			AccountViewModel account = GetAccountByPosition(groupPosition, childPosition);
			string childText = account.accountName;

			if (account.isExpiredWarning)
			{
				warningButton.Visibility = ViewStates.Visible;
				warningButton.Click += (sender, e) =>
				{
					Toast.MakeText(context, Constants.WARNING_PASSWORD_EXPIRED_MESSAGE
					               .Replace("{expired_date}", account.expiredDate), ToastLength.Short).Show();
				};
			}
			else
			{
				warningButton.Visibility = ViewStates.Invisible;
			}

			txtListChild.Text = childText;

			return convertView;
		}

		public override Java.Lang.Object GetGroup(int groupPosition)
		{
			return null;
		}

		public override long GetGroupId(int groupPosition)
		{
			return groupPosition;
		}

		public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
		{
			if (convertView == null)
			{
				var infalInflater = (LayoutInflater) context.GetSystemService(Context.LayoutInflaterService);
				convertView = infalInflater.Inflate(Resource.Layout.CategoryRow, null);
			}

			var lblCategoryName = convertView.FindViewById<TextView>(Resource.Id.lblCategoryName);
			var lblQuantity = convertView.FindViewById<TextView>(Resource.Id.lblQuantity);
			lblCategoryName.Typeface = Typeface.Create(string.Empty, TypefaceStyle.Bold);

			CategoryViewModel category = GetCategoryByPosition(groupPosition);

			lblCategoryName.Text = category.categoryName;
			lblQuantity.Text = category.quantity.ToString();

			return convertView;
		}

		public override bool IsChildSelectable(int groupPosition, int childPosition)
		{
			return true;
		}

		public AccountViewModel GetAccountByPosition(int groupPosition, int childPosition)
		{
			return listAccount[listCategory[groupPosition].categoryName][childPosition];
		}

		public CategoryViewModel GetCategoryByPosition(int groupPosition)
		{
			return listCategory[groupPosition];
		}
	}
}

