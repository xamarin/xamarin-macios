using System;
using System.Runtime.Versioning;
using Foundation;
using ObjCRuntime;

namespace AudioToolbox {
#if NET
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("macos10.12")]
	[SupportedOSPlatform ("ios10.0")]
#else
	[NoWatch]
	[TV (10, 0)]
	[Mac (10, 12)]
	[iOS (10, 0)]
#endif
	[Flags]
	public enum AudioSettingsFlags : uint
	{
		ExpertParameter = (1u << 0),
		InvisibleParameter = (1u << 1),
		MetaParameter = (1u << 2),
		UserInterfaceParameter = (1u << 3),
	}

#if NET
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("macos11.0")]
	[SupportedOSPlatform ("ios14.0")]
#else
	[NoWatch]
	[TV (14, 0)]
	[Mac (11, 0)]
	[iOS (14, 0)]
#endif
	public enum AUSpatialMixerOutputType : uint
	{
		Headphones = 1,
		BuiltInSpeakers = 2,
		ExternalSpeakers = 3,
	}

#if NET
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("macos11.0")]
	[SupportedOSPlatform ("ios14.0")]
#else
	[NoWatch]
	[TV (14, 0)]
	[Mac (11, 0)]
	[iOS (14, 0)]
#endif
	public enum AUSpatialMixerPointSourceInHeadMode : uint
	{
		Mono = 0,
		Bypass = 1,
	}

#if NET
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("macos11.0")]
	[SupportedOSPlatform ("ios14.0")]
#else
	[NoWatch]
	[TV (14, 0)]
	[Mac (11, 0)]
	[iOS (14, 0)]
#endif
	public enum AUSpatialMixerSourceMode : uint
	{
		SpatializeIfMono = 0,
		Bypass = 1,
		PointSource = 2,
		AmbienceBed = 3,
	}

}
