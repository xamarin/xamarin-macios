// Copyright 2013 Xamarin Inc. All rights reserved

#nullable enable

using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using CoreFoundation;

using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Foundation {

	// Helper to (mostly) support NS[Mutable]Copying protocols
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class NSZone : INativeObject {
		[DllImport (Constants.FoundationLibrary)]
		static extern /* NSZone* */ IntPtr NSDefaultMallocZone ();

		[DllImport (Constants.FoundationLibrary)]
		static extern IntPtr /* NSString* */ NSZoneName (/* NSZone* */ IntPtr zone);

		[DllImport (Constants.FoundationLibrary)]
		static extern void NSSetZoneName (/* NSZone* */ IntPtr zone, /* NSString* */ IntPtr name);

#if !NET
		public NSZone (NativeHandle handle)
			: this (handle, false)
		{
		}
#endif

		[Preserve (Conditional = true)]
#if NET
		internal NSZone (NativeHandle handle, bool owns)
#else
		public NSZone (NativeHandle handle, bool owns)
#endif
		{
			// NSZone is just an opaque pointer without reference counting, so we ignore the 'owns' parameter.
			this.Handle = handle;
		}

		public NativeHandle Handle { get; private set; }

#if !COREBUILD
		public string? Name {
			get {
				return CFString.FromHandle (NSZoneName (Handle));
			}
			set {
				var nsHandle = CFString.CreateNative (value);
				try {
					NSSetZoneName (Handle, nsHandle);
				} finally {
					CFString.ReleaseNative (nsHandle);
				}
			}
		}

		// note: Copy(NSZone) and MutableCopy(NSZone) with a nil pointer == default
		public static readonly NSZone Default = new NSZone (NSDefaultMallocZone (), false);
#endif
	}
}
