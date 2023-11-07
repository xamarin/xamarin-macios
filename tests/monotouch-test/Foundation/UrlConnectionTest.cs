//
// Unit tests for UrlConnection
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

using Foundation;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using ObjCRuntime;
using NUnit.Framework;
using MonoTests.System.Net.Http;


namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class UrlConnectionTest {

		class MyDelegate : NSUrlConnectionDelegate {

		}

		[Test]
		public void StartCancel ()
		{
			using (var url = new NSUrl (NetworkResources.MicrosoftUrl))
			using (var r = new NSUrlRequest (url))
			using (var d = new MyDelegate ())
			using (var c = new NSUrlConnection (r, d)) {
				c.Start ();
				c.Cancel ();
			}
		}
	}
}
