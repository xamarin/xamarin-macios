//
// Unit tests for VTFrameSilo
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
	public class VTFrameSiloTests {
		[Test]
		public void FrameSiloCreateTest ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 10, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.TVOS, 10, 2, throwIfOtherPlatform: false);

			using (var silo = VTFrameSilo.Create ()) {
				Assert.IsNotNull (silo, "Silo should not be null");
			}
		}

		[Test]
		public void SetTimeRangesTest ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 10, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.TVOS, 10, 2, throwIfOtherPlatform: false);

			using (var silo = VTFrameSilo.Create ()) {
				var result = silo.SetTimeRangesForNextPass (new CMTimeRange [0]);
				Assert.IsTrue (result == VTStatus.FrameSiloInvalidTimeRange, "SetTimeRangesForNextPass");
			}
		}

		[Test]
		public void ForEachTest ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 10, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.TVOS, 10, 2, throwIfOtherPlatform: false);

			using (var silo = VTFrameSilo.Create ()) {

				var result = silo.ForEach ((arg) => {
					return VTStatus.Ok;
				});
				Assert.IsTrue (result == VTStatus.Ok, "VTFrameSilo ForEach");

				result = silo.ForEach ((arg) => {
					return VTStatus.Ok;
				});
				Assert.IsTrue (result == VTStatus.Ok, "VTFrameSilo ForEach");

				result = silo.ForEach ((arg) => {
					return VTStatus.Ok;
				});
				Assert.IsTrue (result == VTStatus.Ok, "VTFrameSilo ForEach");
			}
		}
	}
}

#endif // !__WATCHOS__
