
//
// NWDataTransferReport.cs: Bindings the Network nw_data_transfer_report_t API.
//
// Authors:
//   Manuel de la Pena (mandel@microsoft.com)
//
// Copyright 2019 Microsoft Inc
//

#nullable enable

using System.Runtime.CompilerServices;
using CoreFoundation;

using OS_nw_establishment_report = System.IntPtr;
using nw_endpoint_t = System.IntPtr;
using nw_report_protocol_enumerator_t = System.IntPtr;
using nw_protocol_definition_t = System.IntPtr;
using nw_resolution_report_t = System.IntPtr;

namespace Network {
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	public class NWEstablishmentReport : NativeObject {

		[Preserve (Conditional = true)]
		internal NWEstablishmentReport (NativeHandle handle, bool owns) : base (handle, owns) { }

		[DllImport (Constants.NetworkLibrary)]
		static extern byte nw_establishment_report_get_used_proxy (OS_nw_establishment_report report);

		public bool UsedProxy => nw_establishment_report_get_used_proxy (GetCheckedHandle ()) != 0;

		[DllImport (Constants.NetworkLibrary)]
		static extern byte nw_establishment_report_get_proxy_configured (OS_nw_establishment_report report);

		public bool ProxyConfigured => nw_establishment_report_get_proxy_configured (GetCheckedHandle ()) != 0;

		[DllImport (Constants.NetworkLibrary)]
		static extern uint nw_establishment_report_get_previous_attempt_count (OS_nw_establishment_report report);

		public uint PreviousAttemptCount => nw_establishment_report_get_previous_attempt_count (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		static extern ulong nw_establishment_report_get_duration_milliseconds (OS_nw_establishment_report report);

		public TimeSpan Duration => TimeSpan.FromMilliseconds (nw_establishment_report_get_duration_milliseconds (GetCheckedHandle ()));

		[DllImport (Constants.NetworkLibrary)]
		static extern ulong nw_establishment_report_get_attempt_started_after_milliseconds (OS_nw_establishment_report report);

		public TimeSpan ConnectionSetupTime => TimeSpan.FromMilliseconds (nw_establishment_report_get_attempt_started_after_milliseconds (GetCheckedHandle ()));

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_establishment_report_enumerate_resolutions (OS_nw_establishment_report report, BlockLiteral* enumerate_block);

		[UnmanagedCallersOnly]
		static byte TrampolineResolutionEnumeratorHandler (IntPtr block, NWReportResolutionSource source, nuint milliseconds, int endpoint_count, nw_endpoint_t successful_endpoint, nw_endpoint_t preferred_endpoint)
		{
			var del = BlockLiteral.GetTarget<Action<NWReportResolutionSource, TimeSpan, int, NWEndpoint, NWEndpoint>> (block);
			if (del is not null) {
				using (var nwSuccesfulEndpoint = new NWEndpoint (successful_endpoint, owns: false))
				using (var nwPreferredEndpoint = new NWEndpoint (preferred_endpoint, owns: false))
					del (source, TimeSpan.FromMilliseconds (milliseconds), endpoint_count, nwSuccesfulEndpoint, nwPreferredEndpoint);
				return 1;
			}
			return 0;
		}

		/// <summary>Enumerates the endpoint resolution steps performed while establishing the connection.</summary>
		/// <param name="handler">The callback invoked for each resolution step. Its arguments are the resolution source, the time spent resolving endpoints, the number of resolved endpoints, the endpoint used to establish the connection, and the first endpoint attempted, respectively.</param>
		/// <exception cref="ArgumentNullException"><paramref name="handler" /> is <see langword="null" />.</exception>
		/// <remarks>
		///   The callback is invoked synchronously zero or more times before this method returns.
		///   The endpoint objects passed to the callback are disposed when the callback returns.
		/// </remarks>
		[BindingImpl (BindingImplOptions.Optimizable)]
		public void EnumerateResolutions (Action<NWReportResolutionSource, TimeSpan, int, NWEndpoint, NWEndpoint> handler)
		{
			if (handler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));

			unsafe {
				delegate* unmanaged<IntPtr, NWReportResolutionSource, nuint, int, nw_endpoint_t, nw_endpoint_t, byte> trampoline = &TrampolineResolutionEnumeratorHandler;
				using var block = new BlockLiteral (trampoline, handler, typeof (NWEstablishmentReport), nameof (TrampolineResolutionEnumeratorHandler));
				nw_establishment_report_enumerate_resolutions (GetCheckedHandle (), &block);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_establishment_report_enumerate_protocols (OS_nw_establishment_report report, BlockLiteral* enumerate_block);

		[UnmanagedCallersOnly]
		static byte TrampolineEnumerateProtocolsHandler (IntPtr block, nw_protocol_definition_t protocol, nuint handshake_milliseconds, nuint handshake_rtt_milliseconds)
		{
			var del = BlockLiteral.GetTarget<Action<NWProtocolDefinition, TimeSpan, TimeSpan>> (block);
			if (del is not null) {
				using (var nwProtocolDefinition = new NWProtocolDefinition (protocol, owns: false))
					del (nwProtocolDefinition, TimeSpan.FromMilliseconds (handshake_milliseconds), TimeSpan.FromMilliseconds (handshake_rtt_milliseconds));
				return 1;
			}
			return 0;
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void EnumerateProtocols (Action<NWProtocolDefinition, TimeSpan, TimeSpan> handler)
		{
			if (handler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));

			unsafe {
				delegate* unmanaged<IntPtr, nw_protocol_definition_t, nuint, nuint, byte> trampoline = &TrampolineEnumerateProtocolsHandler;
				using var block = new BlockLiteral (trampoline, handler, typeof (NWEstablishmentReport), nameof (TrampolineEnumerateProtocolsHandler));
				nw_establishment_report_enumerate_protocols (GetCheckedHandle (), &block);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern nw_endpoint_t nw_establishment_report_copy_proxy_endpoint (OS_nw_establishment_report report);

		public NWEndpoint? ProxyEndpoint {
			get {
				var ptr = nw_establishment_report_copy_proxy_endpoint (GetCheckedHandle ());
				return (ptr == IntPtr.Zero) ? null : new NWEndpoint (ptr, owns: true);
			}
		}

		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_establishment_report_enumerate_resolution_reports (OS_nw_establishment_report report, BlockLiteral* enumerateBlock);

		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnmanagedCallersOnly]
		static byte TrampolineEnumerateResolutionReport (IntPtr block, nw_resolution_report_t report)
		{
			var del = BlockLiteral.GetTarget<Action<NWResolutionReport>> (block);
			if (del is null)
				return 0;
			using var nwReport = new NWResolutionReport (report, owns: false);
			del (nwReport);
			return 1;
		}

		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[BindingImpl (BindingImplOptions.Optimizable)]
		public void EnumerateResolutionReports (Action<NWResolutionReport> handler)
		{
			if (handler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));

			unsafe {
				delegate* unmanaged<IntPtr, nw_resolution_report_t, byte> trampoline = &TrampolineEnumerateResolutionReport;
				using var block = new BlockLiteral (trampoline, handler, typeof (NWEstablishmentReport), nameof (TrampolineEnumerateResolutionReport));
				nw_establishment_report_enumerate_resolution_reports (GetCheckedHandle (), &block);
			}
		}
	}
}
