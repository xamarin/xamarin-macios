//
// Unit tests for NSKeyedUnarchiver
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

using System;
using System.Net;
using System.IO;

using Foundation;
using ObjCRuntime;
#if MONOMAC
using AppKit;
#if NET
using PlatformException = ObjCRuntime.ObjCException;
#else
using PlatformException=Foundation.ObjCException;
#endif
#else
using UIKit;
#if NET
using PlatformException = ObjCRuntime.ObjCException;
#else
using PlatformException = Foundation.MonoTouchException;
#endif
#endif

using NUnit.Framework;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class KeyedUnarchiverTest {

		[Test]
		public void Exceptions ()
		{
			var data = NSData.FromString ("dummy string");
			if (TestRuntime.CheckXcodeVersion (7, 0)) {
				// iOS9 does not throw if it cannot get correct data, it simply returns null (much better)
				Assert.Null (NSKeyedUnarchiver.UnarchiveFile (Path.Combine (NSBundle.MainBundle.ResourcePath, "basn3p08.png")), "UnarchiveFile");
				Assert.Null (NSKeyedUnarchiver.UnarchiveObject (data), "UnarchiveObject");
			} else {
				Assert.Throws<PlatformException> (() => NSKeyedUnarchiver.UnarchiveFile (Path.Combine (NSBundle.MainBundle.ResourcePath, "basn3p08.png")), "UnarchiveFile");
				Assert.Throws<PlatformException> (() => NSKeyedUnarchiver.UnarchiveObject (data), "UnarchiveObject");
			}
		}
	}
}
