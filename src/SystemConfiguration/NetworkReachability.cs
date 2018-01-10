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
		TransientConnection = 1<<0,
		Reachable = 1<<1,
		ConnectionRequired = 1<<2,
		ConnectionOnTraffic = 1<<3,
		InterventionRequired = 1<<4,
		ConnectionOnDemand = 1<<5,
		IsLocalAddress = 1<<16,
		IsDirect = 1<<17,
#if !MONOMAC
		IsWWAN = 1<<18,
#endif
		ConnectionAutomatic = ConnectionOnTraffic
	}
	
	// http://developer.apple.com/library/ios/#documentation/SystemConfiguration/Reference/SCNetworkReachabilityRef/Reference/reference.html
	public class NetworkReachability : INativeObject, IDisposable {
		internal IntPtr handle;

		// netinet/in.h
		[StructLayout (LayoutKind.Explicit, Size = 28)]
		struct sockaddr_in {
			[FieldOffset (0)] byte sin_len;
			[FieldOffset (1)] byte sin_family;
			[FieldOffset (2)] short sin_port;
			[FieldOffset (4)] int sin_addr;
			
			// IPv6
			[FieldOffset (4)] uint sin6_flowinfo;
			[FieldOffset (8)] [MarshalAs (UnmanagedType.ByValArray, SizeConst = 16)] public byte[] sin6_addr8;
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
					sin_addr = (int) address.Address;
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
			/* CFAllocatorRef __nullable */ IntPtr allocator, /* const char* __nonnull */ string address);

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
		
		public NetworkReachability (IPAddress ip)
		{
			if (ip == null)
				throw new ArgumentNullException ("ip");
				
			var s = new sockaddr_in (ip);
			handle = SCNetworkReachabilityCreateWithAddress (IntPtr.Zero, ref s);
			if (handle == IntPtr.Zero)
				throw SystemConfigurationException.FromMostRecentCall ();
		}
		
		public NetworkReachability (string address)
		{
			if (address == null)
				throw new ArgumentNullException ("address");
			
			handle = SCNetworkReachabilityCreateWithName (IntPtr.Zero, address);
			if (handle == IntPtr.Zero)
				throw SystemConfigurationException.FromMostRecentCall ();
		}
		
		public NetworkReachability (IPAddress localAddress, IPAddress remoteAddress)
		{
			if (localAddress == null && remoteAddress == null)
				throw new ArgumentException ("At least one address is required");
				
			if (localAddress == null) {
				var remote = new sockaddr_in (remoteAddress);
				
				handle = SCNetworkReachabilityCreateWithAddressPair (IntPtr.Zero, IntPtr.Zero, ref remote);
			} else if (remoteAddress == null) {
				var local = new sockaddr_in (localAddress);
				
				handle = SCNetworkReachabilityCreateWithAddressPair (IntPtr.Zero, ref local, IntPtr.Zero);
			} else {
				var local = new sockaddr_in (localAddress);
				var remote = new sockaddr_in (remoteAddress);
			
				handle = SCNetworkReachabilityCreateWithAddressPair (IntPtr.Zero, ref local, ref remote);
			}
			
			if (handle == IntPtr.Zero)
				throw SystemConfigurationException.FromMostRecentCall ();
		} 

		~NetworkReachability ()
		{
			Dispose (false);
		}

		public IntPtr Handle {
			get {
				return handle;
			}
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
			if (handle != IntPtr.Zero){
				CFObject.CFRelease (handle);
				handle = IntPtr.Zero;
			}
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
			return SCNetworkReachabilityGetFlags (handle, out flags) == 0 ?
				StatusCodeError.SCError () : StatusCode.OK;
		}

		delegate void SCNetworkReachabilityCallBack (/* SCNetworkReachabilityRef */ IntPtr handle, /* SCNetworkReachabilityFlags */ NetworkReachabilityFlags flags, /* void* */ IntPtr info);

		[DllImport (Constants.SystemConfigurationLibrary)]
		static extern /* Boolean */ bool SCNetworkReachabilitySetCallback (
			/* SCNetworkReachabilityRef __nonnull */ IntPtr handle, 
			/* __nullable */ SCNetworkReachabilityCallBack callout,
			/* __nullable */ ref SCNetworkReachabilityContext context);
		
		[DllImport (Constants.SystemConfigurationLibrary)]
		static extern /* Boolean */ bool SCNetworkReachabilitySetCallback (
			/* SCNetworkReachabilityRef __nullable */ IntPtr handle, 
			/* __nullable */ SCNetworkReachabilityCallBack callout, 
			/* SCNetworkReachabilityContext* __nullable */ IntPtr context);

		public delegate void Notification (NetworkReachabilityFlags flags);

		Notification notification;
		GCHandle gch;
		SCNetworkReachabilityCallBack callouth;
		
		[MonoPInvokeCallback (typeof (SCNetworkReachabilityCallBack))]
		static void Callback (IntPtr handle, NetworkReachabilityFlags flags, IntPtr info)
		{
			GCHandle gch = GCHandle.FromIntPtr (info);
			var r = gch.Target as NetworkReachability;
			if (r == null)
				return;
			r.notification (flags);
		}

#if !XAMCORE_2_0
		[Advice ("Use 'SetNotification' instead.")]
		public bool SetCallback (Notification callback)
		{
			return SetNotification (callback) == StatusCode.OK;
		}
#endif
		
		public StatusCode SetNotification (Notification callback)
		{
			if (notification == null){
				if (callback == null)
					return StatusCode.OK;
			
				gch = GCHandle.Alloc (this);
				var ctx = new SCNetworkReachabilityContext (GCHandle.ToIntPtr (gch));

				lock (typeof (NetworkReachability)){
					if (callouth == null)
						callouth = Callback;
				}
				
				if (!SCNetworkReachabilitySetCallback (handle, callouth, ref ctx))
					return StatusCodeError.SCError ();
			} else {
				if (callback == null){
					this.notification = null;
					callouth = null;
					if (!SCNetworkReachabilitySetCallback (handle, null, IntPtr.Zero))
						return StatusCodeError.SCError ();
					
					return StatusCode.OK;
				}
			}
			
			this.notification = callback;
			return StatusCode.OK;
		}

		[DllImport (Constants.SystemConfigurationLibrary)]
		extern static /* Boolean */ bool SCNetworkReachabilityScheduleWithRunLoop (
			/* SCNetworkReachabilityRef __nonnull */ IntPtr target, /* CFRunLoopRef __nonnull */ IntPtr runloop, 
			/* CFStringRef __nonnull */ IntPtr runLoopMode);
		
		public bool Schedule (CFRunLoop runLoop, string mode)
		{
			if (runLoop == null)
				throw new ArgumentNullException ("runLoop");

			// new CFString already does a null check			
			using (var cfstring = new CFString (mode)){
				return SCNetworkReachabilityScheduleWithRunLoop (handle, runLoop.Handle, cfstring.Handle);
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
			if (runLoop == null)
				throw new ArgumentNullException ("runLoop");

			if (mode == null)
				throw new ArgumentNullException ("mode");
			
			using (var cfstring = new CFString (mode)){
				return SCNetworkReachabilityUnscheduleFromRunLoop (handle, runLoop.Handle, cfstring.Handle) != 0;
			}
		}

		public bool Unschedule ()
		{
			return Unschedule (CFRunLoop.Current, CFRunLoop.ModeDefault);
		}

		[DllImport (Constants.SystemConfigurationLibrary)]
		extern static /* Boolean */ bool SCNetworkReachabilitySetDispatchQueue (
			/* SCNetworkReachabilityRef __nonnull */ IntPtr target,
			/* dispatch_queue_t __nullable */ IntPtr queue);

		public bool SetDispatchQueue (DispatchQueue queue)
		{
			return SCNetworkReachabilitySetDispatchQueue (handle, queue == null ? IntPtr.Zero : queue.Handle);
		}
	}
}
