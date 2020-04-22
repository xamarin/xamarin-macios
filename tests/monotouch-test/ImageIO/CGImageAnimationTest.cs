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

		void MyHandlerSetValueZero (nint index, CGImage image, out bool stop)
		{
			stop = true;
			testValue = 0;
		}

		void MyHandlerSetValueOne (nint index, CGImage image, out bool stop)
		{
			stop = true;
			testValue = 1;
		}

		[TestFixtureSetUp]
		public void Init ()
		{
			TestRuntime.AssertXcodeVersion (11, 4);

			imageUrl = NSBundle.MainBundle.GetUrlForResource ("hack", "gif");
			imageData = NSData.FromUrl (imageUrl);
			imageAnimation = new CGImageAnimation ();
		}

		[TestFixtureTearDown]
		public void Cleanup ()
		{
			imageUrl.Dispose ();
			imageData.Dispose ();
		}

		[SetUp]
		public void InitPerTest ()
		{
			testValue = -1;
		}

		[TearDown]
		public void Dispose ()
		{
			testValue = -1;
		}

		private void CallAnimateImage (bool useUrl, CGImageAnimation.CGImageSourceAnimationHandler handler)
		{
			var taskCompletionSource = new TaskCompletionSource<bool> ();
			bool done = false;

			TestRuntime.RunAsync (TimeSpan.FromSeconds (30), async () => {
				if (useUrl) {
					imageAnimation.AnimateImage (imageUrl, null, handler);
				} else {
					imageAnimation.AnimateImage (imageData, null, handler);
				}
				await taskCompletionSource.Task;
				done = true;
			},
				() => done);
		}

		[Test]
		public void AnimateImageWithUrl ()
		{
			CallAnimateImage ( /* useUrl */ true, MyHandlerSetValueZero);
			Assert.AreEqual (0, testValue, "handler called with url");
		}

		[Test]
		public void AnimateImageWithData ()
		{
			CallAnimateImage ( /* useUrl */ false, MyHandlerSetValueZero);
			Assert.AreEqual (0, testValue, "handler called with data");
		}

		[Test]
		public void AnimateImageWithUrlChangeHandler ()
		{
			CallAnimateImage ( /* useUrl */ true, MyHandlerSetValueZero);
			Assert.AreEqual (0, testValue, "first handler called with url" );

			CallAnimateImage ( /* useUrl */ true, MyHandlerSetValueOne);
			Assert.AreEqual (1, testValue, "second handler called with url");
		}

		[Test]
		public void AnimateImageWithDataChangeHandler ()
		{
			CallAnimateImage ( /* useUrl */ false, MyHandlerSetValueZero);
			Assert.AreEqual (0, testValue, "first handler called with data");

			CallAnimateImage ( /* useUrl */ false, MyHandlerSetValueOne);
			Assert.AreEqual (1, testValue, "second handler called with data");
		}

		[Test]
		public void AnimateImageWithUrlNullUrl ()
		{
			Assert.Throws<ArgumentNullException> (() => imageAnimation.AnimateImage ( (NSUrl) null, null, MyHandlerSetValueZero), "null url");
		}

		[Test]
		public void AnimateImageWithUrlNullHandler ()
		{
			Assert.Throws<ArgumentNullException> (() => imageAnimation.AnimateImage (imageUrl, null, /* CGImageSourceAnimationHandler */ null), "null handler called with url");
		}

		[Test]
		public void AnimateImageWithDataNullData ()
		{
			Assert.Throws<ArgumentNullException> (() => imageAnimation.AnimateImage ( (NSData) null, null, MyHandlerSetValueZero), "null data");
		}

		[Test]
		public void AnimateImageWithDataNullHandler ()
		{
			Assert.Throws<ArgumentNullException> (() => imageAnimation.AnimateImage (imageData, null, /* CGImageSourceAnimationHandler */ null), "null handler called with data");
		}

	}
}
