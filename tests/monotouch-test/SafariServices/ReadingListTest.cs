//
// Unit tests for SSReadingList
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
using System.IO;
#if XAMCORE_2_0
using Foundation;
using SafariServices;
using UIKit;
#else
using MonoTouch.Foundation;
using MonoTouch.SafariServices;
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

namespace MonoTouchFixtures.SafariServices {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ReadingListTest {

		string local_file = Path.Combine (NSBundle.MainBundle.ResourcePath, "Hand.wav");

		[Test]
		public void DefaultReadingList ()
		{
			TestRuntime.AssertiOSSystemVersion (7, 0, throwIfOtherPlatform: false);

			NSError error;
			using (var http = new NSUrl ("http://www.xamarin.com"))
			using (var local = new NSUrl (local_file, false))
			using (var rl = SSReadingList.DefaultReadingList) {
				Assert.True (rl.Add (http, "title", "preview text", out error), "Add-1");
				Assert.Null (error, "error-1");

				Assert.True (rl.Add (http, null, null, out error), "Add-2");
				Assert.Null (error, "error-2");

				Assert.False (rl.Add (local, null, null, out error), "Add-3");
				Assert.That (error.Domain, Is.EqualTo ((string) SSReadingList.ErrorDomain), "Domain");
				Assert.That (error.Code, Is.EqualTo ((nint) (int) SSReadingListError.UrlSchemeNotAllowed), "Code");
			}
		}

		[Test]
		public void SupportsUrl ()
		{
			TestRuntime.AssertiOSSystemVersion (7, 0, throwIfOtherPlatform: false);

			Assert.False (SSReadingList.SupportsUrl (null), "null");

			using (var http = new NSUrl ("http://www.xamarin.com"))
				Assert.True (SSReadingList.SupportsUrl (http), "http");

			using (var local = new NSUrl (local_file, false))
				Assert.False (SSReadingList.SupportsUrl (local), "local");
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
