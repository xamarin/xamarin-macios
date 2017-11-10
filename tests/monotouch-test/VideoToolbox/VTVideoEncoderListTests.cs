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
using CoreMedia;
#else
using MonoTouch.Foundation;
using MonoTouch.VideoToolbox;
using MonoTouch.UIKit;
using MonoTouch.CoreMedia;
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

		[Test]
		public void SupportedEncoderPropertiesTest ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);

			var props = VTVideoEncoder.GetSupportedEncoderProperties (1920, 1080, CMVideoCodecType.H264);
			Assert.NotNull (props, "props should Not be null");
		}
	}
}

#endif // !__WATCHOS__
