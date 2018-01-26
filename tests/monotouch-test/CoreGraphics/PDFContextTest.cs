//
// Unit tests for CGContextPDF
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

using System;
using System.Drawing;
using System.IO;
#if XAMCORE_2_0
using CoreGraphics;
using Foundation;
#else
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
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
	public class PDFContextTest {

		static string filename;

		static PDFContextTest ()
		{
			filename = Environment.GetFolderPath (Environment.SpecialFolder.CommonDocuments) + "/t.pdf";
		}

		[Test]
		public void Context_Url ()
		{
			using (var url = new NSUrl (filename))
			using (var ctx = new CGContextPDF (url)) {
				ctx.BeginPage (PDFInfoTest.GetInfo ());
				ctx.SetUrl (url, RectangleF.Empty);
				ctx.EndPage ();
			}
		}

		[Test]
		public void Context_Url_Rect ()
		{
			var rect = new RectangleF (10, 10, 100, 100);
			using (var url = new NSUrl (filename))
			using (var ctx = new CGContextPDF (url, rect)) {
				ctx.BeginPage ((CGPDFPageInfo) null);
				ctx.SetDestination ("xamarin", rect);
				ctx.EndPage ();
			}
		}

		[Test]
		public void Context_Url_Rect_Info ()
		{
			using (var url = new NSUrl (filename))
			using (var ctx = new CGContextPDF (url, new RectangleF (0, 0, 1000, 1000), PDFInfoTest.GetInfo ())) {
				ctx.AddDestination ("monkey", PointF.Empty);
				ctx.Close ();
			}
		}

		[Test]
		[Ignore ("Crash on simulator using xcode 9.3 beta 1")]
		public void Constructors ()
		{
			Assert.Throws<Exception> (() => new CGContextPDF ((CGDataConsumer) null), "null CGDataConsumer");

			Assert.Throws<Exception> (() => new CGContextPDF ((CGDataConsumer) null, RectangleF.Empty), "null CGDataConsumer, Empty");

			Assert.Throws<Exception> (() => new CGContextPDF ((CGDataConsumer) null, RectangleF.Empty, null), "null CGDataConsumer, Empty, null");

			Assert.Throws<Exception> (() => new CGContextPDF ((CGDataConsumer) null, null), "null CGDataConsumer, null");

			Assert.Throws<Exception> (() => new CGContextPDF ((NSUrl) null), "null NSUrl");

			Assert.Throws<Exception> (() => new CGContextPDF ((NSUrl) null, RectangleF.Empty), "null NSUrl, Empty");

			Assert.Throws<Exception> (() => new CGContextPDF ((NSUrl) null, RectangleF.Empty, null), "null NSUrl, Empty, null");

			Assert.Throws<Exception> (() => new CGContextPDF ((NSUrl) null, null), "null NSUrl, null");

		}
	}
}
