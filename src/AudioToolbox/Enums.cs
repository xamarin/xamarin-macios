using System;

using Foundation;

using ObjCRuntime;

#nullable enable

namespace AudioToolbox {
	[NoWatch]
	[MacCatalyst (13, 1)]
	[Flags]
	public enum AudioSettingsFlags : uint {
		ExpertParameter = (1u << 0),
		InvisibleParameter = (1u << 1),
		MetaParameter = (1u << 2),
		UserInterfaceParameter = (1u << 3),
	}

	[NoWatch]
	[TV (14, 0)]
	[Mac (11, 0)]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	public enum AUSpatialMixerOutputType : uint {
		Headphones = 1,
		BuiltInSpeakers = 2,
		ExternalSpeakers = 3,
	}

	[NoWatch]
	[TV (14, 0)]
	[Mac (11, 0)]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	public enum AUSpatialMixerPointSourceInHeadMode : uint {
		Mono = 0,
		Bypass = 1,
	}

	[NoWatch]
	[TV (14, 0)]
	[Mac (11, 0)]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	public enum AUSpatialMixerSourceMode : uint {
		SpatializeIfMono = 0,
		Bypass = 1,
		PointSource = 2,
		AmbienceBed = 3,
	}

	[NoWatch]
	[TV (16, 0)]
	[Mac (13, 0)]
	[iOS (16, 0)]
	[MacCatalyst (16, 0)]
	public enum AUSpatialMixerPersonalizedHrtfMode : uint {
		[NoiOS, NoTV]
		[NoMacCatalyst]
		Off = 0,
		[NoiOS, NoTV]
		[NoMacCatalyst]
		On = 1,
		[NoiOS, NoTV]
		[NoMacCatalyst]
		Auto = 2,
	}

}
