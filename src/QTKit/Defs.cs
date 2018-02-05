//
// Copyright 2010, Novell, Inc.
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
using System;
using System.Runtime.InteropServices;

using ObjCRuntime;

namespace QTKit {
	
	[Deprecated (PlatformName.MacOSX, 10, 9)]
	[StructLayout (LayoutKind.Sequential)]
	public partial struct QTTime {
		public static readonly QTTime Zero = new QTTime (0, 1, 0);
		public static readonly QTTime IndefiniteTime = new QTTime (0, 1, TimeFlags.TimeIsIndefinite);

#if XAMCORE_2_0
		// QTKit/Headers/QTTime.h:
		// typedef struct {
		//     long long timeValue;
		//     long timeScale;
		//     long flags;
		// } QTTime;
		long timeValue;
		nint timeScale;
		nint flags; // TimeFlags enum

		public long TimeValue {
			get { return timeValue; }
			set { timeValue = value; }
		}

		public nint TimeScale {
			get { return timeScale; }
			set { timeScale = value; }
		}

		public TimeFlags Flags {
			get { return (TimeFlags)(long)flags; }
			set { flags = (nint)(long)value; }
		}
#else
		public long TimeValue;
		public int  TimeScale;
		public TimeFlags Flags;
#endif

		public QTTime (long timeValue, nint timeScale, TimeFlags flags)
		{
#if XAMCORE_2_0
			this.timeValue = timeValue;
			this.timeScale = timeScale;
			this.flags = (nint)(long)flags;
#else
			TimeValue = timeValue;
			TimeScale = timeScale;
			Flags = flags;
#endif
		}

		public QTTime (long timeValue, nint timeScale)
			: this (timeValue, timeScale, 0)
		{
		}

		public override string ToString ()
		{
			if (Flags == 0)
				return String.Format ("[TimeValue={0} scale={1}]", TimeValue, TimeScale);
			else 
				return String.Format ("[TimeValue={0} scale={1} Flags={2}]", TimeValue, TimeScale, Flags);
		}
	}

	[Deprecated (PlatformName.MacOSX, 10, 9)]
	public struct QTTimeRange {
#if XAMCORE_2_0
		// QTKit/Headers/QTTime.h:
		// typedef struct {
		//     QTTime time;
		//     QTTime duration;
		// } QTTimeRange;
		QTTime time;
		QTTime duration;

		public QTTime Time {
			get { return time; }
			set { time = value; }
		}

		public QTTime Duration {
			get { return duration; }
			set { duration = value; }
		}
#else
		public QTTime Time;
		public QTTime Duration;
#endif

		public QTTimeRange (QTTime time, QTTime duration)
		{
#if XAMCORE_2_0
			this.time = time;
			this.duration = duration;
#else
			Time = time;
			Duration = duration;
#endif
		}

		public override string ToString ()
		{
			return String.Format ("[Time={0} Duration={2}]", Time, Duration);
		}
	}

}
