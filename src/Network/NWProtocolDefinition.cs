//
// NWEndpoint.cs: Bindings the Netowrk nw_endpoint_t API.
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

using OS_nw_protocol_definition=System.IntPtr;

namespace Network {

	public enum NWIPVersion {
		Any = 0,
		Version4 = 1,
		Version6 = 2
	}
		
	public class NWProtocolDefinition : INativeObject, IDisposable {
		IntPtr handle;
		public IntPtr Handle {
			get { return handle; }
		}

		public NWProtocolDefinition (IntPtr handle, bool owns)
		{
			this.handle = handle;
			if (owns == false)
				CFObject.CFRetain (handle);
		}

		public NWProtocolDefinition (IntPtr handle) : this (handle, false)
		{
		}

		~NWProtocolDefinition ()
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

		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern bool nw_protocol_definition_is_equal (OS_nw_protocol_definition definition1, OS_nw_protocol_definition definition2);

		public bool Equals (object other)
		{
			if (other == null)
				return false;
			if (!(other is NWProtocolDefinition))
				return false;
			return nw_protocol_definition_is_equal (handle, (other as NWProtocolDefinition).handle);
		}

		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_protocol_definition nw_protocol_copy_ip_definition ();
		
		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		public static NWProtocolDefinition IPDefinition => new NWProtocolDefinition (nw_protocol_copy_ip_definition (), owns: true);

		
	}
}
