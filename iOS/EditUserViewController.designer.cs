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
    [Register ("EditUserViewController")]
    partial class EditUserViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnSave { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField confirmNewPassword { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField newPassword { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField oldPassword { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField userName { get; set; }

        [Action ("BtnSave_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnSave_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (btnSave != null) {
                btnSave.Dispose ();
                btnSave = null;
            }

            if (confirmNewPassword != null) {
                confirmNewPassword.Dispose ();
                confirmNewPassword = null;
            }

            if (newPassword != null) {
                newPassword.Dispose ();
                newPassword = null;
            }

            if (oldPassword != null) {
                oldPassword.Dispose ();
                oldPassword = null;
            }

            if (userName != null) {
                userName.Dispose ();
                userName = null;
            }
        }
    }
}