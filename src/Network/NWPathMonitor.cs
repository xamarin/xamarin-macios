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

	[TV (12,0), Mac (10,14), iOS (12,0)]
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
	}
}
