
//
// NWDataTransferReport.cs: Bindings the Network nw_data_transfer_report_t API.
//
// Authors:
//   Manuel de la Pena (mandel@microsoft.com)
//
// Copyrigh 2019 Microsoft Inc
//
using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using ObjCRuntime;
using Foundation;
using CoreFoundation;

using OS_nw_establishment_report=System.IntPtr;
using nw_endpoint_t=System.IntPtr;
using nw_report_protocol_enumerator_t=System.IntPtr;
using nw_protocol_definition_t=System.IntPtr;

namespace Network {

	[TV (13,0), Mac (10,15), iOS (13,0), Watch (6,0)]
	public enum NWReportResolutionSource {
		Query = 1,
		Cache = 2,
		ExpiredCache = 3,
	}

	[TV (13,0), Mac (10,15), iOS (13,0), Watch (6,0)]
	public class NWEstablishmentReport : NativeObject {

		public NWEstablishmentReport (IntPtr handle, bool owns) : base (handle, owns) {}

		[DllImport (Constants.NetworkLibrary)]
		static extern bool nw_establishment_report_get_used_proxy (OS_nw_establishment_report report);

		public bool UsedProxy => nw_establishment_report_get_used_proxy (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		static extern bool nw_establishment_report_get_proxy_configured (OS_nw_establishment_report report);

		public bool ProxyConfigured => nw_establishment_report_get_proxy_configured (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		static extern uint nw_establishment_report_get_previous_attempt_count (OS_nw_establishment_report report);

		public uint PreviousAttemptCount => nw_establishment_report_get_previous_attempt_count (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		static extern ulong nw_establishment_report_get_duration_milliseconds (OS_nw_establishment_report report);

		public ulong Duration => nw_establishment_report_get_duration_milliseconds (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		static extern ulong nw_establishment_report_get_attempt_started_after_milliseconds (OS_nw_establishment_report report);

		public ulong ConnectionSetupTime => nw_establishment_report_get_attempt_started_after_milliseconds (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_establishment_report_enumerate_resolutions (OS_nw_establishment_report report, void *enumerate_block);

		delegate void nw_report_resolution_enumerator_t (IntPtr block, NWReportResolutionSource source, nuint milliseconds, int endpoint_count, nw_endpoint_t successful_endpoint, nw_endpoint_t preferred_endpoint);
		static nw_report_resolution_enumerator_t static_ResolutionEnumeratorHandler = TrampolineResolutionEnumeratorHandler;

		[MonoPInvokeCallback (typeof (nw_report_resolution_enumerator_t))]
		static void TrampolineResolutionEnumeratorHandler (IntPtr block, NWReportResolutionSource source, nuint milliseconds, int endpoint_count, nw_endpoint_t successful_endpoint, nw_endpoint_t preferred_endpoint)
		{
			var del = BlockLiteral.GetTarget<Action<NWReportResolutionSource, nuint, int, NWEndpoint, NWEndpoint>> (block);
			if (del != null) {
				var nwSuccesfulEndpoint = new NWEndpoint (successful_endpoint, owns: false);
				var nwPreferredEndpoint = new NWEndpoint (preferred_endpoint, owns: false);
				del (source, milliseconds, endpoint_count, nwSuccesfulEndpoint, nwPreferredEndpoint);
			}
		}

		public void ResolutionEnumeration (Action<NWReportResolutionSource, nuint, int, NWEndpoint, NWEndpoint> handler)
		{
			unsafe {
				if (handler == null) {
					nw_establishment_report_enumerate_resolutions (GetCheckedHandle (), null);
					return;
				}
				BlockLiteral block_handler = new BlockLiteral ();
				BlockLiteral *block_ptr_handler = &block_handler;
				block_handler.SetupBlockUnsafe (static_ResolutionEnumeratorHandler, handler);
				try {
					nw_establishment_report_enumerate_resolutions (GetCheckedHandle (), (void*) block_ptr_handler);
				} finally {
					block_handler.CleanupBlock ();
				}
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_establishment_report_enumerate_protocols (OS_nw_establishment_report report, void *enumerate_block);

		delegate void nw_establishment_report_enumerate_protocols_t (IntPtr block, nw_protocol_definition_t protocol, nuint handshake_milliseconds, nuint handshake_rtt_milliseconds);
		static nw_establishment_report_enumerate_protocols_t static_EnumerateProtocolsHandler = TrampolineEnumerateProtocolsHandler;

		[MonoPInvokeCallback (typeof (nw_establishment_report_enumerate_protocols_t))]
		static void TrampolineEnumerateProtocolsHandler (IntPtr block, nw_protocol_definition_t protocol, nuint handshake_milliseconds, nuint handshake_rtt_milliseconds)
		{
			var del = BlockLiteral.GetTarget<Action<NWProtocolDefinition, nuint, nuint>> (block);
			if (del != null) {
				var nwProtocolDefinition = new NWProtocolDefinition (protocol, owns: false);
				del (nwProtocolDefinition, handshake_milliseconds, handshake_rtt_milliseconds); 
			}
		}

		public void EnumerateProtocols (Action<NWProtocolDefinition, nuint, nuint> handler)
		{
			unsafe {
				if (handler == null) {
					nw_establishment_report_enumerate_protocols (GetCheckedHandle (), null);
					return;
				}
				BlockLiteral block_handler = new BlockLiteral ();
				BlockLiteral *block_ptr_handler = &block_handler;
				block_handler.SetupBlockUnsafe (static_EnumerateProtocolsHandler, handler);
				try {
					nw_establishment_report_enumerate_protocols (GetCheckedHandle (), (void*) block_ptr_handler);
				} finally {
					block_handler.CleanupBlock ();
				}
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern nw_endpoint_t nw_establishment_report_copy_proxy_endpoint (OS_nw_establishment_report report);

		public NWEndpoint ProxyEndpoint => new NWEndpoint (nw_establishment_report_copy_proxy_endpoint (GetCheckedHandle ()), owns:true);
	}
}
