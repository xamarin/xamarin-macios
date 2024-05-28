using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mono.Cecil;
using Mono.Cecil.Cil;

using ObjCRuntime;

using Xamarin.Tests;
using Xamarin.Utils;

#nullable enable

namespace Cecil.Tests {
	public partial class BlittablePInvokes {
		static HashSet<string> knownFailuresPInvokes = new HashSet<string> {
			"AVFoundation.AVSampleCursorAudioDependencyInfo ObjCRuntime.Messaging::AVSampleCursorAudioDependencyInfo_objc_msgSend(System.IntPtr,System.IntPtr)",
			"AVFoundation.AVSampleCursorAudioDependencyInfo ObjCRuntime.Messaging::AVSampleCursorAudioDependencyInfo_objc_msgSendSuper(System.IntPtr,System.IntPtr)",
			"AVFoundation.AVSampleCursorChunkInfo ObjCRuntime.Messaging::AVSampleCursorChunkInfo_objc_msgSend_stret(System.IntPtr,System.IntPtr)",
			"AVFoundation.AVSampleCursorChunkInfo ObjCRuntime.Messaging::AVSampleCursorChunkInfo_objc_msgSend(System.IntPtr,System.IntPtr)",
			"AVFoundation.AVSampleCursorChunkInfo ObjCRuntime.Messaging::AVSampleCursorChunkInfo_objc_msgSendSuper_stret(System.IntPtr,System.IntPtr)",
			"AVFoundation.AVSampleCursorChunkInfo ObjCRuntime.Messaging::AVSampleCursorChunkInfo_objc_msgSendSuper(System.IntPtr,System.IntPtr)",
			"AVFoundation.AVSampleCursorSyncInfo ObjCRuntime.Messaging::AVSampleCursorSyncInfo_objc_msgSend_stret(System.IntPtr,System.IntPtr)",
			"AVFoundation.AVSampleCursorSyncInfo ObjCRuntime.Messaging::AVSampleCursorSyncInfo_objc_msgSend(System.IntPtr,System.IntPtr)",
			"AVFoundation.AVSampleCursorSyncInfo ObjCRuntime.Messaging::AVSampleCursorSyncInfo_objc_msgSendSuper_stret(System.IntPtr,System.IntPtr)",
			"AVFoundation.AVSampleCursorSyncInfo ObjCRuntime.Messaging::AVSampleCursorSyncInfo_objc_msgSendSuper(System.IntPtr,System.IntPtr)",
			"System.Boolean Network.NWAdvertiseDescriptor::nw_advertise_descriptor_get_no_auto_rename(System.IntPtr)",
			"System.Boolean Network.NWBrowserDescriptor::nw_browse_descriptor_get_include_txt_record(System.IntPtr)",
			"System.Boolean Network.NWConnectionGroup::nw_connection_group_reinsert_extracted_connection(System.IntPtr,System.IntPtr)",
			"System.Boolean Network.NWContentContext::nw_content_context_get_is_final(System.IntPtr)",
			"System.Boolean Network.NWEstablishmentReport::nw_establishment_report_get_proxy_configured(System.IntPtr)",
			"System.Boolean Network.NWEstablishmentReport::nw_establishment_report_get_used_proxy(System.IntPtr)",
			"System.Boolean Network.NWFramer::nw_framer_deliver_input_no_copy(System.IntPtr,System.UIntPtr,System.IntPtr,System.Boolean)",
			"System.Boolean Network.NWFramer::nw_framer_parse_input(System.IntPtr,System.UIntPtr,System.UIntPtr,System.Byte*,ObjCRuntime.BlockLiteral*)",
			"System.Boolean Network.NWFramer::nw_framer_parse_output(System.IntPtr,System.UIntPtr,System.UIntPtr,System.Byte*,ObjCRuntime.BlockLiteral*)",
			"System.Boolean Network.NWFramer::nw_framer_prepend_application_protocol(System.IntPtr,System.IntPtr)",
			"System.Boolean Network.NWFramer::nw_framer_write_output_no_copy(System.IntPtr,System.UIntPtr)",
			"System.Boolean Network.NWFramerMessage::nw_framer_message_access_value(System.IntPtr,System.IntPtr,ObjCRuntime.BlockLiteral*)",
			"System.Boolean Network.NWMulticastGroup::nw_group_descriptor_add_endpoint(System.IntPtr,System.IntPtr)",
			"System.Boolean Network.NWMulticastGroup::nw_multicast_group_descriptor_get_disable_unicast_traffic(System.IntPtr)",
			"System.Boolean Network.NWParameters::nw_parameters_get_fast_open_enabled(System.IntPtr)",
			"System.Boolean Network.NWParameters::nw_parameters_get_include_peer_to_peer(System.IntPtr)",
			"System.Boolean Network.NWParameters::nw_parameters_get_local_only(System.IntPtr)",
			"System.Boolean Network.NWParameters::nw_parameters_get_prefer_no_proxy(System.IntPtr)",
			"System.Boolean Network.NWParameters::nw_parameters_get_prohibit_constrained(System.IntPtr)",
			"System.Boolean Network.NWParameters::nw_parameters_get_prohibit_expensive(System.IntPtr)",
			"System.Boolean Network.NWParameters::nw_parameters_get_reuse_local_address(System.IntPtr)",
			"System.Boolean Network.NWParameters::nw_parameters_requires_dnssec_validation(System.IntPtr)",
			"System.Boolean Network.NWPath::nw_path_has_dns(System.IntPtr)",
			"System.Boolean Network.NWPath::nw_path_has_ipv4(System.IntPtr)",
			"System.Boolean Network.NWPath::nw_path_has_ipv6(System.IntPtr)",
			"System.Boolean Network.NWPath::nw_path_is_constrained(System.IntPtr)",
			"System.Boolean Network.NWPath::nw_path_is_equal(System.IntPtr,System.IntPtr)",
			"System.Boolean Network.NWPath::nw_path_is_expensive(System.IntPtr)",
			"System.Boolean Network.NWPath::nw_path_uses_interface_type(System.IntPtr,Network.NWInterfaceType)",
			"System.Boolean Network.NWTxtRecord::nw_txt_record_access_bytes(System.IntPtr,ObjCRuntime.BlockLiteral*)",
			"System.Boolean Network.NWTxtRecord::nw_txt_record_access_key(System.IntPtr,System.IntPtr,ObjCRuntime.BlockLiteral*)",
			"System.Boolean Network.NWTxtRecord::nw_txt_record_apply(System.IntPtr,ObjCRuntime.BlockLiteral*)",
			"System.Boolean Network.NWTxtRecord::nw_txt_record_is_equal(System.IntPtr,System.IntPtr)",
			"System.Boolean Network.NWWebSocketRequest::nw_ws_request_enumerate_additional_headers(System.IntPtr,ObjCRuntime.BlockLiteral*)",
			"System.Boolean Network.NWWebSocketRequest::nw_ws_request_enumerate_subprotocols(System.IntPtr,ObjCRuntime.BlockLiteral*)",
			"System.Boolean Network.NWWebSocketResponse::nw_ws_response_enumerate_additional_headers(System.IntPtr,ObjCRuntime.BlockLiteral*)",
			"System.Byte* Network.NWEndpoint::nw_endpoint_get_signature(System.IntPtr,System.UIntPtr&)",
			"System.Int32 AudioUnit.AUGraph::NewAUGraph(System.IntPtr&)",
			"System.IntPtr ObjCRuntime.Selector::GetHandle(System.String)",
			"System.IntPtr Security.SecKey::SecKeyCreateEncryptedData(System.IntPtr,System.IntPtr,System.IntPtr,System.IntPtr&)",
			"System.Void Network.NWAdvertiseDescriptor::nw_advertise_descriptor_set_no_auto_rename(System.IntPtr,System.Boolean)",
			"System.Void Network.NWBrowserDescriptor::nw_browse_descriptor_set_include_txt_record(System.IntPtr,System.Boolean)",
			"System.Void Network.NWConnection::nw_connection_send(System.IntPtr,System.IntPtr,System.IntPtr,System.Boolean,ObjCRuntime.BlockLiteral*)",
			"System.Void Network.NWConnectionGroup::nw_connection_group_set_receive_handler(System.IntPtr,System.UInt32,System.Boolean,ObjCRuntime.BlockLiteral*)",
			"System.Void Network.NWContentContext::nw_content_context_set_is_final(System.IntPtr,System.Boolean)",
			"System.Void Network.NWFramer::nw_framer_deliver_input(System.IntPtr,System.Byte*,System.UIntPtr,System.IntPtr,System.Boolean)",
			"System.Void Network.NWMulticastGroup::nw_multicast_group_descriptor_set_disable_unicast_traffic(System.IntPtr,System.Boolean)",
			"System.Void Network.NWParameters::nw_parameters_set_fast_open_enabled(System.IntPtr,System.Boolean)",
			"System.Void Network.NWParameters::nw_parameters_set_include_peer_to_peer(System.IntPtr,System.Boolean)",
			"System.Void Network.NWParameters::nw_parameters_set_local_only(System.IntPtr,System.Boolean)",
			"System.Void Network.NWParameters::nw_parameters_set_prefer_no_proxy(System.IntPtr,System.Boolean)",
			"System.Void Network.NWParameters::nw_parameters_set_prohibit_constrained(System.IntPtr,System.Boolean)",
			"System.Void Network.NWParameters::nw_parameters_set_prohibit_expensive(System.IntPtr,System.Boolean)",
			"System.Void Network.NWParameters::nw_parameters_set_requires_dnssec_validation(System.IntPtr,System.Boolean)",
			"System.Void Network.NWParameters::nw_parameters_set_reuse_local_address(System.IntPtr,System.Boolean)",
			"System.Void Network.NWPrivacyContext::nw_privacy_context_require_encrypted_name_resolution(System.IntPtr,System.Boolean,System.IntPtr)",
			"System.Void Network.NWWebSocketOptions::nw_ws_options_set_auto_reply_ping(System.IntPtr,System.Boolean)",
			"System.Void Network.NWWebSocketOptions::nw_ws_options_set_skip_handshake(System.IntPtr,System.Boolean)",
			"System.Void ObjCRuntime.Messaging::void_objc_msgSend_GCDualSenseAdaptiveTriggerPositionalAmplitudes_float(System.IntPtr,System.IntPtr,GameController.GCDualSenseAdaptiveTriggerPositionalAmplitudes,System.Single)",
			"System.Void ObjCRuntime.Messaging::void_objc_msgSend_GCDualSenseAdaptiveTriggerPositionalResistiveStrengths(System.IntPtr,System.IntPtr,GameController.GCDualSenseAdaptiveTriggerPositionalResistiveStrengths)",
			"System.Void ObjCRuntime.Messaging::void_objc_msgSendSuper_GCDualSenseAdaptiveTriggerPositionalAmplitudes_float(System.IntPtr,System.IntPtr,GameController.GCDualSenseAdaptiveTriggerPositionalAmplitudes,System.Single)",
			"System.Void ObjCRuntime.Messaging::void_objc_msgSendSuper_GCDualSenseAdaptiveTriggerPositionalResistiveStrengths(System.IntPtr,System.IntPtr,GameController.GCDualSenseAdaptiveTriggerPositionalResistiveStrengths)",
		};
	}
}
