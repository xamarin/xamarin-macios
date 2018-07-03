//
// Unit tests for CFUrl
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

using System;
#if XAMCORE_2_0
using Foundation;
using CoreFoundation;
#else
using MonoTouch.CoreFoundation;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.CoreFoundation {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CFUrlTest {
		
		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void FromFile_Null ()
		{
			CFUrl.FromFile (null);
		}

		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void FromUrlString_Null ()
		{
			CFUrl.FromUrlString (null, CFUrl.FromFile ("/"));
		}
		
		[Test]
		public void ToString_ ()
		{
			using (CFUrl url = CFUrl.FromFile ("/")) {
				string value = "file://localhost/";
#if __IOS__
				if (TestRuntime.CheckiOSSystemVersion (7, 0))
					value = "file:///";
#elif __WATCHOS__ || __TVOS__
				value = "file:///";
#elif __MACOS__
				if (TestRuntime.CheckMacSystemVersion (10, 9))
					value = "file:///";
#endif

				Assert.That (url.ToString (), Is.EqualTo (value), "FromFile");
			}
			using (CFUrl url = CFUrl.FromUrlString ("/", null)) {
				Assert.That (url.ToString (), Is.EqualTo ("/"), "FromUrlString");
			}
		}
	}
}
