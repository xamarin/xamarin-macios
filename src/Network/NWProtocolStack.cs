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
	
	public class NWProtocolStack : INativeObject, IDisposable {
		IntPtr handle;
		public IntPtr Handle {
			get { return handle; }
		}

		public NWProtocolStack (IntPtr handle, bool owns)
		{
			this.handle = handle;
			if (owns == false)
				CFObject.CFRetain (handle);
		}

		public NWProtocolStack (IntPtr handle) : this (handle, false)
		{
		}

		~NWProtocolStack ()
		{
			Dispose (false);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		public virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero) {
				CFObject.CFRelease (handle);
				handle = IntPtr.Zero;
			}
		}

		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_protocol_stack_prepend_application_protocol (nw_protocol_stack_t stack, nw_protocol_options_t options);

		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		public void PrependApplicationProtocol (NWProtocolOptions options)
		{
			if (options == null)
				throw new ArgumentNullException (nameof (options));
			nw_protocol_stack_prepend_application_protocol (handle, options.Handle);
		}

		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_protocol_stack_clear_application_protocols (nw_protocol_stack_t stack);

		public void ClearApplicationProtocols ()
		{
			nw_protocol_stack_clear_application_protocols (handle);
		}

		delegate void nw_protocol_stack_iterate_protocols_block_t (IntPtr block, IntPtr options);
		static nw_protocol_stack_iterate_protocols_block_t static_iterateHandler = TrampolineIterateHandler;

                [MonoPInvokeCallback (typeof (nw_protocol_stack_iterate_protocols_block_t))]
		static unsafe void TrampolineIterateHandler (IntPtr block, IntPtr options)
		{
                        var descriptor = (BlockLiteral *) block;
                        var del = (Action<NWProtocolOptions>) (descriptor->Target);
                        if (del != null){
				var x = new NWProtocolOptions (options, owns: false);
                                del (x);
				x.Dispose ();
			}
		}

		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		unsafe extern static void nw_protocol_stack_iterate_application_protocols (nw_protocol_stack_t stack, void *completion);
		public void IterateProtocols (Action<NWProtocolOptions> callback)
		{
                        unsafe {
                                BlockLiteral *block_ptr_handler;
                                BlockLiteral block_handler;
                                block_handler = new BlockLiteral ();
                                block_ptr_handler = &block_handler;
                                block_handler.SetupBlockUnsafe (static_iterateHandler, callback);

                                nw_protocol_stack_iterate_application_protocols (Handle, (void*) block_ptr_handler);
                                block_ptr_handler->CleanupBlock ();
                        }
		}

		[Watch (5, 0), TV (12, 0), Mac (10, 14), iOS (12, 0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_protocol_stack_copy_transport_protocol (nw_protocol_stack_t stack);

		[Watch (5, 0), TV (12, 0), Mac (10, 14), iOS (12, 0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_protocol_stack_set_transport_protocol (nw_protocol_stack_t stack, IntPtr value);
		
		[Watch (5, 0), TV (12, 0), Mac (10, 14), iOS (12, 0)]
		public NWProtocolOptions TransportProtocol {
			get => new NWProtocolOptions (nw_protocol_stack_copy_transport_protocol (handle), owns: true);
			set => nw_protocol_stack_set_transport_protocol (handle, value.Handle);
		}

		[Watch (5, 0), TV (12, 0), Mac (10, 14), iOS (12, 0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_protocol_stack_copy_internet_protocol (nw_protocol_stack_t stack);

		[Watch (5, 0), TV (12, 0), Mac (10, 14), iOS (12, 0)]
		public NWProtocolOptions InternetProtocol => new NWProtocolOptions (nw_protocol_stack_copy_internet_protocol (handle), owns: true);
	}
}
