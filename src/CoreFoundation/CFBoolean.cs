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

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;

namespace CoreFoundation {

	// CFNumber.h
	partial class CFBoolean : INativeObject, IDisposable {
		IntPtr handle;

		[Preserve (Conditional = true)]
		internal CFBoolean (IntPtr handle, bool owns)
		{
			this.handle = handle;
			if (!owns)
				CFObject.CFRetain (handle);
		}

		~CFBoolean ()
		{
			Dispose (false);
		}

		public IntPtr Handle {
			get {
				return handle;
			}
		}

		[DllImport (Constants.CoreFoundationLibrary, EntryPoint="CFBooleanGetTypeID")]
		public extern static /* CFTypeID */ nint GetTypeID ();

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

#if XAMCORE_2_0
		protected virtual void Dispose (bool disposing)
#else
		public virtual void Dispose (bool disposing)
#endif
		{
			if (handle != IntPtr.Zero){
				CFObject.CFRelease (handle);
				handle = IntPtr.Zero;
			}
		}

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
		[return: MarshalAs (UnmanagedType.I1)]
		extern static /* Boolean */ bool CFBooleanGetValue (/* CFBooleanRef */ IntPtr boolean);

		public bool Value {
			get {return CFBooleanGetValue (handle);}
		}

		public static bool GetValue (IntPtr boolean)
		{
			return CFBooleanGetValue (boolean);
		}
	}
}
