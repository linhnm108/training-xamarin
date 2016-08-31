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
    [Register ("SignUpViewController")]
    partial class SignUpViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField ConfirmPasswordTextView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField PasswordTextView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField UserNameTextView { get; set; }

        [Action ("Cancel:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void Cancel (UIKit.UIButton sender);

        [Action ("SignUp:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SignUp (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (ConfirmPasswordTextView != null) {
                ConfirmPasswordTextView.Dispose ();
                ConfirmPasswordTextView = null;
            }

            if (PasswordTextView != null) {
                PasswordTextView.Dispose ();
                PasswordTextView = null;
            }

            if (UserNameTextView != null) {
                UserNameTextView.Dispose ();
                UserNameTextView = null;
            }
        }
    }
}