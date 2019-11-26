//
// Unit tests for NSDate
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

using System;
using System.Net;
#if XAMCORE_2_0
using Foundation;
using ObjCRuntime;
#else
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.Foundation {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class DateTest {

		[Test]
		public void InLimits ()
		{
			// .NET can represent this date just fine
			Assert.AreEqual (new DateTime (4001, 01, 01), (DateTime)NSDate.DistantFuture, "distant future");

			if (IntPtr.Size == 4) {
				Assert.AreEqual (DateTime.MinValue, (DateTime)NSDate.FromTimeIntervalSinceReferenceDate (-63113904000), "-63113904000");
				Assert.AreEqual (DateTime.MinValue.AddSeconds (1), (DateTime)NSDate.FromTimeIntervalSinceReferenceDate (-63113903999), "-63113903999");
				Assert.AreEqual (DateTime.MinValue, (DateTime)NSDate.FromTimeIntervalSinceReferenceDate (-63113904000), "-63113904000");

				// NSDate doesn't seem to store the 'TimeIntervalSinceReferenceDate' as a double, causing a slight time warp. Round a bit to rewarp back.
				Assert.AreEqual (new DateTime (9999, 12, 31, 23, 59, 58), new DateTime ((((DateTime)NSDate.FromTimeIntervalSinceReferenceDate (252423993598)).Ticks / 10000) * 10000, DateTimeKind.Utc), "252423993598");
				Assert.AreEqual (new DateTime (9999, 12, 31, 23, 59, 59), new DateTime ((((DateTime)NSDate.FromTimeIntervalSinceReferenceDate (252423993599)).Ticks / 10000) * 10000, DateTimeKind.Utc), "252423993599");

			} else {
				Assert.AreEqual (DateTime.MinValue, (DateTime)NSDate.DistantPast, "distant past");
				Assert.AreEqual (DateTime.MinValue.AddSeconds (1), (DateTime)NSDate.FromTimeIntervalSinceReferenceDate (-63114076799), "-63114076799");
				Assert.AreEqual (DateTime.MinValue, (DateTime)NSDate.FromTimeIntervalSinceReferenceDate (-63114076800), "-63114076800");

				// Converting NSDate -> DateTime limits precision to .001
				Assert.AreEqual (new DateTime ((DateTime.MaxValue.Ticks) / 10000 * 10000), (DateTime)NSDate.FromTimeIntervalSinceReferenceDate (252423993599.999));

				Assert.AreEqual (new DateTime (9999, 12, 31, 23, 59, 58), (DateTime)NSDate.FromTimeIntervalSinceReferenceDate (252423993598), "252423993598");
				Assert.AreEqual (new DateTime (9999, 12, 31, 23, 59, 59), (DateTime)NSDate.FromTimeIntervalSinceReferenceDate (252423993599), "252423993599");
			}
		}

		[Test]
		public void OutLimits ()
		{
#if XAMCORE_2_0
			Assert.Throws<ArgumentOutOfRangeException> (() => { var tmp = (DateTime) NSDate.FromTimeIntervalSinceReferenceDate (-63114076801); }, "-63114076801");

			Assert.Throws<ArgumentOutOfRangeException> (() => { var tmp = (DateTime) NSDate.FromTimeIntervalSinceReferenceDate (252423993600); }, "252423993600");

			// [Min|Max]Value are DateTimeKind.Unspecified
			Assert.Throws<ArgumentException> (() => { var tmp = (NSDate) DateTime.MinValue; }, "MinValue");
			Assert.Throws<ArgumentException> (() => { var tmp = (NSDate) DateTime.MaxValue; }, "MaxValue");
#endif
		}

		[Test]
		public void DescriptionWithLocale ()
		{
			Assert.IsNotNull (NSDate.Now.DescriptionWithLocale (NSLocale.CurrentLocale), "1");
		}

		[Test]
		public void Precision32022 ()
		{
			var a = NSDate.Now;
			var b = a.SecondsSinceReferenceDate - ((NSDate) (DateTime) a).SecondsSinceReferenceDate;

			if (IntPtr.Size == 4) {
				// ensure decimals are not truncated - but there's an unavoidable loss of precision
				Assert.AreEqual (b, 0, 0.00001, "1");
			}
			else {
				// ensure decimals are not truncated - but there's an unavoidable loss of precision from ns to ms
				Assert.AreEqual (b, 0, 0.001, "1");
			}
		}

		[TestCase (1, 1, 1, 1, 1, 1, 1)]
		[TestCase (199, 1, 1, 1, 1, 1, 1)]
		[TestCase (2019, 1, 1, 1, 1, 1, 1)]
		public void DateTimeToNSDate (int y, int m, int d, int h, int min, int s, int ms)
		{
			DateTime date;

			if (IntPtr.Size == 4)
				date = new DateTime (y, m, d, h, min, s, DateTimeKind.Utc);
			else
				date = new DateTime (y, m, d, h, min, s, ms, DateTimeKind.Utc);

			Assert.AreEqual (date, (DateTime) (NSDate) date, "DateTime -> NSDate -> DateTime conversion");
		}

		[Test]
		public void LocalTime ()
		{
			// NSDate -> DateTime conversion always returns DateTimeKind.Utc (Universal)
			if (IntPtr.Size == 4)
			{
				DateTime now = DateTime.Now.ToLocalTime ();
				Assert.AreEqual (new DateTime (now.ToUniversalTime ().Ticks / 10000 * 10000), new DateTime((((DateTime)(NSDate)now).Ticks / 10000) * 10000), "now");
			} else {
				// [Min][Max] DateTimeKind.Unspecified cannot be converted
				DateTime minDt = DateTime.MinValue.ToLocalTime ();
				DateTime maxDt = DateTime.MaxValue.ToLocalTime ();

				// NSDate -> DateTime limits precision to .001 on x64 due to ns -> ms conversion
				Assert.AreEqual (new DateTime (minDt.ToUniversalTime ().Ticks / 10000 * 10000), (DateTime)(NSDate)minDt, "-63114076800");
				Assert.AreEqual (new DateTime (maxDt.ToUniversalTime ().Ticks / 10000 * 10000), (DateTime)(NSDate)maxDt, "252423993599.999");
			}
		}

	}
}
