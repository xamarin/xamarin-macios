//
// UIGraphicsRenderer* Unit Tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2016 Xamarin Inc.
//

#if !__WATCHOS__ && !MONOMAC

using System;
#if XAMCORE_2_0
using Foundation;
using UIKit;
#else
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {

	[TestFixture]
	// we want the test to be availble if we use the linker
	[Preserve (AllMembers = true)]
	public class GraphicsRendererTest {

		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);
		}

		[Test]
		public void BaseDefaultFormat ()
		{
			var f = UIGraphicsRendererFormat.DefaultFormat;
			Assert.True (f.Bounds.IsEmpty, "Bounds");
			Assert.That (f.GetType ().Name, Is.EqualTo ("UIGraphicsRendererFormat"), "Name");
		}

		[Test]
		public void ImageDefaultFormat ()
		{
			var f = UIGraphicsImageRendererFormat.DefaultFormat;
			Assert.True (f.Bounds.IsEmpty, "Bounds");
			Assert.False (f.Opaque, "Opaque");
			//Assert.False (f.PrefersExtendedRange, "PrefersExtendedRange"); // new iPhone (7/7+) returns True
			Assert.That (f.Scale, Is.GreaterThan (0), "Scale"); // varies on platform
			Assert.That (f.GetType ().Name, Is.EqualTo ("UIGraphicsImageRendererFormat"), "Name");
		}

		[Test]
		public void PdfDefaultFormat ()
		{
			var f = UIGraphicsPdfRendererFormat.DefaultFormat;
			Assert.True (f.Bounds.IsEmpty, "Bounds");
			Assert.Null (f.DocumentInfo, "DocumentInfo");
			Assert.That (f.GetType ().Name, Is.EqualTo ("UIGraphicsPdfRendererFormat"), "Name");
		}
	}
}

#endif
