//
// NativeObject.cs: A base class for objects that have retain/release semantics plus
// a native handle.

// Authors:
//   Alex Soto
//   Miguel de Icaza
//
// Copyrigh 2018 Microsoft Inc
//
using System;
using System.Runtime.InteropServices;
using ObjCRuntime;
using Foundation;
using CoreFoundation;

namespace CoreFoundation {
	//
	// The NativeObject class is intended to be a base class for many CoreFoundation
	// data types whose lifecycle is managed with retain and release operations, but
	// is not limited to CoreFoundation types.
	//
	// It provides the common boilerplate for this kind of objects and the Dispose
	// pattern.
	//
	// Overriding the Retain and Release methods allow for this
	// base class to be reused for other patterns that use other retain/release
	// systems.
	//
	public abstract class NativeObject : INativeObject, IDisposable {
		internal IntPtr handle;
		public IntPtr Handle {
			get => handle; 
			protected set => InitializeHandle (value); 
		}

		protected NativeObject () {
		}
		
		protected NativeObject (IntPtr handle, bool owns)
		{
			Handle = handle;
			if (!owns)
				Retain ();
		}
	
		~NativeObject ()
		{
			Dispose (false);
		}
	
		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}
	
		protected virtual void Dispose (bool disposing)
		{
			if (Handle != IntPtr.Zero) {
				Release ();
				handle = IntPtr.Zero;
			}
		}
	
		protected virtual void Retain () => CFObject.CFRetain (Handle);
		protected virtual void Release () => CFObject.CFRelease (Handle);
	
		protected virtual void InitializeHandle (IntPtr handle)
		{
#if !COREBUILD
			if (Handle == IntPtr.Zero && Class.ThrowOnInitFailure) {
				throw new Exception ($"Could not initialize an instance of the type '{GetType ().FullName}': handle is null.\n" +
				    "It is possible to ignore this condition by setting ObjCRuntime.Class.ThrowOnInitFailure to false.");
			}
#endif
			this.handle = handle;
		}
	}
}
