using AVFoundation;
using CoreFoundation;
using Foundation;
using ModelIO;
using ObjCRuntime;

#if NET
using Vector2d = global::CoreGraphics.NVector2d;
using Vector3 = global::System.Numerics.Vector3;
using NMatrix4 = global::CoreGraphics.NMatrix4;
using Quaternion = global::System.Numerics.Quaternion;
#else
using Vector2d = global::OpenTK.Vector2d;
using Vector3 = global::OpenTK.Vector3;
using NMatrix4 = global::OpenTK.NMatrix4;
using Quaternion = global::OpenTK.Quaternion;
#endif

using System;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Phase {

	[Mac (12, 0), NoWatch, TV (17, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum PhaseUpdateMode : long {
		Automatic = 0,
		Manual = 1,
	}

	[Mac (12, 0), NoWatch, TV (17, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum PhaseRenderingState : long {
		Stopped = 0,
		Started = 1,
		Paused = 2,
	}

	[Mac (12, 0), NoWatch, TV (17, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum PhaseSpatializationMode : long {
		Automatic = 0,
		AlwaysUseBinaural = 1,
		AlwaysUseChannelBased = 2,
	}

	[Mac (12, 0), NoWatch, TV (17, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum PhaseReverbPreset : long {
		None = 1917742958,
		SmallRoom = 1918063213,
		MediumRoom = 1917669997,
		LargeRoom = 1917604401,
		LargeRoom2 = 1917604402,
		MediumChamber = 1917666152,
		LargeChamber = 1917600616,
		MediumHall = 1917667377,
		MediumHall2 = 1917667378,
		MediumHall3 = 1917667379,
		LargeHall = 1917601841,
		LargeHall2 = 1917601842,
		Cathedral = 1917023336,
	}

	[Mac (12, 0), NoWatch, TV (17, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	[ErrorDomain ("PHASEErrorDomain")]
	public enum PhaseError : long {
		InitializeFailed = 1346913633,
		InvalidObject = 1346913634,
	}

	[Mac (12, 0), NoWatch, TV (17, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	[ErrorDomain ("PHASESoundEventErrorDomain")]
	public enum PhaseSoundEventError : long {
		NotFound = 1346925665,
		BadData = 1346925666,
		InvalidInstance = 1346925667,
		ApiMisuse = 1346925668,
		SystemNotInitialized = 1346925669,
		OutOfMemory = 1346925670,
	}

	[Mac (12, 0), NoWatch, TV (17, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	[ErrorDomain ("PHASEAssetErrorDomain")]
	public enum PhaseAssetError : long {
		FailedToLoad = 1346920801,
		InvalidEngineInstance = 1346920802,
		BadParameters = 1346920803,
		AlreadyExists = 1346920804,
		GeneralError = 1346920805,
		MemoryAllocation = 1346920806,
	}

	[Mac (12, 0), NoWatch, TV (17, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum PhaseSoundEventPrepareHandlerReason : long {
		Error = 0,
		Prepared = 1,
		Terminated = 2,
	}

	[Mac (12, 0), NoWatch, TV (17, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum PhaseSoundEventStartHandlerReason : long {
		Error = 0,
		FinishedPlaying = 1,
		Terminated = 2,
	}

	[Mac (12, 0), NoWatch, TV (17, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum PhaseSoundEventSeekHandlerReason : long {
		Error = 0,
		ErrorSeekAlreadyInProgress = 1,
		SeekSuccessful = 2,
	}

	[Mac (12, 0), NoWatch, TV (17, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum PhaseSoundEventPrepareState : long {
		NotStarted = 0,
		InProgress = 1,
		Prepared = 2,
	}

	[Mac (12, 0), NoWatch, TV (17, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum PhaseAssetType : long {
		Resident = 0,
		Streamed = 1,
	}

	[Mac (12, 0), NoWatch, TV (17, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum PhaseCurveType : long {
		Linear = 1668435054,
		Squared = 1668436849,
		InverseSquared = 1668434257,
		Cubed = 1668432757,
		InverseCubed = 1668434243,
		Sine = 1668436846,
		InverseSine = 1668434259,
		Sigmoid = 1668436839,
		InverseSigmoid = 1668434247,
		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
		HoldStartValue = 1668434003,
		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
		JumpToEndValue = 1668434501,
	}

	[Mac (12, 0), NoWatch, TV (17, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum PhaseCullOption : long {
		Terminate = 0,
		SleepWakeAtZero = 1,
		SleepWakeAtRandomOffset = 2,
		SleepWakeAtRealtimeOffset = 3,
		DoNotCull = 4,
	}

	[Mac (12, 0), NoWatch, TV (17, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum PhasePlaybackMode : long {
		OneShot = 0,
		Looping = 1,
	}

	[Mac (12, 0), NoWatch, TV (17, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum PhaseNormalizationMode : long {
		None = 0,
		Dynamic = 1,
	}

	[Mac (12, 0), NoWatch, TV (17, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum PhaseCalibrationMode : long {
		None = 0,
		RelativeSpl = 1,
		AbsoluteSpl = 2,
	}

	[Mac (12, 0), NoWatch, TV (17, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Flags]
	[Native]
	public enum PhasePushStreamBufferOptions : ulong {
		Default = 1uL << 0,
		Loops = 1uL << 1,
		Interrupts = 1uL << 2,
		InterruptsAtLoop = 1uL << 3,
	}

	[Mac (12, 0), NoWatch, TV (17, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum PhasePushStreamCompletionCallbackCondition : long {
		DataRendered = 0,
	}

	[Mac (12, 0), NoWatch, TV (17, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum PhaseMediumPreset : long {
		PresetAir = 1835286898,
	}

	[Mac (12, 0), NoWatch, TV (17, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum PhaseMaterialPreset : long {
		Cardboard = 1833136740,
		Glass = 1833397363,
		Brickwork = 1833071211,
		Concrete = 1833132914,
		Drywall = 1833202295,
		Wood = 1834448228,
	}

	[Mac (12, 0), NoWatch, TV (17, 0), iOS (15, 0), MacCatalyst (15, 0)]
	public enum PhaseSpatialCategory {
		[Field ("PHASESpatialCategoryDirectPathTransmission")]
		DirectPathTransmission,
		[Field ("PHASESpatialCategoryEarlyReflections")]
		EarlyReflections,
		[Field ("PHASESpatialCategoryLateReverb")]
		LateReverb,
	}

	[Mac (12, 0), NoWatch, TV (17, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Flags]
	[Native]
	public enum PhaseSpatialPipelineFlags : ulong {
		DirectPathTransmission = 1uL << 0,
		EarlyReflections = 1uL << 1,
		LateReverb = 1uL << 2,
	}


	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject), Name = "PHASENumericPair")]
#if TVOS
	[DisableDefaultCtor]
#endif
	interface PhaseNumericPair {
		[Export ("initWithFirstValue:secondValue:")]
		NativeHandle Constructor (double first, double second);

		[Export ("first")]
		double First { get; set; }

		[Export ("second")]
		double Second { get; set; }
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject), Name = "PHASEEnvelopeSegment")]
#if TVOS
	[DisableDefaultCtor]
#endif
	interface PhaseEnvelopeSegment {
		[Export ("initWithEndPoint:curveType:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		NativeHandle Constructor (Vector2d endPoint, PhaseCurveType curveType);

		[Export ("endPoint", ArgumentSemantic.Assign)]
		Vector2d EndPoint {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			set;
		}

		[Export ("curveType", ArgumentSemantic.Assign)]
		PhaseCurveType CurveType { get; set; }
	}


	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject), Name = "PHASEEnvelope")]
	[DisableDefaultCtor]
	interface PhaseEnvelope {
		[Export ("initWithStartPoint:segments:")]
		[DesignatedInitializer]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		NativeHandle Constructor (Vector2d startPoint, PhaseEnvelopeSegment [] segments);

		[Export ("evaluateForValue:")]
		double Evaluate (double x);

		[Export ("startPoint")]
		Vector2d StartPoint {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}

		[Export ("segments", ArgumentSemantic.Copy)]
		PhaseEnvelopeSegment [] Segments { get; }

		[Export ("domain", ArgumentSemantic.Strong)]
		PhaseNumericPair Domain { get; }

		[Export ("range", ArgumentSemantic.Strong)]
		PhaseNumericPair Range { get; }
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject), Name = "PHASEDefinition")]
	[DisableDefaultCtor]
	interface PhaseDefinition {
		[Export ("identifier")]
		string Identifier { get; }
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (PhaseDefinition), Name = "PHASEMetaParameterDefinition")]
	[DisableDefaultCtor]
	interface PhaseMetaParameterDefinition {
		[Export ("value", ArgumentSemantic.Strong)]
		NSObject Value { get; }
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (PhaseMetaParameterDefinition), Name = "PHASENumberMetaParameterDefinition")]
	[DisableDefaultCtor]
	interface PhaseNumberMetaParameterDefinition {
		[Export ("initWithValue:identifier:")]
		NativeHandle Constructor (double value, string identifier);

		[Export ("initWithValue:")]
		NativeHandle Constructor (double value);

		[Export ("initWithValue:minimum:maximum:identifier:")]
		NativeHandle Constructor (double value, double minimum, double maximum, string identifier);

		[Export ("initWithValue:minimum:maximum:")]
		[DesignatedInitializer]
		NativeHandle Constructor (double value, double minimum, double maximum);

		[Export ("minimum")]
		double Minimum { get; }

		[Export ("maximum")]
		double Maximum { get; }
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (PhaseMetaParameterDefinition), Name = "PHASEStringMetaParameterDefinition")]
	[DisableDefaultCtor]
	interface PhaseStringMetaParameterDefinition {
		[Export ("initWithValue:identifier:")]
		NativeHandle Constructor (string value, string identifier);

		[Export ("initWithValue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string value);
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (PhaseNumberMetaParameterDefinition), Name = "PHASEMappedMetaParameterDefinition")]
	[DisableDefaultCtor]
	interface PhaseMappedMetaParameterDefinition {
		[Export ("initWithValue:identifier:")]
		NativeHandle Constructor (double value, string identifier);

		[Export ("initWithValue:minimum:maximum:")]
		[DesignatedInitializer]
		NativeHandle Constructor (double value, double minimum, double maximum);

		[Export ("initWithInputMetaParameterDefinition:envelope:identifier:")]
		NativeHandle Constructor (PhaseNumberMetaParameterDefinition inputMetaParameterDefinition, PhaseEnvelope envelope, string identifier);

		[Export ("initWithInputMetaParameterDefinition:envelope:")]
		[DesignatedInitializer]
		NativeHandle Constructor (PhaseNumberMetaParameterDefinition inputMetaParameterDefinition, PhaseEnvelope envelope);

		[Export ("envelope", ArgumentSemantic.Strong)]
		PhaseEnvelope Envelope { get; }

		[Export ("inputMetaParameterDefinition", ArgumentSemantic.Strong)]
		PhaseNumberMetaParameterDefinition InputMetaParameterDefinition { get; }
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject), Name = "PHASEMetaParameter")]
	[DisableDefaultCtor]
	interface PhaseMetaParameter {
		[Export ("identifier", ArgumentSemantic.Strong)]
		string Identifier { get; }

		[Export ("value", ArgumentSemantic.Strong)]
		NSObject Value { get; set; }
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (PhaseMetaParameter), Name = "PHASEStringMetaParameter")]
	[DisableDefaultCtor]
	interface PhaseStringMetaParameter { }

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (PhaseMetaParameter), Name = "PHASENumberMetaParameter")]
	[DisableDefaultCtor]
	interface PhaseNumberMetaParameter {
		[Export ("minimum")]
		double Minimum { get; }

		[Export ("maximum")]
		double Maximum { get; }

		[Export ("fadeToValue:duration:")]
		void Fade (double value, double duration);
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (PhaseDefinition), Name = "PHASEMixerDefinition")]
	[DisableDefaultCtor]
	interface PhaseMixerDefinition {
		[Export ("gain")]
		double Gain { get; set; }

		[NullAllowed, Export ("gainMetaParameterDefinition", ArgumentSemantic.Strong)]
		PhaseNumberMetaParameterDefinition GainMetaParameterDefinition { get; set; }
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (PhaseMixerDefinition), Name = "PHASESpatialMixerDefinition")]
	[DisableDefaultCtor]
	interface PhaseSpatialMixerDefinition {
		[Export ("initWithSpatialPipeline:")]
		[DesignatedInitializer]
		NativeHandle Constructor (PhaseSpatialPipeline spatialPipeline);

		[Export ("initWithSpatialPipeline:identifier:")]
		NativeHandle Constructor (PhaseSpatialPipeline spatialPipeline, string identifier);

		[Export ("spatialPipeline", ArgumentSemantic.Strong)]
		PhaseSpatialPipeline SpatialPipeline { get; }

		[NullAllowed, Export ("distanceModelParameters", ArgumentSemantic.Strong)]
		PhaseDistanceModelParameters DistanceModelParameters { get; set; }

		[NullAllowed, Export ("listenerDirectivityModelParameters", ArgumentSemantic.Strong)]
		PhaseDirectivityModelParameters ListenerDirectivityModelParameters { get; set; }

		[NullAllowed, Export ("sourceDirectivityModelParameters", ArgumentSemantic.Strong)]
		PhaseDirectivityModelParameters SourceDirectivityModelParameters { get; set; }
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (PhaseMixerDefinition), Name = "PHASEAmbientMixerDefinition")]
	[DisableDefaultCtor]
	interface PhaseAmbientMixerDefinition {
		[Export ("initWithChannelLayout:orientation:identifier:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		NativeHandle Constructor (AVAudioChannelLayout layout, Quaternion orientation, NSString identifier);

		[Export ("initWithChannelLayout:orientation:")]
		[DesignatedInitializer]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		NativeHandle Constructor (AVAudioChannelLayout layout, Quaternion orientation);

		[Export ("orientation")]
		Quaternion Orientation {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}

		[Export ("inputChannelLayout", ArgumentSemantic.Strong)]
		AVAudioChannelLayout InputChannelLayout { get; }
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (PhaseMixerDefinition), Name = "PHASEChannelMixerDefinition")]
	[DisableDefaultCtor]
	interface PhaseChannelMixerDefinition {
		[Export ("initWithChannelLayout:identifier:")]
		NativeHandle Constructor (AVAudioChannelLayout layout, string identifier);

		[Export ("initWithChannelLayout:")]
		[DesignatedInitializer]
		NativeHandle Constructor (AVAudioChannelLayout layout);

		[Export ("inputChannelLayout", ArgumentSemantic.Strong)]
		AVAudioChannelLayout InputChannelLayout { get; }
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject), Name = "PHASEMixer")]
	[DisableDefaultCtor]
	interface PhaseMixer {
		[Export ("identifier", ArgumentSemantic.Strong)]
		string Identifier { get; }

		[Export ("gain")]
		double Gain { get; }

		[NullAllowed, Export ("gainMetaParameter", ArgumentSemantic.Strong)]
		PhaseMetaParameter GainMetaParameter { get; }
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject), Name = "PHASEMixerParameters")]
#if TVOS
	[DisableDefaultCtor]
#endif
	interface PhaseMixerParameters {
		[Export ("addSpatialMixerParametersWithIdentifier:source:listener:")]
		void AddSpatialMixerParameters (string identifier, PhaseSource source, PhaseListener listener);

		[Export ("addAmbientMixerParametersWithIdentifier:listener:")]
		void AddAmbientMixerParameters (string identifier, PhaseListener listener);
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject), Name = "PHASEGroup")]
	[DisableDefaultCtor]
	interface PhaseGroup {
		[Export ("initWithIdentifier:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string identifier);

		[Export ("registerWithEngine:")]
		void Register (PhaseEngine engine);

		[Export ("unregisterFromEngine")]
		void Unregister ();

		[Export ("fadeGain:duration:curveType:")]
		void FadeGain (double gain, double duration, PhaseCurveType curveType);

		[Export ("fadeRate:duration:curveType:")]
		void FadeRate (double rate, double duration, PhaseCurveType curveType);

		[Export ("mute")]
		void Mute ();

		[Export ("unmute")]
		void Unmute ();

		[Export ("solo")]
		void Solo ();

		[Export ("unsolo")]
		void Unsolo ();

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("gain")]
		double Gain { get; set; }

		[Export ("rate")]
		double Rate { get; set; }

		[Export ("muted")]
		bool Muted { [Bind ("isMuted")] get; }

		[Export ("soloed")]
		bool Soloed { [Bind ("isSoloed")] get; }
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (PhaseDefinition), Name = "PHASESoundEventNodeDefinition")]
	[DisableDefaultCtor]
	interface PhaseSoundEventNodeDefinition {
		[Export ("children", ArgumentSemantic.Copy)]
		PhaseSoundEventNodeDefinition [] Children { get; }
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (PhaseSoundEventNodeDefinition), Name = "PHASEGeneratorNodeDefinition")]
	[DisableDefaultCtor]
	interface PhaseGeneratorNodeDefinition {
		[Export ("setCalibrationMode:level:")]
		void SetCalibrationMode (PhaseCalibrationMode calibrationMode, double level);

		[Export ("calibrationMode")]
		PhaseCalibrationMode CalibrationMode { get; }

		[Export ("level")]
		double Level { get; }

		[Export ("rate")]
		double Rate { get; set; }

		[NullAllowed, Export ("group", ArgumentSemantic.Weak)]
		PhaseGroup Group { get; set; }

		[NullAllowed, Export ("gainMetaParameterDefinition", ArgumentSemantic.Strong)]
		PhaseNumberMetaParameterDefinition GainMetaParameterDefinition { get; set; }

		[NullAllowed, Export ("rateMetaParameterDefinition", ArgumentSemantic.Strong)]
		PhaseNumberMetaParameterDefinition RateMetaParameterDefinition { get; set; }

		[Export ("mixerDefinition", ArgumentSemantic.Strong)]
		PhaseMixerDefinition MixerDefinition { get; }
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (PhaseGeneratorNodeDefinition), Name = "PHASESamplerNodeDefinition")]
	[DisableDefaultCtor]
	interface PhaseSamplerNodeDefinition {
		[Export ("initWithSoundAssetIdentifier:mixerDefinition:identifier:")]
		NativeHandle Constructor (string soundAssetIdentifier, PhaseMixerDefinition mixerDefinition, string identifier);

		[Export ("initWithSoundAssetIdentifier:mixerDefinition:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string soundAssetIdentifier, PhaseMixerDefinition mixerDefinition);

		[Export ("assetIdentifier", ArgumentSemantic.Strong)]
		string AssetIdentifier { get; }

		[Export ("cullOption", ArgumentSemantic.Assign)]
		PhaseCullOption CullOption { get; set; }

		[Export ("playbackMode", ArgumentSemantic.Assign)]
		PhasePlaybackMode PlaybackMode { get; set; }
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (PhaseSoundEventNodeDefinition), Name = "PHASEContainerNodeDefinition")]
#if TVOS
	[DisableDefaultCtor]
#endif
	interface PhaseContainerNodeDefinition {
		[Static]
		[Export ("new")]
		[return: Release]
		PhaseContainerNodeDefinition Create ();

		[Export ("initWithIdentifier:")]
		NativeHandle Constructor (string identifier);

		[Export ("addSubtree:")]
		void Add (PhaseSoundEventNodeDefinition subtree);
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (PhaseSoundEventNodeDefinition), Name = "PHASEBlendNodeDefinition")]
	[DisableDefaultCtor]
	interface PhaseBlendNodeDefinition {
		[Export ("initWithBlendMetaParameterDefinition:identifier:")]
		NativeHandle Constructor (PhaseNumberMetaParameterDefinition blendMetaParameterDefinition, string identifier);

		[Export ("initWithBlendMetaParameterDefinition:")]
		[DesignatedInitializer]
		NativeHandle Constructor (PhaseNumberMetaParameterDefinition blendMetaParameterDefinition);

		[Export ("initDistanceBlendWithSpatialMixerDefinition:identifier:")]
		NativeHandle Constructor (PhaseSpatialMixerDefinition spatialMixerDefinition, string identifier);

		[Export ("initDistanceBlendWithSpatialMixerDefinition:")]
		[DesignatedInitializer]
		NativeHandle Constructor (PhaseSpatialMixerDefinition spatialMixerDefinition);

		[NullAllowed, Export ("blendParameterDefinition", ArgumentSemantic.Strong)]
		PhaseNumberMetaParameterDefinition BlendParameterDefinition { get; }

		[NullAllowed, Export ("spatialMixerDefinitionForDistance", ArgumentSemantic.Strong)]
		PhaseSpatialMixerDefinition SpatialMixerDefinitionForDistance { get; }

		[Export ("addRangeForInputValuesBelow:fullGainAtValue:fadeCurveType:subtree:")]
		void AddRangeForInputValuesBelow (double value, double fullGainAtValue, PhaseCurveType fadeCurveType, PhaseSoundEventNodeDefinition subtree);

		[Export ("addRangeForInputValuesBetween:highValue:fullGainAtLowValue:fullGainAtHighValue:lowFadeCurveType:highFadeCurveType:subtree:")]
		void AddRangeForInputValuesBetween (double lowValue, double highValue, double fullGainAtLowValue, double fullGainAtHighValue, PhaseCurveType lowFadeCurveType, PhaseCurveType highFadeCurveType, PhaseSoundEventNodeDefinition subtree);

		[Export ("addRangeForInputValuesAbove:fullGainAtValue:fadeCurveType:subtree:")]
		void AddRangeForInputValuesAbove (double value, double fullGainAtValue, PhaseCurveType fadeCurveType, PhaseSoundEventNodeDefinition subtree);

		[Export ("addRangeWithEnvelope:subtree:")]
		void AddRange (PhaseEnvelope envelope, PhaseSoundEventNodeDefinition subtree);
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (PhaseSoundEventNodeDefinition), Name = "PHASESwitchNodeDefinition")]
	[DisableDefaultCtor]
	interface PhaseSwitchNodeDefinition {
		[Export ("initWithSwitchMetaParameterDefinition:identifier:")]
		NativeHandle Constructor (PhaseStringMetaParameterDefinition switchMetaParameterDefinition, string identifier);

		[Export ("initWithSwitchMetaParameterDefinition:")]
		[DesignatedInitializer]
		NativeHandle Constructor (PhaseStringMetaParameterDefinition switchMetaParameterDefinition);

		[Export ("addSubtree:switchValue:")]
		void AddSubtree (PhaseSoundEventNodeDefinition subtree, string switchValue);

		[Export ("switchMetaParameterDefinition", ArgumentSemantic.Strong)]
		PhaseStringMetaParameterDefinition SwitchMetaParameterDefinition { get; }
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (PhaseSoundEventNodeDefinition), Name = "PHASERandomNodeDefinition")]
	[DisableDefaultCtor]
	interface PhaseRandomNodeDefinition {
		[Export ("init")]
		NativeHandle Constructor ();

		[Export ("initWithIdentifier:")]
		NativeHandle Constructor (string identifier);

		[Export ("addSubtree:weight:")]
		void AddSubtree (PhaseSoundEventNodeDefinition subtree, NSNumber weight);

		[Export ("uniqueSelectionQueueLength")]
		nint UniqueSelectionQueueLength { get; set; }
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (PhaseGeneratorNodeDefinition), Name = "PHASEPushStreamNodeDefinition")]
	[DisableDefaultCtor]
	interface PhasePushStreamNodeDefinition {
		[Export ("initWithMixerDefinition:format:identifier:")]
		NativeHandle Constructor (PhaseMixerDefinition mixerDefinition, AVAudioFormat format, string identifier);

		[Export ("initWithMixerDefinition:format:")]
		[DesignatedInitializer]
		NativeHandle Constructor (PhaseMixerDefinition mixerDefinition, AVAudioFormat format);

		[Export ("format", ArgumentSemantic.Strong)]
		AVAudioFormat Format { get; }

		[Export ("normalize")]
		bool Normalize { get; set; }
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject), Name = "PHASEPushStreamNode")]
	[DisableDefaultCtor]
	interface PhasePushStreamNode {
		[NullAllowed, Export ("gainMetaParameter", ArgumentSemantic.Strong)]
		PhaseNumberMetaParameter GainMetaParameter { get; }

		[NullAllowed, Export ("rateMetaParameter", ArgumentSemantic.Strong)]
		PhaseNumberMetaParameter RateMetaParameter { get; }

		[Export ("mixer", ArgumentSemantic.Strong)]
		PhaseMixer Mixer { get; }

		[Export ("format", ArgumentSemantic.Strong)]
		AVAudioFormat Format { get; }

		[Export ("scheduleBuffer:")]
		void ScheduleBuffer (AVAudioPcmBuffer buffer);

		[Async]
		[Export ("scheduleBuffer:completionCallbackType:completionHandler:")]
		void ScheduleBuffer (AVAudioPcmBuffer buffer, PhasePushStreamCompletionCallbackCondition completionCallbackCondition, Action<PhasePushStreamCompletionCallbackCondition> completionHandler);

		[Export ("scheduleBuffer:atTime:options:")]
		void ScheduleBuffer (AVAudioPcmBuffer buffer, [NullAllowed] AVAudioTime when, PhasePushStreamBufferOptions options);

		[Async]
		[Export ("scheduleBuffer:atTime:options:completionCallbackType:completionHandler:")]
		void ScheduleBuffer (AVAudioPcmBuffer buffer, [NullAllowed] AVAudioTime when, PhasePushStreamBufferOptions options, PhasePushStreamCompletionCallbackCondition completionCallbackCondition, Action<PhasePushStreamCompletionCallbackCondition> completionHandler);
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject), Name = "PHASEAsset")]
	[DisableDefaultCtor]
	interface PhaseAsset {
		[Export ("identifier")]
		string Identifier { get; }
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (PhaseAsset), Name = "PHASEGlobalMetaParameterAsset")]
	[DisableDefaultCtor]
	interface PhaseGlobalMetaParameterAsset { }

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (PhaseAsset), Name = "PHASESoundAsset")]
	[DisableDefaultCtor]
	interface PhaseSoundAsset {
		[NullAllowed, Export ("url")]
		NSUrl Url { get; }

		[NullAllowed, Export ("data")]
		NSData Data { get; }

		[Export ("type")]
		PhaseAssetType Type { get; }
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (PhaseAsset), Name = "PHASESoundEventNodeAsset")]
	[DisableDefaultCtor]
	interface PhaseSoundEventNodeAsset { }

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject), Name = "PHASEAssetRegistry")]
	[DisableDefaultCtor]
	interface PhaseAssetRegistry {
		[Export ("registerGlobalMetaParameter:error:")]
		[return: NullAllowed]
		PhaseGlobalMetaParameterAsset RegisterGlobalMetaParameter (PhaseMetaParameterDefinition metaParameterDefinition, [NullAllowed] out NSError error);

		[Export ("registerSoundEventAssetWithRootNode:identifier:error:")]
		[return: NullAllowed]
		PhaseSoundEventNodeAsset RegisterSoundEventAsset (PhaseSoundEventNodeDefinition rootNode, [NullAllowed] string identifier, [NullAllowed] out NSError error);

		[Async]
		[Export ("unregisterAssetWithIdentifier:completion:")]
		void UnregisterAsset (string identifier, [NullAllowed] Action<bool> handler);

		[Export ("registerSoundAssetAtURL:identifier:assetType:channelLayout:normalizationMode:error:")]
		[return: NullAllowed]
		PhaseSoundAsset RegisterSoundAsset (NSUrl url, [NullAllowed] string identifier, PhaseAssetType assetType, [NullAllowed] AVAudioChannelLayout channelLayout, PhaseNormalizationMode normalizationMode, [NullAllowed] out NSError error);

		[Export ("registerSoundAssetWithData:identifier:format:normalizationMode:error:")]
		[return: NullAllowed]
		PhaseSoundAsset RegisterSoundAsset (NSData data, [NullAllowed] string identifier, AVAudioFormat format, PhaseNormalizationMode normalizationMode, [NullAllowed] out NSError error);

		[Export ("assetForIdentifier:")]
		[return: NullAllowed]
		PhaseAsset GetAsset (string identifier);

		[Export ("globalMetaParameters", ArgumentSemantic.Copy)]
		NSDictionary<NSString, PhaseMetaParameter> GlobalMetaParameters { get; }
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject), Name = "PHASECardioidDirectivityModelSubbandParameters")]
#if TVOS
	[DisableDefaultCtor]
#endif
	interface PhaseCardioidDirectivityModelSubbandParameters {
		[Export ("frequency")]
		double Frequency { get; set; }

		[Export ("pattern")]
		double Pattern { get; set; }

		[Export ("sharpness")]
		double Sharpness { get; set; }
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject), Name = "PHASEConeDirectivityModelSubbandParameters")]
#if TVOS
	[DisableDefaultCtor]
#endif
	interface PhaseConeDirectivityModelSubbandParameters {
		[Export ("setInnerAngle:outerAngle:")]
		void SetInnerAngle (double innerAngle, double outerAngle);

		[Export ("frequency")]
		double Frequency { get; set; }

		[Export ("innerAngle")]
		double InnerAngle { get; }

		[Export ("outerAngle")]
		double OuterAngle { get; }

		[Export ("outerGain")]
		double OuterGain { get; set; }
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject), Name = "PHASEDirectivityModelParameters")]
	[DisableDefaultCtor]
	interface PhaseDirectivityModelParameters { }

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (PhaseDirectivityModelParameters), Name = "PHASECardioidDirectivityModelParameters")]
	[DisableDefaultCtor]
	interface PhaseCardioidDirectivityModelParameters {
		[Export ("initWithSubbandParameters:")]
		NativeHandle Constructor (PhaseCardioidDirectivityModelSubbandParameters [] subbandParameters);

		[Export ("subbandParameters", ArgumentSemantic.Strong)]
		PhaseCardioidDirectivityModelSubbandParameters [] SubbandParameters { get; }
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (PhaseDirectivityModelParameters), Name = "PHASEConeDirectivityModelParameters")]
#if TVOS
	[DisableDefaultCtor]
#endif
	interface PhaseConeDirectivityModelParameters {
		[Export ("initWithSubbandParameters:")]
		NativeHandle Constructor (PhaseConeDirectivityModelSubbandParameters [] subbandParameters);

		[Export ("subbandParameters", ArgumentSemantic.Strong)]
		PhaseConeDirectivityModelSubbandParameters [] SubbandParameters { get; }
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject), Name = "PHASEDistanceModelFadeOutParameters")]
	[DisableDefaultCtor]
	interface PhaseDistanceModelFadeOutParameters {
		[Export ("initWithCullDistance:")]
		[DesignatedInitializer]
		NativeHandle Constructor (double cullDistance);

		[Export ("cullDistance")]
		double CullDistance { get; }
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject), Name = "PHASEDistanceModelParameters")]
	[DisableDefaultCtor]
	interface PhaseDistanceModelParameters {
		[NullAllowed, Export ("fadeOutParameters", ArgumentSemantic.Strong)]
		PhaseDistanceModelFadeOutParameters FadeOutParameters { get; set; }
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (PhaseDistanceModelParameters), Name = "PHASEGeometricSpreadingDistanceModelParameters")]
#if TVOS
	[DisableDefaultCtor]
#endif
	interface PhaseGeometricSpreadingDistanceModelParameters {
		[Export ("rolloffFactor")]
		double RolloffFactor { get; set; }
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (PhaseDistanceModelParameters), Name = "PHASEEnvelopeDistanceModelParameters")]
	[DisableDefaultCtor]
	interface PhaseEnvelopeDistanceModelParameters {
		[Export ("initWithEnvelope:")]
		[DesignatedInitializer]
		NativeHandle Constructor (PhaseEnvelope envelope);

		[Export ("envelope", ArgumentSemantic.Strong)]
		PhaseEnvelope Envelope { get; }
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject), Name = "PHASEDucker")]
	[DisableDefaultCtor]
	interface PhaseDucker {
		[Export ("initWithEngine:sourceGroups:targetGroups:gain:attackTime:releaseTime:attackCurve:releaseCurve:")]
		NativeHandle Constructor (PhaseEngine engine, NSSet<PhaseGroup> sourceGroups, NSSet<PhaseGroup> targetGroups, double gain, double attackTime, double releaseTime, PhaseCurveType attackCurve, PhaseCurveType releaseCurve);

		[Export ("activate")]
		void Activate ();

		[Export ("deactivate")]
		void Deactivate ();

		[Export ("isActive")]
		bool IsActive { get; }

		[Export ("sourceGroups", ArgumentSemantic.Copy)]
		NSSet<PhaseGroup> SourceGroups { get; }

		[Export ("targetGroups", ArgumentSemantic.Copy)]
		NSSet<PhaseGroup> TargetGroups { get; }

		[Export ("gain")]
		double Gain { get; }

		[Export ("attackTime")]
		double AttackTime { get; }

		[Export ("releaseTime")]
		double ReleaseTime { get; }

		[Export ("attackCurve")]
		PhaseCurveType AttackCurve { get; }

		[Export ("releaseCurve")]
		PhaseCurveType ReleaseCurve { get; }

		[Export ("identifier")]
		string Identifier { get; }
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject), Name = "PHASEGroupPresetSetting")]
	[DisableDefaultCtor]
	interface PhaseGroupPresetSetting {
		[Export ("initWithGain:rate:gainCurveType:rateCurveType:")]
		NativeHandle Constructor (double gain, double rate, PhaseCurveType gainCurveType, PhaseCurveType rateCurveType);

		[Export ("gain")]
		double Gain { get; }

		[Export ("rate")]
		double Rate { get; }

		[Export ("gainCurveType")]
		PhaseCurveType GainCurveType { get; }

		[Export ("rateCurveType")]
		PhaseCurveType RateCurveType { get; }
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject), Name = "PHASEGroupPreset")]
	[DisableDefaultCtor]
	interface PhaseGroupPreset {
		[Export ("initWithEngine:settings:timeToTarget:timeToReset:")]
		[DesignatedInitializer]
		NativeHandle Constructor (PhaseEngine engine, NSDictionary<NSString, PhaseGroupPresetSetting> settings, double timeToTarget, double timeToReset);

		[Export ("settings")]
		NSDictionary<NSString, PhaseGroupPresetSetting> Settings { get; }

		[Export ("timeToTarget")]
		double TimeToTarget { get; }

		[Export ("timeToReset")]
		double TimeToReset { get; }

		[Export ("activate")]
		void Activate ();

		[Export ("activateWithTimeToTargetOverride:")]
		void Activate (double timeToTargetOverride);

		[Export ("deactivate")]
		void Deactivate ();

		[Export ("deactivateWithTimeToResetOverride:")]
		void Deactivate (double timeToResetOverride);
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject), Name = "PHASEMedium")]
	[DisableDefaultCtor]
	interface PhaseMedium {
		[Export ("initWithEngine:preset:")]
		NativeHandle Constructor (PhaseEngine engine, PhaseMediumPreset preset);
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject), Name = "PHASEObject")]
	[DisableDefaultCtor]
	interface PhaseObject : NSCopying {
		[Export ("initWithEngine:")]
		[DesignatedInitializer]
		NativeHandle Constructor (PhaseEngine engine);

		[Export ("addChild:error:")]
		bool AddChild (PhaseObject child, [NullAllowed] out NSError error);

		[Export ("removeChild:")]
		void RemoveChild (PhaseObject child);

		[Export ("removeChildren")]
		void RemoveChildren ();

		[NullAllowed, Export ("parent", ArgumentSemantic.Weak)]
		PhaseObject Parent { get; }

		[Export ("children", ArgumentSemantic.Copy)]
		PhaseObject [] Children { get; }

		[Static]
		[Export ("right")]
		Vector3 Right {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}

		[Static]
		[Export ("up")]
		Vector3 Up {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}

		[Static]
		[Export ("forward")]
		Vector3 Forward {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}

		[Export ("transform", ArgumentSemantic.Assign)]
		NMatrix4 Transform {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			set;
		}

		[Export ("worldTransform", ArgumentSemantic.Assign)]
		NMatrix4 WorldTransform {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			set;
		}
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject), Name = "PHASESoundEvent")]
	[DisableDefaultCtor]
	interface PhaseSoundEvent {
		[Export ("initWithEngine:assetIdentifier:mixerParameters:error:")]
		NativeHandle Constructor (PhaseEngine engine, string assetIdentifier, PhaseMixerParameters mixerParameters, [NullAllowed] out NSError error);

		[Export ("initWithEngine:assetIdentifier:error:")]
		NativeHandle Constructor (PhaseEngine engine, string assetIdentifier, [NullAllowed] out NSError error);

		[Async]
		[Export ("prepareWithCompletion:")]
		void Prepare ([NullAllowed] Action<PhaseSoundEventPrepareHandlerReason> completionBlock);

		[Export ("prepareAndReturnError:")]
		bool Prepare ([NullAllowed] out NSError error);

		[Async]
		[Export ("startWithCompletion:")]
		bool Start ([NullAllowed] Action<PhaseSoundEventStartHandlerReason> completionBlock);

		[Export ("startAndReturnError:")]
		bool Start ([NullAllowed] out NSError error);

		[Async]
		[Export ("seekToTime:completion:")]
		bool Seek (double time, [NullAllowed] Action<PhaseSoundEventSeekHandlerReason> completionHandler);

		[Export ("pause")]
		void Pause ();

		[Export ("resume")]
		void Resume ();

		[Export ("stopAndInvalidate")]
		void StopAndInvalidate ();

		[Export ("renderingState")]
		PhaseRenderingState RenderingState { get; }

		[Export ("prepareState")]
		PhaseSoundEventPrepareState PrepareState { get; }

		[Export ("metaParameters", ArgumentSemantic.Copy)]
		NSDictionary<NSString, PhaseMetaParameter> MetaParameters { get; }

		[Export ("mixers", ArgumentSemantic.Copy)]
		NSDictionary<NSString, PhaseMixer> Mixers { get; }

		[Export ("pushStreamNodes", ArgumentSemantic.Copy)]
		NSDictionary<NSString, PhasePushStreamNode> PushStreamNodes { get; }

		[Export ("indefinite")]
		bool Indefinite { [Bind ("isIndefinite")] get; }
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject), Name = "PHASEEngine")]
	[DisableDefaultCtor]
	interface PhaseEngine {
		[Export ("initWithUpdateMode:")]
		[DesignatedInitializer]
		NativeHandle Constructor (PhaseUpdateMode updateMode);

		[Export ("startAndReturnError:")]
		bool Start ([NullAllowed] out NSError error);

		[Export ("pause")]
		void Pause ();

		[Export ("stop")]
		void Stop ();

		[Export ("update")]
		void Update ();

		[Export ("outputSpatializationMode", ArgumentSemantic.Assign)]
		PhaseSpatializationMode OutputSpatializationMode { get; set; }

		[Export ("renderingState")]
		PhaseRenderingState RenderingState { get; }

		[Export ("rootObject", ArgumentSemantic.Strong)]
		PhaseObject RootObject { get; }

		[Export ("defaultMedium", ArgumentSemantic.Strong)]
		PhaseMedium DefaultMedium { get; set; }

		[Export ("defaultReverbPreset", ArgumentSemantic.Assign)]
		PhaseReverbPreset DefaultReverbPreset { get; set; }

		[Export ("unitsPerSecond")]
		double UnitsPerSecond { get; set; }

		[Export ("unitsPerMeter")]
		double UnitsPerMeter { get; set; }

		[Export ("assetRegistry", ArgumentSemantic.Strong)]
		PhaseAssetRegistry AssetRegistry { get; }

		[Export ("soundEvents", ArgumentSemantic.Copy)]
		PhaseSoundEvent [] SoundEvents { get; }

		[Export ("groups", ArgumentSemantic.Copy)]
		NSDictionary<NSString, PhaseGroup> Groups { get; }

		[Export ("duckers", ArgumentSemantic.Copy)]
		PhaseDucker [] Duckers { get; }

		[NullAllowed, Export ("activeGroupPreset", ArgumentSemantic.Strong)]
		PhaseGroupPreset ActiveGroupPreset { get; }
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (PhaseObject), Name = "PHASEListener")]
	[DisableDefaultCtor]
	interface PhaseListener {
		[Export ("initWithEngine:")]
		[DesignatedInitializer]
		NativeHandle Constructor (PhaseEngine engine);

		[Export ("gain")]
		double Gain { get; set; }
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject), Name = "PHASEMaterial")]
	[DisableDefaultCtor]
	interface PhaseMaterial {
		[Export ("initWithEngine:preset:")]
		NativeHandle Constructor (PhaseEngine engine, PhaseMaterialPreset preset);
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject), Name = "PHASEShapeElement")]
	[DisableDefaultCtor]
	interface PhaseShapeElement {
		[NullAllowed, Export ("material", ArgumentSemantic.Strong)]
		PhaseMaterial Material { get; set; }
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject), Name = "PHASEShape")]
	[DisableDefaultCtor]
	interface PhaseShape : NSCopying {
		[Export ("initWithEngine:mesh:")]
		[DesignatedInitializer]
		NativeHandle Constructor (PhaseEngine engine, MDLMesh mesh);

		[Export ("initWithEngine:mesh:materials:")]
		NativeHandle Constructor (PhaseEngine engine, MDLMesh mesh, PhaseMaterial [] materials);

		[Export ("elements", ArgumentSemantic.Copy)]
		PhaseShapeElement [] Elements { get; }
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (PhaseObject), Name = "PHASEOccluder")]
	[DisableDefaultCtor]
	interface PhaseOccluder {
		[Export ("initWithEngine:")]
		[DesignatedInitializer]
		NativeHandle Constructor (PhaseEngine engine);

		[Export ("initWithEngine:shapes:")]
		[DesignatedInitializer]
		NativeHandle Constructor (PhaseEngine engine, PhaseShape [] shapes);

		[Export ("shapes", ArgumentSemantic.Copy)]
		PhaseShape [] Shapes { get; }
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (PhaseObject), Name = "PHASESource")]
	[DisableDefaultCtor]
	interface PhaseSource {
		[Export ("initWithEngine:")]
		[DesignatedInitializer]
		NativeHandle Constructor (PhaseEngine engine);

		[Export ("initWithEngine:shapes:")]
		[DesignatedInitializer]
		NativeHandle Constructor (PhaseEngine engine, PhaseShape [] shapes);

		[Export ("gain")]
		double Gain { get; set; }

		[Export ("shapes", ArgumentSemantic.Copy)]
		PhaseShape [] Shapes { get; }
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject), Name = "PHASESpatialPipelineEntry")]
#if TVOS
	[DisableDefaultCtor]
#endif
	interface PhaseSpatialPipelineEntry {
		[Export ("sendLevel")]
		double SendLevel { get; set; }

		[NullAllowed, Export ("sendLevelMetaParameterDefinition", ArgumentSemantic.Strong)]
		PhaseNumberMetaParameterDefinition SendLevelMetaParameterDefinition { get; set; }
	}

	[NoWatch, TV (17, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject), Name = "PHASESpatialPipeline")]
	[DisableDefaultCtor]
	interface PhaseSpatialPipeline {
		[Export ("initWithFlags:")]
		[DesignatedInitializer]
		NativeHandle Constructor (PhaseSpatialPipelineFlags flags);

		// @property (readonly, nonatomic) PHASESpatialPipelineFlags flags;
		[Export ("flags")]
		PhaseSpatialPipelineFlags Flags { get; }

		[Export ("entries", ArgumentSemantic.Copy)]
		NSDictionary<NSString, PhaseSpatialPipelineEntry> Entries { get; }
	}
}
