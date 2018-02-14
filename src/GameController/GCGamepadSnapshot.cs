//
// GCGamepadSnapshot.cs: extensions to GCGamepadSnapshot iOS API
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
	// GCGamepadSnapshot.h
	// float_t are 4 bytes (at least for ARM64)
	[StructLayout (LayoutKind.Sequential, Pack = 1)]
	[iOS (7,0)]
	[Mac (10,9, onlyOn64: true)]
	public struct GCGamepadSnapShotDataV100 {

		// Standard information
		public ushort /* uint16_t */ Version; // 0x0100
		public ushort /* uint16_t */ Size;    // sizeof(GCGamepadSnapShotDataV100) or larger

		// Standard gamepad data
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

		[DllImport (Constants.GameControllerLibrary)]
		static extern /* NSData * __nullable */ IntPtr NSDataFromGCGamepadSnapShotDataV100 (
			/* GCGamepadSnapShotDataV100 * __nullable */ ref GCGamepadSnapShotDataV100 snapshotData);

		public NSData ToNSData ()
		{
			var p = NSDataFromGCGamepadSnapShotDataV100 (ref this);
			return p == IntPtr.Zero ? null : new NSData (p);
		}
	}

	public partial class GCGamepadSnapshot {
		
		// GCGamepadSnapshot.h
		[DllImport (Constants.GameControllerLibrary)]
		static extern bool GCGamepadSnapShotDataV100FromNSData (
			/* GCGamepadSnapShotDataV100 * __nullable */ out GCGamepadSnapShotDataV100 snapshotData,
			/* NSData * __nullable */ IntPtr data);

		public static bool TryGetSnapshotData (NSData data, out GCGamepadSnapShotDataV100 snapshotData)
		{
			return GCGamepadSnapShotDataV100FromNSData (out snapshotData, data == null ? IntPtr.Zero : data.Handle);
		}
	}
}
