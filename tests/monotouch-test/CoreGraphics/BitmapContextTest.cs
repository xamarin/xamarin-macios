//
// Unit tests for CGBitmapContext
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012-2014 Xamarin Inc. All rights reserved.
//

using System;
using Foundation;
using CoreGraphics;
using NUnit.Framework;

namespace MonoTouchFixtures.CoreGraphics {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class BitmapContextTest {

		[Test]
		public void Ctor_CGBitmapFlags ()
		{
			// https://bugzilla.xamarin.com/show_bug.cgi?id=7441
			byte [] data = new byte [400];
			using (CGColorSpace space = CGColorSpace.CreateDeviceRGB ()) {
				// According to Apple "This value is equivalent to kCGImageAlphaNoneSkipLast" which is not true (at least in this "context" ;-)
				Assert.Throws<Exception> (delegate { new CGBitmapContext (data, 10, 10, 8, 40, space, CGBitmapFlags.None); }, "None");

				using (CGBitmapContext c = new CGBitmapContext (data, 10, 10, 8, 40, space, CGBitmapFlags.PremultipliedLast)) {
					Assert.That (c.Handle, Is.Not.EqualTo (IntPtr.Zero), "PremultipliedLast");
				}

				using (CGBitmapContext c = new CGBitmapContext (data, 10, 10, 8, 40, space, CGBitmapFlags.PremultipliedFirst)) {
					Assert.That (c.Handle, Is.Not.EqualTo (IntPtr.Zero), "PremultipliedFirst");
				}

				Assert.Throws<Exception> (delegate { new CGBitmapContext (data, 10, 10, 8, 40, space, CGBitmapFlags.Last); }, "Last");

				Assert.Throws<Exception> (delegate { new CGBitmapContext (data, 10, 10, 8, 40, space, CGBitmapFlags.First); }, "First");

				using (CGBitmapContext c = new CGBitmapContext (data, 10, 10, 8, 40, space, CGBitmapFlags.NoneSkipLast)) {
					Assert.That (c.Handle, Is.Not.EqualTo (IntPtr.Zero), "NoneSkipLast");
				}

				using (CGBitmapContext c = new CGBitmapContext (data, 10, 10, 8, 40, space, CGBitmapFlags.NoneSkipFirst)) {
					Assert.That (c.Handle, Is.Not.EqualTo (IntPtr.Zero), "NoneSkipFirst");
				}

				Assert.Throws<Exception> (delegate { new CGBitmapContext (data, 10, 10, 8, 40, space, CGBitmapFlags.Only); }, "Only");
			}
		}

		[Test]
		public void Ctor_CGImageAlphaInfo ()
		{
			// https://bugzilla.xamarin.com/show_bug.cgi?id=7441
			byte [] data = new byte [400];
			using (CGColorSpace space = CGColorSpace.CreateDeviceRGB ()) {
				// According to Apple "This value is equivalent to kCGImageAlphaNoneSkipLast" which is not true (at least in this "context" ;-)
				Assert.Throws<Exception> (delegate { new CGBitmapContext (data, 10, 10, 8, 40, space, CGImageAlphaInfo.None); }, "None");

				using (CGBitmapContext c = new CGBitmapContext (data, 10, 10, 8, 40, space, CGImageAlphaInfo.PremultipliedLast)) {
					Assert.That (c.Handle, Is.Not.EqualTo (IntPtr.Zero), "PremultipliedLast");
				}

				using (CGBitmapContext c = new CGBitmapContext (data, 10, 10, 8, 40, space, CGImageAlphaInfo.PremultipliedFirst)) {
					Assert.That (c.Handle, Is.Not.EqualTo (IntPtr.Zero), "PremultipliedFirst");
				}

				Assert.Throws<Exception> (delegate { new CGBitmapContext (data, 10, 10, 8, 40, space, CGImageAlphaInfo.Last); }, "Last");

				Assert.Throws<Exception> (delegate { new CGBitmapContext (data, 10, 10, 8, 40, space, CGImageAlphaInfo.First); }, "First");

				using (CGBitmapContext c = new CGBitmapContext (data, 10, 10, 8, 40, space, CGImageAlphaInfo.NoneSkipLast)) {
					Assert.That (c.Handle, Is.Not.EqualTo (IntPtr.Zero), "NoneSkipLast");
				}

				using (CGBitmapContext c = new CGBitmapContext (data, 10, 10, 8, 40, space, CGImageAlphaInfo.NoneSkipFirst)) {
					Assert.That (c.Handle, Is.Not.EqualTo (IntPtr.Zero), "NoneSkipFirst");
				}

				Assert.Throws<Exception> (delegate { new CGBitmapContext (data, 10, 10, 8, 40, space, CGImageAlphaInfo.Only); }, "Only");
			}
		}

		[Test]
		public void Ctor_CGColorSpace_Null ()
		{
			byte [] data = new byte [400];
			// a null colorspace is not always accepted - that will return an invalid (IntPtr.Zero) handle and CGContext.set_Handle will throw
			Assert.Throws<Exception> (delegate { new CGBitmapContext (data, 10, 10, 8, 40, null, CGImageAlphaInfo.NoneSkipFirst); }, "null");

			// OTOH a null colorspace is possible with the valid parameters, e.g. bug #25600, so we can't throw a ANE blindly
			using (var context = new CGBitmapContext (null, 16, 32, 8, 0, null, CGImageAlphaInfo.Only)) {
				Assert.That (context.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
				Assert.Null (context.ColorSpace, "ColorSpace");
			}
		}

		[Test]
		public void ToImage ()
		{
			byte [] data = new byte [400];
			using (CGColorSpace space = CGColorSpace.CreateDeviceRGB ()) {
				CGBitmapContext c = new CGBitmapContext (data, 10, 10, 8, 40, space, CGImageAlphaInfo.PremultipliedLast);
				using (var img = c.ToImage ())
					Assert.NotNull (img, "ToImage");
				c.Dispose (); // Handle is now 0x0
				Assert.Null (c.ToImage (), "ToImage/Disposed");
			}
		}
	}
}
