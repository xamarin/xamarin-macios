//
// Unit tests for CGPDFScanner
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

using System;
using System.Runtime.InteropServices;

using Foundation;
using CoreGraphics;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.CoreGraphics {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PDFScannerTest {

		int bt_count;
		int do_checks;

#if NET
		[UnmanagedCallersOnly]
#else
		[MonoPInvokeCallback (typeof (Action<IntPtr, IntPtr>))]
#endif
		static void BT (IntPtr reserved, IntPtr info)
		{
			// sadly the parameters are always identical and we can't know the operator name
			// IOW we can NOT dispatch (internally) to a nice C# looking method

			// We do NOT want people to use the "obvious" new CGPDFScanner(reserved) because that will
			// call retain (and this gets called a lot of time)
			// So _a_ solution is to provide an helper method to give them what they want
			var scanner = CGPDFOperatorTable.GetScannerFromInfo (info);
			(scanner.UserInfo as PDFScannerTest).bt_count++;
		}

#if NET
		[UnmanagedCallersOnly]
#else
		[MonoPInvokeCallback (typeof (Action<IntPtr, IntPtr>))]
#endif
		static void Do (IntPtr reserved, IntPtr info)
		{
			// sadly the parameters are always identical and we can't know the operator name
			// IOW we can NOT dispatch (internally) to a nice C# looking method

			// We do NOT want people to use the "obvious" new CGPDFScanner(reserved) because that will
			// call retain (and this gets called a lot of time)
			// So _a_ solution is to provide an helper method to give them what they want
			var scanner = CGPDFOperatorTable.GetScannerFromInfo (info);
			string name;
			if (!scanner.TryPopName (out name))
				return;

			var test = (scanner.UserInfo as PDFScannerTest);
			test.do_checks--;

			var cs = scanner.GetContentStream ();
			if (cs is null)
				return;
			test.do_checks--;

			var ro = cs.GetResource ("XObject", name);
			if (ro.Type != CGPDFObjectType.Stream)
				return;
			test.do_checks--;

			CGPDFStream s;
			if (!ro.TryGetValue (out s))
				return;
			test.do_checks--;

			var dict = s.Dictionary;
			if (dict is null)
				return;
			test.do_checks--;

			string dictname;
			if (!dict.GetName ("Subtype", out dictname))
				return;
			test.do_checks--;

			if (dictname != "Image")
				return;
			test.do_checks--;
		}

		[Test]
		public void Tamarin ()
		{
			using (var table = new CGPDFOperatorTable ()) {
				// note: the native API is horrible as we can't share the same callback and dispatch it to the right
				// managed method. That force every operator to have one ugly, MonoPInvokeCallback-decorated, per
				// operator
#if WE_HAD_A_NICE_API
				table.SetCallback ("BT", delegate (CGPDFScanner scanner) {
					bt_count++;
				});

				table.SetCallback ("Do", delegate (CGPDFScanner scanner) {
				// ... drill down to the image
				});
#elif NET
				unsafe {
					// BT == new paragraph
					table.SetCallback ("BT", &BT);
					// Do == the image is inside it
					table.SetCallback ("Do", &Do);
				}
#else
				// BT == new paragraph
				table.SetCallback ("BT", BT);
				// Do == the image is inside it
				table.SetCallback ("Do", Do);
#endif
				using (var doc = CGPDFDocument.FromFile (NSBundle.MainBundle.PathForResource ("Tamarin", "pdf")))
				using (var page = doc.GetPage (1))
				using (var cs = new CGPDFContentStream (page))
				using (var scanner = new CGPDFScanner (cs, table, this)) {
					Assert.That (scanner.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");

					Assert.That (scanner.GetContentStream ().Handle, Is.EqualTo (cs.Handle), "GetContentStream");

					bt_count = 0;
					do_checks = 7;
					Assert.True (scanner.Scan (), "Scan");
					Assert.That (bt_count, Is.EqualTo (45), "new paragraph");
					Assert.That (do_checks, Is.EqualTo (0), "found the image");
					if (TestRuntime.CheckXcodeVersion (14, 0)) {
						scanner.Stop ();
					}
				}
			}
		}
	}
}
