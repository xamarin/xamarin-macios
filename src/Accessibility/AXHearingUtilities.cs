//
// Copyright 2021 Microsoft Corp
//
// Authors:
//	Rachel Kang (rachelkang@microsoft.com)
//

#nullable enable

#if !TVOS && !MONOMAC

using System;
using System.Runtime.InteropServices;
using CoreGraphics;
using Foundation;
using ObjCRuntime;

namespace Accessibility {

	// accessibility.cs already provide the following attributes on the type
	// [iOS (15,0), NoMac, NoTV, Watch (8,0), MacCatalyst (15,0)]
	public static partial class AXHearingUtilities {

		[DllImport (Constants.AccessibilityLibrary)]
		static extern /* AXHearingDeviceEar */ nint AXMFiHearingDeviceStreamingEar ();

		[DllImport (Constants.AccessibilityLibrary)]
		static extern /* NSArray<NSUUID *> * */ IntPtr AXMFiHearingDevicePairedUUIDs ();

		[DllImport (Constants.AccessibilityLibrary, EntryPoint = "AXSupportsBidirectionalAXMFiHearingDeviceStreaming")]
		[return: MarshalAs (UnmanagedType.I1)]
		public static extern bool SupportsBidirectionalStreaming ();

		public static AXHearingDeviceEar GetMFiHearingDeviceStreamingEar ()
		{
			return (AXHearingDeviceEar) (long) AXMFiHearingDeviceStreamingEar ();
		}

		public static NSUuid [] GetMFiHearingDevicePairedUuids ()
		{
			return NSArray.ArrayFromHandle<NSUuid> (AXMFiHearingDevicePairedUUIDs ());
		}
	}
}
#endif
