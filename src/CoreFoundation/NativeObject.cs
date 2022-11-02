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
using System.Runtime.Versioning;
using ObjCRuntime;
using Foundation;

#nullable enable

#if !NET
using NativeHandle = System.IntPtr;
#endif

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
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public abstract class NativeObject : DisposableObject {
		protected NativeObject ()
		{
		}

		protected NativeObject (NativeHandle handle, bool owns)
			: this (handle, owns, true)
		{
		}

		protected NativeObject (NativeHandle handle, bool owns, bool verify)
			: base (handle, owns, verify)
		{
			if (!owns && handle != NativeHandle.Zero)
				Retain ();
		}

		protected override void Dispose (bool disposing)
		{
			if (Handle != NativeHandle.Zero)
				Release ();
			base.Dispose (disposing);
		}

		// <quote>If cf is NULL, this will cause a runtime error and your application will crash.</quote>
		// https://developer.apple.com/documentation/corefoundation/1521269-cfretain?language=occ
		protected internal virtual void Retain () => CFObject.CFRetain (GetCheckedHandle ());

		// <quote>If cf is NULL, this will cause a runtime error and your application will crash.</quote>
		// https://developer.apple.com/documentation/corefoundation/1521153-cfrelease
		protected internal virtual void Release () => CFObject.CFRelease (GetCheckedHandle ());
	}
}
