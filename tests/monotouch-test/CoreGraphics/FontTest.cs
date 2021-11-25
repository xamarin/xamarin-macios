//
// Unit tests for CGFont
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using System;
using System.Runtime.InteropServices;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.CoreGraphics {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class FontTest {

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGFontRef */ IntPtr CGFontCreateWithDataProvider (/* CGDataProviderRef __nullable */ IntPtr provider);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGFontRef */ IntPtr CGFontCreateWithFontName (/* CFStringRef __nullable */ IntPtr name);

		[Test]
		public void Nullable ()
		{
			Assert.That (CGFontCreateWithDataProvider (IntPtr.Zero), Is.EqualTo (IntPtr.Zero), "CGFontCreateWithDataProvider");
			Assert.That (CGFontCreateWithFontName (IntPtr.Zero), Is.EqualTo (IntPtr.Zero), "CGFontCreateWithFontName");
		}

		[Test]
		public void CreateFromProvider ()
		{
			Assert.Null (CGFont.CreateFromProvider (null), "CreateFromProvider");
		}

		[Test]
		public void CreateWithFontName ()
		{
			Assert.Null (CGFont.CreateWithFontName (null), "CreateWithFontName");
		}

		[Test]
		public void GetGlyphWithGlyphName ()
		{
			using (var f = CGFont.CreateWithFontName ("Courier New")) {
				Assert.That (f.Handle, Is.Not.EqualTo (IntPtr.Zero), "CreateWithFontName");
				// note: the API is marked to accept a null CFStringRef but it currently (iOS9 beta 4) crash when provided one
				Assert.Throws<ArgumentNullException> (() => f.GetGlyphWithGlyphName (null), "GetGlyphWithGlyphName");
			}
		}
	}
}
