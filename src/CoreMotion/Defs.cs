//
// CoreMotion's struct and enum definitions used by the API file
//

using System;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;

namespace CoreMotion {

	// CMAccelerometer.h
	[StructLayout (LayoutKind.Sequential)]
	public struct CMAcceleration {
		public double X, Y, Z;

		public CMAcceleration (double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public override string ToString ()
		{
			return String.Format ("a=({0},{1},{2})", X, Y, Z);
		}
	}

	// CMAttitude.h
	[StructLayout (LayoutKind.Sequential)]
	public struct  CMRotationMatrix {
		public double m11, m12, m13;
		public double m21, m22, m23;
		public double m31, m32, m33;
	}

	// CMAttitude.h
	[StructLayout (LayoutKind.Sequential)]
	public struct  CMQuaternion {
		public double x, y, z, w;

		public CMQuaternion (double x, double y, double z, double w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		public override string ToString ()
		{
			return String.Format ("quaternion=({0},{1},{2},{3})", x, y, z, w);
		}
	}

	// CMGyro.h
	[StructLayout (LayoutKind.Sequential)]
	public struct CMRotationRate {
		public double x;
		public double y;
		public double z;

		public CMRotationRate (double x, double y, double z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public override string ToString ()
		{
			return String.Format ("rotationRate=({0},{1},{2}", x, y, z);
		}
	}

	// untyped enum -> CMError.h
	public enum CMError {
		Null = 100,
		DeviceRequiresMovement, 
		TrueNorthNotAvailable,
		Unknown,
		MotionActivityNotAvailable,
		MotionActivityNotAuthorized,
		MotionActivityNotEntitled,
		InvalidParameter,
		[iOS (8,2)]
		InvalidAction,
		[iOS (8,2)]
		NotAvailable,
		[iOS (8,2)]
		NotEntitled,
		[iOS (8,2)]
		NotAuthorized,
	}

	// untyped enum -> CMDeviceMotion.h
	public enum CMMagneticFieldCalibrationAccuracy {
		Uncalibrated = -1, Low, Medium, High
	}

	// untyped enum -> CMAttitude.h
	// in Xcode 6.3 SDK is became an NSUInteger
	[Flags]
	[Native]
	public enum CMAttitudeReferenceFrame : ulong {
		XArbitraryZVertical = 1 << 0,
		XArbitraryCorrectedZVertical = 1 << 1,
		XMagneticNorthZVertical = 1 << 2,
		XTrueNorthZVertical = 1 << 3
	}

	// NSInteger -> CMMotionActivity.h
	[Native]
	public enum CMMotionActivityConfidence : long {
		Low = 0,
		Medium,
		High
	}

}
