//
// CoreHaptics C# bindings
//
// Authors:
//	Manuel de la Pena Saenz <mandel@microsoft.com>
//
// Copyright 2019 Microsoft Corporation All rights reserved.
//

using System;
	
using AVFoundation;
using CoreFoundation;
using Foundation;
using ObjCRuntime;

namespace CoreHaptics {

// we are not binding the API on Mac OS X yet due to an issue on Apples side: https://github.com/xamarin/maccore/issues/1951
#if MONOMAC
	interface AVAudioSession {}
#endif

	[Mac (10,15), iOS (13,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface CHHapticEventParameter {
		[BindAs (typeof (CHHapticEventParameterId))]
		[Export ("parameterID")]
		NSString ParameterId { get; }

		[Export ("value")]
		float Value { get; set; }

		[Export ("initWithParameterID:value:")]
		[DesignatedInitializer]
		IntPtr Constructor ([BindAs (typeof (CHHapticEventParameterId))] NSString parameterId, float value);
	}

	[Mac (10,15), iOS (13,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface CHHapticDynamicParameter {
		[BindAs (typeof (CHHapticDynamicParameterId))]
		[Export ("parameterID")]
		NSString ParameterId { get; }

		[Export ("value")]
		float Value { get; set; }

		[Export ("relativeTime")]
		double RelativeTime { get; set; }

		[Export ("initWithParameterID:value:relativeTime:")]
		[DesignatedInitializer]
		IntPtr Constructor ([BindAs (typeof (CHHapticDynamicParameterId))] NSString parameterId, float value, double time);
	}

	[Mac (10,15), iOS (13,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface CHHapticParameterCurveControlPoint {
		[Export ("relativeTime")]
		double RelativeTime { get; set; }

		[Export ("value")]
		float Value { get; set; }

		[Export ("initWithRelativeTime:value:")]
		[DesignatedInitializer]
		IntPtr Constructor (double time, float value);
	}

	[Mac (10,15), iOS (13,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface CHHapticParameterCurve {
		[BindAs (typeof (CHHapticDynamicParameterId))]
		[Export ("parameterID")]
		NSString ParameterId { get; }

		[Export ("relativeTime")]
		double RelativeTime { get; set; }

		[Export ("controlPoints")]
		CHHapticParameterCurveControlPoint[] ControlPoints { get; }

		[Export ("initWithParameterID:controlPoints:relativeTime:")]
		[DesignatedInitializer]
		IntPtr Constructor ([BindAs (typeof (CHHapticDynamicParameterId))]NSString parameterId, CHHapticParameterCurveControlPoint[] controlPoints, double relativeTime);
	}

	[Mac (10,15), iOS (13,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface CHHapticEvent {
		[BindAs (typeof (CHHapticEventType))]
		[Export ("type")]
		NSString Type { get; }

		[Export ("eventParameters")]
		CHHapticEventParameter[] EventParameters { get; }

		[Export ("relativeTime")]
		double RelativeTime { get; set; }

		[Export ("duration")]
		double Duration { get; set; }

		[Export ("initWithEventType:parameters:relativeTime:")]
		IntPtr Constructor ([BindAs (typeof (CHHapticEventType))] NSString type, CHHapticEventParameter[] eventParams, double time);

		[Export ("initWithEventType:parameters:relativeTime:duration:")]
		IntPtr Constructor ([BindAs (typeof (CHHapticEventType))] NSString type, CHHapticEventParameter[] eventParams, double time, double duration);

		[Export ("initWithAudioResourceID:parameters:relativeTime:")]
		IntPtr Constructor (nuint resourceId, CHHapticEventParameter[] eventParams, double time);

		[Export ("initWithAudioResourceID:parameters:relativeTime:duration:")]
		IntPtr Constructor (nuint resourceId, CHHapticEventParameter[] eventParams, double time, double duration);
	}

	interface ICHHapticParameterAttributes { }

	[Mac (10,15), iOS (13,0)]
	[Protocol]
	interface CHHapticParameterAttributes {
		[Abstract]
		[Export ("minValue")]
		float MinValue { get; }

		[Abstract]
		[Export ("maxValue")]
		float MaxValue { get; }

		[Abstract]
		[Export ("defaultValue")]
		float DefaultValue { get; }
	}

	interface ICHHapticDeviceCapability { } 

	[iOS (13,0)][NoMac]
	[Protocol]
	interface CHHapticDeviceCapability {
		[Abstract]
		[Export ("supportsHaptics")]
		bool SupportsHaptics { get; }

		[Abstract]
		[Export ("supportsAudio")]
		bool SupportsAudio { get; }

		// Protocols do not like BindAs yet we know is the enum CHHapticEventType
		[Abstract]
		[Export ("attributesForEventParameter:eventType:error:")]
		[return: NullAllowed]
		ICHHapticParameterAttributes GetAttributes (NSString eventParameter, string type, [NullAllowed] out NSError outError);

		// Protocols do not like BindAs yet we know is the enum CHHapticEventType
		[Abstract]
		[Export ("attributesForDynamicParameter:error:")]
		[return: NullAllowed]
		ICHHapticParameterAttributes GetAttributes (NSString eventParameter, [NullAllowed] out NSError outError);
	}

	interface ICHHapticPatternPlayer { }

	[Mac (10,15), iOS (13,0)]
	[Protocol]
	interface CHHapticPatternPlayer {
		[Abstract]
		[Export ("startAtTime:error:")]
		bool Start (double time, [NullAllowed] out NSError outError);

		[Abstract]
		[Export ("stopAtTime:error:")]
		bool Stop (double time, [NullAllowed] out NSError outError);

		[Abstract]
		[Export ("sendParameters:atTime:error:")]
		bool Send (CHHapticDynamicParameter[] parameters, double time, [NullAllowed] out NSError outError);

		[Abstract]
		[Export ("scheduleParameterCurve:atTime:error:")]
		bool Schedule (CHHapticParameterCurve parameterCurve, double time, [NullAllowed] out NSError outError);

		[Abstract]
		[Export ("cancelAndReturnError:")]
		bool Cancel ([NullAllowed] out NSError outError);

		[Abstract]
		[Export ("isMuted")]
		bool IsMuted { get; set; }
	}

	interface ICHHapticAdvancedPatternPlayer {}

	[Mac (10,15), iOS (13,0)]
	[Protocol]
	interface CHHapticAdvancedPatternPlayer : CHHapticPatternPlayer {
		[Abstract]
		[Export ("pauseAtTime:error:")]
		bool Pause (double time, [NullAllowed] out NSError outError);

		[Abstract]
		[Export ("resumeAtTime:error:")]
		bool Resume (double time, [NullAllowed] out NSError outError);

		[Abstract]
		[Export ("seekToOffset:error:")]
		bool Seek (double offsetTime, [NullAllowed] out NSError outError);

		[Abstract]
		[Export ("loopEnabled")]
		bool LoopEnabled { get; set; }

		[Abstract]
		[Export ("loopEnd")]
		double LoopEnd { get; set; }

		[Abstract]
		[Export ("playbackRate")]
		float PlaybackRate { get; set; }

		[Abstract]
		[Export ("completionHandler", ArgumentSemantic.Assign)]
		Action<NSError> CompletionHandler { get; set; }
	}

	[Mac (10,15), iOS (13,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface CHHapticEngine
	{
		[Static]
		[Export ("capabilitiesForHardware")]
		ICHHapticDeviceCapability GetHardwareCapabilities ();

		[Export ("currentTime")]
		double CurrentTime { get; }

		[Export ("stoppedHandler", ArgumentSemantic.Assign)]
		Action<CHHapticEngineStoppedReason> StoppedHandler { get; set; }

		[Export ("resetHandler", ArgumentSemantic.Assign)]
		Action ResetHandler { get; set; }

		[Export ("playsHapticsOnly")]
		bool PlaysHapticsOnly { get; set; }

		[Export ("isMutedForAudio")]
		bool IsMutedForAudio { get; set; }

		[Export ("isMutedForHaptics")]
		bool IsMutedForHaptics { get; set; }

		[Export ("autoShutdownEnabled")]
		bool AutoShutdownEnabled { [Bind ("isAutoShutdownEnabled")] get; set; }

		[Export ("initAndReturnError:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] out NSError error);

		[NoMac]
		[Export ("initWithAudioSession:error:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] AVAudioSession audioSession, [NullAllowed] out NSError error);

		[Async]
		[Export ("startWithCompletionHandler:")]
		void Start ([NullAllowed] Action<NSError> completionHandler);

		[Export ("startAndReturnError:")]
		bool Start ([NullAllowed] out NSError outError);

		[Async]
		[Export ("stopWithCompletionHandler:")]
		void Stop ([NullAllowed] Action<NSError> completionHandler);

		[Async]
		[Export ("notifyWhenPlayersFinished:")]
		void NotifyWhenPlayersFinished (Action<NSError> finishedHandler);

		[Export ("createPlayerWithPattern:error:")]
		[return: NullAllowed]
		ICHHapticPatternPlayer CreatePlayer (CHHapticPattern pattern, [NullAllowed] out NSError outError);

		[Export ("createAdvancedPlayerWithPattern:error:")]
		[return: NullAllowed]
		ICHHapticAdvancedPatternPlayer CreateAdvancedPlayer (CHHapticPattern pattern, [NullAllowed] out NSError outError);

		[Export ("registerAudioResource:options:error:")]
		nuint RegisterAudioResource (NSUrl resourceUrl, NSDictionary options, [NullAllowed] out NSError outError);

		[Export ("unregisterAudioResource:error:")]
		bool UnregisterAudioResource (nuint resourceId, [NullAllowed] out NSError outError);

		[Export ("playPatternFromURL:error:")]
		bool PlayPattern (NSUrl fileUrl, [NullAllowed] out NSError outError);

		[Export ("playPatternFromData:error:")]
		bool PlayPattern (NSData data, [NullAllowed] out NSError outError);
	}

	[Static]
	[Internal]
	[Mac (10,15), iOS (13,0)]
	partial interface CHHapticPatternDefinitionKeys {
		[Field ("CHHapticPatternKeyVersion")]
		NSString VersionKey { get; }

		[Field ("CHHapticPatternKeyPattern")]
		NSString PatternKey { get; }

		[Field ("CHHapticPatternKeyEvent")]
		NSString EventKey { get; }

		[Field ("CHHapticPatternKeyEventType")]
		NSString EventTypeKey { get; }

		[Field ("CHHapticPatternKeyTime")]
		NSString TimeKey { get; }

		[Field ("CHHapticPatternKeyEventDuration")]
		NSString EventDurationKey { get; }

		[Field ("CHHapticPatternKeyEventWaveformPath")]
		NSString EventWaveformPathKey { get; }

		[Field ("CHHapticPatternKeyEventParameters")]
		NSString EventParametersKey { get; }

		[Field ("CHHapticPatternKeyParameter")]
		NSString ParameterKey { get; }

		[Field ("CHHapticPatternKeyParameterID")]
		NSString ParameterIdKey { get; }

		[Field ("CHHapticPatternKeyParameterValue")]
		NSString ParameterValueKey { get; }

		[Field ("CHHapticPatternKeyParameterCurve")]
		NSString WeakCHHapticParameterCurveKey { get; }

		[Field ("CHHapticPatternKeyParameterCurveControlPoints")]
		NSString WeakCHHapticParameterCurveControlPointsKey { get; }
	}

	[Mac (10,15), iOS (13,0)]
	[StrongDictionary ("CHHapticPatternDefinitionKeys")]
	partial interface CHHapticPatternDefinition {
		double Version { get; set; }
		NSArray Pattern { get; set; }
		NSDictionary Event { get; set; }
		CHHapticEventType EventType { get; set; }
		double Time { get; set; }
		double EventDuration { get; set; }
		NSString EventWaveformPath { get; set; }
		NSArray EventParameters { get; set; }
		NSDictionary Parameter { get; set; }
		// we need to do a NSString because it can be a CHHapticEventParameterID or a CHHapticDynamicParameterID 
		// which are two different enums. User will have to do the conversion
		[Advice ("Value can be either a CHHapticEventParameterId or a CHHapticDynamicParameterId. A check is needed against both enums.")]
		NSString ParameterId { get; set; }
		double ParameterValue { get; set; }
		// we do not have docs or header information about the exact type
		NSObject WeakCHHapticParameterCurve { get; set; }
		NSObject WeakCHHapticParameterCurveControlPoints { get; set; }
	}

	[Mac (10,15), iOS (13,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface CHHapticPattern {
		[Export ("duration")]
		double Duration { get; }

		[Export ("initWithEvents:parameters:error:")]
		IntPtr Constructor (CHHapticEvent[] events, CHHapticDynamicParameter[] parameters, [NullAllowed] out NSError outError);

		[Export ("initWithEvents:parameterCurves:error:")]
		IntPtr Constructor (CHHapticEvent[] events, CHHapticParameterCurve[] parameterCurves, [NullAllowed] out NSError outError);

		[Export ("initWithDictionary:error:")]
		IntPtr Constructor (NSDictionary patternDict, [NullAllowed] out NSError outError);

		[Wrap ("this (patternDefinition?.Dictionary, out outError)")]
		IntPtr Constructor (CHHapticPatternDefinition patternDefinition, [NullAllowed] out NSError outError);

		[Export ("exportDictionaryAndReturnError:")]
		[return: NullAllowed]
		NSDictionary<NSString, NSObject> _ExportDictionary ([NullAllowed] out NSError outError);

		[Wrap ("new CHHapticPatternDefinition (_ExportDictionary (out outError))")]
		CHHapticPatternDefinition ExportDictionary ([NullAllowed] out NSError outError);
	}
}