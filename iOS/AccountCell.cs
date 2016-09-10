using Foundation;
using System;
using UIKit;

namespace mPassword.iOS
{
    public partial class AccountCell : UITableViewCell
    {
		public AccountCell (IntPtr handle) : base (handle)
        {
        }

		public UIButton WarningButton
		{
			set { btnExpiredWarning = value; }
			get { return btnExpiredWarning; }
		}

		public UILabel SubTitleLabel
		{
			set { subTitleLabel = value; }
			get { return subTitleLabel; }
		}
	}
}