//
// Unit tests for VTCompressionSession
//
// Authors:
//	Alex Soto <alex.soto@xamarin.com>
//	
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

using Foundation;
using VideoToolbox;
using CoreMedia;
using CoreVideo;
using AVFoundation;
using CoreFoundation;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.VideoToolbox {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class VTCompressionSessionTests {
		[Test]
		public void CompressionSessionCreateTest ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 8, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.TVOS, 10, 2, throwIfOtherPlatform: false);

			using (var session = CreateSession ()) {
				Assert.IsNotNull (session, "Session should not be null");
			}
		}

		[Test]
		public void CompressionSessionSetCompressionPropertiesTest ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 10, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.TVOS, 10, 2, throwIfOtherPlatform: false);

			using (var session = CreateSession ()) {

				var result = session.SetCompressionProperties (new VTCompressionProperties {
					RealTime = true,
					AllowFrameReordering = false
				});

				Assert.That (result, Is.EqualTo (VTStatus.Ok), "SetCompressionProperties");
			}
		}

		[Test]
		public void CompressionSessionSetPropertiesTest ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 8, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.TVOS, 10, 2, throwIfOtherPlatform: false);

			using (var session = CreateSession ()) {

				var result = session.SetProperties (new VTPropertyOptions {
					ReadWriteStatus = VTReadWriteStatus.ReadWrite,
					ShouldBeSerialized = true
				});

				Assert.That (result == VTStatus.Ok, "SetProperties");
			}
		}

		[Test]
		public void CompressionSessionSetCompressionPropertiesMultiPassStorageTest ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 10, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.TVOS, 10, 2, throwIfOtherPlatform: false);

			using (var session = CreateSession ())
			using (var storage = VTMultiPassStorage.Create ()) {
				var result = session.SetCompressionProperties (new VTCompressionProperties {
					RealTime = false,
					AllowFrameReordering = true,
					MultiPassStorage = storage
				});

				Assert.That (result == VTStatus.Ok, "SetCompressionPropertiesMultiPassStorage");
			}
		}

		// On iOS 8 all properties in GetSupportedProperties for Compression session return false on ShouldBeSerialized
		// with this test we will be able to catch if apple changes its mind about this in the future.
		[Test]
		public void CompressionSessionGetSupportedPropertiesTest ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 8, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.TVOS, 10, 2, throwIfOtherPlatform: false);

			using (var session = CreateSession ()) {
				var supportedProps = session.GetSupportedProperties ();
				Assert.NotNull (supportedProps, "GetSupportedProperties IsNull");

				var key = new NSString ("ShouldBeSerialized");
				foreach (var item in supportedProps) {
					var dict = (NSDictionary) item.Value;
					if (dict is null) continue;

					NSObject value;
					if (dict.TryGetValue (key, out value) && value is not null) {
						var number = (NSNumber) value;
						Assert.IsFalse (number.BoolValue, "CompressionSession GetSupportedPropertiesTest ShouldBeSerialized is True");
					}
				}
			}
		}

		// This test is (kind of) expected to be null due to as of iOS 8 all supported properties are not meant to be serialized
		// see CompressionSessionGetSupportedPropertiesTest.
		[Test]
#if MONOMAC || __MACCATALYST__ // https://bugzilla.xamarin.com/show_bug.cgi?id=51258
		[Ignore ("Crashes with SIGSEGV when trying to dispose session after calling session.GetSerializableProperties ()")]
#endif
		public void CompressionSessionGetSerializablePropertiesTest ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 8, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.TVOS, 10, 2, throwIfOtherPlatform: false);

			using (var session = CreateSession ()) {
				var supportedProps = session.GetSerializableProperties ();
				Assert.IsNull (supportedProps, "CompressionSession GetSerializableProperties is not null");
			}
		}

		VTCompressionSession CreateSession ()
		{
			var session = VTCompressionSession.Create (1024, 768, CMVideoCodecType.H264,
				(sourceFrame, status, flags, buffer) => { });
			return session;
		}

#if !NET
		[DllImport ("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		static extern IntPtr CFRetain (IntPtr obj);
#endif

		[TestCase (true)]
		[TestCase (false)]
		public void TestCallback (bool stronglyTyped)
		{
			Exception ex = null;
			var thread = new Thread (() => {
				try {
					TestCallbackBackground (stronglyTyped);
				} catch (Exception e) {
					ex = e;
				}
			});
			thread.IsBackground = true;
			thread.Start ();
			var completed = thread.Join (TimeSpan.FromSeconds (30));
			Assert.IsNull (ex); // We check for this before the completion assert, to show any other assertion failures that may occur in CI.
			if (!completed)
				TestRuntime.IgnoreInCI ("This test fails occasionally in CI");
			Assert.IsTrue (completed, "timed out");
		}

		public void TestCallbackBackground (bool stronglyTyped)
		{
			var width = 640;
			var height = 480;
			var encoder_specification = new VTVideoEncoderSpecification ();
			var source_attributes = new CVPixelBufferAttributes (CVPixelFormatType.CV420YpCbCr8BiPlanarFullRange, width, height);
			var duration = new CMTime (40, 1);
			VTStatus status;
			using var frameProperties = new NSDictionary ();

			int callbackCounter = 0;
			var failures = new List<string> ();
			var callback = new VTCompressionSession.VTCompressionOutputCallback ((IntPtr sourceFrame, VTStatus status, VTEncodeInfoFlags flags, CMSampleBuffer buffer) => {
				Interlocked.Increment (ref callbackCounter);
				if (status != VTStatus.Ok)
					failures.Add ($"Callback #{callbackCounter} failed. Expected status = Ok, got status = {status}");
#if !NET
				// Work around a crash that occur if the buffer isn't retained
				if (stronglyTyped) {
					CFRetain (buffer.Handle);
				}
#endif
			});
			using var session = stronglyTyped
				? VTCompressionSession.Create (
					width, height,
					CMVideoCodecType.H264,
					callback,
					encoder_specification,
					source_attributes
					)
				: VTCompressionSession.Create (
					width, height,
					CMVideoCodecType.H264,
					callback,
					encoder_specification,
					source_attributes.Dictionary
					);

			var frameCount = 20;
			for (var i = 0; i < frameCount; i++) {
				using var imageBuffer = new CVPixelBuffer (width, height, CVPixelFormatType.CV420YpCbCr8BiPlanarFullRange);
				var pts = new CMTime (40 * i, 1);
				status = session.EncodeFrame (imageBuffer, pts, duration, null, imageBuffer, out var infoFlags);
				Assert.AreEqual (status, VTStatus.Ok, $"status #{i}");
				// This looks weird, but it seems the video encoder can become overwhelmed otherwise, and it
				// will start failing (and taking a long time to do so, eventually timing out the test).
				Thread.Sleep (10);
			};
			status = session.CompleteFrames (new CMTime (40 * frameCount, 1));
			Assert.AreEqual (status, VTStatus.Ok, "status finished");
			Assert.AreEqual (callbackCounter, frameCount, "frame count");
			Assert.That (failures, Is.Empty, "no callback failures");
		}


	}
}

#endif // !__WATCHOS__
