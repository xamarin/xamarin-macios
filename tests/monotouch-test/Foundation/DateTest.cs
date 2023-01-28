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

		// Converting NSDate -> DateTime limits precision. The exact amount
		// depends on the date (the further in the future the bigger the
		// loss), but at DateTime.MaxValue it's around 91.5 microseconds.
		TimeSpan tolerance = TimeSpan.FromTicks (920 /* 92 microseconds */);

		[Test]
		public void InLimits ()
		{
			// .NET can represent this date just fine
			Assert.AreEqual (new DateTime (4001, 01, 01), (DateTime) NSDate.DistantFuture, "distant future");

			Assert.AreEqual (DateTime.MinValue, (DateTime) NSDate.DistantPast, "distant past");
			Assert.AreEqual (DateTime.MinValue.AddSeconds (1), (DateTime) NSDate.FromTimeIntervalSinceReferenceDate (-63114076799), "-63114076799");
			Assert.AreEqual (DateTime.MinValue, (DateTime) NSDate.FromTimeIntervalSinceReferenceDate (-63114076800), "-63114076800");

			Asserts.AreEqual (DateTime.MaxValue, (DateTime) NSDate.FromTimeIntervalSinceReferenceDate (252423993599.9999), tolerance, "DateTime.MaxValue");
			Asserts.AreEqual (DateTime.MaxValue, (DateTime) NSDate.FromTimeIntervalSinceReferenceDate (252423993599.9999999), tolerance, "DateTime.MaxValue");
			Asserts.AreEqual (DateTime.MaxValue, (DateTime) NSDate.FromTimeIntervalSinceReferenceDate (252423993600), tolerance, "DateTime.MaxValue");

			Assert.AreEqual (new DateTime (9999, 12, 31, 23, 59, 58), (DateTime) NSDate.FromTimeIntervalSinceReferenceDate (252423993598), "252423993598");
			Assert.AreEqual (new DateTime (9999, 12, 31, 23, 59, 59), (DateTime) NSDate.FromTimeIntervalSinceReferenceDate (252423993599), "252423993599");
		}

		static IEnumerable<object []> GetArgumentOutOfRangeExceptionValues ()
		{
			yield return new object [] { NSDate.FromTimeIntervalSinceReferenceDate (double.MaxValue), "double.MaxValue" };
			yield return new object [] { NSDate.FromTimeIntervalSinceReferenceDate (double.MinValue), "double.MinValue" };
			yield return new object [] { NSDate.FromTimeIntervalSinceReferenceDate (-63114076801), "-63114076801" };
			yield return new object [] { NSDate.FromTimeIntervalSinceReferenceDate (252423993601), "252423993601" };
			yield return new object [] { NSDate.FromTimeIntervalSinceReferenceDate (252423993600.0001), "252423993600.0001" };
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
			yield return DateTime.SpecifyKind (DateTime.MaxValue, DateTimeKind.Utc);
			yield return DateTime.MinValue.ToUniversalTime ();
			yield return DateTime.SpecifyKind (DateTime.MinValue, DateTimeKind.Utc);
			yield return DateTime.UtcNow;
		}

		[Test]
		[TestCaseSource (nameof (GetRoundTripFromDateTimeValues))]
		public void RoundTripFromDateTime (DateTime start)
		{
			NSDate nsdate = null;
			DateTime backAgain = default (DateTime);
			Assert.DoesNotThrow (() => nsdate = (NSDate) start, $"Start ticks: {start.Ticks}");
			Assert.DoesNotThrow (() => backAgain = (DateTime) nsdate, $"Start ticks: {start.Ticks} Subsequent seconds: {nsdate.SecondsSinceReferenceDate:R}");
			Asserts.AreEqual (start, backAgain, tolerance, "RoundTrip");
		}

		static IEnumerable<object []> GetRoundTripFromNSDateValues ()
		{
			yield return new object [] { NSDate.DistantPast, "DistantPast" };
			yield return new object [] { NSDate.DistantFuture, "DistantFuture" };
			yield return new object [] { NSDate.FromTimeIntervalSinceReferenceDate (252423993600.00001), "252423993600.00001" };
		}

		[Test]
		[TestCaseSource (nameof (GetRoundTripFromNSDateValues))]
		public void RoundTripFromNSDate (NSDate start, string message)
		{
			var date = (DateTime) start;
			var backAgain = (NSDate) date;
			Assert.AreEqual (start, backAgain, message);
		}

		[Test]
		public void DescriptionWithLocale ()
		{
			Assert.IsNotNull (NSDate.Now.DescriptionWithLocale (NSLocale.CurrentLocale), "1");
		}

		[TestCase (1, 1, 1, 1, 1, 1, 1)]
		[TestCase (199, 1, 1, 1, 1, 1, 1)]
		[TestCase (2019, 1, 1, 1, 1, 1, 1)]
		public void DateTimeToNSDate (int y, int m, int d, int h, int min, int s, int ms)
		{
			var date = new DateTime (y, m, d, h, min, s, ms, DateTimeKind.Utc);

			Asserts.AreEqual (date, (DateTime) (NSDate) date, tolerance, "DateTime -> NSDate -> DateTime conversion");
		}

		[Test]
		public void LocalTime ()
		{
			// NSDate -> DateTime conversion always returns DateTimeKind.Utc (Universal)
			// [Min][Max] DateTimeKind.Unspecified cannot be converted
			var minDt = DateTime.MinValue;
			var maxDt = DateTime.MaxValue;

			// NSDate -> DateTime limits precision to .001 on x64 due to ns -> ms conversion
			Asserts.AreEqual (minDt.ToUniversalTime (), (DateTime) (NSDate) minDt.ToLocalTime (), tolerance, "-63114076800");
			Asserts.AreEqual (maxDt.ToUniversalTime (), (DateTime) (NSDate) maxDt.ToLocalTime (), tolerance, "252423993599.999");
		}

	}
}
