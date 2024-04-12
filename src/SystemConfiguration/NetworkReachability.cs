//
// NetworkReachability.cs: NetworkReachability binding
//
// Authors:
//    Miguel de Icaza (miguel@novell.com)
//    Marek Safar (marek.safar@gmail.com)
//    Aaron Bockover (abock@xamarin.com)
//
// Copyright 2012-2014 Xamarin Inc. All rights reserved.
//

#nullable enable

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ObjCRuntime;
using CoreFoundation;
using Foundation;
using System.Net;
using System.Net.Sockets;

namespace SystemConfiguration {

	// SCNetworkReachabilityFlags -> uint32_t -> SCNetworkReachability.h
	[Flags]
	public enum NetworkReachabilityFlags {
		TransientConnection = 1 << 0,
		Reachable = 1 << 1,
		ConnectionRequired = 1 << 2,
		ConnectionOnTraffic = 1 << 3,
		InterventionRequired = 1 << 4,
		ConnectionOnDemand = 1 << 5,
		IsLocalAddress = 1 << 16,
		IsDirect = 1 << 17,
#if NET
		[UnsupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
#else
		[Unavailable (PlatformName.MacOSX)]
#endif
		IsWWAN = 1 << 18,
		ConnectionAutomatic = ConnectionOnTraffic
	}

	// http://developer.apple.com/library/ios/#documentation/SystemConfiguration/Reference/SCNetworkReachabilityRef/Reference/reference.html
	public class NetworkReachability : NativeObject {
		// netinet/in.h
		[StructLayout (LayoutKind.Sequential)]
		struct sockaddr_in {
			// We're defining fields to make the struct the correct size (expected size = 28, so 7 * 4 bytes = 28),
			// and then we're defining properties that accesses these fields to get and set field values.
			// This looks a bit convoluted, but the purpose is to avoid .NET's built-in marshaling support,
			// so that we're able to trim away the corresponding marshalling code in .NET to minimize app size.
			uint value1;
			uint value2;
			uint value3;
			uint value4;
			uint value5;
			uint value6;
			uint value7;

			unsafe byte sin_len {
				get {
					fixed (sockaddr_in* myself = &this) {
						byte* self = (byte*) myself;
						return self [0];
					}
				}
				set {
					fixed (sockaddr_in* myself = &this) {
						byte* self = (byte*) myself;
						self [0] = value;
					}
				}
			}

			unsafe byte sin_family {
				get {
					fixed (sockaddr_in* myself = &this) {
						byte* self = (byte*) myself;
						return self [1];
					}
				}
				set {
					fixed (sockaddr_in* myself = &this) {
						byte* self = (byte*) myself;
						self [1] = value;
					}
				}
			}

			unsafe short sin_port {
				get {
					fixed (sockaddr_in* myself = &this) {
						short* self = (short*) myself;
						return self [1];
					}
				}
				set {
					fixed (sockaddr_in* myself = &this) {
						short* self = (short*) myself;
						self [1] = value;
					}
				}
			}

			unsafe int sin_addr {
				get {
					fixed (sockaddr_in* myself = &this) {
						int* self = (int*) myself;
						return self [1];
					}
				}
				set {
					fixed (sockaddr_in* myself = &this) {
						int* self = (int*) myself;
						self [1] = value;
					}
				}
			}

			// IPv6
			unsafe uint sin6_flowinfo {
				get {
					fixed (sockaddr_in* myself = &this) {
						uint* self = (uint*) myself;
						return self [1];
					}
				}
				set {
					fixed (sockaddr_in* myself = &this) {
						uint* self = (uint*) myself;
						self [1] = value;
					}
				}
			}

			unsafe byte [] sin6_addr8 {
				get {
					var rv = new byte [16];
					fixed (sockaddr_in* myself = &this) {
						byte* self = (byte*) myself;
						Marshal.Copy ((IntPtr) (self + 8), rv, 0, 16);
					}
					return rv;
				}
				set {
					fixed (sockaddr_in* myself = &this) {
						byte* self = (byte*) myself;
						Marshal.Copy (value, 0, (IntPtr) (self + 8), 16);
					}
				}
			}

			unsafe uint sin6_scope_id {
				get {
					fixed (sockaddr_in* myself = &this) {
						uint* self = (uint*) myself;
						return self [6];
					}
				}
				set {
					fixed (sockaddr_in* myself = &this) {
						uint* self = (uint*) myself;
						self [6] = value;
					}
				}
			}

			public sockaddr_in (IPAddress address)
				: this ()
			{
				sin_addr = 0;
				sin_len = 28;
				sin6_flowinfo = 0;
				sin6_scope_id = 0;
				sin6_addr8 = new byte [16];

				switch (address.AddressFamily) {
				case AddressFamily.InterNetwork:
					sin_family = 2;  // Address for IPv4
#pragma warning disable CS0618 // Type or member is obsolete
					sin_addr = (int) address.Address;
#pragma warning restore CS0618 // Type or member is obsolete
					break;
				case AddressFamily.InterNetworkV6:
					sin_family = 30; // Address for IPv6
					sin6_addr8 = address.GetAddressBytes ();
					sin6_scope_id = (uint) address.ScopeId;
					break;
				default:
					throw new NotSupportedException (address.AddressFamily.ToString ());
				}

				sin_port = 0;
			}
		}

		// SCNetworkReachability.h
		[StructLayout (LayoutKind.Sequential)]
		struct SCNetworkReachabilityContext {
			public /* CFIndex */ IntPtr version;
			public /* void* */ IntPtr info;
			public IntPtr retain;
			public IntPtr release;
			public IntPtr copyDescription;

			public SCNetworkReachabilityContext (IntPtr val)
			{
				info = val;
				version = retain = release = copyDescription = IntPtr.Zero;
			}
		}

		[DllImport (Constants.SystemConfigurationLibrary)]
		extern static /* SCNetworkReachabilityRef __nullable */ IntPtr SCNetworkReachabilityCreateWithName (
			/* CFAllocatorRef __nullable */ IntPtr allocator, /* const char* __nonnull */ IntPtr address);

		[DllImport (Constants.SystemConfigurationLibrary)]
		unsafe extern static /* SCNetworkReachabilityRef __nullable */ IntPtr SCNetworkReachabilityCreateWithAddress (
			/* CFAllocatorRef __nullable */ IntPtr allocator,
			/* const struct sockaddr * __nonnull */ sockaddr_in* address);

		[DllImport (Constants.SystemConfigurationLibrary)]
		unsafe extern static /* SCNetworkReachabilityRef __nullable */ IntPtr SCNetworkReachabilityCreateWithAddressPair (
			/* CFAllocatorRef __nullable */ IntPtr allocator,
			/* const struct sockaddr * __nullable */ sockaddr_in* localAddress,
			/* const struct sockaddr * __nullable */ sockaddr_in* remoteAddress);

		[DllImport (Constants.SystemConfigurationLibrary)]
		unsafe extern static /* SCNetworkReachabilityRef __nullable */ IntPtr SCNetworkReachabilityCreateWithAddressPair (
			/* CFAllocatorRef __nullable */ IntPtr allocator,
			/* const struct sockaddr * __nullable */ IntPtr localAddress,
			/* const struct sockaddr * __nullable */ sockaddr_in* remoteAddress);

		[DllImport (Constants.SystemConfigurationLibrary)]
		unsafe extern static /* SCNetworkReachabilityRef __nullable */ IntPtr SCNetworkReachabilityCreateWithAddressPair (
			/* CFAllocatorRef __nullable */ IntPtr allocator,
			/* const struct sockaddr * __nullable */ sockaddr_in* localAddress,
			/* const struct sockaddr * __nullable */ IntPtr remoteAddress);

		static IntPtr CheckFailure (IntPtr handle)
		{
			if (handle == IntPtr.Zero)
				throw SystemConfigurationException.FromMostRecentCall ();
			return handle;
		}

		static IntPtr Create (IPAddress ip)
		{
			if (ip is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (ip));

			var s = new sockaddr_in (ip);
			unsafe {
				return CheckFailure (SCNetworkReachabilityCreateWithAddress (IntPtr.Zero, &s));
			}
		}

		public NetworkReachability (IPAddress ip)
			: base (Create (ip), true)
		{
		}

		static IntPtr Create (string address)
		{
			if (address is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (address));

			using var addressStr = new TransientString (address);
			return CheckFailure (SCNetworkReachabilityCreateWithName (IntPtr.Zero, addressStr));
		}

		public NetworkReachability (string address)
			: base (Create (address), true)
		{
		}

		static IntPtr Create (IPAddress localAddress, IPAddress remoteAddress)
		{
			if (localAddress is null && remoteAddress is null)
				throw new ArgumentException ("At least one address is required");

			IntPtr handle;
			if (localAddress is null) {
				var remote = new sockaddr_in (remoteAddress);

				unsafe {
					handle = SCNetworkReachabilityCreateWithAddressPair (IntPtr.Zero, IntPtr.Zero, &remote);
				}
			} else if (remoteAddress is null) {
				var local = new sockaddr_in (localAddress);

				unsafe {
					handle = SCNetworkReachabilityCreateWithAddressPair (IntPtr.Zero, &local, IntPtr.Zero);
				}
			} else {
				var local = new sockaddr_in (localAddress);
				var remote = new sockaddr_in (remoteAddress);

				unsafe {
					handle = SCNetworkReachabilityCreateWithAddressPair (IntPtr.Zero, &local, &remote);
				}
			}

			return CheckFailure (handle);
		}

		public NetworkReachability (IPAddress localAddress, IPAddress remoteAddress)
			: base (Create (localAddress, remoteAddress), true)
		{
		}

		[DllImport (Constants.SystemConfigurationLibrary)]
		unsafe static extern int SCNetworkReachabilityGetFlags (/* SCNetworkReachabilityRef __nonnull */ IntPtr target,
			/* SCNetworkReachabilityFlags* __nonnull */ NetworkReachabilityFlags* flags);

		public bool TryGetFlags (out NetworkReachabilityFlags flags)
		{
			return GetFlags (out flags) == StatusCode.OK;
		}

		public StatusCode GetFlags (out NetworkReachabilityFlags flags)
		{
			flags = default;
			int rv;
			unsafe {
				rv = SCNetworkReachabilityGetFlags (Handle, (NetworkReachabilityFlags*) Unsafe.AsPointer<NetworkReachabilityFlags> (ref flags));
			}
			return rv == 0 ? StatusCodeError.SCError () : StatusCode.OK;
		}

#if !NET
		delegate void SCNetworkReachabilityCallBack (/* SCNetworkReachabilityRef */ IntPtr handle, /* SCNetworkReachabilityFlags */ NetworkReachabilityFlags flags, /* void* */ IntPtr info);
#endif

		[DllImport (Constants.SystemConfigurationLibrary)]
		unsafe static extern /* Boolean */ byte SCNetworkReachabilitySetCallback (
			/* SCNetworkReachabilityRef __nonnull */ IntPtr handle,
#if NET
			/* __nullable SCNetworkReachabilityCallBack */ delegate* unmanaged<IntPtr, NetworkReachabilityFlags, IntPtr, void> callout,
#else
			/* __nullable */ IntPtr callout,
#endif
			/* __nullable */ SCNetworkReachabilityContext* context);

		public delegate void Notification (NetworkReachabilityFlags flags);

		Notification? notification;
		GCHandle gch;
#if !NET
		SCNetworkReachabilityCallBack? callouth;
#endif

#if NET
		[UnmanagedCallersOnly]
#else
		[MonoPInvokeCallback (typeof (SCNetworkReachabilityCallBack))]
#endif
		static void Callback (IntPtr handle, NetworkReachabilityFlags flags, IntPtr info)
		{
			GCHandle gch = GCHandle.FromIntPtr (info);
			var r = gch.Target as NetworkReachability;
			if (r?.notification is null)
				return;
			r.notification (flags);
		}

		public StatusCode SetNotification (Notification callback)
		{
			bool rv;
			if (notification is null) {
				if (callback is null)
					return StatusCode.OK;

				gch = GCHandle.Alloc (this);
				var ctx = new SCNetworkReachabilityContext (GCHandle.ToIntPtr (gch));

#if !NET
				lock (typeof (NetworkReachability)) {
					if (callouth is null)
						callouth = Callback;
				}
#endif

				unsafe {
#if NET
					rv = SCNetworkReachabilitySetCallback (Handle, &Callback, &ctx) != 0;
#else
					rv = SCNetworkReachabilitySetCallback (Handle, Marshal.GetFunctionPointerForDelegate (callouth), &ctx) != 0;
#endif
				}
			} else {
				if (callback is null) {
					this.notification = null;
#if !NET
					callouth = null;
#endif
					unsafe {
#if NET
						rv = SCNetworkReachabilitySetCallback (Handle, null, null) != 0;
#else
						rv = SCNetworkReachabilitySetCallback (Handle, IntPtr.Zero, null) != 0;
#endif
					}
					if (!rv)
						return StatusCodeError.SCError ();

					return StatusCode.OK;
				}
			}

			this.notification = callback;
			return StatusCode.OK;
		}

		[DllImport (Constants.SystemConfigurationLibrary)]
		extern static /* Boolean */ byte SCNetworkReachabilityScheduleWithRunLoop (
			/* SCNetworkReachabilityRef __nonnull */ IntPtr target, /* CFRunLoopRef __nonnull */ IntPtr runloop,
			/* CFStringRef __nonnull */ IntPtr runLoopMode);

		public bool Schedule (CFRunLoop runLoop, string mode)
		{
			if (runLoop is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (runLoop));

			if (mode is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (mode));

			var modeHandle = CFString.CreateNative (mode);
			try {
				return SCNetworkReachabilityScheduleWithRunLoop (Handle, runLoop.Handle, modeHandle) != 0;
			} finally {
				CFString.ReleaseNative (modeHandle);
			}
		}

		public bool Schedule ()
		{
			return Schedule (CFRunLoop.Current, CFRunLoop.ModeDefault);
		}

		[DllImport (Constants.SystemConfigurationLibrary)]
		extern static int SCNetworkReachabilityUnscheduleFromRunLoop (/* SCNetworkReachabilityRef */ IntPtr target, /* CFRunLoopRef */ IntPtr runloop, /* CFStringRef */ IntPtr runLoopMode);

		public bool Unschedule (CFRunLoop runLoop, string mode)
		{
			if (runLoop is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (runLoop));

			if (mode is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (mode));

			var modeHandle = CFString.CreateNative (mode);
			try {
				return SCNetworkReachabilityUnscheduleFromRunLoop (Handle, runLoop.Handle, modeHandle) != 0;
			} finally {
				CFString.ReleaseNative (modeHandle);
			}
		}

		public bool Unschedule ()
		{
			return Unschedule (CFRunLoop.Current, CFRunLoop.ModeDefault);
		}

		[DllImport (Constants.SystemConfigurationLibrary)]
		extern static /* Boolean */ byte SCNetworkReachabilitySetDispatchQueue (
			/* SCNetworkReachabilityRef __nonnull */ IntPtr target,
			/* dispatch_queue_t __nullable */ IntPtr queue);

		public bool SetDispatchQueue (DispatchQueue queue)
		{
			return SCNetworkReachabilitySetDispatchQueue (Handle, queue.GetHandle ()) != 0;
		}
	}
}
