//
// NWAdvertiseDescriptor.cs: Bindings the Netowrk nw_advertise_descriptor_t API
//
// Authors:
//   Miguel de Icaza (miguel@microsoft.com)
//
// Copyrigh 2018 Microsoft Inc
//
using System;
using System.Runtime.InteropServices;
using ObjCRuntime;
using Foundation;
using CoreFoundation;

using nw_advertise_descriptor_t=System.IntPtr;
using OS_nw_advertise_descriptor=System.IntPtr;

namespace Network {
	
	[TV (12,0), Mac (10,14), iOS (12,0)]
	public class NWAdvertiseDescriptor : NativeObject {
		public NWAdvertiseDescriptor (IntPtr handle, bool owns) : base (handle, owns)
		{ }

		[DllImport (Constants.NetworkLibrary)]
		static extern IntPtr nw_advertise_descriptor_create_bonjour_service (string name, string type, string domain);

		public static NWAdvertiseDescriptor CreateBonjourService (string name, string type = null, string domain = null)
		{
			if (name == null)
				throw new ArgumentNullException (nameof (name));

			var x = nw_advertise_descriptor_create_bonjour_service (name, type, domain);
			if (x == IntPtr.Zero)
				return null;
			return new NWAdvertiseDescriptor (x, owns: true);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_advertise_descriptor_set_txt_record (IntPtr handle, string txtRecord, IntPtr txtLen);

		public void SetTxtRecord (string txt)
		{
			if (txt == null)
				throw new ArgumentNullException (nameof (txt));
			var n = System.Text.Encoding.UTF8.GetByteCount (txt);
			nw_advertise_descriptor_set_txt_record (GetHandle (), txt, (IntPtr) n);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_advertise_descriptor_set_no_auto_rename (IntPtr handle, [MarshalAs(UnmanagedType.I1)]  bool no_auto_rename);

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool nw_advertise_descriptor_get_no_auto_rename (IntPtr handle);
		
		public bool NoAutoRename {
			set => nw_advertise_descriptor_set_no_auto_rename (GetHandle (), value);
			get => nw_advertise_descriptor_get_no_auto_rename (GetHandle ());
		}
	}
}
