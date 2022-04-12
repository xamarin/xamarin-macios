//
// NWPath.cs: Bindings the Netowrk nw_path_monitor_t API.
//
// Authors:
//   Miguel de Icaza (miguel@microsoft.com)
//
// Copyrigh 2018 Microsoft Inc
//

#nullable enable

using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using ObjCRuntime;
using Foundation;
using CoreFoundation;

using OS_nw_path_monitor=System.IntPtr;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Network {

#if NET
	[SupportedOSPlatform ("tvos12.0")]
	[SupportedOSPlatform ("macos10.14")]
	[SupportedOSPlatform ("ios12.0")]
	[SupportedOSPlatform ("maccatalyst")]
#else
	[TV (12,0)]
	[Mac (10,14)]
	[iOS (12,0)]
	[Watch (6,0)]
#endif
	public class NWPathMonitor : NativeObject {
		[Preserve (Conditional = true)]
#if NET
		internal NWPathMonitor (NativeHandle handle, bool owns) : base (handle, owns) {}
#else
		public NWPathMonitor (NativeHandle handle, bool owns) : base (handle, owns) {}
#endif

		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_path_monitor_create ();

		NWPath? currentPath;
		public NWPath? CurrentPath => currentPath;
		public NWPathMonitor ()
		{
			InitializeHandle (nw_path_monitor_create ());
			_SetUpdatedSnapshotHandler (SetUpdatedSnapshotHandlerWrapper);
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_path_monitor_create_with_type (NWInterfaceType interfaceType);

		public NWPathMonitor (NWInterfaceType interfaceType)
		{
			InitializeHandle (nw_path_monitor_create_with_type (interfaceType));
			_SetUpdatedSnapshotHandler (SetUpdatedSnapshotHandlerWrapper);
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
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (queue));
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
		void _SetUpdatedSnapshotHandler (Action<NWPath> callback)
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

		Action<NWPath>? userSnapshotHandler;
		public Action<NWPath>? SnapshotHandler {
			get => userSnapshotHandler;
			set => userSnapshotHandler = value;
		}

#if !NET
		[Obsolete ("Use the 'SnapshotHandler' property instead.")]
		public void SetUpdatedSnapshotHandler (Action<NWPath> callback)
		{
			userSnapshotHandler = callback;
		}
#endif

		void SetUpdatedSnapshotHandlerWrapper (NWPath path)
		{
			currentPath = path;
			if (userSnapshotHandler != null) {
				userSnapshotHandler (currentPath);
			}
		}

		delegate void nw_path_monitor_cancel_handler_t (IntPtr block);
		static nw_path_monitor_cancel_handler_t static_MonitorCanceled = TrampolineMonitorCanceled;

		[MonoPInvokeCallback (typeof (nw_path_monitor_cancel_handler_t))]
		static void TrampolineMonitorCanceled (IntPtr block)
		{
			var del = BlockLiteral.GetTarget<Action> (block);
			if (del != null) {
				del ();
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern unsafe void nw_path_monitor_set_cancel_handler (IntPtr handle, void *callback);

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void SetMonitorCanceledHandler (Action callback)
		{
			unsafe {
				if (callback == null) {
					nw_path_monitor_set_cancel_handler (GetCheckedHandle (), null);
					return;
				}

				BlockLiteral block_handler = new BlockLiteral ();
				BlockLiteral *block_ptr_handler = &block_handler;
				block_handler.SetupBlockUnsafe (static_MonitorCanceled, callback);

				try {
					nw_path_monitor_set_cancel_handler (GetCheckedHandle (), (void*) block_ptr_handler);
				} finally {
					block_handler.CleanupBlock ();
				}
			}
		}
		
		
#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos12.0")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[Watch (8,0)]
		[TV (15,0)]
		[Mac (12,0)]
		[iOS (15,0)]
		[MacCatalyst (15,0)]
#endif
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_path_monitor_prohibit_interface_type (OS_nw_path_monitor monitor, NWInterfaceType interfaceType);

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos12.0")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[Watch (8,0)]
		[TV (15,0)]
		[Mac (12,0)]
		[iOS (15,0)]
		[MacCatalyst (15,0)]
#endif
		public void ProhibitInterfaceType (NWInterfaceType interfaceType)
			=> nw_path_monitor_prohibit_interface_type (GetCheckedHandle (), interfaceType);
	}
}
