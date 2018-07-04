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
#if XAMCORE_2_0
using Foundation;
using UIKit;
using QuickLook;
using ObjCRuntime;
#else
using MonoTouch.Foundation;
using MonoTouch.QuickLook;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

#if XAMCORE_2_0
using RectangleF=CoreGraphics.CGRect;
using SizeF=CoreGraphics.CGSize;
using PointF=CoreGraphics.CGPoint;
#else
using nfloat=global::System.Single;
using nint=global::System.Int32;
using nuint=global::System.UInt32;
#endif

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
				if (TestRuntime.CheckSystemVersion (PlatformName.iOS, 10, 0))
					index = nint.MaxValue;
				else if (TestRuntime.CheckSystemVersion (PlatformName.iOS, 7, 1))
					index = -1;
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
#if XAMCORE_2_0
				pc.ShouldOpenUrl += delegate (QLPreviewController controller, NSUrl url, IQLPreviewItem item) { 
#else
					pc.ShouldOpenUrl += delegate (QLPreviewController controller, NSUrl url, QLPreviewItem item) { 
#endif
					return false; 
				};
				pc.FrameForPreviewItem += delegate {
					return new RectangleF (1, 2, 3, 4);
				};
				pc.TransitionImageForPreviewItem += delegate {
					return new UIImage ();
				};
				
				Assert.NotNull (pc.Delegate, "Delegate");
				Assert.NotNull (pc.WeakDelegate, "WeakDelegate");
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
