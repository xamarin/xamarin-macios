// Copyright 2013 Xamarin Inc. All rights reserved

using System;
using System.Runtime.InteropServices;
using ObjCRuntime;

namespace Foundation {

	// Helper to (mostly) support NS[Mutable]Copying protocols
	public class NSZone : INativeObject {
		[DllImport (Constants.FoundationLibrary)]
		static extern /* NSZone* */ IntPtr NSDefaultMallocZone ();

		[DllImport (Constants.FoundationLibrary)]
		static extern IntPtr /* NSString* */ NSZoneName (/* NSZone* */ IntPtr zone);

		[DllImport (Constants.FoundationLibrary)]
		static extern void NSSetZoneName (/* NSZone* */ IntPtr zone, /* NSString* */ IntPtr name);

		internal NSZone ()
		{
		}

		public NSZone (IntPtr handle)
		{
			this.Handle = handle;
		}

		[Preserve (Conditional = true)]
		public NSZone (IntPtr handle, bool owns)
			: this (handle)
		{
			// NSZone is just an opaque pointer without reference counting, so we ignore the 'owns' parameter.
		}

		public IntPtr Handle { get; private set; }

#if !COREBUILD
		public string Name {
			get {
				return new NSString (NSZoneName (Handle)).ToString ();
			}
			set {
				using (var ns = new NSString (value))
					NSSetZoneName (Handle, ns.Handle);
			}
		}

		// note: Copy(NSZone) and MutableCopy(NSZone) with a nil pointer == default
		public static readonly NSZone Default = new NSZone (NSDefaultMallocZone ());
#endif
	}
}