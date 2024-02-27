//
// Unit tests for CIDetector
//
// Authors:
//	Rolf Bjarne Kvinge (rolf@xamarin.com)
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012-2013 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using Foundation;
using CoreImage;
using CoreGraphics;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.CoreImage {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class DetectorTest {

		[Test]
		public void NullContext ()
		{
			using (var dtor = CIDetector.CreateFaceDetector (null, true)) {
			}

			using (var dtor = CIDetector.CreateFaceDetector (null, false)) {
			}

			using (var dtor = CIDetector.CreateFaceDetector (null, false, 2.0f)) {
			}

			using (var dtor = CIDetector.CreateFaceDetector (null, null, null, null)) {
			}
		}

		[Test]
		public void EmptyOptions ()
		{
			CIDetectorOptions options = new CIDetectorOptions ();
			using (var dtor = CIDetector.CreateFaceDetector (null, options)) {
				Assert.That (dtor.Description, Does.Contain ("CIFaceCoreDetector").Or.Contain ("FaceDetector"), "Description");
			}
		}
	}
}

#endif // !__WATCHOS__
