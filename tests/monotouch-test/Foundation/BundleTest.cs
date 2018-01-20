//
// Unit tests for NSBundle
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012-2013 Xamarin Inc. All rights reserved.
//

using System;
using System.Net;
#if XAMCORE_2_0
using Foundation;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using ObjCRuntime;
#else
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.Foundation {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class BundleTest {
		
		NSBundle main = NSBundle.MainBundle;
		
		[Test]
		public void LocalizedString2 ()
		{
			string s = main.LocalizedString (null, "comment");
			Assert.That (s, Is.Empty, "key");

			s = main.LocalizedString ("key", null);
			Assert.That (s, Is.EqualTo ("key"), "comment");

			s = main.LocalizedString (null, null);
			Assert.That (s, Is.Empty, "all-null");
		}

		[Test]
		public void LocalizedString3 ()
		{
			string s = main.LocalizedString (null, "value", "table");
			Assert.That (s, Is.EqualTo ("value"), "key");

			s = NSBundle.MainBundle.LocalizedString ("key", null, "table");
			Assert.That (s, Is.EqualTo ("key"), "value");

			s = NSBundle.MainBundle.LocalizedString (null, "value", null);
			Assert.That (s, Is.EqualTo ("value"), "comment");
			
			s = main.LocalizedString (null, null, null);
			Assert.That (s, Is.Empty, "all-null");
		}

		[Test]
		public void LocalizedString4 ()
		{
			string s = main.LocalizedString (null, "value", "table", "comment");
			Assert.That (s, Is.EqualTo ("value"), "key");
			
			s = NSBundle.MainBundle.LocalizedString ("key", null, "table", "comment");
			Assert.That (s, Is.EqualTo ("key"), "value");
			
			s = NSBundle.MainBundle.LocalizedString ("key", "value", null, "comment");
			Assert.That (s, Is.EqualTo ("value"), "table");
			
			s = NSBundle.MainBundle.LocalizedString ("key", "value", "table", null);
			Assert.That (s, Is.EqualTo ("value"), "comment");
			
			s = main.LocalizedString (null, null, null, null);
			Assert.That (s, Is.Empty, "all-null");
		}
		
		// http://developer.apple.com/library/ios/#documentation/uikit/reference/NSBundle_UIKitAdditions/Introduction/Introduction.html
		
#if false // Disabling for now due to Xcode 9 does not support nibs if deployment target == 6.0
#if !__WATCHOS__
		[Test]
		public void LoadNibWithOptions ()
		{
#if MONOMAC
			NSArray objects;
			Assert.NotNull (main.LoadNibNamed ("EmptyNib", main, out objects));
#else
			Assert.NotNull (main.LoadNib ("EmptyNib", main, null));
#endif
		}
#endif // !__WATCHOS__
#endif

#if false
		// some selectors are only in AppKit but we included them in MonoTouch (and this match Apple documentation)
		// https://developer.apple.com/library/mac/#documentation/Cocoa/Reference/ApplicationKit/Classes/NSBundle_AppKitAdditions/Reference/Reference.html
		
		// I guess no one ever used them since they don't work...
		// commented (selectors removed from MT bindings) - can be re-enabled to test newer iOS releases
		[Test]
		[ExpectedException (typeof (MonoTouchException))]
		public void PathForImageResource ()
		{
			main.PathForImageResource ("basn3p08.png");
		}

		[Test]
		[ExpectedException (typeof (MonoTouchException))]
		public void PathForSoundResource ()
		{
			main.PathForSoundResource ("basn3p08.png");
		}

		[Test]
		[ExpectedException (typeof (MonoTouchException))]
		public void LoadNib ()
		{
			NSBundle.LoadNib (String.Empty, main);
		}
#endif
		[Test]
		public void Localizations ()
		{
			string [] locz = main.Localizations;
			Assert.That (locz.Length, Is.GreaterThanOrEqualTo (0), "Length");
		}

		[Test]
		public void PreferredLocalizations ()
		{
			string [] locz = main.PreferredLocalizations;
			Assert.That (locz.Length, Is.GreaterThanOrEqualTo (1), "Length");
			Assert.That (locz, Contains.Item ("Base"), "Base");
		}

		[Test]
		public void AppStoreReceiptURL ()
		{
			if (!TestRuntime.CheckXcodeVersion (5, 0))
				Assert.Inconclusive ("Requires iOS7 or later");

			// on iOS8 device this now ends with "/StoreKit/sandboxReceipt" 
			// instead of "/StokeKit/receipt"
			Assert.That (main.AppStoreReceiptUrl.AbsoluteString, Is.StringEnding ("eceipt"), "AppStoreReceiptUrl");
		}

#if !XAMCORE_4_0
		[Test]
		public void LocalizedString ()
		{
			// null values are fine
			var l2 = main.LocalizedString (null, null);
			Assert.That (l2.Length, Is.EqualTo (0), "null,null");
			var l3 = main.LocalizedString (null, null, null);
			Assert.That (l3.Length, Is.EqualTo (0), "null,null,null");
			var l4 = main.LocalizedString (null, null, null, null);
			Assert.That (l4.Length, Is.EqualTo (0), "null,null,null,null");

			// NoKey does not exists so the same string is returned
			l2 = main.LocalizedString ("NoKey", null);
			Assert.That (l2, Is.EqualTo ("NoKey"), "string,null");
			l3 = main.LocalizedString ("NoKey", null, null);
			Assert.That (l3, Is.EqualTo ("NoKey"), "string,null,null");
			l4 = main.LocalizedString ("NoKey", null, null, null);
			Assert.That (l4, Is.EqualTo ("NoKey"), "string,null,null,null");

			// TestKey exists (Localizable.strings) and returns TestValue
			l2 = main.LocalizedString ("TestKey", null);
			Assert.That (l2, Is.EqualTo ("TestValue"), "string,null-2");
			l3 = main.LocalizedString ("TestKey", null, null);
			Assert.That (l3, Is.EqualTo ("TestValue"), "string,null,null-2");
			l4 = main.LocalizedString ("TestKey", null, null, null);
			Assert.That (l4, Is.EqualTo ("TestValue"), "string,null,null,null-2");
		}
#endif

		[Test]
		public void GetLocalizedString ()
		{
			// null values are fine
			using (var l = main.GetLocalizedString (null, null, null))
				Assert.That (l.Length, Is.EqualTo (0), "null,null,null");

			// NoKey does not exists so the same string is returned
			using (var l = main.GetLocalizedString ("NoKey", null, null))
				Assert.That (l.ToString (), Is.EqualTo ("NoKey"), "string,null,null");
			using (var key = new NSString ("NoKey"))
			using (var l = main.GetLocalizedString (key, null, null))
				Assert.That (l.ToString (), Is.EqualTo ("NoKey"), "NString,null,null");

			// TestKey exists (Localizable.strings) and returns TestValue
			using (var l = main.GetLocalizedString ("TestKey", null, null))
				Assert.That (l.ToString (), Is.EqualTo ("TestValue"), "string,null,null-2");
			using (var key = new NSString ("TestKey"))
			using (var l = main.GetLocalizedString (key, null, null))
				Assert.That (l.ToString (), Is.EqualTo ("TestValue"), "NString,null,null-2");
		}
	}
}
