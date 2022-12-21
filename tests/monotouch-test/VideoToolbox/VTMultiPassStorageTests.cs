//
// Unit tests for VTMultiPassStorage
//
// Authors:
//	Alex Soto <alex.soto@xamarin.com>
//	
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;

using Foundation;
using VideoToolbox;
using CoreMedia;
using AVFoundation;
using CoreFoundation;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.VideoToolbox {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class VTMultiPassStorageTests {
		[Test]
		public void MultiPassStorageCreateTest ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 10, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.TVOS, 10, 2, throwIfOtherPlatform: false);

			using (var storage = VTMultiPassStorage.Create ()) {
				Assert.IsNotNull (storage, "Storage should not be null");
			}
		}

		[Test]
		public void MultiPassStorageCloseTest ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 10, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.TVOS, 10, 2, throwIfOtherPlatform: false);

			using (var storage = VTMultiPassStorage.Create ()) {
				var result = storage.Close ();
				Assert.IsTrue (result == VTStatus.Ok, "VTMultiPassStorage Close");
			}
		}
	}
}

#endif // !__WATCHOS__
