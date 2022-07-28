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

namespace StoryboardSample
{
	[Register ("MenuController")]
	partial class MenuController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton ContentButton { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (ContentButton is not null) {
				ContentButton.Dispose ();
				ContentButton = null;
			}
		}
	}
}
