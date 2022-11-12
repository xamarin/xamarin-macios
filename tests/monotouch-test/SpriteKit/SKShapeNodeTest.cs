//
// Unit tests for SKShapeNodeTest
//
// Authors:
//	Alex Soto <alex.soto@xamarin.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using System.Drawing;
using CoreGraphics;
using Foundation;
using SpriteKit;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.SpriteKit {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class SKShapeNodeTest {

		[Test]
		public void FromPointsTest ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 10, throwIfOtherPlatform: false);

			var pts = new [] {
				new CGPoint (0, 0),
				new CGPoint (320, 568)
			};

			var result = SKShapeNode.FromPoints (pts);
			Assert.IsNotNull (result, "result should not be null");
		}

		[Test]
		public void FromPointsOffsetTest ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 10, throwIfOtherPlatform: false);

			var pts = new [] {
				new CGPoint (0, 0),
				new CGPoint (320, 568)
			};

			var result = SKShapeNode.FromPoints (pts, 1, 1);
			Assert.IsNotNull (result, "result should not be null");

			Assert.Throws<InvalidOperationException> (() => SKShapeNode.FromPoints (pts, 1, 2));
		}

		[Test]
		public void FromSplinePointsTest ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 10, throwIfOtherPlatform: false);

			var pts = new [] {
				new CGPoint (0, 0),
				new CGPoint (320, 568)
			};

			var result = SKShapeNode.FromSplinePoints (pts);
			Assert.IsNotNull (result, "result should not be null");
		}

		[Test]
		public void FromSplinePointsOffsetTest ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 10, throwIfOtherPlatform: false);

			var pts = new [] {
				new CGPoint (0, 0),
				new CGPoint (320, 568)
			};

			var result = SKShapeNode.FromSplinePoints (pts, 1, 1);
			Assert.IsNotNull (result, "result should not be null");

			Assert.Throws<InvalidOperationException> (() => SKShapeNode.FromSplinePoints (pts, 1, 2));
		}
	}
}

#endif // !__WATCHOS__
