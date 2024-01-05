//
// NSLocale.cs: extensions for the NSLocale class
//
// Authors:
//   Miguel de Icaza (miguel@xamarin.com)
//
// Copyright 2011 Xamarin Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//

using System;
using ObjCRuntime;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Foundation {

	public partial class NSLocale {
		public string Identifier {
			get {
				return (string) (NSString) ObjectForKey (_Identifier);
			}
		}

		public string GetIdentifierDisplayName (string value)
		{
			return DisplayNameForKey (_Identifier, value);
		}

		public string LanguageCode {
			get {
				return (string) (NSString) ObjectForKey (_LanguageCode);
			}
		}

		public string GetLanguageCodeDisplayName (string value)
		{
			return DisplayNameForKey (_LanguageCode, value);
		}

		public string CountryCode {
			get {
				return (string) (NSString) ObjectForKey (_CountryCode);
			}
		}

		public string GetCountryCodeDisplayName (string value)
		{
			return DisplayNameForKey (_CountryCode, value);
		}

		public string ScriptCode {
			get {
				return (string) (NSString) ObjectForKey (_ScriptCode);
			}
		}

		public string VariantCode {
			get {
				return (string) (NSString) ObjectForKey (_VariantCode);
			}
		}

		public NSCharacterSet ExemplarCharacterSet {
			get {
				return ObjectForKey (_ExemplarCharacterSet) as NSCharacterSet;
			}
		}

		public NSCalendar Calendar {
			get {
				return ObjectForKey (_Calendar) as NSCalendar;
			}
		}

		public string CollationIdentifier {
			get {
				return (string) (NSString) ObjectForKey (_CollationIdentifier);
			}
		}

		public bool UsesMetricSystem {
			get {
				return ((NSNumber) ObjectForKey (_UsesMetricSystem)).Int32Value != 0;
			}
		}

		public string MeasurementSystem {
			get {
				return (string) (NSString) ObjectForKey (_MeasurementSystem);
			}
		}

		public string DecimalSeparator {
			get {
				return (string) (NSString) ObjectForKey (_DecimalSeparator);
			}
		}

		public string GroupingSeparator {
			get {
				return (string) (NSString) ObjectForKey (_GroupingSeparator);
			}
		}

		public string CurrencySymbol {
			get {
				return (string) (NSString) ObjectForKey (_CurrencySymbol);
			}
		}

		public string CurrencyCode {
			get {
				return (string) (NSString) ObjectForKey (_CurrencyCode);
			}
		}

		public string GetCurrencyCodeDisplayName (string value)
		{
			return DisplayNameForKey (_CurrencyCode, value);
		}

		public string CollatorIdentifier {
			get {
				return (string) (NSString) ObjectForKey (_CollatorIdentifier);
			}
		}

		public string QuotationBeginDelimiterKey {
			get {
				return (string) (NSString) ObjectForKey (_QuotationBeginDelimiterKey);
			}
		}

		public string QuotationEndDelimiterKey {
			get {
				return (string) (NSString) ObjectForKey (_QuotationEndDelimiterKey);
			}
		}

		public string AlternateQuotationBeginDelimiterKey {
			get {
				return (string) (NSString) ObjectForKey (_AlternateQuotationBeginDelimiterKey);
			}
		}

		public string AlternateQuotationEndDelimiterKey {
			get {
				return (string) (NSString) ObjectForKey (_AlternateQuotationEndDelimiterKey);
			}
		}
	}
}
