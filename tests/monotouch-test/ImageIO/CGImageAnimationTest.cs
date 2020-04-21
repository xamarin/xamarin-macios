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

		private void CallAnimateImage (bool useUrl, CGImageAnimation.CGImageSourceAnimationBlock block)
		{
			TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool> ();
			bool done = false;

			TestRuntime.RunAsync (TimeSpan.FromSeconds (30), async () => {
				if (useUrl) {
					imageAnimation.AnimateImage (imageUrl, null, block);
				} else {
					imageAnimation.AnimateImage (imageData, null, block);
				}
				await taskCompletionSource.Task;
				done = true;
			},
				() => done);
		}

		[Test]
		public void AnimateImageWithUrl ()
		{
			CallAnimateImage ( /* useUrl */ true, MyBlockSetValueZero);
			Assert.AreEqual (0, testValue, "block called with url");
		}

		[Test]
		public void AnimateImageWithData ()
		{
			CallAnimateImage ( /* useUrl */ false, MyBlockSetValueZero);
			Assert.AreEqual (0, testValue, "block called with data");
		}

		[Test]
		public void AnimateImageWithUrlChangeBlock ()
		{
			CallAnimateImage ( /* useUrl */ true, MyBlockSetValueZero);
			Assert.AreEqual (0, testValue, "first block called with url" );

			CallAnimateImage ( /* useUrl */ true, MyBlockSetValueOne);
			Assert.AreEqual (1, testValue, "second block called with url");
		}

		[Test]
		public void AnimateImageWithDataChangeBlock ()
		{
			CallAnimateImage ( /* useUrl */ false, MyBlockSetValueZero);
			Assert.AreEqual (0, testValue, "first block called with data");

			CallAnimateImage ( /* useUrl */ false, MyBlockSetValueOne);
			Assert.AreEqual (1, testValue, "second block called with data");
		}

		[Test]
		public void AnimateImageWithUrlNullUrl ()
		{
			Assert.Throws<ArgumentNullException>( () => imageAnimation.AnimateImage ( (NSUrl) null, null, MyBlockSetValueZero), "null url");
		}

		[Test]
		public void AnimateImageWithUrlNullBlock ()
		{
			Assert.Throws<ArgumentNullException>( () => imageAnimation.AnimateImage (imageUrl, null, /* CGImageSourceAnimationBlock */ null), "null block called with url");
		}

		[Test]
		public void AnimateImageWithDataNullData ()
		{
			Assert.Throws<ArgumentNullException>( () => imageAnimation.AnimateImage ( (NSData) null, null, MyBlockSetValueZero), "null data");
		}

		[Test]
		public void AnimateImageWithDataNullBlock ()
		{
			Assert.Throws<ArgumentNullException>( () => imageAnimation.AnimateImage (imageData, null, /* CGImageSourceAnimationBlock */ null), "null block called with data");
		}

	}
}
