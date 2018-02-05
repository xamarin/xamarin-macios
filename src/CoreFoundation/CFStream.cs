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

using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
#if XAMCORE_4_0
using CFNetwork;
#elif !WATCH
using CoreServices;
#endif
using ObjCRuntime;
using Foundation;

#if XAMCORE_2_0
using CFIndex = System.nint;
#endif

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
		
		public override string ToString ()
		{
			if (copyDescription != IntPtr.Zero) {
				var ptr = CFReadStreamRef_InvokeCopyDescription (copyDescription, Info);
				if (ptr != IntPtr.Zero) {
					// Copy* -> so we must not retain again
					using (var s = new CFString (ptr, true))
						return s.ToString ();
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
			return ((RetainDelegate)Marshal.GetDelegateForFunctionPointer (retain, typeof (RetainDelegate))) (info);
		}
		
		[MonoNativeFunctionWrapper]
		delegate void ReleaseDelegate (IntPtr info);

		static void CFReadStreamRef_InvokeRelease (IntPtr release, IntPtr info)
		{
			((ReleaseDelegate)Marshal.GetDelegateForFunctionPointer (release, typeof (ReleaseDelegate))) (info);
		}
		
		[MonoNativeFunctionWrapper]
		delegate IntPtr CopyDescriptionDelegate (IntPtr info);

		static IntPtr CFReadStreamRef_InvokeCopyDescription (IntPtr copyDescription, IntPtr info)
		{
			return ((CopyDescriptionDelegate)Marshal.GetDelegateForFunctionPointer (copyDescription, typeof (CopyDescriptionDelegate))) (info);
		}
		
		[MonoNativeFunctionWrapper]
		delegate void CallbackDelegate (IntPtr stream, IntPtr /* CFStreamEventType */ eventType, IntPtr info);

		static void CFReadStreamRef_InvokeCallback (IntPtr callback, IntPtr stream, CFStreamEventType eventType, IntPtr info)
		{
			((CallbackDelegate)Marshal.GetDelegateForFunctionPointer (callback, typeof (CallbackDelegate))) (stream, (IntPtr) eventType, info);
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

	public abstract class CFStream : CFType, INativeObject, IDisposable {
		IntPtr handle;
		GCHandle gch;
		CFRunLoop loop;
		NSString loopMode;
		bool open, closed;

		#region Stream Constructors

		[DllImport (Constants.CoreFoundationLibrary)]
		internal extern static void CFStreamCreatePairWithSocket (/* CFAllocatorRef */ IntPtr allocator, CFSocketNativeHandle sock,
			/* CFReadStreamRef* */ out IntPtr readStream, /* CFWriteStreamRef* */ out IntPtr writeStream);

		public static void CreatePairWithSocket (CFSocket socket, out CFReadStream readStream,
		                                         out CFWriteStream writeStream)
		{
			if (socket == null)
				throw new ArgumentNullException ("socket");

			IntPtr read, write;
			CFStreamCreatePairWithSocket (IntPtr.Zero, socket.GetNative (), out read, out write);
			readStream = new CFReadStream (read);
			writeStream = new CFWriteStream (write);
		}

		[DllImport (Constants.CFNetworkLibrary)]
		internal extern static void CFStreamCreatePairWithPeerSocketSignature (/* CFAllocatorRef */ IntPtr allocator, 
			/* CFSocketSignature* */ ref CFSocketSignature sig, 
			/* CFReadStreamRef* */ out IntPtr readStream, /* CFWriteStreamRef* */ out IntPtr writeStream);

		public static void CreatePairWithPeerSocketSignature (AddressFamily family, SocketType type,
		                                                      ProtocolType proto, IPEndPoint endpoint,
		                                                      out CFReadStream readStream,
		                                                      out CFWriteStream writeStream)
		{
			using (var address = new CFSocketAddress (endpoint)) {
				var sig = new CFSocketSignature (family, type, proto, address);
				IntPtr read, write;
				CFStreamCreatePairWithPeerSocketSignature (IntPtr.Zero, ref sig, out read, out write);
				readStream = new CFReadStream (read);
				writeStream = new CFWriteStream (write);
			}
		}

#if !WATCH
		// CFSocketStream.h in CFNetwork.framework (not CoreFoundation)
		[DllImport (Constants.CFNetworkLibrary)]
		internal extern static void CFStreamCreatePairWithSocketToCFHost (
			/* CFAllocatorRef __nullable */ IntPtr allocator, 
			/* CFHostRef __nonnull */ IntPtr host, /* SInt32 */ int port,
			/* CFReadStreamRef __nullable * __nullable */ out IntPtr readStream,
			/* CFWriteStreamRef __nullable * __nullable */ out IntPtr writeStream);

		public static void CreatePairWithSocketToHost (IPEndPoint endpoint,
		                                               out CFReadStream readStream,
		                                               out CFWriteStream writeStream)
		{
			using (var host = CFHost.Create (endpoint)) {
				IntPtr read, write;
				CFStreamCreatePairWithSocketToCFHost (IntPtr.Zero, host.Handle, endpoint.Port, out read, out write);
				// API can return null streams
				readStream = read == IntPtr.Zero ? null : new CFReadStream (read);
				writeStream = write == IntPtr.Zero ? null : new CFWriteStream (write);
			}
		}
#endif

		[DllImport (Constants.CFNetworkLibrary)]
		internal extern static void CFStreamCreatePairWithSocketToHost (/* CFAllocatorRef */ IntPtr allocator, 
			/* CFStringRef */ IntPtr host, /* UInt32 */ int port,
			/* CFReadStreamRef* */ out IntPtr readStream, /* CFWriteStreamRef* */ out IntPtr writeStream);

		public static void CreatePairWithSocketToHost (string host, int port,
		                                               out CFReadStream readStream,
		                                               out CFWriteStream writeStream)
		{
			using (var str = new CFString (host)) {
				IntPtr read, write;
				CFStreamCreatePairWithSocketToHost (
					IntPtr.Zero, str.Handle, port, out read, out write);
				// API not annotated (yet?) but it's safe to bet it match CFStreamCreatePairWithSocketToCFHost
				readStream = read == IntPtr.Zero ? null : new CFReadStream (read);
				writeStream = write == IntPtr.Zero ? null : new CFWriteStream (write);
			}
		}
#if !WATCH
		// CFHTTPStream.h in CFNetwork.framework (not CoreFoundation)
		[Deprecated (PlatformName.iOS, 9, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 11)]
		[DllImport (Constants.CFNetworkLibrary)]
		internal extern static /* CFReadStreamRef __nonnull */ IntPtr CFReadStreamCreateForHTTPRequest (
			/* CFAllocatorRef __nullable */ IntPtr alloc, /* CFHTTPMessageRef __nonnull */ IntPtr request);

		[Deprecated (PlatformName.iOS, 9, 0, message : "Use 'NSUrlSession'.")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message : "Use 'NSUrlSession'.")]
		public static CFHTTPStream CreateForHTTPRequest (CFHTTPMessage request)
		{
			if (request == null)
				throw new ArgumentNullException ("request");

			var handle = CFReadStreamCreateForHTTPRequest (IntPtr.Zero, request.Handle);
			return new CFHTTPStream (handle);
		}

		// CFHTTPStream.h in CFNetwork.framework (not CoreFoundation)
		[DllImport (Constants.CFNetworkLibrary)]
		internal extern static /* CFReadStreamRef __nonnull */ IntPtr CFReadStreamCreateForStreamedHTTPRequest (
			/* CFAllocatorRef __nullable */ IntPtr alloc, /* CFHTTPMessageRef __nonnull */ IntPtr requestHeaders,
			/* CFReadStreamRef __nonnull */ IntPtr requestBody);

		public static CFHTTPStream CreateForStreamedHTTPRequest (CFHTTPMessage request, CFReadStream body)
		{
			if (request == null)
				throw new ArgumentNullException ("request");
			if (body == null)
				throw new ArgumentNullException ("body");

			var handle = CFReadStreamCreateForStreamedHTTPRequest (IntPtr.Zero, request.Handle, body.Handle);
			return new CFHTTPStream (handle);
		}

		public static CFHTTPStream CreateForStreamedHTTPRequest (CFHTTPMessage request, NSInputStream body)
		{
			if (request == null)
				throw new ArgumentNullException ("request");
			if (body == null)
				throw new ArgumentNullException ("body");

			var handle = CFReadStreamCreateForStreamedHTTPRequest (IntPtr.Zero, request.Handle, body.Handle);
			return new CFHTTPStream (handle);
		}
#endif

		[DllImport (Constants.CFNetworkLibrary)]
		internal extern static void CFStreamCreateBoundPair (/* CFAllocatorRef */ IntPtr alloc, 
			/* CFReadStreamRef* */ out IntPtr readStream, /* CFWriteStreamRef* */ out IntPtr writeStream,
			/* CFIndex */ nint transferBufferSize);

		public static void CreateBoundPair (out CFReadStream readStream, out CFWriteStream writeStream, nint bufferSize)
		{
			IntPtr read, write;
			CFStreamCreateBoundPair (IntPtr.Zero, out read, out write, bufferSize);
			readStream = new CFReadStream (read);
			writeStream = new CFWriteStream (write);
		}

		#endregion

		#region Stream API

		public abstract CFException GetError ();

		protected void CheckError ()
		{
			var exc = GetError ();
			if (exc != null)
				throw exc;
		}

		public void Open ()
		{
			if (open || closed)
				throw new InvalidOperationException ();
			CheckHandle ();
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
			CheckHandle ();
			if (loop != null) {
				DoSetClient (null, 0, IntPtr.Zero);
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
			CheckHandle ();
			return DoGetStatus ();
		}

		protected abstract CFStreamStatus DoGetStatus ();

		internal IntPtr GetProperty (NSString name)
		{
			CheckHandle ();
			return DoGetProperty (name);
		}

		protected abstract IntPtr DoGetProperty (NSString name);

		protected abstract bool DoSetProperty (NSString name, INativeObject value);

		internal void SetProperty (NSString name, INativeObject value)
		{
			CheckHandle ();
			if (DoSetProperty (name, value))
				return;
			throw new InvalidOperationException (string.Format (
				"Cannot set property '{0}' on {1}.", name, GetType ().Name)
			);
		}

		#endregion

		#region Events

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

		public event EventHandler<StreamEventArgs> OpenCompletedEvent;
		public event EventHandler<StreamEventArgs> HasBytesAvailableEvent;
		public event EventHandler<StreamEventArgs> CanAcceptBytesEvent;
		public event EventHandler<StreamEventArgs> ErrorEvent;
		public event EventHandler<StreamEventArgs> ClosedEvent;

		protected virtual void OnOpenCompleted (StreamEventArgs args)
		{
			var e = OpenCompletedEvent;
			if (e != null)
				e (this, args);
		}

		protected virtual void OnHasBytesAvailableEvent (StreamEventArgs args)
		{
			var e = HasBytesAvailableEvent;
			if (e != null)
				e (this, args);
		}

		protected virtual void OnCanAcceptBytesEvent (StreamEventArgs args)
		{
			var e = CanAcceptBytesEvent;
			if (e != null)
				e (this, args);
		}

		protected virtual void OnErrorEvent (StreamEventArgs args)
		{
			var e = ErrorEvent;
			if (e != null)
				e (this, args);
		}

		protected virtual void OnClosedEvent (StreamEventArgs args)
		{
			var e = ClosedEvent;
			if (e != null)
				e (this, args);
		}

		#endregion

		protected abstract void ScheduleWithRunLoop (CFRunLoop loop, NSString mode);

		protected abstract void UnscheduleFromRunLoop (CFRunLoop loop, NSString mode);

		protected delegate void CFStreamCallback (IntPtr s, nint type, IntPtr info);

		[MonoPInvokeCallback (typeof(CFStreamCallback))]
		static void OnCallback (IntPtr s, nint type, IntPtr info)
		{
			var stream = GCHandle.FromIntPtr (info).Target as CFStream;
			stream.OnCallback ((CFStreamEventType) (long) type);
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
			if (open || closed || (loop != null))
				throw new InvalidOperationException ();
			CheckHandle ();

			loop = runLoop;
			loopMode = runLoopMode;

			var ctx = new CFStreamClientContext ();
			ctx.Info = GCHandle.ToIntPtr (gch);

			var args = CFStreamEventType.OpenCompleted |
				CFStreamEventType.CanAcceptBytes | CFStreamEventType.HasBytesAvailable |
				CFStreamEventType.CanAcceptBytes | CFStreamEventType.ErrorOccurred |
				CFStreamEventType.EndEncountered;

			var ptr = Marshal.AllocHGlobal (Marshal.SizeOf (typeof (CFStreamClientContext)));
			try {
				Marshal.StructureToPtr (ctx, ptr, false);
				if (!DoSetClient (OnCallback, (nint) (long) args, ptr))
					throw new InvalidOperationException ("Stream does not support async events.");
			} finally {
				Marshal.FreeHGlobal (ptr);
			}

			ScheduleWithRunLoop (runLoop, runLoopMode);
		}

		protected abstract bool DoSetClient (CFStreamCallback callback, CFIndex eventTypes,
		                                     IntPtr context);

		protected CFStream (IntPtr handle)
		{
			this.handle = handle;
			gch = GCHandle.Alloc (this);
		}

		protected void CheckHandle ()
		{
			if (handle == IntPtr.Zero)
				throw new ObjectDisposedException (GetType ().Name);
		}

		~CFStream ()
		{
			Dispose (false);
		}
		
		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		public IntPtr Handle {
			get { return handle; }
		}
		
		protected virtual void Dispose (bool disposing)
		{
			if (disposing) {
				Close ();
				if (gch.IsAllocated)
					gch.Free ();
			}
			if (handle != IntPtr.Zero) {
				CFObject.CFRelease (handle);
				handle = IntPtr.Zero;
			}
		}

		[iOS (7,0)][Mac (10,9)]
		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFReadStreamSetDispatchQueue (/* CFReadStreamRef */ IntPtr stream, /* dispatch_queue_t */ IntPtr queue);
		
		[iOS (7,0)][Mac (10,9)]
		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFWriteStreamSetDispatchQueue (/* CFWriteStreamRef */ IntPtr stream, /* dispatch_queue_t */ IntPtr queue);

		[iOS (7,0)][Mac (10,9)]
		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* dispatch_queue_t */ IntPtr CFReadStreamCopyDispatchQueue (/* CFReadStreamRef */ IntPtr stream);

		[iOS (7,0)][Mac (10,9)]
		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* dispatch_queue_t */ IntPtr CFWriteStreamCopyDispatchQueue (/* CFWriteStreamRef */ IntPtr stream);
	
		[iOS (7,0)][Mac (10,9)]
		public DispatchQueue ReadDispatchQueue {
			get {
				return new DispatchQueue (CFReadStreamCopyDispatchQueue (handle));
			}
			set {
				CFReadStreamSetDispatchQueue (handle, value == null ? IntPtr.Zero : value.Handle);
			}
		}

		[iOS (7,0)][Mac (10,9)]
		public DispatchQueue WriteDispatchQueue {
			get {
				return new DispatchQueue (CFWriteStreamCopyDispatchQueue (handle));
			}
			set {
				CFWriteStreamSetDispatchQueue (handle, value == null ? IntPtr.Zero : value.Handle);
			}
		}
	}
}
