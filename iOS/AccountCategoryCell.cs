using System;
using UIKit;

namespace mPassword.iOS
{
    public partial class AccountCategoryCell : UITableViewCell
    {
        public AccountCategoryCell (IntPtr handle) : base (handle)
        {
        }

		public UIButton ToggleButton
		{
			set { toggleButton = value; }
			get { return toggleButton; }
		}

		public UILabel TitleLabel
		{
			set { titleLabel = value; }
			get { return titleLabel; }
		}

		public UILabel QuantityLabel
		{
			set { quantityLabel = value; }
			get { return quantityLabel; }
		}
    }
}