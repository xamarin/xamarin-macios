//
// NWTxtRecord.cs: Bindings the Network nw_txt_record_t API
//
// Authors:
//   Miguel de Icaza (miguel@microsoft.com)
//
// Copyright 2019 Microsoft Inc
//

#nullable enable

using System.Runtime.CompilerServices;
using System.Text;
using CoreFoundation;

using nw_advertise_descriptor_t = System.IntPtr;
using OS_nw_advertise_descriptor = System.IntPtr;
using OS_nw_txt_record = System.IntPtr;

namespace Network {
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	public class NWTxtRecord : NativeObject {
		[Preserve (Conditional = true)]
		internal NWTxtRecord (NativeHandle handle, bool owns) : base (handle, owns) { }

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern IntPtr nw_txt_record_create_with_bytes (byte* txtBytes, nuint len);

		public static NWTxtRecord? FromBytes (ReadOnlySpan<byte> bytes)
		{
			unsafe {
				fixed (byte* mh = bytes) {
					var x = nw_txt_record_create_with_bytes (mh, (nuint) bytes.Length);
					if (x == IntPtr.Zero)
						return null;
					return new NWTxtRecord (x, owns: true);
				}
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern IntPtr nw_txt_record_create_dictionary ();

		public static NWTxtRecord? CreateDictionary ()
		{
			var x = nw_txt_record_create_dictionary ();
			if (x == IntPtr.Zero)
				return null;
			return new NWTxtRecord (x, owns: true);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern IntPtr nw_txt_record_copy (IntPtr other);

		public NWTxtRecord Clone () => new NWTxtRecord (nw_txt_record_copy (GetCheckedHandle ()), owns: true);

		[DllImport (Constants.NetworkLibrary)]
		static extern NWTxtRecordFindKey nw_txt_record_find_key (IntPtr handle, IntPtr key);

		static NWTxtRecordFindKey nw_txt_record_find_key (IntPtr handle, string key)
		{
			using var keyPtr = new TransientString (key);
			return nw_txt_record_find_key (handle, keyPtr);
		}

		public NWTxtRecordFindKey FindKey (string key) => nw_txt_record_find_key (GetCheckedHandle (), key);

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern byte nw_txt_record_set_key (IntPtr handle, IntPtr key, IntPtr value, nuint valueLen);

		public bool Add (string key, ReadOnlySpan<byte> value)
		{
			unsafe {
				using var keyPtr = new TransientString (key);
				fixed (byte* mh = value)
					return nw_txt_record_set_key (GetCheckedHandle (), keyPtr, (IntPtr) mh, (nuint) value.Length) != 0;
			}
		}

		public bool Add (string key)
		{
			unsafe {
				using var keyPtr = new TransientString (key);
				return nw_txt_record_set_key (GetCheckedHandle (), keyPtr, IntPtr.Zero, 0) != 0;
			}
		}

		public bool Add (string key, string value)
			=> Add (key, value is null ? null : Encoding.UTF8.GetBytes (value));

		[DllImport (Constants.NetworkLibrary)]
		static extern byte nw_txt_record_remove_key (IntPtr handle, IntPtr key);

		static byte nw_txt_record_remove_key (IntPtr handle, string key)
		{
			using var keyPtr = new TransientString (key);
			return nw_txt_record_remove_key (handle, keyPtr);
		}

		public bool Remove (string key) => nw_txt_record_remove_key (GetCheckedHandle (), key) != 0;

		[DllImport (Constants.NetworkLibrary)]
		static extern long nw_txt_record_get_key_count (IntPtr handle);

		public long KeyCount => nw_txt_record_get_key_count (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		static extern byte nw_txt_record_is_dictionary (IntPtr handle);

		public bool IsDictionary => nw_txt_record_is_dictionary (GetCheckedHandle ()) != 0;

		[DllImport (Constants.NetworkLibrary)]
		static extern byte nw_txt_record_is_equal (OS_nw_txt_record left, OS_nw_txt_record right);

		public bool Equals (NWTxtRecord other)
		{
			if (other is null)
				return false;
			bool result = nw_txt_record_is_equal (GetCheckedHandle (), other.GetCheckedHandle ()) != 0;
			GC.KeepAlive (other);
			return result;
		}

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern byte nw_txt_record_apply (OS_nw_txt_record txt_record, BlockLiteral* applier);

		public delegate bool NWTxtRecordApplyDelegate (string? key, NWTxtRecordFindKey result, ReadOnlySpan<byte> value);

		[UnmanagedCallersOnly]
		unsafe static byte TrampolineApplyHandler (IntPtr block, IntPtr keyPointer, NWTxtRecordFindKey found, IntPtr value, nuint valueLen)
		{
			var del = BlockLiteral.GetTarget<NWTxtRecordApplyDelegate> (block);
			if (del is null)
				return (byte) 0;

			var mValue = new ReadOnlySpan<byte> ((void*) value, (int) valueLen);
			var key = Marshal.PtrToStringAuto (keyPointer);
			return del (key, found, mValue) ? (byte) 1 : (byte) 0;
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public bool Apply (NWTxtRecordApplyDelegate handler)
		{
			if (handler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));

			unsafe {
				delegate* unmanaged<IntPtr, IntPtr, NWTxtRecordFindKey, IntPtr, nuint, byte> trampoline = &TrampolineApplyHandler;
				using var block = new BlockLiteral (trampoline, handler, typeof (NWTxtRecord), nameof (TrampolineApplyHandler));
				var result = nw_txt_record_apply (GetCheckedHandle (), &block) != 0;
				GC.KeepAlive (this);
				return result;
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern unsafe byte nw_txt_record_access_key (OS_nw_txt_record txt_record, IntPtr key, BlockLiteral* access_value);

		public delegate void NWTxtRecordGetValueDelegete (string? key, NWTxtRecordFindKey result, ReadOnlySpan<byte> value);

		[UnmanagedCallersOnly]
		unsafe static void TrampolineAccessKeyHandler (IntPtr block, IntPtr keyPointer, NWTxtRecordFindKey found, IntPtr value, nuint valueLen)
		{
			var del = BlockLiteral.GetTarget<NWTxtRecordGetValueDelegete> (block);
			if (del is not null) {
				ReadOnlySpan<byte> mValue;
				if (found == NWTxtRecordFindKey.NonEmptyValue)
					mValue = new ReadOnlySpan<byte> ((void*) value, (int) valueLen);
				else
					mValue = Array.Empty<byte> ();
				var key = Marshal.PtrToStringAuto (keyPointer);
				del (key, found, mValue);
			}
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public bool GetValue (string key, NWTxtRecordGetValueDelegete handler)
		{
			if (handler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));

			unsafe {
				delegate* unmanaged<IntPtr, IntPtr, NWTxtRecordFindKey, IntPtr, nuint, void> trampoline = &TrampolineAccessKeyHandler;
				using var block = new BlockLiteral (trampoline, handler, typeof (NWTxtRecord), nameof (TrampolineAccessKeyHandler));
				using var keyPtr = new TransientString (key);
				var result = nw_txt_record_access_key (GetCheckedHandle (), keyPtr, &block) != 0;
				GC.KeepAlive (this);
				return result;
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern byte nw_txt_record_access_bytes (OS_nw_txt_record txt_record, BlockLiteral* access_bytes);

		public delegate void NWTxtRecordGetRawByteDelegate (ReadOnlySpan<byte> value);

		[UnmanagedCallersOnly]
		unsafe static void TrampolineRawBytesHandler (IntPtr block, IntPtr value, nuint valueLen)
		{
			var del = BlockLiteral.GetTarget<NWTxtRecordGetRawByteDelegate> (block);
			if (del is not null) {
				var mValue = new ReadOnlySpan<byte> ((void*) value, (int) valueLen);
				del (mValue);
			}
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public bool GetRawBytes (NWTxtRecordGetRawByteDelegate handler)
		{
			if (handler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));

			unsafe {
				delegate* unmanaged<IntPtr, IntPtr, nuint, void> trampoline = &TrampolineRawBytesHandler;
				using var block = new BlockLiteral (trampoline, handler, typeof (NWTxtRecord), nameof (TrampolineRawBytesHandler));
				var result = nw_txt_record_access_bytes (GetCheckedHandle (), &block) != 0;
				GC.KeepAlive (this);
				return result;
			}
		}
	}
}
