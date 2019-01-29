using System;
using Foundation;

using NUnit.Framework;

namespace LinkAllTests
{
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class LinkAllTest
	{

		static void Check (string calendarName, bool present)
		{
			var type = Type.GetType ("System.Globalization." + calendarName);
			bool success = present == (type != null);
			Assert.AreEqual (present, type != null, calendarName);
		}

		[Test]
		public void Calendars ()
		{
			Check ("GregorianCalendar", true);
			Check ("UmAlQuraCalendar", false);
			Check ("HijriCalendar", false);
			Check ("ThaiBuddhistCalendar", false);
		}
	}
}
