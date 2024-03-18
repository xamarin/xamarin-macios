//
// NWFramer.cs: Bindings the Network nw_framer_t API.
//
// Authors:
//   Manuel de la Pena (mandel@microsoft.com)
//
// Copyright 2019 Microsoft
//

#nullable enable

using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using ObjCRuntime;
using Foundation;
using CoreFoundation;

using OS_nw_framer = System.IntPtr;
using OS_nw_protocol_metadata = System.IntPtr;
using OS_dispatch_data = System.IntPtr;
using OS_nw_protocol_definition = System.IntPtr;
using OS_nw_protocol_options = System.IntPtr;
using OS_nw_endpoint = System.IntPtr;
using OS_nw_parameters = System.IntPtr;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Network {

#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("maccatalyst")]
#else
	[TV (13, 0)]
	[iOS (13, 0)]
	[Watch (6, 0)]
#endif
	public class NWFramerMessage : NWProtocolMetadata {
		[Preserve (Conditional = true)]
		internal NWFramerMessage (NativeHandle handle, bool owns) : base (handle, owns) { }

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_protocol_metadata nw_framer_protocol_create_message (OS_nw_protocol_definition definition);

		// nw_framer_protocol can be condisered to be a nw_framer which is a protocol and is mapped to NWFramer, for a
		// detailed explanation of the reasoning behind the naming please read the following discussion:
		// https://github.com/xamarin/xamarin-macios/pull/7256#discussion_r337066971
		public static NWFramerMessage Create (NWProtocolDefinition protocolDefinition)
		{
			if (protocolDefinition is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (protocolDefinition));
			return new NWFramerMessage (nw_framer_protocol_create_message (protocolDefinition.Handle), owns: true);
		}

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_framer_message_set_value (OS_nw_protocol_metadata message, IntPtr key, IntPtr value, BlockLiteral* dispose_value);
#if !NET
		delegate void nw_framer_message_set_value_t (IntPtr block, IntPtr data);
		static nw_framer_message_set_value_t static_SetDataHandler = TrampolineSetDataHandler;

		[MonoPInvokeCallback (typeof (nw_framer_message_set_value_t))]
#else
		[UnmanagedCallersOnly]
#endif
		static void TrampolineSetDataHandler (IntPtr block, IntPtr data)
		{
			// get and call, this is internal and we are trying to do all the magic in the call
			var del = BlockLiteral.GetTarget<Action<IntPtr>> (block);
			if (del is not null) {
				del (data);
			}
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void SetData (string key, byte [] value)
		{
			// the method takes a callback to cleanup the data, but we do not need that since we are managed
			if (key is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (key));

			// pin the handle so that is not collected,  let our callback release it
			var pinned = GCHandle.Alloc (value, GCHandleType.Pinned);
			Action<IntPtr> callback = (_) => {
				pinned.Free ();
			};
			unsafe {
#if NET
				delegate* unmanaged<IntPtr, IntPtr, void> trampoline = &TrampolineSetDataHandler;
				using var block = new BlockLiteral (trampoline, callback, typeof (NWFramerMessage), nameof (TrampolineSetDataHandler));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_SetDataHandler, callback);
#endif
				using var keyPtr = new TransientString (key);
				nw_framer_message_set_value (GetCheckedHandle (), keyPtr, pinned.AddrOfPinnedObject (), &block);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		unsafe static extern bool nw_framer_message_access_value (OS_nw_protocol_metadata message, IntPtr key, BlockLiteral* access_value);
#if !NET
		delegate byte nw_framer_message_access_value_t (IntPtr block, IntPtr data);
		static nw_framer_message_access_value_t static_AccessValueHandler = TrampolineAccessValueHandler;


		[MonoPInvokeCallback (typeof (nw_framer_message_access_value_t))]
#else
		[UnmanagedCallersOnly]
#endif
		static byte TrampolineAccessValueHandler (IntPtr block, IntPtr data)
		{
			// get and call, this is internal and we are trying to do all the magic in the call
			var del = BlockLiteral.GetTarget<Func<IntPtr, bool>> (block);
			if (del is not null) {
				return del (data) ? (byte) 1 : (byte) 0;
			}
			return 0;
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public bool GetData (string key, int dataLength, out ReadOnlySpan<byte> outData)
		{
			IntPtr outPointer = IntPtr.Zero;
			// create a function that will get the data, and the data length passed and will set the out param returning the value
			Func<IntPtr, bool> callback = (inData) => {
				if (inData != IntPtr.Zero) {
					outPointer = inData;
					return true;
				} else {
					return false;
				}
			};

			unsafe {
#if NET
				delegate* unmanaged<IntPtr, IntPtr, byte> trampoline = &TrampolineAccessValueHandler;
				using var block = new BlockLiteral (trampoline, callback, typeof (NWFramerMessage), nameof (TrampolineAccessValueHandler));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_AccessValueHandler, callback);
#endif
				// the callback is inlined!!!
				using var keyPtr = new TransientString (key);
				var found = nw_framer_message_access_value (GetCheckedHandle (), keyPtr, &block);
				if (found) {
					unsafe {
						outData = new ReadOnlySpan<byte> ((void*) outPointer, dataLength);
					}
				} else {
					outData = ReadOnlySpan<byte>.Empty;
				}
				return found;
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_framer_message_set_object_value (OS_nw_protocol_metadata message, IntPtr key, IntPtr value);

		static void nw_framer_message_set_object_value (OS_nw_protocol_metadata message, string key, IntPtr value)
		{
			using var keyPtr = new TransientString (key);
			nw_framer_message_set_object_value (message, keyPtr, value);
		}

		public void SetObject (string key, NSObject value)
			=> nw_framer_message_set_object_value (GetCheckedHandle (), key, value.GetHandle ());

		[DllImport (Constants.NetworkLibrary)]
		static extern IntPtr nw_framer_message_copy_object_value (OS_nw_protocol_metadata message, IntPtr key);

		static IntPtr nw_framer_message_copy_object_value (OS_nw_protocol_metadata message, string key)
		{
			using var keyPtr = new TransientString (key);
			return nw_framer_message_copy_object_value (message, keyPtr);
		}

		public T? GetObject<T> (string key) where T : NSObject
			=> Runtime.GetNSObject<T> (nw_framer_message_copy_object_value (GetCheckedHandle (), key), owns: true);
	}

}
