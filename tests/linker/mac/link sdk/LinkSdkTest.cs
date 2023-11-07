using System;
using Foundation;

using NUnit.Framework;

namespace LinkSdkTests {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class LinkSdkTest {
		static void Check (string calendarName, bool present)
		{
			var type = Type.GetType ("System.Globalization." + calendarName);
			bool success = present == (type is not null);
			Assert.AreEqual (present, type is not null, calendarName);
		}

		[Test]
		public void Calendars ()
		{
			Check ("GregorianCalendar", true);
			// because project options enabled them
			Check ("UmAlQuraCalendar", true);
			Check ("HijriCalendar", true);
			Check ("ThaiBuddhistCalendar", true);
		}
	}
}
