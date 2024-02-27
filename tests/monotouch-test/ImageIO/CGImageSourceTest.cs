// Copyright 2012-2013 Xamarin Inc. All rights reserved

using System;
using System.Drawing;
using Foundation;
using CoreGraphics;
using ImageIO;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.ImageIO {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CGImageSourceTest {
		NSUrl fileUrl = NSBundle.MainBundle.GetUrlForResource ("xamarin2", "png");

		[Test]
		public void FromUrlTest ()
		{
			using (var img = CGImageSource.FromUrl (fileUrl)) {
				Assert.NotNull (img, "#a1");
			}

			using (var img = CGImageSource.FromUrl (fileUrl, new CGImageOptions ())) {
				Assert.NotNull (img, "#b1");
			}

			using (var img = CGImageSource.FromUrl (fileUrl, null)) {
				Assert.NotNull (img, "#c1");
			}
		}

		[Test]
		public void FromDataProviderTest ()
		{
			var file = NSBundle.MainBundle.PathForResource ("xamarin2", "png");
			using (var dp = new CGDataProvider (file)) {
				using (var img = CGImageSource.FromDataProvider (dp)) {
					Assert.NotNull (img, "#a1");
				}
			}

			using (var dp = new CGDataProvider (file)) {
				using (var img = CGImageSource.FromDataProvider (dp, new CGImageOptions ())) {
					Assert.NotNull (img, "#b1");
				}
			}

			using (var dp = new CGDataProvider (file)) {
				using (var img = CGImageSource.FromDataProvider (dp, null)) {
					Assert.NotNull (img, "#c1");
				}
			}
		}

		[Test]
		public void FromDataTest ()
		{
			NSData data = NSData.FromFile (NSBundle.MainBundle.PathForResource ("xamarin2", "png"));

			using (var img = CGImageSource.FromData (data)) {
				Assert.NotNull (img, "#a1");
			}

			using (var img = CGImageSource.FromData (data, new CGImageOptions ())) {
				Assert.NotNull (img, "#b1");
			}

			using (var img = CGImageSource.FromData (data, null)) {
				Assert.NotNull (img, "#c1");
			}
		}

		[Test]
		public void CreateImageTest ()
		{
			using (var imgsrc = CGImageSource.FromUrl (fileUrl)) {
				using (var img = imgsrc.CreateImage (0, null)) {
					Assert.NotNull (img, "#a1");
				}
				using (var img = imgsrc.CreateImage (0, new CGImageOptions ())) {
					Assert.NotNull (img, "#b1");
				}
			}
		}

		[Test]
		public void CreateThumbnailTest ()
		{
			using (var imgsrc = CGImageSource.FromUrl (fileUrl)) {
				using (var img = imgsrc.CreateThumbnail (0, null)) {
#if NET
					Assert.Null (img, "#a1");
#else
					Assert.NotNull (img, "#a1");
					Assert.AreEqual (IntPtr.Zero, img.Handle, "#a2");
#endif
				}
				using (var img = imgsrc.CreateThumbnail (0, new CGImageThumbnailOptions ())) {
#if NET
					Assert.Null (img, "#b1");
#else
					Assert.NotNull (img, "#b1");
					Assert.AreEqual (IntPtr.Zero, img.Handle, "#b2");
#endif
				}
			}
		}

		[Test]
		public void CreateIncrementalTest ()
		{
			using (var img = CGImageSource.CreateIncremental (null)) {
				Assert.NotNull (img, "#a1");
			}

			using (var img = CGImageSource.CreateIncremental (new CGImageOptions ())) {
				Assert.NotNull (img, "#b1");
			}
		}

		[Test]
		public void CopyProperties ()
		{
			// what we had to answer with 5.2 for http://stackoverflow.com/q/10753108/220643
			IntPtr lib = Dlfcn.dlopen (Constants.ImageIOLibrary, 0);
			try {
				NSString kCGImageSourceShouldCache = Dlfcn.GetStringConstant (lib, "kCGImageSourceShouldCache");
				NSString kCGImagePropertyPixelWidth = Dlfcn.GetStringConstant (lib, "kCGImagePropertyPixelWidth");
				NSString kCGImagePropertyPixelHeight = Dlfcn.GetStringConstant (lib, "kCGImagePropertyPixelHeight");

				using (var imageSource = CGImageSource.FromUrl (fileUrl)) {
					using (var dict = new NSMutableDictionary ()) {
						dict [kCGImageSourceShouldCache] = NSNumber.FromBoolean (false);
						using (var props = imageSource.CopyProperties (dict)) {
							Assert.Null (props.ValueForKey (kCGImagePropertyPixelWidth), "kCGImagePropertyPixelWidth");
							Assert.Null (props.ValueForKey (kCGImagePropertyPixelHeight), "kCGImagePropertyPixelHeight");
							NSNumber n = (NSNumber) props ["FileSize"];
							// image is "optimized" for devices (and a lot bigger at 10351 bytes ;-)
							Assert.That ((int) n, Is.AtLeast (7318), "FileSize");
						}
					}
				}
			} finally {
				Dlfcn.dlclose (lib);
			}
		}

		[Test]
		public void GetProperties ()
		{
			using (var imageSource = CGImageSource.FromUrl (fileUrl)) {
				CGImageOptions options = new CGImageOptions () { ShouldCache = false };

				var props = imageSource.GetProperties (options);
				Assert.Null (props.PixelWidth, "PixelHeight-0");
				Assert.Null (props.PixelHeight, "PixelWidth-0");
				// image is "optimized" for devices (and a lot bigger at 10351 bytes ;-)
				Assert.That (props.FileSize, Is.AtLeast (7318), "FileSize");

				props = imageSource.GetProperties (0, options);
				Assert.AreEqual (57, props.PixelWidth, "PixelHeight");
				Assert.AreEqual (57, props.PixelHeight, "PixelWidth");
				Assert.AreEqual (CGImageColorModel.RGB, props.ColorModel, "ColorModel");
				Assert.AreEqual (8, props.Depth, "Depth");
			}
		}

#if !MONOMAC // CopyMetadata and RemoveCache not available on mac
		[Test]
		public void CopyMetadata ()
		{
			TestRuntime.AssertXcodeVersion (5, 0);

			using (var imageSource = CGImageSource.FromUrl (fileUrl)) {
				CGImageOptions options = new CGImageOptions () { ShouldCacheImmediately = true };
				using (CGImageMetadata metadata = imageSource.CopyMetadata (0, options)) {
					Console.WriteLine ();
				}
			}
		}

		[Test]
		public void RemoveCache ()
		{
			TestRuntime.AssertXcodeVersion (5, 0);

			using (var imageSource = CGImageSource.FromUrl (fileUrl)) {
				imageSource.RemoveCache (0);
			}
		}
#endif
	}
}
