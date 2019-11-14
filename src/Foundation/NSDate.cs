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

namespace Foundation {

	public partial class NSDate {
		const double NANOSECS_PER_MILLISEC = 1000000.0;

		private static readonly NSCalendar calendar;
		private static readonly ThreadLocal<NSDateComponents> threadComponents = new ThreadLocal<NSDateComponents>(() => new NSDateComponents ());

		static NSDate ()
		{
			calendar = new NSCalendar (NSCalendarType.Gregorian);
			calendar.TimeZone = NSTimeZone.FromName ("UTC");
		}

#if XAMCORE_2_0
		// now explicit since data can be lost for small/large values of DateTime
		public static explicit operator DateTime (NSDate d)
		{
			double secs = d.SecondsSinceReferenceDate;

			if ((secs < -63114076800) || (secs > 252423993599.9994)) // we round to the nearest .001 in the conversion from ns to ms
				throw new ArgumentOutOfRangeException (nameof (d), d, $"{nameof (d)} is outside the range of NSDate {secs} seconds");

			using (NSDateComponents calComponents = calendar.Components (NSCalendarUnit.Year | NSCalendarUnit.Month | NSCalendarUnit.Day | NSCalendarUnit.Hour |
				NSCalendarUnit.Minute | NSCalendarUnit.Second | NSCalendarUnit.Nanosecond | NSCalendarUnit.Calendar, d))
			{
				var retDate = new DateTime ((int) calComponents.Year, (int) calComponents.Month, (int) calComponents.Day, (int) calComponents.Hour,
				(int) calComponents.Minute, (int) (calComponents.Second), Convert.ToInt32 (calComponents.Nanosecond / NANOSECS_PER_MILLISEC), DateTimeKind.Utc);

				return retDate;
			}
		}

		// now explicit since data can be lost for DateTimeKind.Unspecified
		public static explicit operator NSDate (DateTime dt)
		{
			if (dt.Kind == DateTimeKind.Unspecified)
				throw new ArgumentException ("DateTimeKind.Unspecified cannot be safely converted");

			var dtUnv = dt.ToUniversalTime ();

			threadComponents.Value.Day = dtUnv.Day;
			threadComponents.Value.Month = dtUnv.Month;
			threadComponents.Value.Year = dtUnv.Year;
			threadComponents.Value.Hour = dtUnv.Hour;
			threadComponents.Value.Minute = dtUnv.Minute;
			threadComponents.Value.Second = dtUnv.Second;
			threadComponents.Value.Nanosecond = (int) (dtUnv.Millisecond * NANOSECS_PER_MILLISEC);

			var retDate = calendar.DateFromComponents (threadComponents.Value);
			if (retDate == null)
				throw new ArgumentOutOfRangeException (nameof (dt), dt, $"{nameof (dt)} is outside the range of NSDate");

			return retDate;
		}
#endif
	}
}
