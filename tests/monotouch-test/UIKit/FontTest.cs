// Copyright 2011 Xamarin Inc. All rights reserved

#if !MONOMAC
using System;
using System.Drawing;
#if XAMCORE_2_0
using Foundation;
using UIKit;
#else
using MonoTouch.Foundation;
using MonoTouch.UIKit;
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


namespace MonoTouchFixtures.UIKit {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class FontTest {
		
		[Test]
		public void WithSize ()
		{
			var f1 = UIFont.SystemFontOfSize (10).WithSize (20);
			Assert.AreEqual (f1.PointSize, (nfloat) 20, "#size");
		}

		[Test]
		public void TestDescriptors ()
		{
			TestRuntime.AssertXcodeVersion (5, 0);

			var font = UIFont.BoldSystemFontOfSize (80);
			var descriptor = font.FontDescriptor;

			// Ensure that Apple does not break things behind us, they documented
			// the size property as being a string, but it is a number (makes sense)
			// but make sure we dont regress if they fix it.

			var size = descriptor.FontAttributes.Size;
			Assert.AreEqual (true, size.HasValue);
			Assert.AreEqual (80.0f, size.Value);
		}

		// ref: https://trello.com/c/wKZyugio/437-many-managed-peers-on-a-single-native-instance

		void SemiFactory_25511 (UIFont f1, UIFont f2, string api)
		{
			using (f1) {
				// the same instance will be returned (from an iOS cache)
				Assert.That (f1.Handle, Is.EqualTo (f2.Handle), "{0} Handle", api);
				// using means f1 will be disposed and it's handle will be zero'ed
				// but f2 is the same (managed) instance and _normally_ would become unusable
				// to fix this we now return a different instance - but we must still match the existing behavior
				Assert.True (f1 == f2, "{0} ==", api);
				Assert.True (f1.Equals ((object) f2), "{0} Equals(object)", api);
				// IEquatable<NSObject> is only in unified - otherwise it would be the same call as above
				Assert.True (f1.Equals (f2), "{0} Equals", api);
			}
			Assert.That (f1.Handle, Is.EqualTo (IntPtr.Zero), "{0} 1", api);
			// without our "fix" that would be the same managed instance (as f1) and the handle would be nil
			Assert.That (f2.Handle, Is.Not.EqualTo (IntPtr.Zero), "{0} 2", api);
		}

		[Test]
		public void Methods ()
		{
			var f1 = UIFont.FromName ("Helvetica", 20.0f);
			// the same instance will be returned (from an iOS cache)
			var f2 = UIFont.FromName ("Helvetica", 20.0f);
			// first instance will be disposed and the 2nd one needs to stay valid
			SemiFactory_25511 (f1, f2, "FromName");

			f1 = UIFont.SystemFontOfSize (12);
			f2 = UIFont.SystemFontOfSize (12);
			SemiFactory_25511 (f1, f2, "SystemFontOfSize");

			f1 = UIFont.BoldSystemFontOfSize (12);
			f2 = UIFont.BoldSystemFontOfSize (12);
			SemiFactory_25511 (f1, f2, "BoldSystemFontOfSize");

			f1 = UIFont.ItalicSystemFontOfSize (12);
			f2 = UIFont.ItalicSystemFontOfSize (12);
			SemiFactory_25511 (f1, f2, "ItalicSystemFontOfSize");

			f1 = UIFont.SystemFontOfSize (12);
			f2 = UIFont.SystemFontOfSize (12);
			SemiFactory_25511 (f1, f2, "SystemFontOfSize");

			// instance
			f1 = f2.WithSize (12);
			f2 = f2.WithSize (12);
			SemiFactory_25511 (f1, f2, "WithSize");

			if (!TestRuntime.CheckXcodeVersion (5, 0))
				return;

			using (var name = new NSString ("UICTFontTextStyleBody")) {
				f1 = UIFont.GetPreferredFontForTextStyle (name);
				f2 = UIFont.GetPreferredFontForTextStyle (name);
				SemiFactory_25511 (f1, f2, "GetPreferredFontForTextStyle");
			}

			var d = f2.FontDescriptor;
			f1 = UIFont.FromDescriptor (d, 12);
			f2 = UIFont.FromDescriptor (d, 12);
			SemiFactory_25511 (f1, f2, "FromDescriptor");
		}

		[Test]
		public void Properties ()
		{
			TestRuntime.AssertXcodeVersion (5, 0);

			var f1 = UIFont.PreferredBody;
			// the same instance will be returned (from an iOS cache)
			var f2 = UIFont.PreferredBody;
			// first instance will be disposed and the 2nd one needs to stay valid
			SemiFactory_25511 (f1, f2, "PreferredBody");

			f1 = UIFont.PreferredCaption1;
			f2 = UIFont.PreferredCaption1;
			SemiFactory_25511 (f1, f2, "PreferredCaption1");

			f1 = UIFont.PreferredCaption2;
			f2 = UIFont.PreferredCaption2;
			SemiFactory_25511 (f1, f2, "PreferredCaption2");

			f1 = UIFont.PreferredFootnote;
			f2 = UIFont.PreferredFootnote;
			SemiFactory_25511 (f1, f2, "PreferredFootnote");

			f1 = UIFont.PreferredHeadline;
			f2 = UIFont.PreferredHeadline;
			SemiFactory_25511 (f1, f2, "PreferredHeadline");

			f1 = UIFont.PreferredSubheadline;
			f2 = UIFont.PreferredSubheadline;
			SemiFactory_25511 (f1, f2, "PreferredSubheadline");
		}

		[Test]
		public void NullFonts ()
		{
			var invalidFontName = new NSString ("Invalid Font Name");
			if (TestRuntime.CheckXcodeVersion (5, 0)) {
				Assert.IsNotNull (UIFont.GetPreferredFontForTextStyle (invalidFontName), "GetPreferredFontForTextStyle");
				Assert.IsNotNull (UIFont.FromDescriptor (new UIFontDescriptor (), -2), "FromDescriptor (,)");
			}

			Assert.IsNull (UIFont.FromName (invalidFontName, 1), "FromName");

			Assert.IsNotNull (UIFont.SystemFontOfSize (-3), "SystemFontOfSize()");

			if (TestRuntime.CheckXcodeVersion (6, 2)) {
				Assert.IsNotNull (UIFont.SystemFontOfSize (0, UIFontWeight.Regular), "SystemFontOfSize (nfloat, UIFontWeight)");
				Assert.IsNotNull (UIFont.SystemFontOfSize (0, (nfloat) 0), "SystemFontOfSize (nfloat, nfloat)");
			}

			Assert.IsNotNull (UIFont.BoldSystemFontOfSize (-4), "BoldSystemFontOfSize");
			Assert.IsNotNull (UIFont.ItalicSystemFontOfSize (-5), "ItalicSystemFontOfSize");

			using (var font = UIFont.SystemFontOfSize (12)) {
				Assert.IsNotNull (font.WithSize (-6), "WithSize");
			}
		}
	}
}
#endif