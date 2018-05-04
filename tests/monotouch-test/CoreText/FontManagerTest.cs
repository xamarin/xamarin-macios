//
// Unit tests for CTFontManager
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using System;
using System.IO;
#if XAMCORE_2_0
using Foundation;
using CoreText;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
#else
using MonoTouch.CoreText;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;
using System.Linq;

#if XAMCORE_2_0
using RectangleF=CoreGraphics.CGRect;
using SizeF=CoreGraphics.CGSize;
using PointF=CoreGraphics.CGPoint;
#else
using nfloat=global::System.Single;
using nint=global::System.Int32;
using nuint=global::System.UInt32;
#endif

namespace MonoTouchFixtures.CoreText {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class FontManagerTest {

		[Test]
		public void RegisterTTF ()
		{
			var ttf = Path.GetFullPath ("Pacifico.ttf");
			if (!File.Exists (ttf))
				Assert.Ignore ("Could not find the font file {0}", ttf);

			var url = NSUrl.FromFilename (ttf);
			var err = CTFontManager.RegisterFontsForUrl (url, CTFontManagerScope.Process);
			Assert.IsNull (err, "err 1");
			err = CTFontManager.UnregisterFontsForUrl (url, CTFontManagerScope.Process);
			Assert.IsNull (err, "err 2");

			url = NSUrl.FromFilename (Path.GetFullPath ("NonExistent.ttf"));
			err = CTFontManager.RegisterFontsForUrl (url, CTFontManagerScope.Process);
			Assert.IsNotNull (err, "err 3");
			err = CTFontManager.UnregisterFontsForUrl (url, CTFontManagerScope.Process);
			Assert.IsNotNull (err, "err 4");
		}

		[Test]
		public void RegisterTTFs ()
		{
			var ttf = Path.GetFullPath ("Pacifico.ttf");
			if (!File.Exists (ttf))
				Assert.Ignore ("Could not find the font file {0}", ttf);

			var url = NSUrl.FromFilename (ttf);
			var err = CTFontManager.RegisterFontsForUrl (new [] { url }, CTFontManagerScope.Process);
			Assert.IsNull (err, "err 1");
			err = CTFontManager.UnregisterFontsForUrl (new [] { url }, CTFontManagerScope.Process);
			Assert.IsNull (err, "err 2");

			url = NSUrl.FromFilename (Path.GetFullPath ("NonExistent.ttf"));
			err = CTFontManager.RegisterFontsForUrl (new [] { url }, CTFontManagerScope.Process);
			Assert.IsNotNull (err, "err 3");
			Assert.AreEqual (1, err.Length, "err 3 l");
			Assert.IsNotNull (err [0], "err 3[0]");
			err = CTFontManager.UnregisterFontsForUrl (new [] { url }, CTFontManagerScope.Process);
			Assert.IsNotNull (err, "err 4");
			Assert.AreEqual (1, err.Length, "err 4 l");
			Assert.IsNotNull (err [0], "err 4[0]");
		}

		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void GetFontsNullUrl ()
		{
			if (!TestRuntime.CheckXcodeVersion (5, 0))
				Assert.Ignore ("Requires iOS 7.0+ or macOS 10.9+");
			var fonts = CTFontManager.GetFonts (null);
		}

		[Test]
		public void GetFontsPresent ()
		{
			if (!TestRuntime.CheckXcodeVersion (5, 0))
				Assert.Ignore ("Requires iOS 7.0+ or macOS 10.9+");
			var ttf = Path.GetFullPath ("Pacifico.ttf");
			if (!File.Exists (ttf))
				Assert.Ignore ("Could not find the font file {0}", ttf);

			var url = NSUrl.FromFilename (ttf);
			var err = CTFontManager.RegisterFontsForUrl (url, CTFontManagerScope.Process);
			Assert.IsNull (err, "Register error");

			// method under test
			var fonts = CTFontManager.GetFonts (url);
			Assert.AreEqual (1, fonts.Length);
			Assert.AreEqual ("Pacifico", fonts[0].GetAttributes().Name?.ToString ());

			err = CTFontManager.UnregisterFontsForUrl (url, CTFontManagerScope.Process);
			Assert.IsNull (err, "Unregister error");
		}

		[Test]
		public void GetFontsMissing ()
		{
			if (!TestRuntime.CheckXcodeVersion (5, 0))
				Assert.Ignore ("Requires iOS 7.0+ or macOS 10.9+");
			var ttf = Path.GetFullPath ("NonExistent.ttf");
			if (!File.Exists (ttf))
				Assert.Ignore ("Could not find the font file {0}", ttf);

			var url = NSUrl.FromFilename (ttf);

			// method under test
			var fonts = CTFontManager.GetFonts (url);
			Assert.AreEqual (0, fonts.Length);
		}

	}
}
