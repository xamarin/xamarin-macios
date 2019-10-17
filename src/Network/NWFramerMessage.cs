//
// NWFramer.cs: Bindings the Network nw_framer_t API.
//
// Authors:
//   Manuel de la Pena (mandel@microsoft.com)
//
// Copyright 2019 Microsoft Inc
//
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

namespace Network {

	[TV (13,0), Mac (10,15), iOS (13,0), Watch (6,0)]
	public class NWFramerMessage : NWProtocolMetadata {
		internal NWFramerMessage (IntPtr handle, bool owns) : base (handle, owns) {}

		[DllImport (Constants.NetworkLibrary)]
		static extern unsafe void nw_framer_message_set_value (OS_nw_protocol_metadata message, string key, byte *value, void *dispose_value);

		public void SetData (string key, ReadOnlySpan<byte> value)
		{
			// the method takes a callback to cleanup the data, but we do not need that since we are managed
			if (key == null)
				throw new ArgumentNullException (nameof (key));

			unsafe {
				fixed (byte* mh = value)
					nw_framer_message_set_value (GetCheckedHandle (), key,  mh, null);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
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
		public bool GetData (string key, int dataLenght, out ReadOnlySpan<byte> outData) {
			IntPtr outPointer = IntPtr.Zero;
			// create a function that will get the data, and the data length passed and will set the out param returning the value
			Func<IntPtr,bool> callback = (inData) => {
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
						outData = new ReadOnlySpan<byte>((void*)outPointer, dataLenght);
					}
				} else {
					outData = new ReadOnlySpan<byte> (Array.Empty<byte> ());
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

		public T GetObject<T> (string key) where T : NSObject
			=> Runtime.GetNSObject<T> (nw_framer_message_copy_object_value (GetCheckedHandle (), key), owns: true);
	}

}