//
// NWBrowseDescriptor.cs: Bindings the Network nw_browse_descriptor_t API.
//
// Authors:
//   Manuel de la Pena (mandel@microsoft.com)
//
// Copyrigh 2019 Microsoft Inc
//
#nullable enable

using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using ObjCRuntime;
using Foundation;
using CoreFoundation;

using OS_nw_browse_descriptor=System.IntPtr;

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
	public class NWBrowserDescriptor: NativeObject {

		[Preserve (Conditional = true)]
		internal NWBrowserDescriptor (NativeHandle handle, bool owns) : base (handle, owns) {}

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_browse_descriptor nw_browse_descriptor_create_bonjour_service (string type, string? domain);

		public static NWBrowserDescriptor CreateBonjourService (string type, string? domain)
		{
			// domain can be null, type CANNOT	
			if (type == null)
				throw new ArgumentNullException (nameof (type));

			return new NWBrowserDescriptor (nw_browse_descriptor_create_bonjour_service (type, domain), owns: true);
		}

		public static NWBrowserDescriptor CreateBonjourService (string type) => CreateBonjourService (type, null);

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool nw_browse_descriptor_get_include_txt_record (OS_nw_browse_descriptor descriptor);

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_browse_descriptor_set_include_txt_record (OS_nw_browse_descriptor descriptor, [MarshalAs (UnmanagedType.I1)] bool include_txt_record);

		public bool IncludeTxtRecord {
			get => nw_browse_descriptor_get_include_txt_record (GetCheckedHandle ()); 
			set => nw_browse_descriptor_set_include_txt_record (GetCheckedHandle (), value); 
		}

		[DllImport (Constants.NetworkLibrary, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		static extern IntPtr nw_browse_descriptor_get_bonjour_service_type (OS_nw_browse_descriptor descriptor);

		public string? BonjourType
			=> Marshal.PtrToStringAnsi (nw_browse_descriptor_get_bonjour_service_type (GetCheckedHandle ())); 

		[DllImport (Constants.NetworkLibrary, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		static extern IntPtr nw_browse_descriptor_get_bonjour_service_domain (OS_nw_browse_descriptor descriptor);

		public string? BonjourDomain
			=> Marshal.PtrToStringAnsi (nw_browse_descriptor_get_bonjour_service_domain (GetCheckedHandle ())); 
	}
}
