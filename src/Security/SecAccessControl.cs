// 
// SecAccessControl.cs: Implements the managed SecAccessControl representation
//
// Authors: 
//  Miguel de Icaza  <miguel@xamarin.com>
//
// Copyright 2014, 2015 Xamarin Inc.
//
// Notice: to avoid having to track the object and then having to remove it
// this class exists merely to set the desired flags, and the sole consumer
// of this API, creates the object on demands and passes ownership when
// calling SecAddItem.
//

#nullable enable

using System;
using System.Runtime.InteropServices;
using ObjCRuntime;
using CoreFoundation;
using Foundation;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Security {

	[Flags]
	[Native]
#if NET
	// changed to CFOptionFlags in Xcode 8 SDK
	public enum SecAccessControlCreateFlags : ulong {
#else
	// CFOptionFlags -> SecAccessControl.h
	public enum SecAccessControlCreateFlags : long {
#endif
		UserPresence = 1 << 0,

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		[Advice ("'BiometryAny' is preferred over 'TouchIDAny' since Xcode 9.3. Touch ID and Face ID together are biometric authentication mechanisms.")]
		TouchIDAny = BiometryAny,

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		[Advice ("'BiometryCurrentSet' is preferred over 'TouchIDCurrentSet' since Xcode 9.3. Touch ID and Face ID together are biometric authentication mechanisms.")]
		TouchIDCurrentSet = BiometryCurrentSet,

		// Added in iOS 11.3 and macOS 10.13.4 but keeping initial availability attribute because it's using the value
		// of 'TouchIDAny' which iOS 9 / macOS 10.12.1 will accept.
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		BiometryAny = 1 << 1,

		// Added in iOS 11.3 and macOS 10.13.4 but keeping initial availability attribute because it's using the value
		// of 'TouchIDCurrentSet' which iOS 9 / macOS 10.12.1 will accept.
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		BiometryCurrentSet = 1 << 3,

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		DevicePasscode = 1 << 4,

#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[NoiOS]
		[NoTV]
		[NoWatch]
#endif
		Watch = 1 << 5,

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		Or = 1 << 14,

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		And = 1 << 15,

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		PrivateKeyUsage = 1 << 30,

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
#if NET
		ApplicationPassword = 1UL << 31,
#else
		ApplicationPassword = 1 << 31,
#endif
	}

#if NET
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos")]
#endif
	public partial class SecAccessControl : NativeObject {
#if !COREBUILD
		[Preserve (Conditional = true)]
		internal SecAccessControl (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		public SecAccessControl (SecAccessible accessible, SecAccessControlCreateFlags flags = SecAccessControlCreateFlags.UserPresence)
			: base (SecAccessControlCreateWithFlags (IntPtr.Zero, KeysAccessible.FromSecAccessible (accessible), (nint) (long) flags, out var _), true)
		{
			Accessible = accessible;
			Flags = flags;
		}

		public SecAccessible Accessible { get; private set; }
		public SecAccessControlCreateFlags Flags { get; private set; }

		[DllImport (Constants.SecurityLibrary)]
		extern static IntPtr SecAccessControlCreateWithFlags (IntPtr allocator, /* CFTypeRef */ IntPtr protection, /* SecAccessControlCreateFlags */ nint flags, out IntPtr error);
#endif
	}
}
