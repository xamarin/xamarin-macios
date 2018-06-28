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
	
	public class NWAdvertiseDescriptor : INativeObject, IDisposable {
		IntPtr handle;
		public IntPtr Handle {
			get { return handle; }
		}

		public NWAdvertiseDescriptor (IntPtr handle, bool owns)
		{
			this.handle = handle;
			if (owns == false)
				CFObject.CFRetain (handle);
		}

		public NWAdvertiseDescriptor (IntPtr handle) : this (handle, false)
		{
		}

		~NWAdvertiseDescriptor ()
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

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_advertise_descriptor_set_txt_record (IntPtr handle, string txtRecord, IntPtr txtLen);

		public void SetTxtRecord (string txt)
		{
			if (txt == null)
				throw new ArgumentNullException (nameof (txt));
			var n = System.Text.Encoding.UTF8.GetByteCount (txt);
			nw_advertise_descriptor_set_txt_record (handle, txt, (IntPtr) n);
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_advertise_descriptor_set_no_auto_rename (IntPtr handle, [MarshalAs(UnmanagedType.I1)]  bool no_auto_rename);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool nw_advertise_descriptor_get_no_auto_rename (IntPtr handle);
		
		[TV (12,0), Mac (10,14), iOS (12,0)]
		public bool NoAutoRename {
			set => nw_advertise_descriptor_set_no_auto_rename (handle, value);
			get => nw_advertise_descriptor_get_no_auto_rename (handle);
		}
	}
}
