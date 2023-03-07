//
// NWTxtRecord.cs: Bindings the Network nw_txt_record_t API
//
// Authors:
//   Miguel de Icaza (miguel@microsoft.com)
//
// Copyright 2019 Microsoft Inc
//

#nullable enable

using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Text;
using ObjCRuntime;
using Foundation;
using CoreFoundation;

using nw_advertise_descriptor_t = System.IntPtr;
using OS_nw_advertise_descriptor = System.IntPtr;
using OS_nw_txt_record = System.IntPtr;

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
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool nw_txt_record_is_equal (OS_nw_txt_record left, OS_nw_txt_record right);

		public bool Equals (NWTxtRecord other)
		{
			if (other is null)
				return false;
			return nw_txt_record_is_equal (GetCheckedHandle (), other.GetCheckedHandle ());
		}

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		unsafe static extern bool nw_txt_record_apply (OS_nw_txt_record txt_record, BlockLiteral* applier);

#if !NET
		delegate byte nw_txt_record_apply_t (IntPtr block, IntPtr key, NWTxtRecordFindKey found, IntPtr value, nuint valueLen);
		unsafe static nw_txt_record_apply_t static_ApplyHandler = TrampolineApplyHandler;
#endif

#if NET
		public delegate bool NWTxtRecordApplyDelegate (string? key, NWTxtRecordFindKey result, ReadOnlySpan<byte> value);
#else
		public delegate void NWTxtRecordApplyDelegate (string? key, NWTxtRecordFindKey rersult, ReadOnlySpan<byte> value);
		public delegate bool NWTxtRecordApplyDelegate2 (string? key, NWTxtRecordFindKey result, ReadOnlySpan<byte> value);
#endif

#if !NET
		[MonoPInvokeCallback (typeof (nw_txt_record_apply_t))]
#else
		[UnmanagedCallersOnly]
#endif
		unsafe static byte TrampolineApplyHandler (IntPtr block, IntPtr keyPointer, NWTxtRecordFindKey found, IntPtr value, nuint valueLen)
		{
#if NET
			var del = BlockLiteral.GetTarget<NWTxtRecordApplyDelegate> (block);
#else
			var del = BlockLiteral.GetTarget<MulticastDelegate> (block);
#endif
			if (del is null)
				return (byte) 0;

			var mValue = new ReadOnlySpan<byte> ((void*) value, (int) valueLen);
			var key = Marshal.PtrToStringAuto (keyPointer);
#if NET
			return del (key, found, mValue) ? (byte) 1 : (byte) 0;
#else
			if (del is NWTxtRecordApplyDelegate apply) {
				apply (key, found, mValue);
				return (byte) 1;
			}
			if (del is NWTxtRecordApplyDelegate2 apply2)
				return apply2 (key, found, mValue) ? (byte) 1 : (byte) 0; ;

			return (byte) 0;
#endif
		}

#if !NET
		[Obsolete ("Use the overload that takes an NWTxtRecordApplyDelegate2 instead.")]
#endif
		[BindingImpl (BindingImplOptions.Optimizable)]
		public bool Apply (NWTxtRecordApplyDelegate handler)
		{
			if (handler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));

			unsafe {
#if NET
				delegate* unmanaged<IntPtr, IntPtr, NWTxtRecordFindKey, IntPtr, nuint, byte> trampoline = &TrampolineApplyHandler;
				using var block = new BlockLiteral (trampoline, handler, typeof (NWTxtRecord), nameof (TrampolineApplyHandler));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_ApplyHandler, handler);
#endif
				return nw_txt_record_apply (GetCheckedHandle (), &block);
			}
		}

#if !NET
		[BindingImpl (BindingImplOptions.Optimizable)]
		public bool Apply (NWTxtRecordApplyDelegate2 handler)
		{
			if (handler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));

			unsafe {
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_ApplyHandler, handler);
				return nw_txt_record_apply (GetCheckedHandle (), &block);
			}
		}
#endif

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern unsafe bool nw_txt_record_access_key (OS_nw_txt_record txt_record, IntPtr key, BlockLiteral* access_value);

#if !NET
		unsafe delegate void nw_txt_record_access_key_t (IntPtr IntPtr, IntPtr key, NWTxtRecordFindKey found, IntPtr value, nuint valueLen);
		unsafe static nw_txt_record_access_key_t static_AccessKeyHandler = TrampolineAccessKeyHandler;
#endif

		public delegate void NWTxtRecordGetValueDelegete (string? key, NWTxtRecordFindKey result, ReadOnlySpan<byte> value);

#if !NET
		[MonoPInvokeCallback (typeof (nw_txt_record_access_key_t))]
#else
		[UnmanagedCallersOnly]
#endif
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
#if NET
				delegate* unmanaged<IntPtr, IntPtr, NWTxtRecordFindKey, IntPtr, nuint, void> trampoline = &TrampolineAccessKeyHandler;
				using var block = new BlockLiteral (trampoline, handler, typeof (NWTxtRecord), nameof (TrampolineAccessKeyHandler));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_AccessKeyHandler, handler);
#endif
				using var keyPtr = new TransientString (key);
				return nw_txt_record_access_key (GetCheckedHandle (), keyPtr, &block);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		unsafe static extern bool nw_txt_record_access_bytes (OS_nw_txt_record txt_record, BlockLiteral* access_bytes);

#if !NET
		unsafe delegate void nw_txt_record_access_bytes_t (IntPtr block, IntPtr value, nuint valueLen);
		unsafe static nw_txt_record_access_bytes_t static_RawBytesHandler = TrampolineRawBytesHandler;
#endif

		public delegate void NWTxtRecordGetRawByteDelegate (ReadOnlySpan<byte> value);

#if !NET
		[MonoPInvokeCallback (typeof (nw_txt_record_access_bytes_t))]
#else
		[UnmanagedCallersOnly]
#endif
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
#if NET
				delegate* unmanaged<IntPtr, IntPtr, nuint, void> trampoline = &TrampolineRawBytesHandler;
				using var block = new BlockLiteral (trampoline, handler, typeof (NWTxtRecord), nameof (TrampolineRawBytesHandler));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_RawBytesHandler, handler);
#endif
				return nw_txt_record_access_bytes (GetCheckedHandle (), &block);
			}
		}
	}
}
