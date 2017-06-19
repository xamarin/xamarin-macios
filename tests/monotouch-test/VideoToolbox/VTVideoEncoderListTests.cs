//
// Unit tests for VTVideoEncoderList
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
#else
using MonoTouch.Foundation;
using MonoTouch.VideoToolbox;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.VideoToolbox {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class VTVideoEncoderListTests
	{
		[Test]
		public void VideoEncoderListTest ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (8, 0))
				Assert.Ignore ("Ignoring VideoToolbox tests: Requires iOS8+");

			var encoders = VTVideoEncoder.GetEncoderList ();
			Assert.NotNull (encoders, "VTVideoEncoder.GetEncoderList () Should Not be null");
		}
	}
}

#endif // !__WATCHOS__
