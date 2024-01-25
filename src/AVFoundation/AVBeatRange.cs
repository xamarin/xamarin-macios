// Copyright 2009, Novell, Inc.
// Copyright 2010, Novell, Inc.
// Copyright 2011, 2012, 2014-2015 Xamarin Inc.
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
using System.Runtime.InteropServices;

using Foundation;

using ObjCRuntime;

#nullable enable

namespace AVFoundation {

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct AVBeatRange {
		public double Start;

		public double Length;

		public AVBeatRange (double startBeat, double lengthInBeats)
		{
			Start = startBeat;
			Length = lengthInBeats;
		}

		public override string ToString ()
		{
			return $"(Start={Start},Length={Length})";
		}

		public static bool operator == (AVBeatRange left, AVBeatRange right)
		{
			return left.Equals (right);
		}

		public static bool operator != (AVBeatRange left, AVBeatRange right)
		{
			return !left.Equals (right);
		}

		public override bool Equals (object? obj)
		{
			if (!(obj is AVBeatRange))
				return false;

			return this.Equals ((AVBeatRange) obj);
		}

		public bool Equals (AVBeatRange other)
		{
			return Start == other.Start && Length == other.Length;
		}

		public override int GetHashCode ()
		{
			return HashCode.Combine (Start, Length);
		}
	}
}
