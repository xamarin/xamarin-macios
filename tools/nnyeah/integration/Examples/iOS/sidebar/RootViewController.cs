using System;
using System.Drawing;
using UIKit;
using Foundation;
using CoreGraphics;
using SidebarNavigation;

namespace StoryboardSample
{
	public partial class RootViewController : UIViewController
	{
		private UIStoryboard? _storyboard;


		// the sidebar controller for the app
		public SidebarNavigation.SidebarController SidebarController { get; private set; }

		// the navigation controller
		public NavController NavController { get; private set; }

		// the storyboard
		public override UIStoryboard Storyboard {
			get {
				if (_storyboard is null)
					_storyboard = UIStoryboard.FromName("Phone", null);
				return _storyboard;
			}
		}

		public RootViewController() : base(null, null)
		{

		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var introController = (IntroController)Storyboard.InstantiateViewController("IntroController");
			var menuController = (MenuController)Storyboard.InstantiateViewController("MenuController");

			// create a slideout navigation controller with the top navigation controller and the menu view controller
			NavController = new NavController();
			NavController.PushViewController(introController, false);
			SidebarController = new SidebarNavigation.SidebarController(this, NavController, menuController);
			SidebarController.MenuWidth = 220;
			SidebarController.ReopenOnRotate = false;
		}
	}
}

