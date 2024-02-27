// 
// CFDictionary.cs: P/Invokes for CFDictionary, CFMutableDictionary
//
// Authors:
//    Mono Team
//    Rolf Bjarne Kvinge (rolf@xamarin.com)
//     
// Copyright 2010 Novell, Inc
// Copyright 2012 Xamarin Inc
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
using Foundation;
using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreFoundation {

	class CFDictionary : NativeObject {
		public static IntPtr KeyCallbacks;
		public static IntPtr ValueCallbacks;

		[Preserve (Conditional = true)]
		internal CFDictionary (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		[DllImport (Constants.CoreFoundationLibrary, EntryPoint = "CFDictionaryGetTypeID")]
		public extern static nint GetTypeID ();

		static CFDictionary ()
		{
			var lib = Libraries.CoreFoundation.Handle;
			KeyCallbacks = Dlfcn.GetIndirect (lib, "kCFTypeDictionaryKeyCallBacks");
			ValueCallbacks = Dlfcn.GetIndirect (lib, "kCFTypeDictionaryValueCallBacks");
		}

		public static CFDictionary FromObjectAndKey (INativeObject obj, INativeObject key)
		{
			return new CFDictionary (CFDictionaryCreate (IntPtr.Zero, new IntPtr [] { key.Handle }, new IntPtr [] { obj.Handle }, 1, KeyCallbacks, ValueCallbacks), true);
		}

		public static CFDictionary FromObjectsAndKeys (INativeObject [] objects, INativeObject [] keys)
		{
			if (objects is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (objects));

			if (keys is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (keys));

			if (objects.Length != keys.Length)
				throw new ArgumentException ("The length of both arrays must be the same");

			IntPtr [] k = new IntPtr [keys.Length];
			IntPtr [] v = new IntPtr [keys.Length];

			for (int i = 0; i < k.Length; i++) {
				k [i] = keys [i].Handle;
				v [i] = objects [i].Handle;
			}

			return new CFDictionary (CFDictionaryCreate (IntPtr.Zero, k, v, k.Length, KeyCallbacks, ValueCallbacks), true);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static IntPtr CFDictionaryCreate (IntPtr allocator, IntPtr [] keys, IntPtr [] vals, nint len, IntPtr keyCallbacks, IntPtr valCallbacks);

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static IntPtr CFDictionaryGetValue (IntPtr theDict, IntPtr key);
		public static IntPtr GetValue (IntPtr theDict, IntPtr key)
		{
			return CFDictionaryGetValue (theDict, key);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static nint CFDictionaryGetCount (IntPtr theDict);
		public nint Count {
			get { return CFDictionaryGetCount (Handle); }
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFDictionaryGetKeysAndValues (IntPtr theDict, IntPtr [] keys, IntPtr [] values);
		public void GetKeysAndValues (out IntPtr [] keys, out IntPtr [] values)
		{
			nint count = this.Count;

			keys = new IntPtr [count];
			values = new IntPtr [count];
			CFDictionaryGetKeysAndValues (Handle, keys, values);
		}

		public static bool GetBooleanValue (IntPtr theDict, IntPtr key)
		{
			var value = GetValue (theDict, key);
			if (value == IntPtr.Zero)
				return false;
			return CFBoolean.GetValue (value);
		}

		public string? GetStringValue (string key)
		{
			var keyHandle = CFString.CreateNative (key);
			try {
				return CFString.FromHandle (CFDictionaryGetValue (Handle, keyHandle));
			} finally {
				CFString.ReleaseNative (keyHandle);
			}
		}

		public int GetInt32Value (string key)
		{
			int value = 0;
			var keyHandle = CFString.CreateNative (key);
			try {
				byte rv;
				unsafe {
					rv = CFNumberGetValue (CFDictionaryGetValue (Handle, keyHandle), /* kCFNumberSInt32Type */ 3, &value);
				}
				if (rv == 0)
					throw new System.Collections.Generic.KeyNotFoundException (string.Format ("Key {0} not found", key));
				return value;
			} finally {
				CFString.ReleaseNative (keyHandle);
			}
		}

		public long GetInt64Value (string key)
		{
			long value = 0;
			var keyHandle = CFString.CreateNative (key);
			try {
				byte rv;
				unsafe {
					rv = CFNumberGetValue (CFDictionaryGetValue (Handle, keyHandle), /* kCFNumberSInt64Type */ 4, &value);
				}
				if (rv == 0)
					throw new System.Collections.Generic.KeyNotFoundException (string.Format ("Key {0} not found", key));
				return value;
			} finally {
				CFString.ReleaseNative (keyHandle);
			}
		}

		public IntPtr GetIntPtrValue (string key)
		{
			var keyHandle = CFString.CreateNative (key);
			try {
				return CFDictionaryGetValue (Handle, keyHandle);
			} finally {
				CFString.ReleaseNative (keyHandle);
			}
		}

		public CFDictionary? GetDictionaryValue (string key)
		{
			var keyHandle = CFString.CreateNative (key);
			try {
				var ptr = CFDictionaryGetValue (Handle, keyHandle);
				return ptr == IntPtr.Zero ? null : new CFDictionary (ptr, false);
			} finally {
				CFString.ReleaseNative (keyHandle);
			}
		}

		public bool ContainsKey (string key)
		{
			var keyHandle = CFString.CreateNative (key);
			try {
				return CFDictionaryContainsKey (Handle, keyHandle) != 0;
			} finally {
				CFString.ReleaseNative (keyHandle);
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		unsafe internal static extern byte CFNumberGetValue (IntPtr number, nint theType, int* value);

		[DllImport (Constants.CoreFoundationLibrary)]
		unsafe internal static extern byte CFNumberGetValue (IntPtr number, nint theType, long* value);

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static byte CFDictionaryContainsKey (IntPtr theDict, IntPtr key);
	}

	static class CFMutableDictionary {

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFDictionarySetValue (IntPtr theDict, IntPtr key, IntPtr value);
		public static void SetValue (IntPtr theDict, IntPtr key, IntPtr value)
		{
			CFDictionarySetValue (theDict, key, value);
		}

		public static void SetValue (IntPtr theDict, IntPtr key, bool value)
		{
			SetValue (theDict, key, value ? CFBoolean.TrueHandle : CFBoolean.FalseHandle);
		}
	}
}
