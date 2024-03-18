//
// CoreHaptics C# bindings
//
// Authors:
//	Manuel de la Pena Saenz <mandel@microsoft.com>
//
// Copyright 2019 Microsoft Corporation All rights reserved.
//
using System;
using Foundation;
using ObjCRuntime;

#nullable enable

namespace CoreHaptics {

	[iOS (13, 0), TV (14, 0)]
	[MacCatalyst (13, 1)]
	public enum CHHapticEventParameterId {
		[Field ("CHHapticEventParameterIDHapticIntensity")]
		HapticIntensity,

		[Field ("CHHapticEventParameterIDHapticSharpness")]
		HapticSharpness,

		[Field ("CHHapticEventParameterIDAttackTime")]
		AttackTime,

		[Field ("CHHapticEventParameterIDDecayTime")]
		DecayTime,

		[Field ("CHHapticEventParameterIDReleaseTime")]
		ReleaseTime,

		[Field ("CHHapticEventParameterIDSustained")]
		Sustained,

		[Field ("CHHapticEventParameterIDAudioVolume")]
		AudioVolume,

		[Field ("CHHapticEventParameterIDAudioPitch")]
		AudioPitch,

		[Field ("CHHapticEventParameterIDAudioPan")]
		AudioPan,

		[Field ("CHHapticEventParameterIDAudioBrightness")]
		AudioBrightness,
	}

	[iOS (13, 0), TV (14, 0)]
	[MacCatalyst (13, 1)]
	public enum CHHapticDynamicParameterId {
		[Field ("CHHapticDynamicParameterIDHapticIntensityControl")]
		HapticIntensityControl,

		[Field ("CHHapticDynamicParameterIDHapticSharpnessControl")]
		HapticSharpnessControl,

		[Field ("CHHapticDynamicParameterIDHapticAttackTimeControl")]
		HapticAttackTimeControl,

		[Field ("CHHapticDynamicParameterIDHapticDecayTimeControl")]
		HapticDecayTimeControl,

		[Field ("CHHapticDynamicParameterIDHapticReleaseTimeControl")]
		HapticReleaseTimeControl,

		[Field ("CHHapticDynamicParameterIDAudioVolumeControl")]
		AudioVolumeControl,

		[Field ("CHHapticDynamicParameterIDAudioPanControl")]
		AudioPanControl,

		[Field ("CHHapticDynamicParameterIDAudioBrightnessControl")]
		AudioBrightnessControl,

		[Field ("CHHapticDynamicParameterIDAudioPitchControl")]
		AudioPitchControl,

		[Field ("CHHapticDynamicParameterIDAudioAttackTimeControl")]
		AudioAttackTimeControl,

		[Field ("CHHapticDynamicParameterIDAudioDecayTimeControl")]
		AudioDecayTimeControl,

		[Field ("CHHapticDynamicParameterIDAudioReleaseTimeControl")]
		AudioReleaseTimeControl,
	}

	[iOS (13, 0), TV (14, 0)]
	[MacCatalyst (13, 1)]
	public enum CHHapticEventType {
		[Field ("CHHapticEventTypeHapticTransient")]
		HapticTransient,

		[Field ("CHHapticEventTypeHapticContinuous")]
		HapticContinuous,

		[Field ("CHHapticEventTypeAudioContinuous")]
		AudioContinuous,

		[Field ("CHHapticEventTypeAudioCustom")]
		AudioCustom,
	}

	[iOS (13, 0), TV (14, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum CHHapticErrorCode : long {
		EngineNotRunning = -4805,
		OperationNotPermitted = -4806,
		EngineStartTimeout = -4808,
		NotSupported = -4809,
		ServerInitFailed = -4810,
		ServerInterrupted = -4811,
		InvalidPatternPlayer = -4812,
		InvalidPatternData = -4813,
		InvalidPatternDictionary = -4814,
		InvalidAudioSession = -4815,
		InvalidEngineParameter = -4816,
		InvalidParameterType = -4820,
		InvalidEventType = -4821,
		InvalidEventTime = -4822,
		InvalidEventDuration = -4823,
		InvalidAudioResource = -4824,
		ResourceNotAvailable = -4825,
		BadEventEntry = -4830,
		BadParameterEntry = -4831,
		InvalidTime = -4840,
		FileNotFound = -4851,
		InsufficientPower = -4897,
		UnknownError = -4898,
		MemoryError = -4899,
	}

	[iOS (13, 0), TV (14, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum CHHapticEngineFinishedAction : long {
		StopEngine = 1,
		LeaveEngineRunning = 2,
	}

	[iOS (13, 0), TV (14, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum CHHapticEngineStoppedReason : long {
		AudioSessionInterrupt = 1,
		ApplicationSuspended = 2,
		IdleTimeout = 3,
		NotifyWhenFinished = 4,
		[iOS (14, 0)]
		[NoMac]
		[MacCatalyst (14, 0)]
		EngineDestroyed = 5,
		[iOS (14, 0)]
		[NoMac]
		[MacCatalyst (14, 0)]
		GameControllerDisconnect = 6,
		SystemError = -1,
	}
}
