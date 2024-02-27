//
// Unit tests for CGPDFDocument
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Text;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using NUnit.Framework;

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

			if (TestRuntime.CheckXcodeVersion (10, 0))
				DumpPdf (pdf);
		}

		HashSet<IntPtr> processed = new HashSet<IntPtr> ();
		void Dump (StringBuilder sb, int indentation, object obj)
		{
			var ident = new string (' ', indentation + 4);
			if (obj is CGPDFArray array) {
				//Console.WriteLine ($"{ident}Array with {array.Count} elements (0x{array.Handle.ToString ("x")})");
				if (processed.Contains (array.Handle)) {
					sb.AppendLine ($"{ident}Already procesed CGPDFArray with handle 0x{array.Handle.ToString ("x")}");
					return;
				}
				processed.Add (array.Handle);

				sb.AppendLine ("Iterate using callback");
				array.Apply ((idx, value, info) => {
					sb.AppendLine ($"{ident}    #{idx + 1}:");
					Dump (sb, indentation + 1, value);
					return true;
				});
				sb.AppendLine ("Iterate manually");
				for (var i = 0; i < array.Count; i++) {
					if (array.GetInt (i, out var val1))
						sb.AppendLine ($"{ident}    #{i + 1}: {val1} (Int)");
					else if (array.GetName (i, out var val2))
						sb.AppendLine ($"{ident}    #{i + 1}: {val2} (Name)");
					else if (array.GetArray (i, out var val3)) {
						sb.AppendLine ($"{ident}    #{i + 1}: {val3} (Array)");
						Dump (sb, indentation + 1, val3);
					} else if (array.GetFloat (i, out var val4)) {
						sb.AppendLine ($"{ident}    #{i + 1}: {val4} (Float)");
					} else if (array.GetStream (i, out var val5))
						sb.AppendLine ($"{ident}    #{i + 1}: {val5} (Stream)");
					else if (array.GetString (i, out var val6))
						sb.AppendLine ($"{ident}    #{i + 1}: {val6} (String)");
					else if (array.GetBoolean (i, out var val7))
						sb.AppendLine ($"{ident}    #{i + 1}: {val7} (Boolean)");
					else if (array.GetDictionary (i, out var val8)) {
						sb.AppendLine ($"{ident}    #{i + 1}: {val8} (Dictionary)");
						Dump (sb, indentation + 1, val8);
					} else {
						throw new NotImplementedException ();
					}
				}
			} else if (obj is CGPDFStream stream) {
				sb.AppendLine ($"{ident}Stream with length: {stream.GetData (out var fmt).Length}");
				sb.AppendLine ($"{ident}Stream format: {fmt}");
			} else if (obj is CGPDFDictionary dict) {
				Dump (sb, indentation, dict);
			} else if (obj is nint intobj) {
				sb.AppendLine ($"{ident}{intobj} (Integer)");
			} else if (obj is string str) {
				sb.AppendLine ($"{ident}{str} (String)");
			} else if (obj is nfloat real) {
				sb.AppendLine ($"{ident}{real} (Float)");
			} else if (obj is bool b) {
				sb.AppendLine ($"{ident}{b} (Boolean)");
			} else {
				throw new NotImplementedException (obj.GetType ().FullName);
			}
		}
		void Dump (StringBuilder sb, int indentation, CGPDFDictionary dict)
		{
			var ident = new string (' ', indentation + 4);
			if (processed.Contains (dict.Handle)) {
				sb.AppendLine ($"{ident}Already procesed CGPDFDictionary with handle 0x{dict.Handle.ToString ("x")}");
				return;
			}
			processed.Add (dict.Handle);
			indentation++;
			sb.AppendLine ($"{ident}Dictionary with {dict.Count} elements (0x{dict.Handle.ToString ("x")})");
			dict.Apply ((string key, object obj, object info) => {
				sb.AppendLine ($"{ident}{key} => Object={obj} (Type: {obj.GetType ()})");
				Dump (sb, indentation + 1, obj);
			});
			indentation--;
		}

		void DumpPdf (CGPDFDocument doc)
		{
			var sb = new StringBuilder ();
			sb.AppendLine ($"{doc.Pages} pages found");
			for (var i = 0; i < doc.Pages; i++) {
				var page = doc.GetPage (i + 1);
				sb.AppendLine ($"Page #{i + 1}");
				var dict = page.Dictionary;
				Dump (sb, 1, dict);
			}
			//Console.WriteLine (sb);
		}
	}
}
