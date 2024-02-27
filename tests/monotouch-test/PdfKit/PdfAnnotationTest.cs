//
// PdfAnnotation Unit Tests
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2017 Microsoft Inc.
//

#if __IOS__ || MONOMAC

using System;

using CoreGraphics;
using Foundation;
using ObjCRuntime;
using PdfKit;

using NUnit.Framework;

namespace MonoTouchFixtures.PdfKit {

	[TestFixture]
	// we want the test to be available if we use the linker
	[Preserve (AllMembers = true)]
	public class PdfAnnotationTest {
		[OneTimeSetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);
		}

		[Test]
		public void QuadrilateralPoints ()
		{
			using (var obj = new PdfAnnotation ()) {
				Assert.IsNotNull (obj.QuadrilateralPoints, "Q1");
				Assert.AreEqual (0, obj.QuadrilateralPoints.Length, "Q1b");

				var points = new CGPoint []
				{
					new CGPoint (0, 1),
					new CGPoint (2, 3),
					new CGPoint (4, 5),
					new CGPoint (6, 7),
				};

				obj.QuadrilateralPoints = points;
				Assert.AreEqual (points, obj.QuadrilateralPoints, "Q2");

				obj.QuadrilateralPoints = null;
				Assert.IsNotNull (obj.QuadrilateralPoints, "Q3");
				Assert.AreEqual (0, obj.QuadrilateralPoints.Length, "Q3b");
			}
		}
	}
}

#endif // __IOS__ || MONOMAC
