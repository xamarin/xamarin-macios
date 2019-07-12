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
		public void RegisterFonts_Null ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);
			Assert.Throws<ArgumentNullException> (() => CTFontManager.RegisterFonts (null, CTFontManagerScope.Process, true, null), "null array");
			Assert.Throws<ArgumentException> (() => CTFontManager.RegisterFonts (new NSUrl [] { null }, CTFontManagerScope.Process, true, null), "null element");
		}

		[Test]
		public void UnregisterFonts_Null ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);
			Assert.Throws<ArgumentNullException> (() => CTFontManager.UnregisterFonts (null, CTFontManagerScope.Process, null), "null array");
			Assert.Throws<ArgumentException> (() => CTFontManager.UnregisterFonts (new NSUrl [] { null }, CTFontManagerScope.Process, null), "null element");
		}

		[Test]
		public void RegisterFonts_NoCallback ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);
			var ttf = Path.GetFullPath ("Pacifico.ttf");
			if (!File.Exists (ttf))
				Assert.Ignore ("Could not find the font file {0}", ttf);

			using (var url = NSUrl.FromFilename (ttf)) {
				var array = new [] { url };
				CTFontManager.RegisterFonts (array, CTFontManagerScope.Process, true, null);
				CTFontManager.UnregisterFonts (array, CTFontManagerScope.Process, null);
			}

			using (var url = NSUrl.FromFilename (Path.GetFullPath ("NonExistent.ttf"))) {
				var array = new [] { url };
				CTFontManager.RegisterFonts (array, CTFontManagerScope.Process, true, null);
				CTFontManager.UnregisterFonts (array, CTFontManagerScope.Process, null);
			}
		}

		static bool SuccessDone (NSError [] errors, bool done)
		{
			Assert.That (errors.Length, Is.EqualTo (0), "errors");
			Assert.True (done, "done");
			return true;
		}

		static bool FailureDone (NSError [] errors, bool done)
		{
			Assert.That (errors.Length, Is.EqualTo (1), "errors");
			Assert.True (errors [0].UserInfo.TryGetValue (CTFontManagerErrorKeys.FontUrlsKey, out var urls), "FontUrlsKey");
			Assert.True ((urls as NSArray).GetItem<NSUrl> (0).AbsoluteString.EndsWith ("NonExistent.ttf", StringComparison.Ordinal), "NonExistent"); 
			Assert.True (done, "done");
			return true;
		}

		[Test]
		public void RegisterFonts_WithCallback ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);
			var ttf = Path.GetFullPath ("Pacifico.ttf");
			if (!File.Exists (ttf))
				Assert.Ignore ("Could not find the font file {0}", ttf);

			using (var url = NSUrl.FromFilename (ttf)) {
				var array = new [] { url };
				CTFontManager.RegisterFonts (array, CTFontManagerScope.Process, true, SuccessDone);
				CTFontManager.UnregisterFonts (array, CTFontManagerScope.Process, SuccessDone);
			}

			using (var url = NSUrl.FromFilename (Path.GetFullPath ("NonExistent.ttf"))) {
				var array = new [] { url };
				CTFontManager.RegisterFonts (array, CTFontManagerScope.Process, true, FailureDone);
				CTFontManager.UnregisterFonts (array, CTFontManagerScope.Process, FailureDone);
			}
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
		public void RegisterFontDescriptors_Null ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);
			Assert.Throws<ArgumentNullException> (() => CTFontManager.RegisterFontDescriptors (null, CTFontManagerScope.Process, true, null), "null array");
			Assert.Throws<ArgumentException> (() => CTFontManager.RegisterFontDescriptors (new CTFontDescriptor [] { null }, CTFontManagerScope.Process, true, null), "null element");
		}

		[Test]
		public void UnregisterFontDescriptors_Null ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);
			Assert.Throws<ArgumentNullException> (() => CTFontManager.UnregisterFontDescriptors (null, CTFontManagerScope.Process, null), "null array");
			Assert.Throws<ArgumentException> (() => CTFontManager.UnregisterFontDescriptors (new CTFontDescriptor [] { null }, CTFontManagerScope.Process, null), "null element");
		}

		[Test]
		public void RegisterFontDescriptors_NoCallback ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);
			var ttf = Path.GetFullPath ("Pacifico.ttf");
			if (!File.Exists (ttf))
				Assert.Ignore ("Could not find the font file {0}", ttf);

			CTFontDescriptorAttributes fda = new CTFontDescriptorAttributes () {
				FamilyName = "Courier",
				StyleName = "Bold",
				Size = 16.0f
			};
			using (CTFontDescriptor fd = new CTFontDescriptor (fda)) {
				var array = new [] { fd };
				CTFontManager.RegisterFontDescriptors (array, CTFontManagerScope.Process, true, null);
				CTFontManager.UnregisterFontDescriptors (array, CTFontManagerScope.Process, null);
			}
		}

		[Test]
		public void RegisterFontDescriptors_WithCallback ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);
			var ttf = Path.GetFullPath ("Pacifico.ttf");
			if (!File.Exists (ttf))
				Assert.Ignore ("Could not find the font file {0}", ttf);

			CTFontDescriptorAttributes fda = new CTFontDescriptorAttributes () {
				FamilyName = "Courier",
				StyleName = "Bold",
				Size = 16.0f
			};
			using (CTFontDescriptor fd = new CTFontDescriptor (fda)) {
				var array = new [] { fd };
				CTFontManager.RegisterFontDescriptors (array, CTFontManagerScope.Process, true, SuccessDone);
				CTFontManager.UnregisterFontDescriptors (array, CTFontManagerScope.Process, SuccessDone);
			}
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

			using (var url = NSUrl.FromFilename (ttf)) {
				var fonts = CTFontManager.GetFonts (url);
				Assert.AreEqual (0, fonts.Length);
			}
		}

		[Test]
		public void CreateFontDescriptor ()
		{
			Assert.Throws<ArgumentNullException> (() => CTFontManager.CreateFontDescriptor (null), "null");

			using (var data = NSData.FromFile (Path.GetFullPath ("Pacifico.ttf")))
				Assert.NotNull (CTFontManager.CreateFontDescriptor (data), "font");
			using (var data = NSData.FromFile (Path.GetFullPath ("Tamarin.pdf")))
				Assert.Null (CTFontManager.CreateFontDescriptor (data), "not a font");
		}

		[Test]
		public void CreateFontDescriptors ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);
			Assert.Throws<ArgumentNullException> (() => CTFontManager.CreateFontDescriptors (null), "null");

			using (var data = NSData.FromFile (Path.GetFullPath ("Pacifico.ttf"))) {
				var fds = CTFontManager.CreateFontDescriptors (data);
				Assert.That (fds.Length, Is.EqualTo (1), "font");
			}
			using (var data = NSData.FromFile (Path.GetFullPath ("Tamarin.pdf"))) {
				var fds = CTFontManager.CreateFontDescriptors (data);
				Assert.That (fds.Length, Is.EqualTo (0), "not font(s)");
			}
		}

#if __IOS__
		[Test]
		public void RequestFonts ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);
			CTFontDescriptorAttributes fda = new CTFontDescriptorAttributes () {
				FamilyName = "Courier",
				StyleName = "Bold",
				Size = 16.0f
			};
			using (CTFontDescriptor fd = new CTFontDescriptor (fda)) {
				Assert.Throws<ArgumentNullException> (() => CTFontManager.RequestFonts (new [] { fd }, null), "null");

				var callback = false;
				CTFontManager.RequestFonts (new [] { fd }, (unresolved) => {
					Assert.That (unresolved.Length, Is.EqualTo (0), "all resolved");
					callback = true;
				});
				Assert.True (callback, "callback");
			}
		}
#endif
	}
}
