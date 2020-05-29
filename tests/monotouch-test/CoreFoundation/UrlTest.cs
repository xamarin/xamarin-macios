//
// Unit tests for CFUrl
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

using System;
using Foundation;
using CoreFoundation;
using ObjCRuntime;
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
		public void RetainCountFromFile ()
		{
			var path = typeof (int).Assembly.Location;

			using (var url = CFUrl.FromFile (path)) {
				Assert.That (TestRuntime.CFGetRetainCount (url.Handle), Is.EqualTo ((nint) 1), "RetainCount");
			}
		}

		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void FromUrlString_Null ()
		{
			CFUrl.FromUrlString (null, CFUrl.FromFile ("/"));
		}

		[Test]
		public void RetainCountFromUrl ()
		{
			using (var url = CFUrl.FromUrlString ("http://xamarin.com", null)) {
				Assert.That(TestRuntime.CFGetRetainCount (url.Handle), Is.EqualTo ((nint) 1), "RetainCount");
			}
		}

		[Test]
		public void ToString_ ()
		{
			using (CFUrl url = CFUrl.FromFile ("/")) {
				string value = "file://localhost/";
#if __IOS__
				if (TestRuntime.CheckSystemVersion (PlatformName.iOS, 7, 0))
					value = "file:///";
#elif __WATCHOS__ || __TVOS__
				value = "file:///";
#elif __MACOS__
				if (TestRuntime.CheckSystemVersion (PlatformName.MacOSX, 10, 9))
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
