//
// DispatchSource.cs: DispatchSource objects
//
// Authors:
//   Miguel de Icaza (miguel@gnome.org)
//   Marek Safar (marek.safar@gmail.com)
//
// Copyright 2015 Xamarin Inc
//
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using ObjCRuntime;
using Foundation;
using dispatch_source_type_t=System.IntPtr;
using dispatch_source_t=System.IntPtr;
using dispatch_queue_t=System.IntPtr;

namespace CoreFoundation {

	[Flags]
	public enum MemoryPressureFlags { 
		Normal = 1, Warn = 2, Critical = 4
	}

	[Flags]
	public enum ProcessMonitorFlags : uint {
		Exit   = 0x80000000,
		Fork   = 0x40000000,
		Exec   = 0x20000000,
		Signal = 0x08000000
	}

	[Flags]
	public enum VnodeMonitorKind : uint {
		Delete = 1,
		Write = 2,
		Extend = 4,
		Attrib = 8,
		Link = 0x10,
		Rename = 0x20,
		Revoke = 0x40
	}
	
	public class DispatchSource : DispatchObject  {
		DispatchQueue queue;

		// constructors for use in bindings
		[Preserve (Conditional = true)]
		internal DispatchSource (IntPtr handle, bool owns) : base (handle, owns)
		{
		}

		// constructors for use in bindings
		internal DispatchSource (IntPtr handle) : base (handle, false)
		{
		}

		// Invoked by subclasses in this file that fully initialize both
		// queue and handle
		internal DispatchSource () {}
		

		[DllImport (Constants.libcLibrary)]
		extern static dispatch_source_t dispatch_source_create (dispatch_source_type_t type, IntPtr handle, IntPtr mask, dispatch_queue_t queue);

		[DllImport (Constants.libcLibrary)]
		extern static IntPtr dispatch_source_get_handle (dispatch_source_t source);

		[DllImport (Constants.libcLibrary)]
		extern static IntPtr dispatch_source_get_mask (dispatch_source_t source);
	
		[DllImport (Constants.libcLibrary)]
		extern static IntPtr dispatch_source_get_data (dispatch_source_t source);

		[DllImport (Constants.libcLibrary)]
		extern static IntPtr dispatch_source_merge_data (dispatch_source_t source, IntPtr data);

		[DllImport (Constants.libcLibrary)]
		unsafe extern static IntPtr dispatch_source_set_event_handler (dispatch_source_t source, IntPtr handler);

		[DllImport (Constants.libcLibrary)]
		unsafe extern static IntPtr dispatch_source_set_registration_handler (dispatch_source_t source, IntPtr handler);

		[DllImport (Constants.libcLibrary)]
		unsafe extern static IntPtr dispatch_source_set_cancel_handler (dispatch_source_t source, IntPtr handler);

		[DllImport (Constants.libcLibrary)]
		unsafe extern static IntPtr dispatch_source_set_event_handler_f (dispatch_source_t source, IntPtr handler);

		[DllImport (Constants.libcLibrary)]
		extern static IntPtr dispatch_source_cancel (dispatch_source_t source);
		
		[DllImport (Constants.libcLibrary)]
		extern static IntPtr dispatch_source_testcancel (dispatch_source_t source);

		public void SetEventHandler (Action handler)
		{
			if (handler == null){
				dispatch_source_set_event_handler_f (GetCheckedHandle (), IntPtr.Zero);
				return;
			}

			unsafe {
				DispatchBlock.Invoke (
					delegate {
						var sc = SynchronizationContext.Current;
						if (sc == null)
							SynchronizationContext.SetSynchronizationContext (new DispatchQueueSynchronizationContext (queue));
						try {
							handler ();
						} finally {
							if (sc == null)
								SynchronizationContext.SetSynchronizationContext (null);
						}
					}, block=> dispatch_source_set_event_handler (GetCheckedHandle (), block));
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
		
		public void SetRegistrationHandler (Action handler)
		{
			if (handler == null)
				throw new ArgumentNullException ("handler");

			unsafe {
				DispatchBlock.Invoke (
					delegate {
						var sc = SynchronizationContext.Current;
						if (sc == null)
							SynchronizationContext.SetSynchronizationContext (new DispatchQueueSynchronizationContext (queue));
						try {
							handler ();
						} finally {
							if (sc == null)
								SynchronizationContext.SetSynchronizationContext (null);
						}
					}, block => dispatch_source_set_registration_handler (GetCheckedHandle (), block));
			}
		}

		public void SetCancelHandler (Action handler)
		{
			if (handler == null)
				throw new ArgumentNullException ("handler");

			unsafe {
				DispatchBlock.Invoke (
					delegate {
						var sc = SynchronizationContext.Current;
						if (sc == null)
							SynchronizationContext.SetSynchronizationContext (new DispatchQueueSynchronizationContext (queue));
						try {
							handler ();
						} finally {
							if (sc == null)
								SynchronizationContext.SetSynchronizationContext (null);
						}
					}, block => dispatch_source_set_cancel_handler (GetCheckedHandle (), block));
			}
		}

		public void Cancel ()
		{
			dispatch_source_cancel (GetCheckedHandle ());
		}

		protected override void Dispose (bool disposing)
		{
			// Do not call the Cancel method here
			// This is important because the Dispathc Source might be electible for garbage collection
			// but might still have a side effect (for example, invoking a handler).
			base.Dispose (disposing);
		}
		
		public bool IsCanceled {
			get {
				return dispatch_source_testcancel (GetCheckedHandle ()) != IntPtr.Zero;
			}
		}
		
		public class Data : DispatchSource {
			internal Data () {}
			internal Data (IntPtr handle, bool owns) : base (handle, owns) {}

			public void MergeData (IntPtr value)
			{
				dispatch_source_merge_data (Handle, value);
			}

			public IntPtr PendingData {
				get {
					return dispatch_source_get_data (Handle);
				}
			}
		}
	
		public class DataAdd : Data {
			static IntPtr type_data_add;

			public DataAdd (IntPtr handle, bool owns) : base (handle, owns) { }
			public DataAdd (IntPtr handle) : base (handle, false) { }

			public DataAdd (DispatchQueue queue = null) 
			{
				if (type_data_add == IntPtr.Zero)
					type_data_add = Dlfcn.dlsym (Libraries.System.Handle, "_dispatch_source_type_data_add");

				this.queue = queue;
				var handle = dispatch_source_create (type_data_add,
								 handle: IntPtr.Zero,
								 mask:   IntPtr.Zero,
								 queue:  queue == null ? IntPtr.Zero : queue.Handle);
				if (handle != IntPtr.Zero)
					InitializeHandle (handle);
			}
		}
	
		public class DataOr : Data {
			static IntPtr type_data_or;

			public DataOr (IntPtr handle, bool owns) : base (handle, owns) { }
			public DataOr (IntPtr handle) : base (handle, false) { }
			
			public DataOr (DispatchQueue queue = null)
			{
				if (type_data_or == IntPtr.Zero)
					type_data_or = Dlfcn.dlsym (Libraries.System.Handle, "_dispatch_source_type_data_or");
				this.queue = queue;
				var handle = dispatch_source_create (type_data_or,
								 handle: IntPtr.Zero,
								 mask:   IntPtr.Zero,
								 queue:  queue == null ? IntPtr.Zero : queue.Handle);
				if (handle != IntPtr.Zero)
					InitializeHandle (handle);
			}
		}

		public class Mach : DispatchSource {
			internal Mach (IntPtr handle, bool owns) : base (handle, owns) { }
			internal Mach (IntPtr handle) : base (handle, false) { }
			internal Mach () 
			{ }
			
			public int MachPort {
				get {
					return (int) dispatch_source_get_handle (GetCheckedHandle ());
				}
			}
		}

		public class MachSend : Mach {
			static IntPtr type_mach_send;

			public MachSend (IntPtr handle, bool owns) : base (handle, owns) { }
			public MachSend (IntPtr handle) : base (handle, false) { }
			
			public MachSend (int machPort, bool sendDead = false, DispatchQueue queue = null)
			{
				if (type_mach_send == IntPtr.Zero)
					type_mach_send = Dlfcn.dlsym (Libraries.System.Handle, "_dispatch_source_type_mach_send");
				this.queue = queue;
				var handle = dispatch_source_create (type_mach_send,
								 handle: (IntPtr) machPort,
								 mask:   (IntPtr) (sendDead ? 1 : 0),
								 queue:  queue == null ? IntPtr.Zero : queue.Handle);
				if (handle != IntPtr.Zero)
					InitializeHandle (handle);
			}

			public bool SendRightsDestroyed  {
				get {
					return dispatch_source_get_data (GetCheckedHandle ()) != IntPtr.Zero;
				}
			}
		}
		public class MachReceive : DispatchSource {
			static IntPtr type_mach_recv;

			public MachReceive (IntPtr handle, bool owns) : base (handle, owns) { }
			public MachReceive (IntPtr handle) : base (handle, false) { }
			
			public MachReceive (int machPort, DispatchQueue queue = null)
			{
				if (type_mach_recv == IntPtr.Zero)
					type_mach_recv = Dlfcn.dlsym (Libraries.System.Handle, "_dispatch_source_type_mach_recv");
				this.queue = queue;
				var handle = dispatch_source_create (type_mach_recv,
								 handle: (IntPtr) machPort,
								 mask:   IntPtr.Zero,
								 queue:  queue == null ? IntPtr.Zero : queue.Handle);
				if (handle != IntPtr.Zero)
					InitializeHandle (handle);
			}
		}

		
		public class MemoryPressure : DispatchSource {
			static IntPtr type_memorypressure;
			public MemoryPressure (IntPtr handle, bool owns) : base (handle, owns){}
			public MemoryPressure (IntPtr handle) : base (handle, false){}
			
			public MemoryPressure (MemoryPressureFlags monitorFlags = MemoryPressureFlags.Normal | MemoryPressureFlags.Warn, DispatchQueue queue = null)
			{
				if (type_memorypressure == IntPtr.Zero)
					type_memorypressure = Dlfcn.dlsym (Libraries.System.Handle, "_dispatch_source_type_memorypressure");
				this.queue = queue;
				var handle = dispatch_source_create (type_memorypressure,
								 handle: IntPtr.Zero,
								 mask:   (IntPtr) monitorFlags,
								 queue:  queue == null ? IntPtr.Zero : queue.Handle);
				if (handle != IntPtr.Zero)
					InitializeHandle (handle);
			}

			public MemoryPressureFlags PressureFlags {
				get {
					return (MemoryPressureFlags) dispatch_source_get_data (GetCheckedHandle ());
				}
			}
		}

		public class ProcessMonitor : DispatchSource {
			static IntPtr type_proc;

			public ProcessMonitor (IntPtr handle, bool owns) : base (handle, owns){}
			public ProcessMonitor (IntPtr handle) : base (handle, false){}
			public ProcessMonitor (int processId, ProcessMonitorFlags monitorKind = ProcessMonitorFlags.Exit, DispatchQueue queue = null)
			{
				
				if (type_proc == IntPtr.Zero)
					type_proc = Dlfcn.dlsym (Libraries.System.Handle, "_dispatch_source_type_proc");
				this.queue = queue;
				var handle = dispatch_source_create (type_proc,
								 handle: (IntPtr) processId,
								 mask:   (IntPtr) monitorKind,
								 queue:  queue == null ? IntPtr.Zero : queue.Handle);
				if (handle != IntPtr.Zero)
					InitializeHandle (handle);
			}

			public int ProcessId {
				get {
					return (int) dispatch_source_get_handle (GetCheckedHandle ());
				}
			}

			public ProcessMonitorFlags MonitorFlags {
				get {
					return (ProcessMonitorFlags) dispatch_source_get_data (GetCheckedHandle ());
				}
			}
		}

		public class ReadMonitor : DispatchSource {
			static IntPtr type_read;
			public ReadMonitor (IntPtr handle, bool owns) : base (handle, owns){}
			public ReadMonitor (IntPtr handle) : base (handle, false){}
			public ReadMonitor (int fileDescriptor, DispatchQueue queue = null)
			{
				
				if (type_read == IntPtr.Zero)
					type_read = Dlfcn.dlsym (Libraries.System.Handle, "_dispatch_source_type_read");
				this.queue = queue;
				var handle = dispatch_source_create (type_read,
								 handle: (IntPtr) fileDescriptor,
								 mask:   IntPtr.Zero,
								 queue:  queue == null ? IntPtr.Zero : queue.Handle);
				if (handle != IntPtr.Zero)
					InitializeHandle (handle);
			}

			public int FileDescriptor {
				get {
					return (int) dispatch_source_get_handle (GetCheckedHandle ());
				}
			}

			public int BytesAvailable {
				get {
					return (int) dispatch_source_get_data (GetCheckedHandle ());
				}
			}
		}

		public class SignalMonitor : DispatchSource {
			static IntPtr type_signal;
			public SignalMonitor (IntPtr handle, bool owns) : base (handle, owns){}
			public SignalMonitor (IntPtr handle) : base (handle, false){}
			public SignalMonitor (int signalNumber, DispatchQueue queue = null)
			{
				if (type_signal == IntPtr.Zero)
					type_signal = Dlfcn.dlsym (Libraries.System.Handle, "_dispatch_source_type_signal");
				this.queue = queue;
				var handle = dispatch_source_create (type_signal,
								 handle: (IntPtr) signalNumber,
								 mask:   IntPtr.Zero,
								 queue:  queue == null ? IntPtr.Zero : queue.Handle);
				if (handle != IntPtr.Zero)
					InitializeHandle (handle);
			}

			public int SignalNumber {
				get {
					return (int) dispatch_source_get_handle (GetCheckedHandle ());
				}
			}

			public int SignalsDelivered {
				get {
					return (int) dispatch_source_get_data (GetCheckedHandle ());
				}
			}
		}
		
		public class Timer : DispatchSource {
			static IntPtr type_timer;
			public Timer (IntPtr handle, bool owns) : base (handle, owns){}
			public Timer (IntPtr handle) : base (handle, false){}
			public Timer (DispatchQueue queue = null) : this (false, queue) {}
				
			public Timer (bool strict = false, DispatchQueue queue = null)
			{
				if (type_timer == IntPtr.Zero)
					type_timer = Dlfcn.dlsym (Libraries.System.Handle, "_dispatch_source_type_timer");
				this.queue = queue;
				var handle = dispatch_source_create (type_timer,
								 handle: IntPtr.Zero,
								 mask: strict ? (IntPtr) 1 : IntPtr.Zero,
								 queue: queue == null ? IntPtr.Zero : queue.Handle);
				if (handle != IntPtr.Zero)
					InitializeHandle (handle);
			}

			public int TimerFiredCount {
				get {
					return (int) dispatch_source_get_data (GetCheckedHandle ());
				}
			}
			[DllImport (Constants.libcLibrary)]
			extern static void dispatch_source_set_timer (dispatch_source_t source, /* dispathc_time_t */ulong start, long interval, long leeway);

			public void SetTimer (DispatchTime time, long nanosecondInterval, long nanosecondLeeway)
			{
				dispatch_source_set_timer (GetCheckedHandle (), time.Nanoseconds, nanosecondInterval, nanosecondLeeway);
			}
		}
		
		public class VnodeMonitor : DispatchSource {
			static IntPtr type_vnode;

			// If different than -1, we opened the descriptor and must close it.
			int fd;
			
			public VnodeMonitor (IntPtr handle, bool owns) : base (handle, owns){}
			public VnodeMonitor (IntPtr handle) : base (handle, false){}
			
			public VnodeMonitor (int fileDescriptor, VnodeMonitorKind vnodeKind, DispatchQueue queue = null)
			{
				if (type_vnode == IntPtr.Zero)
					type_vnode = Dlfcn.dlsym (Libraries.System.Handle, "_dispatch_source_type_vnode");
				this.queue = queue;
				fd = -1;
				var handle = dispatch_source_create (type_vnode,
								 handle: (IntPtr) fileDescriptor,
								 mask:   (IntPtr) vnodeKind,
								 queue:  queue == null ? IntPtr.Zero : queue.Handle);
				if (handle != IntPtr.Zero)
					InitializeHandle (handle);
			}

			const int O_EVTONLY = 0x8000;
			[DllImport (Constants.libcLibrary, SetLastError = true)]
			extern static int open (string path, int flags);

			[DllImport (Constants.libcLibrary)]
			internal extern static int close (int fd);
			
			public VnodeMonitor (string path, VnodeMonitorKind vnodeKind, DispatchQueue queue = null)
			{
				if (path == null)
					throw new ArgumentNullException ("path");

				fd = open (path, O_EVTONLY);
				if (fd == -1)
					throw new IOException ("Failure to open the file", Marshal.GetLastWin32Error ());
				if (type_vnode == IntPtr.Zero)
					type_vnode = Dlfcn.dlsym (Libraries.System.Handle, "_dispatch_source_type_vnode");
				this.queue = queue;
				
				var handle = dispatch_source_create (type_vnode,
								 handle: (IntPtr) fd,
								 mask:   (IntPtr) vnodeKind,
								 queue:  queue == null ? IntPtr.Zero : queue.Handle);
				if (handle != IntPtr.Zero)
					InitializeHandle (handle);
			}

			protected override void Dispose (bool disposing)
			{
				if (fd != -1){
					close (fd);
					fd = -1;
				}
				base.Dispose (disposing);
			}

			public int FileDescriptor {
				get {
					return (int) dispatch_source_get_handle (GetCheckedHandle ());
				}
			}

			public VnodeMonitorKind ObservedEvents  {
				get {
					return (VnodeMonitorKind) (int) dispatch_source_get_data (GetCheckedHandle ());
				}
			}
				
		}

		public class WriteMonitor : DispatchSource {
			static IntPtr type_write;
			public WriteMonitor (IntPtr handle, bool owns) : base (handle, owns){}
			public WriteMonitor (IntPtr handle) : base (handle, false){}
			
			public WriteMonitor (int fileDescriptor, DispatchQueue queue = null)
			{
				if (type_write == IntPtr.Zero)
					type_write = Dlfcn.dlsym (Libraries.System.Handle, "_dispatch_source_type_write");
				this.queue = queue;
				var handle = dispatch_source_create (type_write,
								 handle: (IntPtr) fileDescriptor,
								 mask: IntPtr.Zero,
								 queue: queue == null ? IntPtr.Zero : queue.Handle);
				if (handle != IntPtr.Zero)
					InitializeHandle (handle);
			}
			public int FileDescriptor {
				get {
					return (int) dispatch_source_get_handle (GetCheckedHandle ());
				}
			}
			
			public int BufferSpaceAvailable {
				get {
					return (int) dispatch_source_get_data (GetCheckedHandle ());
				}
			}
		}
	}
}
