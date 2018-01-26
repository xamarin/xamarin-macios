//
// Unit tests for CGPDFInfo
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2012-2014 Xamarin Inc. All rights reserved.
//

using System;
using System.Drawing;
#if XAMCORE_2_0
using Foundation;
using ObjCRuntime;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using CoreGraphics;
#else
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
#endif

using NUnit.Framework;

#if XAMCORE_2_0
using RectangleF=CoreGraphics.CGRect;
using SizeF=CoreGraphics.CGSize;
using PointF=CoreGraphics.CGPoint;
#else
using nfloat=global::System.Single;
using nint=global::System.Int32;
using nuint=global::System.UInt32;
#endif

namespace MonoTouchFixtures.CoreGraphics {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PDFInfoTest {

		static public CGPDFInfo GetInfo ()
		{
			return new CGPDFInfo () {
				AllowsCopying = true,
				AllowsPrinting = true,
				Author = "My Name",
				Creator = "My Creator",
				EncryptionKeyLength = 123,
				Keywords = new string [] { "K1", "K2" },
				OwnerPassword = "My OwnerPassword",
				Subject = "My Subject",
				Title = "My Title",
				UserPassword = "My UserPassword",
			};
		}

#if !MONOMAC // Not on mac
		[Test]
		public void ToDictionary ()
		{
			if (TestRuntime.CheckXcodeVersion (9,3))
				Assert.Ignore ("Crash (at least on devices) with iOS 11.3 beta 1");
			// Bug #8879
			var info = GetInfo ();
			UIGraphics.BeginPDFContext("file", RectangleF.Empty, info); 
		}

		[Test]
		public void ToDictionaryWithPermissions ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);

			var filename = Environment.GetFolderPath (Environment.SpecialFolder.CommonDocuments) + "/t.pdf";

			var info = GetInfo ();
			info.AccessPermissions = CGPDFAccessPermissions.AllowsContentCopying;

			using (var url = new NSUrl (filename)) {
				using (var ctx = new CGContextPDF (url, new RectangleF (0, 0, 1000, 1000), info)) {
					Assert.IsNotNull (ctx, "1");
				}
				using (var consumer = new CGDataConsumer (url)) {
					using (var ctx = new CGContextPDF (consumer, new RectangleF (0, 0, 1000, 1000), info)) {
						Assert.IsNotNull (ctx, "2");
					}
				}
			}
		}
#endif
	}
}
