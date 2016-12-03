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
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using CoreGraphics;
#else
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif

using NUnit.Framework;

#if XAMCORE_2_0
using RectangleF = CoreGraphics.CGRect;
using SizeF = CoreGraphics.CGSize;
using PointF = CoreGraphics.CGPoint;
#else
using nfloat=global::System.Single;
using nint=global::System.Int32;
using nuint=global::System.UInt32;
#endif

namespace MonoTouchFixtures.CoreGraphics
{

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PDFInfoTest
	{

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
			// Bug #8879
			var info = GetInfo ();
			UIGraphics.BeginPDFContext("file", RectangleF.Empty, info); 
		}
#endif
	}
}
