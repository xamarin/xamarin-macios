//
// NWProtocolMetadata.cs: Bindings the Netowrk nw_protocol_metadata_t API.
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

using OS_nw_protocol_definition=System.IntPtr;
using OS_nw_protocol_metadata=System.IntPtr;
using nw_service_class_t=System.IntPtr;

namespace Network {

	public enum NWServiceClass {
		BestEffort = 0,
		Background = 1,
		InteractiveVideo = 2,
		InteractiveVoice = 3,
		ResponsiveData = 4,
		Signaling = 5,
	}

	public enum NWIPEcnFlag {
		NonEct = 0,
		Ect = 2,
		Ect1 = 1,
		Ce = 3,
	}
	
	public class NWProtocolMetadata : INativeObject, IDisposable {
		IntPtr handle;
		public IntPtr Handle {
			get { return handle; }
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_protocol_metadata nw_ip_create_metadata ();

		public NWProtocolMetadata ()
		{
			handle = nw_ip_create_metadata ();
		}
	
		public NWProtocolMetadata (IntPtr handle, bool owns)
		{
			this.handle = handle;
			if (owns == false)
				CFObject.CFRetain (handle);
		}

		public NWProtocolMetadata (IntPtr handle) : this (handle, false)
		{
		}

		~NWProtocolMetadata ()
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
		static extern OS_nw_protocol_definition nw_protocol_metadata_copy_definition (OS_nw_protocol_metadata metadata);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public NWProtocolDefinition ProtocolDefinition => new NWProtocolDefinition (nw_protocol_metadata_copy_definition (handle), owns: true);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool nw_protocol_metadata_is_ip (OS_nw_protocol_metadata metadata);
	
		public bool IsIP => nw_protocol_metadata_is_ip (handle);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_ip_metadata_set_ecn_flag (OS_nw_protocol_metadata metadata, NWIPEcnFlag ecn_flag);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern NWIPEcnFlag nw_ip_metadata_get_ecn_flag (OS_nw_protocol_metadata metadata);
			
		[TV (12,0), Mac (10,14), iOS (12,0)]
		public NWIPEcnFlag IPMetadataEcnFlag {
			get => nw_ip_metadata_get_ecn_flag (handle);
			set => nw_ip_metadata_set_ecn_flag (handle, value);
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_ip_metadata_set_service_class (OS_nw_protocol_metadata metadata, NWServiceClass service_class);
	
		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern NWServiceClass nw_ip_metadata_get_service_class (OS_nw_protocol_metadata metadata);
		
		[TV (12,0), Mac (10,14), iOS (12,0)]
		public NWServiceClass ServiceClass {
			get => nw_ip_metadata_get_service_class (handle);
			set => nw_ip_metadata_set_service_class (handle, value);
		}
	}
}
