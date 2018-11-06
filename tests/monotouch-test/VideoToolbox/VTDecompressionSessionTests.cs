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
using ObjCRuntime;
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
			TestRuntime.AssertSystemVersion (PlatformName.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 8, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (PlatformName.TvOS, 10, 2, throwIfOtherPlatform: false);

			using (var asset = AVAsset.FromUrl (NSBundle.MainBundle.GetUrlForResource ("xamvideotest", "mp4"))) 
			using (var session = CreateSession (asset)){
				Assert.IsNotNull (session, "Session should not be null");
			}
		}

		[Test]
		public void DecompressionSessionSetDecompressionPropertiesTest ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 10, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (PlatformName.TvOS, 10, 2, throwIfOtherPlatform: false);

			using (var asset = AVAsset.FromUrl (NSBundle.MainBundle.GetUrlForResource ("xamvideotest", "mp4"))) 
			using (var session = CreateSession (asset)){

				var result = session.SetDecompressionProperties (new VTDecompressionProperties {
					RealTime = true,
					OnlyTheseFrames = VTOnlyTheseFrames.AllFrames
				});

				Assert.AreEqual (VTStatus.Ok, result, "SetDecompressionProperties");
			}
		}

		[Test]
		public void DecompressionSessionSetPropertiesTest ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 8, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (PlatformName.TvOS, 10, 2, throwIfOtherPlatform: false);

			using (var asset = AVAsset.FromUrl (NSBundle.MainBundle.GetUrlForResource ("xamvideotest", "mp4"))) 
			using (var session = CreateSession (asset)){

				var result = session.SetProperties (new VTPropertyOptions {
					ReadWriteStatus = VTReadWriteStatus.ReadWrite,
					ShouldBeSerialized = true
				});

				Assert.AreEqual (VTStatus.Ok, result, "SetProperties");
			}
		}

		[Test]
		public void DecompressionSessionGetSupportedPropertiesTest ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 8, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (PlatformName.TvOS, 10, 2, throwIfOtherPlatform: false);

			using (var asset = AVAsset.FromUrl (NSBundle.MainBundle.GetUrlForResource ("xamvideotest", "mp4")))
			using (var session = CreateSession (asset)) {
				var supportedProps = session.GetSupportedProperties ();
				Assert.NotNull (supportedProps, "GetSupportedProperties");
				Assert.That (supportedProps.Count, Is.GreaterThan (0), "GetSupportedProperties should be more than zero");
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
