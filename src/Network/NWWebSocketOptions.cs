//
// NWWebSocketOptions.cs: Bindings the Network nw_browser_t API.
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

using OS_nw_protocol_options=System.IntPtr;
using nw_ws_request_t=System.IntPtr;

namespace Network {

	// this maps to `nw_ws_version_t` in Network.framework/Headers/ws_options.h (and not the enum from NetworkExtension)
	[TV (13,0), Mac (10,15), iOS (13,0), Watch (6,0)]
	public enum NWWebSocketVersion {
		Invalid = 0,
		Version13 = 1,
	}

	[TV (12,0), Mac (10,14), iOS (12,0)]
	[Watch (6,0)]
	public class NWWebSocketOptions : NWProtocolOptions {
		bool autoReplyPing = false;
		bool skipHandShake = false;
		nuint maximumMessageSize;

		public NWWebSocketOptions (IntPtr handle, bool owns) : base (handle, owns) {}

		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_ws_create_options (NWWebSocketVersion version);

		public NWWebSocketOptions (NWWebSocketVersion version) : base (nw_ws_create_options (version), true) { } 

		[DllImport (Constants.NetworkLibrary, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		static extern void nw_ws_options_add_additional_header (OS_nw_protocol_options options, string name, string value);

		public void SetHeader (string header, string value) => nw_ws_options_add_additional_header (GetCheckedHandle(), header, value); 

		[DllImport (Constants.NetworkLibrary, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		static extern void nw_ws_options_add_subprotocol (OS_nw_protocol_options options, string subprotocol);

		public void AddSubprotocol (string subprotocol) => nw_ws_options_add_subprotocol (GetCheckedHandle (), subprotocol);

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_ws_options_set_auto_reply_ping (OS_nw_protocol_options options, bool auto_reply_ping);

		public bool AutoReplyPing {
			get { return autoReplyPing;}
			set {
				autoReplyPing = value;
				nw_ws_options_set_auto_reply_ping (GetCheckedHandle (), value);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_ws_options_set_maximum_message_size (OS_nw_protocol_options options, nuint maximum_message_size);

		public nuint MaximumMessageSize {
			get { return maximumMessageSize; }
			set {
				maximumMessageSize = value;
				nw_ws_options_set_maximum_message_size (GetCheckedHandle(), value);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_ws_options_set_skip_handshake (OS_nw_protocol_options options, bool skip_handshake);

		public bool SkipHandShake {
			get { return skipHandShake; }
			set {
				skipHandShake = value;
				nw_ws_options_set_skip_handshake (GetCheckedHandle (), value);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_ws_options_set_client_request_handler (OS_nw_protocol_options options, IntPtr client_queue, void *handler);

		delegate void nw_ws_options_set_client_request_handler_t (IntPtr block, nw_ws_request_t request);
		static nw_ws_options_set_client_request_handler_t static_ClientRequestHandler = TrampolineClientRequestHandler;

		[MonoPInvokeCallback (typeof (nw_ws_options_set_client_request_handler_t))]
		static void TrampolineClientRequestHandler (IntPtr block, nw_ws_request_t request)
		{
			var del = BlockLiteral.GetTarget<Action<NWWebSocketRequest>> (block);
			if (del != null) {
				var nwRequest = new NWWebSocketRequest (request, owns: true);
				del (nwRequest);
			}
		}

		public void SetClientRequestHandler (DispatchQueue queue, Action<NWWebSocketRequest> handler)
		{
			if (queue == null)
				throw new ArgumentNullException (nameof (handler));
			unsafe {
				if (handler == null) {
					nw_ws_options_set_client_request_handler (GetCheckedHandle (), queue.Handle, null);
					return;
				}
				BlockLiteral block_handler = new BlockLiteral ();
				BlockLiteral *block_ptr_handler = &block_handler;
				block_handler.SetupBlockUnsafe (static_ClientRequestHandler, handler);
				try {
					nw_ws_options_set_client_request_handler (GetCheckedHandle (), queue.Handle, (void*) block_ptr_handler);
				} finally {
					block_handler.CleanupBlock ();
				}
			}
		}
	}
}