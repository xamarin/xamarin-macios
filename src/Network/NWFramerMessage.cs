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

#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("macos10.15")]
	[SupportedOSPlatform ("ios13.0")]
#else
	[TV (13,0)]
	[Mac (10,15)]
	[iOS (13,0)]
	[Watch (6,0)]
#endif
	public class NWFramerMessage : NWProtocolMetadata {
		[Preserve (Conditional = true)]
		internal NWFramerMessage (NativeHandle handle, bool owns) : base (handle, owns) {}

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_protocol_metadata nw_framer_protocol_create_message (OS_nw_protocol_definition definition);

		// nw_framer_protocol can be condisered to be a nw_framer which is a protocol and is mapped to NWFramer, for a
		// detailed explanation of the reasoning behind the naming please read the following discussion:
		// https://github.com/xamarin/xamarin-macios/pull/7256#discussion_r337066971
		public static NWFramerMessage Create (NWProtocolDefinition protocolDefinition)
		{
			if (protocolDefinition == null)
				throw new ArgumentNullException (nameof (protocolDefinition));
			return new NWFramerMessage (nw_framer_protocol_create_message (protocolDefinition.Handle), owns: true);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_framer_message_set_value (OS_nw_protocol_metadata message, string key, IntPtr value, ref BlockLiteral dispose_value);
		delegate void nw_framer_message_set_value_t (IntPtr block, IntPtr data);
		static nw_framer_message_set_value_t static_SetDataHandler = TrampolineSetDataHandler;

		[MonoPInvokeCallback (typeof (nw_framer_message_set_value_t))]
		static void TrampolineSetDataHandler (IntPtr block, IntPtr data)
		{
			// get and call, this is internal and we are trying to do all the magic in the call
			var del = BlockLiteral.GetTarget<Action<IntPtr>> (block);
			if (del != null) {
				del (data);
			}
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void SetData (string key, byte[] value)
		{
			// the method takes a callback to cleanup the data, but we do not need that since we are managed
			if (key == null)
				throw new ArgumentNullException (nameof (key));

			// pin the handle so that is not collected,  let our callback release it
			var pinned = GCHandle.Alloc (value, GCHandleType.Pinned);
			Action<IntPtr> callback = (_) => {
				pinned.Free ();
			};
			BlockLiteral block_handler = new BlockLiteral ();
			block_handler.SetupBlockUnsafe (static_SetDataHandler, callback);
			try {
				nw_framer_message_set_value (GetCheckedHandle (), key,  pinned.AddrOfPinnedObject (), ref block_handler);
			} finally {
				block_handler.CleanupBlock ();
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool nw_framer_message_access_value (OS_nw_protocol_metadata message, string key, ref BlockLiteral access_value);
		delegate bool nw_framer_message_access_value_t (IntPtr block, IntPtr data);
		static nw_framer_message_access_value_t static_AccessValueHandler = TrampolineAccessValueHandler;


		[MonoPInvokeCallback (typeof (nw_framer_message_access_value_t))]
		static bool TrampolineAccessValueHandler (IntPtr block, IntPtr data)
		{
			// get and call, this is internal and we are trying to do all the magic in the call
			var del = BlockLiteral.GetTarget<Func<IntPtr, bool>> (block);
			if (del != null) {
				return del (data);
			}
			return false;
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public bool GetData (string key, int dataLength, out ReadOnlySpan<byte> outData) {
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

			BlockLiteral block_handler = new BlockLiteral ();
			block_handler.SetupBlockUnsafe (static_AccessValueHandler, callback);
			try {
				// the callback is inlined!!!
				var found = nw_framer_message_access_value (GetCheckedHandle (), key, ref block_handler);
				if (found) {
					unsafe {
						outData = new ReadOnlySpan<byte>((void*) outPointer, dataLength);
					}
				} else {
					outData = ReadOnlySpan<byte>.Empty; 
				}
				return found;
			} finally {
				block_handler.CleanupBlock ();
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_framer_message_set_object_value (OS_nw_protocol_metadata message, string key, IntPtr value);

		public void SetObject (string key, NSObject value)
			=> nw_framer_message_set_object_value (GetCheckedHandle (), key, value.GetHandle ()); 

		[DllImport (Constants.NetworkLibrary)]
		static extern IntPtr nw_framer_message_copy_object_value (OS_nw_protocol_metadata message, string key);

		public T? GetObject<T> (string key) where T : NSObject
			=> Runtime.GetNSObject<T> (nw_framer_message_copy_object_value (GetCheckedHandle (), key), owns: true);
	}

}
