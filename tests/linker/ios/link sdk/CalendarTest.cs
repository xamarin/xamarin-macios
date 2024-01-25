// Copyright 2013 Xamarin Inc. All rights reserved.

using System;
using System.Globalization;

using Foundation;

using NUnit.Framework;

namespace LinkSdk.Calendars {

	[TestFixture]
	// we want the tests to be available because we use the linker
	[Preserve (AllMembers = true)]
	public class CalendarTest {

		// application must *NOT* be build with I18N.MidEast and I18N.Other (Thai)

		[Test]
		public void UmAlQura ()
		{
			var ci = CultureInfo.GetCultureInfo ("ar");
			Assert.That (ci.Calendar.ToString (), Is.EqualTo ("System.Globalization.GregorianCalendar"), "Calendar");
		}

		[Test]
		public void Hijri ()
		{
			var ci = CultureInfo.GetCultureInfo ("ps");
#if NET // https://github.com/dotnet/runtime/issues/50859
			Assert.That (ci.Calendar.ToString (), Is.EqualTo ("System.Globalization.PersianCalendar"), "Calendar");
#else
			Assert.That (ci.Calendar.ToString (), Is.EqualTo ("System.Globalization.GregorianCalendar"), "Calendar");
#endif
		}

		[Test]
		public void ThaiBuddhist ()
		{
			var ci = CultureInfo.GetCultureInfo ("th");
#if NET // https://github.com/dotnet/runtime/issues/50859
			Assert.That (ci.Calendar.ToString (), Is.EqualTo ("System.Globalization.ThaiBuddhistCalendar"), "Calendar");
#else
			Assert.That (ci.Calendar.ToString (), Is.EqualTo ("System.Globalization.GregorianCalendar"), "Calendar");
#endif
		}
	}
}
