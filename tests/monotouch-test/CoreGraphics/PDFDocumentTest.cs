//
// Unit tests for CGPDFDocument
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

using System;
#if XAMCORE_2_0
using Foundation;
using CoreGraphics;
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
	public class PDFDocumentTest {
		
		[Test]
		public void DataProvider ()
		{
			using (CGDataProvider dp = new CGDataProvider (NSBundle.MainBundle.PathForResource ("Tamarin", "pdf")))
			using (CGPDFDocument doc = new CGPDFDocument (dp)) {
				CheckTamarin (doc);
			}
		}

		[Test]
		public void FromFile ()
		{
			using (CGPDFDocument doc = CGPDFDocument.FromFile (NSBundle.MainBundle.PathForResource ("Tamarin", "pdf"))) {
				CheckTamarin (doc);
			}
		}

		[Test]
		public void FromUrl ()
		{
			string url = NSBundle.MainBundle.GetUrlForResource ("Tamarin", "pdf").ToString ();
			using (CGPDFDocument doc = CGPDFDocument.FromUrl (url)) {
				CheckTamarin (doc);
			}
		}

		void CheckTamarin (CGPDFDocument pdf)
		{
			Assert.True (pdf.AllowsCopying, "AllowsCopying");
			Assert.True (pdf.AllowsPrinting, "AllowsPrinting");
			Assert.False (pdf.IsEncrypted, "IsEncrypted");
			Assert.True (pdf.IsUnlocked, "IsUnlocked");
			Assert.That (pdf.Pages, Is.EqualTo ((nint) 3), "Pages");

			Assert.That (pdf.GetInfo ().Count, Is.EqualTo (7), "GetInfo");

			if (TestRuntime.CheckXcodeVersion (9, 0)) {
				// Merely check that the P/Invoke goes through.
				var perms = pdf.GetAccessPermissions ();

				// Get and set outline
				var outline = pdf.GetOutline ();
				pdf.SetOutline (outline);
			}

		}
	}
}
