//
// GCExtendedGamepadSnapshot.cs: extensions to GCExtendedGamepadSnapshot iOS API
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2013-2014 Xamarin Inc.

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;

namespace GameController {
	// GCExtendedGamepadSnapshot.h
	// float_t are 4 bytes (at least for ARM64)
	[StructLayout (LayoutKind.Sequential, Pack = 1)]
	[iOS (7,0)]
	[Mac (10,9, onlyOn64: true)]
	[Deprecated (PlatformName.iOS, message: "Use 'GCExtendedGamepadSnapshotData' instead.")]
	[Deprecated (PlatformName.MacOSX, message: "Use 'GCExtendedGamepadSnapshotData' instead.")]
	[Deprecated (PlatformName.TvOS, message: "Use 'GCExtendedGamepadSnapshotData' instead.")]
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

		[DllImport (Constants.GameControllerLibrary)]
		static extern /* NSData * __nullable */ IntPtr NSDataFromGCExtendedGamepadSnapShotDataV100 (
			/* GCExtendedGamepadSnapShotDataV100 * __nullable */ ref GCExtendedGamepadSnapShotDataV100 snapshotData);

		public NSData ToNSData ()
		{
			var p = NSDataFromGCExtendedGamepadSnapShotDataV100 (ref this);
			return p == IntPtr.Zero ? null : new NSData (p);
		}
	}
	
	// float_t are 4 bytes (at least for ARM64)
	[StructLayout (LayoutKind.Sequential, Pack = 1)]
	[iOS (7,0)]
	[Mac (10,9, onlyOn64: true)]
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

		[TV (12, 2), Mac (10, 14, 4, onlyOn64: true), iOS (12, 2)]
		bool LeftThumbstickButton;
		
		[TV (12, 2), Mac (10, 14, 4, onlyOn64: true), iOS (12, 2)]
		bool RightThumbstickButton;

		[DllImport (Constants.GameControllerLibrary)]
		static extern /* NSData * __nullable */ IntPtr NSDataFromGCExtendedGamepadSnapshotData (
			/* GCExtendedGamepadSnapshotData * __nullable */ ref GCExtendedGamepadSnapshotData snapshotData);

		public NSData ToNSData ()
		{
			var p = NSDataFromGCExtendedGamepadSnapshotData (ref this);
			return p == IntPtr.Zero ? null : new NSData (p);
		}
	}

	public partial class GCExtendedGamepadSnapshot {
		
		// GCExtendedGamepadSnapshot.h
		[DllImport (Constants.GameControllerLibrary)]
		static extern bool GCExtendedGamepadSnapShotDataV100FromNSData (
			/* GCExtendedGamepadSnapShotDataV100 * __nullable */ out GCExtendedGamepadSnapShotDataV100 snapshotData, 
			/* NSData * __nullable */ IntPtr data);
		
		[DllImport (Constants.GameControllerLibrary)]
		static extern bool GCExtendedGamepadSnapshotDataFromNSData (
			/* GCExtendedGamepadSnapshotData * __nullable */ out GCExtendedGamepadSnapshotData snapshotData, 
			/* NSData * __nullable */ IntPtr data);


		[Deprecated (PlatformName.iOS, message: "Use 'TryGetExtendedSnapShotData' instead.")]
		[Deprecated (PlatformName.MacOSX, message: "Use 'TryGetExtendedSnapShotData' instead.")]
		[Deprecated (PlatformName.TvOS, message: "Use 'TryGetExtendedSnapShotData' instead.")]
		public static bool TryGetSnapShotData (NSData data, out GCExtendedGamepadSnapShotDataV100 snapshotData)
		{
			return GCExtendedGamepadSnapShotDataV100FromNSData (out snapshotData, data == null ? IntPtr.Zero : data.Handle);
		}
		
		public static bool TryGetExtendedSnapShotData (NSData data, out GCExtendedGamepadSnapshotData snapshotData)
		{
			return GCExtendedGamepadSnapshotDataFromNSData (out snapshotData, data == null ? IntPtr.Zero : data.Handle);
		}
	}
}
