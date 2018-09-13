//
// Dispatch.cs: Support for Grand Central Dispatch framework
//
// Authors:
//   Miguel de Icaza (miguel@gnome.org)
//   Marek Safar (marek.safar@gmail.com)
//
// Copyright 2010 Novell, Inc.
// Copyright 2011-2014 Xamarin Inc
//
// this contains DispathcObject, Group and Queue
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
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using ObjCRuntime;
using Foundation;

namespace CoreFoundation {

	// The native constants are defined in usr/include/dispatch/queue.h, but since they're
	// not in any enum, they're untyped.
	public enum DispatchQueuePriority : int {
		High = 2,
		Default = 0,
		Low = -2,
		Background = Int16.MinValue
	}

	// dispatch_qos_class_t is defined in usr/include/dispatch/queue.h, but redirects to qos_class_t
	// the qos_class_t enum is defined in usr/include/sys/qos.h (typed as 'unsigned int')
	public enum DispatchQualityOfService : uint {
		UserInteractive = 0x21,
		UserInitiated   = 0x19,
		Default         = 0x15,
		Utility         = 0x11,
		Background      = 0x09,
		Unspecified     = 0x00,
	}
	
	public abstract class DispatchObject : NativeObject
	{
#if !COREBUILD
		//
		// Constructors and lifecycle
		//
		[Preserve (Conditional = true)]
		internal DispatchObject (IntPtr handle, bool owns)
			: base (handle, owns)
		{
			if (handle == IntPtr.Zero)
				throw new ArgumentNullException ("handle");
		}

		internal DispatchObject ()
		{
		}      

		[DllImport (Constants.libcLibrary)]
		extern static IntPtr dispatch_release (IntPtr o);

		[DllImport (Constants.libcLibrary)]
		extern static IntPtr dispatch_retain (IntPtr o);

		protected override void Retain ()
		{
			dispatch_retain (Handle);
		}

		protected override void Release ()
		{
			dispatch_release (Handle);
		}

		public static bool operator == (DispatchObject a, DispatchObject b)
		{
			var oa = a as object;
			var ob = b as object;
			
			if (oa == null){
				if (ob == null)
					return true;
				return false;
			} else {
				if (ob == null)
					return false;
				return a.Handle == b.Handle;
			}
		}

		public static bool operator != (DispatchObject a, DispatchObject b)
		{
			return !(a == b);
		}

		public override bool Equals (object other)
		{
			var od = other as DispatchQueue;
			if (od == null)
				return false;
			return od.Handle == Handle;
		}

		public override int GetHashCode ()
		{
			return (int) Handle;
		}

#if !XAMCORE_4_0
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("Use 'GetCheckedHandle' instead.")]
		protected void Check ()
		{
			GetCheckedHandle ();
		}
#endif

		[DllImport (Constants.libcLibrary)]
		extern static void dispatch_set_target_queue (/* dispatch_object_t */ IntPtr queue, /* dispatch_queue_t */ IntPtr target);

		public void SetTargetQueue (DispatchQueue queue)
		{
			// note: null is allowed because DISPATCH_TARGET_QUEUE_DEFAULT is defined as NULL (dispatch/queue.h)
			IntPtr q = queue == null ? IntPtr.Zero : queue.Handle;
			dispatch_set_target_queue (Handle, q);
		}

		[DllImport (Constants.libcLibrary)]
		internal extern static void dispatch_resume (IntPtr o);

		[DllImport (Constants.libcLibrary)]
		internal extern static void dispatch_suspend (IntPtr o);

		[Mac (10,12)]
		[iOS (10,0)]
		[TV (10,0)]
		[Watch (3,0)]
		public void Activate ()
		{
			dispatch_activate (GetCheckedHandle ());
		}

		[DllImport (Constants.libcLibrary)]
		extern static void dispatch_activate (/* dispatch_object_t */ IntPtr @object);
#endif // !COREBUILD
	}

	public sealed class DispatchQueue : DispatchObject  {
#if !COREBUILD
		[Preserve (Conditional = true)]
		internal DispatchQueue (IntPtr handle, bool owns) : base (handle, owns)
		{
		}

		public DispatchQueue (IntPtr handle) : base (handle, false)
		{
		}
		
		public DispatchQueue (string label)
			: base (dispatch_queue_create (label, IntPtr.Zero), true)
		{
			if (Handle == IntPtr.Zero)
				throw new Exception ("Error creating dispatch queue");
		}

		static IntPtr concurrent_queue;
		static IntPtr ConcurrentQueue {
			get {
				if (concurrent_queue == IntPtr.Zero)
					concurrent_queue = Dlfcn.dlsym (Dlfcn.RTLD.Self, "_dispatch_queue_attr_concurrent");
				return concurrent_queue;
			}
		}
		
		public DispatchQueue (string label, bool concurrent)
			: base (dispatch_queue_create (label, concurrent ? ConcurrentQueue : IntPtr.Zero), true)
		{
			if (Handle == IntPtr.Zero)
				throw new Exception ("Error creating dispatch queue");
		}
		
		[Mac (10,12)]
		[iOS (10,0)]
		[TV (10,0)]
		[Watch (3,0)]
		public DispatchQueue (string label, Attributes attributes, DispatchQueue target = null)
			: base (dispatch_queue_create_with_target (label, attributes?.Create () ?? IntPtr.Zero, target.GetHandle ()), true)
		{
			if (Handle == IntPtr.Zero)
				throw new Exception ("Error creating dispatch queue");
		}

		//
		// Properties and methods
		//

		public string Label {
			get {
				return Marshal.PtrToStringAnsi (dispatch_queue_get_label (GetCheckedHandle ()));
			}
		}

		[iOS (7,0)]
		public static string CurrentQueueLabel {
			get {
				return Marshal.PtrToStringAnsi (dispatch_queue_get_label (IntPtr.Zero));
			}
		}

		public void Suspend ()
		{
			dispatch_suspend (GetCheckedHandle ());
		}

		public void Resume ()
		{
			dispatch_resume (GetCheckedHandle ());
		}

		[DllImport (Constants.libcLibrary)]
		extern static IntPtr dispatch_get_context (IntPtr o);

		[DllImport (Constants.libcLibrary)]
		extern static void dispatch_set_context (IntPtr o, IntPtr ctx);

		[DllImport (Constants.libcLibrary)]
		extern static void dispatch_apply_f (IntPtr iterations, IntPtr queue, IntPtr ctx, dispatch_callback_iterations_t dispatch);

		public IntPtr Context {
			get {
				return dispatch_get_context (GetCheckedHandle ());
			}
			set {
				dispatch_set_context (GetCheckedHandle (), value);
			}
		}
	
		[Deprecated (PlatformName.iOS, 6, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 9)]
		public static DispatchQueue CurrentQueue {
			get {
				return new DispatchQueue (dispatch_get_current_queue (), false);
			}
		}

		public static DispatchQueue GetGlobalQueue (DispatchQueuePriority priority)
		{
			return new DispatchQueue (dispatch_get_global_queue ((nint) (int) priority, 0), false);
		}

		public static DispatchQueue DefaultGlobalQueue {
			get {
				return new DispatchQueue (dispatch_get_global_queue ((nint) (int) DispatchQueuePriority.Default, 0), false);
			}
		}

		static IntPtr main_q;

		public static DispatchQueue MainQueue {
			get {
				if (main_q == IntPtr.Zero) {
					// Can't use a Field attribute because we don't support generating a call to Dlfcn.GetIndirect.
					main_q = Dlfcn.GetIndirect (Libraries.libdispatch.Handle, "_dispatch_main_q");
				}

				return new DispatchQueue (main_q, false);
			}
		}


		//
		// Dispatching
		//
		internal delegate void dispatch_callback_t (IntPtr context);
		internal static readonly dispatch_callback_t static_dispatch = static_dispatcher_to_managed;

		internal delegate void dispatch_callback_iterations_t (IntPtr context, IntPtr count);
		internal static readonly dispatch_callback_iterations_t static_dispatch_iterations = static_dispatcher_iterations_to_managed;
		
		[MonoPInvokeCallback (typeof (dispatch_callback_t))]
		static void static_dispatcher_to_managed (IntPtr context)
		{
			GCHandle gch = GCHandle.FromIntPtr (context);
			var obj = gch.Target as Tuple<Action, DispatchQueue>;
			gch.Free ();
			if (obj != null) {
				var sc = SynchronizationContext.Current;

				// Set GCD synchronization context. Mainly used when await executes inside GCD to continue
				// execution on same dispatch queue. Set the context only when there is no user context
				// set, including UIKitSynchronizationContext
				//
				// This assumes that only 1 queue can run on thread at the same time
				//
				if (sc == null)
					SynchronizationContext.SetSynchronizationContext (new DispatchQueueSynchronizationContext (obj.Item2));

				try {
					obj.Item1 ();
				} finally {
					if (sc == null)
						SynchronizationContext.SetSynchronizationContext (null);
				}
			}

		}

		[MonoPInvokeCallback (typeof (dispatch_callback_iterations_t))]
		static void static_dispatcher_iterations_to_managed (IntPtr context, IntPtr count)
		{
			GCHandle gch = GCHandle.FromIntPtr (context);
			var obj = gch.Target as Tuple<Action<long>, DispatchQueue>;
			gch.Free ();
			if (obj != null) {
				var sc = SynchronizationContext.Current;

				// Set GCD synchronization context. Mainly used when await executes inside GCD to continue
				// execution on same dispatch queue. Set the context only when there is no user context
				// set, including UIKitSynchronizationContext
				//
				// This assumes that only 1 queue can run on thread at the same time
				//
				if (sc == null)
					SynchronizationContext.SetSynchronizationContext (new DispatchQueueSynchronizationContext (obj.Item2));

				try {
					obj.Item1 ((long) count);
				} finally {
					if (sc == null)
						SynchronizationContext.SetSynchronizationContext (null);
				}
			}

		}
						     
		internal static readonly dispatch_callback_t free_gchandle = static_free_gchandle;

		[MonoPInvokeCallback (typeof (dispatch_callback_t))]
		static void static_free_gchandle (IntPtr context)
		{
			GCHandle.FromIntPtr (context).Free ();
		}

		public void DispatchAsync (Action action)
		{
			if (action == null)
				throw new ArgumentNullException ("action");
			
			dispatch_async_f (Handle, (IntPtr) GCHandle.Alloc (Tuple.Create (action, this)), static_dispatch);
		}

		public void DispatchSync (Action action)
		{
			if (action == null)
				throw new ArgumentNullException ("action");
			
			dispatch_sync_f (Handle, (IntPtr) GCHandle.Alloc (Tuple.Create (action, this)), static_dispatch);
		}

		public void DispatchBarrierAsync (Action action)
		{
			if (action == null)
				throw new ArgumentNullException ("action");
			
			dispatch_barrier_async_f (Handle, (IntPtr) GCHandle.Alloc (Tuple.Create (action, this)), static_dispatch);
		}

		public void DispatchBarrierSync (Action action)
		{
			if (action == null)
				throw new ArgumentNullException (nameof (action));

			dispatch_barrier_sync_f (Handle, (IntPtr) GCHandle.Alloc (Tuple.Create (action, this)), static_dispatch);
		}

		public void DispatchAfter (DispatchTime when, Action action)
		{
			if (action == null)
				throw new ArgumentNullException ("action");

			dispatch_after_f (when.Nanoseconds, Handle, (IntPtr) GCHandle.Alloc (Tuple.Create (action, this)), static_dispatch);
		}

		public void Submit (Action<int> action, long times)
		{
			if (action == null)
				throw new ArgumentNullException ("action");
			dispatch_apply_f ((IntPtr) times, Handle, (IntPtr) GCHandle.Alloc (Tuple.Create (action, this)), static_dispatch_iterations);
		}
		
		public void SetSpecific (IntPtr key, object context)
		{
			dispatch_queue_set_specific (GetCheckedHandle (), key, (IntPtr) GCHandle.Alloc (context), free_gchandle);
		}

		public object GetSpecific (IntPtr key)
		{
			GCHandle gchandle = (GCHandle) dispatch_queue_get_specific (GetCheckedHandle (), key);
			return gchandle.Target;
		}

		public DispatchQualityOfService GetQualityOfService (out int relative_priority)
		{
			unsafe {
				fixed (int* rel_pri = &relative_priority)
					return dispatch_queue_get_qos_class (Handle, rel_pri);
			}
		}

		public DispatchQualityOfService QualityOfService {
			get {
				unsafe {
					return dispatch_queue_get_qos_class (Handle, null);
				}
			}
		}

		//
		// Native methods
		//
		[DllImport (Constants.libcLibrary)]
		extern static IntPtr dispatch_queue_create (string label, IntPtr attr);

		[DllImport (Constants.libcLibrary)]
		extern static IntPtr dispatch_queue_create_with_target (string label, IntPtr attr, IntPtr target);

		[DllImport (Constants.libcLibrary)]
		extern static void dispatch_async_f (IntPtr queue, IntPtr context, dispatch_callback_t dispatch);

		[DllImport (Constants.libcLibrary)]
		extern static void dispatch_sync_f (IntPtr queue, IntPtr context, dispatch_callback_t dispatch);

		[DllImport (Constants.libcLibrary)]
		extern static void dispatch_barrier_async_f (IntPtr queue, IntPtr context, dispatch_callback_t dispatch);

		[DllImport(Constants.libcLibrary)]
		extern static void dispatch_barrier_sync_f (IntPtr queue, IntPtr context, dispatch_callback_t dispatch);

		[DllImport (Constants.libcLibrary)]
		extern static void dispatch_after_f (/* dispath_time_t */ ulong time, IntPtr queue, IntPtr context, dispatch_callback_t dispatch);

		[DllImport (Constants.libcLibrary)]
		extern static IntPtr dispatch_get_current_queue ();

		[DllImport (Constants.libcLibrary)]
		// dispatch_queue_t dispatch_get_global_queue (long priority, unsigned long flags);
		extern static IntPtr dispatch_get_global_queue (nint priority, nuint flags);

		[DllImport (Constants.libcLibrary)]
		// this returns a "const char*" so we cannot make a string out of it since it will be freed (and crash)
		extern static IntPtr dispatch_queue_get_label (IntPtr queue);

		[DllImport(Constants.libcLibrary)]
		extern static void dispatch_queue_set_specific (IntPtr queue, /* const void* */ IntPtr key, /* void *_Nullable */ IntPtr context, dispatch_callback_t /* _Nullable */ destructor);

		[DllImport(Constants.libcLibrary)]
		extern static IntPtr dispatch_queue_get_specific (IntPtr queue, /* const void* */ IntPtr key);

		[DllImport (Constants.libcLibrary)]
		unsafe extern static /* dispatch_qos_class_t */ DispatchQualityOfService dispatch_queue_get_qos_class (/* dispatch_queue_t */ IntPtr queue, /* int *_Nullable */ int* relative_priority);

		public override bool Equals (object other)
		{
			DispatchQueue o = other as DispatchQueue;
			if (o == null)
				return false;
			return (o.Handle == Handle);
		}

		public static bool operator == (DispatchQueue left, DispatchQueue right)
		{
			if ((object) left == null)
				return (object) right == null;
			return left.Equals (right);
		}

		public static bool operator != (DispatchQueue left, DispatchQueue right)
		{
			if ((object) left == null)
				return (object) right != null;
			return !left.Equals (right);
		}

		public override int GetHashCode ()
		{
			return (int) Handle;
		}
		
#if MONOMAC
		//
		// Not to be used by apps that use UIApplicationMain, NSApplicationMain or CFRunLoopRun,
		// so not available on Monotouch
		//
		[DllImport (Constants.libcLibrary)]
		static extern IntPtr dispatch_main ();

		public static void MainIteration ()
		{
			dispatch_main ();
		}
#endif

		public class Attributes
		{
			public bool Concurrent { get; set; }

			public bool IsInitiallyInactive { get; set; }

			public AutoreleaseFrequency? AutoreleaseFrequency { get; set; }

			public int RelativePriority { get; set; }

			public DispatchQualityOfService? QualityOfService { get; set; }

			internal IntPtr Create ()
			{
				IntPtr rv = IntPtr.Zero;

				if (Concurrent)
					rv = DispatchQueue.ConcurrentQueue;

				if (IsInitiallyInactive)
					rv = dispatch_queue_attr_make_initially_inactive (rv);

				if (AutoreleaseFrequency.HasValue)
					rv = dispatch_queue_attr_make_with_autorelease_frequency (rv, (nuint) (ulong) AutoreleaseFrequency.Value);

				if (QualityOfService.HasValue)
					rv = dispatch_queue_attr_make_with_qos_class (rv, QualityOfService.Value, RelativePriority);

				return rv;
			}

			[DllImport (Constants.libcLibrary)]
			static extern /* dispatch_queue_attr_t */ IntPtr dispatch_queue_attr_make_initially_inactive (/* dispatch_queue_attr_t _Nullable */ IntPtr attr);

			[DllImport (Constants.libcLibrary)]
			static extern /* dispatch_queue_attr_t */ IntPtr dispatch_queue_attr_make_with_autorelease_frequency (/* dispatch_queue_attr_t _Nullable */ IntPtr attr, /* dispatch_autorelease_frequency_t */ nuint frequency);

			[DllImport (Constants.libcLibrary)]
			static extern /* dispatch_queue_attr_t */ IntPtr dispatch_queue_attr_make_with_qos_class (/* dispatch_queue_attr_t _Nullable */ IntPtr attr, /* dispatch_qos_class_t */ DispatchQualityOfService qos_class, int relative_priority);
		}

		[Mac (10,12)]
		[iOS (10,0)]
		[TV (10,0)]
		[Watch (3,0)]
		[Native]
		public enum AutoreleaseFrequency : ulong /* unsigned long */
		{
			Inherit = 0,
			WorkItem = 1,
			Never = 2,
		}
#endif // !COREBUILD
	}

	// Some insights from: https://opensource.apple.com/source/libdispatch/libdispatch-442.1.4/src/time.c
	
	public struct DispatchTime
	{
#if !COREBUILD
		public static readonly DispatchTime Now = new DispatchTime ();
		public static readonly DispatchTime Forever = new DispatchTime (ulong.MaxValue);

		public DispatchTime (ulong nanoseconds)
			: this ()
		{
			Nanoseconds = nanoseconds;
		}

		public DispatchTime (DispatchTime when, long deltaNanoseconds)
			: this ()
		{
			Nanoseconds = dispatch_time (when.Nanoseconds, deltaNanoseconds);
		}

		public DispatchTime (DispatchTime when, TimeSpan delta) : this ()
		{
			Nanoseconds = dispatch_time (when.Nanoseconds, delta.Ticks * 100);
		}

		public ulong Nanoseconds { get; private set; }

		public DispatchTime WallTime {
			get {
				// This gives us access to setting the time to _dispatch_get_nanoseconds.
				// We can create the alternative dispatch_walltime with a time by setting a
				// negative value for nanoseconds.
				return new DispatchTime (dispatch_walltime (IntPtr.Zero, 0));
			}
		}

		[DllImport (Constants.libcLibrary)]
		extern static ulong dispatch_time (/* dispatch_time_t */ ulong when, long delta);

		[DllImport (Constants.libcLibrary)]
		extern static ulong dispatch_walltime (IntPtr /* struct timespec */ when, long delta);
#endif // !COREBUILD
	}

	public class DispatchGroup : DispatchObject
	{
#if !COREBUILD
		private DispatchGroup (IntPtr handle, bool owns)
			: base (handle, owns)
		{
		}

		public static DispatchGroup Create ()
		{
			var ptr = dispatch_group_create ();
			if (ptr == IntPtr.Zero)
				return null;

			return new DispatchGroup (ptr, true);
		}

		public void DispatchAsync (DispatchQueue queue, Action action)
		{
			if (queue == null)
				throw new ArgumentNullException ("queue");
			if (action == null)
				throw new ArgumentNullException ("action");

			dispatch_group_async_f (GetCheckedHandle (), queue.Handle, (IntPtr) GCHandle.Alloc (Tuple.Create (action, queue)), DispatchQueue.static_dispatch);
		}

		public void Notify (DispatchQueue queue, Action action)
		{
			if (queue == null)
				throw new ArgumentNullException ("queue");
			if (action == null)
				throw new ArgumentNullException ("action");

			dispatch_group_notify_f (GetCheckedHandle (), queue.Handle, (IntPtr) GCHandle.Alloc (Tuple.Create (action, queue)), DispatchQueue.static_dispatch);
		}

		public void Enter ()
		{
			dispatch_group_enter (GetCheckedHandle ());
		}

		public void Leave ()
		{
			dispatch_group_leave (GetCheckedHandle ());
		}

		public bool Wait (DispatchTime timeout)
		{
			return dispatch_group_wait (GetCheckedHandle (), timeout.Nanoseconds) == 0;
		}

		[DllImport (Constants.libcLibrary)]
		extern static IntPtr dispatch_group_create ();

		[DllImport (Constants.libcLibrary)]
		extern static void dispatch_group_async_f (IntPtr group, IntPtr queue, IntPtr context, DispatchQueue.dispatch_callback_t block);

		[DllImport (Constants.libcLibrary)]
		extern static void dispatch_group_notify_f (IntPtr group, IntPtr queue, IntPtr context, DispatchQueue.dispatch_callback_t block);

		[DllImport (Constants.libcLibrary)]
		extern static void dispatch_group_enter (IntPtr group);

		[DllImport (Constants.libcLibrary)]
		extern static void dispatch_group_leave (IntPtr group);

		[DllImport (Constants.libcLibrary)]
		extern static nint dispatch_group_wait (IntPtr group, ulong timeout);
#endif // !COREBUILD
	}
}
