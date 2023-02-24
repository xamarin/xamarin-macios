//
// Copyright 2019 Microsoft Inc
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
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;
using CoreGraphics;

#if MONOMAC
namespace AppKit {
#else
namespace UIKit {
#endif

#if NET
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct NSDirectionalEdgeInsets {

		// API match for NSDirectionalEdgeInsetsZero field/constant
		[Field ("NSDirectionalEdgeInsetsZero")] // fake (but helps testing and could also help documentation)
		public static readonly NSDirectionalEdgeInsets Zero;

		public nfloat Top, Leading, Bottom, Trailing;

#if !COREBUILD
		public NSDirectionalEdgeInsets (nfloat top, nfloat leading, nfloat bottom, nfloat trailing)
		{
			Top = top;
			Leading = leading;
			Bottom = bottom;
			Trailing = trailing;
		}

		// note: NSDirectionalEdgeInsetsEqualToDirectionalEdgeInsets (UIGeometry.h) is a macro
		public bool Equals (NSDirectionalEdgeInsets other)
		{
			if (Leading != other.Leading)
				return false;
			if (Trailing != other.Trailing)
				return false;
			if (Top != other.Top)
				return false;
			return (Bottom == other.Bottom);
		}

		public override bool Equals (object? obj)
		{
			if (obj is NSDirectionalEdgeInsets insets)
				return Equals (insets);
			return false;
		}

		public static bool operator == (NSDirectionalEdgeInsets insets1, NSDirectionalEdgeInsets insets2)
		{
			return insets1.Equals (insets2);
		}

		public static bool operator != (NSDirectionalEdgeInsets insets1, NSDirectionalEdgeInsets insets2)
		{
			return !insets1.Equals (insets2);
		}

		public override int GetHashCode ()
		{
			return HashCode.Combine (Top, Leading, Trailing, Bottom);
		}

#if !MONOMAC
		[DllImport (Constants.UIKitLibrary)]
		extern static NSDirectionalEdgeInsets NSDirectionalEdgeInsetsFromString (IntPtr /* NSString */ s);

		static public NSDirectionalEdgeInsets FromString (string s)
		{
			// note: null is allowed
			var ptr = NSString.CreateNative (s);
			var value = NSDirectionalEdgeInsetsFromString (ptr);
			NSString.ReleaseNative (ptr);
			return value;
		}
#endif

#if !MONOMAC
		[DllImport (Constants.UIKitLibrary)]
		extern static IntPtr /* NSString */ NSStringFromDirectionalEdgeInsets (NSDirectionalEdgeInsets insets);

		// note: ensure we can roundtrip ToString into FromString
		public override string ToString ()
		{
			using (var ns = new NSString (NSStringFromDirectionalEdgeInsets (this)))
				return ns.ToString ();
		}
#endif
#endif
	}
}
