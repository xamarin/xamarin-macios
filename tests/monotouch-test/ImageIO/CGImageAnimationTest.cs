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
		TaskCompletionSource<bool> tcs;
		int testValue;
		CGImageAnimation imageAnimation;
		CGImageAnimationStatus status;

		void MyHandlerSetValueZero (nint index, CGImage image, out bool stop)
		{
			stop = true;
			testValue = 0;
			tcs.TrySetResult (true);
		}

		void MyHandlerSetValueOne (nint index, CGImage image, out bool stop)
		{
			stop = true;
			testValue = 1;
			tcs.TrySetResult (true);
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
			tcs = new TaskCompletionSource<bool> ();
			status = (CGImageAnimationStatus) 1; /* CGImageAnimationStatus.Ok == 0 */
			bool done = false;

			TestRuntime.RunAsync (TimeSpan.FromSeconds (30), async () => {
				if (useUrl) {
					status = imageAnimation.AnimateImage (imageUrl, null, handler);
				} else {
					status = imageAnimation.AnimateImage (imageData, null, handler);
				}
				await tcs.Task;
				done = true;
			},
				() => done);

			tcs = null;
		}

		[Test]
		public void AnimateImageWithUrl ()
		{
			CallAnimateImage ( /* useUrl */ true, MyHandlerSetValueZero);
			Assert.IsTrue (status == CGImageAnimationStatus.Ok, "status ok: handler called with url");
			Assert.AreEqual (0, testValue, "handler called with url");
		}

		[Test]
		public void AnimateImageWithData ()
		{
			CallAnimateImage ( /* useUrl */ false, MyHandlerSetValueZero);
			Assert.IsTrue (status == CGImageAnimationStatus.Ok, "status ok: handler called with data");
			Assert.AreEqual (0, testValue, "handler called with data");
		}

		[Test]
		public void AnimateImageWithUrlChangeHandler ()
		{
			CallAnimateImage ( /* useUrl */ true, MyHandlerSetValueZero);
			Assert.IsTrue (status == CGImageAnimationStatus.Ok, "status ok: first handler called with url");
			Assert.AreEqual (0, testValue, "first handler called with url" );

			CallAnimateImage ( /* useUrl */ true, MyHandlerSetValueOne);
			Assert.IsTrue (status == CGImageAnimationStatus.Ok, "status ok: second handler called with url");
			Assert.AreEqual (1, testValue, "second handler called with url");
		}

		[Test]
		public void AnimateImageWithDataChangeHandler ()
		{
			CallAnimateImage ( /* useUrl */ false, MyHandlerSetValueZero);
			Assert.IsTrue (status == CGImageAnimationStatus.Ok, "status ok: first handler called with data");
			Assert.AreEqual (0, testValue, "first handler called with data");

			CallAnimateImage ( /* useUrl */ false, MyHandlerSetValueOne);
			Assert.IsTrue (status == CGImageAnimationStatus.Ok, "status ok: second handler called with data");
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
