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

	public class NWProtocolStack : NativeObject {
		public NWProtocolStack (IntPtr handle, bool owns) : base (handle, owns) {}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_protocol_stack_prepend_application_protocol (nw_protocol_stack_t stack, nw_protocol_options_t options);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void PrependApplicationProtocol (NWProtocolOptions options)
		{
			if (options == null)
				throw new ArgumentNullException (nameof (options));
			nw_protocol_stack_prepend_application_protocol (GetHandle(), options.Handle);
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_protocol_stack_clear_application_protocols (nw_protocol_stack_t stack);

		public void ClearApplicationProtocols ()
		{
			nw_protocol_stack_clear_application_protocols (GetHandle());
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

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		unsafe extern static void nw_protocol_stack_iterate_application_protocols (nw_protocol_stack_t stack, void *completion);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[BindingImpl (BindingImplOptions.Optimizable)]
		public void IterateProtocols (Action<NWProtocolOptions> callback)
		{
			unsafe {
				BlockLiteral *block_ptr_handler;
				BlockLiteral block_handler;
				block_handler = new BlockLiteral ();
				block_ptr_handler = &block_handler;
				block_handler.SetupBlockUnsafe (static_iterateHandler, callback);

				nw_protocol_stack_iterate_application_protocols (GetHandle(), (void*) block_ptr_handler);
				block_ptr_handler->CleanupBlock ();
			}
		}

		[TV (12, 0), Mac (10, 14), iOS (12, 0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_protocol_stack_copy_transport_protocol (nw_protocol_stack_t stack);

		[TV (12, 0), Mac (10, 14), iOS (12, 0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_protocol_stack_set_transport_protocol (nw_protocol_stack_t stack, IntPtr value);

		[TV (12, 0), Mac (10, 14), iOS (12, 0)]
		public NWProtocolOptions TransportProtocol {
			get => new NWProtocolOptions (nw_protocol_stack_copy_transport_protocol (GetHandle()), owns: true);
			set => nw_protocol_stack_set_transport_protocol (GetHandle(), value.Handle);
		}

		[TV (12, 0), Mac (10, 14), iOS (12, 0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_protocol_stack_copy_internet_protocol (nw_protocol_stack_t stack);

		[TV (12, 0), Mac (10, 14), iOS (12, 0)]
		public NWProtocolOptions InternetProtocol => new NWProtocolOptions (nw_protocol_stack_copy_internet_protocol (GetHandle()), owns: true);
	}
}
