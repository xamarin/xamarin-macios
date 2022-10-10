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

namespace Foundation  {


#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif

	public struct NSOperatingSystemVersion: IEquatable<NSOperatingSystemVersion>, IComparable{
		public nint Major, Minor, PatchVersion;
		
        public NSOperatingSystemVersion (nint majorVersion, nint minorVersion, nint patchVersionArg)
		{
			Major = majorVersion;
			Minor = minorVersion;
			PatchVersion = patchVersionArg;
		}

        public NSOperatingSystemVersion (nint majorVersion, nint minorVersion)
		{
			Major = majorVersion;
			Minor = minorVersion;
			PatchVersion = 0;
		}

        public NSOperatingSystemVersion (nint majorVersion)
		{
			Major = majorVersion;
			Minor = 0;
			PatchVersion = 0;
		}

        public int CompareTo (NSOperatingSystemVersion otherVersion) {
            int majorValue = Major.CompareTo(otherVersion.Major);
            if (majorValue == 0) {
                int minorValue = Minor.CompareTo(otherVersion.Minor);
                if (minorValue == 0) {
                    int patchValue = PatchVersion.CompareTo(otherVersion.PatchVersion);
                    return patchValue;
                }
                else {
                    return minorValue;
                } 
            }
            else {
                return majorValue;
            }
                
        }

        public int CompareTo(Object obj) {
            if (obj == null) return 1;
            if (obj is NSOperatingSystemVersion other)
                return CompareTo(other);
            return 1;
        }

		public override string ToString ()
		{
			return Major + "." + Minor + "." + PatchVersion;
		}

		public bool Equals (NSOperatingSystemVersion other) 
        {
            if (other == null)
                return false;
            
            if (Major == other.Major && 
                Minor == other.Minor &&
                PatchVersion == other.PatchVersion) 
                return true;
            else
                return false;
        }

		public override bool Equals (Object obj)
        {
            if (obj == null)
                return false;
            if (obj is NSOperatingSystemVersion versionObj)
                return Equals(versionObj);
            else   
                return false; 
        }

        public override int GetHashCode()
        {
            return Tuple.Create(Major, Minor, PatchVersion).GetHashCode();
        }

        public static bool operator == (NSOperatingSystemVersion os1, NSOperatingSystemVersion os2)
        {
            if (((object)os1) == null || ((object)os2) == null)
                return Object.Equals(os1, os2);
            return os1.Equals(os2);
        }

        public static bool operator != (NSOperatingSystemVersion os1, NSOperatingSystemVersion os2)
        {
            if (((object)os1) == null || ((object)os2) == null)
                return ! Object.Equals(os1, os2);
            return ! (os1.Equals(os2));
        }

	}
}
