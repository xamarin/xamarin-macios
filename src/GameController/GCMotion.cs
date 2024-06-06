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

#if !XAMCORE_5_0
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

		internal GCDualSenseAdaptiveTriggerPositionalAmplitudes_Blittable ToBlittable ()
		{
			return new GCDualSenseAdaptiveTriggerPositionalAmplitudes_Blittable (Values);
		}
	}
#endif // !XAMCORE_5_0

	[StructLayout (LayoutKind.Sequential)]
#if XAMCORE_5_0
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
	public struct GCDualSenseAdaptiveTriggerPositionalAmplitudes {
#else
#if COREBUILD
	public
#endif
	struct GCDualSenseAdaptiveTriggerPositionalAmplitudes_Blittable {
#endif
		float value0;
		float value1;
		float value2;
		float value3;
		float value4;
		float value5;
		float value6;
		float value7;
		float value8;
		float value9;
		const int DiscretePositionCount = 10; // From GCDualSenseAdaptiveTrigger.h

		public float [] Values {
			get {
				return new float [] {
					value0, value1, value2, value3, value4,
					value5, value6, value7, value8, value9,
				};
			}
			set {
				if (value.Length > DiscretePositionCount)
					throw new ArgumentOutOfRangeException ($"The array length can't be greater than {DiscretePositionCount}.", nameof (value));

				value0 = value.Length >= 1 ? value [0] : 0;
				value1 = value.Length >= 2 ? value [1] : 0;
				value2 = value.Length >= 3 ? value [2] : 0;
				value3 = value.Length >= 4 ? value [3] : 0;
				value4 = value.Length >= 5 ? value [4] : 0;
				value5 = value.Length >= 6 ? value [5] : 0;
				value6 = value.Length >= 7 ? value [6] : 0;
				value7 = value.Length >= 8 ? value [7] : 0;
				value8 = value.Length >= 9 ? value [8] : 0;
				value9 = value.Length >= 10 ? value [9] : 0;
			}
		}

#if XAMCORE_5_0
		public GCDualSenseAdaptiveTriggerPositionalAmplitudes (float [] values)
#else
		public GCDualSenseAdaptiveTriggerPositionalAmplitudes_Blittable (float [] values)
#endif
		{
			if (values is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (values));

			value0 = values.Length >= 1 ? values [0] : 0;
			value1 = values.Length >= 2 ? values [1] : 0;
			value2 = values.Length >= 3 ? values [2] : 0;
			value3 = values.Length >= 4 ? values [3] : 0;
			value4 = values.Length >= 5 ? values [4] : 0;
			value5 = values.Length >= 6 ? values [5] : 0;
			value6 = values.Length >= 7 ? values [6] : 0;
			value7 = values.Length >= 8 ? values [7] : 0;
			value8 = values.Length >= 9 ? values [8] : 0;
			value9 = values.Length >= 10 ? values [9] : 0;
		}
	}

#if !XAMCORE_5_0
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

		internal GCDualSenseAdaptiveTriggerPositionalResistiveStrengths_Blittable ToBlittable ()
		{
			return new GCDualSenseAdaptiveTriggerPositionalResistiveStrengths_Blittable (Values);
		}
	}
#endif // !XAMCORE_5_0

	[StructLayout (LayoutKind.Sequential)]
#if XAMCORE_5_0
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
	public struct GCDualSenseAdaptiveTriggerPositionalResistiveStrengths {
#else
#if COREBUILD
	public
#endif
	struct GCDualSenseAdaptiveTriggerPositionalResistiveStrengths_Blittable {
#endif
		float value0;
		float value1;
		float value2;
		float value3;
		float value4;
		float value5;
		float value6;
		float value7;
		float value8;
		float value9;
		const int DiscretePositionCount = 10; // From GCDualSenseAdaptiveTrigger.h

		public float [] Values {
			get {
				return new float [] {
					value0, value1, value2, value3, value4,
					value5, value6, value7, value8, value9,
				};
			}
			set {
				if (value.Length > DiscretePositionCount)
					throw new ArgumentOutOfRangeException ($"The array length can't be greater than {DiscretePositionCount}.", nameof (value));

				value0 = value.Length >= 1 ? value [0] : 0;
				value1 = value.Length >= 2 ? value [1] : 0;
				value2 = value.Length >= 3 ? value [2] : 0;
				value3 = value.Length >= 4 ? value [3] : 0;
				value4 = value.Length >= 5 ? value [4] : 0;
				value5 = value.Length >= 6 ? value [5] : 0;
				value6 = value.Length >= 7 ? value [6] : 0;
				value7 = value.Length >= 8 ? value [7] : 0;
				value8 = value.Length >= 9 ? value [8] : 0;
				value9 = value.Length >= 10 ? value [9] : 0;
			}
		}

#if XAMCORE_5_0
		public GCDualSenseAdaptiveTriggerPositionalResistiveStrengths (float [] values)
#else
		public GCDualSenseAdaptiveTriggerPositionalResistiveStrengths_Blittable (float [] values)
#endif
		{
			if (values is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (values));

			value0 = values.Length >= 1 ? values [0] : 0;
			value1 = values.Length >= 2 ? values [1] : 0;
			value2 = values.Length >= 3 ? values [2] : 0;
			value3 = values.Length >= 4 ? values [3] : 0;
			value4 = values.Length >= 5 ? values [4] : 0;
			value5 = values.Length >= 6 ? values [5] : 0;
			value6 = values.Length >= 7 ? values [6] : 0;
			value7 = values.Length >= 8 ? values [7] : 0;
			value8 = values.Length >= 9 ? values [8] : 0;
			value9 = values.Length >= 10 ? values [9] : 0;
		}
	}
}
