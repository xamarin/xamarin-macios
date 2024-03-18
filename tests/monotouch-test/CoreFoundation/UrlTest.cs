//
// Unit tests for CFUrl
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

using System;
using System.IO;

using Foundation;
using CoreFoundation;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.CoreFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CFUrlTest {

		[Test]
		public void FromFile_Null ()
		{
			Assert.Throws<ArgumentNullException> (() => CFUrl.FromFile (null));
		}

		[Test]
		public void RetainCountFromFile ()
		{
			var path = Path.Combine (Path.GetTempPath (), "placeholder.txt"); // the file doesn't have to exist, so just create any filename.

			using (var url = CFUrl.FromFile (path)) {
				Assert.That (TestRuntime.CFGetRetainCount (url.Handle), Is.EqualTo ((nint) 1), "RetainCount");
			}
		}

		[Test]
		public void FromUrlString_Null ()
		{
			Assert.Throws<ArgumentNullException> (() => CFUrl.FromUrlString (null, CFUrl.FromFile ("/")));
		}

		[Test]
		public void RetainCountFromUrl ()
		{
			using (var url = CFUrl.FromUrlString ("http://xamarin.com", null)) {
				Assert.That (TestRuntime.CFGetRetainCount (url.Handle), Is.EqualTo ((nint) 1), "RetainCount");
			}
		}

		[Test]
		public void ToString_ ()
		{
			using (CFUrl url = CFUrl.FromFile ("/")) {
				string value = "file://localhost/";
#if __IOS__
				if (TestRuntime.CheckSystemVersion (ApplePlatform.iOS, 7, 0))
					value = "file:///";
#elif __WATCHOS__ || __TVOS__
				value = "file:///";
#elif __MACOS__
				if (TestRuntime.CheckSystemVersion (ApplePlatform.MacOSX, 10, 9))
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
