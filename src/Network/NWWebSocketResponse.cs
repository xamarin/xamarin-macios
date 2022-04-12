//
// NWWebSocketResponse.cs: Bindings the Network nw_browser_t API.
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

using OS_nw_ws_response=System.IntPtr;

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
	public class NWWebSocketResponse : NativeObject {

		[Preserve (Conditional = true)]
		internal NWWebSocketResponse (NativeHandle handle, bool owns) : base (handle, owns) {}

		[DllImport (Constants.NetworkLibrary, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		static extern unsafe OS_nw_ws_response nw_ws_response_create (NWWebSocketResponseStatus status, string selected_subprotocol);

		public NWWebSocketResponse (NWWebSocketResponseStatus status, string subprotocol)
			=> InitializeHandle (nw_ws_response_create (status, subprotocol));

		[DllImport (Constants.NetworkLibrary, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		static extern string nw_ws_response_get_selected_subprotocol (OS_nw_ws_response response);

		public string SelectedSubprotocol => nw_ws_response_get_selected_subprotocol (GetCheckedHandle ()); 

		[DllImport (Constants.NetworkLibrary)]
		static extern NWWebSocketResponseStatus nw_ws_response_get_status (OS_nw_ws_response response);

		public NWWebSocketResponseStatus Status => nw_ws_response_get_status (GetCheckedHandle ()); 

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		unsafe static extern bool nw_ws_response_enumerate_additional_headers (OS_nw_ws_response response, ref BlockLiteral enumerator);

		delegate void nw_ws_response_enumerate_additional_headers_t (IntPtr block, string header, string value);
		static nw_ws_response_enumerate_additional_headers_t static_EnumerateHeadersHandler = TrampolineEnumerateHeadersHandler;

		[MonoPInvokeCallback (typeof (nw_ws_response_enumerate_additional_headers_t))]
		static void TrampolineEnumerateHeadersHandler (IntPtr block, string header, string value)
		{
			var del = BlockLiteral.GetTarget<Action<string, string>> (block);
			if (del != null) {
				del (header, value);
			}
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public bool EnumerateAdditionalHeaders (Action<string, string> handler)
		{
			if (handler == null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));

			BlockLiteral block_handler = new BlockLiteral ();
			block_handler.SetupBlockUnsafe (static_EnumerateHeadersHandler, handler);
			try {
				return nw_ws_response_enumerate_additional_headers (GetCheckedHandle (), ref block_handler);
			} finally {
				block_handler.CleanupBlock ();
			}
		}

		[DllImport (Constants.NetworkLibrary, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		static extern void nw_ws_response_add_additional_header (OS_nw_ws_response response, string name, string value);

		public void SetHeader (string header, string value) => nw_ws_response_add_additional_header (GetCheckedHandle (), header, value);
	}
}
