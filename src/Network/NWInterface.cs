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
	
	public class NWInterface : INativeObject, IDisposable {
		internal IntPtr handle;
		public IntPtr Handle {
			get { return handle; }
		}

		public NWInterface (IntPtr handle, bool owns)
		{
			this.handle = handle;
			if (owns == false)
				CFObject.CFRetain (handle);
		}

		public NWInterface (IntPtr handle) : this (handle, false)
		{
		}

		~NWInterface ()
		{
			Dispose (false);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		public virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero) {
				CFObject.CFRelease (handle);
				handle = IntPtr.Zero;
			}
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern NWInterfaceType nw_interface_get_type (OS_nw_interface iface);

		public NWInterfaceType InterfaceType => nw_interface_get_type (handle);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern IntPtr nw_interface_get_name (OS_nw_interface iface);

		public string Name => Marshal.PtrToStringAnsi (nw_interface_get_name (handle));

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern uint nw_interface_get_index (OS_nw_interface iface);

		public uint Index => nw_interface_get_index (handle);
	}

	public enum NWInterfaceType {
		Other = 0,
		Wifi = 1,
		Celullar = 2,
		Wired = 3,
		Loopback = 4
	}
}
