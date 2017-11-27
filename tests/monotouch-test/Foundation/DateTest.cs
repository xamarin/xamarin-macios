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

			Assert.AreEqual (DateTime.MinValue.AddSeconds (1), (DateTime)NSDate.FromTimeIntervalSinceReferenceDate (-63113903999), "-63113903999");
			Assert.AreEqual (DateTime.MinValue, (DateTime)NSDate.FromTimeIntervalSinceReferenceDate (-63113904000), "-63113904000");

			// NSDate doesn't seem to store the 'TimeIntervalSinceReferenceDate' as a double, causing a slight time warp. Round a bit to rewarp back.
			Assert.AreEqual (new DateTime (9999, 12, 31, 23, 59, 58), new DateTime ((((DateTime)NSDate.FromTimeIntervalSinceReferenceDate (252423993598)).Ticks / 10000) * 10000, DateTimeKind.Utc), "252423993598");
			Assert.AreEqual (new DateTime (9999, 12, 31, 23, 59, 59), new DateTime ((((DateTime)NSDate.FromTimeIntervalSinceReferenceDate (252423993599)).Ticks / 10000) * 10000, DateTimeKind.Utc), "252423993599");
		}

		[Test]
		public void OutLimits ()
		{
#if XAMCORE_2_0
			Assert.Throws<ArgumentOutOfRangeException> (() => { var tmp = (DateTime) NSDate.DistantPast; }, "distant past");
			Assert.Throws<ArgumentOutOfRangeException> (() => { var tmp = (DateTime) NSDate.FromTimeIntervalSinceReferenceDate (-63113904001); }, "-63113904001");

			Assert.Throws<ArgumentOutOfRangeException> (() => { var tmp = (DateTime) NSDate.FromTimeIntervalSinceReferenceDate (252423993600); }, "252423993600");

			// [Min|Max]Value are DateTimeKind.Unspecified
			Assert.Throws<ArgumentException> (() => { var tmp = (NSDate) DateTime.MinValue; }, "MinValue");
			Assert.Throws<ArgumentException> (() => { var tmp = (NSDate) DateTime.MaxValue; }, "MaxValue");
#else
			Assert.AreEqual (DateTime.MinValue, (DateTime)NSDate.DistantPast, "distant past");
			Assert.AreEqual (DateTime.MinValue, (DateTime)NSDate.FromTimeIntervalSinceReferenceDate (-63113904001), "-63113904001");

			Assert.AreEqual (DateTime.MaxValue, (DateTime)NSDate.FromTimeIntervalSinceReferenceDate (252423993600), "252423993600");

			Assert.DoesNotThrow (() => { var tmp = (NSDate) DateTime.MinValue; }, "MinValue");
			Assert.DoesNotThrow (() => { var tmp = (NSDate) DateTime.MaxValue; }, "MaxValue");
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
			// ensure decimals are not truncated - but there's an unavoidable loss of precision
			Assert.AreEqual (b, 0, 0.00001, "1");
		}
	}
}
