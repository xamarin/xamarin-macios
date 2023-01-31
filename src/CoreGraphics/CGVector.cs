// 
// CGVector.cs: Implements the managed CGPDFDocument
//
// Authors:  
//     Miguel de Icaza
//
// Copyright 2013-2014 Xamarin Inc
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

#nullable enable

using System;
using System.Globalization;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;
using CoreFoundation;

namespace CoreGraphics {


#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	// CGGeometry.h
	public struct CGVector {
		public /* CGFloat */ nfloat dx, dy;

		public CGVector (nfloat dx, nfloat dy)
		{
			this.dx = dx;
			this.dy = dy;
		}

		public static bool operator == (CGVector left, CGVector right)
		{
			return left.dx == right.dx && left.dy == right.dy;
		}

		public static bool operator != (CGVector left, CGVector right)
		{
			return left.dx != right.dx || left.dy != right.dy;
		}

		public override int GetHashCode ()
		{
#if NET
			return HashCode.Combine (dx, dy);
#else
			unchecked {
				return ((int) dx) ^ ((int) dy);
			}
#endif
		}

		public override bool Equals (object? other)
		{
			if (other is CGVector vector)
				return dx == vector.dx && dy == vector.dy;
			return false;
		}

#if MONOTOUCH
#if !COREBUILD
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
#endif
		[DllImport (Constants.UIKitLibrary)]
		extern static IntPtr NSStringFromCGVector (CGVector vector);
		
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
#endif
		public override string? ToString ()
		{
			return CFString.FromHandle (NSStringFromCGVector (this));
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
#endif
		[DllImport (Constants.UIKitLibrary)]
		extern static CGVector CGVectorFromString (IntPtr str);
		
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
#endif
		static public CGVector FromString (string s)
		{
			// note: null is allowed
			var ptr = CFString.CreateNative (s);
			var value = CGVectorFromString (ptr);
			CFString.ReleaseNative (ptr);
			return value;
		}
#endif
#else // MONOMAC
		public override string ToString ()
		{
			return $"{{{dx}, {dy}}}";
		}
#endif

	}
}
