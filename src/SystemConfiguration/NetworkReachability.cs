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
		[StructLayout (LayoutKind.Explicit, Size = 28)]
		struct sockaddr_in {
			[FieldOffset (0)] byte sin_len;
			[FieldOffset (1)] byte sin_family;
			[FieldOffset (2)] short sin_port;
			[FieldOffset (4)] int sin_addr;

			// IPv6
			[FieldOffset (4)] uint sin6_flowinfo;
			[FieldOffset (8)][MarshalAs (UnmanagedType.ByValArray, SizeConst = 16)] public byte [] sin6_addr8;
			[FieldOffset (24)] uint sin6_scope_id;

			public sockaddr_in (IPAddress address)
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
		extern static /* SCNetworkReachabilityRef __nullable */ IntPtr SCNetworkReachabilityCreateWithAddress (
			/* CFAllocatorRef __nullable */ IntPtr allocator,
			/* const struct sockaddr * __nonnull */ ref sockaddr_in address);

		[DllImport (Constants.SystemConfigurationLibrary)]
		extern static /* SCNetworkReachabilityRef __nullable */ IntPtr SCNetworkReachabilityCreateWithAddressPair (
			/* CFAllocatorRef __nullable */ IntPtr allocator,
			/* const struct sockaddr * __nullable */ ref sockaddr_in localAddress,
			/* const struct sockaddr * __nullable */ ref sockaddr_in remoteAddress);

		[DllImport (Constants.SystemConfigurationLibrary)]
		extern static /* SCNetworkReachabilityRef __nullable */ IntPtr SCNetworkReachabilityCreateWithAddressPair (
			/* CFAllocatorRef __nullable */ IntPtr allocator,
			/* const struct sockaddr * __nullable */ IntPtr localAddress,
			/* const struct sockaddr * __nullable */ ref sockaddr_in remoteAddress);

		[DllImport (Constants.SystemConfigurationLibrary)]
		extern static /* SCNetworkReachabilityRef __nullable */ IntPtr SCNetworkReachabilityCreateWithAddressPair (
			/* CFAllocatorRef __nullable */ IntPtr allocator,
			/* const struct sockaddr * __nullable */ ref sockaddr_in localAddress,
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
			return CheckFailure (SCNetworkReachabilityCreateWithAddress (IntPtr.Zero, ref s));
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

				handle = SCNetworkReachabilityCreateWithAddressPair (IntPtr.Zero, IntPtr.Zero, ref remote);
			} else if (remoteAddress is null) {
				var local = new sockaddr_in (localAddress);

				handle = SCNetworkReachabilityCreateWithAddressPair (IntPtr.Zero, ref local, IntPtr.Zero);
			} else {
				var local = new sockaddr_in (localAddress);
				var remote = new sockaddr_in (remoteAddress);

				handle = SCNetworkReachabilityCreateWithAddressPair (IntPtr.Zero, ref local, ref remote);
			}

			return CheckFailure (handle);
		}

		public NetworkReachability (IPAddress localAddress, IPAddress remoteAddress)
			: base (Create (localAddress, remoteAddress), true)
		{
		}

		[DllImport (Constants.SystemConfigurationLibrary)]
		static extern int SCNetworkReachabilityGetFlags (/* SCNetworkReachabilityRef __nonnull */ IntPtr target,
			/* SCNetworkReachabilityFlags* __nonnull */ out NetworkReachabilityFlags flags);

		public bool TryGetFlags (out NetworkReachabilityFlags flags)
		{
			return GetFlags (out flags) == StatusCode.OK;
		}

		public StatusCode GetFlags (out NetworkReachabilityFlags flags)
		{
			return SCNetworkReachabilityGetFlags (Handle, out flags) == 0 ?
				StatusCodeError.SCError () : StatusCode.OK;
		}

#if !NET
		delegate void SCNetworkReachabilityCallBack (/* SCNetworkReachabilityRef */ IntPtr handle, /* SCNetworkReachabilityFlags */ NetworkReachabilityFlags flags, /* void* */ IntPtr info);
#endif

		[DllImport (Constants.SystemConfigurationLibrary)]
		[return: MarshalAs (UnmanagedType.U1)]
		unsafe static extern /* Boolean */ bool SCNetworkReachabilitySetCallback (
			/* SCNetworkReachabilityRef __nonnull */ IntPtr handle,
#if NET
			/* __nullable SCNetworkReachabilityCallBack */ delegate* unmanaged<IntPtr, NetworkReachabilityFlags, IntPtr, void> callout,
#else
			/* __nullable */ SCNetworkReachabilityCallBack? callout,
#endif
			/* __nullable */ ref SCNetworkReachabilityContext context);

		[DllImport (Constants.SystemConfigurationLibrary)]
		[return: MarshalAs (UnmanagedType.U1)]
		unsafe static extern /* Boolean */ bool SCNetworkReachabilitySetCallback (
			/* SCNetworkReachabilityRef __nullable */ IntPtr handle,
#if NET
			/* __nullable SCNetworkReachabilityCallBack */ delegate* unmanaged<IntPtr, NetworkReachabilityFlags, IntPtr, void> callout,
#else
			/* __nullable */ SCNetworkReachabilityCallBack? callout,
#endif
			/* SCNetworkReachabilityContext* __nullable */ IntPtr context);

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

#if NET
				unsafe {
					if (!SCNetworkReachabilitySetCallback (Handle, &Callback, ref ctx))
						return StatusCodeError.SCError ();
				}
#else
				if (!SCNetworkReachabilitySetCallback (Handle, callouth, ref ctx))
					return StatusCodeError.SCError ();
#endif
			} else {
				if (callback is null) {
					this.notification = null;
#if !NET
					callouth = null;
#endif
					unsafe {
						if (!SCNetworkReachabilitySetCallback (Handle, null, IntPtr.Zero))
							return StatusCodeError.SCError ();
					}

					return StatusCode.OK;
				}
			}

			this.notification = callback;
			return StatusCode.OK;
		}

		[DllImport (Constants.SystemConfigurationLibrary)]
		[return: MarshalAs (UnmanagedType.U1)]
		extern static /* Boolean */ bool SCNetworkReachabilityScheduleWithRunLoop (
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
				return SCNetworkReachabilityScheduleWithRunLoop (Handle, runLoop.Handle, modeHandle);
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
		[return: MarshalAs (UnmanagedType.U1)]
		extern static /* Boolean */ bool SCNetworkReachabilitySetDispatchQueue (
			/* SCNetworkReachabilityRef __nonnull */ IntPtr target,
			/* dispatch_queue_t __nullable */ IntPtr queue);

		public bool SetDispatchQueue (DispatchQueue queue)
		{
			return SCNetworkReachabilitySetDispatchQueue (Handle, queue.GetHandle ());
		}
	}
}
