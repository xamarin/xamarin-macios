//
// NWWebSocketRequest.cs: Bindings the Network nw_ws_request_t API.
//
// Authors:
//   Manuel de la Pena (mandel@microsoft.com)
//
// Copyrigh 2019 Microsoft Inc
//

#nullable enable

using System.Runtime.CompilerServices;
using CoreFoundation;

using OS_nw_ws_request = System.IntPtr;

namespace Network {
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	public class NWWebSocketRequest : NativeObject {
		[Preserve (Conditional = true)]
		internal NWWebSocketRequest (NativeHandle handle, bool owns) : base (handle, owns) { }

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern byte nw_ws_request_enumerate_additional_headers (OS_nw_ws_request request, BlockLiteral* enumerator);

		[UnmanagedCallersOnly]
		static byte TrampolineEnumerateHeaderHandler (IntPtr block, IntPtr headerPointer, IntPtr valuePointer)
		{
			var del = BlockLiteral.GetTarget<Action<string?, string?>> (block);
			if (del is not null) {
				var header = Marshal.PtrToStringAuto (headerPointer);
				var value = Marshal.PtrToStringAuto (valuePointer);
				del (header, value);
			}
			return 1;
		}

		/// <summary>Enumerates the additional HTTP headers in the client's WebSocket request.</summary>
		/// <param name="handler">The callback invoked for each header, with the header name and value.</param>
		/// <exception cref="ArgumentNullException"><paramref name="handler" /> is <see langword="null" />.</exception>
		/// <exception cref="ObjectDisposedException">This request has been disposed.</exception>
		/// <remarks>The callback is invoked synchronously and cannot stop enumeration early.</remarks>
		[BindingImpl (BindingImplOptions.Optimizable)]
		public void EnumerateAdditionalHeaders (Action<string?, string?> handler)
		{
			if (handler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));

			unsafe {
				delegate* unmanaged<IntPtr, IntPtr, IntPtr, byte> trampoline = &TrampolineEnumerateHeaderHandler;
				using var block = new BlockLiteral (trampoline, handler, typeof (NWWebSocketRequest), nameof (TrampolineEnumerateHeaderHandler));
				nw_ws_request_enumerate_additional_headers (GetCheckedHandle (), &block);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern byte nw_ws_request_enumerate_subprotocols (OS_nw_ws_request request, BlockLiteral* enumerator);

		[UnmanagedCallersOnly]
		static byte TrampolineEnumerateSubprotocolHandler (IntPtr block, IntPtr subprotocolPointer)
		{
			var del = BlockLiteral.GetTarget<Action<string?>> (block);
			if (del is not null) {
				var subprotocol = Marshal.PtrToStringAuto (subprotocolPointer);
				del (subprotocol);
			}
			return 1;
		}

		/// <summary>Enumerates the subprotocols in the client's WebSocket request.</summary>
		/// <param name="handler">The callback invoked for each subprotocol, with the subprotocol name.</param>
		/// <exception cref="ArgumentNullException"><paramref name="handler" /> is <see langword="null" />.</exception>
		/// <exception cref="ObjectDisposedException">This request has been disposed.</exception>
		/// <remarks>The callback is invoked synchronously and cannot stop enumeration early.</remarks>
		[BindingImpl (BindingImplOptions.Optimizable)]
		public void EnumerateSubprotocols (Action<string?> handler)
		{
			if (handler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));

			unsafe {
				delegate* unmanaged<IntPtr, IntPtr, byte> trampoline = &TrampolineEnumerateSubprotocolHandler;
				using var block = new BlockLiteral (trampoline, handler, typeof (NWWebSocketRequest), nameof (TrampolineEnumerateSubprotocolHandler));
				nw_ws_request_enumerate_subprotocols (GetCheckedHandle (), &block);
			}
		}
	}
}
