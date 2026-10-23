//
// NWContentContext.cs: Bindings the Netowrk nw_content_context_t API
//
// Authors:
//   Miguel de Icaza (miguel@microsoft.com)
//
// Copyrigh 2018 Microsoft Inc
//

#nullable enable

using CoreFoundation;

namespace Network {

	//
	// The content context, there are a few pre-configured content contexts for sending
	// available as static properties on this class
	//
	/// <summary>To be added.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	public class NWContentContext : NativeObject {
		bool global;
		[Preserve (Conditional = true)]
		internal NWContentContext (NativeHandle handle, bool owns) : base (handle, owns)
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

		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		protected internal override void Release ()
		{
			if (global)
				return;
			base.Release ();
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_content_context_create (IntPtr contextIdentifier);

		/// <param name="contextIdentifier">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public NWContentContext (string contextIdentifier)
		{
			if (contextIdentifier is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (contextIdentifier));
			using var contextIdentifierPtr = new TransientString (contextIdentifier);
			InitializeHandle (nw_content_context_create (contextIdentifierPtr));
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_content_context_get_identifier (IntPtr handle);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? Identifier => Marshal.PtrToStringAnsi (nw_content_context_get_identifier (GetCheckedHandle ()));

		[DllImport (Constants.NetworkLibrary)]
		extern static byte nw_content_context_get_is_final (IntPtr handle);

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_content_context_set_is_final (IntPtr handle, byte is_final);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool IsFinal {
			get => nw_content_context_get_is_final (GetCheckedHandle ()) != 0;
			set => nw_content_context_set_is_final (GetCheckedHandle (), value.AsByte ());
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static /* uint64_t */ ulong nw_content_context_get_expiration_milliseconds (IntPtr handle);

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_content_context_set_expiration_milliseconds (IntPtr handle, /* uint64_t */ ulong value);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public ulong ExpirationMilliseconds {
			get => nw_content_context_get_expiration_milliseconds (GetCheckedHandle ());
			set => nw_content_context_set_expiration_milliseconds (GetCheckedHandle (), value);
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static double nw_content_context_get_relative_priority (IntPtr handle);

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_content_context_set_relative_priority (IntPtr handle, double value);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public double RelativePriority {
			get => nw_content_context_get_relative_priority (GetCheckedHandle ());
			set => nw_content_context_set_relative_priority (GetCheckedHandle (), value);
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_content_context_copy_antecedent (IntPtr handle);

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_content_context_set_antecedent (IntPtr handle, IntPtr value);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public NWContentContext? Antecedent {
			get {
				var h = nw_content_context_copy_antecedent (GetCheckedHandle ());
				if (h == IntPtr.Zero)
					return null;
				return new NWContentContext (h, owns: true);
			}
			set {
				nw_content_context_set_antecedent (GetCheckedHandle (), value.GetHandle ());
				GC.KeepAlive (value);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_content_context_copy_protocol_metadata (IntPtr handle, IntPtr protocol);

		/// <param name="protocolDefinition">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public NWProtocolMetadata? GetProtocolMetadata (NWProtocolDefinition protocolDefinition)
		{
			if (protocolDefinition is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (protocolDefinition));
			var x = nw_content_context_copy_protocol_metadata (GetCheckedHandle (), protocolDefinition.Handle);
			GC.KeepAlive (protocolDefinition);
			if (x == IntPtr.Zero)
				return null;
			return new NWProtocolMetadata (x, owns: true);
		}

		public T? GetProtocolMetadata<T> (NWProtocolDefinition protocolDefinition) where T : NWProtocolMetadata
		{
			if (protocolDefinition is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (protocolDefinition));
			var x = nw_content_context_copy_protocol_metadata (GetCheckedHandle (), protocolDefinition.Handle);
			GC.KeepAlive (protocolDefinition);
			return Runtime.GetINativeObject<T> (x, owns: true);
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_content_context_set_metadata_for_protocol (IntPtr handle, IntPtr protocolMetadata);

		/// <param name="protocolMetadata">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public void SetMetadata (NWProtocolMetadata protocolMetadata)
		{
			if (protocolMetadata is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (protocolMetadata));
			nw_content_context_set_metadata_for_protocol (GetCheckedHandle (), protocolMetadata.Handle);
			GC.KeepAlive (protocolMetadata);
		}

		[UnmanagedCallersOnly]
		static void TrampolineProtocolIterator (IntPtr block, IntPtr definition, IntPtr metadata)
		{
			var del = BlockLiteral.GetTarget<Action<NWProtocolDefinition?, NWProtocolMetadata?>> (block);
			if (del is not null) {
				using NWProtocolDefinition? pdef = definition == IntPtr.Zero ? null : new NWProtocolDefinition (definition, owns: false);
				using NWProtocolMetadata? meta = metadata == IntPtr.Zero ? null : new NWProtocolMetadata (metadata, owns: false);

				del (pdef, meta);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_content_context_foreach_protocol_metadata (IntPtr handle, BlockLiteral* callback);

		/// <param name="callback">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[BindingImpl (BindingImplOptions.Optimizable)]
		public void IterateProtocolMetadata (Action<NWProtocolDefinition?, NWProtocolMetadata?> callback)
		{
			unsafe {
				delegate* unmanaged<IntPtr, IntPtr, IntPtr, void> trampoline = &TrampolineProtocolIterator;
				using var block = new BlockLiteral (trampoline, callback, typeof (NWContentContext), nameof (TrampolineProtocolIterator));
				nw_content_context_foreach_protocol_metadata (GetCheckedHandle (), &block);
				GC.KeepAlive (this);
			}
		}

		//
		// Use this as a parameter to NWConnection.Send's with all the default properties
		// ie: NW_CONNECTION_DEFAULT_MESSAGE_CONTEXT, use this for datagrams
		static NWContentContext? defaultMessage;
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
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
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
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
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public static NWContentContext DefaultStream {
			get {
				if (defaultStream is null)
					defaultStream = MakeGlobal (NWContentContextConstants._DefaultStream);
				return defaultStream;
			}
		}
	}
}
