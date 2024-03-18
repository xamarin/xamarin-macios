//
// GCExtendedGamepadSnapshot.cs: extensions to GCExtendedGamepadSnapshot iOS API
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//   TJ Lambert (t-anlamb@microsoft.com)
//
// Copyright 2013-2014 Xamarin Inc.
// Copyright 2019 Microsoft Corporation

#nullable enable

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;

namespace GameController {
	// GCExtendedGamepadSnapshot.h
	// float_t are 4 bytes (at least for ARM64)
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("maccatalyst")]
	[ObsoletedOSPlatform ("macos10.14.4", "Use 'GCExtendedGamepadSnapshotData' instead.")]
	[ObsoletedOSPlatform ("tvos12.2", "Use 'GCExtendedGamepadSnapshotData' instead.")]
	[ObsoletedOSPlatform ("ios12.2", "Use 'GCExtendedGamepadSnapshotData' instead.")]
	[ObsoletedOSPlatform ("maccatalyst13.1", "Use 'GCExtendedGamepadSnapshotData' instead.")]
#else
	[Deprecated (PlatformName.iOS, 12, 2, message: "Use 'GCExtendedGamepadSnapshotData' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 14, 4, message: "Use 'GCExtendedGamepadSnapshotData' instead.")]
	[Deprecated (PlatformName.TvOS, 12, 2, message: "Use 'GCExtendedGamepadSnapshotData' instead.")]
#endif
	[StructLayout (LayoutKind.Sequential, Pack = 1)]

	public struct GCExtendedGamepadSnapShotDataV100 {

		// Standard information
		public ushort /* uint16_t */ Version; // 0x0100
		public ushort /* uint16_t */ Size;    // sizeof(GCExtendedGamepadSnapShotDataV100) or larger

		// Extended gamepad data
		// Axes in the range [-1.0, 1.0]
		public float /* float_t = float */ DPadX;
		public float /* float_t = float */ DPadY;

		// Buttons in the range [0.0, 1.0]
		public float /* float_t = float */ ButtonA;
		public float /* float_t = float */ ButtonB;
		public float /* float_t = float */ ButtonX;
		public float /* float_t = float */ ButtonY;
		public float /* float_t = float */ LeftShoulder;
		public float /* float_t = float */ RightShoulder;

		// Axes in the range [-1.0, 1.0]
		public float /* float_t = float */ LeftThumbstickX;
		public float /* float_t = float */ LeftThumbstickY;
		public float /* float_t = float */ RightThumbstickX;
		public float /* float_t = float */ RightThumbstickY;

		// Buttons in the range [0.0, 1.0]
		public float /* float_t = float */ LeftTrigger;
		public float /* float_t = float */ RightTrigger;

		// radar: https://trello.com/c/7FoGTORD (GCExtendedGamepadSnapShotDataV100 struct size / alignment not backward compatible)
		// [TV (12, 1), iOS (12, 1)]
		// public bool LeftThumbstickButton;
		// [TV (12, 1), iOS (12, 1)]
		// public bool RightThumbstickButton;

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("macos10.15", "Use 'GCController.GetExtendedGamepadController()' instead.")]
		[ObsoletedOSPlatform ("tvos13.0", "Use 'GCController.GetExtendedGamepadController()' instead.")]
		[ObsoletedOSPlatform ("ios13.0", "Use 'GCController.GetExtendedGamepadController()' instead.")]
		[ObsoletedOSPlatform ("maccatalyst13.1", "Use 'GCExtendedGamepadSnapshotData' instead.")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'GCController.GetExtendedGamepadController()' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'GCController.GetExtendedGamepadController()' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'GCController.GetExtendedGamepadController()' instead.")]
#endif
		[DllImport (Constants.GameControllerLibrary)]
		static extern /* NSData * __nullable */ IntPtr NSDataFromGCExtendedGamepadSnapShotDataV100 (
			/* GCExtendedGamepadSnapShotDataV100 * __nullable */ ref GCExtendedGamepadSnapShotDataV100 snapshotData);

		public NSData? ToNSData ()
		{
			var p = NSDataFromGCExtendedGamepadSnapShotDataV100 (ref this);
			return p == IntPtr.Zero ? null : new NSData (p);
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("maccatalyst")]
	[ObsoletedOSPlatform ("macos10.15", "Use 'GCController.GetExtendedGamepadController()' instead.")]
	[ObsoletedOSPlatform ("tvos13.0", "Use 'GCController.GetExtendedGamepadController()' instead.")]
	[ObsoletedOSPlatform ("ios13.0", "Use 'GCController.GetExtendedGamepadController()' instead.")]
	[ObsoletedOSPlatform ("maccatalyst13.1", "Use 'GCExtendedGamepadSnapshotData' instead.")]
#else
	[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'GCController.GetExtendedGamepadController()' instead.")]
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'GCController.GetExtendedGamepadController()' instead.")]
	[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'GCController.GetExtendedGamepadController()' instead.")]
#endif
	// float_t are 4 bytes (at least for ARM64)
	[StructLayout (LayoutKind.Sequential, Pack = 1)]
	public struct GCExtendedGamepadSnapshotData {

		// Standard information
		public ushort /* uint16_t */ Version;
		public ushort /* uint16_t */ Size;

		// Extended gamepad data
		// Axes in the range [-1.0, 1.0]
		public float /* float_t = float */ DPadX;
		public float /* float_t = float */ DPadY;

		// Buttons in the range [0.0, 1.0]
		public float /* float_t = float */ ButtonA;
		public float /* float_t = float */ ButtonB;
		public float /* float_t = float */ ButtonX;
		public float /* float_t = float */ ButtonY;
		public float /* float_t = float */ LeftShoulder;
		public float /* float_t = float */ RightShoulder;

		// Axes in the range [-1.0, 1.0]
		public float /* float_t = float */ LeftThumbstickX;
		public float /* float_t = float */ LeftThumbstickY;
		public float /* float_t = float */ RightThumbstickX;
		public float /* float_t = float */ RightThumbstickY;

		// Buttons in the range [0.0, 1.0]
		public float /* float_t = float */ LeftTrigger;
		public float /* float_t = float */ RightTrigger;

#if NET
		[SupportedOSPlatform ("ios12.2")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos12.2")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("macos10.15", "Use 'GCController.GetExtendedGamepadController()' instead.")]
		[ObsoletedOSPlatform ("tvos13.0", "Use 'GCController.GetExtendedGamepadController()' instead.")]
		[ObsoletedOSPlatform ("ios13.0", "Use 'GCController.GetExtendedGamepadController()' instead.")]
		[ObsoletedOSPlatform ("maccatalyst13.1", "Use 'GCExtendedGamepadSnapshotData' instead.")]
#else
		[iOS (12, 2)]
		[TV (12, 2)]
#endif
		[MarshalAs (UnmanagedType.I1)]
		public bool SupportsClickableThumbsticks;

#if NET
		[SupportedOSPlatform ("tvos12.2")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios12.2")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("macos10.15", "Use 'GCController.GetExtendedGamepadController()' instead.")]
		[ObsoletedOSPlatform ("tvos13.0", "Use 'GCController.GetExtendedGamepadController()' instead.")]
		[ObsoletedOSPlatform ("ios13.0", "Use 'GCController.GetExtendedGamepadController()' instead.")]
		[ObsoletedOSPlatform ("maccatalyst13.1", "Use 'GCExtendedGamepadSnapshotData' instead.")]
#else
		[TV (12, 2)]
		[iOS (12, 2)]
#endif
		[MarshalAs (UnmanagedType.I1)]
		bool LeftThumbstickButton;

#if NET
		[SupportedOSPlatform ("tvos12.2")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios12.2")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("macos10.15", "Use 'GCController.GetExtendedGamepadController()' instead.")]
		[ObsoletedOSPlatform ("tvos13.0", "Use 'GCController.GetExtendedGamepadController()' instead.")]
		[ObsoletedOSPlatform ("ios13.0", "Use 'GCController.GetExtendedGamepadController()' instead.")]
		[ObsoletedOSPlatform ("maccatalyst13.1", "Use 'GCExtendedGamepadSnapshotData' instead.")]
#else
		[TV (12, 2)]
		[iOS (12, 2)]
#endif
		[MarshalAs (UnmanagedType.I1)]
		bool RightThumbstickButton;

#if NET
		[SupportedOSPlatform ("tvos12.2")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios12.2")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("macos10.15", "Use 'GCController.GetExtendedGamepadController()' instead.")]
		[ObsoletedOSPlatform ("tvos13.0", "Use 'GCController.GetExtendedGamepadController()' instead.")]
		[ObsoletedOSPlatform ("ios13.0", "Use 'GCController.GetExtendedGamepadController()' instead.")]
		[ObsoletedOSPlatform ("maccatalyst13.1", "Use 'GCExtendedGamepadSnapshotData' instead.")]
#else
		[TV (12, 2)]
		[iOS (12, 2)]
#endif
		[DllImport (Constants.GameControllerLibrary)]
		static extern /* NSData * __nullable */ IntPtr NSDataFromGCExtendedGamepadSnapshotData (
			/* GCExtendedGamepadSnapshotData * __nullable */ ref GCExtendedGamepadSnapshotData snapshotData);

#if NET
		[SupportedOSPlatform ("tvos12.2")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios12.2")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("macos10.15", "Use 'GCController.GetExtendedGamepadController()' instead.")]
		[ObsoletedOSPlatform ("tvos13.0", "Use 'GCController.GetExtendedGamepadController()' instead.")]
		[ObsoletedOSPlatform ("ios13.0", "Use 'GCController.GetExtendedGamepadController()' instead.")]
		[ObsoletedOSPlatform ("maccatalyst13.1", "Use 'GCExtendedGamepadSnapshotData' instead.")]
#else
		[TV (12, 2)]
		[iOS (12, 2)]
#endif
		public NSData? ToNSData ()
		{
			var p = NSDataFromGCExtendedGamepadSnapshotData (ref this);
			return p == IntPtr.Zero ? null : new NSData (p);
		}
	}

	public partial class GCExtendedGamepadSnapshot {

		// GCExtendedGamepadSnapshot.h
		[DllImport (Constants.GameControllerLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool GCExtendedGamepadSnapShotDataV100FromNSData (
			/* GCExtendedGamepadSnapShotDataV100 * __nullable */ out GCExtendedGamepadSnapShotDataV100 snapshotData,
			/* NSData * __nullable */ IntPtr data);

#if NET
		[SupportedOSPlatform ("tvos12.2")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios12.2")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("macos10.15", "Use 'GCController.GetExtendedGamepadController()' instead.")]
		[ObsoletedOSPlatform ("tvos13.0", "Use 'GCController.GetExtendedGamepadController()' instead.")]
		[ObsoletedOSPlatform ("ios13.0", "Use 'GCController.GetExtendedGamepadController()' instead.")]
		[ObsoletedOSPlatform ("maccatalyst13.1")]
#else
		[TV (12, 2)]
		[iOS (12, 2)]
#endif
		[DllImport (Constants.GameControllerLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool GCExtendedGamepadSnapshotDataFromNSData (
			/* GCExtendedGamepadSnapshotData * __nullable */ out GCExtendedGamepadSnapshotData snapshotData,
			/* NSData * __nullable */ IntPtr data);

		public static bool TryGetSnapShotData (NSData? data, out GCExtendedGamepadSnapShotDataV100 snapshotData)
		{
			return GCExtendedGamepadSnapShotDataV100FromNSData (out snapshotData, data.GetHandle ());
		}

#if NET
		[SupportedOSPlatform ("tvos12.2")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios12.2")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("macos10.15", "Use 'GCController.GetExtendedGamepadController()' instead.")]
		[ObsoletedOSPlatform ("tvos13.0", "Use 'GCController.GetExtendedGamepadController()' instead.")]
		[ObsoletedOSPlatform ("ios13.0", "Use 'GCController.GetExtendedGamepadController()' instead.")]
		[ObsoletedOSPlatform ("maccatalyst13.1")]
#else
		[TV (12, 2)]
		[iOS (12, 2)]
#endif
		public static bool TryGetExtendedSnapShotData (NSData? data, out GCExtendedGamepadSnapshotData snapshotData)
		{
			return GCExtendedGamepadSnapshotDataFromNSData (out snapshotData, data.GetHandle ());
		}
	}
}
