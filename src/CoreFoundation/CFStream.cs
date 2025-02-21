// 
// CFStream.cs:
//
// If you add or change any of the CFStream convenience constructors, update
// the same code in NSStream.
//
// Authors:
//		Martin Baulig <martin.baulig@gmail.com>
//		Rolf Bjarne Kvinge <rolf@xamarin.com>
//     
// Copyright (C) 2012, 2014 Xamarin, Inc.
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
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using CFNetwork;
using ObjCRuntime;
using Foundation;

using CFIndex = System.IntPtr;

namespace CoreFoundation {

	// CFOptionFlags
	[Flags]
	[Native] // System/Library/Frameworks/Foundation.framework/Headers/NSStream.h
	public enum CFStreamEventType : ulong {
		None = 0,
		OpenCompleted = 1,
		HasBytesAvailable = 2,
		CanAcceptBytes = 4,
		ErrorOccurred = 8,
		EndEncountered = 16
	}

	// NSStream.h
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	[StructLayout (LayoutKind.Sequential)]
	public struct CFStreamClientContext {
		public nint Version; // CFIndex
		public /* void*/ IntPtr Info;
		IntPtr retain;
		IntPtr release;
		IntPtr copyDescription;

		public void Retain ()
		{
			if (retain == IntPtr.Zero || Info == IntPtr.Zero)
				return;

			CFReadStreamRef_InvokeRetain (retain, Info);
		}

		public void Release ()
		{
			if (release == IntPtr.Zero || Info == IntPtr.Zero)
				return;

			CFReadStreamRef_InvokeRelease (release, Info);
		}

		public override string? ToString ()
		{
			if (copyDescription != IntPtr.Zero) {
				var ptr = CFReadStreamRef_InvokeCopyDescription (copyDescription, Info);
				if (ptr != IntPtr.Zero) {
					// Copy* -> so we must not retain again
					return CFString.FromHandle (ptr, true);
				}
			}
			return base.ToString ();
		}

		internal void Invoke (IntPtr callback, IntPtr stream, CFStreamEventType eventType)
		{
			if (callback == IntPtr.Zero)
				return;

			CFReadStreamRef_InvokeCallback (callback, stream, eventType, Info);
		}

		[MonoNativeFunctionWrapper]
		delegate IntPtr RetainDelegate (IntPtr info);

		static IntPtr CFReadStreamRef_InvokeRetain (IntPtr retain, IntPtr info)
		{
			return ((RetainDelegate) Marshal.GetDelegateForFunctionPointer (retain, typeof (RetainDelegate))) (info);
		}

		[MonoNativeFunctionWrapper]
		delegate void ReleaseDelegate (IntPtr info);

		static void CFReadStreamRef_InvokeRelease (IntPtr release, IntPtr info)
		{
			((ReleaseDelegate) Marshal.GetDelegateForFunctionPointer (release, typeof (ReleaseDelegate))) (info);
		}

		[MonoNativeFunctionWrapper]
		delegate IntPtr CopyDescriptionDelegate (IntPtr info);

		static IntPtr CFReadStreamRef_InvokeCopyDescription (IntPtr copyDescription, IntPtr info)
		{
			return ((CopyDescriptionDelegate) Marshal.GetDelegateForFunctionPointer (copyDescription, typeof (CopyDescriptionDelegate))) (info);
		}

		[MonoNativeFunctionWrapper]
		delegate void CallbackDelegate (IntPtr stream, IntPtr /* CFStreamEventType */ eventType, IntPtr info);

		static void CFReadStreamRef_InvokeCallback (IntPtr callback, IntPtr stream, CFStreamEventType eventType, IntPtr info)
		{
			((CallbackDelegate) Marshal.GetDelegateForFunctionPointer (callback, typeof (CallbackDelegate))) (stream, (IntPtr) eventType, info);
		}
	}

	// CFIndex
	[Native] // System/Library/Frameworks/CoreFoundation.framework/Headers/CFStream.h
	public enum CFStreamStatus : long {
		NotOpen = 0,
		Opening,
		Open,
		Reading,
		Writing,
		AtEnd,
		Closed,
		Error
	}

	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public abstract class CFStream : CFType {
		GCHandle gch;
		CFRunLoop? loop;
		NSString? loopMode;
		bool open, closed;

		#region Stream Constructors

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("tvos15.0", Constants.UseNetworkInstead)]
		[ObsoletedOSPlatform ("maccatalyst15.0", Constants.UseNetworkInstead)]
		[ObsoletedOSPlatform ("macos12.0", Constants.UseNetworkInstead)]
		[ObsoletedOSPlatform ("ios15.0", Constants.UseNetworkInstead)]
		[DllImport (Constants.CoreFoundationLibrary)]
		internal unsafe extern static void CFStreamCreatePairWithSocket (/* CFAllocatorRef */ IntPtr allocator, CFSocketNativeHandle sock,
			/* CFReadStreamRef* */ IntPtr* readStream, /* CFWriteStreamRef* */ IntPtr* writeStream);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("tvos15.0", Constants.UseNetworkInstead)]
		[ObsoletedOSPlatform ("maccatalyst15.0", Constants.UseNetworkInstead)]
		[ObsoletedOSPlatform ("macos12.0", Constants.UseNetworkInstead)]
		[ObsoletedOSPlatform ("ios15.0", Constants.UseNetworkInstead)]
		public static void CreatePairWithSocket (CFSocket socket, out CFReadStream readStream,
												 out CFWriteStream writeStream)
		{
			if (socket is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (socket));

			IntPtr read, write;
			unsafe {
				CFStreamCreatePairWithSocket (IntPtr.Zero, socket.GetNative (), &read, &write);
			}
			readStream = new CFReadStream (read, true);
			writeStream = new CFWriteStream (write, true);
		}

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("tvos15.0", Constants.UseNetworkInstead)]
		[ObsoletedOSPlatform ("maccatalyst15.0", Constants.UseNetworkInstead)]
		[ObsoletedOSPlatform ("macos12.0", Constants.UseNetworkInstead)]
		[ObsoletedOSPlatform ("ios15.0", Constants.UseNetworkInstead)]
		[DllImport (Constants.CoreFoundationLibrary)]
		internal unsafe extern static void CFStreamCreatePairWithPeerSocketSignature (/* CFAllocatorRef */ IntPtr allocator,
			/* CFSocketSignature* */ CFSocketSignature* sig,
			/* CFReadStreamRef* */ IntPtr* readStream, /* CFWriteStreamRef* */ IntPtr* writeStream);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("tvos15.0", Constants.UseNetworkInstead)]
		[ObsoletedOSPlatform ("maccatalyst15.0", Constants.UseNetworkInstead)]
		[ObsoletedOSPlatform ("macos12.0", Constants.UseNetworkInstead)]
		[ObsoletedOSPlatform ("ios15.0", Constants.UseNetworkInstead)]
		public static void CreatePairWithPeerSocketSignature (AddressFamily family, SocketType type,
															  ProtocolType proto, IPEndPoint endpoint,
															  out CFReadStream readStream,
															  out CFWriteStream writeStream)
		{
			using (var address = new CFSocketAddress (endpoint)) {
				var sig = new CFSocketSignature (family, type, proto, address);
				IntPtr read, write;
				unsafe {
					CFStreamCreatePairWithPeerSocketSignature (IntPtr.Zero, &sig, &read, &write);
				}
				readStream = new CFReadStream (read, true);
				writeStream = new CFWriteStream (write, true);
			}
		}

		// CFSocketStream.h in CFNetwork.framework (not CoreFoundation)
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("tvos15.0", Constants.UseNetworkInstead)]
		[ObsoletedOSPlatform ("maccatalyst15.0", Constants.UseNetworkInstead)]
		[ObsoletedOSPlatform ("macos12.0", Constants.UseNetworkInstead)]
		[ObsoletedOSPlatform ("ios15.0", Constants.UseNetworkInstead)]
		[DllImport (Constants.CFNetworkLibrary)]
		internal unsafe extern static void CFStreamCreatePairWithSocketToCFHost (
			/* CFAllocatorRef __nullable */ IntPtr allocator,
			/* CFHostRef __nonnull */ IntPtr host, /* SInt32 */ int port,
			/* CFReadStreamRef __nullable * __nullable */ IntPtr* readStream,
			/* CFWriteStreamRef __nullable * __nullable */ IntPtr* writeStream);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("tvos15.0", Constants.UseNetworkInstead)]
		[ObsoletedOSPlatform ("maccatalyst15.0", Constants.UseNetworkInstead)]
		[ObsoletedOSPlatform ("macos12.0", Constants.UseNetworkInstead)]
		[ObsoletedOSPlatform ("ios15.0", Constants.UseNetworkInstead)]
		public static void CreatePairWithSocketToHost (IPEndPoint endpoint,
													   out CFReadStream? readStream,
													   out CFWriteStream? writeStream)
		{
			using (var host = CFHost.Create (endpoint)) {
				IntPtr read, write;
				unsafe {
					CFStreamCreatePairWithSocketToCFHost (IntPtr.Zero, host.Handle, endpoint.Port, &read, &write);
				}
				// API can return null streams
				readStream = read == IntPtr.Zero ? null : new CFReadStream (read, true);
				writeStream = write == IntPtr.Zero ? null : new CFWriteStream (write, true);
			}
		}

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("tvos15.0", Constants.UseNetworkInstead)]
		[ObsoletedOSPlatform ("maccatalyst15.0", Constants.UseNetworkInstead)]
		[ObsoletedOSPlatform ("macos12.0", Constants.UseNetworkInstead)]
		[ObsoletedOSPlatform ("ios15.0", Constants.UseNetworkInstead)]
		[DllImport (Constants.CoreFoundationLibrary)]
		unsafe extern static void CFStreamCreatePairWithSocketToHost (/* CFAllocatorRef */ IntPtr allocator,
			/* CFStringRef */ IntPtr host, /* UInt32 */ int port,
			/* CFReadStreamRef* */ IntPtr* readStream, /* CFWriteStreamRef* */ IntPtr* writeStream);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("tvos15.0", Constants.UseNetworkInstead)]
		[ObsoletedOSPlatform ("maccatalyst15.0", Constants.UseNetworkInstead)]
		[ObsoletedOSPlatform ("macos12.0", Constants.UseNetworkInstead)]
		[ObsoletedOSPlatform ("ios15.0", Constants.UseNetworkInstead)]
		public static void CreatePairWithSocketToHost (string host, int port,
													   out CFReadStream? readStream,
													   out CFWriteStream? writeStream)
		{
			using (var str = new CFString (host)) {
				IntPtr read, write;
				unsafe {
					CFStreamCreatePairWithSocketToHost (IntPtr.Zero, str.Handle, port, &read, &write);
				}
				// API not annotated (yet?) but it's safe to bet it match CFStreamCreatePairWithSocketToCFHost
				readStream = read == IntPtr.Zero ? null : new CFReadStream (read, true);
				writeStream = write == IntPtr.Zero ? null : new CFWriteStream (write, true);
			}
		}
		// CFHTTPStream.h in CFNetwork.framework (not CoreFoundation)
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.11")]
		[ObsoletedOSPlatform ("ios9.0")]
		[ObsoletedOSPlatform ("maccatalyst13.0")]
		[ObsoletedOSPlatform ("tvos9.0")]
		[DllImport (Constants.CFNetworkLibrary)]
		internal extern static /* CFReadStreamRef __nonnull */ IntPtr CFReadStreamCreateForHTTPRequest (
			/* CFAllocatorRef __nullable */ IntPtr alloc, /* CFHTTPMessageRef __nonnull */ IntPtr request);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.11", "Use 'NSUrlSession' instead.")]
		[ObsoletedOSPlatform ("ios9.0", "Use 'NSUrlSession' instead.")]
		[ObsoletedOSPlatform ("maccatalyst13.0", "Use 'NSUrlSession' instead.")]
		[ObsoletedOSPlatform ("tvos9.0", "Use 'NSUrlSession' instead.")]
		public static CFHTTPStream CreateForHTTPRequest (CFHTTPMessage request)
		{
			if (request is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (request));

			var handle = CFReadStreamCreateForHTTPRequest (IntPtr.Zero, request.Handle);
			return new CFHTTPStream (handle, true);
		}

		// CFHTTPStream.h in CFNetwork.framework (not CoreFoundation)
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.11")]
		[ObsoletedOSPlatform ("ios9.0")]
		[ObsoletedOSPlatform ("maccatalyst13.0")]
		[ObsoletedOSPlatform ("tvos9.0")]
		[DllImport (Constants.CFNetworkLibrary)]
		internal extern static /* CFReadStreamRef __nonnull */ IntPtr CFReadStreamCreateForStreamedHTTPRequest (
			/* CFAllocatorRef __nullable */ IntPtr alloc, /* CFHTTPMessageRef __nonnull */ IntPtr requestHeaders,
			/* CFReadStreamRef __nonnull */ IntPtr requestBody);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.11", "Use 'NSUrlSession' instead.")]
		[ObsoletedOSPlatform ("ios9.0", "Use 'NSUrlSession' instead.")]
		[ObsoletedOSPlatform ("maccatalyst13.0", "Use 'NSUrlSession' instead.")]
		[ObsoletedOSPlatform ("tvos9.0", "Use 'NSUrlSession' instead.")]
		public static CFHTTPStream CreateForStreamedHTTPRequest (CFHTTPMessage request, CFReadStream body)
		{
			if (request is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (request));
			if (body is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (body));

			var handle = CFReadStreamCreateForStreamedHTTPRequest (IntPtr.Zero, request.Handle, body.Handle);
			return new CFHTTPStream (handle, true);
		}

		public static CFHTTPStream CreateForStreamedHTTPRequest (CFHTTPMessage request, NSInputStream body)
		{
			if (request is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (request));
			if (body is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (body));

			var handle = CFReadStreamCreateForStreamedHTTPRequest (IntPtr.Zero, request.Handle, body.Handle);
			return new CFHTTPStream (handle, true);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		unsafe internal extern static void CFStreamCreateBoundPair (/* CFAllocatorRef */ IntPtr alloc,
			/* CFReadStreamRef* */ IntPtr* readStream, /* CFWriteStreamRef* */ IntPtr* writeStream,
			/* CFIndex */ nint transferBufferSize);

		public static void CreateBoundPair (out CFReadStream readStream, out CFWriteStream writeStream, nint bufferSize)
		{
			IntPtr read, write;
			unsafe {
				CFStreamCreateBoundPair (IntPtr.Zero, &read, &write, bufferSize);
			}
			readStream = new CFReadStream (read, true);
			writeStream = new CFWriteStream (write, true);
		}

		#endregion

		#region Stream API

		public abstract CFException? GetError ();

		protected void CheckError ()
		{
			var exc = GetError ();
			if (exc is not null)
				throw exc;
		}

		public void Open ()
		{
			if (open || closed)
				throw new InvalidOperationException ();
			GetCheckedHandle ();
			if (!DoOpen ()) {
				CheckError ();
				throw new InvalidOperationException ();
			}
			open = true;
		}

		protected abstract bool DoOpen ();

		public void Close ()
		{
			if (!open)
				return;
			GetCheckedHandle ();
			if (loop is not null) {
				unsafe {
					DoSetClient ((delegate* unmanaged<IntPtr, nint, IntPtr, void>) null, (CFIndex) 0, IntPtr.Zero);
				}
				UnscheduleFromRunLoop (loop, loopMode);
				loop = null;
				loopMode = null;
			}
			try {
				DoClose ();
			} finally {
				open = false;
				closed = true;
			}
		}

		protected abstract void DoClose ();

		public CFStreamStatus GetStatus ()
		{
			GetCheckedHandle ();
			return DoGetStatus ();
		}

		protected abstract CFStreamStatus DoGetStatus ();

		internal IntPtr GetProperty (NSString name)
		{
			GetCheckedHandle ();
			return DoGetProperty (name);
		}

		protected abstract IntPtr DoGetProperty (NSString name);

		protected abstract bool DoSetProperty (NSString name, INativeObject? value);

		internal void SetProperty (NSString name, INativeObject? value)
		{
			GetCheckedHandle ();
			if (DoSetProperty (name, value))
				return;
			throw new InvalidOperationException (string.Format (
				"Cannot set property '{0}' on {1}.", name, GetType ().Name)
			);
		}

		#endregion

		#region Events

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		public class StreamEventArgs : EventArgs {
			public CFStreamEventType EventType {
				get;
				private set;
			}

			public StreamEventArgs (CFStreamEventType type)
			{
				this.EventType = type;
			}

			public override string ToString ()
			{
				return string.Format ("[StreamEventArgs: EventType={0}]", EventType);
			}
		}

		public event EventHandler<StreamEventArgs>? OpenCompletedEvent;
		public event EventHandler<StreamEventArgs>? HasBytesAvailableEvent;
		public event EventHandler<StreamEventArgs>? CanAcceptBytesEvent;
		public event EventHandler<StreamEventArgs>? ErrorEvent;
		public event EventHandler<StreamEventArgs>? ClosedEvent;

		protected virtual void OnOpenCompleted (StreamEventArgs args)
		{
			var e = OpenCompletedEvent;
			if (e is not null)
				e (this, args);
		}

		protected virtual void OnHasBytesAvailableEvent (StreamEventArgs args)
		{
			var e = HasBytesAvailableEvent;
			if (e is not null)
				e (this, args);
		}

		protected virtual void OnCanAcceptBytesEvent (StreamEventArgs args)
		{
			var e = CanAcceptBytesEvent;
			if (e is not null)
				e (this, args);
		}

		protected virtual void OnErrorEvent (StreamEventArgs args)
		{
			var e = ErrorEvent;
			if (e is not null)
				e (this, args);
		}

		protected virtual void OnClosedEvent (StreamEventArgs args)
		{
			var e = ClosedEvent;
			if (e is not null)
				e (this, args);
		}

		#endregion

		protected abstract void ScheduleWithRunLoop (CFRunLoop loop, NSString? mode);

		protected abstract void UnscheduleFromRunLoop (CFRunLoop loop, NSString? mode);

		protected delegate void CFStreamCallback (IntPtr s, nint type, IntPtr info);

		[UnmanagedCallersOnly]
		static void NativeCallback (IntPtr s, nint type, IntPtr info)
		{
			var stream = GCHandle.FromIntPtr (info).Target as CFStream;
			stream?.OnCallback ((CFStreamEventType) (long) type);
		}

		protected virtual void OnCallback (CFStreamEventType type)
		{
			var args = new StreamEventArgs (type);
			switch (type) {
			case CFStreamEventType.OpenCompleted:
				OnOpenCompleted (args);
				break;
			case CFStreamEventType.CanAcceptBytes:
				OnCanAcceptBytesEvent (args);
				break;
			case CFStreamEventType.HasBytesAvailable:
				OnHasBytesAvailableEvent (args);
				break;
			case CFStreamEventType.ErrorOccurred:
				OnErrorEvent (args);
				break;
			case CFStreamEventType.EndEncountered:
				OnClosedEvent (args);
				break;
			}
		}

		public void EnableEvents (CFRunLoop runLoop, NSString runLoopMode)
		{
			if (open || closed || (loop is not null))
				throw new InvalidOperationException ();
			GetCheckedHandle ();

			loop = runLoop;
			loopMode = runLoopMode;

			if (!gch.IsAllocated)
				gch = GCHandle.Alloc (this);

			var ctx = new CFStreamClientContext ();
			ctx.Info = GCHandle.ToIntPtr (gch);

			var args = CFStreamEventType.OpenCompleted |
				CFStreamEventType.CanAcceptBytes | CFStreamEventType.HasBytesAvailable |
				CFStreamEventType.CanAcceptBytes | CFStreamEventType.ErrorOccurred |
				CFStreamEventType.EndEncountered;

			var ptr = Marshal.AllocHGlobal (Marshal.SizeOf<CFStreamClientContext> ());
			try {
				Marshal.StructureToPtr<CFStreamClientContext> (ctx, ptr, false);
				bool clientSet;
				unsafe {
					clientSet = DoSetClient (&NativeCallback, (CFIndex) (long) args, ptr) != 0;
				}
				if (!clientSet)
					throw new InvalidOperationException ("Stream does not support async events.");
			} finally {
				Marshal.FreeHGlobal (ptr);
			}

			ScheduleWithRunLoop (runLoop, runLoopMode);
		}

#if !XAMCORE_5_0
		[Obsolete ("Use the other overload.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		protected abstract bool DoSetClient (CFStreamCallback? callback, CFIndex eventTypes,
											 IntPtr context);
#endif

#if XAMCORE_5_0
		unsafe protected abstract byte DoSetClient (delegate* unmanaged<IntPtr, nint, IntPtr, void> callback, CFIndex eventTypes, IntPtr context);
#else
		unsafe protected virtual byte DoSetClient (delegate* unmanaged<IntPtr, nint, IntPtr, void> callback, CFIndex eventTypes, IntPtr context)
		{
			throw new InvalidOperationException ($"This method must be overridden (and don't call base)");
		}
#endif // XAMCORE_5_0

		[Preserve (Conditional = true)]
		protected CFStream (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				Close ();
				if (gch.IsAllocated)
					gch.Free ();
			}
			base.Dispose (disposing);
		}

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFReadStreamSetDispatchQueue (/* CFReadStreamRef */ IntPtr stream, /* dispatch_queue_t */ IntPtr queue);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFWriteStreamSetDispatchQueue (/* CFWriteStreamRef */ IntPtr stream, /* dispatch_queue_t */ IntPtr queue);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* dispatch_queue_t */ IntPtr CFReadStreamCopyDispatchQueue (/* CFReadStreamRef */ IntPtr stream);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* dispatch_queue_t */ IntPtr CFWriteStreamCopyDispatchQueue (/* CFWriteStreamRef */ IntPtr stream);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		public DispatchQueue ReadDispatchQueue {
			get {
				return new DispatchQueue (CFReadStreamCopyDispatchQueue (Handle), true);
			}
			set {
				CFReadStreamSetDispatchQueue (Handle, value.GetHandle ());
			}
		}

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		public DispatchQueue WriteDispatchQueue {
			get {
				return new DispatchQueue (CFWriteStreamCopyDispatchQueue (Handle), true);
			}
			set {
				CFWriteStreamSetDispatchQueue (Handle, value.GetHandle ());
			}
		}
	}
}
