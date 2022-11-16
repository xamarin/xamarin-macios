//
// Unit tests for NSDate
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

using System;
using System.Collections.Generic;

using Foundation;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class DateTest {

		[Test]
		public void InLimits ()
		{
			// .NET can represent this date just fine
			Assert.AreEqual (new DateTime (4001, 01, 01), (DateTime) NSDate.DistantFuture, "distant future");

			Assert.AreEqual (DateTime.MinValue, (DateTime) NSDate.DistantPast, "distant past");
			Assert.AreEqual (DateTime.MinValue.AddSeconds (1), (DateTime) NSDate.FromTimeIntervalSinceReferenceDate (-63114076799), "-63114076799");
			Assert.AreEqual (DateTime.MinValue, (DateTime) NSDate.FromTimeIntervalSinceReferenceDate (-63114076800), "-63114076800");

			// Converting NSDate -> DateTime limits precision to .001
			Assert.AreEqual (new DateTime ((DateTime.MaxValue.Ticks) / 10000 * 10000), (DateTime) NSDate.FromTimeIntervalSinceReferenceDate (252423993599.999));

			Assert.AreEqual (new DateTime (9999, 12, 31, 23, 59, 58), (DateTime) NSDate.FromTimeIntervalSinceReferenceDate (252423993598), "252423993598");
			Assert.AreEqual (new DateTime (9999, 12, 31, 23, 59, 59), (DateTime) NSDate.FromTimeIntervalSinceReferenceDate (252423993599), "252423993599");
		}

		static IEnumerable<object []> GetArgumentOutOfRangeExceptionValues ()
		{
			yield return new object [] { NSDate.FromTimeIntervalSinceReferenceDate (252423993600), "252423993600" };
		}

		[Test]
		[TestCaseSource (nameof (GetArgumentOutOfRangeExceptionValues))]
		public void ThrowsArgumentOutOfRangeException (NSDate value, string message)
		{
			Assert.Throws<ArgumentOutOfRangeException> (() => {
				var tmp = (DateTime) value;
			}, $"Unexpectedly converted {value} ({message})");
		}

		static IEnumerable<object []> GetArgumentExceptionValues ()
		{
			// [Min|Max]Value are DateTimeKind.Unspecified
			yield return new object [] { DateTime.MinValue, "DateTime.MinValue" };
			yield return new object [] { DateTime.MaxValue, "DateTime.MaxValue" };
			yield return new object [] { default (DateTime), "default (DateTime)" };
		}

		[Test]
		[TestCaseSource (nameof (GetArgumentExceptionValues))]
		public void ThrowsArgumentException (DateTime value, string message)
		{
			Assert.Throws<ArgumentException> (() => {
				var tmp = (NSDate) value;
			}, $"Unexpectedly converted {value} ({message})");
		}

		static IEnumerable<DateTime> GetRoundTripFromDateTimeValues ()
		{
			yield return new DateTime (0, DateTimeKind.Utc);
			yield return DateTime.MaxValue.ToUniversalTime ();
			yield return DateTime.UtcNow;
		}

		[Test]
		[TestCaseSource (nameof (GetRoundTripFromDateTimeValues))]
		public void RoundTripFromDateTime (DateTime start)
		{
			var nsdate = (NSDate) start;
			var backAgain = (DateTime) nsdate;
			var range = 10000; // ticks per millisecond; allow for up to a millisecond off
			Assert.That (backAgain.Ticks, Is.InRange (start.Ticks - range, start.Ticks + range), "RoundTrip");
		}

		static IEnumerable<object []> GetRoundTripFromNSDateValues ()
		{
			yield return new object [] { NSDate.DistantPast, "DistantPast" };
			yield return new object [] { NSDate.DistantFuture, "DistantFuture" };
		}

		[Test]
		[TestCaseSource (nameof (GetRoundTripFromNSDateValues))]
		public void RoundTripFromNSDate (NSDate start, string message)
		{
			var nsdate = (DateTime) start;
			var backAgain = (NSDate) nsdate;
			Assert.AreEqual (start, backAgain, message);
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

			// ensure decimals are not truncated - but there's an unavoidable loss of precision from ns to ms
			Assert.AreEqual (b, 0, 0.001, "1");
		}

		[TestCase (1, 1, 1, 1, 1, 1, 1)]
		[TestCase (199, 1, 1, 1, 1, 1, 1)]
		[TestCase (2019, 1, 1, 1, 1, 1, 1)]
		public void DateTimeToNSDate (int y, int m, int d, int h, int min, int s, int ms)
		{
			var date = new DateTime (y, m, d, h, min, s, ms, DateTimeKind.Utc);

			Assert.AreEqual (date, (DateTime) (NSDate) date, "DateTime -> NSDate -> DateTime conversion");
		}

		[Test]
		public void LocalTime ()
		{
			// NSDate -> DateTime conversion always returns DateTimeKind.Utc (Universal)
			// [Min][Max] DateTimeKind.Unspecified cannot be converted
			DateTime minDt = DateTime.MinValue.ToLocalTime ();
			DateTime maxDt = DateTime.MaxValue.ToLocalTime ();

			// NSDate -> DateTime limits precision to .001 on x64 due to ns -> ms conversion
			Assert.AreEqual (new DateTime (minDt.ToUniversalTime ().Ticks / 10000 * 10000), (DateTime) (NSDate) minDt, "-63114076800");
			Assert.AreEqual (new DateTime (maxDt.ToUniversalTime ().Ticks / 10000 * 10000), (DateTime) (NSDate) maxDt, "252423993599.999");
		}

	}
}
