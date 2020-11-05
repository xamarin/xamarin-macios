using System;
using System.Runtime.InteropServices;
using ObjCRuntime;
using Foundation;
using CoreFoundation;
using OS_nw_connection=System.IntPtr;
using OS_nw_connection_group=System.IntPtr;
using OS_nw_group_descriptor=System.IntPtr;
using OS_nw_parameters=System.IntPtr;
using OS_nw_content_context=System.IntPtr;
using OS_nw_path=System.IntPtr;
using OS_nw_endpoint=System.IntPtr;

#nullable enable

namespace Network {

	[TV (14,0), Mac (11,0), iOS (14,0), Watch (7,0)]
	public enum NWConnectionGroupState 
	{
		Invalid = 0,
		Waiting = 1,
		Ready = 2,
		Failed = 3,
		Cancelled = 4,
	}

	[TV (14,0), Mac (11,0), iOS (14,0), Watch (7,0)]
	public delegate void NWConnectionGroupReceiveDelegate (DispatchData content, NWContentContext context, bool isCompleted);

	[TV (14,0), Mac (11,0), iOS (14,0), Watch (7,0)]
	public delegate void NWConnectionGroupStateChangedDelegate (NWConnectionGroupState state, NWError? error);

	[TV (14,0), Mac (11,0), iOS (14,0), Watch (7,0)]
	public class NWConnectionGroup : NativeObject {
		protected NWConnectionGroup (IntPtr handle, bool owns) : base (handle, owns) {}

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_connection_group nw_connection_group_create (OS_nw_group_descriptor group_descriptor, OS_nw_parameters parameters);

		public NWConnectionGroup (NWMulticastGroup groupDescriptor, NWParameters parameters)
		{
			if (groupDescriptor == null)
				throw new ArgumentNullException (nameof (groupDescriptor));
			if (parameters == null)
				throw new ArgumentNullException (nameof (parameters));

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
 			if (queue == null)
				throw new ArgumentNullException (nameof (queue));

			nw_connection_group_set_queue (GetCheckedHandle (), queue.GetCheckedHandle ());
		}

		// can return null
		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_endpoint nw_connection_group_copy_local_endpoint_for_message (OS_nw_connection_group group, OS_nw_content_context context);

		public NWEndpoint? GetLocalEndpoint (NWContentContext context)
		{
			if (context == null)
				throw new ArgumentNullException (nameof (context));
			var ptr = nw_connection_group_copy_local_endpoint_for_message (GetCheckedHandle (), context.GetCheckedHandle ());
			return ptr == IntPtr.Zero ? null : new NWEndpoint (ptr, owns: true);
		}

		// can return null
		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_path nw_connection_group_copy_path_for_message (OS_nw_connection_group group, OS_nw_content_context context);

		public NWPath? GetPath (NWContentContext context)
		{
			if (context == null)
				throw new ArgumentNullException (nameof (context));
			var ptr = nw_connection_group_copy_path_for_message (GetCheckedHandle (), context.GetCheckedHandle ());
			return ptr == IntPtr.Zero ? null : new NWPath (ptr, owns: true);
		}

		// can return null
		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_endpoint nw_connection_group_copy_remote_endpoint_for_message (OS_nw_connection_group group, OS_nw_content_context context);

		public NWEndpoint? GetRemmoteEndpoint (NWContentContext context)
		{
			if (context == null)
				throw new ArgumentNullException (nameof (context));
			var ptr = nw_connection_group_copy_remote_endpoint_for_message (GetCheckedHandle (), context.GetCheckedHandle ());
			return ptr == IntPtr.Zero ? null : new NWEndpoint (ptr, owns: true);
		}

		// can return null
		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_connection nw_connection_group_extract_connection_for_message (OS_nw_connection_group group, OS_nw_content_context context);

		public NWConnection? GetConnection (NWContentContext context)
		{
			if (context == null)
				throw new ArgumentNullException (nameof (context));
			var ptr = nw_connection_group_extract_connection_for_message (GetCheckedHandle (), context.GetCheckedHandle ());
			return ptr == IntPtr.Zero ? null : new NWConnection (ptr, owns: true);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_connection_group_reply (OS_nw_connection_group group, OS_nw_content_context inbound_message, OS_nw_content_context outbound_message, /* [NullAllowed]  DispatchData */ IntPtr content);

		public void Reply (NWContentContext inboundMessage, NWContentContext outboundMessage, DispatchData content)
		{
			if (inboundMessage == null)
				throw new ArgumentNullException (nameof (inboundMessage));
			if (outboundMessage == null)
				throw new ArgumentNullException (nameof (outboundMessage));

			nw_connection_group_reply (GetCheckedHandle (), inboundMessage.GetCheckedHandle  (), outboundMessage.GetCheckedHandle (), content.GetHandle ());
		}

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_connection_group_send_message (OS_nw_connection_group group, /* [NullAllowed] DispatchData */ IntPtr content, /* [NullAllowed] */ OS_nw_endpoint endpoint, OS_nw_content_context context, BlockLiteral *handler);

		delegate void nw_connection_group_send_completion_t (IntPtr block, IntPtr error);
		static nw_connection_group_send_completion_t static_SendCompletion = TrampolineSendCompletion;

		[MonoPInvokeCallback (typeof (nw_connection_group_send_completion_t))]
		static void TrampolineSendCompletion (IntPtr block, IntPtr error)
		{
			var del = BlockLiteral.GetTarget<Action<NWError?>> (block);
			if (del != null) {
				using var err = error == IntPtr.Zero ? null : new NWError (error, owns: false);
				del (err);
			}
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void Send (DispatchData? content, NWEndpoint? endpoint, NWContentContext context, Action<NWError?>? handler)
		{
			unsafe {
				if (handler == null) {
					nw_connection_group_send_message (GetCheckedHandle (),
						content.GetHandle (),
						endpoint.GetHandle (),
						context.GetCheckedHandle (),
						null);
					return;
				}

				BlockLiteral block_handler = new BlockLiteral ();
				block_handler.SetupBlockUnsafe (static_SendCompletion, handler);
				try {
					nw_connection_group_send_message (GetCheckedHandle (),
						content.GetHandle (),
						endpoint.GetHandle (),
						context.GetCheckedHandle (),
						&block_handler);
				} finally {
					block_handler.CleanupBlock ();
				}
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_connection_group_set_receive_handler (OS_nw_connection_group group, uint maximum_message_size, bool reject_oversized_messages, BlockLiteral *handler);

		delegate void nw_connection_group_receive_handler_t (IntPtr block, IntPtr content, IntPtr context, bool isCompleted);
		static nw_connection_group_receive_handler_t static_ReceiveHandler = TrampolineReceiveHandler;

		[MonoPInvokeCallback (typeof (nw_connection_group_receive_handler_t))]
		static void TrampolineReceiveHandler (IntPtr block, IntPtr content, IntPtr context, bool isCompleted)
		{
			var del = BlockLiteral.GetTarget<NWConnectionGroupReceiveDelegate> (block);
			if (del != null) {
				using var nsContent = new DispatchData (content, owns: false);
				using var nsContext = new NWContentContext (context, owns: false);
				del (nsContent, nsContext, isCompleted);
			}
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void SetReceiveHandler (uint maximumMessageSize, bool rejectOversizedMessages, NWConnectionGroupReceiveDelegate? handler)
		{
			unsafe {
				if (handler == null) {
					nw_connection_group_set_receive_handler (GetCheckedHandle (), maximumMessageSize, rejectOversizedMessages, null);
					return;
				}

				BlockLiteral block_handler = new BlockLiteral ();
				block_handler.SetupBlockUnsafe (static_ReceiveHandler, handler);
				try {
					nw_connection_group_set_receive_handler (GetCheckedHandle (), maximumMessageSize, rejectOversizedMessages, &block_handler);
				} finally {
					block_handler.CleanupBlock ();
				}
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_connection_group_set_state_changed_handler (OS_nw_connection_group group, BlockLiteral *handler);

		delegate void nw_connection_group_state_changed_handler_t (IntPtr block, NWConnectionGroupState state , IntPtr error);
		static nw_connection_group_state_changed_handler_t static_StateChangedHandler = TrampolineStateChangedHandler;

		[MonoPInvokeCallback (typeof (nw_connection_group_state_changed_handler_t))]
		static void TrampolineStateChangedHandler (IntPtr block, NWConnectionGroupState state, IntPtr error)
		{
			var del = BlockLiteral.GetTarget<NWConnectionGroupStateChangedDelegate> (block);
			if (del != null) {
				using var nwError = (error == IntPtr.Zero) ? null : new NWError (error, owns: false);
				del (state, nwError);
			}
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void SetStateChangedHandler (NWConnectionGroupStateChangedDelegate handler)
		{
			unsafe {
				if (handler == null) {
					nw_connection_group_set_state_changed_handler (GetCheckedHandle (), null);
					return;
				}

				BlockLiteral block_handler = new BlockLiteral ();
				block_handler.SetupBlockUnsafe (static_StateChangedHandler, handler);
				try {
					nw_connection_group_set_state_changed_handler (GetCheckedHandle (), &block_handler);
				} finally {
					block_handler.CleanupBlock ();
				}
			}
		}
	}
}
