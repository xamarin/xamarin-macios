//
// Unit tests for AVAssetImageGenerator
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using System.IO;
using System.Threading;
using CoreGraphics;
using Foundation;
using AVFoundation;
using CoreMedia;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.AVFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AVAssetImageGeneratorTest {

		[Test]
		public void Defaults ()
		{
			using (NSUrl video_url = NSUrl.FromFilename (video_asset_path))
			using (AVAsset video_asset = AVAsset.FromUrl (video_url))
			using (AVAssetImageGenerator aig = new AVAssetImageGenerator (video_asset)) {
				Assert.Null (aig.ApertureMode, "ApertureMode");
				Assert.False (aig.AppliesPreferredTrackTransform, "AppliesPreferredTrackTransform");
				Assert.That (aig.MaximumSize, Is.EqualTo (CGSize.Empty), "MaximumSize");
				Assert.True (aig.RequestedTimeToleranceAfter.IsPositiveInfinity, "RequestedTimeToleranceAfter");
				Assert.True (aig.RequestedTimeToleranceBefore.IsPositiveInfinity, "RequestedTimeToleranceBefore");
			}
		}

		[Test]
		public void AppliesPreferredTrackTransform ()
		{
			using (NSUrl video_url = NSUrl.FromFilename (video_asset_path))
			using (AVAsset video_asset = AVAsset.FromUrl (video_url))
			using (AVAssetImageGenerator aig = new AVAssetImageGenerator (video_asset)) {
				// setter was missing see https://bugzilla.xamarin.com/show_bug.cgi?id=5216
				aig.AppliesPreferredTrackTransform = true;
				Assert.True (aig.AppliesPreferredTrackTransform, "AppliesPreferredTrackTransform");
			}
		}

		[Test]
		public void CopyCGImageAtTime ()
		{
			// Mp4 file is supported by CopyCGImageAtTime so we can test out actual param
			using (NSUrl video_url = NSUrl.FromFilename (video_asset_path))
			using (AVAsset video_asset = AVAsset.FromUrl (video_url))
			using (AVAssetImageGenerator aig = new AVAssetImageGenerator (video_asset)) {
				// signature errors see https://bugzilla.xamarin.com/show_bug.cgi?id=5218
				CMTime actual;
				NSError error;
				var img = aig.CopyCGImageAtTime (CMTime.Zero, out actual, out error);
				Assert.NotNull (img, "CopyCGImageAtTime");
				Assert.False (actual.IsInvalid, "actual");
				Assert.Null (error, "error");
			}
		}

		[Test]
		public void CopyCGImageAtTime_Invalid ()
		{
			// Mov file is not supported by CopCGImageAtTime so we can test out error param
			using (NSUrl video_url = NSUrl.FromFilename (does_not_exists_asset_path))
			using (AVAsset video_asset = AVAsset.FromUrl (video_url))
			using (AVAssetImageGenerator aig = new AVAssetImageGenerator (video_asset)) {
				// signature errors see https://bugzilla.xamarin.com/show_bug.cgi?id=5218
				CMTime actual;
				NSError error;
				var img = aig.CopyCGImageAtTime (CMTime.Zero, out actual, out error);
				Assert.Null (img, "missing");
				Assert.True (actual.IsInvalid, "actual");
				Assert.NotNull (error, "error");
			}
		}

		string does_not_exists_asset_path = Path.Combine (NSBundle.MainBundle.BundlePath, "xamarin.mov");
		string video_asset_path = Path.Combine (NSBundle.MainBundle.ResourcePath, "xamvideotest.mp4");
		bool handled;
		ManualResetEvent mre;

		[Test]
		public void GenerateCGImagesAsynchronously ()
		{
			// This test deadlocks on Mountain Lion (but works on Lion)
			// https://gist.github.com/rolfbjarne/1190d97af79e554c298f2c133dfd8e87
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 9, throwIfOtherPlatform: false);

			handled = false;
			mre = new ManualResetEvent (false);
			ThreadStart main = () => {
				using (NSUrl video_url = NSUrl.FromFilename (video_asset_path))
				using (AVAsset video_asset = AVAsset.FromUrl (video_url))
				using (AVAssetImageGenerator aig = new AVAssetImageGenerator (video_asset)) {
					NSValue [] values = new NSValue [] { NSValue.FromCMTime (CMTime.Zero) };
					aig.GenerateCGImagesAsynchronously (values, handler);
					mre.WaitOne ();
				}
			};
			var thread = new Thread (main) {
				IsBackground = true,
			};
			thread.Start ();
			Assert.True (mre.WaitOne (2000), "wait");
			Assert.True (handled, "handled");
		}

		void handler (CMTime requestedTime, IntPtr imageRef, CMTime actualTime, AVAssetImageGeneratorResult result, NSError error)
		{
			handled = true;
			mre.Set ();
		}
	}
}

#endif // !__WATCHOS__
