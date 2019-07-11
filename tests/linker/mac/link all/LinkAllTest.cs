using System;
using System.Runtime.CompilerServices;
using System.Threading;

using AppKit;
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

		[Test]
		public void EnsureUIThreadException ()
		{
			// works on main/ui thread
			NSApplication.EnsureUIThread ();

			ThreadPool.QueueUserWorkItem ((v) => Tester.Test ());
			Assert.IsTrue (Tester.mre.WaitOne (TimeSpan.FromSeconds (10)), "Successful wait");
			// The UI thread check only happens for debug builds, on release build it's linked away.
#if DEBUG
			var expected_ex_thrown = true;
#else
			var expected_ex_thrown = false;
#endif
			Assert.AreEqual (expected_ex_thrown, Tester.exception_thrown, "Success");
		}


		class Tester : NSObject {
			public static ManualResetEvent mre = new ManualResetEvent (false);
			public static bool exception_thrown;

			[CompilerGenerated]
			[Export ("foo")]
			public static void Test ()
			{
				try {
					exception_thrown = false;
					NSApplication.EnsureUIThread ();
				} catch (AppKitThreadAccessException) {
					exception_thrown = true;
				} finally {
					mre.Set ();
				}
			}
		}
	}
}
