//
// NWBrowseResult.cs: Bindings the Network nw_browse_result_t API.
//
// Authors:
//   Manuel de la Pena (mandel@microsoft.com)
//
// Copyright 2019 Microsoft Inc
//
#nullable enable

using System.Runtime.CompilerServices;
using CoreFoundation;

using OS_nw_browse_result = System.IntPtr;
using OS_nw_endpoint = System.IntPtr;
using OS_nw_txt_record = System.IntPtr;

namespace Network {
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	public class NWBrowseResult : NativeObject {

		[Preserve (Conditional = true)]
		internal NWBrowseResult (NativeHandle handle, bool owns) : base (handle, owns) { }

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_endpoint nw_browse_result_copy_endpoint (OS_nw_browse_result result);

		public NWEndpoint EndPoint => new NWEndpoint (nw_browse_result_copy_endpoint (GetCheckedHandle ()), owns: true);

		[DllImport (Constants.NetworkLibrary)]
		static extern nuint nw_browse_result_get_interfaces_count (OS_nw_browse_result result);

		public nuint InterfacesCount => nw_browse_result_get_interfaces_count (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_txt_record nw_browse_result_copy_txt_record_object (OS_nw_browse_result result);

		public NWTxtRecord? TxtRecord {
			get {
				var record = nw_browse_result_copy_txt_record_object (GetCheckedHandle ());
				if (record == IntPtr.Zero)
					return null;
				return new NWTxtRecord (record, owns: true);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern NWBrowseResultChange nw_browse_result_get_changes (OS_nw_browse_result old_result, OS_nw_browse_result new_result);

		public static NWBrowseResultChange GetChanges (NWBrowseResult? oldResult, NWBrowseResult? newResult)
		{
			NWBrowseResultChange result = nw_browse_result_get_changes (oldResult.GetHandle (), newResult.GetHandle ());
			GC.KeepAlive (oldResult);
			GC.KeepAlive (newResult);
			return result;
		}

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_browse_result_enumerate_interfaces (OS_nw_browse_result result, BlockLiteral* enumerator);

		[UnmanagedCallersOnly]
		static byte TrampolineEnumerateInterfacesHandler (IntPtr block, IntPtr inter)
		{
			var del = BlockLiteral.GetTarget<Action<NWInterface>> (block);
			if (del is not null) {
				var nwInterface = new NWInterface (inter, owns: false);
				del (nwInterface);
				return 1;
			}
			return 0;
		}

		/// <summary>Invokes a handler for each network interface associated with this browse result.</summary>
		/// <param name="handler">The handler to invoke for each interface.</param>
		/// <exception cref="ArgumentNullException"><paramref name="handler" /> is <see langword="null" />.</exception>
		/// <remarks>The handler is invoked synchronously. Enumeration continues through all interfaces before this method returns.</remarks>
		[BindingImpl (BindingImplOptions.Optimizable)]
		public void EnumerateInterfaces (Action<NWInterface> handler)
		{
			if (handler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));

			unsafe {
				delegate* unmanaged<IntPtr, IntPtr, byte> trampoline = &TrampolineEnumerateInterfacesHandler;
				using var block = new BlockLiteral (trampoline, handler, typeof (NWBrowseResult), nameof (TrampolineEnumerateInterfacesHandler));
				nw_browse_result_enumerate_interfaces (GetCheckedHandle (), &block);
			}
		}
	}
}
