//
// NWWebSocketRequest.cs: Bindings the Network nw_ws_request_t API.
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

using OS_nw_ws_request=System.IntPtr;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Network {

#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("macos10.15")]
	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("maccatalyst")]
#else
	[TV (13,0)]
	[Mac (10,15)]
	[iOS (13,0)]
	[Watch (6,0)]
#endif
	public class NWWebSocketRequest : NativeObject {
		[Preserve (Conditional = true)]
		internal NWWebSocketRequest (NativeHandle handle, bool owns) : base (handle, owns) {}

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		unsafe static extern bool nw_ws_request_enumerate_additional_headers (OS_nw_ws_request request, ref BlockLiteral enumerator);

		delegate void nw_ws_request_enumerate_additional_headers_t (IntPtr block, string header, string value);
		static nw_ws_request_enumerate_additional_headers_t static_EnumerateHeaderHandler = TrampolineEnumerateHeaderHandler;

		[MonoPInvokeCallback (typeof (nw_ws_request_enumerate_additional_headers_t))]
		static void TrampolineEnumerateHeaderHandler (IntPtr block, string header, string value)
		{
			var del = BlockLiteral.GetTarget<Action<string, string>> (block);
			if (del is not null) {
				del (header, value);
			}
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void EnumerateAdditionalHeaders (Action<string, string> handler)
		{
			if (handler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));

			BlockLiteral block_handler = new BlockLiteral ();
			block_handler.SetupBlockUnsafe (static_EnumerateHeaderHandler, handler);
			try {
				nw_ws_request_enumerate_additional_headers (GetCheckedHandle (), ref block_handler);
			} finally {
				block_handler.CleanupBlock ();
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool nw_ws_request_enumerate_subprotocols (OS_nw_ws_request request, ref BlockLiteral enumerator);

		delegate void nw_ws_request_enumerate_subprotocols_t (IntPtr block, string subprotocol);
		static nw_ws_request_enumerate_subprotocols_t static_EnumerateSubprotocolHandler = TrampolineEnumerateSubprotocolHandler;

		[MonoPInvokeCallback (typeof (nw_ws_request_enumerate_subprotocols_t))]
		static void TrampolineEnumerateSubprotocolHandler (IntPtr block, string subprotocol)
		{
			var del = BlockLiteral.GetTarget<Action<string>> (block);
			if (del is not null) {
				del (subprotocol);
			}
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void EnumerateSubprotocols (Action<string> handler)
		{
			if (handler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));

			BlockLiteral block_handler = new BlockLiteral ();
			block_handler.SetupBlockUnsafe (static_EnumerateSubprotocolHandler, handler);
			try {
				nw_ws_request_enumerate_subprotocols (GetCheckedHandle (), ref block_handler);
			} finally {
				block_handler.CleanupBlock ();
			}
		}
	}
}
