//
// NWConnection.cs: Bindings for the Network NWConnection API.
//
// Authors:
//   Miguel de Icaza (miguel@microsoft.com)
//
// Copyrigh 2018 Microsoft Inc
//
#nullable enable

using System;
using System.Runtime.InteropServices;
using ObjCRuntime;
using Foundation;
using CoreFoundation;
using nw_connection_t = System.IntPtr;
using nw_endpoint_t = System.IntPtr;
using nw_parameters_t = System.IntPtr;
using nw_establishment_report_t = System.IntPtr;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Network {

	//
	// Signature for a method invoked on data received, the "data" value can be null if the
	// receive context is complete an the callback is ony delivering the completed event,
	// or the connection encountered an error.    There are scenarios where the data will
	// be present, and *also* the error will be set, indicating that some data was
	// retrieved, before the error was raised.
	//
	public delegate void NWConnectionReceiveCompletion (IntPtr data, nuint dataSize, NWContentContext? context, bool isComplete, NWError? error);

	//
	// Signature for a method invoked on data received, same as NWConnectionReceiveCompletion,
	// but they receive DispatchData instead of data + dataSize
	//
	public delegate void NWConnectionReceiveDispatchDataCompletion (DispatchData? data, NWContentContext? context, bool isComplete, NWError? error);

	//
	// Signature for a method invoked on data received, same as NWConnectionReceiveCompletion,
	// but they receive ReadOnlySpan rather than a data + dataSize
	//
	public delegate void NWConnectionReceiveReadOnlySpanCompletion (ReadOnlySpan<byte> data, NWContentContext? context, bool isComplete, NWError? error);

#if NET
	[SupportedOSPlatform ("tvos12.0")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios12.0")]
	[SupportedOSPlatform ("maccatalyst")]
#else
	[TV (12, 0)]
	[iOS (12, 0)]
	[Watch (6, 0)]
#endif
	public class NWConnection : NativeObject {
		[Preserve (Conditional = true)]
#if NET
		internal NWConnection (NativeHandle handle, bool owns) : base (handle, owns) {}
#else
		public NWConnection (NativeHandle handle, bool owns) : base (handle, owns) { }
#endif

		[DllImport (Constants.NetworkLibrary)]
		static extern nw_connection_t nw_connection_create (nw_endpoint_t endpoint, nw_parameters_t parameters);

		public NWConnection (NWEndpoint endpoint, NWParameters parameters)
		{
			if (endpoint is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (endpoint));
			if (parameters is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (parameters));
			InitializeHandle (nw_connection_create (endpoint.Handle, parameters.Handle));
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern nw_endpoint_t nw_connection_copy_endpoint (nw_connection_t connection);

		public NWEndpoint? Endpoint {
			get {
				var x = nw_connection_copy_endpoint (GetCheckedHandle ());
				if (x == IntPtr.Zero)
					return null;
				return new NWEndpoint (x, owns: true);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern nw_parameters_t nw_connection_copy_parameters (nw_connection_t connection);

		public NWParameters? Parameters {
			get {
				var x = nw_connection_copy_parameters (GetCheckedHandle ());
				if (x == IntPtr.Zero)
					return null;
				return new NWParameters (x, owns: true);
			}
		}

#if !NET
		delegate void StateChangeCallback (IntPtr block, NWConnectionState state, IntPtr error);
		static StateChangeCallback static_stateChangeHandler = Trampoline_StateChangeCallback;

		[MonoPInvokeCallback (typeof (StateChangeCallback))]
#else
		[UnmanagedCallersOnly]
#endif
		static void Trampoline_StateChangeCallback (IntPtr block, NWConnectionState state, IntPtr error)
		{
			var del = BlockLiteral.GetTarget<Action<NWConnectionState, NWError?>> (block);
			if (del is not null) {
				NWError? err = error != IntPtr.Zero ? new NWError (error, owns: false) : null;
				del (state, err);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_connection_set_state_changed_handler (nw_connection_t connection, void* handler);

		[BindingImpl (BindingImplOptions.Optimizable)]
		public unsafe void SetStateChangeHandler (Action<NWConnectionState, NWError?> stateHandler)
		{
			if (stateHandler is null) {
				nw_connection_set_state_changed_handler (GetCheckedHandle (), null);
				return;
			}

			unsafe {
#if NET
				delegate* unmanaged<IntPtr, NWConnectionState, IntPtr, void> trampoline = &Trampoline_StateChangeCallback;
				using var block = new BlockLiteral (trampoline, stateHandler, typeof (NWConnection), nameof (Trampoline_StateChangeCallback));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_stateChangeHandler, stateHandler);
#endif
				nw_connection_set_state_changed_handler (GetCheckedHandle (), &block);
			}
		}

#if !NET
		delegate void nw_connection_boolean_event_handler_t (IntPtr block, byte value);
		static nw_connection_boolean_event_handler_t static_BooleanChangeHandler = TrampolineBooleanChangeHandler;

		[MonoPInvokeCallback (typeof (nw_connection_boolean_event_handler_t))]
#else
		[UnmanagedCallersOnly]
#endif
		static void TrampolineBooleanChangeHandler (IntPtr block, byte value)
		{
			var del = BlockLiteral.GetTarget<Action<bool>> (block);
			if (del is not null)
				del (value != 0);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern unsafe void nw_connection_set_viability_changed_handler (IntPtr handle, void* callback);

		[BindingImpl (BindingImplOptions.Optimizable)]
		public unsafe void SetBooleanChangeHandler (Action<bool> callback)
		{
			if (callback is null) {
				nw_connection_set_viability_changed_handler (GetCheckedHandle (), null);
				return;
			}

			unsafe {
#if NET
				delegate* unmanaged<IntPtr, byte, void> trampoline = &TrampolineBooleanChangeHandler;
				using var block = new BlockLiteral (trampoline, callback, typeof (NWConnection), nameof (TrampolineBooleanChangeHandler));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_BooleanChangeHandler, callback);
#endif
				nw_connection_set_viability_changed_handler (GetCheckedHandle (), &block);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern unsafe void nw_connection_set_better_path_available_handler (IntPtr handle, void* callback);

		[BindingImpl (BindingImplOptions.Optimizable)]
		public unsafe void SetBetterPathAvailableHandler (Action<bool> callback)
		{
			if (callback is null) {
				nw_connection_set_better_path_available_handler (GetCheckedHandle (), null);
				return;
			}

			unsafe {
#if NET
				delegate* unmanaged<IntPtr, byte, void> trampoline = &TrampolineBooleanChangeHandler;
				using var block = new BlockLiteral (trampoline, callback, typeof (NWConnection), nameof (TrampolineBooleanChangeHandler));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_BooleanChangeHandler, callback);
#endif
				nw_connection_set_better_path_available_handler (GetCheckedHandle (), &block);
			}
		}

#if !NET
		delegate void nw_connection_path_event_handler_t (IntPtr block, IntPtr path);
		static nw_connection_path_event_handler_t static_PathChanged = TrampolinePathChanged;

		[MonoPInvokeCallback (typeof (nw_connection_path_event_handler_t))]
#else
		[UnmanagedCallersOnly]
#endif
		static void TrampolinePathChanged (IntPtr block, IntPtr path)
		{
			var del = BlockLiteral.GetTarget<Action<NWPath>> (block);
			if (del is not null) {
				var x = new NWPath (path, owns: false);
				del (x);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_connection_set_path_changed_handler (IntPtr handle, BlockLiteral* callback);

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void SetPathChangedHandler (Action<NWPath> callback)
		{
			unsafe {
#if NET
				delegate* unmanaged<IntPtr, IntPtr, void> trampoline = &TrampolinePathChanged;
				using var block = new BlockLiteral (trampoline, callback, typeof (NWConnection), nameof (TrampolinePathChanged));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_PathChanged, callback);
#endif
				nw_connection_set_path_changed_handler (GetCheckedHandle (), &block);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_connection_set_queue (IntPtr handle, IntPtr queue);

		public void SetQueue (DispatchQueue queue)
		{
			if (queue is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (queue));
			nw_connection_set_queue (GetCheckedHandle (), queue.Handle);
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

#if !NET
		delegate void nw_connection_receive_completion_t (IntPtr block,
								  IntPtr dispatchData,
								  IntPtr contentContext,
								  byte isComplete,
								  IntPtr error);

		static nw_connection_receive_completion_t static_ReceiveCompletion = TrampolineReceiveCompletion;
		static nw_connection_receive_completion_t static_ReceiveCompletionDispatchData = TrampolineReceiveCompletionData;
		static nw_connection_receive_completion_t static_ReceiveCompletionDispatchReadnOnlyData = TrampolineReceiveCompletionReadOnlyData;
#endif

#if !NET
		[MonoPInvokeCallback (typeof (nw_connection_receive_completion_t))]
#else
		[UnmanagedCallersOnly]
#endif
		static void TrampolineReceiveCompletion (IntPtr block, IntPtr dispatchDataPtr, IntPtr contentContext, byte isComplete, IntPtr error)
		{
			var del = BlockLiteral.GetTarget<NWConnectionReceiveCompletion> (block);
			if (del is not null) {
				DispatchData? dispatchData = null, dataCopy = null;
				IntPtr bufferAddress = IntPtr.Zero;
				nuint bufferSize = 0;

				if (dispatchDataPtr != IntPtr.Zero) {
					dispatchData = new DispatchData (dispatchDataPtr, owns: false);
					dataCopy = dispatchData.CreateMap (out bufferAddress, out bufferSize);
				}

				del (bufferAddress,
					 bufferSize,
					 contentContext == IntPtr.Zero ? null : new NWContentContext (contentContext, owns: false),
					 isComplete != 0,
					 error == IntPtr.Zero ? null : new NWError (error, owns: false));

				if (dispatchData is not null) {
					dataCopy?.Dispose ();
					dispatchData?.Dispose ();
				}
			}
		}

#if !NET
		[MonoPInvokeCallback (typeof (nw_connection_receive_completion_t))]
#else
		[UnmanagedCallersOnly]
#endif
		static void TrampolineReceiveCompletionData (IntPtr block, IntPtr dispatchDataPtr, IntPtr contentContext, byte isComplete, IntPtr error)
		{
			var del = BlockLiteral.GetTarget<NWConnectionReceiveDispatchDataCompletion> (block);
			if (del is not null) {
				DispatchData? dispatchData = null;
				IntPtr bufferAddress = IntPtr.Zero;

				if (dispatchDataPtr != IntPtr.Zero)
					dispatchData = new DispatchData (dispatchDataPtr, owns: false);

				del (dispatchData,
					 contentContext == IntPtr.Zero ? null : new NWContentContext (contentContext, owns: false),
					 isComplete != 0,
					 error == IntPtr.Zero ? null : new NWError (error, owns: false));

				if (dispatchData is not null)
					dispatchData.Dispose ();
			}
		}

#if !NET
		[MonoPInvokeCallback (typeof (nw_connection_receive_completion_t))]
#else
		[UnmanagedCallersOnly]
#endif
		static void TrampolineReceiveCompletionReadOnlyData (IntPtr block, IntPtr dispatchDataPtr, IntPtr contentContext, byte isComplete, IntPtr error)
		{
			var del = BlockLiteral.GetTarget<NWConnectionReceiveReadOnlySpanCompletion> (block);
			if (del is not null) {
				var dispatchData = (dispatchDataPtr != IntPtr.Zero) ? new DispatchData (dispatchDataPtr, owns: false) : null;

				var spanData = new ReadOnlySpan<byte> (dispatchData?.ToArray () ?? Array.Empty<byte> ());
				del (spanData,
					contentContext == IntPtr.Zero ? null : new NWContentContext (contentContext, owns: false),
					isComplete != 0,
					error == IntPtr.Zero ? null : new NWError (error, owns: false));

				if (dispatchData is not null) {
					dispatchData.Dispose ();
				}
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_connection_receive (IntPtr handle, /* uint32_t */ uint minimumIncompleteLength, /* uint32_t */ uint maximumLength, BlockLiteral* callback);

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void Receive (uint minimumIncompleteLength, uint maximumLength, NWConnectionReceiveCompletion callback)
		{
			if (callback is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (callback));

			unsafe {
#if NET
				delegate* unmanaged<IntPtr, IntPtr, IntPtr, byte, IntPtr, void> trampoline = &TrampolineReceiveCompletion;
				using var block = new BlockLiteral (trampoline, callback, typeof (NWConnection), nameof (TrampolineReceiveCompletion));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_ReceiveCompletion, callback);
#endif
				nw_connection_receive (GetCheckedHandle (), minimumIncompleteLength, maximumLength, &block);
			}
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void ReceiveData (uint minimumIncompleteLength, uint maximumLength, NWConnectionReceiveDispatchDataCompletion callback)
		{
			if (callback is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (callback));

			unsafe {
#if NET
				delegate* unmanaged<IntPtr, IntPtr, IntPtr, byte, IntPtr, void> trampoline = &TrampolineReceiveCompletionData;
				using var block = new BlockLiteral (trampoline, callback, typeof (NWConnection), nameof (TrampolineReceiveCompletionData));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_ReceiveCompletionDispatchData, callback);
#endif
				nw_connection_receive (GetCheckedHandle (), minimumIncompleteLength, maximumLength, &block);
			}
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void ReceiveReadOnlyData (uint minimumIncompleteLength, uint maximumLength, NWConnectionReceiveReadOnlySpanCompletion callback)
		{
			if (callback is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (callback));

			unsafe {
#if NET
				delegate* unmanaged<IntPtr, IntPtr, IntPtr, byte, IntPtr, void> trampoline = &TrampolineReceiveCompletionReadOnlyData;
				using var block = new BlockLiteral (trampoline, callback, typeof (NWConnection), nameof (TrampolineReceiveCompletionReadOnlyData));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_ReceiveCompletionDispatchReadnOnlyData, callback);
#endif
				nw_connection_receive (GetCheckedHandle (), minimumIncompleteLength, maximumLength, &block);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_connection_receive_message (IntPtr handle, BlockLiteral* callback);

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void ReceiveMessage (NWConnectionReceiveCompletion callback)
		{
			if (callback is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (callback));

			unsafe {
#if NET
				delegate* unmanaged<IntPtr, IntPtr, IntPtr, byte, IntPtr, void> trampoline = &TrampolineReceiveCompletion;
				using var block = new BlockLiteral (trampoline, callback, typeof (NWConnection), nameof (TrampolineReceiveCompletion));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_ReceiveCompletion, callback);
#endif
				nw_connection_receive_message (GetCheckedHandle (), &block);
			}

		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void ReceiveMessageData (NWConnectionReceiveDispatchDataCompletion callback)
		{
			if (callback is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (callback));

			unsafe {
#if NET
				delegate* unmanaged<IntPtr, IntPtr, IntPtr, byte, IntPtr, void> trampoline = &TrampolineReceiveCompletionData;
				using var block = new BlockLiteral (trampoline, callback, typeof (NWConnection), nameof (TrampolineReceiveCompletionData));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_ReceiveCompletionDispatchData, callback);
#endif
				nw_connection_receive_message (GetCheckedHandle (), &block);
			}
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void ReceiveMessageReadOnlyData (NWConnectionReceiveReadOnlySpanCompletion callback)
		{
			if (callback is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (callback));

			unsafe {
#if NET
				delegate* unmanaged<IntPtr, IntPtr, IntPtr, byte, IntPtr, void> trampoline = &TrampolineReceiveCompletionReadOnlyData;
				using var block = new BlockLiteral (trampoline, callback, typeof (NWConnection), nameof (TrampolineReceiveCompletionReadOnlyData));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_ReceiveCompletionDispatchReadnOnlyData, callback);
#endif
				nw_connection_receive_message (GetCheckedHandle (), &block);
			}
		}

#if !NET
		delegate void nw_connection_send_completion_t (IntPtr block, IntPtr error);
		static nw_connection_send_completion_t static_SendCompletion = TrampolineSendCompletion;

		[MonoPInvokeCallback (typeof (nw_connection_send_completion_t))]
#else
		[UnmanagedCallersOnly]
#endif
		static void TrampolineSendCompletion (IntPtr block, IntPtr error)
		{
			var del = BlockLiteral.GetTarget<Action<NWError?>> (block);
			if (del is not null) {
				var err = error == IntPtr.Zero ? null : new NWError (error, owns: false);
				del (err);
				err?.Dispose ();
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern unsafe void nw_connection_send (IntPtr handle,
								  IntPtr dispatchData,
								  IntPtr contentContext,
								  [MarshalAs (UnmanagedType.U1)] bool isComplete,
								  BlockLiteral* callback);

		//
		// This has more uses than the current ones, we might want to introduce
		// additional SendXxx methods to encode the few options that are currently
		// configured via one of the three NWContentContext static properties
		//
		unsafe void LowLevelSend (IntPtr handle, DispatchData? buffer, IntPtr contentContext, bool isComplete, BlockLiteral* callback)
		{
			nw_connection_send (handle: GetCheckedHandle (),
						dispatchData: buffer.GetHandle (),
						contentContext: contentContext,
						isComplete: isComplete,
						callback: callback);
		}

		public void Send (byte [] buffer, NWContentContext context, bool isComplete, Action<NWError?> callback)
		{
			DispatchData? d = null;
			if (buffer is not null)
				d = DispatchData.FromByteBuffer (buffer);

			Send (d, context, isComplete, callback);
		}

		public void Send (byte [] buffer, int start, int length, NWContentContext context, bool isComplete, Action<NWError?> callback)
		{
			DispatchData? d = null;
			if (buffer is not null)
				d = DispatchData.FromByteBuffer (buffer, start, length);

			Send (d, context, isComplete, callback);
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void Send (DispatchData? buffer, NWContentContext context, bool isComplete, Action<NWError?> callback)
		{
			if (context is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (context));
			if (callback is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (callback));

			unsafe {
#if NET
				delegate* unmanaged<IntPtr, IntPtr, void> trampoline = &TrampolineSendCompletion;
				using var block = new BlockLiteral (trampoline, callback, typeof (NWConnection), nameof (TrampolineSendCompletion));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_SendCompletion, callback);
#endif
				LowLevelSend (GetCheckedHandle (), buffer, context.Handle, isComplete, &block);
			}
		}

		public unsafe void SendIdempotent (DispatchData? buffer, NWContentContext context, bool isComplete)
		{
			if (context is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (context));

			LowLevelSend (GetCheckedHandle (), buffer, context.Handle, isComplete, (BlockLiteral*) NWConnectionConstants._SendIdempotentContent);
		}

		public void SendIdempotent (byte [] buffer, NWContentContext context, bool isComplete)
		{
			DispatchData? d = null;
			if (buffer is not null)
				d = DispatchData.FromByteBuffer (buffer);

			SendIdempotent (d, context, isComplete);
		}

		[DllImport (Constants.NetworkLibrary, EntryPoint = "nw_connection_copy_description")]
		extern static IntPtr nw_connection_copy_description_ptr (IntPtr handle);

		static string nw_connection_copy_description (IntPtr handle)
		{
			var ptr = nw_connection_copy_description_ptr (handle);
			return TransientString.ToStringAndFree (ptr)!;
		}

		public string Description => nw_connection_copy_description (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_connection_copy_current_path (IntPtr handle);

		public NWPath? CurrentPath {
			get {
				var x = nw_connection_copy_current_path (GetCheckedHandle ());
				if (x == IntPtr.Zero)
					return null;
				return new NWPath (x, owns: true);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_connection_copy_protocol_metadata (IntPtr handle, IntPtr protocolDefinition);

		public NWProtocolMetadata? GetProtocolMetadata (NWProtocolDefinition definition)
		{
			if (definition is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (definition));

			var x = nw_connection_copy_protocol_metadata (GetCheckedHandle (), definition.Handle);
			if (x == IntPtr.Zero)
				return null;
			return new NWProtocolMetadata (x, owns: true);
		}

		public T? GetProtocolMetadata<T> (NWProtocolDefinition definition) where T : NWProtocolMetadata
		{
			if (definition is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (definition));

			var x = nw_connection_copy_protocol_metadata (GetCheckedHandle (), definition.Handle);
			return Runtime.GetINativeObject<T> (x, owns: true);
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static /* uint32_t */ uint nw_connection_get_maximum_datagram_size (IntPtr handle);

		public uint MaximumDatagramSize => nw_connection_get_maximum_datagram_size (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		unsafe extern static void nw_connection_batch (IntPtr handle, BlockLiteral* callback_block);

		public void Batch (Action method)
		{
			unsafe {
				using var block = BlockStaticDispatchClass.CreateBlock (method);
				nw_connection_batch (GetCheckedHandle (), &block);
			}
		}

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[TV (13, 0)]
		[iOS (13, 0)]
#endif
		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_connection_access_establishment_report (IntPtr connection, IntPtr queue, BlockLiteral* access_block);

#if !NET
		delegate void nw_establishment_report_access_block_t (IntPtr block, nw_establishment_report_t report);
		static nw_establishment_report_access_block_t static_GetEstablishmentReportHandler = TrampolineGetEstablishmentReportHandler;

		[MonoPInvokeCallback (typeof (nw_establishment_report_access_block_t))]
#else
		[UnmanagedCallersOnly]
#endif
		static void TrampolineGetEstablishmentReportHandler (IntPtr block, nw_establishment_report_t report)
		{
			var del = BlockLiteral.GetTarget<Action<NWEstablishmentReport>> (block);
			if (del is not null) {
				// the ownerthip of the object is for the caller
				var nwReport = new NWEstablishmentReport (report, owns: true);
				del (nwReport);
			}
		}

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[TV (13, 0)]
		[iOS (13, 0)]
#endif
		[BindingImpl (BindingImplOptions.Optimizable)]
		public void GetEstablishmentReport (DispatchQueue queue, Action<NWEstablishmentReport> handler)
		{
			if (queue is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (queue));
			if (handler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));

			unsafe {
#if NET
				delegate* unmanaged<IntPtr, nw_establishment_report_t, void> trampoline = &TrampolineGetEstablishmentReportHandler;
				using var block = new BlockLiteral (trampoline, handler, typeof (NWConnection), nameof (TrampolineGetEstablishmentReportHandler));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_GetEstablishmentReportHandler, handler);
#endif
				nw_connection_access_establishment_report (GetCheckedHandle (), queue.Handle, &block);
			}
		}
	}
}
