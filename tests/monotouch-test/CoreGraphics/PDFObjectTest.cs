//
// Unit tests for CGPDFObject
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
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

#if !XAMCORE_2_0
using nfloat=global::System.Single;
using nint=global::System.Int32;
#endif

namespace MonoTouchFixtures.CoreGraphics {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PDFObjectTest {

		[Test]
		public void Zero ()
		{
			// CGPDFObject is not a real "object" that you can retain/release - it's more a querying interface
			var po = new CGPDFObject (IntPtr.Zero);
			Assert.IsTrue (po.IsNull, "IsNull");
			Assert.That (po.Type == CGPDFObjectType.Null);

			CGPDFStream stream;
			Assert.False (po.TryGetValue (out stream), "CGPDFStream");
			CGPDFDictionary dict;
			Assert.False (po.TryGetValue (out dict), "CGPDFDictionary");
			CGPDFArray array;
			Assert.False (po.TryGetValue (out array), "CGPDFArray");
			string s;
			Assert.False (po.TryGetValue (out s), "string");
			nfloat f;
			Assert.False (po.TryGetValue (out f), "nfloat");
			nint i;
			Assert.False (po.TryGetValue (out i), "nint");
			bool b;
			Assert.False (po.TryGetValue (out b), "bool");

			Assert.False (po.TryGetName (out s), "name");
		}
	}
}