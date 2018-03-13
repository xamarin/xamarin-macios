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

using System;
using System.Runtime.InteropServices;
using XamCore.ObjCRuntime;
using XamCore.CoreFoundation;
using XamCore.Foundation;

namespace XamCore.Security {

	[Flags]
	[Native]
#if XAMCORE_4_0
	// changed to CFOptionFlags in Xcode 8 SDK
	public enum SecAccessControlCreateFlags : nuint {
#else
	// CFOptionFlags -> SecAccessControl.h
	public enum SecAccessControlCreateFlags : nint {
#endif
		UserPresence        = 1 << 0,

		[Advice ("'BiometryAny' is preferred over 'TouchIDAny' since Xcode 9.3. Touch ID and Face ID together are biometric authentication mechanisms.")]
		[iOS (9,0)][Mac (10,12,1)]
		TouchIDAny          = BiometryAny,

		[Advice ("'BiometryCurrentSet' is preferred over 'TouchIDCurrentSet' since Xcode 9.3. Touch ID and Face ID together are biometric authentication mechanisms.")]
		[iOS (9,0)][Mac (10,12,1)]
		TouchIDCurrentSet   = BiometryCurrentSet,

		[iOS (9,0), Mac (10,12,1)]
		BiometryAny         = 1 << 1,

		[iOS (9,0), Mac (10,12,1)]
		BiometryCurrentSet  = 1 << 3,

		DevicePasscode      = 1 << 4,

		[iOS (9,0)][Mac (10,12,1)]
		Or                  = 1 << 14,

		[iOS (9,0)][Mac (10,12,1)]
		And                 = 1 << 15,

		[iOS (9,0)][Mac (10,12,1)]
		PrivateKeyUsage     = 1 << 30,

		[iOS (9,0)][Mac (10,12,1)]
		ApplicationPassword = 1 << 31,
	}
	
	[Mac (10,10)][iOS (8,0)]
	public partial class SecAccessControl : INativeObject, IDisposable {

		private IntPtr handle;

		public IntPtr Handle {
			get {
#if !COREBUILD
				if (handle == IntPtr.Zero) {
					IntPtr error;
					handle = SecAccessControlCreateWithFlags (IntPtr.Zero, KeysAccessible.FromSecAccessible (Accessible), (nint)(int)Flags, out error);
				}
#endif
				return handle;
			}
			internal set { handle = value; }
		}

		public void Dispose ()
		{
#if !COREBUILD
			Dispose (true);
#endif
			GC.SuppressFinalize (this);
		}

#if !COREBUILD
		internal SecAccessControl (IntPtr handle)
		{
			// note: the properties won't match reality
			Handle = handle;
		}

		public SecAccessControl (SecAccessible accessible, SecAccessControlCreateFlags flags = SecAccessControlCreateFlags.UserPresence)
		{
			Accessible = accessible;
			Flags = flags;
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

		~SecAccessControl ()
		{
			Dispose (false);
		}
			
		public SecAccessible Accessible { get; private set; }
		public SecAccessControlCreateFlags Flags { get; private set; }

		[Mac (10,10)][iOS (8,0)]
		[DllImport (Constants.SecurityLibrary)]
		extern static IntPtr SecAccessControlCreateWithFlags (IntPtr allocator, /* CFTypeRef */ IntPtr protection, /* SecAccessControlCreateFlags */ nint flags, out IntPtr error);
#endif
	}
}
