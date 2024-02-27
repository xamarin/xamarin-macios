//
// Unit tests for QLPreviewController
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
using CoreGraphics;
using Foundation;
using UIKit;
using QuickLook;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.QuickLook {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PreviewControllerTest {

		[Test]
		public void Defaults ()
		{
			using (QLPreviewController pc = new QLPreviewController ()) {
				Assert.Null (pc.CurrentPreviewItem, "CurrentPreviewItem");
				nint index = 0;
#if !__MACCATALYST__
				if (TestRuntime.CheckSystemVersion (ApplePlatform.iOS, 10, 0))
					index = nint.MaxValue;
				else if (TestRuntime.CheckSystemVersion (ApplePlatform.iOS, 7, 1))
					index = -1;
#endif
				Assert.That (pc.CurrentPreviewItemIndex, Is.EqualTo (index), "CurrentPreviewItemIndex");

				Assert.Null (pc.Delegate, "Delegate");
				Assert.Null (pc.WeakDelegate, "WeakDelegate");

				Assert.Null (pc.DataSource, "DataSource");
				Assert.Null (pc.WeakDataSource, "WeakDataSource");

				pc.RefreshCurrentPreviewItem ();
				pc.ReloadData ();
			}
		}

		[Test]
		public void DelegateEvents ()
		{
			using (QLPreviewController pc = new QLPreviewController ()) {
				pc.ShouldOpenUrl += delegate (QLPreviewController controller, NSUrl url, IQLPreviewItem item)
				{
					return false;
				};
				pc.FrameForPreviewItem += delegate
				{
					return new CGRect (1, 2, 3, 4);
				};
				pc.TransitionImageForPreviewItem += delegate
				{
					return new UIImage ();
				};

				Assert.NotNull (pc.Delegate, "Delegate");
				Assert.NotNull (pc.WeakDelegate, "WeakDelegate");
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
