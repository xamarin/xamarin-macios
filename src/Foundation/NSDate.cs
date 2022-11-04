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
		const double NANOSECS_PER_MILLISEC = 1000000.0;

		static readonly NSCalendar calendar = new NSCalendar (NSCalendarType.Gregorian) { TimeZone = NSTimeZone.FromName ("UTC") };

		// now explicit since data can be lost for small/large values of DateTime
		public static explicit operator DateTime (NSDate d)
		{
			double secs = d.SecondsSinceReferenceDate;
			if ((secs < -63114076800) || (secs > 252423993599.9994)) // we round to the nearest .001 in the conversion from ns to ms
				throw new ArgumentOutOfRangeException (nameof (d), d, $"{nameof (d)} is outside the range of NSDate {secs} seconds");

			// For 64 bit, convert to components representation since we cannot rely on secondsSinceReferenceDate
			var units = NSCalendarUnit.Year | NSCalendarUnit.Month | NSCalendarUnit.Day | NSCalendarUnit.Hour |
				NSCalendarUnit.Minute | NSCalendarUnit.Second | NSCalendarUnit.Nanosecond | NSCalendarUnit.Calendar;
			using var calComponents = calendar.Components (units, d);
			return new DateTime (
				(int) calComponents.Year,
				(int) calComponents.Month,
				(int) calComponents.Day,
				(int) calComponents.Hour,
				(int) calComponents.Minute,
				(int) calComponents.Second,
				Convert.ToInt32 (calComponents.Nanosecond / NANOSECS_PER_MILLISEC),
				DateTimeKind.Utc);
		}

		// now explicit since data can be lost for DateTimeKind.Unspecified
		public static explicit operator NSDate (DateTime dt)
		{
			if (dt.Kind == DateTimeKind.Unspecified)
				throw new ArgumentException ("DateTimeKind.Unspecified cannot be safely converted");

			var dtUnv = dt.ToUniversalTime ();

			// Convert to components representation since we cannot rely on secondsSinceReferenceDate
			using var dateComponents = new NSDateComponents ();
			dateComponents.Day = dtUnv.Day;
			dateComponents.Month = dtUnv.Month;
			dateComponents.Year = dtUnv.Year;
			dateComponents.Hour = dtUnv.Hour;
			dateComponents.Minute = dtUnv.Minute;
			dateComponents.Second = dtUnv.Second;
			dateComponents.Nanosecond = (int) (dtUnv.Millisecond * NANOSECS_PER_MILLISEC);

			var retDate = calendar.DateFromComponents (dateComponents);
			if (retDate is null)
				throw new ArgumentOutOfRangeException (nameof (dt), dt, $"{nameof (dt)} is outside the range of NSDate");

			return retDate;
		}
	}
}
