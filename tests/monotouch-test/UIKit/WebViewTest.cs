// Copyright 2011 Xamarin Inc. All rights reserved

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC && !__MACCATALYST__

using System;
using System.Drawing;
using CoreGraphics;
using Foundation;
using UIKit;
using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class WebViewTest {

		[OneTimeSetUp]
		public void Setup ()
		{
			if (Type.GetType ("UIKit.DeprecatedWebView, Xamarin.iOS") is not null)
				Assert.Ignore ("All type references to UIWebView were removed (optimized).");
		}

		[Test]
		public void InitWithFrame ()
		{
			var frame = new CGRect (10, 10, 100, 100);
			using (UIWebView wv = new UIWebView (frame)) {
				Assert.That (wv.Frame, Is.EqualTo (frame), "Frame");
				Assert.Null (wv.Request, "Request");
			}
		}

		[Test]
		public void InstantiateDerivedClass ()
		{
			using (var derived = new DerivedUIWebView ()) {
				// bug #9261 - just instantiating a derived UIWebView class crashes on iOS 5
			}
		}

		class DerivedUIWebView : UIWebView {
		}
	}

}

#endif // !__TVOS__ && !__WATCHOS__ && !MONOMAC && !__MACCATALYST__
