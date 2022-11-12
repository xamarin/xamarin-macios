//
// Unit tests for EKRecurrenceRule
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#if !__TVOS__

using System;
using EventKit;
using Foundation;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.EventKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class RecurrenceRuleTest {
		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 8, throwIfOtherPlatform: false);
		}

		[Test]
		public void DefaultProperties ()
		{
			using (var rule = new EKRecurrenceRule ()) {
				Assert.AreEqual ("gregorian", rule.CalendarIdentifier, "CalendarIdentifier");
				Assert.IsNull (rule.RecurrenceEnd, "RecurrenceEnd");
				Assert.AreEqual (EKRecurrenceFrequency.Weekly, rule.Frequency, "Frequency");
				Assert.AreEqual ((nint) 1, rule.Interval, "Interval");
#if NET
				Assert.AreEqual (EKWeekday.Monday, rule.FirstDayOfTheWeek, "FirstDayOfTheWeek");
#else
				Assert.AreEqual (EKDay.Monday, rule.FirstDayOfTheWeek, "FirstDayOfTheWeek");
#endif
				Assert.IsNull (rule.DaysOfTheWeek, "DaysOfTheWeek");
				Assert.IsNull (rule.DaysOfTheMonth, "DaysOfTheMonth");
				Assert.IsNull (rule.DaysOfTheYear, "DaysOfTheYear");
				Assert.IsNull (rule.WeeksOfTheYear, "WeeksOfTheYear");
				Assert.IsNull (rule.MonthsOfTheYear, "MonthsOfTheYear");
				Assert.IsNull (rule.SetPositions, "SetPositions");
			}
		}

		[Test]
		public void Constructors ()
		{
			using (var rule = new EKRecurrenceRule (EKRecurrenceFrequency.Daily, 9, null)) {
			}
			using (var rule = new EKRecurrenceRule (EKRecurrenceFrequency.Yearly, 8, null, null, null, null, null, null, null)) {
			}
		}
	}
}

#endif // !__TVOS__
