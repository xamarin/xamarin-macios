//
// Unit tests for CGContextPDF
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

using System;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using NUnit.Framework;

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
				ctx.SetUrl (url, CGRect.Empty);
				ctx.EndPage ();
			}
		}

		[Test]
		public void Context_Url_Rect ()
		{
			var rect = new CGRect (10, 10, 100, 100);
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
			using (var ctx = new CGContextPDF (url, new CGRect (0, 0, 1000, 1000), PDFInfoTest.GetInfo ())) {
				ctx.AddDestination ("monkey", CGPoint.Empty);
				ctx.Close ();
			}
		}

		[Test]
		public void Constructors ()
		{
			if (TestRuntime.CheckXcodeVersion (9, 3))
				Assert.Ignore ("Crash (at least on simulator) with iOS 11.3 beta 1");

			Assert.Throws<Exception> (() => new CGContextPDF ((CGDataConsumer) null), "null CGDataConsumer");

			Assert.Throws<Exception> (() => new CGContextPDF ((CGDataConsumer) null, CGRect.Empty), "null CGDataConsumer, Empty");

			Assert.Throws<Exception> (() => new CGContextPDF ((CGDataConsumer) null, CGRect.Empty, null), "null CGDataConsumer, Empty, null");

			Assert.Throws<Exception> (() => new CGContextPDF ((CGDataConsumer) null, null), "null CGDataConsumer, null");

			Assert.Throws<Exception> (() => new CGContextPDF ((NSUrl) null), "null NSUrl");

			Assert.Throws<Exception> (() => new CGContextPDF ((NSUrl) null, CGRect.Empty), "null NSUrl, Empty");

			Assert.Throws<Exception> (() => new CGContextPDF ((NSUrl) null, CGRect.Empty, null), "null NSUrl, Empty, null");

			Assert.Throws<Exception> (() => new CGContextPDF ((NSUrl) null, null), "null NSUrl, null");

		}

		[Test]
		public void Context_Tag ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);
			using (var d = new NSDictionary ())
			using (var url = new NSUrl (filename))
			using (var ctx = new CGContextPDF (url)) {
				ctx.BeginPage (PDFInfoTest.GetInfo ());
				ctx.BeginTag (CGPdfTagType.Header, (NSDictionary) null);
				ctx.EndTag ();
				ctx.BeginTag (CGPdfTagType.Caption, d);
				ctx.SetUrl (url, CGRect.Empty);
				ctx.EndTag ();
				ctx.EndPage ();
			}
		}

		[Test]
		public void Context_Tag_Strong ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);
			using (var url = new NSUrl (filename))
			using (var ctx = new CGContextPDF (url)) {
				var tp = new CGPdfTagProperties () {
					ActualText = "ActualText",
					AlternativeText = "AlternativeText",
					TitleText = "TitleText",
					LanguageText = "LanguageText",
				};
				ctx.BeginPage (PDFInfoTest.GetInfo ());
				ctx.BeginTag (CGPdfTagType.Header, tp);
				ctx.EndTag ();
				ctx.BeginTag (CGPdfTagType.Caption, (CGPdfTagProperties) null);
				ctx.SetUrl (url, CGRect.Empty);
				ctx.EndTag ();
				ctx.EndPage ();
			}
		}
	}
}
