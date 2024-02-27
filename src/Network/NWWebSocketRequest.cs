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

using OS_nw_ws_request = System.IntPtr;

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
	public class NWWebSocketRequest : NativeObject {
		[Preserve (Conditional = true)]
		internal NWWebSocketRequest (NativeHandle handle, bool owns) : base (handle, owns) { }

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		unsafe static extern bool nw_ws_request_enumerate_additional_headers (OS_nw_ws_request request, BlockLiteral* enumerator);

#if !NET
		delegate void nw_ws_request_enumerate_additional_headers_t (IntPtr block, IntPtr header, IntPtr value);
		static nw_ws_request_enumerate_additional_headers_t static_EnumerateHeaderHandler = TrampolineEnumerateHeaderHandler;

		[MonoPInvokeCallback (typeof (nw_ws_request_enumerate_additional_headers_t))]
#else
		[UnmanagedCallersOnly]
#endif
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
#if NET
				delegate* unmanaged<IntPtr, IntPtr, IntPtr, void> trampoline = &TrampolineEnumerateHeaderHandler;
				using var block = new BlockLiteral (trampoline, handler, typeof (NWWebSocketRequest), nameof (TrampolineEnumerateHeaderHandler));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_EnumerateHeaderHandler, handler);
#endif
				nw_ws_request_enumerate_additional_headers (GetCheckedHandle (), &block);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		unsafe static extern bool nw_ws_request_enumerate_subprotocols (OS_nw_ws_request request, BlockLiteral* enumerator);

#if !NET
		delegate void nw_ws_request_enumerate_subprotocols_t (IntPtr block, IntPtr subprotocol);
		static nw_ws_request_enumerate_subprotocols_t static_EnumerateSubprotocolHandler = TrampolineEnumerateSubprotocolHandler;

		[MonoPInvokeCallback (typeof (nw_ws_request_enumerate_subprotocols_t))]
#else
		[UnmanagedCallersOnly]
#endif
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
#if NET
				delegate* unmanaged<IntPtr, IntPtr, void> trampoline = &TrampolineEnumerateSubprotocolHandler;
				using var block = new BlockLiteral (trampoline, handler, typeof (NWWebSocketRequest), nameof (TrampolineEnumerateSubprotocolHandler));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_EnumerateSubprotocolHandler, handler);
#endif
				nw_ws_request_enumerate_subprotocols (GetCheckedHandle (), &block);
			}
		}
	}
}
