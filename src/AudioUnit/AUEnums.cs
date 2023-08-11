//
// AUEnums.cs: AudioUnit enumerations
//
// Authors:
//   AKIHIRO Uehara (u-akihiro@reinforce-lab.com)
//   Marek Safar (marek.safar@gmail.com)
//
// Copyright 2010 Reinforce Lab.
// Copyright 2011-2013 Xamarin Inc
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using AudioToolbox;
using ObjCRuntime;
using CoreFoundation;
using Foundation;

#nullable enable

namespace AudioUnit {
	public enum AudioUnitStatus { // Implictly cast to OSType
		NoError = 0,
		OK = NoError,
		FileNotFound = -43,
		ParameterError = -50,
		InvalidProperty = -10879,
		InvalidParameter = -10878,
		InvalidElement = -10877,
		NoConnection = -10876,
		FailedInitialization = -10875,
		TooManyFramesToProcess = -10874,
		InvalidFile = -10871,
		FormatNotSupported = -10868,
		Uninitialized = -10867,
		InvalidScope = -10866,
		PropertyNotWritable = -10865,
		CannotDoInCurrentContext = -10863,
		InvalidPropertyValue = -10851,
		PropertyNotInUse = -10850,
		Initialized = -10849,
		InvalidOfflineRender = -10848,
		Unauthorized = -10847,
		[NoWatch]
		[MacCatalyst (13, 1)]
		MidiOutputBufferFull = -66753,
		[iOS (11, 3), TV (11, 3), NoWatch]
		[MacCatalyst (13, 1)]
		InvalidParameterValue = -66743,
		[NoWatch]
		[MacCatalyst (13, 1)]
		ExtensionNotFound = -66744,
	}

	public enum AudioComponentStatus { // Implictly cast to OSType
		OK = 0,
		DuplicateDescription = -66752,
		UnsupportedType = -66751,
		TooManyInstances = -66750,
		InstanceInvalidated = -66749,
		NotPermitted = -66748,
		InitializationTimedOut = -66747,
		InvalidFormat = -66746,
		[MacCatalyst (13, 1)]
		RenderTimeout = -66745,
	}

	public enum AudioCodecManufacturer : uint  // Implictly cast to OSType in CoreAudio.framework - CoreAudioTypes.h
	{
		AppleSoftware = 0x6170706c, // 'appl'
		AppleHardware = 0x61706877, // 'aphw'
	}

	public enum InstrumentType : byte // UInt8 in AUSamplerInstrumentData
	{
		DLSPreset = 1,
		SF2Preset = DLSPreset,
		AUPreset = 2,
		Audiofile = 3,
		EXS24 = 4,
	}

	public enum AudioUnitParameterUnit // UInt32 AudioUnitParameterUnit
	{
		Generic = 0,
		Indexed = 1,
		Boolean = 2,
		Percent = 3,
		Seconds = 4,
		SampleFrames = 5,
		Phase = 6,
		Rate = 7,
		Hertz = 8,
		Cents = 9,
		RelativeSemiTones = 10,
		MIDINoteNumber = 11,
		MIDIController = 12,
		Decibels = 13,
		LinearGain = 14,
		Degrees = 15,
		EqualPowerCrossfade = 16,
		MixerFaderCurve1 = 17,
		Pan = 18,
		Meters = 19,
		AbsoluteCents = 20,
		Octaves = 21,
		BPM = 22,
		Beats = 23,
		Milliseconds = 24,
		Ratio = 25,
		CustomUnit = 26,
		[iOS (15, 0), TV (15, 0), Mac (12, 0), MacCatalyst (15, 0)]
		MIDI2Controller = 27,
	}

	[Flags]
	public enum AudioUnitParameterFlag : uint // UInt32 in AudioUnitParameterInfo
	{
		CFNameRelease = (1 << 4),

		[MacCatalyst (13, 1)]
		OmitFromPresets = (1 << 13),
		PlotHistory = (1 << 14),
		MeterReadOnly = (1 << 15),

		// bit positions 18,17,16 are set aside for display scales. bit 19 is reserved.
		DisplayMask = (7 << 16) | (1 << 22),
		DisplaySquareRoot = (1 << 16),
		DisplaySquared = (2 << 16),
		DisplayCubed = (3 << 16),
		DisplayCubeRoot = (4 << 16),
		DisplayExponential = (5 << 16),

		HasClump = (1 << 20),
		ValuesHaveStrings = (1 << 21),

		DisplayLogarithmic = (1 << 22),

		IsHighResolution = (1 << 23),
		NonRealTime = (1 << 24),
		CanRamp = (1 << 25),
		ExpertMode = (1 << 26),
		HasCFNameString = (1 << 27),
		IsGlobalMeta = (1 << 28),
		IsElementMeta = (1 << 29),
		IsReadable = (1 << 30),
		IsWritable = ((uint) 1 << 31),
	}

	public enum AudioUnitClumpID // UInt32 in AudioUnitParameterInfo
	{
		System = 0,
	}

	[MacCatalyst (13, 1)]
	[NoTV, NoWatch]
#if NET
	[NoiOS]
#endif
	public enum AudioObjectPropertySelector : uint {
		PropertyDevices = 1684370979, // 'dev#'
		Devices = 1684370979, // 'dev#'
		DefaultInputDevice = 1682533920, // 'dIn '
		DefaultOutputDevice = 1682929012, // 'dOut'
		DefaultSystemOutputDevice = 1934587252, // 'sOut'
		TranslateUIDToDevice = 1969841252, // 'uidd'
		MixStereoToMono = 1937010031, // 'stmo'
		PlugInList = 1886152483, // 'plg#'
		TranslateBundleIDToPlugIn = 1651074160, // 'bidp'
		TransportManagerList = 1953326883, // 'tmg#'
		TranslateBundleIDToTransportManager = 1953325673, // 'tmbi'
		BoxList = 1651472419, // 'box#'
		TranslateUIDToBox = 1969841250, // 'uidb'
		ClockDeviceList = 1668049699, //'clk#'
		TranslateUidToClockDevice = 1969841251, // 'uidc',
#if !XAMCORE_5_0
		[MacCatalyst (13, 1)] // This is required for .NET, because otherwise the generator thinks it's not available because it's not available on iOS.
		[Deprecated (PlatformName.iOS, 15, 0, message: "Use the 'ProcessIsMain' element instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use the 'ProcessIsMain' element instead.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use the 'ProcessIsMain' element instead.")]
		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use the 'ProcessIsMain' element instead.")]
		[Obsolete ("Use the 'ProcessIsMain' element instead.")]
		ProcessIsMaster = 1835103092, // 'mast'
#endif // !XAMCORE_5_0
		[NoiOS]
		[MacCatalyst (15, 0), Mac (12, 0), NoTV, NoWatch]
		ProcessIsMain = 1835100526, // 'main'
		IsInitingOrExiting = 1768845172, // 'inot'
		UserIDChanged = 1702193508, // 'euid'
		ProcessIsAudible = 1886221684, // 'pmut'
		SleepingIsAllowed = 1936483696, // 'slep'
		UnloadingIsAllowed = 1970170980, // 'unld'
		HogModeIsAllowed = 1752131442, // 'hogr'
		UserSessionIsActiveOrHeadless = 1970496882, // 'user'
		ServiceRestarted = 1936880500, // 'srst'
		PowerHint = 1886353256, // 'powh'
		ActualSampleRate = 1634955892,// 'asrt',
		ClockDevice = 1634755428, // 'apcd',
		IOThreadOSWorkgroup = 1869838183, // 'oswg'
		[NoiOS]
		[MacCatalyst (15, 0), Mac (12, 0), NoTV, NoWatch]
		ProcessMute = 1634758765, // 'appm'
		[MacCatalyst (17, 0), Mac (14, 0), NoTV, NoWatch]
		InputMute = 1852403056, //pmin
	}

	[MacCatalyst (13, 1)]
	[NoTV, NoWatch]
#if NET
	[NoiOS]
#endif
	public enum AudioObjectPropertyScope : uint {
		Global = 1735159650, // 'glob'
		Input = 1768845428, // 'inpt'
		Output = 1869968496, // 'outp'
		PlayThrough = 1886679669, // 'ptru'
	}

	[MacCatalyst (13, 1)]
	[NoTV, NoWatch]
#if NET
	[NoiOS]
#endif
	public enum AudioObjectPropertyElement : uint {
#if !NET
		[Obsolete ("Use the 'Main' element instead.")]
		Master = 0, // 0
#endif
		Main = 0, // 0
	}

#if XAMCORE_3_0
	[Internal]
#else
	[Obsolete ("Please use the strongly typed properties instead.")]
#endif
	enum AudioUnitPropertyIDType { // UInt32 AudioUnitPropertyID
								   // Audio Unit Properties
		ClassInfo = 0,
		MakeConnection = 1,
		SampleRate = 2,
		ParameterList = 3,
		ParameterInfo = 4,
		CPULoad = 6,
		StreamFormat = 8,
		ElementCount = 11,
		Latency = 12,
		SupportedNumChannels = 13,
		MaximumFramesPerSlice = 14,
		ParameterValueStrings = 16,
		AudioChannelLayout = 19,
		TailTime = 20,
		BypassEffect = 21,
		LastRenderError = 22,
		SetRenderCallback = 23,
		FactoryPresets = 24,
		RenderQuality = 26,
		HostCallbacks = 27,
		InPlaceProcessing = 29,
		ElementName = 30,
		SupportedChannelLayoutTags = 32,
		PresentPreset = 36,
		DependentParameters = 45,
		InputSampleInOutput = 49,
		ShouldAllocateBuffer = 51,
		FrequencyResponse = 52,
		ParameterHistoryInfo = 53,
		Nickname = 54,
		OfflineRender = 37,
		[MacCatalyst (13, 1)]
		ParameterIDName = 34,
		[MacCatalyst (13, 1)]
		ParameterStringFromValue = 33,
		ParameterClumpName = 35,
		[MacCatalyst (13, 1)]
		ParameterValueFromString = 38,
		ContextName = 25,
		PresentationLatency = 40,
		ClassInfoFromDocument = 50,
		RequestViewController = 56,
		ParametersForOverview = 57,
		[MacCatalyst (13, 1)]
		SupportsMpe = 58,
		[iOS (15, 0), TV (15, 0), Mac (12, 0), MacCatalyst (15, 0)]
		LastRenderSampleTime = 61,
		[iOS (14, 5), TV (14, 5), Mac (11, 3)]
		[MacCatalyst (14, 5)]
		LoadedOutOfProcess = 62,
		[iOS (15, 0), TV (15, 0), Mac (12, 0), MacCatalyst (15, 0)]
		MIDIOutputEventListCallback = 63,
		[iOS (15, 0), TV (15, 0), Mac (12, 0), MacCatalyst (15, 0)]
		AudioUnitMIDIProtocol = 64,
		[iOS (15, 0), TV (15, 0), Mac (12, 0), MacCatalyst (15, 0)]
		HostMIDIProtocol = 65,

#if MONOMAC
		FastDispatch = 5,
		SetExternalBuffer = 15,
		GetUIComponentList = 18,
		CocoaUI = 31,
		IconLocation = 39,
		AUHostIdentifier = 46,
		MIDIOutputCallbackInfo = 47,
		MIDIOutputCallback = 48,
#else
		RemoteControlEventListener = 100,
		IsInterAppConnected = 101,
		PeerURL = 102,
#endif // MONOMAC

		// Output Unit
		IsRunning = 2001,

		// OS X Availability
#if MONOMAC

		// Music Effects and Instruments
		AllParameterMIDIMappings = 41,
		AddParameterMIDIMapping = 42,
		RemoveParameterMIDIMapping = 43,
		HotMapParameterMIDIMapping = 44,

		// Music Device
		MIDIXMLNames = 1006,
		PartGroup = 1010,
		DualSchedulingMode = 1013,
		SupportsStartStopNote = 1014,

		// Offline Unit
		InputSize = 3020,
		OutputSize = 3021,
		StartOffset = 3022,
		PreflightRequirements = 3023,
		PreflightName = 3024,

		// Translation Service
		FromPlugin = 4000,
		OldAutomation = 4001,

#endif // MONOMAC

		// Apple Specific Properties
		// AUConverter
		SampleRateConverterComplexity = 3014,

		// AUHAL and device units
		CurrentDevice = 2000,
		ChannelMap = 2002, // this will also work with AUConverter
		EnableIO = 2003,
		StartTime = 2004,
		SetInputCallback = 2005,
		HasIO = 2006,
		StartTimestampsAtZero = 2007, // this will also work with AUConverter

#if !MONOMAC
		MIDICallbacks = 2010,
		HostReceivesRemoteControlEvents = 2011,
		RemoteControlToHost = 2012,
		HostTransportState = 2013,
		NodeComponentDescription = 2014,
#endif // !MONOMAC

		// AUVoiceProcessing unit
		BypassVoiceProcessing = 2100,
		VoiceProcessingEnableAGC = 2101,
		MuteOutput = 2104,
		[iOS (15, 0), MacCatalyst (15, 0), NoMac, NoWatch, NoTV]
		MutedSpeechActivityEventListener = 2106,

		// AUNBandEQ unit
		NumberOfBands = 2200,
		MaxNumberOfBands = 2201,
		BiquadCoefficients = 2203,

		// Mixers
		// General mixers
		MeteringMode = 3007,

		// Matrix Mixer
		MatrixLevels = 3006,
		MatrixDimensions = 3009,
		MeterClipping = 3011,
		[MacCatalyst (13, 1)]
		InputAnchorTimeStamp = 3016,

		// SpatialMixer
		ReverbRoomType = 10,
		UsesInternalReverb = 1005,
		SpatializationAlgorithm = 3000,
		[Deprecated (PlatformName.iOS, 9, 0)]
		[Deprecated (PlatformName.TvOS, 9, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Deprecated (PlatformName.MacOSX, 10, 11)]
		DistanceParams = 3010,
		[Deprecated (PlatformName.iOS, 9, 0)]
		[Deprecated (PlatformName.TvOS, 9, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Deprecated (PlatformName.MacOSX, 10, 11)]
		AttenuationCurve = 3013,
		[Deprecated (PlatformName.iOS, 9, 0)]
		[Deprecated (PlatformName.TvOS, 9, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Deprecated (PlatformName.MacOSX, 10, 11)]
		RenderingFlags = 3003,

		// AUScheduledSoundPlayer
		ScheduleAudioSlice = 3300,
		ScheduleStartTimeStamp = 3301,
		CurrentPlayTime = 3302,

		// AUAudioFilePlayer
		ScheduledFileIDs = 3310,
		ScheduledFileRegion = 3311,
		ScheduledFilePrime = 3312,
		ScheduledFileBufferSizeFrames = 3313,
		ScheduledFileNumberBuffers = 3314,

#if MONOMAC
		// OS X-specific Music Device Properties
		SoundBankData = 1008,
		StreamFromDisk = 1011,
		SoundBankFSRef = 1012,

#endif // !MONOMAC

		// Music Device Properties
		InstrumentName = 1001,
		InstrumentNumber = 1004,

		// Music Device Properties used by DLSMusicDevice and AUMIDISynth
		InstrumentCount = 1000,
		BankName = 1007,
		SoundBankURL = 1100,

		// AUMIDISynth
		MidiSynthEnablePreload = 4119,

		// AUSampler
		LoadInstrument = 4102,
		LoadAudioFiles = 4101,

		// AUDeferredRenderer
		DeferredRendererPullSize = 3320,
		DeferredRendererExtraLatency = 3321,
		DeferredRendererWaitFrames = 3322,

#if MONOMAC
		// AUNetReceive
		Hostname = 3511,
		NetReceivePassword = 3512,

		// AUNetSend
		PortNum = 3513,
		TransmissionFormat = 3514,
		TransmissionFormatIndex = 3515,
		ServiceName = 3516,
		Disconnect = 3517,
		NetSendPassword = 3518,
#endif // MONOMAC
	}

	public enum AudioUnitParameterType // UInt32 in AudioUnitParameterInfo
	{
		// AUMixer3D unit
		Mixer3DAzimuth = 0,
		Mixer3DElevation = 1,
		Mixer3DDistance = 2,
		Mixer3DGain = 3,
		Mixer3DPlaybackRate = 4,
#if MONOMAC
		Mixer3DReverbBlend					= 5,
		Mixer3DGlobalReverbGain				= 6,
		Mixer3DOcclusionAttenuation			= 7,
		Mixer3DObstructionAttenuation		= 8,
		Mixer3DMinGain						= 9,
		Mixer3DMaxGain						= 10,
		Mixer3DPreAveragePower				= 1000,
		Mixer3DPrePeakHoldLevel				= 2000,
		Mixer3DPostAveragePower				= 3000,
		Mixer3DPostPeakHoldLevel			= 4000,
#else
		Mixer3DEnable = 5,
		Mixer3DMinGain = 6,
		Mixer3DMaxGain = 7,
		Mixer3DReverbBlend = 8,
		Mixer3DGlobalReverbGain = 9,
		Mixer3DOcclusionAttenuation = 10,
		Mixer3DObstructionAttenuation = 11,
#endif

		// AUSpatialMixer unit
		SpatialAzimuth = 0,
		SpatialElevation = 1,
		SpatialDistance = 2,
		SpatialGain = 3,
		SpatialPlaybackRate = 4,
		SpatialEnable = 5,
		SpatialMinGain = 6,
		SpatialMaxGain = 7,
		SpatialReverbBlend = 8,
		SpatialGlobalReverbGain = 9,
		SpatialOcclusionAttenuation = 10,
		SpatialObstructionAttenuation = 11,

		// Reverb applicable to the 3DMixer or AUSpatialMixer
		ReverbFilterFrequency = 14,
		ReverbFilterBandwidth = 15,
		ReverbFilterGain = 16,
		[MacCatalyst (13, 1)]
		ReverbFilterType = 17,
		[MacCatalyst (13, 1)]
		ReverbFilterEnable = 18,

		// AUMultiChannelMixer
		MultiChannelMixerVolume = 0,
		MultiChannelMixerEnable = 1,
		MultiChannelMixerPan = 2,

		// AUMatrixMixer unit
		MatrixMixerVolume = 0,
		MatrixMixerEnable = 1,

		// AudioDeviceOutput, DefaultOutputUnit, and SystemOutputUnit units
		HALOutputVolume = 14,

		// AUTimePitch, AUTimePitch (offline), AUPitch units
		TimePitchRate = 0,
#if MONOMAC
		TimePitchPitch						= 1,
		TimePitchEffectBlend				= 2,
#endif

		// AUNewTimePitch
		NewTimePitchRate = 0,
		NewTimePitchPitch = 1,
		NewTimePitchOverlap = 4,
		NewTimePitchEnablePeakLocking = 6,

		// AUSampler unit
		AUSamplerGain = 900,
		AUSamplerCoarseTuning = 901,
		AUSamplerFineTuning = 902,
		AUSamplerPan = 903,

		// AUBandpass
		BandpassCenterFrequency = 0,
		BandpassBandwidth = 1,

		// AUHipass
		HipassCutoffFrequency = 0,
		HipassResonance = 1,

		// AULowpass
		LowPassCutoffFrequency = 0,
		LowPassResonance = 1,

		// AUHighShelfFilter
		HighShelfCutOffFrequency = 0,
		HighShelfGain = 1,

		// AULowShelfFilter
		AULowShelfCutoffFrequency = 0,
		AULowShelfGain = 1,

#if !XAMCORE_5_0 // I can't find this value in the headers anymore
		[Obsoleted (PlatformName.iOS, 7, 0)]
		[Obsoleted (PlatformName.MacCatalyst, 13, 1)]
		AUDCFilterDecayTime = 0,
#endif

		// AUParametricEQ
		ParametricEQCenterFreq = 0,
		ParametricEQQ = 1,
		ParametricEQGain = 2,

		// AUPeakLimiter
		LimiterAttackTime = 0,
		LimiterDecayTime = 1,
		LimiterPreGain = 2,

		// AUDynamicsProcessor
		DynamicsProcessorThreshold = 0,
		DynamicsProcessorHeadRoom = 1,
		DynamicsProcessorExpansionRatio = 2,
		DynamicsProcessorExpansionThreshold = 3,
		DynamicsProcessorAttackTime = 4,
		DynamicsProcessorReleaseTime = 5,
		[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'DynamicsProcessorOverallGain' instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'DynamicsProcessorOverallGain' instead.")]
		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'DynamicsProcessorOverallGain' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'DynamicsProcessorOverallGain' instead.")]
		DynamicsProcessorMasterGain = 6,
		[iOS (15, 0), TV (15, 0), Mac (12, 0), MacCatalyst (15, 0)]
		DynamicsProcessorOverallGain = 6,
		DynamicsProcessorCompressionAmount = 1000,
		DynamicsProcessorInputAmplitude = 2000,
		DynamicsProcessorOutputAmplitude = 3000,

		// AUVarispeed
		VarispeedPlaybackRate = 0,
		VarispeedPlaybackCents = 1,

		// Distortion unit 
		DistortionDelay = 0,
		DistortionDecay = 1,
		DistortionDelayMix = 2,
		DistortionDecimation = 3,
		DistortionRounding = 4,
		DistortionDecimationMix = 5,
		DistortionLinearTerm = 6,
		DistortionSquaredTerm = 7,
		DistortionCubicTerm = 8,
		DistortionPolynomialMix = 9,
		DistortionRingModFreq1 = 10,
		DistortionRingModFreq2 = 11,
		DistortionRingModBalance = 12,
		DistortionRingModMix = 13,
		DistortionSoftClipGain = 14,
		DistortionFinalMix = 15,

		// AUDelay
		DelayWetDryMix = 0,
		DelayTime = 1,
		DelayFeedback = 2,
		DelayLopassCutoff = 3,

		// AUNBandEQ
		AUNBandEQGlobalGain = 0,
		AUNBandEQBypassBand = 1000,
		AUNBandEQFilterType = 2000,
		AUNBandEQFrequency = 3000,
		AUNBandEQGain = 4000,
		AUNBandEQBandwidth = 5000,

		// AURandomUnit
		RandomBoundA = 0,
		RandomBoundB = 1,
		RandomCurve = 2,

#if !MONOMAC
		// iOS reverb
		Reverb2DryWetMix = 0,
		Reverb2Gain = 1,
		Reverb2MinDelayTime = 2,
		Reverb2MaxDelayTime = 3,
		Reverb2DecayTimeAt0Hz = 4,
		Reverb2DecayTimeAtNyquist = 5,
		Reverb2RandomizeReflections = 6,
#endif

		// RoundTripAAC
		RoundTripAacFormat = 0,
		RoundTripAacEncodingStrategy = 1,
		RoundTripAacRateOrQuality = 2,

		// Spacial Mixer
		SpacialMixerAzimuth = 0,
		Elevation = 1,
		Distance = 2,
		Gain = 3,
		PlaybackRate = 4,
		Enable = 5,
		MinGain = 6,
		MaxGain = 7,
		ReverbBlend = 8,
		GlobalReverbGain = 9,
		OcclussionAttenuation = 10,
		ObstructionAttenuation = 11,
	}

	[MacCatalyst (13, 1)]
	public enum SpatialMixerAttenuation {
		Power = 0,
		Exponential = 1,
		Inverse = 2,
		Linear = 3,
	}

	[Flags]
	[MacCatalyst (13, 1)]
	public enum SpatialMixerRenderingFlags {
		InterAuralDelay = (1 << 0),
		[Deprecated (PlatformName.iOS, 9, 0)]
		[Deprecated (PlatformName.TvOS, 9, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Deprecated (PlatformName.MacOSX, 10, 11)]
		DistanceAttenuation = (1 << 2),
	}

	[Flags]
	public enum ScheduledAudioSliceFlag {
		Complete = 0x01,
		BeganToRender = 0x02,
		BeganToRenderLate = 0x04,

		[MacCatalyst (13, 1)]
		Loop = 0x08,
		[MacCatalyst (13, 1)]
		Interrupt = 0x10,
		[MacCatalyst (13, 1)]
		InterruptAtLoop = 0x20,
	}

	public enum AudioUnitScopeType { // UInt32 AudioUnitScope
		Global = 0,
		Input = 1,
		Output = 2,
		Group = 3,
		Part = 4,
		Note = 5,
		Layer = 6,
		LayerItem = 7,
	}

	[Flags]
	public enum AudioUnitRenderActionFlags { // UInt32 AudioUnitRenderActionFlags
		PreRender = (1 << 2),
		PostRender = (1 << 3),
		OutputIsSilence = (1 << 4),
		OfflinePreflight = (1 << 5),
		OfflineRender = (1 << 6),
		OfflineComplete = (1 << 7),
		PostRenderError = (1 << 8),
		DoNotCheckRenderArgs = (1 << 9),
	}

	public enum AudioUnitRemoteControlEvent // Unused?
	{
		TogglePlayPause = 1,
		ToggleRecord = 2,
		Rewind = 3,
	}

	[Native]
	public enum AudioUnitBusType : long {
		Input = 1,
		Output = 2,
	}

	[Native]
	public enum AUHostTransportStateFlags : ulong {
		Changed = 1,
		Moving = 2,
		Recording = 4,
		Cycling = 8,
	}

	public enum AUEventSampleTime : long {
		Immediate = unchecked((long) 0xffffffff00000000),
	}

	[MacCatalyst (13, 1)]
	public enum AudioComponentInstantiationOptions : uint {
		OutOfProcess = 1,
		[NoiOS, NoTV, NoMacCatalyst]
		InProcess = 2,
		[iOS (14, 5), TV (14, 5), NoMac]
		[MacCatalyst (14, 5)]
		LoadedRemotely = 1u << 31,
	}

	[Native]
	public enum AUAudioUnitBusType : long {
		Input = 1,
		Output = 2
	}

	public enum AudioUnitParameterOptions : uint {
		CFNameRelease = (1 << 4),
		OmitFromPresets = (1 << 13),
		PlotHistory = (1 << 14),
		MeterReadOnly = (1 << 15),
		DisplayMask = (7 << 16) | (1 << 22),
		DisplaySquareRoot = (1 << 16),
		DisplaySquared = (2 << 16),
		DisplayCubed = (3 << 16),
		DisplayCubeRoot = (4 << 16),
		DisplayExponential = (5 << 16),
		HasClump = (1 << 20),
		ValuesHaveStrings = (1 << 21),
		DisplayLogarithmic = (1 << 22),
		IsHighResolution = (1 << 23),
		NonRealTime = (1 << 24),
		CanRamp = (1 << 25),
		ExpertMode = (1 << 26),
		HasCFNameString = (1 << 27),
		IsGlobalMeta = (1 << 28),
		IsElementMeta = (1 << 29),
		IsReadable = (1 << 30),
		IsWritable = unchecked((uint) 1 << 31),
	}

	public enum AudioComponentValidationResult : uint {
		Unknown = 0,
		Passed,
		Failed,
		TimedOut,
		UnauthorizedErrorOpen,
		UnauthorizedErrorInit,
	}

	public enum AUSpatialMixerAttenuationCurve : uint {
		Power = 0,
		Exponential = 1,
		Inverse = 2,
		Linear = 3,
	}

	public enum AU3DMixerRenderingFlags : uint {
		InterAuralDelay = (1 << 0),
		DopplerShift = (1 << 1),
		DistanceAttenuation = (1 << 2),
		DistanceFilter = (1 << 3),
		DistanceDiffusion = (1 << 4),
		LinearDistanceAttenuation = (1 << 5),
		ConstantReverbBlend = (1 << 6),
	}

	public enum AUReverbRoomType : uint {
		SmallRoom = 0,
		MediumRoom = 1,
		LargeRoom = 2,
		MediumHall = 3,
		LargeHall = 4,
		Plate = 5,
		MediumChamber = 6,
		LargeChamber = 7,
		Cathedral = 8,
		LargeRoom2 = 9,
		MediumHall2 = 10,
		MediumHall3 = 11,
		LargeHall2 = 12,
	}

	public enum AUScheduledAudioSliceFlags : uint {
		Complete = 1,
		BeganToRender = 2,
		BeganToRenderLate = 4,
		Loop = 8,
		Interrupt = 16,
		InterruptAtLoop = 32,
	}

	public enum AUSpatializationAlgorithm : uint {
		EqualPowerPanning = 0,
		SphericalHead = 1,
		Hrtf = 2,
		SoundField = 3,
		VectorBasedPanning = 4,
		StereoPassThrough = 5,
		HrtfHQ = 6,
		[iOS (14, 0)]
		[TV (14, 0)]
		[Mac (11, 0)]
		[MacCatalyst (14, 0)]
		UseOutputType = 7,
	}

	public enum AU3DMixerAttenuationCurve : uint {
		Power = 0,
		Exponential = 1,
		Inverse = 2,
		Linear = 3,
	}

	public enum AUSpatialMixerRenderingFlags : uint {
		InterAuralDelay = (1 << 0),
		DistanceAttenuation = (1 << 2),
	}

	[MacCatalyst (13, 1)]
	public enum AUParameterAutomationEventType : uint {
		Value = 0,
		Touch = 1,
		Release = 2,
	}

	[iOS (15, 0), TV (15, 0), Mac (12, 0), MacCatalyst (15, 0)]
	public enum AUVoiceIOSpeechActivityEvent : uint {
		Started = 0,
		Ended = 1,
	}

	[iOS (16, 0), TV (16, 0), Mac (13, 0), MacCatalyst (16, 0)]
	public enum AudioUnitEventType : uint {
		ParameterValueChange = 0,
		BeginParameterChangeGesture = 1,
		EndParameterChangeGesture = 2,
		PropertyChange = 3,
	}


	public enum AudioUnitSubType : uint {
		AUConverter = 0x636F6E76, // 'conv'
		Varispeed = 0x76617269, // 'vari'
		DeferredRenderer = 0x64656672, // 'defr'
		Splitter = 0x73706C74, // 'splt'
		MultiSplitter = 0x6D73706C, // 'mspl'
		Merger = 0x6D657267, // 'merg'
		NewTimePitch = 0x6E757470, // 'nutp'
		AUiPodTimeOther = 0x6970746F, // 'ipto'
		RoundTripAac = 0x72616163, // 'raac'
		GenericOutput = 0x67656E72, // 'genr'
		VoiceProcessingIO = 0x7670696F, // 'vpio'
		Sampler = 0x73616D70, // 'samp'
		MidiSynth = 0x6D73796E, // 'msyn'
		PeakLimiter = 0x6C6D7472, // 'lmtr'
		DynamicsProcessor = 0x64636D70, // 'dcmp'
		LowPassFilter = 0x6C706173, // 'lpas'
		HighPassFilter = 0x68706173, // 'hpas'
		BandPassFilter = 0x62706173, // 'bpas'
		HighShelfFilter = 0x68736866, // 'hshf'
		LowShelfFilter = 0x6C736866, // 'lshf'
		ParametricEQ = 0x706D6571, // 'pmeq'
		Distortion = 0x64697374, // 'dist'
		Delay = 0x64656C79, // 'dely'
		SampleDelay = 0x73646C79, // 'sdly'
		NBandEQ = 0x6E626571, // 'nbeq'
		MultiChannelMixer = 0x6D636D78, // 'mcmx'
		MatrixMixer = 0x6D786D78, // 'mxmx'
		SpatialMixer = 0x3364656D, // '3dem'
		ScheduledSoundPlayer = 0x7373706C, // 'sspl'
		AudioFilePlayer = 0x6166706C, // 'afpl'

#if MONOMAC
		HALOutput 				= 0x6168616C, // 'ahal'
		DefaultOutput 			= 0x64656620, // 'def '
		SystemOutput 			= 0x73797320, // 'sys '
		DLSSynth 				= 0x646C7320, // 'dls '
		TimePitch 				= 0x746D7074, // 'tmpt'
		GraphicEQ 				= 0x67726571, // 'greq'
		MultiBandCompressor 	= 0x6D636D70, // 'mcmp'
		MatrixReverb 			= 0x6D726576, // 'mrev'
		Pitch 					= 0x746D7074, // 'tmpt'
		AUFilter 				= 0x66696C74, // 'filt
		NetSend 				= 0x6E736E64, // 'nsnd'
		RogerBeep 				= 0x726F6772, // 'rogr'
		StereoMixer 			= 0x736D7872, // 'smxr'
		SphericalHeadPanner 	= 0x73706872, // 'sphr'
		VectorPanner 			= 0x76626173, // 'vbas'
		SoundFieldPanner 		= 0x616D6269, // 'ambi'
		HRTFPanner 				= 0x68727466, // 'hrtf'
		NetReceive 				= 0x6E726376, // 'nrcv'
#endif
	}

	[MacCatalyst (17, 0), Mac (14, 0), NoTV, NoWatch, NoiOS]
	public enum AudioAggregateDriftCompensation : uint {
		MinQuality = 0,
		LowQuality = 0x20,
		MediumQuality = 0x40,
		HighQuality = 0x60,
		MaxQuality = 0x7F,
	}
}
