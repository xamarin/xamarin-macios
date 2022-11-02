//
// Unit tests for NSDateFormatter
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

using Foundation;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class DateFormatterTest {

		[Test]
		public void ToLocalizedStringTest ()
		{
			var str = NSDateFormatter.ToLocalizedString (NSDate.Now, NSDateFormatterStyle.Full, NSDateFormatterStyle.Full);
			Assert.IsNotNull (str);
		}

		[Test]
		public void GetDateFormatFromTemplateTest ()
		{
			var us_locale = new NSLocale ("en_US");
			var gb_locale = new NSLocale ("en_GB");
			const string dateComponents = "yMMMMd";

			var dateFormat = NSDateFormatter.GetDateFormatFromTemplate (dateComponents, 0, us_locale);
			Assert.AreEqual ("MMMM d, y", dateFormat, "#US");

			dateFormat = NSDateFormatter.GetDateFormatFromTemplate (dateComponents, 0, gb_locale);
			Assert.AreEqual ("d MMMM y", dateFormat, "GB");
		}
	}
}
