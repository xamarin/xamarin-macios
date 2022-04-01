// Copyright 2015 Xamarin Inc.

#nullable enable

#if !WATCHOS

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;

namespace GameController {

	// GCMicroGamepadSnapshot.h
	// float_t are 4 bytes (at least for ARM64)
#if NET
	[SupportedOSPlatform ("ios10.0")]
	[SupportedOSPlatform ("macos10.12")]
	[UnsupportedOSPlatform ("macos10.14.4")]
	[UnsupportedOSPlatform ("tvos12.2")]
	[UnsupportedOSPlatform ("ios12.2")]
#if MONOMAC
	[Obsolete ("Starting with macos10.14.4 use 'GCController.GetMicroGamepadController()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
	[Obsolete ("Starting with tvos12.2 use 'GCController.GetMicroGamepadController()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
	[Obsolete ("Starting with ios12.2 use 'GCController.GetMicroGamepadController()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
	[iOS (10,0)]
	[Mac (10,12)]
	[Deprecated (PlatformName.iOS, 12, 2, message: "Use 'GCController.GetMicroGamepadController()' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 14, 4, message: "Use 'GCController.GetMicroGamepadController()' instead.")]
	[Deprecated (PlatformName.TvOS, 12, 2, message: "Use 'GCController.GetMicroGamepadController()' instead.")]
#endif
	[StructLayout (LayoutKind.Sequential, Pack = 1)]
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

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("macos10.12")]
		[UnsupportedOSPlatform ("macos10.15")]
		[UnsupportedOSPlatform ("tvos13.0")]
		[UnsupportedOSPlatform ("ios13.0")]
#if MONOMAC
		[Obsolete ("Starting with macos10.15 use 'GCController.GetMicroGamepadController()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
		[Obsolete ("Starting with tvos13.0 use 'GCController.GetMicroGamepadController()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios13.0 use 'GCController.GetMicroGamepadController()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'GCController.GetMicroGamepadController()' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'GCController.GetMicroGamepadController()' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'GCController.GetMicroGamepadController()' instead.")]
#endif
		[DllImport (Constants.GameControllerLibrary)]
		static extern /* NSData * __nullable */ IntPtr NSDataFromGCMicroGamepadSnapShotDataV100 (
			/* __nullable */ ref GCMicroGamepadSnapShotDataV100 snapshotData);

		public NSData? ToNSData ()
		{
			var p = NSDataFromGCMicroGamepadSnapShotDataV100 (ref this);
			return p == IntPtr.Zero ? null : new NSData (p);
		}
	}

	// GCMicroGamepadSnapshot.h
	// float_t are 4 bytes (at least for ARM64)
#if NET
	[UnsupportedOSPlatform ("macos10.15")]
	[UnsupportedOSPlatform ("tvos13.0")]
	[UnsupportedOSPlatform ("ios13.0")]
#if MONOMAC
	[Obsolete ("Starting with macos10.15 use 'GCController.GetMicroGamepadController()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
	[Obsolete ("Starting with tvos13.0 use 'GCController.GetMicroGamepadController()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
	[Obsolete ("Starting with ios13.0 use 'GCController.GetMicroGamepadController()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
	[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'GCController.GetMicroGamepadController()' instead.")]
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'GCController.GetMicroGamepadController()' instead.")]
	[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'GCController.GetMicroGamepadController()' instead.")]
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

#if NET
		[SupportedOSPlatform ("tvos12.2")]
		[SupportedOSPlatform ("macos10.14.4")]
		[SupportedOSPlatform ("ios12.2")]
		[UnsupportedOSPlatform ("macos10.15")]
		[UnsupportedOSPlatform ("tvos13.0")]
		[UnsupportedOSPlatform ("ios13.0")]
#if MONOMAC
		[Obsolete ("Starting with macos10.15 use 'GCController.GetMicroGamepadController()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
		[Obsolete ("Starting with tvos13.0 use 'GCController.GetMicroGamepadController()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios13.0 use 'GCController.GetMicroGamepadController()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[TV (12, 2)]
		[Mac (10, 14, 4)]
		[iOS (12, 2)]
#endif
		[DllImport (Constants.GameControllerLibrary)]
		static extern /* NSData * __nullable */ IntPtr NSDataFromGCMicroGamepadSnapshotData (
			/* __nullable */ ref GCMicroGamepadSnapshotData snapshotData);

#if NET
		[SupportedOSPlatform ("tvos12.2")]
		[SupportedOSPlatform ("macos10.14.4")]
		[SupportedOSPlatform ("ios12.2")]
		[UnsupportedOSPlatform ("macos10.15")]
		[UnsupportedOSPlatform ("tvos13.0")]
		[UnsupportedOSPlatform ("ios13.0")]
#if MONOMAC
		[Obsolete ("Starting with macos10.15 use 'GCController.GetMicroGamepadController()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
		[Obsolete ("Starting with tvos13.0 use 'GCController.GetMicroGamepadController()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios13.0 use 'GCController.GetMicroGamepadController()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[TV (12, 2)]
		[Mac (10, 14, 4)]
		[iOS (12, 2)]
#endif
		public NSData? ToNSData ()
		{
			var p = NSDataFromGCMicroGamepadSnapshotData (ref this);
			return p == IntPtr.Zero ? null : new NSData (p);
		}
	}


	public partial class GCMicroGamepadSnapshot {

		// GCGamepadSnapshot.h
#if NET
		[SupportedOSPlatform ("macos10.12")]
		[SupportedOSPlatform ("ios10.0")]
		[UnsupportedOSPlatform ("macos10.15")]
		[UnsupportedOSPlatform ("tvos13.0")]
		[UnsupportedOSPlatform ("ios13.0")]
#if MONOMAC
		[Obsolete ("Starting with macos10.15 use 'GCController.GetMicroGamepadController()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
		[Obsolete ("Starting with tvos13.0 use 'GCController.GetMicroGamepadController()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios13.0 use 'GCController.GetMicroGamepadController()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'GCController.GetMicroGamepadController()' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'GCController.GetMicroGamepadController()' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'GCController.GetMicroGamepadController()' instead.")]
#endif
		[DllImport (Constants.GameControllerLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool GCMicroGamepadSnapShotDataV100FromNSData (out GCMicroGamepadSnapShotDataV100 snapshotData, /* NSData */ IntPtr data);

#if NET
		[SupportedOSPlatform ("macos10.12")]
		[SupportedOSPlatform ("ios10.0")]
		[UnsupportedOSPlatform ("macos10.14.4")]
		[UnsupportedOSPlatform ("tvos12.2")]
		[UnsupportedOSPlatform ("ios12.2")]
#if MONOMAC
		[Obsolete ("Starting with macos10.14.4 use 'TryGetSnapshotData (NSData, out GCMicroGamepadSnapshotData)' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
		[Obsolete ("Starting with tvos12.2 use 'TryGetSnapshotData (NSData, out GCMicroGamepadSnapshotData)' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios12.2 use 'TryGetSnapshotData (NSData, out GCMicroGamepadSnapshotData)' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.iOS, 12, 2, message: "Use 'TryGetSnapshotData (NSData, out GCMicroGamepadSnapshotData)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 14, 4, message: "Use 'TryGetSnapshotData (NSData, out GCMicroGamepadSnapshotData)' instead.")]
		[Deprecated (PlatformName.TvOS, 12, 2, message: "Use 'TryGetSnapshotData (NSData, out GCMicroGamepadSnapshotData)' instead.")]
#endif
		public static bool TryGetSnapshotData (NSData data, out GCMicroGamepadSnapShotDataV100 snapshotData)
		{
			return GCMicroGamepadSnapShotDataV100FromNSData (out snapshotData, data.GetHandle ());
		}
		
#if NET
		[SupportedOSPlatform ("tvos12.2")]
		[SupportedOSPlatform ("macos10.14.4")]
		[SupportedOSPlatform ("ios12.2")]
		[UnsupportedOSPlatform ("macos10.15")]
		[UnsupportedOSPlatform ("tvos13.0")]
		[UnsupportedOSPlatform ("ios13.0")]
#if MONOMAC
		[Obsolete ("Starting with macos10.15 use 'GCController.GetMicroGamepadController()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
		[Obsolete ("Starting with tvos13.0 use 'GCController.GetMicroGamepadController()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios13.0 use 'GCController.GetMicroGamepadController()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'GCController.GetMicroGamepadController()' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'GCController.GetMicroGamepadController()' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'GCController.GetMicroGamepadController()' instead.")]
		[TV (12, 2)]
		[Mac (10, 14, 4)]
		[iOS (12, 2)]
#endif
		[DllImport (Constants.GameControllerLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool GCMicroGamepadSnapshotDataFromNSData (out GCMicroGamepadSnapshotData snapshotData, /* NSData */ IntPtr data);

#if NET
		[SupportedOSPlatform ("tvos12.2")]
		[SupportedOSPlatform ("macos10.14.4")]
		[SupportedOSPlatform ("ios12.2")]
		[UnsupportedOSPlatform ("macos10.15")]
		[UnsupportedOSPlatform ("tvos13.0")]
		[UnsupportedOSPlatform ("ios13.0")]
#if MONOMAC
		[Obsolete ("Starting with macos10.15 use 'GCController.Capture()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
		[Obsolete ("Starting with tvos13.0 use 'GCController.Capture()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios13.0 use 'GCController.Capture()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[TV (12, 2)]
		[Mac (10, 14, 4)]
		[iOS (12, 2)]
#endif
		public static bool TryGetSnapshotData (NSData data, out GCMicroGamepadSnapshotData snapshotData)
		{
			return GCMicroGamepadSnapshotDataFromNSData (out snapshotData, data.GetHandle ());
		}

	}
}

#endif 
