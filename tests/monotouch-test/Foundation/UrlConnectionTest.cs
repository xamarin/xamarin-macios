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

		[Test]
		public void SendSynchronousRequest ()
		{
			using var url = new NSUrl (NetworkResources.MicrosoftUrl);
			using var request = new NSUrlRequest (url);
			using var data = NSUrlConnection.SendSynchronousRequest (request, out var response, out var error);
			Assert.IsNull (error, "Error");
			Assert.IsNotNull (data, "Data");
			Assert.IsNotNull (response, "Response");
			response?.Dispose ();
			error?.Dispose ();
		}
	}
}
