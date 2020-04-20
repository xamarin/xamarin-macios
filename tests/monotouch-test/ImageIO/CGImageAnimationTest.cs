//
// Unit tests for CGImageAnimation
//
// Authors:
//	Whitney Schmidt <whschm@microsoft.com>
//
// Copyright 2020 Microsoft Corp. All rights reserved.
//
using System;
using System.IO;
#if XAMCORE_2_0
using Foundation;
using System.Threading.Tasks;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using CoreGraphics;
using ImageIO;
#else
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.ImageIO;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.ImageIO {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CGImageAnimationTest {

		NSUrl imageUrl;
		NSData imageData;
		int testValue;
		CGImageAnimation imageAnimation;

		void MyBlockSetValueZero (nint index, CGImage image, out bool stop)
		{
			stop = true;
			testValue = 0;
		}

		void MyBlockSetValueOne (nint index, CGImage image, out bool stop)
		{
			stop = true;
			testValue = 1;
		}

		[TestFixtureSetUp]
		public void Init ()
		{
			TestRuntime.AssertXcodeVersion (11, 4);

			imageUrl = NSBundle.MainBundle.GetUrlForResource ("hack", "gif"); // why not just initialize in declaration?
			imageData = NSData.FromUrl(imageUrl);
			imageAnimation = new CGImageAnimation ();
		}

		[SetUp]
		public void InitPerTest()
		{
			TestRuntime.AssertXcodeVersion (11, 4);
			testValue = -1;
		}

		[TearDown]
		public void Dispose()
		{
			TestRuntime.AssertXcodeVersion (11, 4); // this seems cleaner, but isn't strictly necessary?
			testValue = -1;
		}

		[Test]
		public void AnimateImageWithUrl ()
		{
			Assert.AreEqual(-1, testValue);
			TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool> ();

			bool done = false;
			TestRuntime.RunAsync (TimeSpan.FromSeconds (30), async () => {
				imageAnimation.AnimateImage(imageUrl, null, MyBlockSetValueZero);
				await taskCompletionSource.Task;
				done = true;
			},
				() => done);
			Assert.AreEqual (0, testValue);
		}

		[Test]
		public void AnimateImageWithData ()
		{

			Assert.AreEqual (-1, testValue);
			TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool> ();

			bool done = false;
			TestRuntime.RunAsync (TimeSpan.FromSeconds (30), async () => {
				imageAnimation.AnimateImage(imageData, null, MyBlockSetValueZero);
				await taskCompletionSource.Task;
				done = true;
			},
				() => done);
			Assert.AreEqual (0, testValue);
		}

		[Test]
		public void AnimateImageWithUrlChangeBlock ()
		{
			Assert.AreEqual (-1, testValue);
			TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool> ();

			bool done = false;
			TestRuntime.RunAsync (TimeSpan.FromSeconds (30), async () => {
				imageAnimation.AnimateImage (imageUrl, null, MyBlockSetValueZero);
				await taskCompletionSource.Task;
				done = true;
			},
				() => done);
			Assert.AreEqual (0, testValue);

			done = false; // reset after first call
			TestRuntime.RunAsync (TimeSpan.FromSeconds (30), async () => {
				imageAnimation.AnimateImage (imageUrl, null, MyBlockSetValueOne);
				await taskCompletionSource.Task;
				done = true;
			},
				() => done);
			Assert.AreEqual (1, testValue);
		}

		[Test]
		public void AnimateImageWithDataChangeBlock ()
		{
			Assert.AreEqual (-1, testValue);
			TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool> ();

			bool done = false;
			TestRuntime.RunAsync (TimeSpan.FromSeconds (30), async () => {
				imageAnimation.AnimateImage (imageData, null, MyBlockSetValueZero);
				await taskCompletionSource.Task;
				done = true;
			},
				() => done);
			Assert.AreEqual (0, testValue);

			done = false; // reset after first call
			TestRuntime.RunAsync (TimeSpan.FromSeconds (30), async () => {
				imageAnimation.AnimateImage (imageData, null, MyBlockSetValueOne);
				await taskCompletionSource.Task;
				done = true;
			},
				() => done);
			Assert.AreEqual (1, testValue);
		}

		[Test]
		public void AnimateImageWithUrlNullUrl()
		{
			NSUrl nullUrl = null;
			Assert.Throws<ArgumentNullException>(() => imageAnimation.AnimateImage(nullUrl, null, MyBlockSetValueZero));
		}

		[Test]
		public void AnimateImageWithUrlNullBlock()
		{
			Assert.Throws<ArgumentNullException>(() => imageAnimation.AnimateImage(imageUrl, null, /* CGImageSourceAnimationBlock */ null));
		}

		[Test]
		public void AnimateImageWithDataNullData()
		{
			NSData nullData = null;
			Assert.Throws<ArgumentNullException>(() => imageAnimation.AnimateImage(nullData, null, MyBlockSetValueZero));
		}

		[Test]
		public void AnimateImageWithDataNullBlock()
		{
			Assert.Throws<ArgumentNullException>(() => imageAnimation.AnimateImage(imageData, null, /* CGImageSourceAnimationBlock */ null));
		}

	}
}
