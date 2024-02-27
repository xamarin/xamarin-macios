//
// NIAlgorithmConvergenceStatusReasonDescription manual binding
//
// Authors:
//	TJ Lambert  <TJ.Lambert@microsoft.com>
//
// Copyright 2022 Microsoft Corp.
//

#if IOS || WATCH || __MACCATALYST__

#nullable enable

using System;
using System.Runtime.InteropServices;
using CoreFoundation;
using Foundation;
using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace NearbyInteraction {
	public partial class NIAlgorithmConvergenceStatusReasonValues
	{
#if NET
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
#else
		[iOS (16,0), NoMac, Watch (9,0), NoTV, MacCatalyst (16,0)]
#endif // NET
		[DllImport (Constants.NearbyInteractionLibrary)]
		static extern NativeHandle /* NSString */ NIAlgorithmConvergenceStatusReasonDescription (NativeHandle /* NIAlgorithmConvergenceStatusReason */ reason);

#if NET
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
#else
		[iOS (16,0), NoMac, Watch (9,0), NoTV, MacCatalyst (16,0)]
#endif // NET
		public static NSString GetConvergenceStatusReason (NIAlgorithmConvergenceStatusReason reason)
		{
			return Runtime.GetNSObject<NSString> (NIAlgorithmConvergenceStatusReasonDescription (reason.GetConstant ().GetHandle ()))!;
		}
	}
}
#endif // IOS || WATCH || __MACCATALYST__
