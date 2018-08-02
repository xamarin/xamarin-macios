//
// NWInterface.cs: Bindings the Netowrk nw_interface API.
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

using OS_nw_interface=System.IntPtr;

namespace Network {

	[TV (12,0), Mac (10,14), iOS (12,0)]
	public class NWInterface : NativeObject {
		public NWInterface (IntPtr handle, bool owns) : base (handle, owns) {}

		[DllImport (Constants.NetworkLibrary)]
		static extern NWInterfaceType nw_interface_get_type (OS_nw_interface iface);

		public NWInterfaceType InterfaceType => nw_interface_get_type (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		static extern IntPtr nw_interface_get_name (OS_nw_interface iface);

		public string Name => Marshal.PtrToStringAnsi (nw_interface_get_name (GetCheckedHandle ()));

		[DllImport (Constants.NetworkLibrary)]
		static extern /* uint32_t */ uint nw_interface_get_index (OS_nw_interface iface);

		public uint Index => nw_interface_get_index (GetCheckedHandle ());
	}

	public enum NWInterfaceType {
		Other = 0,
		Wifi = 1,
		Cellular = 2,
		Wired = 3,
		Loopback = 4,
	}
}
