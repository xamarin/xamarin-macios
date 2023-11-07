//
// NWContentContext.cs: Bindings the Netowrk nw_content_context_t API
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

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Network {

	//
	// The content context, there are a few pre-configured content contexts for sending
	// available as static properties on this class
	//
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
	public class NWContentContext : NativeObject {
		bool global;
		[Preserve (Conditional = true)]
#if NET
		internal NWContentContext (NativeHandle handle, bool owns) : base (handle, owns)
#else
		public NWContentContext (NativeHandle handle, bool owns) : base (handle, owns)
#endif
		{
		}

		// This constructor is only called by MakeGlobal
		NWContentContext (IntPtr handle, bool owns, bool global) : base (handle, owns)
		{
			this.global = global;
		}

		// To prevent creating many versions of fairly common objects, we create versions
		// that set "global = true" and in that case, we do not release the object.
		static NWContentContext MakeGlobal (IntPtr handle)
		{
			return new NWContentContext (handle, owns: true, global: true);
		}

		protected internal override void Release ()
		{
			if (global)
				return;
			base.Release ();
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_content_context_create (IntPtr contextIdentifier);

		public NWContentContext (string contextIdentifier)
		{
			if (contextIdentifier is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (contextIdentifier));
			using var contextIdentifierPtr = new TransientString (contextIdentifier);
			InitializeHandle (nw_content_context_create (contextIdentifierPtr));
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_content_context_get_identifier (IntPtr handle);

		public string? Identifier => Marshal.PtrToStringAnsi (nw_content_context_get_identifier (GetCheckedHandle ()));

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool nw_content_context_get_is_final (IntPtr handle);

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_content_context_set_is_final (IntPtr handle, [MarshalAs (UnmanagedType.I1)] bool is_final);

		public bool IsFinal {
			get => nw_content_context_get_is_final (GetCheckedHandle ());
			set => nw_content_context_set_is_final (GetCheckedHandle (), value);
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static /* uint64_t */ ulong nw_content_context_get_expiration_milliseconds (IntPtr handle);

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_content_context_set_expiration_milliseconds (IntPtr handle, /* uint64_t */ ulong value);

		public ulong ExpirationMilliseconds {
			get => nw_content_context_get_expiration_milliseconds (GetCheckedHandle ());
			set => nw_content_context_set_expiration_milliseconds (GetCheckedHandle (), value);
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static double nw_content_context_get_relative_priority (IntPtr handle);

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_content_context_set_relative_priority (IntPtr handle, double value);

		public double RelativePriority {
			get => nw_content_context_get_relative_priority (GetCheckedHandle ());
			set => nw_content_context_set_relative_priority (GetCheckedHandle (), value);
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_content_context_copy_antecedent (IntPtr handle);

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_content_context_set_antecedent (IntPtr handle, IntPtr value);

		public NWContentContext? Antecedent {
			get {
				var h = nw_content_context_copy_antecedent (GetCheckedHandle ());
				if (h == IntPtr.Zero)
					return null;
				return new NWContentContext (h, owns: true);
			}
			set {
				nw_content_context_set_antecedent (GetCheckedHandle (), value.GetHandle ());
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_content_context_copy_protocol_metadata (IntPtr handle, IntPtr protocol);

		public NWProtocolMetadata? GetProtocolMetadata (NWProtocolDefinition protocolDefinition)
		{
			if (protocolDefinition is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (protocolDefinition));
			var x = nw_content_context_copy_protocol_metadata (GetCheckedHandle (), protocolDefinition.Handle);
			if (x == IntPtr.Zero)
				return null;
			return new NWProtocolMetadata (x, owns: true);
		}

		public T? GetProtocolMetadata<T> (NWProtocolDefinition protocolDefinition) where T : NWProtocolMetadata
		{
			if (protocolDefinition is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (protocolDefinition));
			var x = nw_content_context_copy_protocol_metadata (GetCheckedHandle (), protocolDefinition.Handle);
			return Runtime.GetINativeObject<T> (x, owns: true);
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_content_context_set_metadata_for_protocol (IntPtr handle, IntPtr protocolMetadata);

		public void SetMetadata (NWProtocolMetadata protocolMetadata)
		{
			if (protocolMetadata is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (protocolMetadata));
			nw_content_context_set_metadata_for_protocol (GetCheckedHandle (), protocolMetadata.Handle);
		}

#if !NET
		delegate void ProtocolIterator (IntPtr block, IntPtr definition, IntPtr metadata);
		static ProtocolIterator static_ProtocolIterator = TrampolineProtocolIterator;

		[MonoPInvokeCallback (typeof (ProtocolIterator))]
#else
		[UnmanagedCallersOnly]
#endif
		static void TrampolineProtocolIterator (IntPtr block, IntPtr definition, IntPtr metadata)
		{
			var del = BlockLiteral.GetTarget<Action<NWProtocolDefinition?, NWProtocolMetadata?>> (block);
			if (del is not null) {
				using NWProtocolDefinition? pdef = definition == IntPtr.Zero ? null : new NWProtocolDefinition (definition, owns: true);
				using NWProtocolMetadata? meta = metadata == IntPtr.Zero ? null : new NWProtocolMetadata (metadata, owns: true);

				del (pdef, meta);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_content_context_foreach_protocol_metadata (IntPtr handle, BlockLiteral* callback);

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void IterateProtocolMetadata (Action<NWProtocolDefinition?, NWProtocolMetadata?> callback)
		{
			unsafe {
#if NET
				delegate* unmanaged<IntPtr, IntPtr, IntPtr, void> trampoline = &TrampolineProtocolIterator;
				using var block = new BlockLiteral (trampoline, callback, typeof (NWContentContext), nameof (TrampolineProtocolIterator));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_ProtocolIterator, callback);
#endif
				nw_content_context_foreach_protocol_metadata (GetCheckedHandle (), &block);
			}
		}

		//
		// Use this as a parameter to NWConnection.Send's with all the default properties
		// ie: NW_CONNECTION_DEFAULT_MESSAGE_CONTEXT, use this for datagrams
		static NWContentContext? defaultMessage;
		public static NWContentContext DefaultMessage {
			get {
				if (defaultMessage is null)
					defaultMessage = MakeGlobal (NWContentContextConstants._DefaultMessage);

				return defaultMessage;
			}
		}

		// Use this as a parameter to NWConnection.Send's to indicate that no more sends are expected
		// (ie: NW_CONNECTION_FINAL_MESSAGE_CONTEXT)
		static NWContentContext? finalMessage;
		public static NWContentContext FinalMessage {
			get {
				if (finalMessage is null)
					finalMessage = MakeGlobal (NWContentContextConstants._FinalSend);
				return finalMessage;
			}
		}

		// This sending context represents the entire connection
		// ie: NW_CONNECTION_DEFAULT_STREAM_CONTEXT
		static NWContentContext? defaultStream;
		public static NWContentContext DefaultStream {
			get {
				if (defaultStream is null)
					defaultStream = MakeGlobal (NWContentContextConstants._DefaultStream);
				return defaultStream;
			}
		}
	}
}
