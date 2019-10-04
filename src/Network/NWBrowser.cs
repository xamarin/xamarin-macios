//
// NWBrowser.cs: Bindings the Network nw_browser_t API.
//
// Authors:
//   Manuel de la Pena (mandel@microsoft.com)
//
// Copyrigh 2019 Microsoft Inc
//
using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using ObjCRuntime;
using Foundation;
using CoreFoundation;

using OS_nw_browser=System.IntPtr;
using OS_nw_browse_descriptor=System.IntPtr;
using OS_nw_parameters=System.IntPtr;
using dispatch_queue_t =System.IntPtr;

namespace Network {

	[TV (13,0), Mac (10,15), iOS (13,0), Watch (6,0)]
	public enum NWBrowserState {
		Invalid = 0,
		Ready = 1,
		Failed = 2,
		Cancelled = 3,
	}

	[TV (13,0), Mac (10,15), iOS (13,0), Watch (6,0)]
	public class NWBrowser : NativeObject {

		bool started = false;
		bool queueSet = false;
		object startLock = new Object ();

		internal NWBrowser (IntPtr handle, bool owns) : base (handle, owns) {}

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_browser nw_browser_create (OS_nw_browse_descriptor descriptor, OS_nw_parameters parameters);

		public NWBrowser (NWBrowserDescriptor descriptor, NWParameters parameters)
		{
			if (descriptor == null)
				throw new ArgumentNullException (nameof (descriptor));

			InitializeHandle (nw_browser_create (descriptor.Handle, parameters?.GetHandle () ?? IntPtr.Zero));
		}

		public NWBrowser (NWBrowserDescriptor descriptor) : this (descriptor, null) {}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_browser_set_queue (OS_nw_browser browser, dispatch_queue_t queue);

		public void SetDispatchQueue (DispatchQueue queue){
			if (queue == null)
				throw new ArgumentNullException (nameof (queue));
			lock (startLock) {
				nw_browser_set_queue (GetCheckedHandle (), queue.Handle);
				queueSet = true;
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_browser_start (OS_nw_browser browser);

		public void Start ()
		{
			lock (startLock) {
				if (!queueSet) {
					throw new InvalidOperationException ("Cannot start the browser without a DispatchQueue.");
				}
				nw_browser_start (GetCheckedHandle ());
				started = true;
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_browser_cancel (OS_nw_browser browser);

		public void Cancel ()
		{
			lock (startLock) {
				try {
					nw_browser_cancel (GetCheckedHandle ());
				} finally {
					started = false;
				}
			}
		}

		public bool IsActive {
			get {
				lock (startLock) 
					return started;
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_browse_descriptor nw_browser_copy_browse_descriptor (OS_nw_browser browser);

		public NWBrowserDescriptor Descriptor
			=> new NWBrowserDescriptor (nw_browser_copy_browse_descriptor (GetCheckedHandle ()), owns: true);

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_parameters nw_browser_copy_parameters (OS_nw_browser browser);

		public NWParameters Parameters
			=> new NWParameters (nw_browser_copy_parameters (GetCheckedHandle ()), owns: true);

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_browser_set_browse_results_changed_handler (OS_nw_browser browser, void *handler);

		delegate void nw_browser_browse_results_changed_handler_t (IntPtr block, IntPtr oldResult, IntPtr newResult);
		static nw_browser_browse_results_changed_handler_t static_ChangesHandler = TrampolineChangesHandler;

		[MonoPInvokeCallback (typeof (nw_browser_browse_results_changed_handler_t))]
		static void TrampolineChangesHandler (IntPtr block, IntPtr oldResult, IntPtr newResult)
		{
			var del = BlockLiteral.GetTarget<Action<NWBrowseResult, NWBrowseResult>> (block);
			if (del != null) {
				using (var nwOldResult = new NWBrowseResult (oldResult, owns: false))
				using (var nwNewResult = new NWBrowseResult (newResult, owns: false))
					del (nwOldResult, nwNewResult);
			}
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void SetChangesHandler (Action<NWBrowseResult, NWBrowseResult> handler) {
			unsafe {
				if (handler == null) {
					nw_browser_set_browse_results_changed_handler (GetCheckedHandle (), null);
					return;
				}
				BlockLiteral block_handler = new BlockLiteral ();
				BlockLiteral *block_ptr_handler = &block_handler;
				block_handler.SetupBlockUnsafe (static_ChangesHandler, handler);
				try {
					nw_browser_set_browse_results_changed_handler (GetCheckedHandle (), (void*) block_ptr_handler);
				} finally {
					block_handler.CleanupBlock ();
				}
			}
		}	

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_browser_set_state_changed_handler (OS_nw_browser browser, void *state_changed_handler);

		delegate void nw_browser_set_state_changed_handler_t (IntPtr block, NWBrowserState state, IntPtr error);
		static nw_browser_set_state_changed_handler_t static_StateChangesHandler = TrampolineStateChangesHandler;

		[MonoPInvokeCallback (typeof (nw_browser_set_state_changed_handler_t))]
		static void TrampolineStateChangesHandler (IntPtr block, NWBrowserState state, IntPtr error)
		{
			var del = BlockLiteral.GetTarget<Action<NWBrowserState, NWError>> (block);
			if (del != null) {
				var nwError = (error == IntPtr.Zero)? null : new NWError (error, owns: false);
				del (state, nwError);
			}
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void SetStateChangesHandler (Action<NWBrowserState, NWError> handler)
		{
			unsafe {
				if (handler == null) {
					nw_browser_set_state_changed_handler (GetCheckedHandle (), null);
					return;
				}
				BlockLiteral block_handler = new BlockLiteral ();
				BlockLiteral *block_ptr_handler = &block_handler;
				block_handler.SetupBlockUnsafe (static_StateChangesHandler, handler);
				try {
					nw_browser_set_state_changed_handler (GetCheckedHandle (), (void*) block_ptr_handler);
				} finally {
					block_handler.CleanupBlock ();
				}
			}
		}	
	}
}
