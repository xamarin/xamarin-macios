using System;
using System.Runtime.InteropServices;
using ObjCRuntime;
using Foundation;
using CoreFoundation;
using OS_nw_connection = System.IntPtr;
using OS_nw_connection_group = System.IntPtr;
using OS_nw_group_descriptor = System.IntPtr;
using OS_nw_parameters = System.IntPtr;
using OS_nw_content_context = System.IntPtr;
using OS_nw_path = System.IntPtr;
using OS_nw_endpoint = System.IntPtr;
using OS_nw_protocol_metadata = System.IntPtr;
using OS_nw_protocol_definition = System.IntPtr;
using OS_nw_protocol_options = System.IntPtr;

#if !NET
using NativeHandle = System.IntPtr;
#endif

#nullable enable

namespace Network {

#if NET
	// [SupportedOSPlatform ("tvos14.0")]  -  Not valid on Delegates
	// [SupportedOSPlatform ("macos11.0")]
	// [SupportedOSPlatform ("ios14.0")]
	// [SupportedOSPlatform ("maccatalyst14.0")]
#else
	[TV (14, 0)]
	[Mac (11, 0)]
	[iOS (14, 0)]
	[Watch (7, 0)]
	[MacCatalyst (14, 0)]
#endif
	public delegate void NWConnectionGroupReceiveDelegate (DispatchData content, NWContentContext context, bool isCompleted);

#if NET
	// [SupportedOSPlatform ("tvos14.0")]  -  Not valid on Delegates
	// [SupportedOSPlatform ("macos11.0")]
	// [SupportedOSPlatform ("ios14.0")]
	// [SupportedOSPlatform ("maccatalyst14.0")]
#else
	[TV (14, 0)]
	[Mac (11, 0)]
	[iOS (14, 0)]
	[Watch (7, 0)]
	[MacCatalyst (14, 0)]
#endif
	public delegate void NWConnectionGroupStateChangedDelegate (NWConnectionGroupState state, NWError? error);

#if NET
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("macos11.0")]
	[SupportedOSPlatform ("ios14.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[TV (14, 0)]
	[Mac (11, 0)]
	[iOS (14, 0)]
	[Watch (7, 0)]
	[MacCatalyst (14, 0)]
#endif
	public class NWConnectionGroup : NativeObject {
		[Preserve (Conditional = true)]
		protected internal NWConnectionGroup (NativeHandle handle, bool owns) : base (handle, owns) { }

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_connection_group nw_connection_group_create (OS_nw_group_descriptor group_descriptor, OS_nw_parameters parameters);

		public NWConnectionGroup (NWMulticastGroup groupDescriptor, NWParameters parameters)
		{
			if (groupDescriptor is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (groupDescriptor));
			if (parameters is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (parameters));

			InitializeHandle (nw_connection_group_create (groupDescriptor.GetCheckedHandle (), parameters.GetCheckedHandle ()));
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_group_descriptor nw_connection_group_copy_descriptor (OS_nw_connection_group group);

		public NWMulticastGroup? GroupDescriptor {
			get {
				var x = nw_connection_group_copy_descriptor (GetCheckedHandle ());
				if (x == IntPtr.Zero)
					return null;
				return new NWMulticastGroup (x, owns: true);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_parameters nw_connection_group_copy_parameters (OS_nw_connection_group group);

		public NWParameters? Parameters {
			get {
				var x = nw_connection_group_copy_parameters (GetCheckedHandle ());
				if (x == IntPtr.Zero)
					return null;
				return new NWParameters (x, owns: true);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_connection_group_start (OS_nw_connection_group group);

		public void Start () => nw_connection_group_start (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_connection_group_cancel (OS_nw_connection_group group);

		public void Cancel () => nw_connection_group_cancel (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_connection_group_set_queue (OS_nw_connection_group group, IntPtr queue);

		public void SetQueue (DispatchQueue queue)
		{
			if (queue is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (queue));

			nw_connection_group_set_queue (GetCheckedHandle (), queue.GetCheckedHandle ());
		}

		// can return null
		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_endpoint nw_connection_group_copy_local_endpoint_for_message (OS_nw_connection_group group, OS_nw_content_context context);

		public NWEndpoint? GetLocalEndpoint (NWContentContext context)
		{
			if (context is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (context));
			var ptr = nw_connection_group_copy_local_endpoint_for_message (GetCheckedHandle (), context.GetCheckedHandle ());
			return ptr == IntPtr.Zero ? null : new NWEndpoint (ptr, owns: true);
		}

		// can return null
		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_path nw_connection_group_copy_path_for_message (OS_nw_connection_group group, OS_nw_content_context context);

		public NWPath? GetPath (NWContentContext context)
		{
			if (context is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (context));
			var ptr = nw_connection_group_copy_path_for_message (GetCheckedHandle (), context.GetCheckedHandle ());
			return ptr == IntPtr.Zero ? null : new NWPath (ptr, owns: true);
		}

		// can return null
		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_endpoint nw_connection_group_copy_remote_endpoint_for_message (OS_nw_connection_group group, OS_nw_content_context context);

		public NWEndpoint? GetRemmoteEndpoint (NWContentContext context)
		{
			if (context is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (context));
			var ptr = nw_connection_group_copy_remote_endpoint_for_message (GetCheckedHandle (), context.GetCheckedHandle ());
			return ptr == IntPtr.Zero ? null : new NWEndpoint (ptr, owns: true);
		}

		// can return null
		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_connection nw_connection_group_extract_connection_for_message (OS_nw_connection_group group, OS_nw_content_context context);

		public NWConnection? GetConnection (NWContentContext context)
		{
			if (context is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (context));
			var ptr = nw_connection_group_extract_connection_for_message (GetCheckedHandle (), context.GetCheckedHandle ());
			return ptr == IntPtr.Zero ? null : new NWConnection (ptr, owns: true);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_connection_group_reply (OS_nw_connection_group group, OS_nw_content_context inbound_message, OS_nw_content_context outbound_message, /* [NullAllowed]  DispatchData */ IntPtr content);

		public void Reply (NWContentContext inboundMessage, NWContentContext outboundMessage, DispatchData content)
		{
			if (inboundMessage is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (inboundMessage));
			if (outboundMessage is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (outboundMessage));

			nw_connection_group_reply (GetCheckedHandle (), inboundMessage.GetCheckedHandle (), outboundMessage.GetCheckedHandle (), content.GetHandle ());
		}

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_connection_group_send_message (OS_nw_connection_group group, /* [NullAllowed] DispatchData */ IntPtr content, /* [NullAllowed] */ OS_nw_endpoint endpoint, OS_nw_content_context context, BlockLiteral* handler);

#if !NET
		delegate void nw_connection_group_send_completion_t (IntPtr block, IntPtr error);
		static nw_connection_group_send_completion_t static_SendCompletion = TrampolineSendCompletion;

		[MonoPInvokeCallback (typeof (nw_connection_group_send_completion_t))]
#else
		[UnmanagedCallersOnly]
#endif
		static void TrampolineSendCompletion (IntPtr block, IntPtr error)
		{
			var del = BlockLiteral.GetTarget<Action<NWError?>> (block);
			if (del is not null) {
				using var err = error == IntPtr.Zero ? null : new NWError (error, owns: false);
				del (err);
			}
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void Send (DispatchData? content, NWEndpoint? endpoint, NWContentContext context, Action<NWError?>? handler)
		{
			unsafe {
				if (handler is null) {
					nw_connection_group_send_message (GetCheckedHandle (),
						content.GetHandle (),
						endpoint.GetHandle (),
						context.GetCheckedHandle (),
						null);
					return;
				}

#if NET
				delegate* unmanaged<IntPtr, IntPtr, void> trampoline = &TrampolineSendCompletion;
				using var block = new BlockLiteral (trampoline, handler, typeof (NWConnectionGroup), nameof (TrampolineSendCompletion));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_SendCompletion, handler);
#endif
				nw_connection_group_send_message (GetCheckedHandle (),
					content.GetHandle (),
					endpoint.GetHandle (),
					context.GetCheckedHandle (),
					&block);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_connection_group_set_receive_handler (OS_nw_connection_group group, uint maximum_message_size, [MarshalAs (UnmanagedType.I1)] bool reject_oversized_messages, BlockLiteral* handler);

#if !NET
		delegate void nw_connection_group_receive_handler_t (IntPtr block, IntPtr content, IntPtr context, byte isCompleted);
		static nw_connection_group_receive_handler_t static_ReceiveHandler = TrampolineReceiveHandler;

		[MonoPInvokeCallback (typeof (nw_connection_group_receive_handler_t))]
#else
		[UnmanagedCallersOnly]
#endif
		static void TrampolineReceiveHandler (IntPtr block, IntPtr content, IntPtr context, byte isCompleted)
		{
			var del = BlockLiteral.GetTarget<NWConnectionGroupReceiveDelegate> (block);
			if (del is not null) {
				using var nsContent = new DispatchData (content, owns: false);
				using var nsContext = new NWContentContext (context, owns: false);
				del (nsContent, nsContext, isCompleted != 0);
			}
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void SetReceiveHandler (uint maximumMessageSize, bool rejectOversizedMessages, NWConnectionGroupReceiveDelegate? handler)
		{
			unsafe {
				if (handler is null) {
					nw_connection_group_set_receive_handler (GetCheckedHandle (), maximumMessageSize, rejectOversizedMessages, null);
					return;
				}

#if NET
				delegate* unmanaged<IntPtr, IntPtr, IntPtr, byte, void> trampoline = &TrampolineReceiveHandler;
				using var block = new BlockLiteral (trampoline, handler, typeof (NWConnectionGroup), nameof (TrampolineReceiveHandler));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_ReceiveHandler, handler);
#endif
				nw_connection_group_set_receive_handler (GetCheckedHandle (), maximumMessageSize, rejectOversizedMessages, &block);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_connection_group_set_state_changed_handler (OS_nw_connection_group group, BlockLiteral* handler);

#if !NET
		delegate void nw_connection_group_state_changed_handler_t (IntPtr block, NWConnectionGroupState state, IntPtr error);
		static nw_connection_group_state_changed_handler_t static_StateChangedHandler = TrampolineStateChangedHandler;

		[MonoPInvokeCallback (typeof (nw_connection_group_state_changed_handler_t))]
#else
		[UnmanagedCallersOnly]
#endif
		static void TrampolineStateChangedHandler (IntPtr block, NWConnectionGroupState state, IntPtr error)
		{
			var del = BlockLiteral.GetTarget<NWConnectionGroupStateChangedDelegate> (block);
			if (del is not null) {
				using var nwError = (error == IntPtr.Zero) ? null : new NWError (error, owns: false);
				del (state, nwError);
			}
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void SetStateChangedHandler (NWConnectionGroupStateChangedDelegate handler)
		{
			unsafe {
				if (handler is null) {
					nw_connection_group_set_state_changed_handler (GetCheckedHandle (), null);
					return;
				}

#if NET
				delegate* unmanaged<IntPtr, NWConnectionGroupState, IntPtr, void> trampoline=  &TrampolineStateChangedHandler;
				using var block = new BlockLiteral (trampoline, handler, typeof (NWConnectionGroup), nameof (TrampolineStateChangedHandler));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_StateChangedHandler, handler);
#endif
				nw_connection_group_set_state_changed_handler (GetCheckedHandle (), &block);
			}
		}

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos12.0")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[Mac (12, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_protocol_metadata nw_connection_group_copy_protocol_metadata (OS_nw_connection_group group, OS_nw_protocol_definition definition);

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos12.0")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[Mac (12, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
		public NWProtocolMetadata? GetProtocolMetadata (NWContentContext context)
		{
			if (context is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (context));
			var ptr = nw_connection_group_copy_protocol_metadata (GetCheckedHandle (), context.Handle);
			return ptr == IntPtr.Zero ? null : new NWProtocolMetadata (ptr, true);
		}

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos12.0")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[Mac (12, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_protocol_metadata nw_connection_group_copy_protocol_metadata_for_message (OS_nw_connection_group group, OS_nw_content_context context, OS_nw_protocol_definition definition);

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos12.0")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[Mac (12, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
		public NWProtocolMetadata? GetProtocolMetadata (NWContentContext context, NWProtocolDefinition definition)
		{
			if (context is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (context));
			if (definition is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (context));
			var ptr = nw_connection_group_copy_protocol_metadata_for_message (GetCheckedHandle (), context.Handle, definition.Handle);
			return ptr == IntPtr.Zero ? null : new NWProtocolMetadata (ptr, true);
		}

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos12.0")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[Mac (12, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_connection nw_connection_group_extract_connection (OS_nw_connection_group group, OS_nw_endpoint endpoint, OS_nw_protocol_options protocolOptions);

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos12.0")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[Mac (12, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
		public NWConnection? ExtractConnection (NWEndpoint endpoint, NWProtocolOptions protocolOptions)
		{
			var ptr = nw_connection_group_extract_connection (GetCheckedHandle (), endpoint.GetCheckedHandle (), protocolOptions.GetCheckedHandle ());
			return ptr == IntPtr.Zero ? null : new NWConnection (ptr, true);
		}

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos12.0")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[Mac (12, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool nw_connection_group_reinsert_extracted_connection (OS_nw_connection_group group, OS_nw_connection connection);

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos12.0")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[Mac (12, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
		public bool TryReinsertExtractedConnection (NWConnection connection)
		{
			if (connection is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (connection));
			return nw_connection_group_reinsert_extracted_connection (GetCheckedHandle (), connection.Handle);
		}

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos12.0")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[Mac (12, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_connection_group_set_new_connection_handler (OS_nw_connection_group group, BlockLiteral* connectionHandler);

#if !NET
		delegate void nw_connection_group_new_connection_handler_t (IntPtr block, IntPtr connection);
		static nw_connection_group_new_connection_handler_t static_SetNewConnectionHandler = TrampolineSetNewConnectionHandler;

		[MonoPInvokeCallback (typeof (nw_connection_group_new_connection_handler_t))]
#else
		[UnmanagedCallersOnly]
#endif
		static void TrampolineSetNewConnectionHandler (IntPtr block, IntPtr connection)
		{
			var del = BlockLiteral.GetTarget<Action<NWConnection>> (block);
			if (del is not null) {
				// the ownership of the object is for the caller
				using var nwReport = new NWConnection (connection, owns: true);
				del (nwReport);
			}
		}

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos12.0")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[Mac (12, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
		[BindingImpl (BindingImplOptions.Optimizable)]
		public void SetNewConnectionHandler (Action<NWConnection> handler)
		{
			if (handler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));

			unsafe {
#if NET
				delegate* unmanaged<IntPtr, IntPtr, void> trampoline = &TrampolineSetNewConnectionHandler;
				using var block = new BlockLiteral (trampoline, handler, typeof (NWConnectionGroup), nameof (TrampolineSetNewConnectionHandler));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_SetNewConnectionHandler, handler);
#endif
				nw_connection_group_set_new_connection_handler (GetCheckedHandle (), &block);
			}
		}
	}
}
