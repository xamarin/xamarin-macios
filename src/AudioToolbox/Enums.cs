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
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	public enum AUSpatialMixerOutputType : uint {
		Headphones = 1,
		BuiltInSpeakers = 2,
		ExternalSpeakers = 3,
	}

	[NoWatch]
	[TV (14, 0)]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	public enum AUSpatialMixerPointSourceInHeadMode : uint {
		Mono = 0,
		Bypass = 1,
	}

	[NoWatch]
	[TV (14, 0)]
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
		[iOS (18, 0), TV (18, 0), MacCatalyst (18, 0)]
		Off = 0,
		[iOS (18, 0), TV (18, 0), MacCatalyst (18, 0)]
		On = 1,
		[iOS (18, 0), TV (18, 0), MacCatalyst (18, 0)]
		Auto = 2,
	}

	[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0), Mac (14, 0)]
	public enum AUVoiceIOOtherAudioDuckingLevel : uint {
		Default = 0,
		Min = 10,
		Mid = 20,
		Max = 30,
	}
}
