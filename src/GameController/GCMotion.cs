//
// GCMotion.cs: extensions to GCMotion iOS API
//
// Authors:
//   TJ Lambert (t-anlamb@microsoft.com)
//
// Copyright 2019 Microsoft Corporation.

#nullable enable

using System;
using System.Runtime.Versioning;
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;

namespace GameController {

#if NET
	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("maccatalyst")]
#else
	[Introduced (PlatformName.iOS, 13, 0)]
	[Introduced (PlatformName.TvOS, 13, 0)]
#endif
	public struct GCAcceleration {
		public double X;
		public double Y;
		public double Z;
	}
#if NET
	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("maccatalyst")]
#else
	[Introduced (PlatformName.iOS, 13, 0)]
	[Introduced (PlatformName.TvOS, 13, 0)]
#endif
	public struct GCRotationRate {
		public double X;
		public double Y;
		public double Z;
	}

#if NET
	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("maccatalyst")]
#else
	[Introduced (PlatformName.iOS, 13, 0)]
	[Introduced (PlatformName.TvOS, 13, 0)]
#endif
	public struct GCQuaternion {
		public double X;
		public double Y;
		public double Z;
		public double W;
	}

#if NET
	[SupportedOSPlatform ("ios15.4")]
	[SupportedOSPlatform ("maccatalyst15.4")]
	[SupportedOSPlatform ("macos12.3")]
	[SupportedOSPlatform ("tvos15.4")]
#else
	[Introduced (PlatformName.iOS, 15, 4)]
	[Introduced (PlatformName.MacOSX, 12, 3)]
	[Introduced (PlatformName.TvOS, 15, 4)]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct GCDualSenseAdaptiveTriggerPositionalAmplitudes {

		const int DiscretePositionCount = 10; // From GCDualSenseAdaptiveTrigger.h
		[MarshalAs (UnmanagedType.ByValArray, SizeConst = DiscretePositionCount)]
		public float [] Values;

		public GCDualSenseAdaptiveTriggerPositionalAmplitudes (float [] values)
		{
			if (values is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (values));
			if (values.Length > DiscretePositionCount)
				throw new ArgumentOutOfRangeException ($"The '{nameof (values)}' array length can't be greater than {DiscretePositionCount}.");

			Values = values;
		}
	}

#if NET
	[SupportedOSPlatform ("ios15.4")]
	[SupportedOSPlatform ("maccatalyst15.4")]
	[SupportedOSPlatform ("macos12.3")]
	[SupportedOSPlatform ("tvos15.4")]
#else
	[Introduced (PlatformName.iOS, 15, 4)]
	[Introduced (PlatformName.MacOSX, 12, 3)]
	[Introduced (PlatformName.TvOS, 15, 4)]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct GCDualSenseAdaptiveTriggerPositionalResistiveStrengths {

		const int DiscretePositionCount = 10; // From GCDualSenseAdaptiveTrigger.h
		[MarshalAs (UnmanagedType.ByValArray, SizeConst = DiscretePositionCount)]
		public float [] Values;

		public GCDualSenseAdaptiveTriggerPositionalResistiveStrengths (float [] values)
		{
			if (values is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (values));
			if (values.Length > DiscretePositionCount)
				throw new ArgumentOutOfRangeException ($"The '{nameof (values)}' array length can't be greater than {DiscretePositionCount}.");

			Values = values;
		}
	}
}
