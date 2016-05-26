//
// Unit tests for CGPDFArray
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
	public class PDFArrayTest {
		
		[Test]
		public void InvalidHandle ()
		{
			var array = new CGPDFArray (IntPtr.Zero);
			Assert.That (array.Count, Is.EqualTo ((nint) 0), "Count");
			
			CGPDFArray a;
			Assert.False (array.GetArray (0, out a), "GetArray");
			Assert.Null (a, "array");

			bool b;
			Assert.False (array.GetBoolean (0, out b), "GetBoolean");
			Assert.False (b, "bool");

			CGPDFDictionary d;
			Assert.False (array.GetDictionary (0, out d), "GetDictionary");
			Assert.Null (d, "dict");
			
			nfloat f;
			Assert.False (array.GetFloat (0, out f), "GetFloat");
			Assert.That (f, Is.EqualTo ((nfloat) 0f), "float");

			nint i;
			Assert.False (array.GetInt (0, out i), "GetInt");
			Assert.That (i, Is.EqualTo ((nint) 0), "int");
			
			string str;
			Assert.False (array.GetName (0, out str), "GetName");
			Assert.Null (str, "Name");
			
			CGPDFStream stream;
			Assert.False (array.GetStream (0, out stream), "GetStream");
			Assert.Null (stream, "Stream");
			
			Assert.False (array.GetString (0, out str), "GetString");
			Assert.Null (str, "string");
		}
	}
}
