using System;
using Foundation;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSLocaleTest {

		public void DisplayCountryCodeNames (NSString s)
		{
			Console.WriteLine (s);
			NSLocale current = NSLocale.CurrentLocale;
			IntPtr handle = current.Handle;
			IntPtr selDisplayNameForKeyValue = new Selector ("displayNameForKey:value:").Handle;
			foreach (var countryCode in NSLocale.ISOCountryCodes) {
				using (var nsvalue = new NSString (countryCode)) {
					string ret = NSString.FromHandle (Messaging.IntPtr_objc_msgSend_IntPtr_IntPtr (handle, selDisplayNameForKeyValue, s.Handle, nsvalue.Handle));
					if (!String.IsNullOrWhiteSpace (ret))
						Console.WriteLine ("{0} -> {1}", countryCode, ret);
				}
			}
		}

		[Test]
		public void GetNSObject_IntPtrZero ()
		{
#if false
			DisplayCountryCodeNames (NSLocale._AlternateQuotationBeginDelimiterKey);
			DisplayCountryCodeNames (NSLocale._AlternateQuotationEndDelimiterKey);
			DisplayCountryCodeNames (NSLocale._Calendar);
			DisplayCountryCodeNames (NSLocale._CollationIdentifier);
			DisplayCountryCodeNames (NSLocale._CollatorIdentifier);
			DisplayCountryCodeNames (NSLocale._CurrencyCode);
			DisplayCountryCodeNames (NSLocale._CountryCode);
			DisplayCountryCodeNames (NSLocale._CurrencySymbol);
			DisplayCountryCodeNames (NSLocale._DecimalSeparator);
			DisplayCountryCodeNames (NSLocale._ExemplarCharacterSet);
			DisplayCountryCodeNames (NSLocale._GroupingSeparator);
			DisplayCountryCodeNames (NSLocale._Identifier);
			DisplayCountryCodeNames (NSLocale._LanguageCode);
			DisplayCountryCodeNames (NSLocale._MeasurementSystem);
			DisplayCountryCodeNames (NSLocale._QuotationBeginDelimiterKey);
			DisplayCountryCodeNames (NSLocale._QuotationEndDelimiterKey);
			DisplayCountryCodeNames (NSLocale._ScriptCode);
			DisplayCountryCodeNames (NSLocale._UsesMetricSystem);
			DisplayCountryCodeNames (NSLocale._VariantCode);
#endif
		}
	}
}
