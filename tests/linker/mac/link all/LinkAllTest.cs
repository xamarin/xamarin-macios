using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

using AppKit;
using Foundation;
using ObjCRuntime;

using NUnit.Framework;

namespace LinkAllTests {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class LinkAllTest {
#if !NET // this test is in a file shared with all platforms for .NET
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
			Check ("UmAlQuraCalendar", false);
			Check ("HijriCalendar", false);
			Check ("ThaiBuddhistCalendar", false);
		}
#endif // !NET

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
			[BindingImpl (BindingImplOptions.Optimizable)]
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

		[Test]
		public void XmlSerialization ()
		{
			const string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
				"<SerializeMe xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" SetMe=\"2\">" +
				"</SerializeMe>";
			using (var sr = new StringReader (xml))
			using (var xr = new XmlTextReader (sr)) {
				var xs = new XmlSerializer (typeof (SerializeMe));
				SerializeMe item = xs.Deserialize (xr) as SerializeMe;
				Assert.AreEqual (2, item.SetMe, "SetMe");
			}
		}

		public class SerializeMe {

			[XmlAttribute]
			public int SetMe { get; set; }

			public SerializeMe ()
			{
				SetMe = 1;
			}
		}
	}
}
