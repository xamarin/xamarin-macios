//
// NWPath.cs: Bindings the Netowrk nw_path_monitor_t API.
//
// Authors:
//   Miguel de Icaza (miguel@microsoft.com)
//
// Copyrigh 2018 Microsoft Inc
//
using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using ObjCRuntime;
using Foundation;
using CoreFoundation;

namespace Network {

	[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
	public class NWPathMonitor : NativeObject {
		public NWPathMonitor (IntPtr handle, bool owns) : base (handle, owns) {}

		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_path_monitor_create ();

		public NWPathMonitor ()
		{
			InitializeHandle (nw_path_monitor_create ());
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_path_monitor_create_with_type (NWInterfaceType interfaceType);

		public NWPathMonitor (NWInterfaceType interfaceType)
		{
			InitializeHandle (nw_path_monitor_create_with_type (interfaceType));
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_path_monitor_cancel (IntPtr handle);

		public void Cancel () => nw_path_monitor_cancel (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_path_monitor_start (IntPtr handle);

		public void Start () => nw_path_monitor_start (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_path_monitor_set_queue (IntPtr handle, IntPtr queue);

		public void SetQueue (DispatchQueue queue)
		{
			if (queue == null)
				throw new ArgumentNullException (nameof (queue));
			nw_path_monitor_set_queue (GetCheckedHandle (), queue.Handle);
		}

		delegate void nw_path_monitor_update_handler_t (IntPtr block, IntPtr path);
		static nw_path_monitor_update_handler_t static_UpdateSnapshot = TrampolineUpdatedSnapshot;

		[MonoPInvokeCallback (typeof (nw_path_monitor_update_handler_t))]
		static void TrampolineUpdatedSnapshot (IntPtr block, IntPtr path)
		{
			var del = BlockLiteral.GetTarget<Action<NWPath>> (block);
			if (del != null) {
				var nwPath = new NWPath (path, owns: false);
				del (nwPath);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern unsafe void nw_path_monitor_set_update_handler (IntPtr handle, void *callback);

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void SetUpdatedSnapshotHandler (Action<NWPath> callback)
		{
			unsafe {
				if (callback == null) {
					nw_path_monitor_set_update_handler (GetCheckedHandle (), null);
					return;
				}

				BlockLiteral block_handler = new BlockLiteral ();
				BlockLiteral *block_ptr_handler = &block_handler;
				block_handler.SetupBlockUnsafe (static_UpdateSnapshot, callback);

				try {
					nw_path_monitor_set_update_handler (GetCheckedHandle (), (void*) block_ptr_handler);
				} finally {
					block_handler.CleanupBlock ();
				}
			}
		}

		delegate void nw_path_monitor_cancel_handler_t (IntPtr block);
		static nw_path_monitor_cancel_handler_t static_MonitorCancelled = TrampolineMonitorCancelled;

		[MonoPInvokeCallback (typeof (nw_path_monitor_cancel_handler_t))]
		static void TrampolineMonitorCancelled (IntPtr block)
		{
			var del = BlockLiteral.GetTarget<Action> (block);
			if (del != null) {
				del ();
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern unsafe void nw_path_monitor_set_cancel_handler (IntPtr handle, void *callback);

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void SetMonitorCancelledHandler (Action callback)
		{
			unsafe {
				if (callback == null) {
					nw_path_monitor_set_update_handler (GetCheckedHandle (), null);
					return;
				}

				BlockLiteral block_handler = new BlockLiteral ();
				BlockLiteral *block_ptr_handler = &block_handler;
				block_handler.SetupBlockUnsafe (static_MonitorCancelled, callback);

				try {
					nw_path_monitor_set_cancel_handler (GetCheckedHandle (), (void*) block_ptr_handler);
				} finally {
					block_handler.CleanupBlock ();
				}
			}
		}
	}
}
