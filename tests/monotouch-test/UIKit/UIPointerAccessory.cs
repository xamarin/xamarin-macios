//
// Unit tests for UIPointerAccessory
//
// Authors:
//	Alex Soto <alexsoto@microsoft.com>
//
//
// Copyright Microsoft Corporation.
//

#if __IOS__
using System;
using NUnit.Framework;
using Foundation;
using UIKit;
using ObjCRuntime;

namespace MonoTouchFixtures.UIKit {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class UIPointerAccessoryTest {

		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (13, 0);
		}

		[Test]
		public void UIPointerAccessoryPositionTopTest ()
		{
			UIPointerAccessory acc = null;
			Assert.DoesNotThrow (() => acc = UIPointerAccessory.CreateArrow (UIPointerAccessoryPosition.Top), "Should not throw");
			Assert.NotNull (acc, $"{nameof (acc)} was null");
			Assert.AreEqual (acc.Position.Offset, UIPointerAccessoryPosition.Top.Offset, "Offset");
			Assert.AreEqual (acc.Position.Angle, UIPointerAccessoryPosition.Top.Angle, "Angle");
		}

		[Test]
		public void UIPointerAccessoryPositionTopRightTest ()
		{
			UIPointerAccessory acc = null;
			Assert.DoesNotThrow (() => acc = UIPointerAccessory.CreateArrow (UIPointerAccessoryPosition.TopRight), "Should not throw");
			Assert.NotNull (acc, $"{nameof (acc)} was null");
			Assert.AreEqual (acc.Position.Offset, UIPointerAccessoryPosition.TopRight.Offset, "Offset");
			Assert.AreEqual (acc.Position.Angle, UIPointerAccessoryPosition.TopRight.Angle, "Angle");
		}

		[Test]
		public void UIPointerAccessoryPositionRightTest ()
		{
			UIPointerAccessory acc = null;
			Assert.DoesNotThrow (() => acc = UIPointerAccessory.CreateArrow (UIPointerAccessoryPosition.Right), "Should not throw");
			Assert.NotNull (acc, $"{nameof (acc)} was null");
			Assert.AreEqual (acc.Position.Offset, UIPointerAccessoryPosition.Right.Offset, "Offset");
			Assert.AreEqual (acc.Position.Angle, UIPointerAccessoryPosition.Right.Angle, "Angle");
		}

		[Test]
		public void UIPointerAccessoryPositionBottomRightTest ()
		{
			UIPointerAccessory acc = null;
			Assert.DoesNotThrow (() => acc = UIPointerAccessory.CreateArrow (UIPointerAccessoryPosition.BottomRight), "Should not throw");
			Assert.NotNull (acc, $"{nameof (acc)} was null");
			Assert.AreEqual (acc.Position.Offset, UIPointerAccessoryPosition.BottomRight.Offset, "Offset");
			Assert.AreEqual (acc.Position.Angle, UIPointerAccessoryPosition.BottomRight.Angle, "Angle");
		}

		[Test]
		public void UIPointerAccessoryPositionBottomTest ()
		{
			UIPointerAccessory acc = null;
			Assert.DoesNotThrow (() => acc = UIPointerAccessory.CreateArrow (UIPointerAccessoryPosition.Bottom), "Should not throw");
			Assert.NotNull (acc, $"{nameof (acc)} was null");
			Assert.AreEqual (acc.Position.Offset, UIPointerAccessoryPosition.Bottom.Offset, "Offset");
			Assert.AreEqual (acc.Position.Angle, UIPointerAccessoryPosition.Bottom.Angle, "Angle");
		}

		[Test]
		public void UIPointerAccessoryPositionBottomLeftTest ()
		{
			UIPointerAccessory acc = null;
			Assert.DoesNotThrow (() => acc = UIPointerAccessory.CreateArrow (UIPointerAccessoryPosition.BottomLeft), "Should not throw");
			Assert.NotNull (acc, $"{nameof (acc)} was null");
			Assert.AreEqual (acc.Position.Offset, UIPointerAccessoryPosition.BottomLeft.Offset, "Offset");
			Assert.AreEqual (acc.Position.Angle, UIPointerAccessoryPosition.BottomLeft.Angle, "Angle");
		}

		[Test]
		public void UIPointerAccessoryPositionLeftTest ()
		{
			UIPointerAccessory acc = null;
			Assert.DoesNotThrow (() => acc = UIPointerAccessory.CreateArrow (UIPointerAccessoryPosition.Left), "Should not throw");
			Assert.NotNull (acc, $"{nameof (acc)} was null");
			Assert.AreEqual (acc.Position.Offset, UIPointerAccessoryPosition.Left.Offset, "Offset");
			Assert.AreEqual (acc.Position.Angle, UIPointerAccessoryPosition.Left.Angle, "Angle");
		}

		[Test]
		public void UIPointerAccessoryPositionTopLeftTest ()
		{
			UIPointerAccessory acc = null;
			Assert.DoesNotThrow (() => acc = UIPointerAccessory.CreateArrow (UIPointerAccessoryPosition.TopLeft), "Should not throw");
			Assert.NotNull (acc, $"{nameof (acc)} was null");
			Assert.AreEqual (acc.Position.Offset, UIPointerAccessoryPosition.TopLeft.Offset, "Offset");
			Assert.AreEqual (acc.Position.Angle, UIPointerAccessoryPosition.TopLeft.Angle, "Angle");
		}
	}
}
#endif // __IOS__
