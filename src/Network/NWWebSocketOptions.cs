//
// NWWebSocketOptions.cs: Bindings the Network nw_browser_t API.
//
// Authors:
//   Manuel de la Pena (mandel@microsoft.com)
//
// Copyrigh 2019 Microsoft Inc
//

#nullable enable

using System.ComponentModel;
using System.Runtime.CompilerServices;
using CoreFoundation;

using OS_nw_protocol_options = System.IntPtr;
using nw_ws_request_t = System.IntPtr;

namespace Network {
	/// <summary>Handles a WebSocket client's handshake request.</summary>
	/// <param name="request">The client's WebSocket handshake request.</param>
	/// <returns>The server's response to the handshake.</returns>
	public delegate NWWebSocketResponse NWWebSocketClientRequestHandler (NWWebSocketRequest request);

	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
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
		static extern void nw_ws_options_set_auto_reply_ping (OS_nw_protocol_options options, byte auto_reply_ping);

		public bool AutoReplyPing {
			get { return autoReplyPing; }
			set {
				autoReplyPing = value;
				nw_ws_options_set_auto_reply_ping (GetCheckedHandle (), value.AsByte ());
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
		static extern void nw_ws_options_set_skip_handshake (OS_nw_protocol_options options, byte skip_handshake);

		public bool SkipHandShake {
			get { return skipHandShake; }
			set {
				skipHandShake = value;
				nw_ws_options_set_skip_handshake (GetCheckedHandle (), value.AsByte ());
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_ws_options_set_client_request_handler (OS_nw_protocol_options options, IntPtr client_queue, BlockLiteral* handler);

		[UnmanagedCallersOnly]
		static IntPtr TrampolineClientRequestHandler (IntPtr block, nw_ws_request_t request)
		{
			var del = BlockLiteral.GetTarget<NWWebSocketClientRequestHandler> (block);
			if (del is null)
				return IntPtr.Zero;

			using var nwRequest = new NWWebSocketRequest (request, owns: false);
			var response = del (nwRequest);
			response ??= new NWWebSocketResponse (NWWebSocketResponseStatus.Reject, "ignored");
			return ObjCRuntime.Runtime.RetainAndAutoreleaseHandle (response);
		}

		void SetClientRequestHandlerCore (DispatchQueue queue, NWWebSocketClientRequestHandler handler)
		{
			if (queue is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (queue));
			if (handler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));

			unsafe {
				delegate* unmanaged<IntPtr, nw_ws_request_t, IntPtr> trampoline = &TrampolineClientRequestHandler;
				using var block = new BlockLiteral (trampoline, handler, typeof (NWWebSocketOptions), nameof (TrampolineClientRequestHandler));
				nw_ws_options_set_client_request_handler (GetCheckedHandle (), queue.Handle, &block);
				GC.KeepAlive (queue);
			}
		}

#if XAMCORE_5_0
		/// <summary>Sets the callback that handles incoming WebSocket handshake requests.</summary>
		/// <param name="queue">The queue on which to invoke <paramref name="handler" />.</param>
		/// <param name="handler">The callback that returns the server's response to each request.</param>
		[BindingImpl (BindingImplOptions.Optimizable)]
		public void SetClientRequestHandler (DispatchQueue queue, NWWebSocketClientRequestHandler handler)
			=> SetClientRequestHandlerCore (queue, handler);
#endif

#if !XAMCORE_6_0
		/// <summary>Sets the callback that handles incoming WebSocket handshake requests.</summary>
		/// <param name="queue">The queue on which to invoke <paramref name="handler" />.</param>
		/// <param name="handler">The callback that returns the server's response to each request.</param>
#if XAMCORE_5_0
		[Obsolete ("Use 'SetClientRequestHandler' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
#endif
		[BindingImpl (BindingImplOptions.Optimizable)]
		public void SetClientRequestHandler2 (DispatchQueue queue, NWWebSocketClientRequestHandler handler)
			=> SetClientRequestHandlerCore (queue, handler);
#endif

#if !XAMCORE_5_0
		[Obsolete ("Use 'SetClientRequestHandler2' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		[OverloadResolutionPriority (-1)]
		public void SetClientRequestHandler (DispatchQueue queue, Action<NWWebSocketRequest> handler)
		{
			if (handler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));

			SetClientRequestHandler2 (queue, request => {
				handler (request);
				return new NWWebSocketResponse (NWWebSocketResponseStatus.Reject, "ignored");
			});
		}
#endif
	}
}
