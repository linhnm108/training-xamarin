using System;
using UIKit;

namespace mPassword.iOS
{
    public partial class FlyMenuView : UIView
    {
		public FlyMenuView (IntPtr handle) : base (handle)
        {
        }

		partial void BtnLogout_TouchUpInside(UIButton sender)
		{
			//Create an instance of our AppDelegate
			var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;

			//Get an instance of our MainStoryboard.storyboard
			var mainStoryboard = appDelegate.MainStoryboard;

			//Get an instance of our Login Page View Controller
			var loginPageViewController = appDelegate.GetViewController(mainStoryboard, "LoginViewController") as LoginViewController;

			//Wire our event handler to show the MainTabBarController after we successfully logged in.
			loginPageViewController.OnLoginSuccess += (s, e) =>
			{
				var tabBarController = appDelegate.GetViewController(mainStoryboard, "MainScreenViewController");
				appDelegate.SetRootViewController(tabBarController, true);
			};

			//Set the Login Page as our RootViewController
			appDelegate.SetRootViewController(loginPageViewController, true);
		}
	}
}