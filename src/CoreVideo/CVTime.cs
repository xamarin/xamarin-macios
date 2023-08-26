// 
// CVTime.cs
//
// Authors: Mono Team
//     
// Copyright 2011 Novell, Inc
// Copyright 2011-2014 Xamarin Inc
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
using CoreFoundation;
using ObjCRuntime;

#nullable enable

namespace CoreVideo {

	// CVBase.h
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#else
	[Watch (4, 0)]
#endif
	public struct CVTime {

		public /* int64_t */ long TimeValue;
		public /* int64_t */ long TimeScale;
		public /* int32_t */ CVTimeFlags TimeFlags;

		public int Flags { get { return (int) TimeFlags; } set { TimeFlags = (CVTimeFlags) value; } }

#if !COREBUILD
		public static CVTime ZeroTime {
			get {
				return Marshal.PtrToStructure<CVTime> (Dlfcn.GetIndirect (Libraries.CoreVideo.Handle, "kCVZeroTime"))!;
			}
		}

		public static CVTime IndefiniteTime {
			get {
				return Marshal.PtrToStructure<CVTime> (Dlfcn.GetIndirect (Libraries.CoreVideo.Handle, "kCVIndefiniteTime"))!;
			}
		}
#endif

		public override bool Equals (object? other)
		{
			if (!(other is CVTime))
				return false;

			CVTime b = (CVTime) other;

			return (TimeValue == b.TimeValue) && (TimeScale == b.TimeScale) && (TimeFlags == b.TimeFlags);
		}

		public override int GetHashCode ()
		{
			return HashCode.Combine (TimeValue, TimeScale, Flags);
		}

		// CVHostTime.h

		[DllImport (Constants.CoreVideoLibrary, EntryPoint = "CVGetCurrentHostTime")]
		public static extern /* uint64_t */ ulong GetCurrentHostTime ();


		[DllImport (Constants.CoreVideoLibrary, EntryPoint = "CVGetHostClockFrequency")]
		public static extern /* double */ double GetHostClockFrequency ();

		[DllImport (Constants.CoreVideoLibrary, EntryPoint = "CVGetHostClockMinimumTimeDelta")]
		public static extern /* uint32_t */ uint GetHostClockMinimumTimeDelta ();
	}
}
