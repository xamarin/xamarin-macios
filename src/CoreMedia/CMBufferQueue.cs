//
// CMBufferQueue.cs: Implements the CMBufferQueue and CMBuffer managed bindings
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2012-2016 Xamarin Inc. All rights reserved.
//
//

#nullable enable

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Foundation;
using CoreFoundation;
using ObjCRuntime;
using OSStatus = System.Int32;

namespace CoreMedia {

	public delegate CMTime CMBufferGetTime (INativeObject buffer);
	public delegate bool CMBufferGetBool (INativeObject buffer);
	public delegate int CMBufferCompare (INativeObject first, INativeObject second);

#if NET
	// [SupportedOSPlatform ("ios")] -  SupportedOSPlatform is not valid on this declaration type "delegate" 
#else
	[Watch (6, 0)]
#endif
	public delegate nint CMBufferGetSize (INativeObject buffer);

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CMBufferQueue : NativeObject {
#if !COREBUILD
		GCHandle gch;
		Dictionary<IntPtr, INativeObject> queueObjects;
		CMBufferGetTime? getDecodeTimeStamp;
		CMBufferGetTime? getPresentationTimeStamp;
		CMBufferGetTime? getDuration;
		CMBufferGetBool? isDataReady;
		CMBufferCompare? compare;
		CMBufferGetSize? getTotalSize;

#if !NET
		delegate CMTime BufferGetTimeCallback (/* CMBufferRef */ IntPtr buf, /* void* */ IntPtr refcon);
		[return: MarshalAs (UnmanagedType.I1)]
		delegate bool BufferGetBooleanCallback (/* CMBufferRef */ IntPtr buf, /* void* */ IntPtr refcon);
		delegate int BufferCompareCallback (/* CMBufferRef */ IntPtr buf1, /* CMBufferRef */ IntPtr buf2, /* void* */ IntPtr refcon);
		delegate nint BufferGetSizeCallback (/* CMBufferRef */ IntPtr buffer, /* void* */ IntPtr refcon);
#endif

		[StructLayout (LayoutKind.Sequential)]
		struct CMBufferCallbacks {
			internal uint version;
			internal IntPtr refcon;
#if NET
			internal unsafe delegate* unmanaged<IntPtr, IntPtr, CMTime> XgetDecodeTimeStamp;
			internal unsafe delegate* unmanaged<IntPtr, IntPtr, CMTime> XgetPresentationTimeStamp;
			internal unsafe delegate* unmanaged<IntPtr, IntPtr, CMTime> XgetDuration;
			internal unsafe delegate* unmanaged<IntPtr, IntPtr, byte> XisDataReady;
			internal unsafe delegate* unmanaged<IntPtr, IntPtr, IntPtr, int> Xcompare;
#else
			internal BufferGetTimeCallback? XgetDecodeTimeStamp;
			internal BufferGetTimeCallback? XgetPresentationTimeStamp;
			internal BufferGetTimeCallback? XgetDuration;
			internal BufferGetBooleanCallback? XisDataReady;
			internal BufferCompareCallback? Xcompare;
#endif
			internal IntPtr cfStringPtr_dataBecameReadyNotification;
#if NET
			internal unsafe delegate* unmanaged<IntPtr, IntPtr, nint> XgetSize;
#else
			internal BufferGetSizeCallback? XgetSize;
#endif
		}

		// A version with no delegates, just native pointers
		[StructLayout (LayoutKind.Sequential)]
		struct CMBufferCallbacks2 {
			internal uint version;
			internal IntPtr refcon;
			internal IntPtr XgetDecodeTimeStamp;
			internal IntPtr XgetPresentationTimeStamp;
			internal IntPtr XgetDuration;
			internal IntPtr XisDataReady;
			internal IntPtr Xcompare;
			internal IntPtr cfStringPtr_dataBecameReadyNotification;
			internal IntPtr XgetSize;
		}

		protected override void Dispose (bool disposing)
		{
			queueObjects.Clear ();
			if (gch.IsAllocated)
				gch.Free ();
			base.Dispose (disposing);
		}

		// CMItemCount -> CMBase.h (looks weird but it's 4 bytes in 32bits and 8 bytes in 64bits, x86_64 and ARM64)

		[DllImport (Constants.CoreMediaLibrary)]
		extern static OSStatus CMBufferQueueCreate (/* CFAllocatorRef */ IntPtr allocator, /* CMItemCount */ nint capacity, CMBufferCallbacks cbacks, /* CMBufferQueueRef* */ out IntPtr result);

		[DllImport (Constants.CoreMediaLibrary)]
		extern static OSStatus CMBufferQueueCreate (/* CFAllocatorRef */ IntPtr allocator, /* CMItemCount */ nint capacity, /* CMBufferCallbacks */ IntPtr cbacks, /* CMBufferQueueRef* */ out IntPtr result);

		CMBufferQueue (IntPtr handle, bool owns, int count)
			: base (handle, owns)
		{
			queueObjects = new Dictionary<IntPtr, INativeObject> (count, Runtime.IntPtrEqualityComparer);
			gch = GCHandle.Alloc (this);
		}

		CMBufferQueue (int count)
			: this (IntPtr.Zero, true, count)
		{
		}

		// for compatibility with 7.0 and earlier
		public static CMBufferQueue? FromCallbacks (int count, CMBufferGetTime? getDecodeTimeStamp, CMBufferGetTime? getPresentationTimeStamp, CMBufferGetTime? getDuration,
			CMBufferGetBool? isDataReady, CMBufferCompare? compare, NSString dataBecameReadyNotification)
		{
			return FromCallbacks (count, getDecodeTimeStamp, getPresentationTimeStamp, getDuration, isDataReady,
				compare, dataBecameReadyNotification, null);
		}

		public static CMBufferQueue? FromCallbacks (int count, CMBufferGetTime? getDecodeTimeStamp, CMBufferGetTime? getPresentationTimeStamp, CMBufferGetTime? getDuration,
			CMBufferGetBool? isDataReady, CMBufferCompare? compare, NSString dataBecameReadyNotification, CMBufferGetSize? getTotalSize)
		{
			var bq = new CMBufferQueue (count);
#if NET
			CMBufferCallbacks cbacks;
			unsafe {
				cbacks = new CMBufferCallbacks () {
					version = (uint) (getTotalSize is null ? 0 : 1),
					refcon = GCHandle.ToIntPtr (bq.gch),
					XgetDecodeTimeStamp = getDecodeTimeStamp is not null ? &GetDecodeTimeStamp : null,
					XgetPresentationTimeStamp = getPresentationTimeStamp is not null ? &GetPresentationTimeStamp : null,
					XgetDuration = getDuration is not null ? &GetDuration : null,
					XisDataReady = isDataReady is not null ? &GetDataReady : null,
					Xcompare = compare is not null ? &Compare : null,
					cfStringPtr_dataBecameReadyNotification = dataBecameReadyNotification is null ? IntPtr.Zero : dataBecameReadyNotification.Handle,
					XgetSize = getTotalSize is not null ? &GetTotalSize : null
				};
			}
#else
			var cbacks = new CMBufferCallbacks () {
				version = (uint) (getTotalSize is null ? 0 : 1),
				refcon = GCHandle.ToIntPtr (bq.gch),
				XgetDecodeTimeStamp = getDecodeTimeStamp is not null ? GetDecodeTimeStamp : null,
				XgetPresentationTimeStamp = getPresentationTimeStamp is not null ? GetPresentationTimeStamp : null,
				XgetDuration = getDuration is not null ? GetDuration : null,
				XisDataReady = isDataReady is not null ? GetDataReady : null,
				Xcompare = compare is not null ? Compare : null,
				cfStringPtr_dataBecameReadyNotification = dataBecameReadyNotification is null ? IntPtr.Zero : dataBecameReadyNotification.Handle,
				XgetSize = getTotalSize is not null ? GetTotalSize : null
			};
#endif

			bq.getDecodeTimeStamp = getDecodeTimeStamp;
			bq.getPresentationTimeStamp = getPresentationTimeStamp;
			bq.getDuration = getDuration;
			bq.isDataReady = isDataReady;
			bq.compare = compare;
			bq.getTotalSize = getTotalSize;

			if (CMBufferQueueCreate (IntPtr.Zero, count, cbacks, out var handle) == 0) {
				bq.InitializeHandle (handle);
				return bq;
			}

			bq.Dispose ();
			return null;
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static /* CMBufferCallbacks */ IntPtr CMBufferQueueGetCallbacksForUnsortedSampleBuffers ();

		public static CMBufferQueue? CreateUnsorted (int count)
		{
			// note: different version of iOS can return a different (size) structure, e.g. iOS 7.1,
			// that structure might not map to our `CMBufferCallbacks2` managed representation
			// and can cause a crash, e.g. bug #17330
			// since we don't need the managed bcallbacks (it's the native callback that will be used for this queue)
			// then we can simply use an IntPtr to represent them (no GCHandle)
			var callbacks = CMBufferQueueGetCallbacksForUnsortedSampleBuffers ();

			if (CMBufferQueueCreate (IntPtr.Zero, count, callbacks, out var handle) == 0)
				return new CMBufferQueue (handle, true, count);

			return null;
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static OSStatus CMBufferQueueEnqueue (/* CMBufferQueueRef */ IntPtr queue, /* CMBufferRef */ IntPtr buf);

		//
		// It really should be ICFType, and we should pepper various classes with ICFType
		//
		public void Enqueue (INativeObject cftypeBuffer)
		{
			if (cftypeBuffer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (cftypeBuffer));
			lock (queueObjects) {
				var cfh = cftypeBuffer.Handle;
				CMBufferQueueEnqueue (Handle, cfh);
				if (!queueObjects.ContainsKey (cfh))
					queueObjects [cfh] = cftypeBuffer;
			}
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* CMBufferRef */ IntPtr CMBufferQueueDequeueAndRetain (/* CMBufferQueueRef */ IntPtr queue);

		public INativeObject? Dequeue ()
		{
			//
			// Our managed objects already take a reference on the object,
			// and by keeping the objects alive in the `queueObjects'
			// dictionary, we kept the reference alive.   So we need to
			// release the newly acquired reference
			//
			var oHandle = CMBufferQueueDequeueAndRetain (Handle);
			if (oHandle == IntPtr.Zero)
				return null;

			CFObject.CFRelease (oHandle);
			lock (queueObjects) {
				var managed = queueObjects [oHandle];
				queueObjects.Remove (oHandle);
				return managed;
			}
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* CMBufferRef */ IntPtr CMBufferQueueDequeueIfDataReadyAndRetain (/* CMBufferQueueRef */ IntPtr queue);

		public INativeObject? DequeueIfDataReady ()
		{
			//
			// Our managed objects already take a reference on the object,
			// and by keeping the objects alive in the `queueObjects'
			// dictionary, we kept the reference alive.   So we need to
			// release the newly acquired reference
			//
			var oHandle = CMBufferQueueDequeueIfDataReadyAndRetain (Handle);
			if (oHandle == IntPtr.Zero)
				return null;

			CFObject.CFRelease (oHandle);
			lock (queueObjects) {
				var managed = queueObjects [oHandle];
				queueObjects.Remove (oHandle);
				return managed;
			}
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static byte CMBufferQueueIsEmpty (/* CMBufferQueueRef */ IntPtr queue);
		public bool IsEmpty {
			get {
				return CMBufferQueueIsEmpty (Handle) != 0;
			}
		}


		[DllImport (Constants.CoreMediaLibrary)]
		extern static OSStatus CMBufferQueueMarkEndOfData (/* CMBufferQueueRef */ IntPtr queue);
		public int MarkEndOfData ()
		{
			return CMBufferQueueMarkEndOfData (Handle);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static byte CMBufferQueueContainsEndOfData (/* CMBufferQueueRef */ IntPtr queue);
		public bool ContainsEndOfData {
			get {
				return CMBufferQueueContainsEndOfData (Handle) != 0;
			}
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static byte CMBufferQueueIsAtEndOfData (/* CMBufferQueueRef */ IntPtr queue);
		public bool IsAtEndOfData {
			get {
				return CMBufferQueueIsAtEndOfData (Handle) != 0;
			}
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static OSStatus CMBufferQueueReset (/* CMBufferQueueRef */ IntPtr queue);
		public OSStatus Reset ()
		{
			return CMBufferQueueReset (Handle);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static nint CMBufferQueueGetBufferCount (/* CMBufferQueueRef */ IntPtr queue);
		public nint BufferCount {
			get {
				return CMBufferQueueGetBufferCount (Handle);
			}
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static CMTime CMBufferQueueGetDuration (/* CMBufferQueueRef */ IntPtr queue);
		public CMTime Duration {
			get {
				return CMBufferQueueGetDuration (Handle);
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* size_t */ nint CMBufferQueueGetTotalSize (/* CMBufferQueueRef */ IntPtr queue);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public nint GetTotalSize ()
		{
			return CMBufferQueueGetTotalSize (Handle);
		}


		// Surfaces the given buffer pointer to a managed object
		INativeObject Surface (IntPtr v)
		{
			return queueObjects [v];
		}

#if NET
		[UnmanagedCallersOnly]
#else
#if !MONOMAC
		[MonoPInvokeCallback (typeof (BufferGetTimeCallback))]
#endif
#endif
		static CMTime GetDecodeTimeStamp (IntPtr buffer, IntPtr refcon)
		{
			var queue = (CMBufferQueue?) GCHandle.FromIntPtr (refcon).Target;
			if (queue?.getDecodeTimeStamp is null)
				return default (CMTime);
			return queue.getDecodeTimeStamp (queue.Surface (buffer));
		}

#if NET
		[UnmanagedCallersOnly]
#else
#if !MONOMAC
		[MonoPInvokeCallback (typeof (BufferGetTimeCallback))]
#endif
#endif
		static CMTime GetPresentationTimeStamp (IntPtr buffer, IntPtr refcon)
		{
			var queue = (CMBufferQueue?) GCHandle.FromIntPtr (refcon).Target;
			if (queue?.getPresentationTimeStamp is null)
				return default (CMTime);
			return queue.getPresentationTimeStamp (queue.Surface (buffer));
		}

#if NET
		[UnmanagedCallersOnly]
#else
#if !MONOMAC
		[MonoPInvokeCallback (typeof (BufferGetTimeCallback))]
#endif
#endif
		static CMTime GetDuration (IntPtr buffer, IntPtr refcon)
		{
			var queue = (CMBufferQueue?) GCHandle.FromIntPtr (refcon).Target;
			if (queue?.getDuration is null)
				return default (CMTime);
			return queue.getDuration (queue.Surface (buffer));
		}

#if NET
		[UnmanagedCallersOnly]
		static byte GetDataReady (IntPtr buffer, IntPtr refcon)
#else
#if !MONOMAC
		[MonoPInvokeCallback (typeof (BufferGetBooleanCallback))]
#endif
		static bool GetDataReady (IntPtr buffer, IntPtr refcon)
#endif
		{
			var queue = (CMBufferQueue?) GCHandle.FromIntPtr (refcon).Target;
			if (queue?.isDataReady is null)
#if NET
				return 0;
			return (byte) (queue.isDataReady (queue.Surface (buffer)) ? 1 : 0);
#else
				return false;
			return queue.isDataReady (queue.Surface (buffer));
#endif
		}

#if NET
		[UnmanagedCallersOnly]
#else
#if !MONOMAC
		[MonoPInvokeCallback (typeof (BufferCompareCallback))]
#endif
#endif
		static int Compare (IntPtr buffer1, IntPtr buffer2, IntPtr refcon)
		{
			var queue = (CMBufferQueue?) GCHandle.FromIntPtr (refcon).Target;
			if (queue?.compare is null)
				return 0;
			return queue.compare (queue.Surface (buffer1), queue.Surface (buffer2));
		}

#if NET
		[UnmanagedCallersOnly]
#else
#if !MONOMAC
		[MonoPInvokeCallback (typeof (BufferGetSizeCallback))]
#endif
#endif
		static nint GetTotalSize (IntPtr buffer, IntPtr refcon)
		{
			var queue = (CMBufferQueue?) GCHandle.FromIntPtr (refcon).Target;
			if (queue?.getTotalSize is null)
				return 0;
			return queue.getTotalSize (queue.Surface (buffer));
		}
#endif // !COREBUILD

#if !NET
		[Watch (6, 0)]
#endif
		public enum TriggerCondition {
			WhenDurationBecomesLessThan = 1,
			WhenDurationBecomesLessThanOrEqualTo = 2,
			WhenDurationBecomesGreaterThan = 3,
			WhenDurationBecomesGreaterThanOrEqualTo = 4,
			WhenMinPresentationTimeStampChanges = 5,
			WhenMaxPresentationTimeStampChanges = 6,
			WhenDataBecomesReady = 7,
			WhenEndOfDataReached = 8,
			WhenReset = 9,
			WhenBufferCountBecomesLessThan = 10,
			WhenBufferCountBecomesGreaterThan = 11,
			WhenDurationBecomesGreaterThanOrEqualToAndBufferCountBecomesGreaterThan = 12,
		}
	}
}
