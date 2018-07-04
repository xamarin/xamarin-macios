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
using System.Drawing;
using System.IO;
using System.Threading;
#if XAMCORE_2_0
using Foundation;
using AVFoundation;
using CoreMedia;
using ObjCRuntime;
#else
using MonoTouch.AVFoundation;
using MonoTouch.CoreMedia;
using MonoTouch.Foundation;
#endif
using NUnit.Framework;

#if XAMCORE_2_0
using RectangleF=CoreGraphics.CGRect;
using SizeF=CoreGraphics.CGSize;
using PointF=CoreGraphics.CGPoint;
#else
using nfloat=global::System.Single;
using nint=global::System.Int32;
using nuint=global::System.UInt32;
#endif

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
				Assert.That (aig.MaximumSize, Is.EqualTo (SizeF.Empty), "MaximumSize");
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
#if MONOMAC
		string video_asset_path = Path.Combine (NSBundle.MainBundle.BundlePath, "Contents/Resources/xamvideotest.mp4");
#else
		string video_asset_path = Path.Combine (NSBundle.MainBundle.BundlePath, "xamvideotest.mp4");
#endif
		bool handled;
		ManualResetEvent mre;

		[Test]
		public void GenerateCGImagesAsynchronously ()
		{
			// This test deadlocks on Mountain Lion (but works on Lion)
			// https://gist.github.com/rolfbjarne/1190d97af79e554c298f2c133dfd8e87
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 9, throwIfOtherPlatform: false);

			handled = false;
			mre = new ManualResetEvent (false);
			ThreadStart main = () => {
				using (NSUrl video_url = NSUrl.FromFilename (video_asset_path))
				using (AVAsset video_asset = AVAsset.FromUrl (video_url))
				using (AVAssetImageGenerator aig = new AVAssetImageGenerator (video_asset)) {
					NSValue[] values = new NSValue[] { NSValue.FromCMTime (CMTime.Zero) };
					aig.GenerateCGImagesAsynchronously (values, handler);
					mre.WaitOne ();
				}
			};
			var asyncResult = main.BeginInvoke (null, null);
			main.EndInvoke (asyncResult);
			Assert.True (mre.WaitOne (2000), "wait");
			Assert.True (handled, "handled");
		}

#if !XAMCORE_2_0
		[Test]
		public void GenerateCGImagesAsynchronously_Compat ()
		{
			handled = false;
			mre = new ManualResetEvent (false);
			ThreadStart main = () => {
				using (NSUrl video_url = NSUrl.FromFilename (video_asset_path))
				using (AVAsset video_asset = AVAsset.FromUrl (video_url))
				using (AVAssetImageGenerator aig = new AVAssetImageGenerator (video_asset)) {
					aig.GenerateCGImagesAsynchronously (NSValue.FromCMTime (CMTime.Zero), handler);
					mre.WaitOne ();
				}
			};
			var asyncResult = main.BeginInvoke (null, null);
			main.EndInvoke (asyncResult);
			Assert.True (mre.WaitOne (2000));
			Assert.True (handled, "handled");
		}
#endif

		void handler (CMTime requestedTime, IntPtr imageRef, CMTime actualTime, AVAssetImageGeneratorResult result, NSError error) 
		{
			handled = true;
			mre.Set ();
		}
	}
}

#endif // !__WATCHOS__
