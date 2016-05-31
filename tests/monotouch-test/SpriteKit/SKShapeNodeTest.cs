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
#if XAMCORE_2_0
using Foundation;
using UIKit;
using SpriteKit;
#else
using MonoTouch.Foundation;
using MonoTouch.SpriteKit;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

#if XAMCORE_2_0
using RectangleF=CoreGraphics.CGRect;
using SizeF=CoreGraphics.CGSize;
using PointF=CoreGraphics.CGPoint;
#else
using nfloat = global::System.Single;
using nint = global::System.Int32;
using nuint = global::System.UInt32;
#endif

namespace MonoTouchFixtures.SpriteKit {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class SKShapeNodeTest {

		[Test]
		public void FromPointsTest ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (8, 0))
				Assert.Ignore ("requires iOS8+");

			var pts = new[] {
				new PointF (0, 0),
				new PointF (320, 568)
			};

			var result = SKShapeNode.FromPoints (pts);
			Assert.IsNotNull (result, "result should not be null");
		}

		[Test]
		public void FromPointsOffsetTest ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (8, 0))
				Assert.Ignore ("requires iOS8+");

			var pts = new [] {
				new PointF (0, 0),
				new PointF (320, 568)
			};

			var result = SKShapeNode.FromPoints (pts, 1, 1);
			Assert.IsNotNull (result, "result should not be null");

			Assert.Throws<InvalidOperationException> (() => SKShapeNode.FromPoints (pts, 1, 2));
		}

		[Test]
		public void FromSplinePointsTest ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (8, 0))
				Assert.Ignore ("requires iOS8+");

			var pts = new[] {
				new PointF (0, 0),
				new PointF (320, 568)
			};

			var result = SKShapeNode.FromSplinePoints (pts);
			Assert.IsNotNull (result, "result should not be null");
		}

		[Test]
		public void FromSplinePointsOffsetTest ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (8, 0))
				Assert.Ignore ("requires iOS8+");

			var pts = new [] {
				new PointF (0, 0),
				new PointF (320, 568)
			};

			var result = SKShapeNode.FromSplinePoints (pts, 1, 1);
			Assert.IsNotNull (result, "result should not be null");

			Assert.Throws<InvalidOperationException> (() => SKShapeNode.FromSplinePoints (pts, 1, 2));
		}
	}
}

#endif // !__WATCHOS__

