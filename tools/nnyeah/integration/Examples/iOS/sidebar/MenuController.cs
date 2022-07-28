using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using ObjCRuntime;

namespace StoryboardSample
{
	partial class MenuController : BaseController
	{
		public MenuController (NativeHandle handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var contentController = (ContentController)Storyboard.InstantiateViewController("ContentController");

			ContentButton.TouchUpInside += (o, e) => {
				if (NavController.TopViewController as ContentController is null)
					NavController.PushViewController(contentController, false);
				SidebarController.CloseMenu();
			};
		}
	}
}
