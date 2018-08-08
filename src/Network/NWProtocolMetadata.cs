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
using Security;
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

	[TV (12,0), Mac (10,14), iOS (12,0)]
	public class NWProtocolMetadata : NativeObject {

#if false
		// Officially listed on header files, but seems to not work on Mac/iOS
		// https://bugreport.apple.com/web/?problemID=42443077
		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_protocol_metadata nw_ip_create_metadata ();

		public static NWProtocolMetadata CreateIPMetadata ()
		{
			return new NWProtocolMetadata (nw_ip_create_metadata (), owns: true);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_protocol_metadata nw_udp_create_metadata ();
		public static NWProtocolMetadata CreateUDPMetadata ()
		{
			return new NWProtocolMetadata (nw_udp_create_metadata (), owns: true);
		}
#endif

		public NWProtocolMetadata (IntPtr handle, bool owns) : base (handle, owns) {}

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_protocol_definition nw_protocol_metadata_copy_definition (OS_nw_protocol_metadata metadata);

		public NWProtocolDefinition ProtocolDefinition => new NWProtocolDefinition (nw_protocol_metadata_copy_definition (GetCheckedHandle ()), owns: true);

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool nw_protocol_metadata_is_ip (OS_nw_protocol_metadata metadata);

		public bool IsIP => nw_protocol_metadata_is_ip (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool nw_protocol_metadata_is_udp (OS_nw_protocol_metadata metadata);

		public bool IsUdp => nw_protocol_metadata_is_udp (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool nw_protocol_metadata_is_tls (OS_nw_protocol_metadata metadata);

		public bool IsTls => nw_protocol_metadata_is_tls (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		static extern IntPtr nw_tls_copy_sec_protocol_metadata (IntPtr handle);

		public SecProtocolMetadata SecProtocolMetadata => new SecProtocolMetadata (nw_tls_copy_sec_protocol_metadata (GetCheckedHandle ()), owns: true);

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_ip_metadata_set_ecn_flag (OS_nw_protocol_metadata metadata, NWIPEcnFlag ecn_flag);

		[DllImport (Constants.NetworkLibrary)]
		static extern NWIPEcnFlag nw_ip_metadata_get_ecn_flag (OS_nw_protocol_metadata metadata);

		public NWIPEcnFlag IPMetadataEcnFlag {
			get => nw_ip_metadata_get_ecn_flag (GetCheckedHandle ());
			set => nw_ip_metadata_set_ecn_flag (GetCheckedHandle (), value);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_ip_metadata_set_service_class (OS_nw_protocol_metadata metadata, NWServiceClass service_class);

		[DllImport (Constants.NetworkLibrary)]
		static extern NWServiceClass nw_ip_metadata_get_service_class (OS_nw_protocol_metadata metadata);

		public NWServiceClass ServiceClass {
			get => nw_ip_metadata_get_service_class (GetCheckedHandle ());
			set => nw_ip_metadata_set_service_class (GetCheckedHandle (), value);
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static /* uint32_t */ uint nw_tcp_get_available_receive_buffer (IntPtr handle);

		public uint TcpGetAvailableReceiveBuffer () => nw_tcp_get_available_receive_buffer (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		extern static /* uint32_t */ uint nw_tcp_get_available_send_buffer (IntPtr handle);

		public uint TcpGetAvailableSendBuffer () => nw_tcp_get_available_send_buffer (GetCheckedHandle ());
	}
}
