//
// NWProtocolStack.cs: Bindings the Netowrk nw_protocol_stack_t API.
//
// Authors:
//   Miguel de Icaza (miguel@microsoft.com)
//
// Copyrigh 2018 Microsoft Inc
//
using System;
using System.Runtime.InteropServices;
using ObjCRuntime;
using Foundation;
using CoreFoundation;

using OS_nw_protocol_definition=System.IntPtr;
using OS_nw_protocol_metadata=System.IntPtr;
using nw_service_class_t=System.IntPtr;
using nw_protocol_stack_t=System.IntPtr;
using nw_protocol_options_t=System.IntPtr;

namespace Network {

	[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
	public class NWProtocolStack : NativeObject {
		public NWProtocolStack (IntPtr handle, bool owns) : base (handle, owns) {}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_protocol_stack_prepend_application_protocol (nw_protocol_stack_t stack, nw_protocol_options_t options);

		public void PrependApplicationProtocol (NWProtocolOptions options)
		{
			if (options == null)
				throw new ArgumentNullException (nameof (options));
			nw_protocol_stack_prepend_application_protocol (GetCheckedHandle (), options.Handle);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_protocol_stack_clear_application_protocols (nw_protocol_stack_t stack);

		public void ClearApplicationProtocols ()
		{
			nw_protocol_stack_clear_application_protocols (GetCheckedHandle ());
		}

		delegate void nw_protocol_stack_iterate_protocols_block_t (IntPtr block, IntPtr options);
		static nw_protocol_stack_iterate_protocols_block_t static_iterateHandler = TrampolineIterateHandler;

		[MonoPInvokeCallback (typeof (nw_protocol_stack_iterate_protocols_block_t))]
		static void TrampolineIterateHandler (IntPtr block, IntPtr options)
		{
			var del = BlockLiteral.GetTarget<Action<NWProtocolOptions>> (block);
			if (del != null) {
				var x = new NWProtocolOptions (options, owns: false);
				del (x);
				x.Dispose ();
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_protocol_stack_iterate_application_protocols (nw_protocol_stack_t stack, ref BlockLiteral completion);

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void IterateProtocols (Action<NWProtocolOptions> callback)
		{
			BlockLiteral block_handler = new BlockLiteral ();
			block_handler.SetupBlockUnsafe (static_iterateHandler, callback);

			try {
				nw_protocol_stack_iterate_application_protocols (GetCheckedHandle (), ref block_handler);
			} finally {
				block_handler.CleanupBlock ();
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_protocol_stack_copy_transport_protocol (nw_protocol_stack_t stack);

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_protocol_stack_set_transport_protocol (nw_protocol_stack_t stack, IntPtr value);

		public NWProtocolOptions TransportProtocol {
			get => new NWProtocolOptions (nw_protocol_stack_copy_transport_protocol (GetCheckedHandle ()), owns: true);
			set => nw_protocol_stack_set_transport_protocol (GetCheckedHandle (), value.GetHandle ());
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_protocol_stack_copy_internet_protocol (nw_protocol_stack_t stack);

		public NWProtocolOptions InternetProtocol => new NWProtocolOptions (nw_protocol_stack_copy_internet_protocol (GetCheckedHandle ()), owns: true);
	}
}
