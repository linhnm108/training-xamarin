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
    [Register ("BankAccDetailsViewController")]
    partial class BankAccDetailsViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField accountName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField accountNumber { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField atmPassword { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField iBankPassword { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField iBankUser { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField note { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField passwordExpiredDuration { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField securityAnswer { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField securityQuestion { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField updatedDate { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (accountName != null) {
                accountName.Dispose ();
                accountName = null;
            }

            if (accountNumber != null) {
                accountNumber.Dispose ();
                accountNumber = null;
            }

            if (atmPassword != null) {
                atmPassword.Dispose ();
                atmPassword = null;
            }

            if (iBankPassword != null) {
                iBankPassword.Dispose ();
                iBankPassword = null;
            }

            if (iBankUser != null) {
                iBankUser.Dispose ();
                iBankUser = null;
            }

            if (note != null) {
                note.Dispose ();
                note = null;
            }

            if (passwordExpiredDuration != null) {
                passwordExpiredDuration.Dispose ();
                passwordExpiredDuration = null;
            }

            if (securityAnswer != null) {
                securityAnswer.Dispose ();
                securityAnswer = null;
            }

            if (securityQuestion != null) {
                securityQuestion.Dispose ();
                securityQuestion = null;
            }

            if (updatedDate != null) {
                updatedDate.Dispose ();
                updatedDate = null;
            }
        }
    }
}