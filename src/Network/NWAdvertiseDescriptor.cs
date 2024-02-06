//
// NWAdvertiseDescriptor.cs: Bindings the Netowrk nw_advertise_descriptor_t API
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

using nw_advertise_descriptor_t = System.IntPtr;
using OS_nw_advertise_descriptor = System.IntPtr;
using OS_nw_txt_record = System.IntPtr;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Network {
#if NET
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
#else
	[Watch (6, 0)]
#endif
	public class NWAdvertiseDescriptor : NativeObject {
		[Preserve (Conditional = true)]
#if NET
		internal NWAdvertiseDescriptor (NativeHandle handle, bool owns) : base (handle, owns)
#else
		public NWAdvertiseDescriptor (NativeHandle handle, bool owns) : base (handle, owns)
#endif
		{ }

#if NET
		[SupportedOSPlatform ("tvos16.0")]
		[SupportedOSPlatform ("macos13.0")]
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
#else
		[TV (16, 0)]
		[Mac (13, 0)]
		[iOS (16, 0)]
		[Watch (9, 0)]
#endif
		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_advertise_descriptor nw_advertise_descriptor_create_application_service (IntPtr application_service_name);

		static OS_nw_advertise_descriptor nw_advertise_descriptor_create_application_service (string application_service_name)
		{
			using var namePtr = new TransientString (application_service_name);
			return nw_advertise_descriptor_create_application_service (namePtr);
		}

#if NET
		[SupportedOSPlatform ("tvos16.0")]
		[SupportedOSPlatform ("macos13.0")]
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
#else
		[TV (16, 0)]
		[Mac (13, 0)]
		[iOS (16, 0)]
		[Watch (9, 0)]
#endif
		public NWAdvertiseDescriptor (string applicationServiceName) : base (nw_advertise_descriptor_create_application_service (applicationServiceName), true) { }

#if NET
		[SupportedOSPlatform ("tvos16.0")]
		[SupportedOSPlatform ("macos13.0")]
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
#else
		[TV (16, 0)]
		[Mac (13, 0)]
		[iOS (16, 0)]
		[Watch (9, 0)]
#endif
		[DllImport (Constants.NetworkLibrary)]
		static extern IntPtr nw_advertise_descriptor_get_application_service_name (OS_nw_advertise_descriptor advertise_descriptor);

#if NET
		[SupportedOSPlatform ("tvos16.0")]
		[SupportedOSPlatform ("macos13.0")]
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
#else
		[TV (16, 0)]
		[Mac (13, 0)]
		[iOS (16, 0)]
		[Watch (9, 0)]
#endif
		public string? ApplicationServiceName {
			get {
				var appNamePtr = nw_advertise_descriptor_get_application_service_name (GetCheckedHandle ());
				return Marshal.PtrToStringAnsi (appNamePtr);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern IntPtr nw_advertise_descriptor_create_bonjour_service (IntPtr name, IntPtr type, IntPtr domain);

		public static NWAdvertiseDescriptor? CreateBonjourService (string name, string type, string? domain = null)
		{
			if (name is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (name));

			if (type is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (type));

			using var namePtr = new TransientString (name);
			using var typePtr = new TransientString (type);
			using var domainPtr = new TransientString (domain);
			var x = nw_advertise_descriptor_create_bonjour_service (namePtr, typePtr, domainPtr);
			if (x == IntPtr.Zero)
				return null;
			return new NWAdvertiseDescriptor (x, owns: true);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_advertise_descriptor_set_txt_record (IntPtr handle, IntPtr txtRecord, nuint txtLen);

		public void SetTxtRecord (string txt)
		{
			if (txt is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (txt));
			var n = System.Text.Encoding.UTF8.GetByteCount (txt);
			using var txtPtr = new TransientString (txt);
			nw_advertise_descriptor_set_txt_record (GetCheckedHandle (), txtPtr, (nuint) n);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_advertise_descriptor_set_no_auto_rename (IntPtr handle, [MarshalAs (UnmanagedType.I1)] bool no_auto_rename);

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool nw_advertise_descriptor_get_no_auto_rename (IntPtr handle);

		public bool NoAutoRename {
			set => nw_advertise_descriptor_set_no_auto_rename (GetCheckedHandle (), value);
			get => nw_advertise_descriptor_get_no_auto_rename (GetCheckedHandle ());
		}

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[TV (13, 0)]
		[iOS (13, 0)]
#endif
		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_txt_record nw_advertise_descriptor_copy_txt_record_object (OS_nw_advertise_descriptor advertise_descriptor);

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[TV (13, 0)]
		[iOS (13, 0)]
#endif
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_advertise_descriptor_set_txt_record_object (OS_nw_advertise_descriptor advertise_descriptor, OS_nw_txt_record txt_record);

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[TV (13, 0)]
		[iOS (13, 0)]
#endif
		public NWTxtRecord TxtRecord {
			get => new NWTxtRecord (nw_advertise_descriptor_copy_txt_record_object (GetCheckedHandle ()), owns: true);
			set => nw_advertise_descriptor_set_txt_record_object (GetCheckedHandle (), value.GetHandle ());
		}
	}
}
