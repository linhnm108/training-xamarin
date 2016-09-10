// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace mPassword.iOS
{
    [Register ("AccountCell")]
    partial class AccountCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnExpiredWarning { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel subTitleLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnExpiredWarning != null) {
                btnExpiredWarning.Dispose ();
                btnExpiredWarning = null;
            }

            if (subTitleLabel != null) {
                subTitleLabel.Dispose ();
                subTitleLabel = null;
            }
        }
    }
}