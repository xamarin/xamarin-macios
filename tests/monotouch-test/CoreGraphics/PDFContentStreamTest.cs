//
// Unit tests for CGPDFContentStream
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

using System;
using Foundation;
using CoreGraphics;
using NUnit.Framework;

namespace MonoTouchFixtures.CoreGraphics {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PDFContentStreamTest {

		[Test]
		public void FromPage ()
		{
			using (var doc = CGPDFDocument.FromFile (NSBundle.MainBundle.PathForResource ("Tamarin", "pdf")))
			using (var page = doc.GetPage (1))
			using (var cs = new CGPDFContentStream (page)) {
				Assert.That (cs.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");

				var streams = cs.GetStreams ();
				Assert.That (streams.Length, Is.EqualTo (1), "GetStreams.Length");
				Assert.That (streams [0].Handle, Is.Not.EqualTo (cs.Handle), "GetStreams");

				Assert.Null (cs.GetResource ("XObject", ""), "GetResource");
			}
		}
	}
}
