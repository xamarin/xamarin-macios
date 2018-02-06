//
// CMBufferQueue.cs: Implements the CMBufferQueue and CMBuffer managed bindings
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2012-2016 Xamarin Inc. All rights reserved.
//
//
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Foundation;
using CoreFoundation;
using ObjCRuntime;
using OSStatus = System.Int32;

// FIXME64: this will change on 64 bit builds
//
using CMItemCount = System.Int32;

namespace CoreMedia {
	
	public delegate CMTime CMBufferGetTime (INativeObject buffer);
	public delegate bool   CMBufferGetBool (INativeObject buffer);
	public delegate int    CMBufferCompare (INativeObject first, INativeObject second);

	[iOS (7,1)]
	public delegate nint   CMBufferGetSize (INativeObject buffer);

	public class CMBufferQueue : INativeObject
#if !COREBUILD
		, IDisposable
#endif
		{
#if !COREBUILD
		GCHandle gch;
		Dictionary<IntPtr, INativeObject> queueObjects;
		internal IntPtr handle;
		CMBufferGetTime getDecodeTimeStamp;
		CMBufferGetTime getPresentationTimeStamp;
		CMBufferGetTime getDuration;
		CMBufferGetBool isDataReady;
		CMBufferCompare compare;
		CMBufferGetSize getTotalSize;
		
		delegate CMTime BufferGetTimeCallback (/* CMBufferRef */ IntPtr buf, /* void* */ IntPtr refcon);
		delegate bool   BufferGetBooleanCallback (/* CMBufferRef */ IntPtr buf, /* void* */ IntPtr refcon);
		delegate int    BufferCompareCallback (/* CMBufferRef */ IntPtr buf1, /* CMBufferRef */ IntPtr buf2, /* void* */ IntPtr refcon);
		delegate nint   BufferGetSizeCallback (/* CMBufferRef */ IntPtr buffer, /* void* */ IntPtr refcon);
		
		[StructLayout (LayoutKind.Sequential)]
		struct CMBufferCallbacks {
			internal uint version;
			internal IntPtr refcon;
			internal BufferGetTimeCallback XgetDecodeTimeStamp;
			internal BufferGetTimeCallback XgetPresentationTimeStamp;
			internal BufferGetTimeCallback XgetDuration;
			internal BufferGetBooleanCallback XisDataReady;
			internal BufferCompareCallback Xcompare;
			internal IntPtr cfStringPtr_dataBecameReadyNotification;
			internal BufferGetSizeCallback XgetSize;
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

		~CMBufferQueue ()
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
			queueObjects = null;
			if (handle != IntPtr.Zero){
				CFObject.CFRelease (handle);
				handle = IntPtr.Zero;
			}
			if (gch.IsAllocated)
				gch.Free ();
		}

		// CMItemCount -> CMBase.h (looks weird but it's 4 bytes in 32bits and 8 bytes in 64bits, x86_64 and ARM64)

		[DllImport(Constants.CoreMediaLibrary)]
		extern static OSStatus CMBufferQueueCreate (/* CFAllocatorRef */ IntPtr allocator, /* CMItemCount */ nint capacity, CMBufferCallbacks cbacks, /* CMBufferQueueRef* */ out IntPtr result);

		[DllImport(Constants.CoreMediaLibrary)]
		extern static OSStatus CMBufferQueueCreate (/* CFAllocatorRef */ IntPtr allocator, /* CMItemCount */ nint capacity, /* CMBufferCallbacks */ IntPtr cbacks, /* CMBufferQueueRef* */ out IntPtr result);

		internal CMBufferQueue (int count)
		{
			queueObjects = new Dictionary<IntPtr,INativeObject> (count, Runtime.IntPtrEqualityComparer);
			gch = GCHandle.Alloc (this);
		}

		// for compatibility with 7.0 and earlier
		public static CMBufferQueue FromCallbacks (int count, CMBufferGetTime getDecodeTimeStamp, CMBufferGetTime getPresentationTimeStamp, CMBufferGetTime getDuration,
			CMBufferGetBool isDataReady, CMBufferCompare compare, NSString dataBecameReadyNotification)
		{
			return FromCallbacks (count, getDecodeTimeStamp, getPresentationTimeStamp, getDuration, isDataReady, 
				compare, dataBecameReadyNotification, null);
		}

		public static CMBufferQueue FromCallbacks (int count, CMBufferGetTime getDecodeTimeStamp, CMBufferGetTime getPresentationTimeStamp, CMBufferGetTime getDuration,
			CMBufferGetBool isDataReady, CMBufferCompare compare, NSString dataBecameReadyNotification, CMBufferGetSize getTotalSize)
		{
			var bq = new CMBufferQueue (count);
			var cbacks = new CMBufferCallbacks () {
				version = (uint) (getTotalSize == null ? 0 : 1),
				refcon = GCHandle.ToIntPtr (bq.gch),
				XgetDecodeTimeStamp = getDecodeTimeStamp == null ? (BufferGetTimeCallback) null : GetDecodeTimeStamp,
				XgetPresentationTimeStamp = getPresentationTimeStamp == null ? (BufferGetTimeCallback) null : GetPresentationTimeStamp,
				XgetDuration = getDuration == null ? (BufferGetTimeCallback) null : GetDuration,
				XisDataReady = isDataReady == null ? (BufferGetBooleanCallback) null : GetDataReady,
				Xcompare = compare == null ? (BufferCompareCallback) null : Compare,
				cfStringPtr_dataBecameReadyNotification = dataBecameReadyNotification == null ? IntPtr.Zero : dataBecameReadyNotification.Handle,
				XgetSize = getTotalSize == null ? (BufferGetSizeCallback) null : GetTotalSize
			};

			bq.getDecodeTimeStamp = getDecodeTimeStamp;
			bq.getPresentationTimeStamp = getPresentationTimeStamp;
			bq.getDuration = getDuration;
			bq.isDataReady = isDataReady;
			bq.compare = compare;
			bq.getTotalSize = getTotalSize;

			if (CMBufferQueueCreate (IntPtr.Zero, count, cbacks, out bq.handle) == 0)
				return bq;

			bq.Dispose ();
			return null;
		}

		[DllImport(Constants.CoreMediaLibrary)]
		unsafe extern static /* CMBufferCallbacks */ IntPtr CMBufferQueueGetCallbacksForUnsortedSampleBuffers ();
		
		public static CMBufferQueue CreateUnsorted (int count)
		{
			var bq = new CMBufferQueue (count);
			// note: different version of iOS can return a different (size) structure, e.g. iOS 7.1,
			// that structure might not map to our `CMBufferCallbacks2` managed representation
			// and can cause a crash, e.g. bug #17330
			// since we don't need the managed bcallbacks (it's the native callback that will be used for this queue)
			// then we can simply use an IntPtr to represent them (no GCHandle)
			var callbacks = CMBufferQueueGetCallbacksForUnsortedSampleBuffers ();
			
			if (CMBufferQueueCreate (IntPtr.Zero, count, callbacks, out bq.handle) == 0)
				return bq;

			return null;
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static OSStatus CMBufferQueueEnqueue (/* CMBufferQueueRef */ IntPtr queue, /* CMBufferRef */ IntPtr buf);
		
		//
		// It really should be ICFType, and we should pepper various classes with ICFType
		//
		public void Enqueue (INativeObject cftypeBuffer)
		{
			if (cftypeBuffer == null)
				throw new ArgumentNullException ("cftypeBuffer");
			lock (queueObjects){
				var cfh = cftypeBuffer.Handle;
				CMBufferQueueEnqueue (handle, cfh);
				if (!queueObjects.ContainsKey (cfh))
					queueObjects [cfh] = cftypeBuffer;
			}
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* CMBufferRef */ IntPtr CMBufferQueueDequeueAndRetain (/* CMBufferQueueRef */ IntPtr queue);

		public INativeObject Dequeue ()
		{
			//
			// Our managed objects already take a reference on the object,
			// and by keeping the objects alive in the `queueObjects'
			// dictionary, we kept the reference alive.   So we need to
			// release the newly acquired reference
			//
			var oHandle = CMBufferQueueDequeueAndRetain (handle);
			if (oHandle == IntPtr.Zero)
				return null;

			CFObject.CFRelease (oHandle);
			lock (queueObjects){
				var managed = queueObjects [oHandle];
				queueObjects.Remove (oHandle);
				return managed;
			}
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* CMBufferRef */ IntPtr CMBufferQueueDequeueIfDataReadyAndRetain (/* CMBufferQueueRef */ IntPtr queue);
		
		public INativeObject DequeueIfDataReady ()
		{
			//
			// Our managed objects already take a reference on the object,
			// and by keeping the objects alive in the `queueObjects'
			// dictionary, we kept the reference alive.   So we need to
			// release the newly acquired reference
			//
			var oHandle = CMBufferQueueDequeueIfDataReadyAndRetain (handle);
			if (oHandle == IntPtr.Zero)
				return null;

			CFObject.CFRelease (oHandle);
			lock (queueObjects){
				var managed = queueObjects [oHandle];
				queueObjects.Remove (oHandle);
				return managed;
			}
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static byte CMBufferQueueIsEmpty (/* CMBufferQueueRef */ IntPtr queue);
		public bool IsEmpty {
			get {
				return CMBufferQueueIsEmpty (handle) != 0;
			}
		}

		
		[DllImport(Constants.CoreMediaLibrary)]
		extern static OSStatus CMBufferQueueMarkEndOfData (/* CMBufferQueueRef */ IntPtr queue);
		public int MarkEndOfData ()
		{
			return CMBufferQueueMarkEndOfData (handle);
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static byte CMBufferQueueContainsEndOfData (/* CMBufferQueueRef */ IntPtr queue);
		public bool ContainsEndOfData {
			get {
				return CMBufferQueueContainsEndOfData (handle) != 0;
			}
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static byte CMBufferQueueIsAtEndOfData (/* CMBufferQueueRef */ IntPtr queue);
		public bool IsAtEndOfData {
			get {
				return CMBufferQueueIsAtEndOfData (handle) != 0;
			}
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static OSStatus CMBufferQueueReset (/* CMBufferQueueRef */ IntPtr queue);
		public OSStatus Reset ()
		{
			return CMBufferQueueReset (handle);
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static nint CMBufferQueueGetBufferCount (/* CMBufferQueueRef */ IntPtr queue);
		public nint BufferCount {
			get {
				return CMBufferQueueGetBufferCount (handle);
			}
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static CMTime CMBufferQueueGetDuration (/* CMBufferQueueRef */ IntPtr queue);
		public CMTime Duration {
			get {
				return CMBufferQueueGetDuration (handle);
			}
		}
		
		[iOS (7,1)][Mac (10,10)]
		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* size_t */ nint CMBufferQueueGetTotalSize (/* CMBufferQueueRef */ IntPtr queue);

		[iOS (7,1)]
		[Mac (10, 10)]
		public nint GetTotalSize ()
		{
			return CMBufferQueueGetTotalSize (handle);
		}


		// Surfaces the given buffer pointer to a managed object
		INativeObject Surface (IntPtr v)
		{
			return queueObjects [v];
		}

#if !MONOMAC
		[MonoPInvokeCallback (typeof (BufferGetTimeCallback))]
#endif
		static CMTime GetDecodeTimeStamp (IntPtr buffer, IntPtr refcon)
		{
			var queue = (CMBufferQueue) GCHandle.FromIntPtr (refcon).Target;
			return queue.getDecodeTimeStamp (queue.Surface (buffer));
		}

#if !MONOMAC
		[MonoPInvokeCallback (typeof (BufferGetTimeCallback))]
#endif
		static CMTime GetPresentationTimeStamp (IntPtr buffer, IntPtr refcon)
		{
			var queue = (CMBufferQueue) GCHandle.FromIntPtr (refcon).Target;
			return queue.getPresentationTimeStamp (queue.Surface (buffer));
		}

#if !MONOMAC
		[MonoPInvokeCallback (typeof (BufferGetTimeCallback))]
#endif
		static CMTime GetDuration (IntPtr buffer, IntPtr refcon)
		{
			var queue = (CMBufferQueue) GCHandle.FromIntPtr (refcon).Target;
			return queue.getDuration (queue.Surface (buffer));
		}

#if !MONOMAC
		[MonoPInvokeCallback (typeof (BufferGetBooleanCallback))]
#endif
		static bool GetDataReady (IntPtr buffer, IntPtr refcon)
		{
			var queue = (CMBufferQueue) GCHandle.FromIntPtr (refcon).Target;
			return queue.isDataReady (queue.Surface (buffer));
		}

#if !MONOMAC
		[MonoPInvokeCallback (typeof (BufferCompareCallback))]
#endif
		static int Compare (IntPtr buffer1, IntPtr buffer2, IntPtr refcon)
		{
			var queue = (CMBufferQueue) GCHandle.FromIntPtr (refcon).Target;
			return queue.compare (queue.Surface (buffer1), queue.Surface (buffer2));
		}

#if !MONOMAC
		[MonoPInvokeCallback (typeof (BufferGetSizeCallback))]
#endif
		static nint GetTotalSize (IntPtr buffer, IntPtr refcon)
		{
			var queue = (CMBufferQueue) GCHandle.FromIntPtr (refcon).Target;
			return queue.getTotalSize (queue.Surface (buffer));
		}
#endif // !COREBUILD

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
