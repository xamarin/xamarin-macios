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
using CoreGraphics;
using CoreImage;
using Foundation;
using ObjCRuntime;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif

using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.CoreImage {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ImageInitializationOptionsTest {

		[Test]
		public void Defaults ()
		{
			var options = new CIImageInitializationOptions ();
			Assert.That (options.Dictionary.Count, Is.EqualTo ((nuint) 0), "Count");
			Assert.Null (options.ColorSpace, "ColorSpace");
		}

		[Test]
		public void ColorSpace ()
		{
			var options = new CIImageInitializationOptions () {
				ColorSpace = CGColorSpace.CreateDeviceRGB ()
			};
			Assert.That (options.Dictionary.Count, Is.EqualTo ((nuint) 1), "Count");
			Assert.NotNull (options.ColorSpace, "ColorSpace");
		}

		[Test]
		public void WithMetadataDefaults ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 8, throwIfOtherPlatform: false);

			var options = new CIImageInitializationOptionsWithMetadata ();
			Assert.That (options.Dictionary.Count, Is.EqualTo ((nuint) 0), "Count");
			Assert.Null (options.Properties, "Properties");
		}

		[Test]
		public void WithMetadataProperties ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 8, throwIfOtherPlatform: false);

			var suboptions = new CGImageProperties () {
				ProfileName = "Xamarin"
			};
			var options = new CIImageInitializationOptionsWithMetadata () {
				Properties = suboptions
			};
			Assert.That (options.Dictionary.Count, Is.EqualTo ((nuint) 1), "Count");
			Assert.That (options.Properties.ProfileName, Is.EqualTo ("Xamarin"), "Properties");
		}
	}
}

#endif // !__WATCHOS__
