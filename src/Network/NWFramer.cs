//
// NWFramer.cs: Bindings the Network nw_framer_t API.
//
// Authors:
//   Manuel de la Pena (mandel@microsoft.com)
//
// Copyright 2019 Microsoft Inc
//

#nullable enable

using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using ObjCRuntime;
using Foundation;
using CoreFoundation;

using OS_nw_framer=System.IntPtr;
using OS_nw_protocol_metadata=System.IntPtr;
using OS_dispatch_data=System.IntPtr;
using OS_nw_protocol_definition=System.IntPtr;
using OS_nw_protocol_options=System.IntPtr;
using OS_nw_endpoint=System.IntPtr;
using OS_nw_parameters=System.IntPtr;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Network {

	public delegate nuint NWFramerParseCompletionDelegate (Memory<byte> buffer, [MarshalAs (UnmanagedType.I1)] bool isCompleted);
	public delegate nuint NWFramerInputDelegate (NWFramer framer); 

#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("macos10.15")]
	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("maccatalyst")]
#else
	[TV (13,0)]
	[Mac (10,15)]
	[iOS (13,0)]
	[Watch (6,0)]
#endif
	public class NWFramer : NativeObject {
		[Preserve (Conditional = true)]
		internal NWFramer (NativeHandle handle, bool owns) : base (handle, owns) {}

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool nw_framer_write_output_no_copy (OS_nw_framer framer, nuint output_length);

		public bool WriteOutputNoCopy (nuint outputLength) => nw_framer_write_output_no_copy (GetCheckedHandle (), outputLength);

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_framer_write_output_data (OS_nw_framer framer, OS_dispatch_data output_data);

		public void WriteOutput (DispatchData data)
		{
			if (data is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (data));
			nw_framer_write_output_data (GetCheckedHandle (), data.Handle);
		}

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_framer_write_output (OS_nw_framer framer, byte *output_buffer, nuint output_length);

		public void WriteOutput (ReadOnlySpan<byte> data)
		{
			unsafe {
				fixed (byte *mh = data)
					nw_framer_write_output (GetCheckedHandle (), mh, (nuint) data.Length);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_framer_set_wakeup_handler (OS_nw_framer framer, void *wakeup_handler);

		delegate void nw_framer_set_wakeup_handler_t (IntPtr block, OS_nw_framer framer);
		static nw_framer_set_wakeup_handler_t static_WakeupHandler = TrampolineWakeupHandler;

		[MonoPInvokeCallback (typeof (nw_framer_set_wakeup_handler_t))]
		static void TrampolineWakeupHandler (IntPtr block, OS_nw_framer framer)
		{
			var del = BlockLiteral.GetTarget<Action<NWFramer>> (block);
			if (del is not null) {
				var nwFramer = new NWFramer (framer, owns: true);
				del (nwFramer);
			}
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public Action<NWFramer> WakeupHandler {
			set {
				unsafe {
					if (value is null) {
						nw_framer_set_wakeup_handler (GetCheckedHandle (), null);
						return;
					}
					BlockLiteral block_handler = new BlockLiteral ();
					BlockLiteral *block_ptr_handler = &block_handler;
					block_handler.SetupBlockUnsafe (static_WakeupHandler, value);
					try {
						nw_framer_set_wakeup_handler (GetCheckedHandle (), (void*) block_ptr_handler);
					} finally {
						block_handler.CleanupBlock ();
					}
				}
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_framer_set_stop_handler (OS_nw_framer framer, void *stop_handler);

		delegate void nw_framer_set_stop_handler_t (IntPtr block, OS_nw_framer framer);
		static nw_framer_set_stop_handler_t static_StopHandler = TrampolineStopHandler;

		[MonoPInvokeCallback (typeof (nw_framer_set_stop_handler_t))]
		static void TrampolineStopHandler (IntPtr block, OS_nw_framer framer)
		{
			var del = BlockLiteral.GetTarget<Action<NWFramer>> (block);
			if (del is not null) {
				var nwFramer = new NWFramer (framer, owns: true);
				del (nwFramer);
			}
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public Action<NWFramer> StopHandler {
			set {
				unsafe {
					if (value is null) {
						nw_framer_set_stop_handler (GetCheckedHandle (), null);
						return;
					}
					BlockLiteral block_handler = new BlockLiteral ();
					BlockLiteral *block_ptr_handler = &block_handler;
					block_handler.SetupBlockUnsafe (static_StopHandler, value);
					try {
						nw_framer_set_stop_handler (GetCheckedHandle (), (void*) block_ptr_handler);
					} finally {
						block_handler.CleanupBlock ();
					}
				}
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_framer_set_output_handler (OS_nw_framer framer, void *output_handler);

		delegate void nw_framer_set_output_handler_t (IntPtr block, OS_nw_framer framer, OS_nw_protocol_metadata message, nuint message_length, bool is_complete);
		static nw_framer_set_output_handler_t static_OutputHandler = TrampolineOutputHandler;

		[MonoPInvokeCallback (typeof (nw_framer_set_output_handler_t))]
		static void TrampolineOutputHandler (IntPtr block, OS_nw_framer framer, OS_nw_protocol_metadata message, nuint message_length, bool is_complete)
		{
			var del = BlockLiteral.GetTarget<Action<NWFramer, NWProtocolMetadata, nuint, bool>> (block);
			if (del is not null) {
				var nwFramer = new NWFramer (framer, owns: true);
				var nwProtocolMetadata = new NWFramerMessage (message, owns: true);
				del (nwFramer, nwProtocolMetadata, message_length, is_complete);
			}
		}
		
		[BindingImpl (BindingImplOptions.Optimizable)]
		public Action<NWFramer, NWFramerMessage, nuint, bool> OutputHandler {
			set {
				unsafe {
					if (value is null) {
						nw_framer_set_output_handler (GetCheckedHandle (), null);
						return;
					}
					BlockLiteral block_handler = new BlockLiteral ();
					BlockLiteral *block_ptr_handler = &block_handler;
					block_handler.SetupBlockUnsafe (static_OutputHandler, value);
					try {
						nw_framer_set_output_handler (GetCheckedHandle (), (void*) block_ptr_handler);
					} finally {
						block_handler.CleanupBlock ();
					}
				}
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_framer_set_input_handler (OS_nw_framer framer, void *input_handler);

		delegate nuint nw_framer_set_input_handler_t (IntPtr block, OS_nw_framer framer);
		static nw_framer_set_input_handler_t static_InputHandler = TrampolineInputHandler;

		[MonoPInvokeCallback (typeof (nw_framer_set_input_handler_t))]
		static nuint TrampolineInputHandler (IntPtr block, OS_nw_framer framer)
		{
			var del = BlockLiteral.GetTarget<NWFramerInputDelegate> (block);
			if (del is not null) {
				var nwFramer = new NWFramer (framer, owns: true);
				return del (nwFramer);
			}
			return 0;
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public NWFramerInputDelegate InputHandler {
			set {
				unsafe {
					if (value is null) {
						nw_framer_set_input_handler (GetCheckedHandle (), null);
						return;
					}
					BlockLiteral block_handler = new BlockLiteral ();
					BlockLiteral *block_ptr_handler = &block_handler;
					block_handler.SetupBlockUnsafe (static_InputHandler, value);
					try {
						nw_framer_set_input_handler (GetCheckedHandle (), (void*) block_ptr_handler);
					} finally {
						block_handler.CleanupBlock ();
					}
				}
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_framer_set_cleanup_handler (OS_nw_framer framer, void *cleanup_handler);

		delegate void nw_framer_set_cleanup_handler_t (IntPtr block, OS_nw_framer framer);
		static nw_framer_set_cleanup_handler_t static_CleanupHandler = TrampolineCleanupHandler;

		[MonoPInvokeCallback (typeof (nw_framer_set_cleanup_handler_t))]
		static void TrampolineCleanupHandler (IntPtr block, OS_nw_framer framer)
		{
			var del = BlockLiteral.GetTarget<Action<NWFramer>> (block);
			if (del is not null) {
				var nwFramer = new NWFramer (framer, owns: true);
				del (nwFramer);
			}
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public Action<NWFramer> CleanupHandler {
			set {
				unsafe {
					if (value is null) {
						nw_framer_set_cleanup_handler (GetCheckedHandle (), null);
						return;
					}
					BlockLiteral block_handler = new BlockLiteral ();
					BlockLiteral *block_ptr_handler = &block_handler;
					block_handler.SetupBlockUnsafe (static_InputHandler, value);
					try {
						nw_framer_set_cleanup_handler (GetCheckedHandle (), (void*) block_ptr_handler);
					} finally {
						block_handler.CleanupBlock ();
					}
				}
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_framer_schedule_wakeup (OS_nw_framer framer, ulong milliseconds);

		public void ScheduleWakeup (ulong milliseconds) => nw_framer_schedule_wakeup (GetCheckedHandle (), milliseconds);

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_protocol_metadata nw_framer_message_create (OS_nw_framer framer);

		public NWFramerMessage CreateMessage ()
			=> new NWFramerMessage (nw_framer_message_create (GetCheckedHandle ()), owns: true);

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool nw_framer_prepend_application_protocol (OS_nw_framer framer, OS_nw_protocol_options protocol_options);

		public bool PrependApplicationProtocol (NWProtocolOptions options)
		{
			if (options is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (options));
			return nw_framer_prepend_application_protocol (GetCheckedHandle (), options.Handle);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_framer_pass_through_output (OS_nw_framer framer);

		public void PassThroughOutput () => nw_framer_pass_through_output (GetCheckedHandle ()); 

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_framer_pass_through_input (OS_nw_framer framer);

		public void PassThroughInput () => nw_framer_pass_through_input (GetCheckedHandle ()); 

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_framer_mark_ready (OS_nw_framer framer);

		public void MarkReady () => nw_framer_mark_ready (GetCheckedHandle ()); 

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_framer_mark_failed_with_error (OS_nw_framer framer, int error_code);

		public void MarkFailedWithError (int errorCode) => nw_framer_mark_failed_with_error (GetCheckedHandle (), errorCode); 

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool nw_framer_deliver_input_no_copy (OS_nw_framer framer, nuint input_length, OS_nw_protocol_metadata message, [MarshalAs (UnmanagedType.I1)] bool is_complete);

		public bool DeliverInputNoCopy (nuint length, NWFramerMessage message, bool isComplete)
		{
			if (message is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (message));
			return nw_framer_deliver_input_no_copy (GetCheckedHandle (), length, message.Handle, isComplete);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_protocol_options nw_framer_create_options (OS_nw_protocol_definition framer_definition);

		public static T? CreateOptions<T> (NWProtocolDefinition protocolDefinition) where T: NWProtocolOptions
		{
			if (protocolDefinition is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (protocolDefinition));
			var x = nw_framer_create_options (protocolDefinition.Handle);
			return Runtime.GetINativeObject<T> (x, owns: true);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_endpoint nw_framer_copy_remote_endpoint (OS_nw_framer framer);

		public NWEndpoint Endpoint => new NWEndpoint (nw_framer_copy_remote_endpoint (GetCheckedHandle ()), owns: true);

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_parameters nw_framer_copy_parameters (OS_nw_framer framer);

		public NWParameters Parameters => new NWParameters (nw_framer_copy_parameters (GetCheckedHandle ()), owns: true);

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_endpoint nw_framer_copy_local_endpoint (OS_nw_framer framer);

		public NWEndpoint LocalEndpoint => new NWEndpoint (nw_framer_copy_local_endpoint (GetCheckedHandle ()), owns: true);

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_framer_async (OS_nw_framer framer, void *async_block);

		delegate void nw_framer_async_t (IntPtr block);
		static nw_framer_async_t static_ScheduleHandler = TrampolineScheduleHandler;

		[MonoPInvokeCallback (typeof (nw_framer_async_t))]
		static void TrampolineScheduleHandler (IntPtr block) 
		{
			var del = BlockLiteral.GetTarget<Action> (block);
			if (del is not null) {
				del ();
			}
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void ScheduleAsync (Action handler)
		{
			unsafe {
				if (handler is null) {
					nw_framer_async (GetCheckedHandle (), null);
					return;
				}
				BlockLiteral block_handler = new BlockLiteral ();
				BlockLiteral *block_ptr_handler = &block_handler;
				block_handler.SetupBlockUnsafe (static_ScheduleHandler, handler);
				try {
					nw_framer_async (GetCheckedHandle (), (void*) block_ptr_handler);
				} finally {
					block_handler.CleanupBlock ();
				}
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern unsafe bool nw_framer_parse_output (OS_nw_framer framer, nuint minimum_incomplete_length, nuint maximum_length, byte *temp_buffer, ref BlockLiteral parse);

		delegate void nw_framer_parse_output_t (IntPtr block, IntPtr buffer, nuint buffer_length, bool is_complete);
		static nw_framer_parse_output_t static_ParseOutputHandler = TrampolineParseOutputHandler;

		[MonoPInvokeCallback (typeof (nw_framer_parse_output_t))]
		static void TrampolineParseOutputHandler (IntPtr block, IntPtr buffer, nuint buffer_length, bool is_complete)
		{
			var del = BlockLiteral.GetTarget<Action<Memory<byte>, bool>> (block);
			if (del is not null) {
				var bBuffer = new byte[buffer_length];
				Marshal.Copy (buffer, bBuffer, 0, (int)buffer_length);
				var mValue = new Memory<byte>(bBuffer);
				del (mValue, is_complete);
			}
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public bool ParseOutput (nuint minimumIncompleteLength, nuint maximumLength, Memory<byte> tempBuffer, Action<Memory<byte>, bool> handler)
		{
			if (handler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));
			unsafe {
				BlockLiteral block_handler = new BlockLiteral ();
				block_handler.SetupBlockUnsafe (static_ParseOutputHandler, handler);
				try {
					using (var mh = tempBuffer.Pin ())
						return nw_framer_parse_output (GetCheckedHandle (), minimumIncompleteLength, maximumLength, (byte*)mh.Pointer, ref block_handler);
				} finally {
					block_handler.CleanupBlock ();
				}
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern unsafe bool nw_framer_parse_input (OS_nw_framer framer, nuint minimum_incomplete_length, nuint maximum_length, byte *temp_buffer, ref BlockLiteral parse);

		delegate nuint nw_framer_parse_input_t (IntPtr block, IntPtr buffer, nuint buffer_length, bool is_complete);
		static nw_framer_parse_input_t static_ParseInputHandler = TrampolineParseInputHandler;

		[MonoPInvokeCallback (typeof (nw_framer_parse_input_t))]
		static nuint TrampolineParseInputHandler (IntPtr block, IntPtr buffer, nuint buffer_length, bool is_complete)
		{
			var del = BlockLiteral.GetTarget<NWFramerParseCompletionDelegate> (block);
			if (del is not null) {
				var bBuffer = new byte[buffer_length];
				Marshal.Copy (buffer, bBuffer, 0, (int)buffer_length);
				var mValue = new Memory<byte>(bBuffer);
				return del (mValue, is_complete);
			}
			return 0;
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public bool ParseInput (nuint minimumIncompleteLength, nuint maximumLength, Memory<byte> tempBuffer, NWFramerParseCompletionDelegate handler)
		{
			if (handler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));
			unsafe {
				BlockLiteral block_handler = new BlockLiteral ();
				block_handler.SetupBlockUnsafe (static_ParseInputHandler, handler);
				try {
					using (var mh = tempBuffer.Pin ())
						return nw_framer_parse_input (GetCheckedHandle (), minimumIncompleteLength, maximumLength, (byte*)mh.Pointer, ref block_handler);
				} finally {
					block_handler.CleanupBlock ();
				}
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_framer_deliver_input (OS_nw_framer framer, byte *input_buffer, nuint input_length, OS_nw_protocol_metadata message, [MarshalAs (UnmanagedType.I1)] bool is_complete);

		public void DeliverInput (ReadOnlySpan<byte> buffer, NWFramerMessage message, bool isComplete)
		{
			if (message is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (message));
			unsafe {
				fixed (byte *mh = buffer)
					nw_framer_deliver_input (GetCheckedHandle (),mh, (nuint)buffer.Length, message.Handle, isComplete);
			}
		}
	}
}
