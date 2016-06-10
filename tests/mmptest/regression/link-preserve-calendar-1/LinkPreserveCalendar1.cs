using System;
using MonoMac.AppKit;

// Test
// * application .exe does not *directly* use any Calendar
// * only the default GregorianCalendar is linked in
//
// Requirement
// * Link SDK or Link All must be enabled

namespace Xamarin.Mac.Linker.Test {
	
	class PreserveCalendar {

		static void Check (string calendarName, bool present)
		{
			var type = Type.GetType ("System.Globalization." + calendarName);
			bool success = present == (type != null);
			Test.Log.WriteLine ("[{0}]\t{1} {2}", success ? "PASS" : "FAIL", calendarName, present ? "present" : "absent");
		}

		static void Main (string[] args)
		{
			NSApplication.Init ();
			
			Test.EnsureLinker (true);

			Check ("GregorianCalendar", true);
			Check ("UmAlQuraCalendar", false);
			Check ("HijriCalendar", false);
			Check ("ThaiBuddhistCalendar", false);

			Test.Terminate ();
		}
	}
}