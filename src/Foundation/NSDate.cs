//
// Authors:
//   Miguel de Icaza
//
// Copyright 2009-2010, Novell, Inc.
// Copyright 2014 Xamarin Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

using ObjCRuntime;

#nullable enable

namespace Foundation {

	public partial class NSDate {
		const int NANOSECS_PER_TICKS = 100;
		const int NANOSECS_PER_MICROSEC = 1000;
		const int NANOSECS_PER_MILLISEC = 1000000;

		static readonly NSCalendar calendar = new NSCalendar (NSCalendarType.Gregorian) { TimeZone = NSTimeZone.FromName ("UTC") };

		// Converting NSDate -> DateTime limits precision. The exact amount
		// depends on the date (the further in the future the bigger the
		// loss), but at DateTime.MaxValue it's around 91.5 microseconds.

		// DateTime and NSDate computes time differently: basically they disagree on how many seconds or ticks have passed
		// since a given date.
		// Example: DateTime.MinValue is "January 1, 0001", but if you ask DateTime how many seconds ago that was, and give that
		// number to NSDate, you get a date a couple of days later.
		// So instead convert between NSDate and DateTime by getting the date and time components.
		//
		// Also note that converting NSDate -> DateTime limits precision. The
		// exact amount depends on the date (the further in the future the
		// bigger the loss), but at DateTime.MaxValue it's around 91.5
		// microseconds.
		public static explicit operator DateTime (NSDate d)
		{
			var units = NSCalendarUnit.Era | NSCalendarUnit.Year | NSCalendarUnit.Month | NSCalendarUnit.Day | NSCalendarUnit.Hour |
				NSCalendarUnit.Minute | NSCalendarUnit.Second | NSCalendarUnit.Nanosecond | NSCalendarUnit.Calendar;

			using var calComponents = calendar.Components (units, d);

			// DateTime doesn't support dates starting with year 10.000.
			// Except for the year 10.000 on the dot: we convert it to DateTime.MaxValue.
			//
			// DateTime.MaxValue is actually one tick before year 10.000. This
			// means that when we convert DateTime.MaxValue to NSDate, we
			// actually end up with a date in year 10.000 due to precision
			// differences. In order to be able to roundtrip a
			// DateTime.MaxValue value, we hardcode the corresponding
			// NSDate.SecondsSinceReferenceDate here.
			if (calComponents.Year >= 10000) {
				if (d.SecondsSinceReferenceDate == 252423993600)
					return DateTime.SpecifyKind (DateTime.MaxValue, DateTimeKind.Utc);
				throw new ArgumentOutOfRangeException (nameof (d), d, $"The date is outside the range of DateTime: {d.SecondsSinceReferenceDate}");
			}

			// DateTime doesn't support BC dates (AD dates have Era = 1)
			if (calComponents.Era != 1)
				throw new ArgumentOutOfRangeException (nameof (d), d, "The date is outside the range of DateTime.");

			// NSCalendar gives us the number of nanoseconds corresponding
			// with the fractional second. DateTime's constructor wants
			// milliseconds and microseconds separately, where microseconds is
			// the fractional number of milliseconds. That doesn't count for
			// any remaining ticks, so add that at the end manually. This
			// means we need to do some math here, to split the sub-second
			// number of nanoseconds into milliseconds, microseconds and
			// ticks.
			var nanosecondsLeft = calComponents.Nanosecond;
			var milliseconds = nanosecondsLeft / NANOSECS_PER_MILLISEC;
			nanosecondsLeft -= milliseconds * NANOSECS_PER_MILLISEC;
			var microseconds = nanosecondsLeft / NANOSECS_PER_MICROSEC;
			nanosecondsLeft -= microseconds * NANOSECS_PER_MICROSEC;
			var ticks = nanosecondsLeft / NANOSECS_PER_TICKS;

			var rv = new DateTime (
				(int) calComponents.Year,
				(int) calComponents.Month,
				(int) calComponents.Day,
				(int) calComponents.Hour,
				(int) calComponents.Minute,
				(int) calComponents.Second,
				(int) milliseconds,
#if NET
				(int) microseconds,
#endif
				DateTimeKind.Utc);

#if NET
			if (ticks > 0)
				rv = rv.AddTicks (ticks);
#else
			if (microseconds > 0 || ticks > 0)
				rv = rv.AddTicks (ticks + 10 * (int) microseconds);
#endif


			return rv;
		}

		// now explicit since data can be lost for DateTimeKind.Unspecified
		public static explicit operator NSDate (DateTime dt)
		{
			if (dt.Kind == DateTimeKind.Unspecified)
				throw new ArgumentException ("DateTimeKind.Unspecified cannot be safely converted");

			var dtUnv = dt.ToUniversalTime ();

			// Compute the sub-second fraction of nanoseconds.
			var subsecondTicks = dtUnv.Ticks % TimeSpan.TicksPerSecond;
			var nanoseconds = subsecondTicks * NANOSECS_PER_TICKS;
			using var dateComponents = new NSDateComponents ();
			dateComponents.Day = dtUnv.Day;
			dateComponents.Month = dtUnv.Month;
			dateComponents.Year = dtUnv.Year;
			dateComponents.Hour = dtUnv.Hour;
			dateComponents.Minute = dtUnv.Minute;
			dateComponents.Second = dtUnv.Second;
			dateComponents.Nanosecond = (int) nanoseconds;

			var retDate = calendar.DateFromComponents (dateComponents);
			if (retDate is null)
				throw new ArgumentOutOfRangeException (nameof (dt), dt, $"The date is outside the range of NSDate");

			return retDate;
		}
	}
}
