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

using OS_nw_data_transfer_report=System.IntPtr;
using OS_nw_connection=System.IntPtr;
using OS_nw_interface=System.IntPtr;

namespace Network {

	[TV (13,0), Mac (10,15), iOS (13,0), Watch (6,0)]
	public enum NWDataTransferReportState {
		Collecting = 1,
		Collected = 2,
	}

	[TV (13,0), Mac (10,15), iOS (13,0), Watch (6,0)]
	public class NWDataTransferReport : NativeObject {

		public NWDataTransferReport (IntPtr handle, bool owns) : base (handle, owns) {}

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_data_transfer_report nw_connection_create_new_data_transfer_report (OS_nw_connection connection);

		public NWDataTransferReport (NWConnection connection)
		{
			if (connection == null)
				throw new ArgumentNullException (nameof (connection));

			InitializeHandle (nw_connection_create_new_data_transfer_report (connection.Handle));
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern uint nw_data_transfer_report_get_path_count (OS_nw_data_transfer_report report);

		public uint PathCount => nw_data_transfer_report_get_path_count (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		static extern ulong nw_data_transfer_report_get_duration_milliseconds (OS_nw_data_transfer_report report);

		public ulong Duration => nw_data_transfer_report_get_duration_milliseconds (GetCheckedHandle ());

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

		public ulong GetTransportSmoothedRoundTripTime (uint pathIndex)
			=> nw_data_transfer_report_get_transport_smoothed_rtt_milliseconds (GetCheckedHandle (), pathIndex);

		[DllImport (Constants.NetworkLibrary)]
		static extern ulong nw_data_transfer_report_get_transport_minimum_rtt_milliseconds (OS_nw_data_transfer_report report, uint path_index);

		public ulong GetTransportMinimumRoundTripTime (uint pathIndex)
			=> nw_data_transfer_report_get_transport_minimum_rtt_milliseconds (GetCheckedHandle (), pathIndex);

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
		unsafe static extern void nw_data_transfer_report_collect (OS_nw_data_transfer_report report, IntPtr queue, void *collect_block);

		delegate void nw_data_transfer_report_collect_t (IntPtr block, IntPtr report);
		static nw_data_transfer_report_collect_t static_CollectHandler = TrampolineCollectHandler;

		[MonoPInvokeCallback (typeof (nw_data_transfer_report_collect_t))]
		static void TrampolineCollectHandler (IntPtr block, IntPtr report)
		{
			var del = BlockLiteral.GetTarget<Action<NWDataTransferReport>> (block);
			if (del != null) {
				var nwReport = new NWDataTransferReport (report, owns: false);
				del (nwReport);
			}
		}

		public void Collect (DispatchQueue queue, Action<NWDataTransferReport> handler)
		{
			if (queue == null)
				throw new ArgumentNullException (nameof (queue));
			unsafe {
				if (handler == null) {
					nw_data_transfer_report_collect (GetCheckedHandle (), queue.Handle, null);
					return;
				}
				BlockLiteral block_handler = new BlockLiteral ();
				BlockLiteral *block_ptr_handler = &block_handler;
				block_handler.SetupBlockUnsafe (static_CollectHandler, handler);
				try {
					nw_data_transfer_report_collect (GetCheckedHandle (), queue.Handle, (void*) block_ptr_handler);
				} finally {
					block_handler.CleanupBlock ();
				}
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern NWDataTransferReportState nw_data_transfer_report_get_state (OS_nw_data_transfer_report report);

		public NWDataTransferReportState State => nw_data_transfer_report_get_state (GetCheckedHandle ());
	}
}
