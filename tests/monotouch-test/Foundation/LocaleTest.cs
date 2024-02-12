//
// Unit tests for NSLocale
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012-2013 Xamarin Inc. All rights reserved.
//

using System;
using System.Globalization;
using Foundation;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class LocaleTest {

		[Test]
		public void CurrentLocale ()
		{
			Assert.NotNull (NSLocale.CurrentLocale, "CurrentLocale");
		}

		[Test]
		public void FromLocaleIdentifier ()
		{
			TestRuntime.AssertXcodeVersion (5, 0);

			string ident = NSLocale.CurrentLocale.Identifier;
			Assert.That (NSLocale.FromLocaleIdentifier (ident).Identifier, Is.EqualTo (ident), "FromLocaleIdentifier");
		}

		[Test]
		public void InitRegionInfo ()
		{
			string name = NSLocale.CurrentLocale.CountryCode; // two letter code
															  // Handle manually set locale (without country) in iOS Simulator (plist) - ref bug #18520
			if (name == null)
				Assert.Inconclusive ("You can construct locale without countries");
			RegionInfo ri = new RegionInfo (name);
			Assert.That (ri.Name, Is.EqualTo (name), "Name");
		}

		[Test]
		public void CountryLessLocale ()
		{
			string name = "zh-Hans"; // there's no country data from the supplied name - ref bug #18520
			using (NSLocale locale = new NSLocale (name)) {
				Assert.Null (locale.CountryCode, "CountryCode");
			}
		}

		[Test]
		public void Properties ()
		{
			using (NSLocale en = new NSLocale ("en-US")) {
				Assert.That (en.AlternateQuotationBeginDelimiterKey, Is.EqualTo ("‘"), "AlternateQuotationBeginDelimiterKey");
				Assert.That (en.AlternateQuotationEndDelimiterKey, Is.EqualTo ("’"), "AlternateQuotationEndDelimiterKey");
				if (TestRuntime.CheckXcodeVersion (15, 0)) {
					Assert.That (en.CollationIdentifier, Is.EqualTo ("standard"), "CollationIdentifier");
				} else {
					Assert.Null (en.CollationIdentifier, "CollationIdentifier");
				}
				Assert.That (en.CollatorIdentifier, Is.EqualTo ("en-US"), "CollatorIdentifier");
				Assert.That (en.CountryCode, Is.EqualTo ("US"), "CountryCode");
				Assert.That (en.CurrencyCode, Is.EqualTo ("USD"), "CurrencyCode");
				Assert.That (en.CurrencySymbol, Is.EqualTo ("$"), "CurrencySymbol");
				Assert.That (en.DecimalSeparator, Is.EqualTo ("."), "DecimalSeparator");
				Assert.That (en.GroupingSeparator, Is.EqualTo (","), "GroupingSeparator");
				Assert.That (en.Identifier, Is.EqualTo ("en-US"), "Identifier");
				Assert.That (en.LanguageCode, Is.EqualTo ("en"), "LanguageCode");
				Assert.That (en.LocaleIdentifier, Is.EqualTo ("en-US"), "LocaleIdentifier");
				Assert.That (en.MeasurementSystem, Is.EqualTo ("U.S."), "MeasurementSystem");
				Assert.That (en.QuotationBeginDelimiterKey, Is.EqualTo ("“"), "QuotationBeginDelimiterKey");
				Assert.That (en.QuotationEndDelimiterKey, Is.EqualTo ("”"), "QuotationEndDelimiterKey");
			}
		}

		[Test]
		public void DisplayName_En ()
		{
			using (NSLocale en = new NSLocale ("en-US")) {
				Assert.That (en.GetIdentifierDisplayName (en.Identifier), Is.EqualTo ("English (United States)"), "Identifier");
				Assert.That (en.GetLanguageCodeDisplayName (en.LanguageCode), Is.EqualTo ("English"), "LanguageCode");
				Assert.That (en.GetCountryCodeDisplayName (en.CountryCode), Is.EqualTo ("United States"), "CountryCode");
				Assert.That (en.GetCurrencyCodeDisplayName (en.CurrencyCode), Is.EqualTo ("US Dollar"), "CurrencyCode");
			}
		}

		[Test]
		public void DisplayName_Fr ()
		{
			using (NSLocale en = new NSLocale ("en-US"))
			using (NSLocale fr = new NSLocale ("fr-CA")) {
				Assert.That (fr.GetIdentifierDisplayName (en.Identifier), Is.EqualTo ("anglais (États-Unis)"), "Identifier");
				Assert.That (fr.GetLanguageCodeDisplayName (en.LanguageCode), Is.EqualTo ("anglais"), "LanguageCode");
				Assert.That (fr.GetCountryCodeDisplayName (en.CountryCode), Is.EqualTo ("États-Unis"), "CountryCode");
				Assert.That (fr.GetCurrencyCodeDisplayName (en.CurrencyCode), Is.EqualTo ("dollar des États-Unis"), "CurrencyCode");

				Assert.That (en.GetIdentifierDisplayName (fr.Identifier), Is.EqualTo ("French (Canada)"), "Identifier");
				Assert.That (en.GetLanguageCodeDisplayName (fr.LanguageCode), Is.EqualTo ("French"), "LanguageCode");
				Assert.That (en.GetCountryCodeDisplayName (fr.CountryCode), Is.EqualTo ("Canada"), "CountryCode");
				Assert.That (en.GetCurrencyCodeDisplayName (fr.CurrencyCode), Is.EqualTo ("Canadian Dollar"), "CurrencyCode");
			}
		}
	}
}
