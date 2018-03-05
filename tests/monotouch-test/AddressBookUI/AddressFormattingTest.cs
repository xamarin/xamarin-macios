//
// Unit tests for ABAddressFormatting
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012-2014, 2016 Xamarin Inc. All rights reserved.
//

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
using System.Globalization;
#if XAMCORE_2_0
using Foundation;
using AddressBookUI;
using ObjCRuntime;
using UIKit;
#else
using MonoTouch.AddressBookUI;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.AddressBookUI {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ABAddressFormattingTest {
		
		[Test]
		public void ChateauFrontenac ()
		{
			using (NSMutableDictionary dict = new NSMutableDictionary () {
				{ new NSString ("Street"), new NSString ("1–3 Rue Des Carrières") }, 
				{ new NSString ("SubAdministrativeArea"), new NSString ("Québec") }, 
				{ new NSString ("Thoroughfare"), new NSString ("Rue Des Carrières") }, 
				{ new NSString ("ZIP"), new NSString ("G1R 5J5") }, 
				{ new NSString ("Name"), new NSString ("1–3 Rue Des Carrières") }, 
				{ new NSString ("City"), new NSString ("Quebec City") }, 
				{ new NSString ("State"), new NSString ("Quebec") }, 
				{ new NSString ("SubLocality"), new NSString ("Vieux-Quebec") }, 
				{ new NSString ("SubThoroughfare"), new NSString ("1-3") }, 
				{ new NSString ("CountryCode"), new NSString ("CA") }, 
			}) {
				string expected1 = "1–3 Rue Des Carrières\nQuebec City‎ Quebec‎ G1R 5J5";
				string expected2 = "1–3 Rue Des Carrières\nQuebec City Quebec G1R 5J5"; // there's a "(char) 8206" character just after 'Quebec'
				string expected;
				string s = ABAddressFormatting.ToString (dict, false);
				if (TestRuntime.CheckXcodeVersion (9, 0)) {
					expected = expected2;
				} else {
					expected = expected1;
				}
				Assert.That (s, Is.EqualTo (expected), "false");
				// country names can be translated, e.g. chinese, so we can't compare it
				s = ABAddressFormatting.ToString (dict, true);
				Assert.That (s, Is.StringStarting (expected), "prefix");

				// Apple broke this again (8.0.x are hard to predict) - test will fail once it's corrected
				// iOS 8.1.2 device: working
				// iOS 8.0 (12A365) simulator (Xcode 6.0.1): working
				// iOS 8.1 (12B411) simulator (Xcode 6.1): broken
				// iOS 8.2 beta 5 (12D5480a) simulator (Xcode 6.2 beta 5): working

				// we don't check before 8.2 - where both device and simulators works again properly
				if (!UIDevice.CurrentDevice.CheckSystemVersion (8,2))
					return;

				// iOS 11.0 beta 1, 2, 3 and 4 are broken
				// and I give up (this test was not meant to track Apple breakages)
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
