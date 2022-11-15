//
// NSSearchPath.cs
//
// Authors:
//   Marek Safar (marek.safar@gmail.com)
//
// Copyright 2013 Xamarin Inc.
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
using System.Runtime.Versioning;

namespace Foundation {
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public static class NSSearchPath {
		public static string [] GetDirectories (NSSearchPathDirectory directory, NSSearchPathDomain domainMask, bool expandTilde = true)
		{
			return CFArray.StringArrayFromHandle (NSSearchPathForDirectoriesInDomains ((nuint) (ulong) directory, (nuint) (ulong) domainMask, expandTilde));
		}

		[DllImport (Constants.FoundationLibrary)]
		static extern IntPtr NSSearchPathForDirectoriesInDomains (nuint directory, nuint domainMask, [MarshalAs (UnmanagedType.I1)] bool expandTilde);
	}
}
