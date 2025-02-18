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
using System.ComponentModel;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using AudioToolbox;
using ObjCRuntime;
using CoreFoundation;
using Foundation;

#nullable enable

namespace AudioUnit {
	/// <summary>An enumeration whose values specify the status of an <see cref="T:AudioUnit.AudioUnit" />.</summary>
	public enum AudioUnitStatus { // Implictly cast to OSType
		/// <summary>To be added.</summary>
		NoError = 0,
		/// <summary>To be added.</summary>
		OK = NoError,
		/// <summary>To be added.</summary>
		FileNotFound = -43,
		/// <summary>To be added.</summary>
		ParameterError = -50,
		/// <summary>To be added.</summary>
		InvalidProperty = -10879,
		/// <summary>To be added.</summary>
		InvalidParameter = -10878,
		/// <summary>To be added.</summary>
		InvalidElement = -10877,
		/// <summary>To be added.</summary>
		NoConnection = -10876,
		/// <summary>To be added.</summary>
		FailedInitialization = -10875,
		/// <summary>To be added.</summary>
		TooManyFramesToProcess = -10874,
		/// <summary>To be added.</summary>
		InvalidFile = -10871,
		/// <summary>To be added.</summary>
		FormatNotSupported = -10868,
		/// <summary>To be added.</summary>
		Uninitialized = -10867,
		/// <summary>To be added.</summary>
		InvalidScope = -10866,
		/// <summary>To be added.</summary>
		PropertyNotWritable = -10865,
		/// <summary>To be added.</summary>
		CannotDoInCurrentContext = -10863,
		/// <summary>To be added.</summary>
		InvalidPropertyValue = -10851,
		/// <summary>To be added.</summary>
		PropertyNotInUse = -10850,
		/// <summary>To be added.</summary>
		Initialized = -10849,
		/// <summary>To be added.</summary>
		InvalidOfflineRender = -10848,
		/// <summary>To be added.</summary>
		Unauthorized = -10847,
		/// <summary>To be added.</summary>
		MidiOutputBufferFull = -66753,
		RenderTimeout = -66745,
		/// <summary>To be added.</summary>
		InvalidParameterValue = -66743,
		/// <summary>To be added.</summary>
		ExtensionNotFound = -66744,
		InvalidFilePath = -66742,
		MissingKey = -66741,
		ComponentManagerNotSupported = -66740,
		MultipleVoiceProcessors = -66635,
	}

	/// <summary>Enumerates status values returned by <see cref="M:AudioUnit.AudioUnit.AudioOutputUnitPublish(AudioUnit.AudioComponentDescription,System.String,System.UInt32)" />.</summary>
	public enum AudioComponentStatus { // Implictly cast to OSType
		/// <summary>To be added.</summary>
		OK = 0,
		/// <summary>To be added.</summary>
		DuplicateDescription = -66752,
		/// <summary>To be added.</summary>
		UnsupportedType = -66751,
		/// <summary>To be added.</summary>
		TooManyInstances = -66750,
		InstanceTimedOut = -66754,
		/// <summary>To be added.</summary>
		InstanceInvalidated = -66749,
		/// <summary>To be added.</summary>
		NotPermitted = -66748,
		/// <summary>To be added.</summary>
		InitializationTimedOut = -66747,
		/// <summary>To be added.</summary>
		InvalidFormat = -66746,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		RenderTimeout = -66745,
	}

	/// <summary>An enumeration whose values specify whether to use a hardware or software encoder.</summary>
	public enum AudioCodecManufacturer : uint  // Implictly cast to OSType in CoreAudio.framework - CoreAudioTypes.h
	{
		/// <summary>To be added.</summary>
		AppleSoftware = 0x6170706c, // 'appl'
		/// <summary>To be added.</summary>
		AppleHardware = 0x61706877, // 'aphw'
	}

	/// <summary>Enumerates instrument types.</summary>
	public enum InstrumentType : byte // UInt8 in AUSamplerInstrumentData
	{
		/// <summary>To be added.</summary>
		DLSPreset = 1,
		/// <summary>To be added.</summary>
		SF2Preset = DLSPreset,
		/// <summary>To be added.</summary>
		AUPreset = 2,
		/// <summary>To be added.</summary>
		Audiofile = 3,
		/// <summary>To be added.</summary>
		EXS24 = 4,
	}

	/// <summary>The unit of measure used by an audio unit parameter.</summary>
	public enum AudioUnitParameterUnit // UInt32 AudioUnitParameterUnit
	{
		/// <summary>To be added.</summary>
		Generic = 0,
		/// <summary>To be added.</summary>
		Indexed = 1,
		/// <summary>To be added.</summary>
		Boolean = 2,
		/// <summary>To be added.</summary>
		Percent = 3,
		/// <summary>To be added.</summary>
		Seconds = 4,
		/// <summary>To be added.</summary>
		SampleFrames = 5,
		/// <summary>To be added.</summary>
		Phase = 6,
		/// <summary>To be added.</summary>
		Rate = 7,
		/// <summary>To be added.</summary>
		Hertz = 8,
		/// <summary>To be added.</summary>
		Cents = 9,
		/// <summary>To be added.</summary>
		RelativeSemiTones = 10,
		/// <summary>To be added.</summary>
		MIDINoteNumber = 11,
		/// <summary>To be added.</summary>
		MIDIController = 12,
		/// <summary>To be added.</summary>
		Decibels = 13,
		/// <summary>To be added.</summary>
		LinearGain = 14,
		/// <summary>To be added.</summary>
		Degrees = 15,
		/// <summary>To be added.</summary>
		EqualPowerCrossfade = 16,
		/// <summary>To be added.</summary>
		MixerFaderCurve1 = 17,
		/// <summary>To be added.</summary>
		Pan = 18,
		/// <summary>To be added.</summary>
		Meters = 19,
		/// <summary>To be added.</summary>
		AbsoluteCents = 20,
		/// <summary>To be added.</summary>
		Octaves = 21,
		/// <summary>To be added.</summary>
		BPM = 22,
		/// <summary>To be added.</summary>
		Beats = 23,
		/// <summary>To be added.</summary>
		Milliseconds = 24,
		/// <summary>To be added.</summary>
		Ratio = 25,
		/// <summary>To be added.</summary>
		CustomUnit = 26,
		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		MIDI2Controller = 27,
	}

	/// <summary>Flagging enumeration used with <see cref="P:AudioUnit.AudioUnitParameterInfo.Flags" />.</summary>
	[Flags]
	public enum AudioUnitParameterFlag : uint // UInt32 in AudioUnitParameterInfo
	{
		/// <summary>To be added.</summary>
		CFNameRelease = (1 << 4),

		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		OmitFromPresets = (1 << 13),
		/// <summary>To be added.</summary>
		PlotHistory = (1 << 14),
		/// <summary>To be added.</summary>
		MeterReadOnly = (1 << 15),

		// bit positions 18,17,16 are set aside for display scales. bit 19 is reserved.
		/// <summary>To be added.</summary>
		DisplayMask = (7 << 16) | (1 << 22),
		/// <summary>To be added.</summary>
		DisplaySquareRoot = (1 << 16),
		/// <summary>To be added.</summary>
		DisplaySquared = (2 << 16),
		/// <summary>To be added.</summary>
		DisplayCubed = (3 << 16),
		/// <summary>To be added.</summary>
		DisplayCubeRoot = (4 << 16),
		/// <summary>To be added.</summary>
		DisplayExponential = (5 << 16),

		/// <summary>To be added.</summary>
		HasClump = (1 << 20),
		/// <summary>To be added.</summary>
		ValuesHaveStrings = (1 << 21),

		/// <summary>To be added.</summary>
		DisplayLogarithmic = (1 << 22),

		/// <summary>To be added.</summary>
		IsHighResolution = (1 << 23),
		/// <summary>To be added.</summary>
		NonRealTime = (1 << 24),
		/// <summary>To be added.</summary>
		CanRamp = (1 << 25),
		/// <summary>To be added.</summary>
		ExpertMode = (1 << 26),
		/// <summary>To be added.</summary>
		HasCFNameString = (1 << 27),
		/// <summary>To be added.</summary>
		IsGlobalMeta = (1 << 28),
		/// <summary>To be added.</summary>
		IsElementMeta = (1 << 29),
		/// <summary>To be added.</summary>
		IsReadable = (1 << 30),
		/// <summary>To be added.</summary>
		IsWritable = ((uint) 1 << 31),
	}

	/// <summary>Enumerates values used by <see cref="T:AudioUnit.AudioUnitParameterInfo" />. Currenty reserved for system use.</summary>
	public enum AudioUnitClumpID // UInt32 in AudioUnitParameterInfo
	{
		/// <summary>To be added.</summary>
		System = 0,
	}

	[MacCatalyst (13, 1)]
	[NoTV]
#if NET
	[NoiOS]
#endif
	public enum AudioObjectPropertySelector : uint {
		/// <summary>To be added.</summary>
		PropertyDevices = 1684370979, // 'dev#'
		/// <summary>To be added.</summary>
		Devices = 1684370979, // 'dev#'
		/// <summary>To be added.</summary>
		DefaultInputDevice = 1682533920, // 'dIn '
		/// <summary>To be added.</summary>
		DefaultOutputDevice = 1682929012, // 'dOut'
		/// <summary>To be added.</summary>
		DefaultSystemOutputDevice = 1934587252, // 'sOut'
		/// <summary>To be added.</summary>
		TranslateUIDToDevice = 1969841252, // 'uidd'
		/// <summary>To be added.</summary>
		MixStereoToMono = 1937010031, // 'stmo'
		/// <summary>To be added.</summary>
		PlugInList = 1886152483, // 'plg#'
		/// <summary>To be added.</summary>
		TranslateBundleIDToPlugIn = 1651074160, // 'bidp'
		/// <summary>To be added.</summary>
		TransportManagerList = 1953326883, // 'tmg#'
		/// <summary>To be added.</summary>
		TranslateBundleIDToTransportManager = 1953325673, // 'tmbi'
		/// <summary>To be added.</summary>
		BoxList = 1651472419, // 'box#'
		/// <summary>To be added.</summary>
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
		[MacCatalyst (15, 0), NoTV]
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
		[MacCatalyst (15, 0), NoTV]
		ProcessMute = 1634758765, // 'appm'
		[MacCatalyst (17, 0), Mac (14, 0), NoTV]
		InputMute = 1852403056, //pmin
	}

	[MacCatalyst (13, 1)]
	[NoTV]
#if NET
	[NoiOS]
#endif
	public enum AudioObjectPropertyScope : uint {
		/// <summary>To be added.</summary>
		Global = 1735159650, // 'glob'
		/// <summary>To be added.</summary>
		Input = 1768845428, // 'inpt'
		/// <summary>To be added.</summary>
		Output = 1869968496, // 'outp'
		/// <summary>To be added.</summary>
		PlayThrough = 1886679669, // 'ptru'
	}

	[MacCatalyst (13, 1)]
	[NoTV]
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

	/// <summary>An enumeration whose values specify a kind of <see cref="T:AudioUnit.AudioUnit" />.</summary>
	[Internal]
	enum AudioUnitPropertyIDType { // UInt32 AudioUnitPropertyID
								   // Audio Unit Properties
		/// <summary>To be added.</summary>
		ClassInfo = 0,
		/// <summary>To be added.</summary>
		MakeConnection = 1,
		/// <summary>To be added.</summary>
		SampleRate = 2,
		/// <summary>To be added.</summary>
		ParameterList = 3,
		/// <summary>To be added.</summary>
		ParameterInfo = 4,
		/// <summary>To be added.</summary>
		CPULoad = 6,
		/// <summary>To be added.</summary>
		StreamFormat = 8,
		/// <summary>To be added.</summary>
		ElementCount = 11,
		/// <summary>To be added.</summary>
		Latency = 12,
		/// <summary>To be added.</summary>
		SupportedNumChannels = 13,
		/// <summary>To be added.</summary>
		MaximumFramesPerSlice = 14,
		/// <summary>To be added.</summary>
		ParameterValueStrings = 16,
		/// <summary>To be added.</summary>
		AudioChannelLayout = 19,
		/// <summary>To be added.</summary>
		TailTime = 20,
		/// <summary>To be added.</summary>
		BypassEffect = 21,
		/// <summary>To be added.</summary>
		LastRenderError = 22,
		/// <summary>To be added.</summary>
		SetRenderCallback = 23,
		/// <summary>To be added.</summary>
		FactoryPresets = 24,
		/// <summary>To be added.</summary>
		RenderQuality = 26,
		/// <summary>To be added.</summary>
		HostCallbacks = 27,
		/// <summary>To be added.</summary>
		InPlaceProcessing = 29,
		/// <summary>To be added.</summary>
		ElementName = 30,
		/// <summary>To be added.</summary>
		SupportedChannelLayoutTags = 32,
		/// <summary>To be added.</summary>
		PresentPreset = 36,
		/// <summary>To be added.</summary>
		DependentParameters = 45,
		/// <summary>To be added.</summary>
		InputSampleInOutput = 49,
		/// <summary>To be added.</summary>
		ShouldAllocateBuffer = 51,
		/// <summary>To be added.</summary>
		FrequencyResponse = 52,
		/// <summary>To be added.</summary>
		ParameterHistoryInfo = 53,
		/// <summary>To be added.</summary>
		Nickname = 54,
		/// <summary>To be added.</summary>
		OfflineRender = 37,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		ParameterIDName = 34,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		ParameterStringFromValue = 33,
		/// <summary>To be added.</summary>
		ParameterClumpName = 35,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		ParameterValueFromString = 38,
		/// <summary>To be added.</summary>
		ContextName = 25,
		/// <summary>To be added.</summary>
		PresentationLatency = 40,
		/// <summary>To be added.</summary>
		ClassInfoFromDocument = 50,
		/// <summary>To be added.</summary>
		RequestViewController = 56,
		/// <summary>To be added.</summary>
		ParametersForOverview = 57,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		SupportsMpe = 58,
		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		LastRenderSampleTime = 61,
		[iOS (14, 5), TV (14, 5)]
		[MacCatalyst (14, 5)]
		LoadedOutOfProcess = 62,
		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		MIDIOutputEventListCallback = 63,
		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		AudioUnitMIDIProtocol = 64,
		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
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
		/// <summary>To be added.</summary>
		RemoteControlEventListener = 100,
		/// <summary>To be added.</summary>
		IsInterAppConnected = 101,
		/// <summary>To be added.</summary>
		PeerURL = 102,
#endif // MONOMAC

		// Output Unit
		/// <summary>To be added.</summary>
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
		/// <summary>To be added.</summary>
		SampleRateConverterComplexity = 3014,

		// AUHAL and device units
		/// <summary>To be added.</summary>
		CurrentDevice = 2000,
		/// <summary>To be added.</summary>
		ChannelMap = 2002, // this will also work with AUConverter
		/// <summary>To be added.</summary>
		EnableIO = 2003,
		/// <summary>To be added.</summary>
		StartTime = 2004,
		/// <summary>To be added.</summary>
		SetInputCallback = 2005,
		/// <summary>To be added.</summary>
		HasIO = 2006,
		/// <summary>To be added.</summary>
		StartTimestampsAtZero = 2007, // this will also work with AUConverter

#if !MONOMAC
		/// <summary>To be added.</summary>
		MIDICallbacks = 2010,
		/// <summary>To be added.</summary>
		HostReceivesRemoteControlEvents = 2011,
		/// <summary>To be added.</summary>
		RemoteControlToHost = 2012,
		/// <summary>To be added.</summary>
		HostTransportState = 2013,
		/// <summary>To be added.</summary>
		NodeComponentDescription = 2014,
#endif // !MONOMAC

		// AUVoiceProcessing unit
		/// <summary>To be added.</summary>
		BypassVoiceProcessing = 2100,
		/// <summary>To be added.</summary>
		VoiceProcessingEnableAGC = 2101,
		/// <summary>To be added.</summary>
		MuteOutput = 2104,
		[iOS (15, 0), MacCatalyst (15, 0), NoMac, NoTV]
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
		SpatialMixerRenderingFlags = 3003,
		SpatialMixerSourceMode = 3005,
		SpatialMixerDistanceParams = 3010,
#if !XAMCORE_5_0
		[Obsolete ("Use 'SpatialMixerDistanceParams' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		DistanceParams = SpatialMixerDistanceParams,
		[Obsolete ("Use 'SpatialMixerAttenuationCurve' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		AttenuationCurve = SpatialMixerAttenuationCurve,
		[Obsolete ("Use 'SpatialMixerRenderingFlags' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		RenderingFlags = SpatialMixerRenderingFlags,
#endif
		SpatialMixerAttenuationCurve = 3013,
		SpatialMixerOutputType = 3100,
		SpatialMixerPointSourceInHeadMode = 3103,
		[Mac (12, 3), iOS (18, 0), TV (18, 0), MacCatalyst (18, 0)]
		SpatialMixerEnableHeadTracking = 3111,
		[Mac (13, 0), iOS (18, 0), TV (18, 0), MacCatalyst (18, 0)]
		SpatialMixerPersonalizedHrtfMode = 3113,
		[Mac (14, 0), iOS (18, 0), TV (18, 0), MacCatalyst (18, 0)]
		SpatialMixerAnyInputIsUsingPersonalizedHrtf = 3116,

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

	/// <summary>An enumeration whose values represent adjustable attributes such as pitch or volume.</summary>
	public enum AudioUnitParameterType // UInt32 in AudioUnitParameterInfo
	{
		// AUMixer3D unit
		/// <summary>To be added.</summary>
		Mixer3DAzimuth = 0,
		/// <summary>To be added.</summary>
		Mixer3DElevation = 1,
		/// <summary>To be added.</summary>
		Mixer3DDistance = 2,
		/// <summary>To be added.</summary>
		Mixer3DGain = 3,
		/// <summary>To be added.</summary>
		Mixer3DPlaybackRate = 4,
#if MONOMAC
		Mixer3DReverbBlend = 5,
		Mixer3DGlobalReverbGain = 6,
		Mixer3DOcclusionAttenuation = 7,
		Mixer3DObstructionAttenuation = 8,
		Mixer3DMinGain = 9,
		Mixer3DMaxGain = 10,
		Mixer3DPreAveragePower = 1000,
		Mixer3DPrePeakHoldLevel = 2000,
		Mixer3DPostAveragePower = 3000,
		Mixer3DPostPeakHoldLevel = 4000,
#else
		/// <summary>To be added.</summary>
		Mixer3DEnable = 5,
		/// <summary>To be added.</summary>
		Mixer3DMinGain = 6,
		/// <summary>To be added.</summary>
		Mixer3DMaxGain = 7,
		/// <summary>To be added.</summary>
		Mixer3DReverbBlend = 8,
		/// <summary>To be added.</summary>
		Mixer3DGlobalReverbGain = 9,
		/// <summary>To be added.</summary>
		Mixer3DOcclusionAttenuation = 10,
		/// <summary>To be added.</summary>
		Mixer3DObstructionAttenuation = 11,
#endif

		// AUSpatialMixer unit
		/// <summary>To be added.</summary>
		SpatialAzimuth = 0,
		/// <summary>To be added.</summary>
		SpatialElevation = 1,
		/// <summary>To be added.</summary>
		SpatialDistance = 2,
		/// <summary>To be added.</summary>
		SpatialGain = 3,
		/// <summary>To be added.</summary>
		SpatialPlaybackRate = 4,
		/// <summary>To be added.</summary>
		SpatialEnable = 5,
		/// <summary>To be added.</summary>
		SpatialMinGain = 6,
		/// <summary>To be added.</summary>
		SpatialMaxGain = 7,
		/// <summary>To be added.</summary>
		SpatialReverbBlend = 8,
		/// <summary>To be added.</summary>
		SpatialGlobalReverbGain = 9,
		/// <summary>To be added.</summary>
		SpatialOcclusionAttenuation = 10,
		/// <summary>To be added.</summary>
		SpatialObstructionAttenuation = 11,

		// Reverb applicable to the 3DMixer or AUSpatialMixer
		/// <summary>To be added.</summary>
		ReverbFilterFrequency = 14,
		/// <summary>To be added.</summary>
		ReverbFilterBandwidth = 15,
		/// <summary>To be added.</summary>
		ReverbFilterGain = 16,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		ReverbFilterType = 17,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		ReverbFilterEnable = 18,

		// AUMultiChannelMixer
		/// <summary>To be added.</summary>
		MultiChannelMixerVolume = 0,
		/// <summary>To be added.</summary>
		MultiChannelMixerEnable = 1,
		/// <summary>To be added.</summary>
		MultiChannelMixerPan = 2,

		// AUMatrixMixer unit
		/// <summary>To be added.</summary>
		MatrixMixerVolume = 0,
		/// <summary>To be added.</summary>
		MatrixMixerEnable = 1,

		// AudioDeviceOutput, DefaultOutputUnit, and SystemOutputUnit units
		/// <summary>To be added.</summary>
		HALOutputVolume = 14,

		// AUTimePitch, AUTimePitch (offline), AUPitch units
		/// <summary>To be added.</summary>
		TimePitchRate = 0,
#if MONOMAC
		TimePitchPitch = 1,
		TimePitchEffectBlend = 2,
#endif

		// AUNewTimePitch
		/// <summary>To be added.</summary>
		NewTimePitchRate = 0,
		/// <summary>To be added.</summary>
		NewTimePitchPitch = 1,
		/// <summary>To be added.</summary>
		NewTimePitchOverlap = 4,
		/// <summary>To be added.</summary>
		NewTimePitchEnablePeakLocking = 6,

		// AUSampler unit
		/// <summary>To be added.</summary>
		AUSamplerGain = 900,
		/// <summary>To be added.</summary>
		AUSamplerCoarseTuning = 901,
		/// <summary>To be added.</summary>
		AUSamplerFineTuning = 902,
		/// <summary>To be added.</summary>
		AUSamplerPan = 903,

		// AUBandpass
		/// <summary>To be added.</summary>
		BandpassCenterFrequency = 0,
		/// <summary>To be added.</summary>
		BandpassBandwidth = 1,

		// AUHipass
		/// <summary>To be added.</summary>
		HipassCutoffFrequency = 0,
		/// <summary>To be added.</summary>
		HipassResonance = 1,

		// AULowpass
		/// <summary>To be added.</summary>
		LowPassCutoffFrequency = 0,
		/// <summary>To be added.</summary>
		LowPassResonance = 1,

		// AUHighShelfFilter
		/// <summary>To be added.</summary>
		HighShelfCutOffFrequency = 0,
		/// <summary>To be added.</summary>
		HighShelfGain = 1,

		// AULowShelfFilter
		/// <summary>To be added.</summary>
		AULowShelfCutoffFrequency = 0,
		/// <summary>To be added.</summary>
		AULowShelfGain = 1,

#if !XAMCORE_5_0 // I can't find this value in the headers anymore
		/// <summary>To be added.</summary>
		[Obsoleted (PlatformName.iOS, 7, 0)]
		[Obsoleted (PlatformName.MacCatalyst, 13, 1)]
		AUDCFilterDecayTime = 0,
#endif

		// AUParametricEQ
		/// <summary>To be added.</summary>
		ParametricEQCenterFreq = 0,
		/// <summary>To be added.</summary>
		ParametricEQQ = 1,
		/// <summary>To be added.</summary>
		ParametricEQGain = 2,

		// AUPeakLimiter
		/// <summary>To be added.</summary>
		LimiterAttackTime = 0,
		/// <summary>To be added.</summary>
		LimiterDecayTime = 1,
		/// <summary>To be added.</summary>
		LimiterPreGain = 2,

		// AUDynamicsProcessor
		/// <summary>To be added.</summary>
		DynamicsProcessorThreshold = 0,
		/// <summary>To be added.</summary>
		DynamicsProcessorHeadRoom = 1,
		/// <summary>To be added.</summary>
		DynamicsProcessorExpansionRatio = 2,
		/// <summary>To be added.</summary>
		DynamicsProcessorExpansionThreshold = 3,
		/// <summary>To be added.</summary>
		DynamicsProcessorAttackTime = 4,
		/// <summary>To be added.</summary>
		DynamicsProcessorReleaseTime = 5,
		[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'DynamicsProcessorOverallGain' instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'DynamicsProcessorOverallGain' instead.")]
		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'DynamicsProcessorOverallGain' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'DynamicsProcessorOverallGain' instead.")]
		DynamicsProcessorMasterGain = 6,
		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
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

	/// <summary>Enumerates attenuation modes.</summary>
	[MacCatalyst (13, 1)]
	public enum SpatialMixerAttenuation {
		Power = 0,
		Exponential = 1,
		Inverse = 2,
		Linear = 3,
	}

	/// <summary>Flagging enumeration used to control spatial mixing.</summary>
	[Flags]
	[MacCatalyst (13, 1)]
	public enum SpatialMixerRenderingFlags {
		/// <summary>To be added.</summary>
		InterAuralDelay = (1 << 0),
		/// <summary>Developers should not use this deprecated field. </summary>
		[Deprecated (PlatformName.iOS, 9, 0)]
		[Deprecated (PlatformName.TvOS, 9, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Deprecated (PlatformName.MacOSX, 10, 11)]
		DistanceAttenuation = (1 << 2),
	}

	/// <summary>Enumerates timing flags for rendering audio slices.</summary>
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

	/// <summary>An enumeration whose values specify roles and contexts for audio unit properties.</summary>
	public enum AudioUnitScopeType { // UInt32 AudioUnitScope
		/// <summary>To be added.</summary>
		Global = 0,
		/// <summary>To be added.</summary>
		Input = 1,
		/// <summary>To be added.</summary>
		Output = 2,
		/// <summary>To be added.</summary>
		Group = 3,
		/// <summary>To be added.</summary>
		Part = 4,
		/// <summary>To be added.</summary>
		Note = 5,
		/// <summary>To be added.</summary>
		Layer = 6,
		/// <summary>To be added.</summary>
		LayerItem = 7,
	}

	/// <summary>An enumeration whose values specify configuration flags for audio-unit rendering.</summary>
	[Flags]
	public enum AudioUnitRenderActionFlags { // UInt32 AudioUnitRenderActionFlags
		/// <summary>To be added.</summary>
		PreRender = (1 << 2),
		/// <summary>To be added.</summary>
		PostRender = (1 << 3),
		/// <summary>To be added.</summary>
		OutputIsSilence = (1 << 4),
		/// <summary>To be added.</summary>
		OfflinePreflight = (1 << 5),
		/// <summary>To be added.</summary>
		OfflineRender = (1 << 6),
		/// <summary>To be added.</summary>
		OfflineComplete = (1 << 7),
		/// <summary>To be added.</summary>
		PostRenderError = (1 << 8),
		/// <summary>To be added.</summary>
		DoNotCheckRenderArgs = (1 << 9),
	}

	/// <summary>Enumerates events relating to remote control commands.</summary>
	public enum AudioUnitRemoteControlEvent // Unused?
	{
		/// <summary>To be added.</summary>
		TogglePlayPause = 1,
		/// <summary>To be added.</summary>
		ToggleRecord = 2,
		/// <summary>To be added.</summary>
		Rewind = 3,
	}

	[Native]
	public enum AudioUnitBusType : long {
		/// <summary>To be added.</summary>
		Input = 1,
		/// <summary>To be added.</summary>
		Output = 2,
	}

	/// <summary>Enumerates flag values that describe the state of an audio transport.</summary>
	[Native]
	public enum AUHostTransportStateFlags : ulong {
		/// <summary>Indicates that state change has occured, such as a stop, start, seek, or other change since the host transport state block was last called.</summary>
		Changed = 1,
		/// <summary>Indicates that the transport is moving.</summary>
		Moving = 2,
		/// <summary>Indicates that the host is able to record, or is currently recording.</summary>
		Recording = 4,
		/// <summary>Indicates that the host is cycling.</summary>
		Cycling = 8,
	}

	public enum AUEventSampleTime : long {
		/// <summary>To be added.</summary>
		Immediate = unchecked((long) 0xffffffff00000000),
	}

	/// <summary>Enumerates options that can be used while instantiating a <see cref="T:AudioUnit.AUAudioUnit" />.</summary>
	[MacCatalyst (13, 1)]
	public enum AudioComponentInstantiationOptions : uint {
		/// <summary>To be added.</summary>
		OutOfProcess = 1,
		/// <summary>To be added.</summary>
		[NoiOS, NoTV, NoMacCatalyst]
		InProcess = 2,
		[iOS (14, 5), TV (14, 5), NoMac]
		[MacCatalyst (14, 5)]
		LoadedRemotely = 1u << 31,
	}

	/// <summary>Enumerates audio unit bus input-ouput capabilities.</summary>
	[Native]
	public enum AUAudioUnitBusType : long {
		/// <summary>Indicates an input bus.</summary>
		Input = 1,
		/// <summary>Indicates an output bus.</summary>
		Output = 2
	}

	public enum AudioUnitParameterOptions : uint {
		/// <summary>To be added.</summary>
		CFNameRelease = (1 << 4),
		/// <summary>To be added.</summary>
		OmitFromPresets = (1 << 13),
		/// <summary>To be added.</summary>
		PlotHistory = (1 << 14),
		/// <summary>To be added.</summary>
		MeterReadOnly = (1 << 15),
		/// <summary>To be added.</summary>
		DisplayMask = (7 << 16) | (1 << 22),
		/// <summary>To be added.</summary>
		DisplaySquareRoot = (1 << 16),
		/// <summary>To be added.</summary>
		DisplaySquared = (2 << 16),
		/// <summary>To be added.</summary>
		DisplayCubed = (3 << 16),
		/// <summary>To be added.</summary>
		DisplayCubeRoot = (4 << 16),
		/// <summary>To be added.</summary>
		DisplayExponential = (5 << 16),
		/// <summary>To be added.</summary>
		HasClump = (1 << 20),
		/// <summary>To be added.</summary>
		ValuesHaveStrings = (1 << 21),
		/// <summary>To be added.</summary>
		DisplayLogarithmic = (1 << 22),
		/// <summary>To be added.</summary>
		IsHighResolution = (1 << 23),
		/// <summary>To be added.</summary>
		NonRealTime = (1 << 24),
		/// <summary>To be added.</summary>
		CanRamp = (1 << 25),
		/// <summary>To be added.</summary>
		ExpertMode = (1 << 26),
		/// <summary>To be added.</summary>
		HasCFNameString = (1 << 27),
		/// <summary>To be added.</summary>
		IsGlobalMeta = (1 << 28),
		/// <summary>To be added.</summary>
		IsElementMeta = (1 << 29),
		/// <summary>To be added.</summary>
		IsReadable = (1 << 30),
		/// <summary>To be added.</summary>
		IsWritable = unchecked((uint) 1 << 31),
	}

	public enum AudioComponentValidationResult : uint {
		/// <summary>To be added.</summary>
		Unknown = 0,
		/// <summary>To be added.</summary>
		Passed,
		/// <summary>To be added.</summary>
		Failed,
		/// <summary>To be added.</summary>
		TimedOut,
		/// <summary>To be added.</summary>
		UnauthorizedErrorOpen,
		/// <summary>To be added.</summary>
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
		[MacCatalyst (14, 0)]
		UseOutputType = 7,
	}

	/// <summary>Enumerates attentuation curve types.</summary>
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
		/// <summary>To be added.</summary>
		Value = 0,
		/// <summary>To be added.</summary>
		Touch = 1,
		/// <summary>To be added.</summary>
		Release = 2,
	}

	[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
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
		/// <summary>To be added.</summary>
		AUConverter = 0x636F6E76, // 'conv'
		/// <summary>To be added.</summary>
		Varispeed = 0x76617269, // 'vari'
		/// <summary>To be added.</summary>
		DeferredRenderer = 0x64656672, // 'defr'
		/// <summary>To be added.</summary>
		Splitter = 0x73706C74, // 'splt'
		/// <summary>To be added.</summary>
		MultiSplitter = 0x6D73706C, // 'mspl'
		/// <summary>To be added.</summary>
		Merger = 0x6D657267, // 'merg'
		/// <summary>To be added.</summary>
		NewTimePitch = 0x6E757470, // 'nutp'
		/// <summary>To be added.</summary>
		AUiPodTimeOther = 0x6970746F, // 'ipto'
		/// <summary>To be added.</summary>
		RoundTripAac = 0x72616163, // 'raac'
		/// <summary>To be added.</summary>
		GenericOutput = 0x67656E72, // 'genr'
		/// <summary>To be added.</summary>
		VoiceProcessingIO = 0x7670696F, // 'vpio'
		/// <summary>To be added.</summary>
		Sampler = 0x73616D70, // 'samp'
		/// <summary>To be added.</summary>
		MidiSynth = 0x6D73796E, // 'msyn'
		/// <summary>To be added.</summary>
		PeakLimiter = 0x6C6D7472, // 'lmtr'
		/// <summary>To be added.</summary>
		DynamicsProcessor = 0x64636D70, // 'dcmp'
		/// <summary>To be added.</summary>
		LowPassFilter = 0x6C706173, // 'lpas'
		/// <summary>To be added.</summary>
		HighPassFilter = 0x68706173, // 'hpas'
		/// <summary>To be added.</summary>
		BandPassFilter = 0x62706173, // 'bpas'
		/// <summary>To be added.</summary>
		HighShelfFilter = 0x68736866, // 'hshf'
		/// <summary>To be added.</summary>
		LowShelfFilter = 0x6C736866, // 'lshf'
		/// <summary>To be added.</summary>
		ParametricEQ = 0x706D6571, // 'pmeq'
		/// <summary>To be added.</summary>
		Distortion = 0x64697374, // 'dist'
		/// <summary>To be added.</summary>
		Delay = 0x64656C79, // 'dely'
		/// <summary>To be added.</summary>
		SampleDelay = 0x73646C79, // 'sdly'
		/// <summary>To be added.</summary>
		NBandEQ = 0x6E626571, // 'nbeq'
		/// <summary>To be added.</summary>
		MultiChannelMixer = 0x6D636D78, // 'mcmx'
		/// <summary>To be added.</summary>
		MatrixMixer = 0x6D786D78, // 'mxmx'
		/// <summary>To be added.</summary>
		SpatialMixer = 0x3364656D, // '3dem'
		/// <summary>To be added.</summary>
		ScheduledSoundPlayer = 0x7373706C, // 'sspl'
		/// <summary>To be added.</summary>
		AudioFilePlayer = 0x6166706C, // 'afpl'

#if MONOMAC
		HALOutput = 0x6168616C, // 'ahal'
		DefaultOutput = 0x64656620, // 'def '
		SystemOutput = 0x73797320, // 'sys '
		DLSSynth = 0x646C7320, // 'dls '
		TimePitch = 0x746D7074, // 'tmpt'
		GraphicEQ = 0x67726571, // 'greq'
		MultiBandCompressor = 0x6D636D70, // 'mcmp'
		MatrixReverb = 0x6D726576, // 'mrev'
		Pitch = 0x746D7074, // 'tmpt'
		AUFilter = 0x66696C74, // 'filt
		NetSend = 0x6E736E64, // 'nsnd'
		RogerBeep = 0x726F6772, // 'rogr'
		StereoMixer = 0x736D7872, // 'smxr'
		SphericalHeadPanner = 0x73706872, // 'sphr'
		VectorPanner = 0x76626173, // 'vbas'
		SoundFieldPanner = 0x616D6269, // 'ambi'
		HRTFPanner = 0x68727466, // 'hrtf'
		NetReceive = 0x6E726376, // 'nrcv'
#endif
	}

	[MacCatalyst (17, 0), Mac (14, 0), NoTV, NoiOS]
	public enum AudioAggregateDriftCompensation : uint {
		MinQuality = 0,
		LowQuality = 0x20,
		MediumQuality = 0x40,
		HighQuality = 0x60,
		MaxQuality = 0x7F,
	}
}
