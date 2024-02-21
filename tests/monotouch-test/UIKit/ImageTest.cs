// Copyright 2011, 2013 Xamarin Inc. All rights reserved

#if !MONOMAC
using System;
using System.Drawing;
using System.IO;
using System.Threading;
using CoreGraphics;
using Foundation;
using UIKit;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ImageTest {

		[Test]
		public void TestAutorelease ()
		{
			int iWidth = 2500;
			int iHeight = 2500;
			int r = 0;

			try {
				for (r = 0; r < 1000; r++) {
					using (CGColorSpace oColorSpace = CGColorSpace.CreateDeviceRGB ()) {
						using (CGBitmapContext oContext = new CGBitmapContext (null, iWidth, iHeight, 8, iWidth * 4, oColorSpace, CGImageAlphaInfo.PremultipliedFirst)) {
							using (CGImage oImage = oContext.ToImage ()) {
								using (var img = UIImage.FromImage (oImage)) {
									Assert.That (img.Size.Width, Is.GreaterThan ((nfloat) 0), "w" + r.ToString ());
									Assert.That (img.Size.Height, Is.GreaterThan ((nfloat) 0), "h" + r.ToString ());
								}
							}
						}
					}
				}
			} catch (AssertionException) {
				throw;
			} catch (Exception ex) {
				throw new Exception (string.Format ("Test failed after {0} iterations: {1}", r, ex.Message), ex);
			}
		}

		[Test]
		public void CGImageBackend ()
		{
			string file = Path.Combine (NSBundle.MainBundle.ResourcePath, "basn3p08.png");
			using (var original = UIImage.FromFile (file)) {
				Assert.That (original.CGImage.Width, Is.EqualTo ((nint) 32), "Width-a");
				Assert.That (original.CGImage.Height, Is.EqualTo ((nint) 32), "Height-a");
				var half = new CGSize (16f, 16f);
				IntPtr handle = original.CGImage.Handle;
				using (var resized = original.Scale (half)) {
					Assert.That (resized.CGImage.Height, Is.EqualTo (resized.CGImage.Width), "Width-Height-identical");
					// caching of the backing CGImage occurs on devices (but not always)
					if (TestRuntime.IsSimulatorOrDesktop) {
						Assert.That ((nint) 16, Is.EqualTo (resized.CGImage.Width), "half");
					} else {
						var h = resized.CGImage.Height;
						Assert.True (h == 16 || h == 32, "mostly");
					}
					Assert.That (handle, Is.Not.EqualTo (resized.CGImage.Handle), "Handle");
				}
			}
		}

		[Test]
		public void CreateAnimatedImage ()
		{
			using (var i = UIImage.CreateAnimatedImage ("xamarin", UIEdgeInsets.Zero, 1d)) {
				Assert.That (i.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
				Assert.That (i.Images.Length, Is.EqualTo (3), "3 images");
				Assert.True (i.Description.Contains ("UIAnimatedImage"), "UIAnimatedImage");
			}
		}

		[Test]
		public void FromImage_Null ()
		{
			Assert.Throws<ArgumentNullException> (() => UIImage.FromImage ((CGImage) null), "CGImage");
		}
	}
}
#endif
