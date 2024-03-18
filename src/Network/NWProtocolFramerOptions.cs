//
// NSProtocolFramerOptions : Bindings the Network nw_protocol_options API focus on Framer options.
//
// Authors:
//   Manuel de la Pena <mandel@microsoft.com>
//
// Copyright 2019 Microsoft Inc
//

#nullable enable

using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using ObjCRuntime;
using Foundation;
using CoreFoundation;
using Security;
using IntPtr = System.IntPtr;

#if !NET
using OS_nw_protocol_options = System.IntPtr;
using NativeHandle = System.IntPtr;
#else
using OS_nw_protocol_options=ObjCRuntime.NativeHandle;
#endif

namespace Network {

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
	[MacCatalyst (16, 0)]
#endif
	public class NSProtocolFramerOptions : NWProtocolOptions {

		[Preserve (Conditional = true)]
		internal NSProtocolFramerOptions (NativeHandle handle, bool owns) : base (handle, owns) { }

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_framer_options_set_object_value (OS_nw_protocol_options options, IntPtr key, NativeHandle value);

		static void nw_framer_options_set_object_value (OS_nw_protocol_options options, string key, NativeHandle value)
		{
			using var keyPtr = new TransientString (key);
			nw_framer_options_set_object_value (options, keyPtr, value);
		}

		public void SetValue<T> (string key, T? value) where T : NSObject
			=> nw_framer_options_set_object_value (GetCheckedHandle (), key, value.GetHandle ());

		[DllImport (Constants.NetworkLibrary)]
		static extern NativeHandle nw_framer_options_copy_object_value (OS_nw_protocol_options options, IntPtr key);

		public T? GetValue<T> (string key) where T : NSObject
		{
			using var keyPtr = new TransientString (key);
			var value = nw_framer_options_copy_object_value (GetCheckedHandle (), keyPtr);
			return Runtime.GetNSObject<T> (value, owns: true);
		}

	}
}
