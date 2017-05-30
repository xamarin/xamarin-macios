//
// Unit tests for VTDecompressionSession
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
	public class VTDecompressionSessionTests
	{
		[Test]
		public void DecompressionSessionCreateTest ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (8, 0))
				Assert.Ignore ("Ignoring VideoToolbox tests: Requires iOS8+");

			using (var asset = AVAsset.FromUrl (NSBundle.MainBundle.GetUrlForResource ("xamvideotest", "mp4"))) 
			using (var session = CreateSession (asset)){
				Assert.IsNotNull (session, "Session should not be null");
			}
		}

		[Test]
		public void DecompressionSessionSetDecompressionPropertiesTest ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (8, 0))
				Assert.Ignore ("Ignoring VideoToolbox tests: Requires iOS8+");

			using (var asset = AVAsset.FromUrl (NSBundle.MainBundle.GetUrlForResource ("xamvideotest", "mp4"))) 
			using (var session = CreateSession (asset)){

				var result = session.SetDecompressionProperties (new VTDecompressionProperties {
					RealTime = true,
					OnlyTheseFrames = VTOnlyTheseFrames.AllFrames
				});

				Assert.That (result == VTStatus.Ok, "SetDecompressionProperties");
			}
		}

		[Test]
		public void DecompressionSessionSetPropertiesTest ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (8, 0))
				Assert.Ignore ("Ignoring VideoToolbox tests: Requires iOS8+");

			using (var asset = AVAsset.FromUrl (NSBundle.MainBundle.GetUrlForResource ("xamvideotest", "mp4"))) 
			using (var session = CreateSession (asset)){

				var result = session.SetProperties (new VTPropertyOptions {
					ReadWriteStatus = VTReadWriteStatus.ReadWrite,
					ShouldBeSerialized = true
				});

				Assert.That (result == VTStatus.Ok, "SetProperties");
			}
		}

		[Test]
		public void DecompressionSessionGetSupportedPropertiesTest ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (8, 0))
				Assert.Ignore ("Ignoring VideoToolbox tests: Requires iOS8+");

			using (var asset = AVAsset.FromUrl (NSBundle.MainBundle.GetUrlForResource ("xamvideotest", "mp4")))
			using (var session = CreateSession (asset)) {
				var supportedProps = session.GetSupportedProperties ();
				Assert.NotNull (supportedProps, "GetSupportedProperties");
				Assert.That (supportedProps.Count > 0, "GetSupportedProperties should be more than zero");
			}
		}

		VTDecompressionSession CreateSession (AVAsset asset)
		{
			var videoTracks = asset.TracksWithMediaType (AVMediaType.Video);
			var track = videoTracks[0];
			var formatDescriptor = track.FormatDescriptions[0] as CMVideoFormatDescription;

			var session = VTDecompressionSession.Create (
				(sourceFrame, status, flags, buffer, presentationTimeStamp, presentationDuration) => {}, 
				formatDescriptor);

			return session;
		}
	}
}

#endif // !__WATCHOS__
