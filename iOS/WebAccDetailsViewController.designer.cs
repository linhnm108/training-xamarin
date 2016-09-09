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
    [Register ("WebAccDetailsViewController")]
    partial class WebAccDetailsViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField accountName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnSave { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField note { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField password { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField url { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField userName { get; set; }

        [Action ("BtnSave_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnSave_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (accountName != null) {
                accountName.Dispose ();
                accountName = null;
            }

            if (btnSave != null) {
                btnSave.Dispose ();
                btnSave = null;
            }

            if (note != null) {
                note.Dispose ();
                note = null;
            }

            if (password != null) {
                password.Dispose ();
                password = null;
            }

            if (url != null) {
                url.Dispose ();
                url = null;
            }

            if (userName != null) {
                userName.Dispose ();
                userName = null;
            }
        }
    }
}