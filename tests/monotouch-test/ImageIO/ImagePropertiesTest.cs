//
// CGImageProperties
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

using System;
#if XAMCORE_2_0
using Foundation;
using ImageIO;
using ObjCRuntime;
#else
using MonoTouch;
using MonoTouch.Foundation;
using MonoTouch.ImageIO;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.ImageIO {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ImagePropertiesTest {

		[Test]
		public void CameraMaker ()
		{
			// since only Canon and Nikon have dictionary key values I assume the others are "placeholder" at the moment !?!
			Assert.NotNull (CGImageProperties.MakerCanonDictionary, "MakerCanonDictionary");
			Assert.NotNull (CGImageProperties.MakerNikonDictionary, "MakerNikonDictionary");

			// iOS 8.3 finally fixed them :)
			if (TestRuntime.CheckXcodeVersion (6, 3) || TestRuntime.CheckMacSystemVersion (10, 7, throwIfOtherPlatform: false)) {
				Assert.NotNull (CGImageProperties.MakerMinoltaDictionary, "MakerMinoltaDictionary");
				Assert.NotNull (CGImageProperties.MakerFujiDictionary, "MakerFujiDictionary");
				Assert.NotNull (CGImageProperties.MakerOlympusDictionary, "MakerOlympusDictionary");
				Assert.NotNull (CGImageProperties.MakerPentaxDictionary, "MakerPentaxDictionary");
				return;
			}

			// documented as new in iOS 4.0 - https://developer.apple.com/library/ios/#documentation/GraphicsImaging/Reference/CGImageProperties_Reference/Reference/reference.html
			// but symbols are not available in iOS 5.1.1
			// this test will fail if Apple decide to include them in the future
			IntPtr lib = Dlfcn.dlopen (Constants.ImageIOLibrary, 0);
			try {
				Assert.That (Dlfcn.dlsym (lib, "kCGImagePropertyMakerMinoltaDictionary"), Is.EqualTo (IntPtr.Zero), "kCGImagePropertyMakerMinoltaDictionary");
				Assert.That (Dlfcn.dlsym (lib, "kCGImagePropertyMakerFujiDictionary"), Is.EqualTo (IntPtr.Zero), "kCGImagePropertyMakerFujiDictionary");
				Assert.That (Dlfcn.dlsym (lib, "kCGImagePropertyMakerOlympusDictionary"), Is.EqualTo (IntPtr.Zero), "kCGImagePropertyMakerOlympusDictionary");
				Assert.That (Dlfcn.dlsym (lib, "kCGImagePropertyMakerPentaxDictionary"), Is.EqualTo (IntPtr.Zero), "kCGImagePropertyMakerPentaxDictionary");
			}
			finally {
				Dlfcn.dlclose (lib);
			}
		}

		[Test]
		public void Exif43Fields ()
		{
			// documented as new in iOS 4.3 - https://developer.apple.com/library/ios/#documentation/GraphicsImaging/Reference/CGImageProperties_Reference/Reference/reference.html
			IntPtr lib = Dlfcn.dlopen (Constants.ImageIOLibrary, 0);
			try {
				var hasSymbols = true;
#if __MACOS__
				if (!TestRuntime.CheckMacSystemVersion (10, 9))
					hasSymbols = false;
#else
				if (!TestRuntime.CheckXcodeVersion (4, 5))
					hasSymbols = false;
#endif
				if (hasSymbols) {
					// symbols are available in iOS 6.0 beta 1
					Assert.That (Dlfcn.dlsym (lib, "kCGImagePropertyExifCameraOwnerName"), Is.Not.EqualTo (IntPtr.Zero), "kCGImagePropertyExifCameraOwnerName");
					Assert.That (Dlfcn.dlsym (lib, "kCGImagePropertyExifBodySerialNumber"), Is.Not.EqualTo (IntPtr.Zero), "kCGImagePropertyExifBodySerialNumber");
					Assert.That (Dlfcn.dlsym (lib, "kCGImagePropertyExifLensSpecification"), Is.Not.EqualTo (IntPtr.Zero), "kCGImagePropertyExifLensSpecification");
					Assert.That (Dlfcn.dlsym (lib, "kCGImagePropertyExifLensMake"), Is.Not.EqualTo (IntPtr.Zero), "kCGImagePropertyExifLensMake");
					Assert.That (Dlfcn.dlsym (lib, "kCGImagePropertyExifLensModel"), Is.Not.EqualTo (IntPtr.Zero), "kCGImagePropertyExifLensModel");
					Assert.That (Dlfcn.dlsym (lib, "kCGImagePropertyExifLensSerialNumber"), Is.Not.EqualTo (IntPtr.Zero), "kCGImagePropertyExifLensSerialNumber");
				} else {
					// but symbols are not available in iOS 5.1.1
					Assert.That (Dlfcn.dlsym (lib, "kCGImagePropertyExifCameraOwnerName"), Is.EqualTo (IntPtr.Zero), "kCGImagePropertyExifCameraOwnerName");
					Assert.That (Dlfcn.dlsym (lib, "kCGImagePropertyExifBodySerialNumber"), Is.EqualTo (IntPtr.Zero), "kCGImagePropertyExifBodySerialNumber");
					Assert.That (Dlfcn.dlsym (lib, "kCGImagePropertyExifLensSpecification"), Is.EqualTo (IntPtr.Zero), "kCGImagePropertyExifLensSpecification");
					Assert.That (Dlfcn.dlsym (lib, "kCGImagePropertyExifLensMake"), Is.EqualTo (IntPtr.Zero), "kCGImagePropertyExifLensMake");
					Assert.That (Dlfcn.dlsym (lib, "kCGImagePropertyExifLensModel"), Is.EqualTo (IntPtr.Zero), "kCGImagePropertyExifLensModel");
					Assert.That (Dlfcn.dlsym (lib, "kCGImagePropertyExifLensSerialNumber"), Is.EqualTo (IntPtr.Zero), "kCGImagePropertyExifLensSerialNumber");
				}
			}
			finally {
				Dlfcn.dlclose (lib);
			}
		}
	}
}
