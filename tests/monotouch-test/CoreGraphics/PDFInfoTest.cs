//
// Unit tests for CGPDFInfo
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2012-2014 Xamarin Inc. All rights reserved.
//

using System;
using Foundation;
using ObjCRuntime;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using CoreGraphics;

using NUnit.Framework;

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
				CreatePdfA2u = true,
				CreateLinearizedPdf = true,
			};
		}

#if !MONOMAC // Not on mac
		[Test]
		public void ToDictionary ()
		{
			if (TestRuntime.CheckXcodeVersion (9, 3))
				Assert.Ignore ("Crash (at least on devices) with iOS 11.3 beta 1");
			// Bug #8879
			var info = GetInfo ();
			UIGraphics.BeginPDFContext ("file", CGRect.Empty, info);
		}
#endif

		[Test]
		public void ToDictionaryWithPermissions ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);

			var filename = Environment.GetFolderPath (Environment.SpecialFolder.CommonDocuments) + "/t.pdf";

			var info = GetInfo ();
			info.AccessPermissions = CGPDFAccessPermissions.AllowsContentCopying;

			using (var url = new NSUrl (filename)) {
				using (var ctx = new CGContextPDF (url, new CGRect (0, 0, 1000, 1000), info)) {
					Assert.IsNotNull (ctx, "1");
				}
				using (var consumer = new CGDataConsumer (url)) {
					using (var ctx = new CGContextPDF (consumer, new CGRect (0, 0, 1000, 1000), info)) {
						Assert.IsNotNull (ctx, "2");
					}
				}
			}
		}
	}
}
