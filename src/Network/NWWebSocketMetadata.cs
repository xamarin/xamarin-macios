//
// NWWebSocketMetadata.cs: Bindings the Network nw_browser_t API.
//
// Authors:
//   Manuel de la Pena (mandel@microsoft.com)
//
// Copyrigh 2019 Microsoft Inc
//

#nullable enable

using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using ObjCRuntime;
using Foundation;
using CoreFoundation;

using OS_nw_protocol_metadata=System.IntPtr;
using OS_nw_ws_response=System.IntPtr;
using dispatch_queue_t =System.IntPtr;

namespace Network {

	[TV (13,0), Mac (10,15), iOS (13,0), Watch (6,0)]
	public enum NWWebSocketOpCode : int {
		Cont = 0x0,
		Text = 0x1,
		Binary = 0x2,
		Close = 0x8,
		Ping = 0x9,
		Pong = 0xA,
		Invalid = -1,
	}

	[TV (13,0), Mac (10,15), iOS (13,0), Watch (6,0)]
	public enum NWWebSocketCloseCode : int {
		NormalClosure = 1000,
		GoingAway = 1001,
		ProtocolError = 1002,
		UnsupportedData = 1003,
		NoStatusReceived = 1005,
		AbnormalClosure = 1006,
		InvalidFramePayloadData = 1007,
		PolicyViolation = 1008,
		MessageTooBig = 1009,
		MandatoryExtension = 1010,
		InternalServerError = 1011,
		TlsHandshake = 1015,
	}

	[TV (13,0), Mac (10,15), iOS (13,0), Watch (6,0)]
	public class NWWebSocketMetadata : NWProtocolMetadata {

		internal NWWebSocketMetadata (IntPtr handle, bool owns) : base (handle, owns) {}

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_protocol_metadata nw_ws_create_metadata (NWWebSocketOpCode opcode);

		public NWWebSocketMetadata (NWWebSocketOpCode opcode) : this (nw_ws_create_metadata (opcode), owns: true) {}

		[DllImport (Constants.NetworkLibrary)]
		static extern NWWebSocketCloseCode nw_ws_metadata_get_close_code (OS_nw_protocol_metadata metadata);

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_ws_metadata_set_close_code (OS_nw_protocol_metadata metadata, NWWebSocketCloseCode close_code);

		public NWWebSocketCloseCode CloseCode {
			get => nw_ws_metadata_get_close_code (GetCheckedHandle ());
			set => nw_ws_metadata_set_close_code (GetCheckedHandle (), value);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern NWWebSocketOpCode nw_ws_metadata_get_opcode (OS_nw_protocol_metadata metadata);

		public NWWebSocketOpCode OpCode => nw_ws_metadata_get_opcode (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_ws_metadata_set_pong_handler (OS_nw_protocol_metadata metadata, dispatch_queue_t client_queue, ref BlockLiteral pong_handler);

		delegate void nw_ws_metadata_set_pong_handler_t (IntPtr block, IntPtr error);
		static nw_ws_metadata_set_pong_handler_t static_PongHandler = TrampolinePongHandler;

		[MonoPInvokeCallback (typeof (nw_ws_metadata_set_pong_handler_t))]
		static void TrampolinePongHandler (IntPtr block, IntPtr error)
		{
			var del = BlockLiteral.GetTarget<Action<NWError?>> (block);
			if (del != null) {
				var nwError = (error == IntPtr.Zero)? null : new NWError (error, owns: false);
				del (nwError);
			}
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void SetPongHandler (DispatchQueue queue, Action<NWError?> handler)
		{
			if (queue == null)
				throw new ArgumentNullException (nameof (queue));

			if (handler == null)
				throw new ArgumentNullException (nameof (handler));

			unsafe {
				BlockLiteral block_handler = new BlockLiteral ();
				block_handler.SetupBlockUnsafe (static_PongHandler, handler);
				try {
					nw_ws_metadata_set_pong_handler (GetCheckedHandle (), queue.Handle, ref block_handler);
				} finally {
					block_handler.CleanupBlock ();
				}
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_ws_response nw_ws_metadata_copy_server_response (OS_nw_protocol_metadata metadata);

		public NWWebSocketResponse? ServerResponse {
			get {
				var reponsePtr = nw_ws_metadata_copy_server_response (GetCheckedHandle ());
				return (reponsePtr == IntPtr.Zero) ? null :  new NWWebSocketResponse (reponsePtr, owns: true);
			}
		} 
	}
}