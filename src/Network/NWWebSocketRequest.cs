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
		static void TrampolineEnumerateHeaderHandler (IntPtr block, IntPtr headerPointer, IntPtr valuePointer)
		{
			var del = BlockLiteral.GetTarget<Action<string?, string?>> (block);
			if (del is not null) {
				var header = Marshal.PtrToStringAuto (headerPointer);
				var value = Marshal.PtrToStringAuto (valuePointer);
				del (header, value);
			}
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void EnumerateAdditionalHeaders (Action<string?, string?> handler)
		{
			if (handler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));

			unsafe {
				delegate* unmanaged<IntPtr, IntPtr, IntPtr, void> trampoline = &TrampolineEnumerateHeaderHandler;
				using var block = new BlockLiteral (trampoline, handler, typeof (NWWebSocketRequest), nameof (TrampolineEnumerateHeaderHandler));
				nw_ws_request_enumerate_additional_headers (GetCheckedHandle (), &block);
				GC.KeepAlive (this);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern byte nw_ws_request_enumerate_subprotocols (OS_nw_ws_request request, BlockLiteral* enumerator);

		[UnmanagedCallersOnly]
		static void TrampolineEnumerateSubprotocolHandler (IntPtr block, IntPtr subprotocolPointer)
		{
			var del = BlockLiteral.GetTarget<Action<string?>> (block);
			if (del is not null) {
				var subprotocol = Marshal.PtrToStringAuto (subprotocolPointer);
				del (subprotocol);
			}
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void EnumerateSubprotocols (Action<string?> handler)
		{
			if (handler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));

			unsafe {
				delegate* unmanaged<IntPtr, IntPtr, void> trampoline = &TrampolineEnumerateSubprotocolHandler;
				using var block = new BlockLiteral (trampoline, handler, typeof (NWWebSocketRequest), nameof (TrampolineEnumerateSubprotocolHandler));
				nw_ws_request_enumerate_subprotocols (GetCheckedHandle (), &block);
				GC.KeepAlive (this);
			}
		}
	}
}
