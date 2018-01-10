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

using ObjCRuntime;

namespace Foundation {

	public partial class NSDate {
		const long NSDATE_TICKS = 631139040000000000;

#if XAMCORE_2_0
		// now explicit since data can be lost for small/large values of DateTime
		public static explicit operator DateTime (NSDate d)
		{
			double secs = d.SecondsSinceReferenceDate;
			if ((secs < -63113904000) || (secs > 252423993599))
				throw new ArgumentOutOfRangeException ("Value is outside the range of NSDate");

			return new DateTime ((long)(secs * TimeSpan.TicksPerSecond + NSDATE_TICKS), DateTimeKind.Utc);
		}

		// now explicit since data can be lost for DateTimeKind.Unspecified
		public static explicit operator NSDate (DateTime dt)
		{
			if (dt.Kind == DateTimeKind.Unspecified)
				throw new ArgumentException ("DateTimeKind.Unspecified cannot be safely converted");

			return FromTimeIntervalSinceReferenceDate ((dt.ToUniversalTime ().Ticks - NSDATE_TICKS) / (double) TimeSpan.TicksPerSecond);
		}
#else
		public static implicit operator DateTime (NSDate d)
		{
			double secs = d.SecondsSinceReferenceDate;

			if (secs < -63113904000)
				return DateTime.MinValue;

			if (secs > 252423993599)
				return DateTime.MaxValue;

			return new DateTime ((long)(secs * TimeSpan.TicksPerSecond + NSDATE_TICKS), DateTimeKind.Utc);
		}

		public static implicit operator NSDate (DateTime dt)
		{
			return FromTimeIntervalSinceReferenceDate ((dt.ToUniversalTime ().Ticks - NSDATE_TICKS) / (double) TimeSpan.TicksPerSecond);
		}

		public override string ToString ()
		{
			return Description;
		}
#endif
	}
}
