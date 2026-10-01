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

#nullable enable

using System.ComponentModel;
using System.Threading;

namespace CoreFoundation {

	// The native constants are defined in usr/include/dispatch/queue.h, but since they're
	// not in any enum, they're untyped.
	/// <summary>An enumeration whose values define priorities available to <see cref="CoreFoundation.DispatchQueue" />s.</summary>
	///     <remarks>To be added.</remarks>
	public enum DispatchQueuePriority : int {
		/// <summary>To be added.</summary>
		High = 2,
		/// <summary>To be added.</summary>
		Default = 0,
		/// <summary>To be added.</summary>
		Low = -2,
		/// <summary>To be added.</summary>
		Background = Int16.MinValue,
	}

	// dispatch_qos_class_t is defined in usr/include/dispatch/queue.h, but redirects to qos_class_t
	// the qos_class_t enum is defined in usr/include/sys/qos.h (typed as 'unsigned int')
	/// <summary>To be added.</summary>
	///     <remarks>To be added.</remarks>
	public enum DispatchQualityOfService : uint {
		/// <summary>To be added.</summary>
		UserInteractive = 0x21,
		/// <summary>To be added.</summary>
		UserInitiated = 0x19,
		/// <summary>To be added.</summary>
		Default = 0x15,
		/// <summary>To be added.</summary>
		Utility = 0x11,
		/// <summary>To be added.</summary>
		Background = 0x09,
		/// <summary>To be added.</summary>
		Unspecified = 0x00,
	}

	/// <include file="../../docs/api/CoreFoundation/DispatchObject.xml" path="/Documentation/Docs[@DocId='T:CoreFoundation.DispatchObject']/*" />
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public abstract class DispatchObject : NativeObject {
#if !COREBUILD
		//
		// Constructors and lifecycle
		//
		[Preserve (Conditional = true)]
		internal DispatchObject (NativeHandle handle, bool owns)
			: base (handle, owns, verify: true)
		{
		}

		internal DispatchObject ()
		{
		}

		[DllImport (Constants.libcLibrary)]
		extern static IntPtr dispatch_release (IntPtr o);

		[DllImport (Constants.libcLibrary)]
		extern static IntPtr dispatch_retain (IntPtr o);

		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		protected internal override void Retain ()
		{
			dispatch_retain (Handle);
		}

		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		protected internal override void Release ()
		{
			dispatch_release (Handle);
		}

		[DllImport (Constants.libcLibrary)]
		extern static void dispatch_set_target_queue (/* dispatch_object_t */ IntPtr queue, /* dispatch_queue_t */ IntPtr target);

		/// <param name="queue">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public void SetTargetQueue (DispatchQueue queue)
		{
			// note: null is allowed because DISPATCH_TARGET_QUEUE_DEFAULT is defined as NULL (dispatch/queue.h)
			IntPtr q = queue.GetHandle ();
			GC.KeepAlive (queue);
			dispatch_set_target_queue (Handle, q);
		}

		[DllImport (Constants.libcLibrary)]
		internal extern static void dispatch_resume (IntPtr o);

		[DllImport (Constants.libcLibrary)]
		internal extern static void dispatch_suspend (IntPtr o);

		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		public void Activate ()
		{
			dispatch_activate (GetCheckedHandle ());
		}

		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.libcLibrary)]
		extern static void dispatch_activate (/* dispatch_object_t */ IntPtr @object);
#endif // !COREBUILD
	}

	/// <include file="../../docs/api/CoreFoundation/DispatchQueue.xml" path="/Documentation/Docs[@DocId='T:CoreFoundation.DispatchQueue']/*" />
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public sealed class DispatchQueue : DispatchObject {
#if !COREBUILD
		[Preserve (Conditional = true)]
		internal DispatchQueue (NativeHandle handle, bool owns) : base (handle, owns)
		{
		}

		/// <param name="label">Name for the dispatch queue, as a convention, use reverse-style DNS names for your queue name.</param>
		///         <summary>Creates a named dispatch queue that serializes all
		/// 	submitted blocks.</summary>
		///         <remarks>
		///           <para>
		/// 	    Creates a dispatching queue that executes code blocks
		/// 	    serially.   
		/// 	  </para>
		///           <para>
		/// 	    If you want to create a dispatch queue that can execute
		/// 	    the submitted code concurrently, use the constructor that
		/// 	    takes a boolean "concurrent" argument. 
		/// 	  </para>
		///         </remarks>
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

		/// <include file="../../docs/api/CoreFoundation/DispatchQueue.xml" path="/Documentation/Docs[@DocId='M:CoreFoundation.DispatchQueue.#ctor(System.String,System.Boolean)']/*" />
		public DispatchQueue (string label, bool concurrent)
				: base (dispatch_queue_create (label, concurrent ? ConcurrentQueue : IntPtr.Zero), true)
		{
			if (Handle == IntPtr.Zero)
				throw new Exception ("Error creating dispatch queue");
		}

		/// <param name="label">To be added.</param>
		///         <param name="attributes">To be added.</param>
		///         <param name="target">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		public DispatchQueue (string label, Attributes attributes, DispatchQueue? target = null)
			: base (dispatch_queue_create_with_target (label, attributes?.Create () ?? IntPtr.Zero, target.GetHandle ()), true)
		{
			GC.KeepAlive (target);
		}

		//
		// Properties and methods
		//

		/// <summary>Returns the label for this DispatchQueue.</summary>
		///         <value>
		///         </value>
		///         <remarks>This is the same name that was provided when the queue was constructed</remarks>
		public string? Label {
			get {
				return Marshal.PtrToStringAnsi (dispatch_queue_get_label (GetCheckedHandle ()));
			}
		}

		/// <summary>Label for the current queue.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		public static string? CurrentQueueLabel {
			get {
				return Marshal.PtrToStringAnsi (dispatch_queue_get_label (IntPtr.Zero));
			}
		}

		/// <summary>Suspends the execution of the queue.</summary>
		///         <remarks>Suspend and Resume calls should be always balanced.</remarks>
		public void Suspend ()
		{
			dispatch_suspend (GetCheckedHandle ());
		}

		/// <summary>Resumes execution of the queue.</summary>
		///         <remarks>Resume and Suspend calls should be always balanced.</remarks>
		public void Resume ()
		{
			dispatch_resume (GetCheckedHandle ());
		}

		[DllImport (Constants.libcLibrary)]
		extern static IntPtr dispatch_get_context (IntPtr o);

		[DllImport (Constants.libcLibrary)]
		extern static void dispatch_set_context (IntPtr o, IntPtr ctx);

		[DllImport (Constants.libcLibrary)]
		extern unsafe static void dispatch_apply_f (IntPtr iterations, IntPtr queue, IntPtr ctx, delegate* unmanaged<IntPtr, IntPtr, void> dispatch);

		/// <summary>User defined context information attachech to a DispatchQueue.</summary>
		///         <value>
		///         </value>
		///         <remarks>You can use the Context property on a DispatchQueue to store state that your application might want to associate with it.</remarks>
		public IntPtr Context {
			get {
				return dispatch_get_context (GetCheckedHandle ());
			}
			set {
				dispatch_set_context (GetCheckedHandle (), value);
			}
		}

		/// <summary>Developers should not use this deprecated property. </summary>
		///         <value>The current dispatch queue if invoked from code that
		///         was queued into a DispatchQueue, otherwise it returns the same
		///         queue as <see cref="CoreFoundation.DispatchQueue.MainQueue" />.</value>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.9")]
		[ObsoletedOSPlatform ("ios6.0")]
		public static DispatchQueue CurrentQueue {
			get {
				return new DispatchQueue (dispatch_get_current_queue (), false);
			}
		}

		/// <param name="priority">Determines the priority of the queue to be returned.</param>
		///         <summary>Returns one of the global dispatch queues based on the requested priority.</summary>
		///         <returns>The queue priority.</returns>
		///         <remarks>
		///           <para>
		/// 	    Unlike the main queue or queues allocated with the named
		/// 	    DispatchQueue constructor, the global concurrent queues
		/// 	    schedule blocks as soon as threads become available
		/// 	    (non-FIFO completion order). The global concurrent queues
		/// 	    represent three priority bands: DispatchQueuePriority.High, DispatchQueuePriority.Default and DispatchQueuePriority.Low.
		/// 	  </para>
		///           <para>
		/// 	    Tasks submitted to the high priority global queue will be invoked before those submitted to the
		/// 	    default or low priority global queues. Blocks submitted to the low priority global queue will only be
		/// 	    invoked if no blocks are pending on the default or high priority queues.
		/// 	  </para>
		///         </remarks>
		public static DispatchQueue GetGlobalQueue (DispatchQueuePriority priority)
		{
			return new DispatchQueue (dispatch_get_global_queue ((nint) (int) priority, 0), false);
		}

		public static DispatchQueue GetGlobalQueue (DispatchQualityOfService service)
		{
			return new DispatchQueue (dispatch_get_global_queue ((nint) (int) service, 0), false);
		}

		/// <summary>Returns the default global queue, which is one of the built-in queues at the default priority.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public static DispatchQueue DefaultGlobalQueue {
			get {
				return new DispatchQueue (dispatch_get_global_queue ((nint) (int) DispatchQueuePriority.Default, 0), false);
			}
		}

		static IntPtr main_q;

		/// <summary>Returns the main global queue.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///
		/// 	  The dispatch framework provides a default serial queue for
		/// 	  the application to use.  If you are using this on Xamarin.Mac Framework
		/// 	  without using AppKit, you must invoke the MainIteration
		/// 	  method to run the main dispatch queue.
		///
		/// 	</remarks>
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
		[UnmanagedCallersOnly]
		internal static void static_dispatcher_to_managed (IntPtr context)
		{
			GCHandle gch = GCHandle.FromIntPtr (context);
			var obj = gch.Target as Tuple<Action, DispatchQueue>;
			gch.Free ();
			if (obj is not null) {
				var sc = SynchronizationContext.Current;

				// Set GCD synchronization context. Mainly used when await executes inside GCD to continue
				// execution on same dispatch queue. Set the context only when there is no user context
				// set, including UIKitSynchronizationContext
				//
				// This assumes that only 1 queue can run on thread at the same time
				//
				if (sc is null)
					SynchronizationContext.SetSynchronizationContext (new DispatchQueueSynchronizationContext (obj.Item2));

				try {
					obj.Item1 ();
				} finally {
					if (sc is null)
						SynchronizationContext.SetSynchronizationContext (null);
				}
			}

		}

		[UnmanagedCallersOnly]
		static void static_dispatcher_iterations_to_managed (IntPtr context, IntPtr count)
		{
			GCHandle gch = GCHandle.FromIntPtr (context);
			var obj = gch.Target as Tuple<Action<long>, DispatchQueue>;
			gch.Free ();
			if (obj is not null) {
				var sc = SynchronizationContext.Current;

				// Set GCD synchronization context. Mainly used when await executes inside GCD to continue
				// execution on same dispatch queue. Set the context only when there is no user context
				// set, including UIKitSynchronizationContext
				//
				// This assumes that only 1 queue can run on thread at the same time
				//
				if (sc is null)
					SynchronizationContext.SetSynchronizationContext (new DispatchQueueSynchronizationContext (obj.Item2));

				try {
					obj.Item1 ((long) count);
				} finally {
					if (sc is null)
						SynchronizationContext.SetSynchronizationContext (null);
				}
			}

		}

		[UnmanagedCallersOnly]
		static void static_free_gchandle (IntPtr context)
		{
			GCHandle.FromIntPtr (context).Free ();
		}

		/// <param name="action">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public void DispatchAsync (Action action)
		{
			if (action is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (action));
			unsafe {
				dispatch_async_f (Handle, (IntPtr) GCHandle.Alloc (Tuple.Create (action, this)), &static_dispatcher_to_managed);
			}
		}

		/// <param name="block">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public void DispatchAsync (DispatchBlock block)
		{
			if (block is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (block));

			dispatch_async (GetCheckedHandle (), block.GetCheckedHandle ());
			GC.KeepAlive (block);
		}

		/// <param name="action">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public void DispatchSync (Action action)
		{
			if (action is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (action));

			unsafe {
				dispatch_sync_f (Handle, (IntPtr) GCHandle.Alloc (Tuple.Create (action, this)), &static_dispatcher_to_managed);
			}
		}

		/// <param name="block">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public void DispatchSync (DispatchBlock block)
		{
			if (block is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (block));

			dispatch_sync (GetCheckedHandle (), block.GetCheckedHandle ());
			GC.KeepAlive (block);
		}

		/// <param name="action">Code block to submit as a barrier.</param>
		///         <summary>Submits a barrier block for asynchronous execution on a dispatch queue</summary>
		///         <remarks>
		///           <para>
		/// 	    Submits a block to a dispatch queue like <see cref="CoreFoundation.DispatchQueue.DispatchAsync(System.Action)" />
		/// 	    does and marks that block as a barrier.  
		/// 	  </para>
		///           <para>
		/// 	    This is only relevant for concurrent queues.
		/// 	  </para>
		///           <para>
		/// 	    The  submitted code block will wait for all
		/// 	    pending concurrent blocks to complete execution, then it
		/// 	    will execute the code block to completion.   During the
		/// 	    time that the barrier executes, any other code blocks
		/// 	    submitted are queued, and will be scheduled to run
		/// 	    (possibly concurrently) after the barrier method completes.
		/// 	  </para>
		///         </remarks>
		public void DispatchBarrierAsync (Action action)
		{
			if (action is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (action));

			unsafe {
				dispatch_barrier_async_f (Handle, (IntPtr) GCHandle.Alloc (Tuple.Create (action, this)), &static_dispatcher_to_managed);
			}
		}

		/// <param name="block">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public void DispatchBarrierAsync (DispatchBlock block)
		{
			if (block is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (block));

			dispatch_barrier_async (GetCheckedHandle (), block.GetCheckedHandle ());
			GC.KeepAlive (block);
		}

		/// <param name="action">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public void DispatchBarrierSync (Action action)
		{
			if (action is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (action));

			unsafe {
				dispatch_barrier_sync_f (Handle, (IntPtr) GCHandle.Alloc (Tuple.Create (action, this)), &static_dispatcher_to_managed);
			}
		}

		/// <param name="block">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public void DispatchBarrierSync (DispatchBlock block)
		{
			if (block is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (block));

			dispatch_barrier_sync (GetCheckedHandle (), block.GetCheckedHandle ());
			GC.KeepAlive (block);
		}

		/// <param name="when">Time at which the code block will be executed.</param>
		///         <param name="action">Code block to execute at some time in the
		/// 	future.</param>
		///         <summary>Executes this time on or after the specified time.</summary>
		///         <remarks>To be added.</remarks>
		public void DispatchAfter (DispatchTime when, Action action)
		{
			if (action is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (action));
			unsafe {
				dispatch_after_f (when.Nanoseconds, Handle, (IntPtr) GCHandle.Alloc (Tuple.Create (action, this)), &static_dispatcher_to_managed);
			}
		}

		/// <param name="when">To be added.</param>
		///         <param name="block">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public void DispatchAfter (DispatchTime when, DispatchBlock block)
		{
			if (block is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (block));

			dispatch_after (when.Nanoseconds, GetCheckedHandle (), block.GetCheckedHandle ());
			GC.KeepAlive (block);
		}

		/// <param name="action">To be added.</param>
		///         <param name="times">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public void Submit (Action<int> action, long times)
		{
			if (action is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (action));
			unsafe {
				dispatch_apply_f ((IntPtr) times, Handle, (IntPtr) GCHandle.Alloc (Tuple.Create (action, this)), &static_dispatcher_iterations_to_managed);
			}
		}

		/// <param name="key">To be added.</param>
		///         <param name="context">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public void SetSpecific (IntPtr key, object context)
		{
			unsafe {
				dispatch_queue_set_specific (GetCheckedHandle (), key, (IntPtr) GCHandle.Alloc (context), &static_free_gchandle);
			}
		}

		/// <param name="key">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public object? GetSpecific (IntPtr key)
		{
			GCHandle gchandle = (GCHandle) dispatch_queue_get_specific (GetCheckedHandle (), key);
			return gchandle.Target;
		}

		/// <param name="relative_priority">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		public DispatchQualityOfService GetQualityOfService (out int relative_priority)
		{
			unsafe {
				fixed (int* rel_pri = &relative_priority)
					return dispatch_queue_get_qos_class (Handle, rel_pri);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		public DispatchQualityOfService QualityOfService {
			get {
				unsafe {
					return dispatch_queue_get_qos_class (Handle, null);
				}
			}
		}

		static IntPtr dispatch_queue_create (string label, IntPtr attr)
		{
			using var labelPtr = new TransientString (label);
			return dispatch_queue_create (labelPtr, attr);
		}
		//
		// Native methods
		//
		[DllImport (Constants.libcLibrary)]
		extern static IntPtr dispatch_queue_create (IntPtr label, IntPtr attr);

		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		static IntPtr dispatch_queue_create_with_target (string label, IntPtr attr, IntPtr target)
		{
			using var labelPtr = new TransientString (label);
			return dispatch_queue_create_with_target (labelPtr, attr, target);
		}

		[DllImport (Constants.libcLibrary, EntryPoint = "dispatch_queue_create_with_target$V2")]
		extern static IntPtr dispatch_queue_create_with_target (IntPtr label, IntPtr attr, IntPtr target);

		[DllImport (Constants.libcLibrary)]
		extern unsafe static void dispatch_async_f (IntPtr queue, IntPtr context, delegate* unmanaged<IntPtr, void> dispatch);

		[DllImport (Constants.libcLibrary)]
		extern static void dispatch_async (IntPtr queue, IntPtr block);

		[DllImport (Constants.libcLibrary)]
		extern unsafe static void dispatch_sync_f (IntPtr queue, IntPtr context, delegate* unmanaged<IntPtr, void> dispatch);

		[DllImport (Constants.libcLibrary)]
		extern static void dispatch_sync (IntPtr queue, IntPtr block);

		[DllImport (Constants.libcLibrary)]
		extern unsafe static void dispatch_barrier_async_f (IntPtr queue, IntPtr context, delegate* unmanaged<IntPtr, void> dispatch);

		[DllImport (Constants.libcLibrary)]
		extern static void dispatch_barrier_async (IntPtr queue, IntPtr block);

		[DllImport (Constants.libcLibrary)]
		extern unsafe static void dispatch_barrier_sync_f (IntPtr queue, IntPtr context, delegate* unmanaged<IntPtr, void> dispatch);

		[DllImport (Constants.libcLibrary)]
		extern static void dispatch_barrier_sync (IntPtr queue, IntPtr block);

		[DllImport (Constants.libcLibrary)]
		extern unsafe static void dispatch_after_f (/* dispath_time_t */ ulong time, IntPtr queue, IntPtr context, delegate* unmanaged<IntPtr, void> dispatch);

		[DllImport (Constants.libcLibrary)]
		extern static void dispatch_after (/* dispath_time_t */ ulong time, IntPtr queue, IntPtr block);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.9")]
		[ObsoletedOSPlatform ("ios6.0")]
		[DllImport (Constants.libcLibrary)]
		extern static IntPtr dispatch_get_current_queue ();

		[DllImport (Constants.libcLibrary)]
		// dispatch_queue_t dispatch_get_global_queue (long identifier, unsigned long flags);
		extern static IntPtr dispatch_get_global_queue (nint identifier, nuint flags);

		[DllImport (Constants.libcLibrary)]
		// this returns a "const char*" so we cannot make a string out of it since it will be freed (and crash)
		extern static IntPtr dispatch_queue_get_label (IntPtr queue);

		[DllImport (Constants.libcLibrary)]
		extern unsafe static void dispatch_queue_set_specific (IntPtr queue, /* const void* */ IntPtr key, /* void *_Nullable */ IntPtr context, delegate* unmanaged<IntPtr, void> /* _Nullable */ destructor);

		[DllImport (Constants.libcLibrary)]
		extern static IntPtr dispatch_queue_get_specific (IntPtr queue, /* const void* */ IntPtr key);

		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[DllImport (Constants.libcLibrary)]
		unsafe extern static /* dispatch_qos_class_t */ DispatchQualityOfService dispatch_queue_get_qos_class (/* dispatch_queue_t */ IntPtr queue, /* int *_Nullable */ int* relative_priority);

#if MONOMAC
		//
		// Not to be used by apps that use UIApplicationMain, NSApplicationMain or CFRunLoopRun,
		// so not available on Monotouch
		//
		[DllImport (Constants.libcLibrary)]
		static extern IntPtr dispatch_main ();

		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public static void MainIteration ()
		{
			dispatch_main ();
		}
#endif

		/// <summary>To be added.</summary>
		///     <remarks>To be added.</remarks>
		public class Attributes {
			/// <summary>To be added.</summary>
			///         <value>To be added.</value>
			///         <remarks>To be added.</remarks>
			public bool Concurrent { get; set; }

			/// <summary>To be added.</summary>
			///         <value>To be added.</value>
			///         <remarks>To be added.</remarks>
			[SupportedOSPlatform ("macos")]
			[SupportedOSPlatform ("maccatalyst")]
			[SupportedOSPlatform ("ios")]
			[SupportedOSPlatform ("tvos")]
			public bool IsInitiallyInactive { get; set; }

			/// <summary>To be added.</summary>
			///         <value>To be added.</value>
			///         <remarks>To be added.</remarks>
			[SupportedOSPlatform ("macos")]
			[SupportedOSPlatform ("maccatalyst")]
			[SupportedOSPlatform ("ios")]
			[SupportedOSPlatform ("tvos")]
			public AutoreleaseFrequency? AutoreleaseFrequency { get; set; }

			/// <summary>To be added.</summary>
			///         <value>To be added.</value>
			///         <remarks>To be added.</remarks>
			[SupportedOSPlatform ("macos")]
			[SupportedOSPlatform ("ios")]
			[SupportedOSPlatform ("tvos")]
			[SupportedOSPlatform ("maccatalyst")]
			public int RelativePriority { get; set; }

			/// <summary>To be added.</summary>
			///         <value>To be added.</value>
			///         <remarks>To be added.</remarks>
			[SupportedOSPlatform ("macos")]
			[SupportedOSPlatform ("ios")]
			[SupportedOSPlatform ("tvos")]
			[SupportedOSPlatform ("maccatalyst")]
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

			[SupportedOSPlatform ("macos")]
			[SupportedOSPlatform ("ios")]
			[SupportedOSPlatform ("tvos")]
			[DllImport (Constants.libcLibrary)]
			static extern /* dispatch_queue_attr_t */ IntPtr dispatch_queue_attr_make_initially_inactive (/* dispatch_queue_attr_t _Nullable */ IntPtr attr);

			[SupportedOSPlatform ("macos")]
			[SupportedOSPlatform ("ios")]
			[SupportedOSPlatform ("tvos")]
			[DllImport (Constants.libcLibrary)]
			static extern /* dispatch_queue_attr_t */ IntPtr dispatch_queue_attr_make_with_autorelease_frequency (/* dispatch_queue_attr_t _Nullable */ IntPtr attr, /* dispatch_autorelease_frequency_t */ nuint frequency);

			[SupportedOSPlatform ("macos")]
			[SupportedOSPlatform ("ios")]
			[DllImport (Constants.libcLibrary)]
			static extern /* dispatch_queue_attr_t */ IntPtr dispatch_queue_attr_make_with_qos_class (/* dispatch_queue_attr_t _Nullable */ IntPtr attr, /* dispatch_qos_class_t */ DispatchQualityOfService qos_class, int relative_priority);
		}

		/// <summary>To be added.</summary>
		///     <remarks>To be added.</remarks>
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[Native]
		public enum AutoreleaseFrequency : ulong /* unsigned long */
		{
			/// <summary>To be added.</summary>
			Inherit = 0,
			/// <summary>To be added.</summary>
			WorkItem = 1,
			/// <summary>To be added.</summary>
			Never = 2,
		}
#endif // !COREBUILD
	}

	// Some insights from: https://opensource.apple.com/source/libdispatch/libdispatch-442.1.4/src/time.c

	/// <summary>Dispatch time and time-out representation.</summary>
	///     <remarks>
	///       <para>The DispatchTime class provides a simple mechanism for expressing temporal milestones for use</para>
	///       <para>with dispatch functions that need timeouts or operate on a schedule.</para>
	///       <para>
	///       </para>
	///       <para>To create an absolute wall time, invoke the DispatchTime constructor with the number of nanoseconds for a particular point in time with a negative time.</para>
	///     </remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public struct DispatchTime {
#if !COREBUILD
		/// <summary>Represents no timeout.</summary>
		///         <remarks>
		///         </remarks>
		public static readonly DispatchTime Now = new DispatchTime ();
		/// <summary>Represents infinity time.</summary>
		///         <remarks>
		///         </remarks>
		public static readonly DispatchTime Forever = new DispatchTime (ulong.MaxValue);

		/// <param name="nanoseconds">The number of nanosecods.   If the value is positive, the returned milestone is relative.  If the number of nanoseconds is negative, then the milestone is an absolute wall clock time.</param>
		///         <summary>Creates new DispatchTime instance from nanoseconds</summary>
		///         <remarks>
		///         </remarks>
		public DispatchTime (ulong nanoseconds)
			: this ()
		{
			Nanoseconds = nanoseconds;
		}

		/// <param name="when">Reference dispatch time.</param>
		///         <param name="deltaNanoseconds">Nanoseconds to add to the dispatch time.</param>
		///         <summary>Creates a new dispatch time instance based on an existing dispatch time and a nanosecond delta.</summary>
		///         <remarks>
		///         </remarks>
		public DispatchTime (DispatchTime when, long deltaNanoseconds)
			: this ()
		{
			Nanoseconds = dispatch_time (when.Nanoseconds, deltaNanoseconds);
		}

		/// <param name="when">Reference dispatch time.</param>
		///         <param name="delta">Timespan to add to the dispatch time.</param>
		///         <summary>Creates a new dispatch time instance based on an existing dispatch time and a the specified delta.</summary>
		///         <remarks>To be added.</remarks>
		public DispatchTime (DispatchTime when, TimeSpan delta) : this ()
		{
			Nanoseconds = dispatch_time (when.Nanoseconds, delta.Ticks * 100);
		}

		/// <summary>The total number of nanoseconds represented by this instance.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public ulong Nanoseconds { get; private set; }

		/// <summary>Returns a milestone relative to a fixed point in time using the wall clock.</summary>
		///         <value>The wall time.</value>
		///         <remarks>
		///           <para />
		///         </remarks>
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

	/// <summary>Manages group of code blocks allows for aggregate synchronization.</summary>
	///     <remarks>Code block can be executed on different dispatch queues but managed as a group.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class DispatchGroup : DispatchObject {
#if !COREBUILD
		[Preserve (Conditional = true)]
		private DispatchGroup (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public DispatchGroup ()
			: base (dispatch_group_create (), true)
		{
		}

		/// <summary>Creates a new dispatch group.</summary>
		///         <returns>
		///         </returns>
		///         <remarks>
		///         </remarks>
		public static DispatchGroup? Create ()
		{
			var ptr = dispatch_group_create ();
			if (ptr == IntPtr.Zero)
				return null;

			return new DispatchGroup (ptr, true);
		}

		/// <param name="queue">The dispatch queue to which the block will be submitted for asynchronous invocation.</param>
		///         <param name="action">The action to invoke asynchronously.</param>
		///         <summary>Submits a block to a dispatch queue and associates the block with the given dispatch group.</summary>
		///         <remarks>Submits a block to a dispatch queue and associates the block with the given dispatch group. The dispatch group may be used to wait for the completion of the blocks it references.</remarks>
		public void DispatchAsync (DispatchQueue queue, Action action)
		{
			if (queue is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (queue));
			if (action is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (action));

			unsafe {
				dispatch_group_async_f (GetCheckedHandle (), queue.Handle, (IntPtr) GCHandle.Alloc (Tuple.Create (action, queue)), &DispatchQueue.static_dispatcher_to_managed);
				GC.KeepAlive (queue);
			}
		}

		/// <param name="queue">To be added.</param>
		///         <param name="block">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public void Notify (DispatchQueue queue, DispatchBlock block)
		{
			if (queue is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (queue));
			if (block is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (block));
			dispatch_group_notify (GetCheckedHandle (), queue.Handle, block.GetCheckedHandle ());
			GC.KeepAlive (queue);
			GC.KeepAlive (block);
		}

		/// <param name="queue">The dispatch queue to which the block will be submitted for asynchronous invocation.</param>
		///         <param name="action">The action to invoke when the group completes.</param>
		///         <summary>Schedule a block to be submitted to a queue when all the blocks associated with a group have completed.</summary>
		///         <remarks>
		///           <para>This function schedules a notification block to be submitted to the specified queue once all blocks associated with the dispatch group have completed.</para>
		///           <para>
		///           </para>
		///           <para>If no blocks are associated with the dispatch group (i.e. the group is empty) then the notification block will be submitted immediately.</para>
		///           <para>
		///           </para>
		///           <para>The group will be empty at the time the notification block is submitted to the target queue. </para>
		///         </remarks>
		public void Notify (DispatchQueue queue, Action action)
		{
			if (queue is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (queue));
			if (action is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (action));
			unsafe {
				dispatch_group_notify_f (GetCheckedHandle (), queue.Handle, (IntPtr) GCHandle.Alloc (Tuple.Create (action, queue)), &DispatchQueue.static_dispatcher_to_managed);
				GC.KeepAlive (queue);
			}
		}

		/// <summary>Explicitly sets that a code block is beeing managed by the group.</summary>
		///         <remarks>It can be used to manually manage dispatch group tasks by incrementing the current count of outstanding tasks in the group.</remarks>
		public void Enter ()
		{
			dispatch_group_enter (GetCheckedHandle ());
		}

		/// <summary>Releases a code block association with the group.</summary>
		///         <remarks>It can be used to manually manage dispatch group tasks by decrementing the current count of outstanding tasks in the group.</remarks>
		public void Leave ()
		{
			dispatch_group_leave (GetCheckedHandle ());
		}

		/// <include file="../../docs/api/CoreFoundation/DispatchGroup.xml" path="/Documentation/Docs[@DocId='M:CoreFoundation.DispatchGroup.Wait(CoreFoundation.DispatchTime)']/*" />
		public bool Wait (DispatchTime timeout)
		{
			return dispatch_group_wait (GetCheckedHandle (), timeout.Nanoseconds) == 0;
		}

		/// <param name="timeout">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public bool Wait (TimeSpan timeout)
		{
			return Wait (new DispatchTime (DispatchTime.Now, timeout));
		}

		[DllImport (Constants.libcLibrary)]
		extern static IntPtr dispatch_group_create ();

		[DllImport (Constants.libcLibrary)]
		extern unsafe static void dispatch_group_async_f (IntPtr group, IntPtr queue, IntPtr context, delegate* unmanaged<IntPtr, void> block);

		[DllImport (Constants.libcLibrary)]
		extern unsafe static void dispatch_group_notify_f (IntPtr group, IntPtr queue, IntPtr context, delegate* unmanaged<IntPtr, void> block);

		[DllImport (Constants.libcLibrary)]
		extern static void dispatch_group_notify (IntPtr group, IntPtr queue, IntPtr block);

		[DllImport (Constants.libcLibrary)]
		extern static void dispatch_group_enter (IntPtr group);

		[DllImport (Constants.libcLibrary)]
		extern static void dispatch_group_leave (IntPtr group);

		[DllImport (Constants.libcLibrary)]
		extern static nint dispatch_group_wait (IntPtr group, ulong timeout);
#endif // !COREBUILD
	}
}
