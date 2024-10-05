//
// NWDataTransferReport.cs: Bindings the Network nw_data_transfer_report_t API.
//
// Authors:
//   Manuel de la Pena (mandel@microsoft.com)
//
// Copyright 2019 Microsoft Inc
//

#nullable enable

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using ObjCRuntime;
using Foundation;
using CoreFoundation;

using OS_nw_data_transfer_report = System.IntPtr;
using OS_nw_connection = System.IntPtr;
using OS_nw_interface = System.IntPtr;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Network {

#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("maccatalyst")]
#else
	[TV (13, 0)]
	[iOS (13, 0)]
	[Watch (6, 0)]
#endif
	public class NWDataTransferReport : NativeObject {

		[Preserve (Conditional = true)]
		internal NWDataTransferReport (NativeHandle handle, bool owns) : base (handle, owns) { }

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_data_transfer_report nw_connection_create_new_data_transfer_report (OS_nw_connection connection);

		public NWDataTransferReport (NWConnection connection)
		{
			if (connection is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (connection));

			InitializeHandle (nw_connection_create_new_data_transfer_report (connection.Handle));
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern uint nw_data_transfer_report_get_path_count (OS_nw_data_transfer_report report);

		public uint PathCount => nw_data_transfer_report_get_path_count (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		static extern ulong nw_data_transfer_report_get_duration_milliseconds (OS_nw_data_transfer_report report);

		public TimeSpan Duration => TimeSpan.FromMilliseconds (nw_data_transfer_report_get_duration_milliseconds (GetCheckedHandle ()));

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_interface nw_data_transfer_report_copy_path_interface (OS_nw_data_transfer_report report, uint path_index);

		public NWInterface GetInterface (uint pathIndex)
			=> new NWInterface (nw_data_transfer_report_copy_path_interface (GetCheckedHandle (), pathIndex), owns: true);

		[DllImport (Constants.NetworkLibrary)]
		static extern ulong nw_data_transfer_report_get_received_application_byte_count (OS_nw_data_transfer_report report, uint path_index);

		public ulong GetApplicationReceivedByteCount (uint pathIndex)
			=> nw_data_transfer_report_get_received_application_byte_count (GetCheckedHandle (), pathIndex);

		[DllImport (Constants.NetworkLibrary)]
		static extern ulong nw_data_transfer_report_get_sent_application_byte_count (OS_nw_data_transfer_report report, uint path_index);

		public ulong GetApplicationSentByteCount (uint pathIndex)
			=> nw_data_transfer_report_get_sent_application_byte_count (GetCheckedHandle (), pathIndex);

		[DllImport (Constants.NetworkLibrary)]
		static extern ulong nw_data_transfer_report_get_received_transport_byte_count (OS_nw_data_transfer_report report, uint path_index);

		public ulong GetTransportReceivedByteCount (uint pathIndex)
			=> nw_data_transfer_report_get_received_transport_byte_count (GetCheckedHandle (), pathIndex);

		[DllImport (Constants.NetworkLibrary)]
		static extern ulong nw_data_transfer_report_get_received_transport_duplicate_byte_count (OS_nw_data_transfer_report report, uint path_index);

		public ulong GetTransportReceivedDuplicateByteCount (uint pathIndex)
			=> nw_data_transfer_report_get_received_transport_duplicate_byte_count (GetCheckedHandle (), pathIndex);

		[DllImport (Constants.NetworkLibrary)]
		static extern ulong nw_data_transfer_report_get_received_transport_out_of_order_byte_count (OS_nw_data_transfer_report report, uint path_index);

		public ulong GetTransportReceivedOutOfOrderByteCount (uint pathIndex)
			=> nw_data_transfer_report_get_received_transport_duplicate_byte_count (GetCheckedHandle (), pathIndex);

		[DllImport (Constants.NetworkLibrary)]
		static extern ulong nw_data_transfer_report_get_sent_transport_byte_count (OS_nw_data_transfer_report report, uint path_index);

		public ulong GetTransportSentByteCount (uint pathIndex)
			=> nw_data_transfer_report_get_sent_transport_byte_count (GetCheckedHandle (), pathIndex);


		[DllImport (Constants.NetworkLibrary)]
		static extern ulong nw_data_transfer_report_get_sent_transport_retransmitted_byte_count (OS_nw_data_transfer_report report, uint path_index);

		public ulong GetTransportRetransmittedByteCount (uint pathIndex)
			=> nw_data_transfer_report_get_sent_transport_retransmitted_byte_count (GetCheckedHandle (), pathIndex);

		[DllImport (Constants.NetworkLibrary)]
		static extern ulong nw_data_transfer_report_get_transport_smoothed_rtt_milliseconds (OS_nw_data_transfer_report report, uint path_index);

		public TimeSpan GetTransportSmoothedRoundTripTime (uint pathIndex)
			=> TimeSpan.FromMilliseconds (nw_data_transfer_report_get_transport_smoothed_rtt_milliseconds (GetCheckedHandle (), pathIndex));

		[DllImport (Constants.NetworkLibrary)]
		static extern ulong nw_data_transfer_report_get_transport_minimum_rtt_milliseconds (OS_nw_data_transfer_report report, uint path_index);

		public TimeSpan GetTransportMinimumRoundTripTime (uint pathIndex)
			=> TimeSpan.FromMilliseconds (nw_data_transfer_report_get_transport_minimum_rtt_milliseconds (GetCheckedHandle (), pathIndex));

		[DllImport (Constants.NetworkLibrary)]
		static extern ulong nw_data_transfer_report_get_transport_rtt_variance (OS_nw_data_transfer_report report, uint path_index);

		public ulong GetTransportRoundTripTimeVariance (uint pathIndex)
			=> nw_data_transfer_report_get_transport_rtt_variance (GetCheckedHandle (), pathIndex);

		[DllImport (Constants.NetworkLibrary)]
		static extern ulong nw_data_transfer_report_get_received_ip_packet_count (OS_nw_data_transfer_report report, uint path_index);

		public ulong GetTransportReceivedIPPackageCount (uint pathIndex)
			=> nw_data_transfer_report_get_received_ip_packet_count (GetCheckedHandle (), pathIndex);

		[DllImport (Constants.NetworkLibrary)]
		static extern ulong nw_data_transfer_report_get_sent_ip_packet_count (OS_nw_data_transfer_report report, uint path_index);

		public ulong GetTransportSentIPPackageCount (uint pathIndex)
			=> nw_data_transfer_report_get_sent_ip_packet_count (GetCheckedHandle (), pathIndex);

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_data_transfer_report_collect (OS_nw_data_transfer_report report, IntPtr queue, BlockLiteral* collect_block);

#if !NET
		delegate void nw_data_transfer_report_collect_t (IntPtr block, IntPtr report);
		static nw_data_transfer_report_collect_t static_CollectHandler = TrampolineCollectHandler;

		[MonoPInvokeCallback (typeof (nw_data_transfer_report_collect_t))]
#else
		[UnmanagedCallersOnly]
#endif
		static void TrampolineCollectHandler (IntPtr block, IntPtr report)
		{
			var del = BlockLiteral.GetTarget<Action<NWDataTransferReport>> (block);
			if (del is not null) {
				using (var nwReport = new NWDataTransferReport (report, owns: false))
					del (nwReport);
			}
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void Collect (DispatchQueue queue, Action<NWDataTransferReport> handler)
		{
			if (queue is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (queue));
			if (handler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));

			unsafe {
#if NET
				delegate* unmanaged<IntPtr, IntPtr, void> trampoline = &TrampolineCollectHandler;
				using var block = new BlockLiteral (trampoline, handler, typeof (NWDataTransferReport), nameof (TrampolineCollectHandler));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_CollectHandler, handler);
#endif
				nw_data_transfer_report_collect (GetCheckedHandle (), queue.Handle, &block);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern NWDataTransferReportState nw_data_transfer_report_get_state (OS_nw_data_transfer_report report);

		public NWDataTransferReportState State => nw_data_transfer_report_get_state (GetCheckedHandle ());

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
		[DllImport (Constants.NetworkLibrary)]
		static extern NWInterfaceRadioType nw_data_transfer_report_get_path_radio_type (OS_nw_data_transfer_report report, uint pathIndex);

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
		public NWInterfaceRadioType GetPathRadioType (uint pathIndex)
			=> nw_data_transfer_report_get_path_radio_type (GetCheckedHandle (), pathIndex);

#if !XAMCORE_5_0
#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
		[Obsolete ("Use the 'GetPathRadioType' property instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public NWInterfaceRadioType get_path_radio_type (uint pathIndex)
			=> nw_data_transfer_report_get_path_radio_type (GetCheckedHandle (), pathIndex);
#endif
	}
}
