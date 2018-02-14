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

using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;

namespace CoreFoundation {

	class CFDictionary : INativeObject, IDisposable {
		public IntPtr Handle { get; private set; }
	
		public static IntPtr KeyCallbacks;
		public static IntPtr ValueCallbacks;

		public CFDictionary (IntPtr handle)
			: this (handle, false)
		{
		}

		public CFDictionary (IntPtr handle, bool owns)
		{
			if (!owns)
				CFObject.CFRetain (handle);
			this.Handle = handle;
		}
		
		[DllImport (Constants.CoreFoundationLibrary, EntryPoint="CFDictionaryGetTypeID")]
		public extern static nint GetTypeID ();

		~CFDictionary ()
		{
			Dispose (false);
		}

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
			if (Handle != IntPtr.Zero){
				CFObject.CFRelease (Handle);
				Handle = IntPtr.Zero;
			}
		}

		static CFDictionary ()
		{
			var lib = Dlfcn.dlopen (Constants.CoreFoundationLibrary, 0);
			try {
				KeyCallbacks = Dlfcn.GetIndirect (lib, "kCFTypeDictionaryKeyCallBacks");
				ValueCallbacks = Dlfcn.GetIndirect (lib, "kCFTypeDictionaryValueCallBacks");
			} finally {
				Dlfcn.dlclose (lib);
			}
		}
		
		public static CFDictionary FromObjectAndKey (INativeObject obj, INativeObject key)
		{
			return new CFDictionary (CFDictionaryCreate (IntPtr.Zero, new IntPtr[] { key.Handle }, new IntPtr [] { obj.Handle }, 1, KeyCallbacks, ValueCallbacks), true);
		}
		
		public static CFDictionary FromObjectsAndKeys (INativeObject[] objects, INativeObject[] keys)
		{
			if (objects == null)
				throw new ArgumentNullException ("objects");

			if (keys == null)
				throw new ArgumentNullException ("keys");

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
		extern static IntPtr CFDictionaryCreate (IntPtr allocator, IntPtr[] keys, IntPtr[] vals, nint len, IntPtr keyCallbacks, IntPtr valCallbacks);
		
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
		extern static void CFDictionaryGetKeysAndValues (IntPtr theDict, IntPtr[] keys, IntPtr[] values);
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
		
		public string GetStringValue (string key)
		{
			using (var str = new CFString (key)) {
				return CFString.FetchString (CFDictionaryGetValue (Handle, str.handle));
			}
		}

		public int GetInt32Value (string key)
		{
			int value = 0;
			using (var str = new CFString (key)) {
				if (!CFNumberGetValue (CFDictionaryGetValue (Handle, str.Handle), /* kCFNumberSInt32Type */ 3, out value))
					throw new System.Collections.Generic.KeyNotFoundException (string.Format ("Key {0} not found", key));
				return value;
			}
		}

		public long GetInt64Value (string key)
		{
			long value = 0;
			using (var str = new CFString (key)) {
				if (!CFNumberGetValue (CFDictionaryGetValue (Handle, str.Handle), /* kCFNumberSInt64Type */ 4, out value))
					throw new System.Collections.Generic.KeyNotFoundException (string.Format ("Key {0} not found", key));
				return value;
			}
		}

		public IntPtr GetIntPtrValue (string key)
		{
			using (var str = new CFString (key)) {
				return CFDictionaryGetValue (Handle, str.handle);
			}
		}

		public CFDictionary GetDictionaryValue (string key)
		{
			using (var str = new CFString (key)) {
				var ptr = CFDictionaryGetValue (Handle, str.handle);
				return ptr == IntPtr.Zero ? null : new CFDictionary (ptr);
			}
		}

		public bool ContainsKey (string key)
		{
			using (var str = new CFString (key)) {
				return CFDictionaryContainsKey (Handle, str.handle);
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		internal static extern bool CFNumberGetValue (IntPtr number, nint theType, out int value);

		[DllImport (Constants.CoreFoundationLibrary)]
		internal static extern bool CFNumberGetValue (IntPtr number, nint theType, out long value);

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static bool CFDictionaryContainsKey (IntPtr theDict, IntPtr key);
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
			SetValue (theDict, key, value ? CFBoolean.True.Handle : CFBoolean.False.Handle);
		}
	}
}
