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

	public class NWPathMonitor : NativeObject {
		public NWPathMonitor (IntPtr handle, bool owns) : base (handle, owns) {}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_path_monitor_create ();

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public NWPathMonitor ()
		{
			handle = nw_path_monitor_create ();
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_path_monitor_create_with_type (NWInterfaceType interfaceType);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public NWPathMonitor (NWInterfaceType interfaceType)
		{
			handle = nw_path_monitor_create_with_type (interfaceType);
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_path_monitor_cancel (IntPtr handle);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void Cancel () => nw_path_monitor_cancel (GetHandle());

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_path_monitor_start (IntPtr handle);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void Start () => nw_path_monitor_start (GetHandle());

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_path_monitor_set_queue (IntPtr handle, IntPtr queue);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void SetQueue (DispatchQueue queue)
		{
			if (queue == null)
				throw new ArgumentNullException (nameof (queue));
			nw_path_monitor_set_queue (GetHandle(), queue.handle);
		}
	}
}
