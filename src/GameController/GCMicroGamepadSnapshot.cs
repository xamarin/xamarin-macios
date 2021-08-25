// Copyright 2015 Xamarin Inc.

#if !WATCHOS

using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using ObjCRuntime;
using Foundation;

namespace GameController {

#if !NET
	[iOS (10,0), Mac (10,12)]
#endif
	// GCMicroGamepadSnapshot.h
	// float_t are 4 bytes (at least for ARM64)
	[StructLayout (LayoutKind.Sequential, Pack = 1)]
#if !NET
	[Deprecated (PlatformName.iOS, 12, 2, message: "Use 'GCController.GetMicroGamepadController()' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 14, 4, message: "Use 'GCController.GetMicroGamepadController()' instead.")]
	[Deprecated (PlatformName.TvOS, 12, 2, message: "Use 'GCController.GetMicroGamepadController()' instead.")]
#else
	[UnsupportedOSPlatform ("ios12.2")]
	[UnsupportedOSPlatform ("tvos12.2")]
	[UnsupportedOSPlatform ("macos10.14.4")]
#if IOS
	[Obsolete ("Starting with ios12.2 Use 'GCController.GetMicroGamepadController()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
	[Obsolete ("Starting with tvos12.2 Use 'GCController.GetMicroGamepadController()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif MONOMAC
	[Obsolete ("Starting with macos10.14.4 Use 'GCController.GetMicroGamepadController()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
	public struct GCMicroGamepadSnapShotDataV100 {

		// Standard information
		public ushort /* uint16_t */ Version; // 0x0100
		public ushort /* uint16_t */ Size;    // sizeof(GCMicroGamepadSnapShotDataV100) or larger

		// Standard gamepad data
		// Axes in the range [-1.0, 1.0]
		public float /* float_t = float */ DPadX;
		public float /* float_t = float */ DPadY;

		// Buttons in the range [0.0, 1.0]
		public float /* float_t = float */ ButtonA;
		public float /* float_t = float */ ButtonX;

#if !NET
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'GCController.GetMicroGamepadController()' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'GCController.GetMicroGamepadController()' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'GCController.GetMicroGamepadController()' instead.")]
#else
		[UnsupportedOSPlatform ("ios13.0")]
		[UnsupportedOSPlatform ("tvos13.0")]
		[UnsupportedOSPlatform ("macos10.15")]
#if IOS
		[Obsolete ("Starting with ios13.0 Use 'GCController.GetMicroGamepadController()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
		[Obsolete ("Starting with tvos13.0 Use 'GCController.GetMicroGamepadController()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif MONOMAC
		[Obsolete ("Starting with macos10.15 Use 'GCController.GetMicroGamepadController()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
		[DllImport (Constants.GameControllerLibrary)]
		static extern /* NSData * __nullable */ IntPtr NSDataFromGCMicroGamepadSnapShotDataV100 (
			/* __nullable */ ref GCMicroGamepadSnapShotDataV100 snapshotData);

		public NSData ToNSData ()
		{
			var p = NSDataFromGCMicroGamepadSnapShotDataV100 (ref this);
			return p == IntPtr.Zero ? null : new NSData (p);
		}
	}

	// GCMicroGamepadSnapshot.h
	// float_t are 4 bytes (at least for ARM64)
#if !NET
	[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'GCController.GetMicroGamepadController()' instead.")]
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'GCController.GetMicroGamepadController()' instead.")]
	[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'GCController.GetMicroGamepadController()' instead.")]
#else
	[UnsupportedOSPlatform ("ios13.0")]
	[UnsupportedOSPlatform ("tvos13.0")]
	[UnsupportedOSPlatform ("macos10.15")]
#if IOS
	[Obsolete ("Starting with ios13.0 Use 'GCController.GetMicroGamepadController()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
	[Obsolete ("Starting with tvos13.0 Use 'GCController.GetMicroGamepadController()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif MONOMAC
	[Obsolete ("Starting with macos10.15 Use 'GCController.GetMicroGamepadController()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
	[StructLayout (LayoutKind.Sequential, Pack = 1)]
	public struct GCMicroGamepadSnapshotData {

		// Standard information
		public ushort /* uint16_t */ Version;
		public ushort /* uint16_t */ Size;

		// Standard gamepad data
		// Axes in the range [-1.0, 1.0]
		public float /* float_t = float */ DPadX;
		public float /* float_t = float */ DPadY;

		// Buttons in the range [0.0, 1.0]
		public float /* float_t = float */ ButtonA;
		public float /* float_t = float */ ButtonX;

		[DllImport (Constants.GameControllerLibrary)]
#if !NET
		[TV (12, 2), Mac (10, 14, 4), iOS (12, 2)]
#else
		[SupportedOSPlatform ("ios12.2")]
		[SupportedOSPlatform ("tvos12.2")]
		[SupportedOSPlatform ("macos10.14.4")]
#endif
		static extern /* NSData * __nullable */ IntPtr NSDataFromGCMicroGamepadSnapshotData (
			/* __nullable */ ref GCMicroGamepadSnapshotData snapshotData);

#if !NET
		[TV (12, 2), Mac (10, 14, 4), iOS (12, 2)]
#else
		[SupportedOSPlatform ("ios12.2")]
		[SupportedOSPlatform ("tvos12.2")]
		[SupportedOSPlatform ("macos10.14.4")]
#endif
		public NSData ToNSData ()
		{
			var p = NSDataFromGCMicroGamepadSnapshotData (ref this);
			return p == IntPtr.Zero ? null : new NSData (p);
		}
	}


	public partial class GCMicroGamepadSnapshot {

		// GCGamepadSnapshot.h
// #if !NET
// 		// [Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'GCController.GetMicroGamepadController()' instead.")]
// 		// [Deprecated (PlatformName.iOS, 13, 0, message: "Use 'GCController.GetMicroGamepadController()' instead.")]
// 		// [Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'GCController.GetMicroGamepadController()' instead.")]
// #else
// // Already provided by partial class in gamecontroller.cs
// // 		[UnsupportedOSPlatform ("ios13.0")]
// // 		[UnsupportedOSPlatform ("tvos13.0")]
// // 		[UnsupportedOSPlatform ("macos10.15")]
// #if IOS
// 		[Obsolete ("Starting with ios13.0 Use 'GCController.GetMicroGamepadController()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
// #elif TVOS
// 		[Obsolete ("Starting with tvos13.0 Use 'GCController.GetMicroGamepadController()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
// #elif MONOMAC
// 		[Obsolete ("Starting with macos10.15 Use 'GCController.GetMicroGamepadController()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
// #endif
// #endif
		[DllImport (Constants.GameControllerLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool GCMicroGamepadSnapShotDataV100FromNSData (out GCMicroGamepadSnapShotDataV100 snapshotData, /* NSData */ IntPtr data);

#if !NET
		[Deprecated (PlatformName.iOS, 12, 2, message: "Use 'TryGetSnapshotData (NSData, out GCMicroGamepadSnapshotData)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 14, 4, message: "Use 'TryGetSnapshotData (NSData, out GCMicroGamepadSnapshotData)' instead.")]
		[Deprecated (PlatformName.TvOS, 12, 2, message: "Use 'TryGetSnapshotData (NSData, out GCMicroGamepadSnapshotData)' instead.")]
#else
		[UnsupportedOSPlatform ("ios12.2")]
		[UnsupportedOSPlatform ("tvos12.2")]
		[UnsupportedOSPlatform ("macos10.14.4")]
#if IOS
		[Obsolete ("Starting with ios12.2 Use 'TryGetSnapshotData (NSData, out GCMicroGamepadSnapshotData)' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
		[Obsolete ("Starting with tvos12.2 Use 'TryGetSnapshotData (NSData, out GCMicroGamepadSnapshotData)' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif MONOMAC
		[Obsolete ("Starting with macos10.14.4 Use 'TryGetSnapshotData (NSData, out GCMicroGamepadSnapshotData)' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
		public static bool TryGetSnapshotData (NSData data, out GCMicroGamepadSnapShotDataV100 snapshotData)
		{
			return GCMicroGamepadSnapShotDataV100FromNSData (out snapshotData, data == null ? IntPtr.Zero : data.Handle);
		}
		
#if !NET
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'GCController.GetMicroGamepadController()' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'GCController.GetMicroGamepadController()' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'GCController.GetMicroGamepadController()' instead.")]
#else
// Already provided by partial class in gamecontroller.cs
// 		[UnsupportedOSPlatform ("ios13.0")]
// 		[UnsupportedOSPlatform ("tvos13.0")]
// 		[UnsupportedOSPlatform ("macos10.15")]
#if IOS
		[Obsolete ("Starting with ios13.0 Use 'GCController.GetMicroGamepadController()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
		[Obsolete ("Starting with tvos13.0 Use 'GCController.GetMicroGamepadController()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif MONOMAC
		[Obsolete ("Starting with macos10.15 Use 'GCController.GetMicroGamepadController()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
		[DllImport (Constants.GameControllerLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
#if !NET
		[TV (12, 2), Mac (10, 14, 4), iOS (12, 2)]
#else
		[SupportedOSPlatform ("ios12.2")]
		[SupportedOSPlatform ("tvos12.2")]
		[SupportedOSPlatform ("macos10.14.4")]
#endif
		static extern bool GCMicroGamepadSnapshotDataFromNSData (out GCMicroGamepadSnapshotData snapshotData, /* NSData */ IntPtr data);

#if !NET
		[TV (12, 2), Mac (10, 14, 4), iOS (12, 2)]
#else
		[SupportedOSPlatform ("ios12.2")]
		[SupportedOSPlatform ("tvos12.2")]
		[SupportedOSPlatform ("macos10.14.4")]
#endif
		public static bool TryGetSnapshotData (NSData data, out GCMicroGamepadSnapshotData snapshotData)
		{
			return GCMicroGamepadSnapshotDataFromNSData (out snapshotData, data == null ? IntPtr.Zero : data.Handle);
		}

	}
}

#endif 
