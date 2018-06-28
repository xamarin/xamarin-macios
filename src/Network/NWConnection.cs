//
// NWConnection.cs: Bindings for the Network NWConnection API.
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
using nw_connection_t=System.IntPtr;
using nw_endpoint_t=System.IntPtr;
using nw_parameters_t=System.IntPtr;

namespace Network {
	public enum NWConnectionState {
		Invalid   = 0,
		Waiting   = 1,
		Preparing = 2,
		Ready     = 3,
		Failed    = 4,
		Cancelled = 5
	}
	
	public class NWConnection : INativeObject, IDisposable {
		IntPtr handle;
		public IntPtr Handle {
			get { return handle; }
		}

		~NWConnection ()
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

		public NWConnection (IntPtr handle, bool owns)
		{
			this.handle = handle;
			if (owns == false)
				CFObject.CFRetain (handle);
		}

		public NWConnection (IntPtr handle) : this (handle, false)
		{
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern nw_connection_t nw_connection_create (nw_endpoint_t endpoint, nw_parameters_t parameters);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public NWConnection (NWEndpoint endpoint, NWParameters parameters)
		{
			if (endpoint == null)
				throw new ArgumentNullException (nameof (endpoint));
			if (parameters == null)
				throw new ArgumentNullException (nameof (parameters));
			handle = nw_connection_create (endpoint.handle, parameters.handle);
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern nw_endpoint_t nw_connection_copy_endpoint (nw_connection_t connection);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public NWEndpoint Endpoint {
			get {
				var x = nw_connection_copy_endpoint (handle);
				if (x == IntPtr.Zero)
					return null;
				return new NWEndpoint (x, owns: true);
			}
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern nw_parameters_t nw_connection_copy_parameters (nw_connection_t connection);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public NWParameters Parameters {
			get {
				var x = nw_connection_copy_parameters (handle);
				if (x == IntPtr.Zero)
					return null;
				return new NWParameters (x, owns: true);
			}
		}

		delegate void StateChangeCallback (IntPtr block, NWConnectionState state, IntPtr error);
		static StateChangeCallback static_stateChangeHandler = Trampoline_StateChangeCallback;

		[MonoPInvokeCallback (typeof (StateChangeCallback))]
		static unsafe void Trampoline_StateChangeCallback (IntPtr block, NWConnectionState state, IntPtr error)
		{
                        var descriptor = (BlockLiteral *) block;
                        var del = (Action<NWConnectionState,NWError>) (descriptor->Target);
                        if (del != null){
				NWError err = error != IntPtr.Zero ? new NWError (error, owns: false) : null;
                                del (state, err);
			}
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_connection_set_state_changed_handler (nw_connection_t connection, void *handler);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public unsafe void SetStateChangeHandler (Action<NWConnectionState,NWError> stateHandler)
		{
			if (stateHandler == null){
				nw_connection_set_state_changed_handler (handle, null);
				return;
			}
			
                        unsafe {
                                BlockLiteral *block_ptr_handler;
                                BlockLiteral block_handler;
                                block_handler = new BlockLiteral ();
                                block_ptr_handler = &block_handler;
                                block_handler.SetupBlockUnsafe (static_stateChangeHandler, stateHandler);

                                nw_connection_set_state_changed_handler (handle, (void*) block_ptr_handler);
                                block_ptr_handler->CleanupBlock ();
                        }
		}

		delegate void nw_connection_boolean_event_handler_t (IntPtr block, [MarshalAs(UnmanagedType.U1)] bool value);
		static nw_connection_boolean_event_handler_t static_BooleanChangeHandler = TrampolineBooleanChangeHandler;
		
		[MonoPInvokeCallback (typeof (nw_connection_boolean_event_handler_t))]
		static unsafe void TrampolineBooleanChangeHandler (IntPtr block, bool value)
		{
			var descriptor = (BlockLiteral *) block;
			var del = (Action<bool>) (descriptor->Target);
			if (del != null){
			        del (value);
			}
		}
		
		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern unsafe void nw_connection_set_viability_changed_handler  (IntPtr handle, void *callback);
		
		[TV (12,0), Mac (10,14), iOS (12,0)]
		public unsafe void SetBooleanChangeHandler (Action<bool> callback)
		{
			if (callback == null){
				nw_connection_set_viability_changed_handler  (handle, null);
				return;
			}
			unsafe {
			        BlockLiteral *block_ptr_handler;
			        BlockLiteral block_handler;
			        block_handler = new BlockLiteral ();
			        block_ptr_handler = &block_handler;
			        block_handler.SetupBlockUnsafe (static_BooleanChangeHandler, callback);
			
			        nw_connection_set_viability_changed_handler (handle, (void*) block_ptr_handler);
			        block_ptr_handler->CleanupBlock ();
			}
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern unsafe void nw_connection_set_better_path_available_handler (IntPtr handle, void *callback);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public unsafe void SetBetterPathAvailableHandler (Action<bool> callback)
		{
			if (callback == null){
				nw_connection_set_better_path_available_handler  (handle, null);
				return;
			}
			unsafe {
			        BlockLiteral *block_ptr_handler;
			        BlockLiteral block_handler;
			        block_handler = new BlockLiteral ();
			        block_ptr_handler = &block_handler;
			        block_handler.SetupBlockUnsafe (static_BooleanChangeHandler, callback);
			
			        nw_connection_set_better_path_available_handler (handle, (void*) block_ptr_handler);
			        block_ptr_handler->CleanupBlock ();
			}
		}
	}
}
