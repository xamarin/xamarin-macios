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
using Foundation;
using CoreText;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using NUnit.Framework;
using System.Linq;

namespace MonoTouchFixtures.CoreText {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class FontManagerTest {

		static string pacifico_ttf_path;
		static string tamarin_pdf_path;
		static string non_existent_path;

		[SetUp]
		public void SetUp ()
		{
			pacifico_ttf_path = NSBundle.MainBundle.PathForResource ("Pacifico", "ttf");
			if (!File.Exists (pacifico_ttf_path))
				Assert.Fail ($"Could not find the font file {pacifico_ttf_path}");

			tamarin_pdf_path = NSBundle.MainBundle.PathForResource ("Tamarin", "pdf");
			if (!File.Exists (tamarin_pdf_path))
				Assert.Fail ($"Could not find the PDF file {tamarin_pdf_path}");

			non_existent_path = Path.GetFullPath ("NonExistent.ttf");
			if (File.Exists (non_existent_path))
				Assert.Fail ($"This file should not exists {non_existent_path}");
		}

		[Test]
		public void RegisterTTF ()
		{
			using (var url = NSUrl.FromFilename (pacifico_ttf_path)) {
				var err = CTFontManager.RegisterFontsForUrl (url, CTFontManagerScope.Process);
				Assert.IsNull (err, "err 1");
				err = CTFontManager.UnregisterFontsForUrl (url, CTFontManagerScope.Process);
				Assert.IsNull (err, "err 2");
			}

			using (var url = NSUrl.FromFilename (non_existent_path)) {
				var err = CTFontManager.RegisterFontsForUrl (url, CTFontManagerScope.Process);
				// xcode 11 beta 4 stopped reporting errors
				// Assert.IsNotNull (err, "err 3");
				err = CTFontManager.UnregisterFontsForUrl (url, CTFontManagerScope.Process);
#if MONOMAC || __MACCATALYST__
				if (TestRuntime.CheckXcodeVersion (12, 2))
					Assert.IsNotNull (err, "err 4");
				else
					Assert.IsNull (err, "err 4");
#else
				Assert.IsNotNull (err, "err 4");
#endif
			}
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

			using (var url = NSUrl.FromFilename (pacifico_ttf_path)) {
				var array = new [] { url };
				CTFontManager.RegisterFonts (array, CTFontManagerScope.Process, true, null);
				CTFontManager.UnregisterFonts (array, CTFontManagerScope.Process, null);
			}

			using (var url = NSUrl.FromFilename (non_existent_path)) {
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

			using (var url = NSUrl.FromFilename (pacifico_ttf_path)) {
				var array = new [] { url };
				CTFontManager.RegisterFonts (array, CTFontManagerScope.Process, true, SuccessDone);
				CTFontManager.UnregisterFonts (array, CTFontManagerScope.Process, SuccessDone);
			}

			// xcode 11 beta 4 stopped reporting errors
			// using (var url = NSUrl.FromFilename (non_existent_path)) {
			// 	var array = new [] { url };
			// 	CTFontManager.RegisterFonts (array, CTFontManagerScope.Process, true, FailureDone);
			// 	CTFontManager.UnregisterFonts (array, CTFontManagerScope.Process, FailureDone);
			// }
		}

		[Test]
		public void RegisterTTFs ()
		{
			using (var url = NSUrl.FromFilename (pacifico_ttf_path)) {
				var err = CTFontManager.RegisterFontsForUrl (new [] { url }, CTFontManagerScope.Process);
				Assert.IsNull (err, "err 1");
				err = CTFontManager.UnregisterFontsForUrl (new [] { url }, CTFontManagerScope.Process);
				Assert.IsNull (err, "err 2");
			}

			using (var url = NSUrl.FromFilename (non_existent_path)) {
				var err = CTFontManager.RegisterFontsForUrl (new [] { url }, CTFontManagerScope.Process);
				// xcode 11 beta 4 stopped reporting errors
				// Assert.IsNotNull (err, "err 3");
				// Assert.AreEqual (1, err.Length, "err 3 l");
				// Assert.IsNotNull (err [0], "err 3[0]");
				err = CTFontManager.UnregisterFontsForUrl (new [] { url }, CTFontManagerScope.Process);
#if MONOMAC || __MACCATALYST__
				if (TestRuntime.CheckXcodeVersion (12, 2)) {
					Assert.IsNotNull (err, "err 4");
					Assert.AreEqual (1, err.Length, "err 4 l");
					Assert.IsNotNull (err [0], "err 4[0]");
				} else
					Assert.IsNull (err, "err 4");
#else
				Assert.IsNotNull (err, "err 4");
				Assert.AreEqual (1, err.Length, "err 4 l");
				Assert.IsNotNull (err [0], "err 4[0]");
#endif
			}
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

#if !__WATCHOS__
		[Test]
		public void RegisterFontDescriptors_NoCallback ()
		{
			TestRuntime.AssertXcodeVersion (11, 1); // Introduced in iOS 13.0, but with a bug that makes it crash. Apple fixed it for iOS 13.1
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

#if __TVOS__
		[Ignore ("Fails on tvOS with undocumented error code 'The operation couldn’t be completed. (com.apple.CoreText.CTFontManagerErrorDomain error 500.'")]
#elif __IOS__
		[Ignore ("https://github.com/xamarin/xamarin-macios/issues/6690. This began failing for no aparent reason in iOS 13 Beta 5. Check back with GM.")]
#endif
		[Test]
		public void RegisterFontDescriptors_WithCallback ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);

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
#endif

		[Test]
		public void GetFontsNullUrl ()
		{
			TestRuntime.AssertXcodeVersion (5, 0);
			Assert.Throws<ArgumentNullException> (() => CTFontManager.GetFonts (null));
		}

		[Test]
		public void GetFontsPresent ()
		{
			TestRuntime.AssertXcodeVersion (5, 0);

			var url = NSUrl.FromFilename (pacifico_ttf_path);
			var err = CTFontManager.RegisterFontsForUrl (url, CTFontManagerScope.Process);
			Assert.IsNull (err, "Register error");

			// method under test
			var fonts = CTFontManager.GetFonts (url);
			Assert.AreEqual (1, fonts.Length);
			Assert.AreEqual ("Pacifico", fonts [0].GetAttributes ().Name?.ToString ());

			err = CTFontManager.UnregisterFontsForUrl (url, CTFontManagerScope.Process);
			Assert.IsNull (err, "Unregister error");
		}

		[Test]
		public void GetFontsMissing ()
		{
			TestRuntime.AssertXcodeVersion (5, 0);

			using (var url = NSUrl.FromFilename (non_existent_path)) {
				var fonts = CTFontManager.GetFonts (url);
				Assert.AreEqual (0, fonts.Length);
			}
		}

		[Test]
		public void CreateFontDescriptor ()
		{
			Assert.Throws<ArgumentNullException> (() => CTFontManager.CreateFontDescriptor (null), "null");

			using (var data = NSData.FromFile (pacifico_ttf_path))
				Assert.NotNull (CTFontManager.CreateFontDescriptor (data), "font");

			using (var data = NSData.FromFile (tamarin_pdf_path))
				Assert.Null (CTFontManager.CreateFontDescriptor (data), "not a font");
		}

		[Test]
		public void CreateFontDescriptors ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);
			Assert.Throws<ArgumentNullException> (() => CTFontManager.CreateFontDescriptors (null), "null");

			using (var data = NSData.FromFile (pacifico_ttf_path)) {
				var fds = CTFontManager.CreateFontDescriptors (data);
				Assert.That (fds.Length, Is.EqualTo (1), "font");
			}

			using (var data = NSData.FromFile (tamarin_pdf_path)) {
				var fds = CTFontManager.CreateFontDescriptors (data);
				Assert.That (fds.Length, Is.EqualTo (0), "not font(s)");
			}
		}

#if __IOS__ && !__MACCATALYST__
		// Running this on Mac Catalyst prints "CTFontManagerRequestFonts not implemented for plaform or scope" to the terminal
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
