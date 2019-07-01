//
// NWTextRecord.cs: Bindings the Netowrk nw_txt_record_t API
//
// Authors:
//   Miguel de Icaza (miguel@microsoft.com)
//
// Copyrigh 2019 Microsoft Inc
//
using System;
using System.Runtime.InteropServices;
using ObjCRuntime;
using Foundation;
using CoreFoundation;

using nw_advertise_descriptor_t=System.IntPtr;
using OS_nw_advertise_descriptor=System.IntPtr;

namespace Network {

	public enum NWTxtRecordFindKey {
		Invalid = 0,
		NotPresent = 1,
		NoValue = 2,
		EmptyValue = 3,
		NonEmptyValue = 4
	}
	
	[TV (13,0), Mac (10,15, onlyOn64: true), iOS (13,0), watchOS(6.0)]
	public class NWTxtRecord : NativeObject {
		public NWTxtRecord (IntPtr handle, bool owns) : base (handle, owns)
		{ }

		[DllImport (Constants.NetworkLibrary)]
		static extern IntPtr nw_txt_record_create_with_bytes (IntPtr txtBytes, IntPtr len);

		public static NWTxtRecord FromBytes (byte [] bytes)
		{
			if (bytes == null || bytes.Length == 0)
				return null;
			unsafe {
				fixed (byte *p = &bytes[0]){
					var x = nw_txt_record_create_with_bytes ((IntPtr) p, bytes.Length);
					if (x == IntPtr.Zero)
						return null;
					return new NWTxtRecord (x, owns: true);
				}
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern IntPtr nw_txt_record_create_dictionary(void);

		public static NWTxtRecord CreateDictionary ()
		{
			var x = nw_txt_record_create_dictionary ();
			if (x == IntPtr.Zero)
				return null;
			return new NWTxtRecord (x, owns: true);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern IntPtr nw_txt_record_copy (IntPtr other);
		
		public NWTxtRecord Clone ()
		{
			if (handle == IntPtr.Zero)
				return null;
			return new NWTxtRecord (nw_txt_record_copy (handle), owns: true);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern NWTxtRecordFindKey nw_txt_record_find_key (IntPtr handle, string key);
		
		public NWTxtRecordFindKey FindKey (string key)
		{
			return nw_txt_record_find_key (handle, key);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern byte nw_txt_record_set_key(IntPtr handle, string key, IntPtr value, IntPtr valueLen);

		public bool SetKey (string key, byte [] value)
		{
			if (value == null)
				return nw_txt_record_set_key (handle, key, IntPtr.Zero, IntPtr.Zero) != 0
			else {
				fixed (var pv = &value [0])
					return nw_txt_record_set_key (handle, key, (IntPtr) pv, value.Length) != 0;
			}
		}
		
		public bool SetKey (string key, string value)
		{
			return SetKey (key, value == null ? null : System.Text.UTF8Encoding.GetBytes (value));
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern byte nw_txt_record_remove_key (IntPtr handle, string key);

		public bool RemoveKey (string key)
		{
			return nw_txt_record_remove_key (handle, key) != 0;
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern IntPtr nw_txt_record_get_key_count(IntPtr handle);
		
		public long KeyCount {
			get {
				return nw_txt_record_get_key_count (handle);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern byte nw_txt_record_is_dictionary(IntPtr handle);

		public bool IsDictionary {
			get => nw_txt_record_is_dictionary (handle) != 0;
		}
	}
}
