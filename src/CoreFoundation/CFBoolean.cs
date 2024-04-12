//
// CFBoolean.cs: Contains base types
//
// Authors:
//    Jonathan Pryor (jpryor@novell.com)
//
// Copyright 2011-2014 Xamarin Inc. All rights reserved.
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
using System.Runtime.Versioning;

using ObjCRuntime;
using Foundation;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreFoundation {


#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	// CFNumber.h
	partial class CFBoolean : NativeObject {
		[Preserve (Conditional = true)]
		internal CFBoolean (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		[DllImport (Constants.CoreFoundationLibrary, EntryPoint = "CFBooleanGetTypeID")]
		public extern static /* CFTypeID */ nint GetTypeID ();

		public static implicit operator bool (CFBoolean value)
		{
			return value.Value;
		}

		public static explicit operator CFBoolean (bool value)
		{
			return FromBoolean (value);
		}

		internal static IntPtr ToHandle (bool value)
		{
			return value ? TrueHandle : FalseHandle;
		}

		public static CFBoolean FromBoolean (bool value)
		{
			return new CFBoolean (value ? TrueHandle : FalseHandle, false);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* Boolean */ byte CFBooleanGetValue (/* CFBooleanRef */ IntPtr boolean);

		public bool Value {
			get { return CFBooleanGetValue (Handle) != 0; }
		}

		public static bool GetValue (IntPtr boolean)
		{
			return CFBooleanGetValue (boolean) != 0;
		}
	}
}
