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

#if XAMCORE_2_0
using Foundation;
using VideoToolbox;
using CoreMedia;
using AVFoundation;
using CoreFoundation;
#else
using MonoTouch.Foundation;
using MonoTouch.VideoToolbox;
using MonoTouch.UIKit;
using MonoTouch.CoreMedia;
using MonoTouch.AVFoundation;
using MonoTouch.CoreFoundation;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.VideoToolbox {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class VTFrameSiloTests
	{
		[Test]
		public void FrameSiloCreateTest ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (8, 0))
				Assert.Ignore ("Ignoring VideoToolbox tests: Requires iOS8+");

			using (var silo = VTFrameSilo.Create ()){
				Assert.IsNotNull (silo, "Silo should not be null");
			}
		}

		[Test]
		public void SetTimeRangesTest ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (8, 0))
				Assert.Ignore ("Ignoring VideoToolbox tests: Requires iOS8+");

			using (var silo = VTFrameSilo.Create ()){
				var result = silo.SetTimeRangesForNextPass (new CMTimeRange [0]);
				Assert.IsTrue (result == VTStatus.FrameSiloInvalidTimeRange, "SetTimeRangesForNextPass");
			}
		}

		[Test]
		public void ForEachTest ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (8, 0))
				Assert.Ignore ("Ignoring VideoToolbox tests: Requires iOS8+");

			using (var silo = VTFrameSilo.Create ()){

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
