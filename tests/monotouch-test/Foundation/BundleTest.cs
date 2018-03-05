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
			Assert.That (locz, Contains.Item ("en"), "en");
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
	}
}
