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
		Cancelled = 5,
	}

	//
	// Signature for a method invoked on data received, the "data" value can be null if the
	// receive context is complete an the callback is ony delivering the completed event,
	// or the connection encountered an error.    There are scenarios where the data will
	// be present, and *also* the error will be set, indicating that some data was
	// retrieved, before the error was raised.
	//
	public delegate void NWConnectionReceiveCompletion (IntPtr data, nuint dataSize, NWContentContext context, bool isComplete, NWError error);

	//
	// Signature for a method invoked on data received, same as NWConnectionReceiveCompletion,
	// but they receive DispatchData instead of data + dataSize
	//
	public delegate void NWConnectionReceiveDispatchDataCompletion (DispatchData data, NWContentContext context, bool isComplete, NWError error);

	[TV (12,0), Mac (10,14), iOS (12,0)]
	public class NWConnection : NativeObject {
		public NWConnection (IntPtr handle, bool owns) : base (handle, owns) {}

		[DllImport (Constants.NetworkLibrary)]
		static extern nw_connection_t nw_connection_create (nw_endpoint_t endpoint, nw_parameters_t parameters);

		public NWConnection (NWEndpoint endpoint, NWParameters parameters)
		{
			if (endpoint == null)
				throw new ArgumentNullException (nameof (endpoint));
			if (parameters == null)
				throw new ArgumentNullException (nameof (parameters));
			InitializeHandle (nw_connection_create (endpoint.Handle, parameters.Handle));
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern nw_endpoint_t nw_connection_copy_endpoint (nw_connection_t connection);

		public NWEndpoint Endpoint {
			get {
				var x = nw_connection_copy_endpoint (GetCheckedHandle ());
				if (x == IntPtr.Zero)
					return null;
				return new NWEndpoint (x, owns: true);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern nw_parameters_t nw_connection_copy_parameters (nw_connection_t connection);

		public NWParameters Parameters {
			get {
				var x = nw_connection_copy_parameters (GetCheckedHandle ());
				if (x == IntPtr.Zero)
					return null;
				return new NWParameters (x, owns: true);
			}
		}

		delegate void StateChangeCallback (IntPtr block, NWConnectionState state, IntPtr error);
		static StateChangeCallback static_stateChangeHandler = Trampoline_StateChangeCallback;

		[MonoPInvokeCallback (typeof (StateChangeCallback))]
		static void Trampoline_StateChangeCallback (IntPtr block, NWConnectionState state, IntPtr error)
		{
			var del = BlockLiteral.GetTarget<Action<NWConnectionState,NWError>> (block);
			if (del != null) {
				NWError err = error != IntPtr.Zero ? new NWError (error, owns: false) : null;
				del (state, err);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_connection_set_state_changed_handler (nw_connection_t connection, void *handler);

		[BindingImpl (BindingImplOptions.Optimizable)]
		public unsafe void SetStateChangeHandler (Action<NWConnectionState,NWError> stateHandler)
		{
			if (stateHandler == null) {
				nw_connection_set_state_changed_handler (GetCheckedHandle (), null);
				return;
			}

			unsafe {
				BlockLiteral block_handler = new BlockLiteral ();
				BlockLiteral *block_ptr_handler = &block_handler;
				block_handler.SetupBlockUnsafe (static_stateChangeHandler, stateHandler);

				nw_connection_set_state_changed_handler (GetCheckedHandle (), (void*) block_ptr_handler);
				block_handler.CleanupBlock ();
			}
		}

		delegate void nw_connection_boolean_event_handler_t (IntPtr block, [MarshalAs(UnmanagedType.U1)] bool value);
		static nw_connection_boolean_event_handler_t static_BooleanChangeHandler = TrampolineBooleanChangeHandler;

		[MonoPInvokeCallback (typeof (nw_connection_boolean_event_handler_t))]
		static void TrampolineBooleanChangeHandler (IntPtr block, bool value)
		{
			var del = BlockLiteral.GetTarget<Action<bool>> (block);
			if (del != null)
			        del (value);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern unsafe void nw_connection_set_viability_changed_handler  (IntPtr handle, void *callback);

		[BindingImpl (BindingImplOptions.Optimizable)]
		public unsafe void SetBooleanChangeHandler (Action<bool> callback)
		{
			if (callback == null) {
				nw_connection_set_viability_changed_handler (GetCheckedHandle (), null);
				return;
			}

			unsafe {
				BlockLiteral block_handler = new BlockLiteral ();
				BlockLiteral *block_ptr_handler = &block_handler;
				block_handler.SetupBlockUnsafe (static_BooleanChangeHandler, callback);

				nw_connection_set_viability_changed_handler (GetCheckedHandle (), (void*) block_ptr_handler);
				block_handler.CleanupBlock ();
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern unsafe void nw_connection_set_better_path_available_handler (IntPtr handle, void *callback);

		[BindingImpl (BindingImplOptions.Optimizable)]
		public unsafe void SetBetterPathAvailableHandler (Action<bool> callback)
		{
			if (callback == null) {
				nw_connection_set_better_path_available_handler (GetCheckedHandle (), null);
				return;
			}

			unsafe {
				BlockLiteral block_handler = new BlockLiteral ();
				BlockLiteral *block_ptr_handler = &block_handler;
				block_handler.SetupBlockUnsafe (static_BooleanChangeHandler, callback);

				nw_connection_set_better_path_available_handler (GetCheckedHandle (), (void*) block_ptr_handler);
				block_handler.CleanupBlock ();
			}
		}

		delegate void nw_connection_path_event_handler_t (IntPtr block, IntPtr path);
		static nw_connection_path_event_handler_t static_PathChanged = TrampolinePathChanged;

		[MonoPInvokeCallback (typeof (nw_connection_path_event_handler_t))]
		static void TrampolinePathChanged (IntPtr block, IntPtr path)
		{
			var del = BlockLiteral.GetTarget<Action<NWPath>> (block);
			if (del != null) {
				var x = new NWPath (path, owns: false);
				del (x);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern unsafe void nw_connection_set_path_changed_handler (IntPtr handle, void *callback);

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void SetPathChangedHandler (Action<NWPath> callback)
		{
			unsafe {
				BlockLiteral block_handler = new BlockLiteral ();
				BlockLiteral *block_ptr_handler = &block_handler;
				block_handler.SetupBlockUnsafe (static_PathChanged, callback);

				nw_connection_set_path_changed_handler (GetCheckedHandle (), (void*) block_ptr_handler);
				block_handler.CleanupBlock ();
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_connection_set_queue (IntPtr handle, IntPtr queue);

		public void SetQueue (DispatchQueue queue)
		{
			if (queue == null)
				throw new ArgumentNullException (nameof (queue));
			nw_connection_set_queue (GetCheckedHandle (), queue.handle);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_connection_start (IntPtr handle);

		public void Start () => nw_connection_start (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_connection_restart (IntPtr handle);

		public void Restart () => nw_connection_restart (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_connection_cancel (IntPtr handle);

		public void Cancel () => nw_connection_cancel (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_connection_force_cancel (IntPtr handle);

		public void ForceCancel () => nw_connection_force_cancel (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_connection_cancel_current_endpoint (IntPtr handle);

		public void CancelCurrentEndpoint () => nw_connection_cancel_current_endpoint (GetCheckedHandle ());

		delegate void nw_connection_receive_completion_t (IntPtr block,
								  IntPtr dispatchData,
								  IntPtr contentContext,
								  [MarshalAs (UnmanagedType.U1)] bool isComplete,
								  IntPtr error);

		static nw_connection_receive_completion_t static_ReceiveCompletion = TrampolineReceiveCompletion;
		static nw_connection_receive_completion_t static_ReceiveCompletionDispatchData = TrampolineReceiveCompletionData;

		[MonoPInvokeCallback (typeof (nw_connection_receive_completion_t))]
		static void TrampolineReceiveCompletion (IntPtr block, IntPtr dispatchDataPtr, IntPtr contentContext, bool isComplete, IntPtr error)
		{
			var del = BlockLiteral.GetTarget<NWConnectionReceiveCompletion> (block);
			if (del != null) {
				DispatchData dispatchData = null, dataCopy = null;
				IntPtr bufferAddress = IntPtr.Zero;
				nuint bufferSize = 0;

				if (dispatchDataPtr != IntPtr.Zero) {
					dispatchData = new DispatchData (dispatchDataPtr, owns: false);
					dataCopy = dispatchData.CreateMap (out bufferAddress, out bufferSize);
				}

				del (bufferAddress,
				     bufferSize,
				     contentContext == IntPtr.Zero ? null : new NWContentContext (contentContext, owns: false),
				     isComplete,
				     error == IntPtr.Zero ? null : new NWError (error, owns: false));

				if (dispatchData != null) {
					dataCopy.Dispose ();
					dispatchData.Dispose ();
				}
			}
		}

		[MonoPInvokeCallback (typeof (nw_connection_receive_completion_t))]
		static void TrampolineReceiveCompletionData (IntPtr block, IntPtr dispatchDataPtr, IntPtr contentContext, bool isComplete, IntPtr error)
		{
			var del = BlockLiteral.GetTarget<NWConnectionReceiveDispatchDataCompletion> (block);
			if (del != null) {
				DispatchData dispatchData = null;
				IntPtr bufferAddress = IntPtr.Zero;

				if (dispatchDataPtr != IntPtr.Zero)
					dispatchData = new DispatchData (dispatchDataPtr, owns: false);

				del (dispatchData,
				     contentContext == IntPtr.Zero ? null : new NWContentContext (contentContext, owns: false),
				     isComplete,
				     error == IntPtr.Zero ? null : new NWError (error, owns: false));

				if (dispatchData != null)
					dispatchData.Dispose ();
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern unsafe void nw_connection_receive (IntPtr handle, uint minimumIncompleteLength, uint maximumLength, void *callback);

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void Receive (uint minimumIncompleteLength, uint maximumLength, NWConnectionReceiveCompletion callback)
		{
			if (callback == null)
				throw new ArgumentNullException (nameof (callback));

			unsafe {
				BlockLiteral block_handler = new BlockLiteral ();
				BlockLiteral *block_ptr_handler = &block_handler;
				block_handler.SetupBlockUnsafe (static_ReceiveCompletion, callback);

				nw_connection_receive (GetCheckedHandle (), minimumIncompleteLength, maximumLength, (void*) block_ptr_handler);
				block_handler.CleanupBlock ();
			}
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void ReceiveData (uint minimumIncompleteLength, uint maximumLength, NWConnectionReceiveDispatchDataCompletion callback)
		{
			if (callback == null)
				throw new ArgumentNullException (nameof (callback));

			unsafe {
				BlockLiteral block_handler = new BlockLiteral ();
				BlockLiteral *block_ptr_handler = &block_handler;
				block_handler.SetupBlockUnsafe (static_ReceiveCompletionDispatchData, callback);

				nw_connection_receive (GetCheckedHandle (), minimumIncompleteLength, maximumLength, (void*) block_ptr_handler);
				block_handler.CleanupBlock ();
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern unsafe void nw_connection_receive_message (IntPtr handle, void *callback);

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void ReceiveMessage (NWConnectionReceiveCompletion callback)
		{
			if (callback == null)
				throw new ArgumentNullException (nameof (callback));

			unsafe {
				BlockLiteral block_handler = new BlockLiteral ();
				BlockLiteral *block_ptr_handler = &block_handler;
				block_handler.SetupBlockUnsafe (static_ReceiveCompletion, callback);

				nw_connection_receive_message (GetCheckedHandle (), (void*) block_ptr_handler);
				block_handler.CleanupBlock ();
			}
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void ReceiveMessageData (NWConnectionReceiveDispatchDataCompletion callback)
		{
			if (callback == null)
				throw new ArgumentNullException (nameof (callback));

			unsafe {
				BlockLiteral block_handler = new BlockLiteral ();
				BlockLiteral *block_ptr_handler = &block_handler;
				block_handler.SetupBlockUnsafe (static_ReceiveCompletionDispatchData, callback);

				nw_connection_receive_message (GetCheckedHandle (), (void*) block_ptr_handler);
				block_handler.CleanupBlock ();
			}
		}

		delegate void nw_connection_send_completion_t (IntPtr block, IntPtr error);
		static nw_connection_send_completion_t static_SendCompletion = TrampolineSendCompletion;

		[MonoPInvokeCallback (typeof (nw_connection_send_completion_t))]
		static void TrampolineSendCompletion (IntPtr block, IntPtr error)
		{
			var del = BlockLiteral.GetTarget<Action<NWError>> (block);
			if (del != null) {
				var err = error == IntPtr.Zero ? null : new NWError (error, owns: false);
				del (err);
				err?.Dispose ();
			}
		}

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
			nw_connection_send (handle: GetCheckedHandle (),
					    dispatchData: buffer.GetHandle (),
					    contentContext: contentContext,
					    isComplete: isComplete,
					    callback: callback);
		}

		public void Send (byte [] buffer, NWContentContext context, bool isComplete, Action<NWError> callback)
		{
			DispatchData d = null;
			if (buffer != null)
				d = DispatchData.FromByteBuffer (buffer);

			Send (d, context, isComplete, callback);
		}

		public void Send (byte [] buffer, int start, int length, NWContentContext context, bool isComplete, Action<NWError> callback)
		{
			DispatchData d = null;
			if (buffer != null)
				d = DispatchData.FromByteBuffer (buffer, start, length);

			Send (d, context, isComplete, callback);
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void Send (DispatchData buffer, NWContentContext context, bool isComplete, Action<NWError> callback)
		{
			if (context == null)
				throw new ArgumentNullException (nameof (context));
			if (callback == null)
				throw new ArgumentNullException (nameof (callback));

			unsafe {
				BlockLiteral block_handler = new BlockLiteral ();
				BlockLiteral *block_ptr_handler = &block_handler;
				block_handler.SetupBlockUnsafe (static_SendCompletion, callback);

				LowLevelSend (GetCheckedHandle (), buffer, context.Handle, isComplete, block_ptr_handler);
				block_handler.CleanupBlock ();
			}
		}

		static IntPtr _nw_connection_send_idempotent_content;

		// This is a special token handled by the library that configures the Send operation to be idempotent.
		static IntPtr NW_CONNECTION_SEND_IDEMPOTENT_CONTENT ()
		{
			if (_nw_connection_send_idempotent_content == IntPtr.Zero)
				_nw_connection_send_idempotent_content = Marshal.ReadIntPtr (Dlfcn.dlsym (Libraries.Network.Handle, "_nw_connection_send_idempotent_content"));
			return _nw_connection_send_idempotent_content;
		}

		public unsafe void SendIdempotent (DispatchData buffer, NWContentContext context, bool isComplete)
		{
			if (context == null)
				throw new ArgumentNullException (nameof (context));

			LowLevelSend (GetCheckedHandle (), buffer, context.Handle, isComplete, (void *) NW_CONNECTION_SEND_IDEMPOTENT_CONTENT ());
		}

		public void SendIdempotent (byte [] buffer, NWContentContext context, bool isComplete)
		{
			DispatchData d = null;
			if (buffer != null)
				d = DispatchData.FromByteBuffer (buffer);

			SendIdempotent (buffer, context, isComplete);
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static string nw_connection_copy_description (IntPtr handle);

		public string Description => nw_connection_copy_description (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_connection_copy_current_path (IntPtr handle);

		public NWPath CurrentPath {
			get {
				var x = nw_connection_copy_current_path (GetCheckedHandle ());
				if (x == IntPtr.Zero)
					return null;
				return new NWPath (x, owns: true);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_connection_copy_protocol_metadata (IntPtr handle, IntPtr protocolDefinition);

		public NWProtocolMetadata GetProtocolMetadata (NWProtocolDefinition definition)
		{
			if (definition == null)
				throw new ArgumentNullException (nameof (definition));

			var x = nw_connection_copy_protocol_metadata (GetCheckedHandle (), definition.Handle);
			if (x == IntPtr.Zero)
				return null;
			return new NWProtocolMetadata (x, owns: true);
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static /* uint32_t */ uint nw_connection_get_maximum_datagram_size (IntPtr handle);

		public uint MaximumDatagramSize => nw_connection_get_maximum_datagram_size (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_connection_batch (IntPtr handle, IntPtr callback_block);

		public void Batch (Action method)
		{
			BlockLiteral.SimpleCall (method, (arg)=> nw_connection_batch (GetCheckedHandle (), arg));
		}
	}
}
