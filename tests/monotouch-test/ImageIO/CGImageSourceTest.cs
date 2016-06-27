// Copyright 2012-2013 Xamarin Inc. All rights reserved

using System;
using System.Drawing;
#if XAMCORE_2_0
using Foundation;
using CoreGraphics;
using ImageIO;
using ObjCRuntime;
using UIKit;
#else
using MonoTouch;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.ImageIO;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.ImageIO {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CGImageSourceTest {
		const string filename = "xamarin2.png";
		
		[Test]
		public void FromUrlTest ()
		{
			using (var img = CGImageSource.FromUrl (NSUrl.FromFilename (filename))) {
				Assert.NotNull (img, "#a1");
			}
			
			using (var img = CGImageSource.FromUrl (NSUrl.FromFilename (filename), new CGImageOptions ())) {
				Assert.NotNull (img, "#b1");
			}
			
			using (var img = CGImageSource.FromUrl (NSUrl.FromFilename (filename), null)) {
				Assert.NotNull (img, "#c1");
			}
		}
		
		[Test]
		public void FromDataProviderTest ()
		{
			using (var dp = new CGDataProvider (filename)) {
				using (var img = CGImageSource.FromDataProvider (dp)) {
					Assert.NotNull (img, "#a1");
				}
			}
			
			using (var dp = new CGDataProvider (filename)) {
				using (var img = CGImageSource.FromDataProvider (dp, new CGImageOptions ())) {
					Assert.NotNull (img, "#b1");
				}
			}
			
			using (var dp = new CGDataProvider (filename)) {
				using (var img = CGImageSource.FromDataProvider (dp, null)) {
					Assert.NotNull (img, "#c1");
				}
			}
		}
		
		[Test]
		public void FromDataTest ()
		{
			NSData data = NSData.FromFile (filename);
			
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
			using (var imgsrc = CGImageSource.FromUrl (NSUrl.FromFilename (filename))) {
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
			using (var imgsrc = CGImageSource.FromUrl (NSUrl.FromFilename (filename))) {
				using (var img = imgsrc.CreateThumbnail (0, null)) {
					Assert.NotNull (img, "#a1");
				}
				using (var img = imgsrc.CreateThumbnail (0, new CGImageThumbnailOptions ())) {
					Assert.NotNull (img, "#b1");
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

				using (var imageSource = CGImageSource.FromUrl (NSUrl.FromFilename (filename))) {
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
			}
			finally {
				Dlfcn.dlclose (lib);
			}
		}

		[Test]
		public void GetProperties ()
		{
			using (var imageSource = CGImageSource.FromUrl (NSUrl.FromFilename (filename))) {
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

		[Test]
		public void CopyMetadata ()
		{
			TestRuntime.AssertXcodeVersion (5, 0);

			using (var imageSource = CGImageSource.FromUrl (NSUrl.FromFilename (filename))) {
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

			using (var imageSource = CGImageSource.FromUrl (NSUrl.FromFilename (filename))) {
				imageSource.RemoveCache (0);
			}
		}
	}
}
