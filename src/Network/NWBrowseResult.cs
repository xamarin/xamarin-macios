//
// NWBrowseResult.cs: Bindings the Network nw_browse_result_t API.
//
// Authors:
//   Manuel de la Pena (mandel@microsoft.com)
//
// Copyright 2019 Microsoft Inc
//
#nullable enable

using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using ObjCRuntime;
using Foundation;
using CoreFoundation;

using OS_nw_browse_result=System.IntPtr;
using OS_nw_endpoint=System.IntPtr;
using OS_nw_txt_record=System.IntPtr;

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
	public class NWBrowseResult : NativeObject {

		[Preserve (Conditional = true)]
		internal NWBrowseResult (NativeHandle handle, bool owns) : base (handle, owns) {}

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_endpoint nw_browse_result_copy_endpoint (OS_nw_browse_result result);

		public NWEndpoint EndPoint => new NWEndpoint (nw_browse_result_copy_endpoint (GetCheckedHandle ()), owns: true);

		[DllImport (Constants.NetworkLibrary)]
		static extern nuint nw_browse_result_get_interfaces_count (OS_nw_browse_result result);

		public nuint InterfacesCount => nw_browse_result_get_interfaces_count (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_txt_record nw_browse_result_copy_txt_record_object (OS_nw_browse_result result);

		public NWTxtRecord TxtRecord => new NWTxtRecord (nw_browse_result_copy_txt_record_object (GetCheckedHandle ()), owns: true); 

		[DllImport (Constants.NetworkLibrary)]
		static extern NWBrowseResultChange nw_browse_result_get_changes (OS_nw_browse_result old_result, OS_nw_browse_result new_result);

		public static NWBrowseResultChange GetChanges (NWBrowseResult? oldResult, NWBrowseResult? newResult)
			=> nw_browse_result_get_changes (oldResult.GetHandle (), newResult.GetHandle ());

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_browse_result_enumerate_interfaces (OS_nw_browse_result result, ref BlockLiteral enumerator);

		delegate void nw_browse_result_enumerate_interfaces_t (IntPtr block, IntPtr nwInterface);
		static nw_browse_result_enumerate_interfaces_t static_EnumerateInterfacesHandler = TrampolineEnumerateInterfacesHandler;

		[MonoPInvokeCallback (typeof (nw_browse_result_enumerate_interfaces_t))]
		static void TrampolineEnumerateInterfacesHandler (IntPtr block, IntPtr inter)
		{
			var del = BlockLiteral.GetTarget<Action<NWInterface>> (block);
			if (del != null) {
				var nwInterface = new NWInterface (inter, owns: false);
				del (nwInterface);
			}
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void EnumerateInterfaces (Action<NWInterface> handler)
		{
			if (handler == null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));

			BlockLiteral block_handler = new BlockLiteral ();
			block_handler.SetupBlockUnsafe (static_EnumerateInterfacesHandler, handler);
			try {
				nw_browse_result_enumerate_interfaces (GetCheckedHandle (), ref block_handler);
			} finally {
				block_handler.CleanupBlock ();
			}
		}
	}
}
