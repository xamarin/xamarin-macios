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

	//
	// Signature for a method invoked on data received, the "data" value can be null if the
	// receive context is complete an the callback is ony delivering the completed event,
	// or the connection encountered an error.    There are scenarios where the data will
	// be present, and *also* the error will be set, indicating that some data was
	// retrieved, before the error was raised.
	//
	public delegate void NWConnectionReceiveCompletion (IntPtr data, ulong dataSize, NWContentContext context, bool isComplete, NWError error);
	
	public class NWConnection : NativeObject {
		public NWConnection (IntPtr handle, bool owns) : base (handle, owns) {} 

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
		
		delegate void nw_connection_path_event_handler_t (IntPtr block, IntPtr path);
		static nw_connection_path_event_handler_t static_PathChanged = TrampolinePathChanged;
		
		[MonoPInvokeCallback (typeof (nw_connection_path_event_handler_t))]
		static unsafe void TrampolinePathChanged (IntPtr block, IntPtr path)
		{
			var descriptor = (BlockLiteral *) block;
			var del = (Action<NWPath>) (descriptor->Target);
			if (del != null){
				var x = new NWPath (path, owns: false);
			        del (x);
				x.Dispose ();
			}
		}
		
		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern unsafe void nw_connection_set_path_changed_handler (IntPtr handle, void *callback);
		
		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void SetPathChangedHandler (Action<NWPath> callback)
		{
			unsafe {
			        BlockLiteral *block_ptr_handler;
			        BlockLiteral block_handler;
			        block_handler = new BlockLiteral ();
			        block_ptr_handler = &block_handler;
			        block_handler.SetupBlockUnsafe (static_PathChanged, callback);
			
			        nw_connection_set_path_changed_handler (handle, (void*) block_ptr_handler);
			        block_ptr_handler->CleanupBlock ();
			}
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_connection_set_queue (IntPtr handle, IntPtr queue);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void SetQueue (DispatchQueue queue)
		{
			if (queue == null)
				throw new ArgumentNullException (nameof (queue));
			nw_connection_set_queue (handle, queue.handle);
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_connection_start (IntPtr handle);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void Start () => nw_connection_start (handle);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_connection_restart (IntPtr handle);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void Restart () => nw_connection_restart (handle);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_connection_cancel (IntPtr handle);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void Cancel () => nw_connection_cancel (handle);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_connection_force_cancel (IntPtr handle);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void ForceCancel () => nw_connection_force_cancel (handle);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_connection_cancel_current_endpoint (IntPtr handle);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void CancelCurrentEndpoint () => nw_connection_cancel_current_endpoint (handle);

		delegate void nw_connection_receive_completion_t (IntPtr block,
								  IntPtr dispatchData,
								  IntPtr contentContext,
								  [MarshalAs (UnmanagedType.U1)] bool isComplete,
								  IntPtr error);
								  
		static nw_connection_receive_completion_t static_ReceiveCompletion = TrampolineReceiveCompletion;
		
		[MonoPInvokeCallback (typeof (nw_connection_receive_completion_t))]
		static unsafe void TrampolineReceiveCompletion (IntPtr block, IntPtr dispatchDataPtr, IntPtr contentContext, bool isComplete, IntPtr error)
		{
			var descriptor = (BlockLiteral *) block;
			var del = (NWConnectionReceiveCompletion) (descriptor->Target);
			if (del != null){
				DispatchData dispatchData = null, dataCopy = null;
				IntPtr bufferAddress = IntPtr.Zero;
				ulong bufferSize = 0;
				
				if (dispatchDataPtr != null){
					dispatchData = new DispatchData (dispatchDataPtr, owns: false);
					dataCopy =  dispatchData.CreateMap (out bufferAddress, out bufferSize);
				}

				del (bufferAddress,
				     bufferSize,
				     contentContext == IntPtr.Zero ? null : new NWContentContext (contentContext, owns: false),
				     isComplete,
				     error == IntPtr.Zero ? null : new NWError (error, owns: false));
				
				if (dispatchData != null){
					dataCopy.Dispose ();
					dispatchData.Dispose ();
				}
			}
		}
		
		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern unsafe void nw_connection_receive (IntPtr handle, uint minimumIncompleteLength, uint maximumLength, void *callback);
		
		public void Receive (uint minimumIncompleteLength, uint maximumLength, NWConnectionReceiveCompletion callback)
		{
			unsafe {
			        BlockLiteral *block_ptr_handler;
			        BlockLiteral block_handler;
			        block_handler = new BlockLiteral ();
			        block_ptr_handler = &block_handler;
			        block_handler.SetupBlockUnsafe (static_ReceiveCompletion, callback);
			
			        nw_connection_receive (handle, minimumIncompleteLength, maximumLength, (void*) block_ptr_handler);
			        block_ptr_handler->CleanupBlock ();
			}
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern unsafe void nw_connection_receive_message (IntPtr handle, void *callback);
		
		public void ReceiveMessage (NWConnectionReceiveCompletion callback)
		{
			unsafe {
			        BlockLiteral *block_ptr_handler;
			        BlockLiteral block_handler;
			        block_handler = new BlockLiteral ();
			        block_ptr_handler = &block_handler;
			        block_handler.SetupBlockUnsafe (static_ReceiveCompletion, callback);
			
			        nw_connection_receive_message (handle, (void*) block_ptr_handler);
			        block_ptr_handler->CleanupBlock ();
			}
		}


		delegate void nw_connection_send_completion_t (IntPtr block, IntPtr error);
		static nw_connection_send_completion_t static_SendCompletion = TrampolineSendCompletion;
		
		[MonoPInvokeCallback (typeof (nw_connection_send_completion_t))]
		static unsafe void TrampolineSendCompletion (IntPtr block, IntPtr error)
		{
			var descriptor = (BlockLiteral *) block;
			var del = (Action<NWError>) (descriptor->Target);
			if (del != null){
				var err = error == IntPtr.Zero ? null : new NWError (error, owns: false);
			        del (err);
				err?.Dispose ();
			}
		}
		
		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern unsafe void nw_connection_send (IntPtr handle,
							      IntPtr dispatchData,
							      IntPtr contentContext,
							      [MarshalAs(UnmanagedType.U1)] bool isComplete,
							      void *callback);

		//
		// This has more uses than the current ones, we might want to introduce
		// additional SendXxx methods to encode the few options that are currently
		// configured via one of the three NWContentContext static properties
		//
		unsafe void LowLevelSend (IntPtr handle, DispatchData buffer, IntPtr contentContext, bool isComplete, void *callback)
		{
			nw_connection_send (handle: handle,
					    dispatchData: buffer == null ? IntPtr.Zero : buffer.Handle,
					    contentContext: contentContext,
					    isComplete: isComplete,
					    callback: callback);
					    
		}
		
		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void Send (byte [] buffer, NWContentContext context, bool isComplete, Action<NWError> callback)
		{
			DispatchData d = null;
			if (buffer != null)
				d = DispatchData.FromByteBuffer (buffer);

			Send (d, context, isComplete, callback);
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void Send (byte [] buffer, int start, int length, NWContentContext context, bool isComplete, Action<NWError> callback)
		{
			DispatchData d = null;
			if (buffer != null)
				d = DispatchData.FromByteBuffer (buffer, start, length);

			Send (d, context, isComplete, callback);
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void Send (DispatchData buffer, NWContentContext context, bool isComplete, Action<NWError> callback)
		{
			if (context == null)
				throw new ArgumentNullException (nameof (context));
			if (callback == null)
				throw new ArgumentNullException (nameof (callback));
			
			unsafe {
			        BlockLiteral *block_ptr_handler;
			        BlockLiteral block_handler;
			        block_handler = new BlockLiteral ();
			        block_ptr_handler = &block_handler;
			        block_handler.SetupBlockUnsafe (static_SendCompletion, callback);

				LowLevelSend (handle, buffer, context.Handle, isComplete, block_ptr_handler);
			        block_ptr_handler->CleanupBlock ();
			}
		}

		static IntPtr _nw_connection_send_idempotent_content;

		// This is a special token handled by the library that configures the Send operation to be idempotent.
		static IntPtr NW_CONNECTION_SEND_IDEMPOTENT_CONTENT ()
		{
			if (_nw_connection_send_idempotent_content == IntPtr.Zero)
				_nw_connection_send_idempotent_content = Dlfcn.dlsym (Libraries.Network.Handle, "_nw_connection_send_idempotent_content");
			return _nw_connection_send_idempotent_content;
		}
		
		[TV (12,0), Mac (10,14), iOS (12,0)]
		public unsafe void SendIdempotent (DispatchData buffer, NWContentContext context, bool isComplete)
		{
			if (context == null)
				throw new ArgumentNullException (nameof (context));

			LowLevelSend (handle, buffer, context.Handle, isComplete, (void *) NW_CONNECTION_SEND_IDEMPOTENT_CONTENT ());
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void SendIdempotent (byte [] buffer, NWContentContext context, bool isComplete)
		{
			DispatchData d = null;
			if (buffer != null)
				d = DispatchData.FromByteBuffer (buffer);

			SendIdempotent (buffer, context, isComplete);
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static string nw_connection_copy_description (IntPtr handle);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public string Description => nw_connection_copy_description (handle);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_connection_copy_current_path (IntPtr handle);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public NWPath CurrentPath {
			get {
				var x = nw_connection_copy_current_path (handle);
				if (x == IntPtr.Zero)
					return null;
				return new NWPath (x, owns: true);
			}
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_connection_copy_protocol_metadata (IntPtr handle, IntPtr protocolDefinition);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public NWProtocolMetadata GetProtocolMetadata (NWProtocolDefinition definition)
		{
			if (definition == null)
				throw new ArgumentNullException (nameof (definition));
			
			var x = nw_connection_copy_protocol_metadata (handle, definition.handle);
			if (x == IntPtr.Zero)
				return null;
			return new NWProtocolMetadata (x, owns: true);
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static uint nw_connection_get_maximum_datagram_size (IntPtr handle);

		public uint MaximumDatagramSize => nw_connection_get_maximum_datagram_size (handle);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_connection_batch (IntPtr handle, IntPtr callback_block);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void Batch (Action method)
		{
			BlockLiteral.SimpleCall (method, (arg)=> nw_connection_batch (handle, arg));
		}
	}
}
