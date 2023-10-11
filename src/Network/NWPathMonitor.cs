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

using OS_nw_path_monitor = System.IntPtr;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Network {

#if NET
	[SupportedOSPlatform ("tvos12.0")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios12.0")]
	[SupportedOSPlatform ("maccatalyst")]
#else
	[TV (12, 0)]
	[iOS (12, 0)]
	[Watch (6, 0)]
#endif
	public class NWPathMonitor : NativeObject {
		[Preserve (Conditional = true)]
#if NET
		internal NWPathMonitor (NativeHandle handle, bool owns) : base (handle, owns)
#else
		public NWPathMonitor (NativeHandle handle, bool owns) : base (handle, owns)
#endif
		{
			_SetUpdatedSnapshotHandler (SetUpdatedSnapshotHandlerWrapper);
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_path_monitor_create ();

		NWPath? currentPath;
		public NWPath? CurrentPath => currentPath;

		public NWPathMonitor ()
			: this (nw_path_monitor_create (), true)
		{
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_path_monitor_create_with_type (NWInterfaceType interfaceType);

		public NWPathMonitor (NWInterfaceType interfaceType)
			: this (nw_path_monitor_create_with_type (interfaceType), true)
		{
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
			if (queue is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (queue));
			nw_path_monitor_set_queue (GetCheckedHandle (), queue.Handle);
		}

#if !NET
		delegate void nw_path_monitor_update_handler_t (IntPtr block, IntPtr path);
		static nw_path_monitor_update_handler_t static_UpdateSnapshot = TrampolineUpdatedSnapshot;

		[MonoPInvokeCallback (typeof (nw_path_monitor_update_handler_t))]
#else
		[UnmanagedCallersOnly]
#endif
		static void TrampolineUpdatedSnapshot (IntPtr block, IntPtr path)
		{
			var del = BlockLiteral.GetTarget<Action<NWPath>> (block);
			if (del is not null) {
				var nwPath = new NWPath (path, owns: false);
				del (nwPath);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern unsafe void nw_path_monitor_set_update_handler (IntPtr handle, BlockLiteral* callback);

		[BindingImpl (BindingImplOptions.Optimizable)]
		void _SetUpdatedSnapshotHandler (Action<NWPath> callback)
		{
			unsafe {
				if (callback is null) {
					nw_path_monitor_set_update_handler (GetCheckedHandle (), null);
					return;
				}

#if NET
				delegate* unmanaged<IntPtr, IntPtr, void> trampoline = &TrampolineUpdatedSnapshot;
				using var block = new BlockLiteral (trampoline, callback, typeof (NWPathMonitor), nameof (TrampolineUpdatedSnapshot));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_UpdateSnapshot, callback);
#endif
				nw_path_monitor_set_update_handler (GetCheckedHandle (), &block);
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
			if (userSnapshotHandler is not null) {
				userSnapshotHandler (currentPath);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern unsafe void nw_path_monitor_set_cancel_handler (IntPtr handle, BlockLiteral* callback);

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void SetMonitorCanceledHandler (Action callback)
		{
			unsafe {
				if (callback is null) {
					nw_path_monitor_set_cancel_handler (GetCheckedHandle (), null);
					return;
				}

				using var block = BlockStaticDispatchClass.CreateBlock (callback);
				nw_path_monitor_set_cancel_handler (GetCheckedHandle (), &block);
			}
		}


#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos12.0")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[Mac (12, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_path_monitor_prohibit_interface_type (OS_nw_path_monitor monitor, NWInterfaceType interfaceType);

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos12.0")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[Mac (12, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
		public void ProhibitInterfaceType (NWInterfaceType interfaceType)
			=> nw_path_monitor_prohibit_interface_type (GetCheckedHandle (), interfaceType);

#if MONOMAC

#if NET
		[SupportedOSPlatform ("macos13.0")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
		[Mac (13,0)]
#endif
		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_path_monitor nw_path_monitor_create_for_ethernet_channel ();

#if NET
		[SupportedOSPlatform ("macos13.0")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
		[NoMacCatalyst]
		[Mac (13,0)]
#endif
		public static NWPathMonitor CreateForEthernetChannel ()
			=> new NWPathMonitor (nw_path_monitor_create_for_ethernet_channel (), true);
#endif
	}

}
