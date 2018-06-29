//
// NWContentContext.cs: Bindings the Netowrk nw_content_context_t API
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

namespace Network {

	//
	// The content context, there are a few pre-configured content contexts for sending
	// available as static properties on this class
	// 
	public class NWContentContext : INativeObject, IDisposable {
		IntPtr handle;
		public IntPtr Handle {
			get { return handle; }
		}

		public NWContentContext (IntPtr handle, bool owns)
		{
			this.handle = handle;
			if (owns == false)
				CFObject.CFRetain (handle);
		}

		public NWContentContext (IntPtr handle) : this (handle, false)
		{
		}

		~NWContentContext ()
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

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_content_context_create (string contextIdentifier);

		public NWContentContext (string contextIdentifier)
		{
			if (contextIdentifier == null)
				throw new ArgumentNullException (nameof (contextIdentifier));
			handle = nw_content_context_create (contextIdentifier);
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_content_context_get_identifier (IntPtr handle);

		public string Identifier => Marshal.PtrToStringAnsi (nw_content_context_get_identifier (handle));

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool nw_content_context_get_is_final (IntPtr handle);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_content_context_set_is_final (IntPtr handle, [MarshalAs (UnmanagedType.I1)] bool is_final);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public bool IsFinal {
			get => nw_content_context_get_is_final (handle);
			set => nw_content_context_set_is_final (handle, value);
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static ulong nw_content_context_get_expiration_milliseconds (IntPtr handle);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_content_context_set_expiration_milliseconds (IntPtr handle, ulong value);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public ulong ExpirationMilliseconds {
			get => nw_content_context_get_expiration_milliseconds (handle);
			set => nw_content_context_set_expiration_milliseconds (handle, value);
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static double nw_content_context_get_relative_priority (IntPtr handle);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_content_context_set_relative_priority (IntPtr handle, double value);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public double RelativePriority {
			get => nw_content_context_get_relative_priority (handle);
			set => nw_content_context_set_relative_priority (handle, value);
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_content_context_copy_antecedent (IntPtr handle);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_content_context_set_antecedent (IntPtr handle, IntPtr value);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public NWContentContext Antecedent {
			get {
				var h = nw_content_context_copy_antecedent (handle);
				if (h == IntPtr.Zero)
					return null;
				return new NWContentContext (h, owns: true);
			}
			set {
				nw_content_context_set_antecedent (handle, value == null ? IntPtr.Zero : value.Handle);
			}
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_content_context_copy_protocol_metadata (IntPtr handle, IntPtr protocol);

		public NWProtocolMetadata GetProtocolMetadata (NWProtocolDefinition protocolDefinition)
		{
			if (protocolDefinition == null)
				throw new ArgumentNullException (nameof (protocolDefinition));
			var x = nw_content_context_copy_protocol_metadata (handle, protocolDefinition.handle);
			if (x == IntPtr.Zero)
				return null;
			return new NWProtocolMetadata (x, owns: true);
		}
		
		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_content_context_set_metadata_for_protocol (IntPtr handle, IntPtr protocolMetadata);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void SetMetadata (NWProtocolMetadata protocolMetadata)
		{
			if (protocolMetadata == null)
				throw new ArgumentNullException (nameof (protocolMetadata));
			nw_content_context_set_metadata_for_protocol (handle, protocolMetadata.Handle);
		}

		delegate void ProtocolIterator (IntPtr block, IntPtr definition, IntPtr metadata);
		static ProtocolIterator static_ProtocolIterator = TrampolineProtocolIterator;
		
		[MonoPInvokeCallback (typeof (ProtocolIterator))]
		static unsafe void TrampolineProtocolIterator (IntPtr block, IntPtr definition, IntPtr metadata)
		{
			var descriptor = (BlockLiteral *) block;
			var del = (Action<NWProtocolDefinition,NWProtocolMetadata>) (descriptor->Target);
			if (del != null){
				var pdef = definition == IntPtr.Zero ? null : new NWProtocolDefinition (definition, owns: true);
				var meta = metadata == IntPtr.Zero ? null : new NWProtocolMetadata (metadata, owns: true);

				del (pdef, meta);
				
				pdef?.Dispose ();
				meta?.Dispose ();
				if (pdef != null)
					pdef.Dispose ();
				if (meta != null)
					meta.Dispose ();
			}
		}
		
		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern unsafe void nw_content_context_foreach_protocol_metadata (IntPtr handle, void *callback);
		
		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void ItereateProtocolMetadata (Action<NWProtocolDefinition,NWProtocolMetadata> callback)
		{
			unsafe {
			        BlockLiteral *block_ptr_handler;
			        BlockLiteral block_handler;
			        block_handler = new BlockLiteral ();
			        block_ptr_handler = &block_handler;
			        block_handler.SetupBlockUnsafe (static_ProtocolIterator, callback);
			
			        nw_content_context_foreach_protocol_metadata (handle, (void*) block_ptr_handler);
			        block_ptr_handler->CleanupBlock ();
			}
		}

		//
		// Use this as a parameter to NWConnection.Send's with all the default properties
		// ie: NW_CONNECTION_DEFAULT_MESSAGE_CONTEXT, use this for datagrams
		public static NWContentContext DefaultMessage => new NWContentContext (Dlfcn.dlsym (Libraries.Network.Handle, "_nw_content_context_default_message"), owns: false);

		// Use this as a parameter to NWConnection.Send's to indicate that no more sends are expected
		// (ie: NW_CONNECTION_FINAL_MESSAGE_CONTEXT)
		public static NWContentContext FinalMessage = new NWContentContext (Dlfcn.dlsym (Libraries.Network.Handle, "_nw_content_context_final_send"), owns: false);

		// This sending context represents the entire connection
		// ie: NW_CONNECTION_DEFAULT_STREAM_CONTEXT
		public static NWContentContext DefaultStream = new NWContentContext (Dlfcn.dlsym (Libraries.Network.Handle, "_nw_content_context_default_stream"), owns: false);	
	}
}
