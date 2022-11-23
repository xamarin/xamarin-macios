using System;
using System.Runtime.InteropServices;
using ObjCRuntime;
using Foundation;
using CoreFoundation;

using nw_report_resolution_source_t = System.IntPtr;
using OS_nw_resolution_report = System.IntPtr;
using OS_nw_endpoint = System.IntPtr;
using nw_report_resolution_protocol_t = System.IntPtr;

#if !NET
using NativeHandle = System.IntPtr;
#endif

#nullable enable

namespace Network {

#if NET
	[SupportedOSPlatform ("tvos15.0")]
	[SupportedOSPlatform ("macos12.0")]
	[SupportedOSPlatform ("ios15.0")]
	[SupportedOSPlatform ("maccatalyst15.0")]
#else
	[Watch (8, 0)]
	[TV (15, 0)]
	[Mac (12, 0)]
	[iOS (15, 0)]
	[MacCatalyst (15, 0)]
#endif
	public class NWResolutionReport : NativeObject {

		[Preserve (Conditional = true)]
		internal NWResolutionReport (NativeHandle handle, bool owns) : base (handle, owns) { }

		[DllImport (Constants.NetworkLibrary)]
		static extern NWReportResolutionSource nw_resolution_report_get_source (OS_nw_resolution_report resolutionReport);

		public NWReportResolutionSource Source
			=> nw_resolution_report_get_source (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		static extern ulong nw_resolution_report_get_milliseconds (OS_nw_resolution_report resolutionReport);

		public ulong Milliseconds
			=> nw_resolution_report_get_milliseconds (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		static extern uint nw_resolution_report_get_endpoint_count (OS_nw_resolution_report resolutionReport);

		public uint EndpointCount
			=> nw_resolution_report_get_endpoint_count (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_endpoint nw_resolution_report_copy_successful_endpoint (OS_nw_resolution_report resolutionReport);

		public NWEndpoint? SuccessfulEndpoint {
			get {
				var ptr = nw_resolution_report_copy_successful_endpoint (GetCheckedHandle ());
				return ptr == IntPtr.Zero ? null : new NWEndpoint (ptr, true);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_endpoint nw_resolution_report_copy_preferred_endpoint (OS_nw_resolution_report resolutionReport);

		public NWEndpoint? PreferredEndpoint {
			get {
				var ptr = nw_resolution_report_copy_preferred_endpoint (GetCheckedHandle ());
				return ptr == IntPtr.Zero ? null : new NWEndpoint (ptr, true);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern NWReportResolutionProtocol nw_resolution_report_get_protocol (OS_nw_resolution_report resolutionReport);

		public NWReportResolutionProtocol Protocol
			=> nw_resolution_report_get_protocol (GetCheckedHandle ());
	}
}
