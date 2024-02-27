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
	public class VTDecompressionSessionTests {
		[Test]
		public void DecompressionSessionCreateTest ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 8, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.TVOS, 10, 2, throwIfOtherPlatform: false);

			using (var asset = AVAsset.FromUrl (NSBundle.MainBundle.GetUrlForResource ("xamvideotest", "mp4")))
			using (var session = CreateSession (asset)) {
				Assert.IsNotNull (session, "Session should not be null");
			}
		}

		[Test]
		public void DecompressionSessionSetDecompressionPropertiesTest ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 10, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.TVOS, 10, 2, throwIfOtherPlatform: false);

			using (var asset = AVAsset.FromUrl (NSBundle.MainBundle.GetUrlForResource ("xamvideotest", "mp4")))
			using (var session = CreateSession (asset)) {

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
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 8, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.TVOS, 10, 2, throwIfOtherPlatform: false);

			using (var asset = AVAsset.FromUrl (NSBundle.MainBundle.GetUrlForResource ("xamvideotest", "mp4")))
			using (var session = CreateSession (asset)) {

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
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 8, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.TVOS, 10, 2, throwIfOtherPlatform: false);

			using (var asset = AVAsset.FromUrl (NSBundle.MainBundle.GetUrlForResource ("xamvideotest", "mp4")))
			using (var session = CreateSession (asset)) {
				var supportedProps = session.GetSupportedProperties ();
				Assert.NotNull (supportedProps, "GetSupportedProperties");
				Assert.That (supportedProps.Count, Is.GreaterThan ((nuint) 0), "GetSupportedProperties should be more than zero");
			}
		}

		VTDecompressionSession CreateSession (AVAsset asset)
		{
			var videoTracks = asset.TracksWithMediaType (AVMediaTypes.Video.GetConstant ());
			var track = videoTracks [0];
			var formatDescriptor = track.FormatDescriptions [0] as CMVideoFormatDescription;

			var session = VTDecompressionSession.Create (
				(sourceFrame, status, flags, buffer, presentationTimeStamp, presentationDuration) => { },
				formatDescriptor);

			return session;
		}
	}
}

#endif // !__WATCHOS__
