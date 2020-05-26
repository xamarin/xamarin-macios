//
// NativeObject.cs: A base class for objects that have retain/release semantics plus
// a native handle.

// Authors:
//   Alex Soto
//   Miguel de Icaza
//
// Copyright 2018, 2020 Microsoft Corp
//
using System;
using System.Runtime.InteropServices;
using ObjCRuntime;
using Foundation;

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
		IntPtr handle;
		public IntPtr Handle {
			get => handle;
			protected set => InitializeHandle (value);
		}

		protected NativeObject ()
		{
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

		// <quote>If cf is NULL, this will cause a runtime error and your application will crash.</quote>
		// https://developer.apple.com/documentation/corefoundation/1521269-cfretain?language=occ
		protected virtual void Retain () => CFObject.CFRetain (GetCheckedHandle ());

		// <quote>If cf is NULL, this will cause a runtime error and your application will crash.</quote>
		// https://developer.apple.com/documentation/corefoundation/1521153-cfrelease
		protected virtual void Release () => CFObject.CFRelease (GetCheckedHandle ());

		protected virtual void InitializeHandle (IntPtr handle)
		{
#if !COREBUILD
			if (handle == IntPtr.Zero && Class.ThrowOnInitFailure) {
				throw new Exception ($"Could not initialize an instance of the type '{GetType ().FullName}': handle is null.\n" +
				    "It is possible to ignore this condition by setting ObjCRuntime.Class.ThrowOnInitFailure to false.");
			}
#endif
			this.handle = handle;
		}

		public IntPtr GetCheckedHandle ()
		{
			if (handle == IntPtr.Zero)
				ObjCRuntime.ThrowHelper.ThrowObjectDisposedException (this);
			return handle;
		}
	}
}
