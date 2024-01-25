//
// MonoMac.CoreFoundation.CFSocket
//
// Authors:
//      Martin Baulig (martin.baulig@xamarin.com)
//
// Copyright 2012 Xamarin Inc. (http://www.xamarin.com)
//
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#nullable enable

using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;

using CoreFoundation;

using Foundation;

using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreFoundation {

	[Flags]
	[Native]
	// defined as CFOptionFlags (unsigned long [long] = nuint) - System/Library/Frameworks/CoreFoundation.framework/Headers/CFSocket.h
	public enum CFSocketCallBackType : ulong {
		NoCallBack = 0,
		ReadCallBack = 1,
		AcceptCallBack = 2,
		DataCallBack = 3,
		ConnectCallBack = 4,
		WriteCallBack = 8
	}

	// defined as CFIndex (long [long] = nint) - System/Library/Frameworks/CoreFoundation.framework/Headers/CFSocket.h
	[Native]
	public enum CFSocketError : long {
		Success = 0,
		Error = -1,
		Timeout = -2
	}

	[Flags]
	// anonymous and typeless native enum - System/Library/Frameworks/CoreFoundation.framework/Headers/CFSocket.h
	public enum CFSocketFlags {
		AutomaticallyReenableReadCallBack = 1,
		AutomaticallyReenableAcceptCallBack = 2,
		AutomaticallyReenableDataCallBack = 3,
		AutomaticallyReenableWriteCallBack = 8,
		LeaveErrors = 64,
		CloseOnInvalidate = 128
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public struct CFSocketNativeHandle {
		// typedef int CFSocketNativeHandle
		internal readonly int handle;

		internal CFSocketNativeHandle (int handle)
		{
			this.handle = handle;
		}

		public override string ToString ()
		{
			return string.Format ("[CFSocketNativeHandle {0}]", handle);
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CFSocketException : Exception {
		public CFSocketError Error {
			get;
			private set;
		}

		public CFSocketException (CFSocketError error)
		{
			this.Error = error;
		}
	}

	struct CFSocketSignature {
		int /* SInt32 */ protocolFamily;
		int /* SInt32 */ socketType;
		int /* SInt32 */ protocol;
		IntPtr address;

		public CFSocketSignature (AddressFamily family, SocketType type,
								  ProtocolType proto, CFSocketAddress address)
		{
			this.protocolFamily = AddressFamilyToInt (family);
			this.socketType = SocketTypeToInt (type);
			this.protocol = ProtocolToInt (proto);
			this.address = address.Handle;
		}

		internal static int AddressFamilyToInt (AddressFamily family)
		{
			switch (family) {
			case AddressFamily.Unspecified:
				return 0;
			case AddressFamily.Unix:
				return 1;
			case AddressFamily.InterNetwork:
				return 2;
			case AddressFamily.AppleTalk:
				return 16;
			case AddressFamily.InterNetworkV6:
				return 30;
			default:
				throw new ArgumentException ();
			}
		}

		internal static int SocketTypeToInt (SocketType type)
		{
			if ((int) type == 0)
				return 0;

			switch (type) {
			case SocketType.Unknown:
				return 0;
			case SocketType.Stream:
				return 1;
			case SocketType.Dgram:
				return 2;
			case SocketType.Raw:
				return 3;
			case SocketType.Rdm:
				return 4;
			case SocketType.Seqpacket:
				return 5;
			default:
				throw new ArgumentException ();
			}
		}

		internal static int ProtocolToInt (ProtocolType type)
		{
			return (int) type;
		}

	}

	class CFSocketAddress : CFDataBuffer {
		public CFSocketAddress (IPEndPoint endpoint)
			: base (CreateData (endpoint))
		{
		}

		internal static IPEndPoint? EndPointFromAddressPtr (IntPtr address, bool owns)
		{
			if (address == IntPtr.Zero)
				return null;

			using (var buffer = new CFDataBuffer (address, owns)) {
				if (buffer [1] == 30) { // AF_INET6
					int port = (buffer [2] << 8) + buffer [3];
					var bytes = new byte [16];
					Buffer.BlockCopy (buffer.Data, 8, bytes, 0, 16);
					return new IPEndPoint (new IPAddress (bytes), port);
				} else if (buffer [1] == 2) { // AF_INET
					int port = (buffer [2] << 8) + buffer [3];
					var bytes = new byte [4];
					Buffer.BlockCopy (buffer.Data, 4, bytes, 0, 4);
					return new IPEndPoint (new IPAddress (bytes), port);
				} else {
					throw new ArgumentException ();
				}
			}
		}

		static byte [] CreateData (IPEndPoint endpoint)
		{
			if (endpoint is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (endpoint));

			if (endpoint.AddressFamily == AddressFamily.InterNetwork) {
				var buffer = new byte [16];
				buffer [0] = 16;
				buffer [1] = 2; // AF_INET
				buffer [2] = (byte) (endpoint.Port >> 8);
				buffer [3] = (byte) (endpoint.Port & 0xff);
				Buffer.BlockCopy (endpoint.Address.GetAddressBytes (), 0, buffer, 4, 4);
				return buffer;
			} else if (endpoint.AddressFamily == AddressFamily.InterNetworkV6) {
				var buffer = new byte [28];
				buffer [0] = 32;
				buffer [1] = 30; // AF_INET6
				buffer [2] = (byte) (endpoint.Port >> 8);
				buffer [3] = (byte) (endpoint.Port & 0xff);
				Buffer.BlockCopy (endpoint.Address.GetAddressBytes (), 0, buffer, 8, 16);
				return buffer;
			} else {
				throw new ArgumentException ();
			}
		}
	}

	[StructLayout (LayoutKind.Sequential)]
	struct CFSocketContext {
		nint Version; // CFIndex
		public /* void*/ IntPtr Info;
#if NET
		unsafe delegate* unmanaged<IntPtr, IntPtr> Retain;
		unsafe delegate* unmanaged<IntPtr, void> Release;
#else
		IntPtr Retain;
		IntPtr Release;
#endif
		IntPtr CopyDescription;

		public CFSocketContext (IntPtr info) : this ()
		{
			Info = info;
#if NET
			unsafe {
				Retain = &OnContextRetain;
				Release = &OnContextRelease;
			}
#else
			Retain = Marshal.GetFunctionPointerForDelegate (retainCallback);
			Release = Marshal.GetFunctionPointerForDelegate (releaseCallback);
#endif
		}

#if !NET
		delegate IntPtr RetainCallback (IntPtr ptr);
		static readonly RetainCallback retainCallback = OnContextRetain;
#endif

#if NET
		[UnmanagedCallersOnly]
#else
		[MonoPInvokeCallback (typeof (RetainCallback))]
#endif
		static IntPtr OnContextRetain (IntPtr ptr)
		{
			var gch = GCHandle.FromIntPtr (ptr);
			var socket = (CFSocket?) gch.Target;
			socket?.RetainContext ();
			return ptr;
		}

#if !NET
		delegate void ReleaseCallback (IntPtr ptr);
		static readonly ReleaseCallback releaseCallback = OnContextRelease;
#endif

#if NET
		[UnmanagedCallersOnly]
#else
		[MonoPInvokeCallback (typeof (ReleaseCallback))]
#endif
		static void OnContextRelease (IntPtr ptr)
		{
			var gch = GCHandle.FromIntPtr (ptr);
			var socket = (CFSocket?) gch.Target;
			socket?.ReleaseContext (gch);
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CFSocket : CFType {
		int contextRetainCount;

		internal void RetainContext ()
		{
			Interlocked.Increment (ref contextRetainCount);
		}

		unsafe internal void ReleaseContext (GCHandle gch)
		{
			var postRC = Interlocked.Decrement (ref contextRetainCount);
			if (postRC == 0 && gch.IsAllocated) {
				gch.Free ();
			}
		}

		protected override void Dispose (bool disposing)
		{
			if (Handle != NativeHandle.Zero)
				CFSocketInvalidate (Handle);
			base.Dispose (disposing);
		}

#if !NET
		delegate void CFSocketCallBack (IntPtr s, nuint type, IntPtr address, IntPtr data, IntPtr info);
#endif

#if NET
		[UnmanagedCallersOnly]
#else
		[MonoPInvokeCallback (typeof (CFSocketCallBack))]
#endif
		static void OnCallback (IntPtr s, nuint type, IntPtr address, IntPtr data, IntPtr info)
		{
			var socket = GCHandle.FromIntPtr (info).Target as CFSocket;
			if (socket is null)
				return;
			CFSocketCallBackType cbType = (CFSocketCallBackType) (ulong) type;

			if (cbType == CFSocketCallBackType.AcceptCallBack && socket.AcceptEvent is not null) {
				var ep = CFSocketAddress.EndPointFromAddressPtr (address, false)!;
				var handle = new CFSocketNativeHandle (Marshal.ReadInt32 (data));
				socket.OnAccepted (new CFSocketAcceptEventArgs (handle, ep));
			} else if (cbType == CFSocketCallBackType.ConnectCallBack && socket.ConnectEvent is not null) {
				CFSocketError result;
				if (data == IntPtr.Zero)
					result = CFSocketError.Success;
				else {
					// Note that we read a 32bit value even if CFSocketError is a nint:
					// 'or a pointer to an SInt32 error code if the connect failed.'
					result = (CFSocketError) Marshal.ReadInt32 (data);
				}
				socket.OnConnect (new CFSocketConnectEventArgs (result));
			} else if (cbType == CFSocketCallBackType.DataCallBack && socket.DataEvent is not null) {
				using (var cfdata = new CFData (data, false)) {
					if (cfdata.Length > 0) {
						var ep = CFSocketAddress.EndPointFromAddressPtr (address, false)!;
						socket.OnData (new CFSocketDataEventArgs (ep, cfdata.GetBuffer ()));
					}
				}
			} else if (cbType == CFSocketCallBackType.NoCallBack) {
				// nothing to do
			} else if (cbType == CFSocketCallBackType.ReadCallBack && socket.ReadEvent is not null) {
				socket.OnRead (new CFSocketReadEventArgs ());
			} else if (cbType == CFSocketCallBackType.WriteCallBack && socket.WriteEvent is not null) {
				socket.OnWrite (new CFSocketWriteEventArgs ());
			}
		}

#if NET
		[DllImport (Constants.CoreFoundationLibrary)]
		unsafe extern static IntPtr CFSocketCreate (IntPtr allocator, int /*SInt32*/ family, int /*SInt32*/ type, int /*SInt32*/ proto,
											 nuint /*CFOptionFlags*/ callBackTypes,
											 delegate* unmanaged<IntPtr, nuint, IntPtr, IntPtr, IntPtr, void> callout, CFSocketContext* ctx);
#else
		[DllImport (Constants.CoreFoundationLibrary)]
		unsafe extern static IntPtr CFSocketCreate (IntPtr allocator, int /*SInt32*/ family, int /*SInt32*/ type, int /*SInt32*/ proto,
											 nuint /*CFOptionFlags*/ callBackTypes,
											 CFSocketCallBack callout, CFSocketContext* ctx);
#endif

#if NET
		[DllImport (Constants.CoreFoundationLibrary)]
		unsafe extern static IntPtr CFSocketCreateWithNative (IntPtr allocator, CFSocketNativeHandle sock,
													   nuint /*CFOptionFlags*/ callBackTypes,
													   delegate* unmanaged<IntPtr, nuint, IntPtr, IntPtr, IntPtr, void> callout, CFSocketContext* ctx);
#else
		[DllImport (Constants.CoreFoundationLibrary)]
		unsafe extern static IntPtr CFSocketCreateWithNative (IntPtr allocator, CFSocketNativeHandle sock,
													   nuint /*CFOptionFlags*/ callBackTypes,
													   CFSocketCallBack callout, CFSocketContext* ctx);
#endif

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static IntPtr CFSocketCreateRunLoopSource (IntPtr allocator, IntPtr socket, nint order);

		public CFSocket ()
			: this (0, 0, 0)
		{
		}

		public CFSocket (AddressFamily family, SocketType type, ProtocolType proto)
			: this (family, type, proto, CFRunLoop.Current)
		{
		}

		public CFSocket (AddressFamily family, SocketType type, ProtocolType proto, CFRunLoop loop)
			: this (CFSocketSignature.AddressFamilyToInt (family),
				CFSocketSignature.SocketTypeToInt (type),
				CFSocketSignature.ProtocolToInt (proto), loop)
		{
		}

		unsafe delegate IntPtr CreateSocket (CFSocketContext* ctx);

		const CFSocketCallBackType defaultCallbackTypes = CFSocketCallBackType.DataCallBack | CFSocketCallBackType.ConnectCallBack;
		CFSocket (int family, int type, int proto, CFRunLoop loop)
		{
			unsafe {
#if NET
				Initialize (
					loop,
					(CFSocketContext* ctx) => CFSocketCreate (IntPtr.Zero, family, type, proto, (nuint) (ulong) defaultCallbackTypes, &OnCallback, ctx)
				);
#else
				Initialize (
					loop,
					(CFSocketContext* ctx) => CFSocketCreate (IntPtr.Zero, family, type, proto, (nuint) (ulong) defaultCallbackTypes, OnCallback, ctx)
				);
#endif
			}
		}

		CFSocket (CFSocketNativeHandle sock)
		{
			unsafe {
#if NET
				Initialize (
					CFRunLoop.Current,
					(CFSocketContext* ctx) => CFSocketCreateWithNative (IntPtr.Zero, sock, (nuint) (ulong) defaultCallbackTypes, &OnCallback, ctx)
				);
#else
				Initialize (
					CFRunLoop.Current,
					(CFSocketContext* ctx) => CFSocketCreateWithNative (IntPtr.Zero, sock, (nuint) (ulong) defaultCallbackTypes, OnCallback, ctx)
				);
#endif
			}
		}

		internal CFSocket (CFSocketSignature sig, double timeout)
		{
			unsafe {
#if NET
				Initialize (
					CFRunLoop.Current,
					(CFSocketContext* ctx) => CFSocketCreateConnectedToSocketSignature (IntPtr.Zero, ref sig, (nuint) (ulong) defaultCallbackTypes, &OnCallback, ctx, timeout)
				);
#else
				Initialize (
					CFRunLoop.Current,
					(CFSocketContext* ctx) => CFSocketCreateConnectedToSocketSignature (IntPtr.Zero, ref sig, (nuint) (ulong) defaultCallbackTypes, OnCallback, ctx, timeout)
				);
#endif
			}
		}

		void Initialize (CFRunLoop runLoop, CreateSocket createSocket)
		{
			var gch = GCHandle.Alloc (this);
			try {
				var ctx = new CFSocketContext ((IntPtr) gch);
				IntPtr handle;
				unsafe {
					CFSocketContext* pctx = &ctx;
					handle = createSocket (pctx);
				}
				if (handle == IntPtr.Zero)
					throw new CFSocketException (CFSocketError.Error);

				using (var source = new CFRunLoopSource (CFSocketCreateRunLoopSource (IntPtr.Zero, handle, 0), true)) {
					runLoop.AddSource (source, CFRunLoop.ModeDefault);
				}

				this.Handle = handle;
			} catch {
				gch.Free ();
				throw;
			}
		}

#if NET
		[DllImport (Constants.CoreFoundationLibrary)]
		unsafe extern static IntPtr CFSocketCreateConnectedToSocketSignature (IntPtr allocator, ref CFSocketSignature signature,
																	   nuint /*CFOptionFlags*/ callBackTypes,
																	   delegate* unmanaged<IntPtr, nuint, IntPtr, IntPtr, IntPtr, void> callout,
																	   CFSocketContext* context, double timeout);
#else
		[DllImport (Constants.CoreFoundationLibrary)]
		unsafe extern static IntPtr CFSocketCreateConnectedToSocketSignature (IntPtr allocator, ref CFSocketSignature signature,
																	   nuint /*CFOptionFlags*/ callBackTypes,
																	   CFSocketCallBack callout,
																	   CFSocketContext* context, double timeout);
#endif

		public static CFSocket CreateConnectedToSocketSignature (AddressFamily family, SocketType type,
																 ProtocolType proto, IPEndPoint endpoint,
																 double timeout)
		{
			using (var address = new CFSocketAddress (endpoint)) {
				var sig = new CFSocketSignature (family, type, proto, address);
				return new CFSocket (sig, timeout);
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static CFSocketNativeHandle CFSocketGetNative (IntPtr handle);

		internal CFSocketNativeHandle GetNative ()
		{
			return CFSocketGetNative (Handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static nint CFSocketSetAddress (IntPtr handle, IntPtr address);

		public void SetAddress (IPAddress address, int port)
		{
			SetAddress (new IPEndPoint (address, port));
		}

		public void SetAddress (IPEndPoint endpoint)
		{
			EnableCallBacks (CFSocketCallBackType.AcceptCallBack);

			var flags = GetSocketFlags ();
			flags |= CFSocketFlags.AutomaticallyReenableAcceptCallBack;
			SetSocketFlags (flags);
			using (var address = new CFSocketAddress (endpoint)) {
				var error = (CFSocketError) (long) CFSocketSetAddress (Handle, address.Handle);
				if (error != CFSocketError.Success)
					throw new CFSocketException (error);
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		static extern IntPtr CFSocketCopyAddress (IntPtr socket);

		public IPEndPoint? Address {
			get {
				var data = CFSocketCopyAddress (Handle);
				return CFSocketAddress.EndPointFromAddressPtr (data, true);
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		static extern IntPtr CFSocketCopyPeerAddress (IntPtr socket);

		public IPEndPoint? RemoteAddress {
			get {
				var data = CFSocketCopyPeerAddress (Handle);
				return CFSocketAddress.EndPointFromAddressPtr (data, true);
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static CFSocketFlags CFSocketGetSocketFlags (IntPtr handle);

		public CFSocketFlags GetSocketFlags ()
		{
			return CFSocketGetSocketFlags (Handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFSocketSetSocketFlags (IntPtr handle, nuint /* CFOptionFlags */ flags);

		public void SetSocketFlags (CFSocketFlags flags)
		{
			CFSocketSetSocketFlags (Handle, (nuint) (ulong) flags);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFSocketDisableCallBacks (IntPtr handle, nuint /* CFOptionFlags */ types);

		public void DisableCallBacks (CFSocketCallBackType types)
		{
			CFSocketDisableCallBacks (Handle, (nuint) (ulong) types);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFSocketEnableCallBacks (IntPtr handle, nuint /* CFOptionFlags */ types);

		public void EnableCallBacks (CFSocketCallBackType types)
		{
			CFSocketEnableCallBacks (Handle, (nuint) (ulong) types);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static nint CFSocketSendData (IntPtr handle, IntPtr address, IntPtr data, double timeout);

		public void SendData (byte [] data, double timeout)
		{
			using (var buffer = new CFDataBuffer (data)) {
				var error = (CFSocketError) (long) CFSocketSendData (Handle, IntPtr.Zero, buffer.Handle, timeout);
				if (error != CFSocketError.Success)
					throw new CFSocketException (error);
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		public class CFSocketAcceptEventArgs : EventArgs {
			internal CFSocketNativeHandle SocketHandle {
				get;
				private set;
			}

			public IPEndPoint RemoteEndPoint {
				get;
				private set;
			}

			public CFSocketAcceptEventArgs (CFSocketNativeHandle handle, IPEndPoint remote)
			{
				this.SocketHandle = handle;
				this.RemoteEndPoint = remote;
			}

			public CFSocket CreateSocket ()
			{
				return new CFSocket (SocketHandle);
			}

			public override string ToString ()
			{
				return string.Format ("[CFSocketAcceptEventArgs: RemoteEndPoint={0}]", RemoteEndPoint);
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		public class CFSocketConnectEventArgs : EventArgs {
			public CFSocketError Result {
				get;
				private set;
			}

			public CFSocketConnectEventArgs (CFSocketError result)
			{
				this.Result = result;
			}

			public override string ToString ()
			{
				return string.Format ("[CFSocketConnectEventArgs: Result={0}]", Result);
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		public class CFSocketDataEventArgs : EventArgs {
			public IPEndPoint RemoteEndPoint {
				get;
				private set;
			}

			public byte [] Data {
				get;
				private set;
			}

			public CFSocketDataEventArgs (IPEndPoint remote, byte [] data)
			{
				this.RemoteEndPoint = remote;
				this.Data = data;
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		public class CFSocketReadEventArgs : EventArgs {
			public CFSocketReadEventArgs () { }
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		public class CFSocketWriteEventArgs : EventArgs {
			public CFSocketWriteEventArgs () { }
		}

		public event EventHandler<CFSocketAcceptEventArgs>? AcceptEvent;
		public event EventHandler<CFSocketConnectEventArgs>? ConnectEvent;
		public event EventHandler<CFSocketDataEventArgs>? DataEvent;
		public event EventHandler<CFSocketReadEventArgs>? ReadEvent;
		public event EventHandler<CFSocketWriteEventArgs>? WriteEvent;

		void OnAccepted (CFSocketAcceptEventArgs args)
		{
			if (AcceptEvent is not null)
				AcceptEvent (this, args);
		}

		void OnConnect (CFSocketConnectEventArgs args)
		{
			if (ConnectEvent is not null)
				ConnectEvent (this, args);
		}

		void OnData (CFSocketDataEventArgs args)
		{
			if (DataEvent is not null)
				DataEvent (this, args);
		}

		void OnRead (CFSocketReadEventArgs args)
		{
			if (ReadEvent is not null)
				ReadEvent (this, args);
		}

		void OnWrite (CFSocketWriteEventArgs args)
		{
			if (WriteEvent is not null)
				WriteEvent (this, args);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static nint CFSocketConnectToAddress (IntPtr handle, IntPtr address, double timeout);

		public void Connect (IPAddress address, int port, double timeout)
		{
			Connect (new IPEndPoint (address, port), timeout);
		}

		public void Connect (IPEndPoint endpoint, double timeout)
		{
			using (var address = new CFSocketAddress (endpoint)) {
				var error = (CFSocketError) (long) CFSocketConnectToAddress (Handle, address.Handle, timeout);
				if (error != CFSocketError.Success)
					throw new CFSocketException (error);
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFSocketInvalidate (IntPtr handle);

		public void Invalidate ()
		{
			Dispose ();
		}
	}
}
