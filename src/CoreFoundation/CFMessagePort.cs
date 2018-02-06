//
// CFMessagePort.cs: CFMessagePort is a wrapper around two native Mach ports with bidirectional communication support
//
// Authors:
//   Oleg Demchenko (oleg.demchenko@xamarin.com)
//
// Copyright 2015 Xamarin Inc
//
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;

using dispatch_queue_t = System.IntPtr;

namespace CoreFoundation {

	// untyped enum from CFMessagePort.h
	// used as a return value of type SInt32 (always 4 bytes)
	public enum CFMessagePortSendRequestStatus {
		Success = 0,
		SendTimeout = -1,
		ReceiveTimeout = -2,
		IsInvalid = -3,
		TransportError = -4,
		BecameInvalidError = -5
	}

	internal class CFMessagePortContext {

		public Func<INativeObject> Retain { get; set; }

		public Action Release { get; set; }

		public Func<NSString> CopyDescription { get; set; }
	}

	public class CFMessagePort : INativeObject, IDisposable {

		// CFMessagePortContext
		[StructLayout (LayoutKind.Sequential)]
		struct ContextProxy {
			/* CFIndex */ nint version; // must be 0
			public /* void * */ IntPtr info;
			public /* CFAllocatorRetainCallBack*/ Func<IntPtr, IntPtr> retain;
			public /* CFAllocatorReleaseCallBack*/ Action<IntPtr> release;
			public /* CFAllocatorCopyDescriptionCallBack*/ Func<IntPtr, IntPtr> copyDescription;
		}

		public delegate NSData CFMessagePortCallBack (int type, NSData data);

		delegate /* CFDataRef */ IntPtr CFMessagePortCallBackProxy (/* CFMessagePortRef */ IntPtr messagePort, /* SInt32 */ int type, /* CFDataRef */ IntPtr data, /* void* */ IntPtr info);

		delegate void CFMessagePortInvalidationCallBackProxy (/* CFMessagePortRef */ IntPtr messagePort, /* void * */ IntPtr info);

		static Dictionary <IntPtr, CFMessagePortCallBack> outputHandles = new Dictionary <IntPtr, CFMessagePortCallBack> (Runtime.IntPtrEqualityComparer);

		static Dictionary <IntPtr, Action> invalidationHandles = new Dictionary <IntPtr, Action> (Runtime.IntPtrEqualityComparer);

		static Dictionary <IntPtr, CFMessagePortContext> messagePortContexts = new Dictionary <IntPtr, CFMessagePortContext> (Runtime.IntPtrEqualityComparer);

		static CFMessagePortCallBackProxy messageOutputCallback = new CFMessagePortCallBackProxy (MessagePortCallback);

		static CFMessagePortInvalidationCallBackProxy messageInvalidationCallback = new CFMessagePortInvalidationCallBackProxy (MessagePortInvalidationCallback);

		GCHandle gch;
		IntPtr handle;
		IntPtr contextHandle = IntPtr.Zero;

		public IntPtr Handle {
			get {
				return handle;
			}
		}

		public bool IsRemote {
			get {
				Check ();
				return CFMessagePortIsRemote (handle);
			}
		}

		public string Name {
			get {
				Check ();
				return NSString.FromHandle (CFMessagePortGetName (handle));
			}
			set {
				Check ();
				IntPtr n = NSString.CreateNative (value);
				CFMessagePortSetName (handle, n);
				NSString.ReleaseNative (n);
			}
		}

		public bool IsValid {
			get {
				Check ();
				return CFMessagePortIsValid (handle);
			}
		}

		internal CFMessagePortContext Context {
			get {
				Check ();

				CFMessagePortContext result;
				ContextProxy context = new ContextProxy ();
				CFMessagePortGetContext (handle, ref context);

				if (context.info == IntPtr.Zero)
					return null;
				
				lock (messagePortContexts)
					messagePortContexts.TryGetValue (context.info, out result);

				return result;
			}
		}

		public Action InvalidationCallback {
			get {
				Check ();
				Action result;

				lock (invalidationHandles)
					invalidationHandles.TryGetValue (handle, out result);

				return result;
			}
			set {
				Check ();

				lock (invalidationHandles) {
					if (value == null)
						invalidationHandles [handle] = null;
					else
						invalidationHandles.Add (handle, value);
				}

				CFMessagePortSetInvalidationCallBack (handle, messageInvalidationCallback);
			}
		}

		internal CFMessagePort (IntPtr handle) : this (handle, false)
		{
		}

		[Preserve (Conditional = true)]
		internal CFMessagePort (IntPtr handle, bool owns)
		{
			this.handle = handle;
			gch = GCHandle.Alloc (this);
			if (!owns)
				CFObject.CFRetain (handle);
		}

		~CFMessagePort ()
		{
			Dispose (false);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool disposing)
		{
			if (disposing) {
				if (gch.IsAllocated)
					gch.Free ();
			}

			if (handle != IntPtr.Zero) {

				lock (outputHandles)
					outputHandles.Remove (handle);

				lock (invalidationHandles) {
					if (invalidationHandles.ContainsKey (handle))
						invalidationHandles.Remove (handle);
				}

				lock (messagePortContexts) {
					if (messagePortContexts.ContainsKey (contextHandle))
						invalidationHandles.Remove (contextHandle);
				}

				CFObject.CFRelease (handle);
				contextHandle = IntPtr.Zero;
				handle = IntPtr.Zero;
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		static extern /* CFMessagePortRef */ IntPtr CFMessagePortCreateLocal (/* CFAllocatorRef */ IntPtr allocator, /* CFStringRef */ IntPtr name, CFMessagePortCallBackProxy callout, /*  CFMessagePortContext */ ref ContextProxy context, [MarshalAs (UnmanagedType.I1)] ref bool shouldFreeInfo);

		[DllImport (Constants.CoreFoundationLibrary)]
		static extern /* CFMessagePortRef */ IntPtr CFMessagePortCreateRemote (/* CFAllocatorRef */ IntPtr allocator, /* CFStringRef */ IntPtr name);

		[DllImport (Constants.CoreFoundationLibrary)]
		static extern void CFMessagePortInvalidate (/* CFMessagePortRef */ IntPtr ms);

		[DllImport (Constants.CoreFoundationLibrary)]
		static extern IntPtr CFMessagePortCreateRunLoopSource (/* CFAllocatorRef */ IntPtr allocator, /* CFMessagePortRef */ IntPtr local, /* CFIndex */ nint order);

		[DllImport (Constants.CoreFoundationLibrary)]
		static extern /* SInt32 */ CFMessagePortSendRequestStatus CFMessagePortSendRequest (/* CFMessagePortRef */ IntPtr remote, /* SInt32 */ int msgid, /* CFDataRef */ IntPtr data, /* CFTiemInterval */ double sendTimeout, /* CFTiemInterval */ double rcvTimeout, /* CFStringRef */ IntPtr replyMode, /* CFDataRef* */ ref IntPtr returnData);

		[DllImport (Constants.CoreFoundationLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern /* Boolean */ bool CFMessagePortIsRemote (/* CFMessagePortRef */ IntPtr ms);

		[DllImport (Constants.CoreFoundationLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern /* Boolean */ bool CFMessagePortSetName (/* CFMessagePortRef */ IntPtr ms, /* CFStringRef */ IntPtr newName);

		[DllImport (Constants.CoreFoundationLibrary)]
		static extern /* CFStringRef */ IntPtr CFMessagePortGetName (/* CFMessagePortRef */ IntPtr ms);

		[DllImport (Constants.CoreFoundationLibrary)]
		static extern void CFMessagePortGetContext (/* CFMessagePortRef */ IntPtr ms, /* CFMessagePortContext* */ ref ContextProxy context);

		[DllImport (Constants.CoreFoundationLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern /* Boolean */ bool CFMessagePortIsValid (/* CFMessagePortRef */ IntPtr ms);

		[DllImport (Constants.CoreFoundationLibrary)]
		static extern void CFMessagePortSetDispatchQueue (/* CFMessagePortRef */ IntPtr ms, dispatch_queue_t queue);

		[DllImport (Constants.CoreFoundationLibrary)]
		static extern void CFMessagePortSetInvalidationCallBack (/* CFMessagePortRef */ IntPtr ms, CFMessagePortInvalidationCallBackProxy callout);

		[DllImport (Constants.CoreFoundationLibrary)]
		static extern IntPtr CFMessagePortGetInvalidationCallBack (/* CFMessagePortRef */ IntPtr ms);

		public static CFMessagePort CreateLocalPort (string name, CFMessagePortCallBack callback, CFAllocator allocator = null)
		{
			if (callback == null)
				throw new ArgumentNullException ("callback");
			
			return CreateLocalPort (allocator, name, callback, context: null);
		}
		
		internal static CFMessagePort CreateLocalPort (CFAllocator allocator, string name, CFMessagePortCallBack callback, CFMessagePortContext context)
		{
			IntPtr a = allocator == null ? IntPtr.Zero : allocator.Handle;
			IntPtr n = NSString.CreateNative (name);
			bool shouldFreeInfo = false;
			var contextProxy = new ContextProxy ();

			// a GCHandle is needed because we do not have an handle before calling CFMessagePortCreateLocal
			// and that will call the RetainProxy. So using this (short-lived) GCHandle allow us to find back the
			// original context defined by developer
			var shortHandle = GCHandle.Alloc (contextProxy);

			if (context != null) {
				if (context.Retain != null)
					contextProxy.retain = RetainProxy;
				if (context.Release != null)
					contextProxy.release = ReleaseProxy;
				if (context.CopyDescription != null)
					contextProxy.copyDescription = CopyDescriptionProxy;
				contextProxy.info = (IntPtr)shortHandle;
				lock (messagePortContexts)
					messagePortContexts.Add (contextProxy.info, context);
			}

			try {
				var portHandle = CFMessagePortCreateLocal (a, n, messageOutputCallback, ref contextProxy, ref shouldFreeInfo);

				// we won't need short GCHandle after the Create call
				shortHandle.Free ();

				// TODO handle should free info
				if (portHandle == IntPtr.Zero)
					return null;

				var result = new CFMessagePort (portHandle);

				lock (outputHandles)
					outputHandles.Add (portHandle, callback);
				
				if (context != null) {
					lock (messagePortContexts) {
						messagePortContexts.Remove (contextProxy.info);
						CFMessagePortGetContext (portHandle, ref contextProxy);
						messagePortContexts.Add (contextProxy.info, context);
					}

					result.contextHandle = contextProxy.info;
				}
			
				return result;
			} finally {
				NSString.ReleaseNative (n);
			}
		}

		//
		// Proxy callbacks
		//
		[MonoPInvokeCallback (typeof (Func<IntPtr, IntPtr>))]
		static IntPtr RetainProxy (IntPtr info)
		{
			INativeObject result = null;
			CFMessagePortContext context;

			lock (messagePortContexts) {
				messagePortContexts.TryGetValue (info, out context);
			}
			
			if (context != null && context.Retain != null)
				result = context.Retain ();

			return result == null ? IntPtr.Zero : result.Handle;
		}

		[MonoPInvokeCallback (typeof (Action<IntPtr>))]
		static void ReleaseProxy (IntPtr info)
		{
			CFMessagePortContext context;

			lock (messagePortContexts)
				messagePortContexts.TryGetValue (info, out context);

			if (context != null && context.Release != null)
				context.Release ();
		}

		[MonoPInvokeCallback (typeof (Func<IntPtr, IntPtr>))]
		static IntPtr CopyDescriptionProxy (IntPtr info)
		{
			NSString result = null;
			CFMessagePortContext context;

			lock (messagePortContexts)
				messagePortContexts.TryGetValue (info, out context);

			if (context != null && context.CopyDescription != null)
				result = context.CopyDescription ();

			return result == null ? IntPtr.Zero : result.Handle;
		}

		[MonoPInvokeCallback (typeof (CFMessagePortCallBackProxy))]
		static IntPtr MessagePortCallback (IntPtr local, int msgid, IntPtr data, IntPtr info)
		{
			CFMessagePortCallBack callback;

			lock (outputHandles)
				callback = outputHandles [local];

			if (callback == null)
				return IntPtr.Zero;
			
			using (var managedData = Runtime.GetNSObject<NSData> (data)) {
				var result = callback.Invoke (msgid, managedData);
				// System will release returned CFData
				result?.DangerousRetain ();
				return result == null ? IntPtr.Zero : result.Handle;
			}
		}

		[MonoPInvokeCallback (typeof (CFMessagePortInvalidationCallBackProxy))]
		static void MessagePortInvalidationCallback (IntPtr messagePort, IntPtr info)
		{
			Action callback;

			lock (invalidationHandles)
				invalidationHandles.TryGetValue (messagePort, out callback);

			if (callback != null)
				callback.Invoke ();
		}

		public static CFMessagePort CreateRemotePort (CFAllocator allocator, string name)
		{
			if (name == null)
				throw new ArgumentNullException ("name");

			IntPtr a = allocator == null ? IntPtr.Zero : allocator.Handle;
			IntPtr n = NSString.CreateNative (name);

			try {
				var portHandle = CFMessagePortCreateRemote (a, n);
				return portHandle == IntPtr.Zero ? null : new CFMessagePort (portHandle);
			} finally {
				NSString.ReleaseNative (n);
			}
		}

		public void Invalidate ()
		{
			Check ();
			CFMessagePortInvalidate (handle);
		}

		public CFMessagePortSendRequestStatus SendRequest (int msgid, NSData data, double sendTimeout, double rcvTimeout, NSString replyMode, out NSData returnData)
		{
			Check ();

			IntPtr replyModeHandle = replyMode == null ? IntPtr.Zero : replyMode.Handle;
			IntPtr returnDataHandle = IntPtr.Zero;
			IntPtr dataHandle = data == null ? IntPtr.Zero : data.Handle;

			var result = CFMessagePortSendRequest (handle, msgid, dataHandle, sendTimeout, rcvTimeout, replyModeHandle, ref returnDataHandle);

			returnData = Runtime.GetINativeObject<NSData> (returnDataHandle, false);

			return result;
		}

		public CFRunLoopSource CreateRunLoopSource ()
		{
			// note: order is currently ignored by CFMessagePort object run loop sources. Pass 0 for this value.
			var runLoopHandle = CFMessagePortCreateRunLoopSource (IntPtr.Zero, handle, 0);
			return new CFRunLoopSource (runLoopHandle, false);
		}

		public void SetDispatchQueue (DispatchQueue queue)
		{
			IntPtr q = queue == null ? IntPtr.Zero : queue.Handle;

			Check ();
			CFMessagePortSetDispatchQueue (handle, q);
		}

		protected void Check ()
		{
			if (handle == IntPtr.Zero)
				throw new ObjectDisposedException (GetType ().ToString ());
		}
	}
}
