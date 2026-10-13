//
// NWWebSocketResponse.cs: Bindings the Network nw_browser_t API.
//
// Authors:
//   Manuel de la Pena (mandel@microsoft.com)
//
// Copyrigh 2019 Microsoft Inc
//

#nullable enable

using System.Runtime.CompilerServices;
using CoreFoundation;

using OS_nw_ws_response = System.IntPtr;

namespace Network {
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	public class NWWebSocketResponse : NativeObject {

		[Preserve (Conditional = true)]
		internal NWWebSocketResponse (NativeHandle handle, bool owns) : base (handle, owns) { }

		[DllImport (Constants.NetworkLibrary, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		static extern unsafe OS_nw_ws_response nw_ws_response_create (NWWebSocketResponseStatus status, IntPtr selected_subprotocol);

		static unsafe OS_nw_ws_response nw_ws_response_create (NWWebSocketResponseStatus status, string selected_subprotocol)
		{
			using var selected_subprotocolPtr = new TransientString (selected_subprotocol);
			return nw_ws_response_create (status, selected_subprotocolPtr);
		}

		public NWWebSocketResponse (NWWebSocketResponseStatus status, string subprotocol)
			=> InitializeHandle (nw_ws_response_create (status, subprotocol));

		[DllImport (Constants.NetworkLibrary, EntryPoint = "nw_ws_response_get_selected_subprotocol", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		static extern IntPtr nw_ws_response_get_selected_subprotocol_ptr (OS_nw_ws_response response);

		static string nw_ws_response_get_selected_subprotocol (OS_nw_ws_response response)
		{
			var ptr = nw_ws_response_get_selected_subprotocol_ptr (response);
			return TransientString.ToStringAndFree (ptr, TransientString.Encoding.Ansi)!;
		}

		public string SelectedSubprotocol => nw_ws_response_get_selected_subprotocol (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		static extern NWWebSocketResponseStatus nw_ws_response_get_status (OS_nw_ws_response response);

		public NWWebSocketResponseStatus Status => nw_ws_response_get_status (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern byte nw_ws_response_enumerate_additional_headers (OS_nw_ws_response response, BlockLiteral* enumerator);

		[UnmanagedCallersOnly]
		static byte TrampolineEnumerateHeadersHandler (IntPtr block, IntPtr headerPointer, IntPtr valuePointer)
		{
			var del = BlockLiteral.GetTarget<Action<string?, string?>> (block);
			if (del is not null) {
				var header = Marshal.PtrToStringAuto (headerPointer);
				var value = Marshal.PtrToStringAuto (valuePointer);
				del (header, value);
				return 1;
			}
			return 0;
		}

		/// <summary>Enumerates the additional HTTP headers in the WebSocket server's response.</summary>
		/// <param name="handler">The callback invoked for each additional header, receiving its name and value.</param>
		/// <returns><see langword="true" /> if enumeration completed; otherwise, <see langword="false" />.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="handler" /> is <see langword="null" />.</exception>
		/// <remarks>The callback is invoked synchronously and cannot stop enumeration early.</remarks>
		[BindingImpl (BindingImplOptions.Optimizable)]
		public bool EnumerateAdditionalHeaders (Action<string?, string?> handler)
		{
			if (handler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));

			unsafe {
				delegate* unmanaged<IntPtr, IntPtr, IntPtr, byte> trampoline = &TrampolineEnumerateHeadersHandler;
				using var block = new BlockLiteral (trampoline, handler, typeof (NWWebSocketResponse), nameof (TrampolineEnumerateHeadersHandler));
				return nw_ws_response_enumerate_additional_headers (GetCheckedHandle (), &block) != 0;
			}
		}

		[DllImport (Constants.NetworkLibrary, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		static extern void nw_ws_response_add_additional_header (OS_nw_ws_response response, IntPtr name, IntPtr value);

		static void nw_ws_response_add_additional_header (OS_nw_ws_response response, string name, string value)
		{
			using var namePtr = new TransientString (name);
			using var valuePtr = new TransientString (value);
			nw_ws_response_add_additional_header (response, namePtr, valuePtr);
		}

		public void SetHeader (string header, string value) => nw_ws_response_add_additional_header (GetCheckedHandle (), header, value);
	}
}
