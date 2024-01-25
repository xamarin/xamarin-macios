//
// NSOperatingSystemVersion.cs
//
// Copyright 2009-2010, Novell, Inc.
// Copyright 2011, 2012 Xamarin Inc
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
using System.Runtime.Versioning;

using ObjCRuntime;
#nullable enable

namespace Foundation {

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif

	public struct NSOperatingSystemVersion : IEquatable<NSOperatingSystemVersion>, IComparable {
		public nint Major, Minor, PatchVersion;

		public NSOperatingSystemVersion (nint major, nint minor, nint patchVersion)
		{
			Major = major;
			Minor = minor;
			PatchVersion = patchVersion;
		}

		public NSOperatingSystemVersion (nint major, nint minor) : this (major, minor, 0) { }

		public NSOperatingSystemVersion (nint major) : this (major, 0, 0) { }

		public int CompareTo (NSOperatingSystemVersion otherVersion)
		{
			var majorValue = Major.CompareTo (otherVersion.Major);
			if (majorValue == 0) {
				var minorValue = Minor.CompareTo (otherVersion.Minor);
				return (minorValue == 0) ? PatchVersion.CompareTo (otherVersion.PatchVersion) : minorValue;
			}
			return majorValue;
		}

		public int CompareTo (Object? obj)
			=> (obj is NSOperatingSystemVersion other) ? CompareTo (other) : 1;

		public override string ToString ()
			=> $"{Major}.{Minor}.{PatchVersion}";

		public bool Equals (NSOperatingSystemVersion other)
			=> Major == other.Major && Minor == other.Minor && PatchVersion == other.PatchVersion;

		public override bool Equals (Object? obj)
		{
			if (obj is NSOperatingSystemVersion versionObj)
				return Equals (versionObj);
			return false;
		}

		public override int GetHashCode ()
			=> HashCode.Combine (Major, Minor, PatchVersion);

		public static bool operator == (NSOperatingSystemVersion os1, NSOperatingSystemVersion os2)
			=> os1.Equals (os2);

		public static bool operator != (NSOperatingSystemVersion os1, NSOperatingSystemVersion os2)
			=> !(os1 == os2);

	}
}
