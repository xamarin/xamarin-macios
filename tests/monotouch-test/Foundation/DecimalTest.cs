//
// Unit tests for NSDecimal
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2014-2015 Xamarin Inc. All rights reserved.
//

using System.Globalization;

using Foundation;

using NUnit.Framework;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class DecimalTest {

		[Test]
		public void CastFloat ()
		{
			float f = 0.7f;
			NSDecimal nsd = (NSDecimal) f;
			// We call NSNumberFormatter to adjust the test according to the iOS (not .NET) culture,
			// since NSDecimal.ToString () takes culture into account. .NET CultureInfo does not handle cases
			// like "en-HU"
			if (TestRuntime.CheckXcodeVersion (4, 5) && (CultureInfo.CurrentCulture.Name != "ar-AE")) {
				// interestingly the call to `NSDecimal.NSDecimalString (ref this, NSLocale.CurrentLocale.Handle);` does not consider the current locale on iOS 5.1
				var expected = NSNumberFormatter.LocalizedStringFromNumbernumberStyle ((NSNumber) f, NSNumberFormatterStyle.Decimal);
				Assert.That (nsd.ToString (), Is.EqualTo (expected), "ToString");
			}
			Assert.That (f, Is.EqualTo ((float) nsd), "float-rountrip");
		}

		[Test]
		public void CastDecimal ()
		{
			decimal m = 0.7m;
			NSDecimal nsd = (NSDecimal) m;
			// We call NSNumberFormatter to adjust the test according to the iOS (not .NET) culture,
			// since NSDecimal.ToString () takes culture into account. .NET CultureInfo does not handle cases
			// like "en-HU"
			// note: there's no NSNumber / Decimal conversions
			if (TestRuntime.CheckXcodeVersion (4, 5) && (CultureInfo.CurrentCulture.Name != "ar-AE")) {
				// interestingly the call to `NSDecimal.NSDecimalString (ref this, NSLocale.CurrentLocale.Handle);` does not consider the current locale on iOS 5.1
				var expected = NSNumberFormatter.LocalizedStringFromNumbernumberStyle ((NSNumber) 0.7d, NSNumberFormatterStyle.Decimal);
				Assert.That (nsd.ToString (), Is.EqualTo (expected), "ToString");
			}
			Assert.That (m, Is.EqualTo ((decimal) nsd), "decimal-rountrip");
		}

		[Test]
		public void CastDouble ()
		{
			double d = 0.7d;
			NSDecimal nsd = (NSDecimal) d;
			// We call NSNumberFormatter to adjust the test according to the iOS (not .NET) culture,
			// since NSDecimal.ToString () takes culture into account. .NET CultureInfo does not handle cases
			// like "en-HU"
			if (TestRuntime.CheckXcodeVersion (4, 5) && (CultureInfo.CurrentCulture.Name != "ar-AE")) {
				// interestingly the call to `NSDecimal.NSDecimalString (ref this, NSLocale.CurrentLocale.Handle);` does not consider the current locale on iOS 5.1
				var expected = NSNumberFormatter.LocalizedStringFromNumbernumberStyle ((NSNumber) d, NSNumberFormatterStyle.Decimal);
				Assert.That (nsd.ToString (), Is.EqualTo (expected), "ToString");
			}
			Assert.That (d, Is.EqualTo ((double) nsd), "double-rountrip");
		}

		[Test]
		public void CastInt ()
		{
			int i = 42;
			NSDecimal nsd = (NSDecimal) i;
			Assert.That (nsd.ToString (), Is.EqualTo ("42"), "ToString");
			Assert.That (i, Is.EqualTo ((int) nsd), "int-rountrip");
		}
	}
}
