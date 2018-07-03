//
// Unit tests for CIImageInitializationOptions
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
#if XAMCORE_2_0
using CoreGraphics;
using CoreImage;
using Foundation;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
#else
using MonoTouch.CoreGraphics;
using MonoTouch.CoreImage;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif

using NUnit.Framework;

namespace MonoTouchFixtures.CoreImage {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ImageInitializationOptionsTest {

		[Test]
		public void Defaults ()
		{
			var options = new CIImageInitializationOptions ();
			Assert.That (options.Dictionary.Count, Is.EqualTo (0), "Count");
			Assert.Null (options.ColorSpace, "ColorSpace");
		}

		[Test]
		public void ColorSpace ()
		{
			var options = new CIImageInitializationOptions () {
				ColorSpace = CGColorSpace.CreateDeviceRGB ()
			};
			Assert.That (options.Dictionary.Count, Is.EqualTo (1), "Count");
			Assert.NotNull (options.ColorSpace, "ColorSpace");
		}

		[Test]
		public void WithMetadataDefaults ()
		{
			TestRuntime.AssertMacSystemVersion (10, 8, throwIfOtherPlatform: false);

			var options = new CIImageInitializationOptionsWithMetadata ();
			Assert.That (options.Dictionary.Count, Is.EqualTo (0), "Count");
			Assert.Null (options.Properties, "Properties");
		}

		[Test]
		public void WithMetadataProperties ()
		{
			TestRuntime.AssertMacSystemVersion (10, 8, throwIfOtherPlatform: false);

			var suboptions = new CGImageProperties () {
				ProfileName = "Xamarin"
			};
			var options = new CIImageInitializationOptionsWithMetadata () {
				Properties = suboptions
			};
			Assert.That (options.Dictionary.Count, Is.EqualTo (1), "Count");
			Assert.That (options.Properties.ProfileName, Is.EqualTo ("Xamarin"), "Properties");
		}
	}
}

#endif // !__WATCHOS__
