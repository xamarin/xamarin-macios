//
// NWWebSocketOptions.cs: Bindings the Network nw_browser_t API.
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

using OS_nw_protocol_options = System.IntPtr;
using nw_ws_request_t = System.IntPtr;

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
	public class NWWebSocketOptions : NWProtocolOptions {
		bool autoReplyPing = false;
		bool skipHandShake = false;
		nuint maximumMessageSize;

		[Preserve (Conditional = true)]
		internal NWWebSocketOptions (NativeHandle handle, bool owns) : base (handle, owns) { }

		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_ws_create_options (NWWebSocketVersion version);

		public NWWebSocketOptions (NWWebSocketVersion version) : base (nw_ws_create_options (version), true) { }

		[DllImport (Constants.NetworkLibrary, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		static extern void nw_ws_options_add_additional_header (OS_nw_protocol_options options, IntPtr name, IntPtr value);

		public void SetHeader (string header, string value)
		{
			if (header is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (header));
			using var headerPtr = new TransientString (header);
			using var valuePtr = new TransientString (value);
			nw_ws_options_add_additional_header (GetCheckedHandle (), headerPtr, valuePtr);
		}

		[DllImport (Constants.NetworkLibrary, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		static extern void nw_ws_options_add_subprotocol (OS_nw_protocol_options options, IntPtr subprotocol);

		public void AddSubprotocol (string subprotocol)
		{
			if (subprotocol is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (subprotocol));
			using var subprotocolPtr = new TransientString (subprotocol);
			nw_ws_options_add_subprotocol (GetCheckedHandle (), subprotocolPtr);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_ws_options_set_auto_reply_ping (OS_nw_protocol_options options, [MarshalAs (UnmanagedType.I1)] bool auto_reply_ping);

		public bool AutoReplyPing {
			get { return autoReplyPing; }
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
				nw_ws_options_set_maximum_message_size (GetCheckedHandle (), value);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_ws_options_set_skip_handshake (OS_nw_protocol_options options, [MarshalAs (UnmanagedType.I1)] bool skip_handshake);

		public bool SkipHandShake {
			get { return skipHandShake; }
			set {
				skipHandShake = value;
				nw_ws_options_set_skip_handshake (GetCheckedHandle (), value);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_ws_options_set_client_request_handler (OS_nw_protocol_options options, IntPtr client_queue, BlockLiteral* handler);

#if !NET
		delegate void nw_ws_options_set_client_request_handler_t (IntPtr block, nw_ws_request_t request);
		static nw_ws_options_set_client_request_handler_t static_ClientRequestHandler = TrampolineClientRequestHandler;

		[MonoPInvokeCallback (typeof (nw_ws_options_set_client_request_handler_t))]
#else
		[UnmanagedCallersOnly]
#endif
		static void TrampolineClientRequestHandler (IntPtr block, nw_ws_request_t request)
		{
			var del = BlockLiteral.GetTarget<Action<NWWebSocketRequest>> (block);
			if (del is not null) {
				var nwRequest = new NWWebSocketRequest (request, owns: true);
				del (nwRequest);
			}
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void SetClientRequestHandler (DispatchQueue queue, Action<NWWebSocketRequest> handler)
		{
			if (queue is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));
			if (handler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));

			unsafe {
#if NET
				delegate* unmanaged<IntPtr, nw_ws_request_t, void> trampoline = &TrampolineClientRequestHandler;
				using var block = new BlockLiteral (trampoline, handler, typeof (NWWebSocketOptions), nameof (TrampolineClientRequestHandler));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_ClientRequestHandler, handler);
#endif
				nw_ws_options_set_client_request_handler (GetCheckedHandle (), queue.Handle, &block);
			}
		}
	}
}
