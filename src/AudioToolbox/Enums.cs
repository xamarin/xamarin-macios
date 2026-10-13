
#nullable enable

namespace AudioToolbox {
	[MacCatalyst (13, 1)]
	[Flags]
	public enum AudioSettingsFlags : uint {
		ExpertParameter = (1u << 0),
		InvisibleParameter = (1u << 1),
		MetaParameter = (1u << 2),
		UserInterfaceParameter = (1u << 3),
	}

	[TV (14, 0)]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	public enum AUSpatialMixerOutputType : uint {
		Headphones = 1,
		BuiltInSpeakers = 2,
		ExternalSpeakers = 3,
	}

	[TV (14, 0)]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	public enum AUSpatialMixerPointSourceInHeadMode : uint {
		Mono = 0,
		Bypass = 1,
	}

	[TV (14, 0)]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	public enum AUSpatialMixerSourceMode : uint {
		SpatializeIfMono = 0,
		Bypass = 1,
		PointSource = 2,
		AmbienceBed = 3,
	}

	[TV (16, 0)]
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

	[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
	public enum AUVoiceIOOtherAudioDuckingLevel : uint {
		Default = 0,
		Min = 10,
		Mid = 20,
		Max = 30,
	}

	/// <summary>This enum specifies properties for <see cref="AudioUnitSubType.AUAudioMix" /> audio units.</summary>
	[Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0), NoTV]
	public enum AUAudioMixProperty {
		/// <summary>Remix the data from the file asset, using an <see cref="NSData" />.</summary>
		SpatialAudioMixMetadata = 5000,
		/// <summary>If spatialization is enabled or not. Specified with a <see langword="uint" />, which is either 0 (false) or 1 (true).</summary>
		EnableSpatialization = 5001,
	}

	/// <summary>This enum specifies parameters for <see cref="AudioUnitSubType.AUAudioMix" /> audio units.</summary>
	public enum AUAudioMixParameter {
		/// <summary>Get or set the style of the audio mixing, using a value of the <see cref="AUAudioMixRenderingStyle" /> enum.</summary>
		[Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0), NoTV]
		Style = 0,
		/// <summary>The remix amount, which is a <see langword="float" />, with valid values ranging from 0.0f to 1.0f. The default value is 0.5f.</summary>
		[Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0), NoTV]
		RemixAmount = 1,
	}

	/// <summary>This enum provides the possible values for the <see cref="AUAudioMixParameter.Style" /> parameter.</summary>
	[Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0), NoTV]
	public enum AUAudioMixRenderingStyle : uint {
		/// <summary>Render with a cinematic style. This is the default value.</summary>
		Cinematic = 0,
		/// <summary>Render with a studio style.</summary>
		Studio = 1,
		/// <summary>Render with an in-frame style.</summary>
		InFrame = 2,
		/// <summary>Render the background only with a cinematic style.</summary>
		CinematicBackgroundStem = 3,
		/// <summary>Render the foreground only with a cinematic style.</summary>
		CinematicForegroundStem = 4,
		/// <summary>Render the foreground only with a stydio style.</summary>
		StudioForegroundStem = 5,
		/// <summary>Render the foreground only with an in-frame style.</summary>
		InFrameForegroundStem = 6,
		/// <summary>Render with a standard style.</summary>
		Standard = 7,
		/// <summary>Render the background only with a studio style.</summary>
		StudioBackgroundStem = 8,
		/// <summary>Render the background only with an in-frame style.</summary>
		InFrameBackgroundStem = 9,
	}

	public enum AudioCodecPropertyId : uint {
		InputBufferSize = 0x74627566, // 'tbuf'
		PacketFrameSize = 0x70616b66, // 'pakf'
		HasVariablePacketByteSizes = 0x76706b3f, // 'vpk?'
		EmploysDependentPackets = 0x64706b3f, // 'dpk?'
		MaximumPacketByteSize = 0x70616b62, // 'pakb'
		PacketSizeLimitForVbr = 0x70616b6c, // 'pakl'
		CurrentInputFormat = 0x69666d74, // 'ifmt'
		CurrentOutputFormat = 0x6f666d74, // 'ofmt'
		MagicCookie = 0x6b756b69, // 'kuki'
		UsedInputBufferSize = 0x75627566, // 'ubuf'
		IsInitialized = 0x696e6974, // 'init'
		CurrentTargetBitRate = 0x62726174, // 'brat'
		CurrentInputSampleRate = 0x63697372, // 'cisr'
		CurrentOutputSampleRate = 0x636f7372, // 'cosr'
		QualitySetting = 0x73726371, // 'srcq'
		ApplicableBitRateRange = 0x62727461, // 'brta'
		RecommendedBitRateRange = 0x62727472, // 'brtr'
		ApplicableInputSampleRates = 0x69737261, // 'isra'
		ApplicableOutputSampleRates = 0x6f737261, // 'osra'
		PaddedZeros = 0x70616430, // 'pad0'
		PrimeMethod = 0x70726d6d, // 'prmm'
		PrimeInfo = 0x7072696d, // 'prim'
		CurrentInputChannelLayout = 0x69636c20, // 'icl '
		CurrentOutputChannelLayout = 0x6f636c20, // 'ocl '
		Settings = 0x61637320, // 'acs '
		FormatList = 0x6163666c, // 'acfl'
		BitRateControlMode = 0x61636266, // 'acbf'
		SoundQualityForVbr = 0x76627271, // 'vbrq'
		BitRateForVbr = 0x76627262, // 'vbrb'
		DelayMode = 0x646d6f64, // 'dmod'
		AdjustLocalQuality = 0x5e71616c, // '^qal'
		DynamicRangeControlMode = 0x6d647263, // 'mdrc'
		AdjustCompressionProfile = 0x5e70726f, // '^pro'
		ProgramTargetLevelConstant = 0x70746c63, // 'ptlc'
		AdjustTargetLevelConstant = 0x5e746c63, // '^tlc'
		ProgramTargetLevel = 0x7070746c, // 'pptl'
		AdjustTargetLevel = 0x5e70746c, // '^ptl'
		[iOS (26, 0), TV (26, 0), Mac (26, 0), MacCatalyst (26, 0)]
		DynamicRangeControlConfiguration = 0x63647263, // 'cdrc'
		[iOS (26, 0), TV (26, 0), Mac (26, 0), MacCatalyst (26, 0)]
		ContentSource = 0x63737263, // 'csrc'
		[iOS (26, 0), TV (26, 0), Mac (26, 0), MacCatalyst (26, 0)]
		AspFrequency = 0x61737066, // 'aspf'
	}

	/// <summary>Specifies the dynamic range compression configuration for encoded audio.</summary>
	[iOS (26, 0), TV (26, 0), Mac (26, 0), MacCatalyst (26, 0)]
	public enum AudioCodecDynamicRangeControlConfiguration : uint {
		/// <summary>Disable dynamic range compression.</summary>
		None = 0,
		/// <summary>Apply dynamic range compression for music.</summary>
		Music = 1,
		/// <summary>Apply dynamic range compression for speech.</summary>
		Speech = 2,
		/// <summary>Apply dynamic range compression for movie soundtracks.</summary>
		Movie = 3,
		/// <summary>Apply dynamic range compression for live encoding.</summary>
		Capture = 4,
	}

	[iOS (26, 0), TV (26, 0), Mac (26, 0), MacCatalyst (26, 0)]
	public enum AudioCodecContentSource : int {
		Unspecified = -1,
		Reserved = 0,
		AppleCaptureTraditional = 1,
		AppleCaptureSpatial = 2,
		AppleCaptureSpatialEnhanced = 3,
		AppleMusicTraditional = 4,
		AppleMusicSpatial = 5,
		AppleAVTraditionalOffline = 6,
		AppleAVSpatialOffline = 7,
		AppleAVTraditionalLive = 8,
		AppleAVSpatialLive = 9,
		ApplePassthrough = 10,

		CaptureTraditional = 33,
		CaptureSpatial = 34,
		CaptureSpatialEnhanced = 35,
		MusicTraditional = 36,
		MusicSpatial = 37,
		AVTraditionalOffline = 38,
		AVSpatialOffline = 39,
		AVTraditionalLive = 40,
		AVSpatialLive = 41,
		Passthrough = 42,
	}
}
