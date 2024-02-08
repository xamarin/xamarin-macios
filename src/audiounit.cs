///
// Authors:
//  Miguel de Icaza (miguel@xamarin.com)
//
// Copyright 2015 Xamarin, Inc.
//
//

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

using AudioUnit;
using CoreFoundation;
using Foundation;
using ObjCRuntime;
using CoreAnimation;
using CoreGraphics;
#if IOS || MONOMAC
using CoreAudioKit;
using CoreMidi;
#endif
using AudioToolbox;
using AVFoundation;

#if MONOMAC
using AppKit;
using AUViewControllerBase = AppKit.NSViewController;
#else
using UIKit;
using MediaToolbox;
using AUViewControllerBase = UIKit.UIViewController;
#endif
#if WATCH || TVOS
using MidiCIProfile = Foundation.NSObject;
using MidiCIProfileState = Foundation.NSObject;
#endif

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace AudioUnit {
	delegate AudioUnitStatus AUInternalRenderBlock (ref AudioUnitRenderActionFlags actionFlags, ref AudioTimeStamp timestamp, uint frameCount, nint outputBusNumber, AudioBuffers outputData, AURenderEventEnumerator realtimeEventListHead, [BlockCallback][NullAllowed] AURenderPullInputBlock pullInputBlock);
	delegate AudioUnitStatus AURenderBlock (ref AudioUnitRenderActionFlags actionFlags, ref AudioTimeStamp timestamp, uint frameCount, nint outputBusNumber, AudioBuffers outputData, [BlockCallback][NullAllowed] AURenderPullInputBlock pullInputBlock);

	internal delegate AudioUnitStatus AURenderPullInputBlock (ref AudioUnitRenderActionFlags actionFlags, ref AudioTimeStamp timestamp,
			uint frameCount, nint inputBusNumber, AudioBuffers inputData);

	delegate void AUScheduleParameterBlock (AUEventSampleTime eventSampleTime, uint rampDurationSampleFrames, ulong parameterAddress, float value);
	[NoWatch]
	[MacCatalyst (13, 1)]
	delegate int AUMidiOutputEventBlock (long eventSampleTime, byte cable, nint length, IntPtr midiBytes);
	delegate void AUImplementorValueObserver (AUParameter param, float value);
	delegate float AUImplementorValueProvider (AUParameter param);

	delegate void AUParameterObserver (ulong address, float value);

	delegate void AUVoiceIOMutedSpeechActivityEventListener (AUVoiceIOSpeechActivityEvent activityEvent);

	// 	AUAudioTODO - We need testing for these bindings
	// 	delegate void AUScheduleMidiEventBlock (AUEventSampleTime eventSampleTime, byte cable, nint length, ref byte midiBytes);
	// 	delegate bool AUHostMusicalContextBlock (ref double currentTempo, ref double timeSignatureNumerator, ref nint timeSignatureDenominator, ref double currentBeatPosition, ref nint sampleOffsetToNextBeat, ref double currentMeasureDownbeatPosition);
#if !NET
	[Advice ("The signature will change in the future to return a string")]
	delegate NSString AUImplementorStringFromValueCallback (AUParameter param, ref float? value);
#else
	delegate string AUImplementorStringFromValueCallback (AUParameter param, ref float? value);
#endif

	delegate string AUImplementorDisplayNameWithLengthCallback (AUParameterNode node, nint desiredLength);
	delegate void AUParameterRecordingObserver (nint numberOfEvents, ref AURecordedParameterEvent events);
	delegate void AUInputHandler (ref AudioUnitRenderActionFlags actionFlags, ref AudioTimeStamp timestamp, uint frameCount, nint inputBusNumber);
	delegate bool AUHostTransportStateBlock (ref AUHostTransportStateFlags transportStateFlags, ref double currentSamplePosition, ref double cycleStartBeatPosition, ref double cycleEndBeatPosition);
	delegate void AURenderObserver (AudioUnitRenderActionFlags actionFlags, ref AudioTimeStamp timestamp, uint frameCount, nint outputBusNumber);
	delegate float AUImplementorValueFromStringCallback (AUParameter param, string str);
	[NoTV]
	[NoWatch]
	[MacCatalyst (13, 1)]
	delegate void AUMidiCIProfileChangedCallback (byte cable, byte channel, MidiCIProfile profile, bool enabled);

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AUAudioUnit {
		[Static]
		[Export ("registerSubclass:asComponentDescription:name:version:")] // AUAudioUnitImplementation
		void RegisterSubclass (Class cls, AudioComponentDescription componentDescription, string name, uint version);

		[Export ("initWithComponentDescription:options:error:")]
		[DesignatedInitializer]
		NativeHandle Constructor (AudioComponentDescription componentDescription, AudioComponentInstantiationOptions options, [NullAllowed] out NSError outError);

		[Export ("initWithComponentDescription:error:")]
		NativeHandle Constructor (AudioComponentDescription componentDescription, [NullAllowed] out NSError outError);

		[Static]
		[Export ("instantiateWithComponentDescription:options:completionHandler:")]
		[Async]
		void FromComponentDescription (AudioComponentDescription componentDescription, AudioComponentInstantiationOptions options, Action<AUAudioUnit, NSError> completionHandler);

		[Export ("componentDescription")]
		AudioComponentDescription ComponentDescription { get; }

		[Export ("renderBlock")]
		AURenderBlock RenderBlock { get; }

		[Export ("internalRenderBlock")]
		AUInternalRenderBlock InternalRenderBlock { get; }

		// @property (readonly, nonatomic) AudioComponent __nonnull component;
		[Export ("component")]
		unsafe AudioComponent Component { get; }

		[NullAllowed, Export ("componentName")]
		string ComponentName { get; }

		[NullAllowed, Export ("audioUnitName")]
		string AudioUnitName { get; }

		[NullAllowed, Export ("manufacturerName")]
		string ManufacturerName { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("audioUnitShortName")]
		string ShortName { get; }

		[Export ("componentVersion")]
		uint ComponentVersion { get; }

		[Export ("allocateRenderResourcesAndReturnError:")]
		bool AllocateRenderResources ([NullAllowed] out NSError outError);

		[Export ("deallocateRenderResources")]
		void DeallocateRenderResources ();

		[Export ("renderResourcesAllocated")]
		bool RenderResourcesAllocated { get; }

		[Export ("reset")]
		void Reset ();

		[Export ("inputBusses")]
		AUAudioUnitBusArray InputBusses { get; }

		[Export ("outputBusses")]
		AUAudioUnitBusArray OutputBusses { get; }

		[Export ("scheduleParameterBlock")]
		AUScheduleParameterBlock ScheduleParameterBlock { get; }

		// TODO: https://github.com/xamarin/xamarin-macios/issues/12489
		// [TV (15,0), NoWatch, Mac (12,0), iOS (15,0), MacCatalyst (15,0)]
		// [NullAllowed]
		// [Export ("scheduleMIDIEventListBlock")]
		// AUMidiEventListBlock ScheduleMidiEventListBlock { get; }

		// 		[Export ("tokenByAddingRenderObserver:")]
		// 		nint GetToken (AURenderObserver observer);

		// 		[NullAllowed, Export ("scheduleMIDIEventBlock")]
		// 		AUScheduleMidiEventBlock ScheduleMidiEventBlock { get; }

		// 		[NullAllowed, Export ("musicalContextBlock", ArgumentSemantic.Copy)]
		// 		AUHostMusicalContextBlock MusicalContextBlock { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("MIDIOutputNames", ArgumentSemantic.Copy)]
		string [] MidiOutputNames { get; }

		// TODO: https://github.com/xamarin/xamarin-macios/issues/12489
		// [TV (15,0), NoWatch, Mac (12,0), iOS (15,0), MacCatalyst (15,0)]
		// [NullAllowed]
		// [Export ("MIDIOutputEventListBlock", ArgumentSemantic.Copy)]
		// AUMidiEventListBlock MidiOutputEventListBlock { get; set; }

		// TODO: https://github.com/xamarin/xamarin-macios/issues/12489
		// [TV (15,0), NoWatch, Mac (12,0), iOS (15,0), MacCatalyst (15,0)]
		// [Export ("AudioUnitMIDIProtocol")]
		// MIDIProtocolID AudioUnitMidiProtocol { get; }

		// TODO: https://github.com/xamarin/xamarin-macios/issues/12489
		// [TV (15,0), NoWatch, Mac (12,0), iOS (15,0), MacCatalyst (15,0)]
		// [Export ("hostMIDIProtocol", ArgumentSemantic.Assign)]
		// MIDIProtocolID HostMIDIProtocol { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("providesUserInterface")]
		bool ProvidesUserInterface { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("MIDIOutputEventBlock", ArgumentSemantic.Copy)]
		AUMidiOutputEventBlock MidiOutputEventBlock { get; set; }

		[NullAllowed, Export ("transportStateBlock", ArgumentSemantic.Copy)]
		AUHostTransportStateBlock TransportStateBlock { get; set; }

		[Export ("removeRenderObserver:")]
		void RemoveRenderObserver (nint token);

		[Export ("maximumFramesToRender")]
		uint MaximumFramesToRender { get; set; }

		[NullAllowed, Export ("parameterTree")]

		AUParameterTree ParameterTree {
			get;
			[Watch (6, 0), TV (13, 0), iOS (13, 0)]
			[MacCatalyst (13, 1)]
			set;
		}

		[Export ("parametersForOverviewWithCount:")]
		NSNumber [] GetParametersForOverview (nint count);

		[Export ("allParameterValues")]
		bool AllParameterValues { get; }

		[Export ("musicDeviceOrEffect")]
		bool MusicDeviceOrEffect { [Bind ("isMusicDeviceOrEffect")] get; }

		[Export ("virtualMIDICableCount")]
		nint VirtualMidiCableCount { get; }

		// @property (copy, nonatomic) NSDictionary<NSString * __nonnull,id __nonnull> * __nullable fullState;
		[NullAllowed, Export ("fullState", ArgumentSemantic.Copy)]
		NSDictionary FullState { get; set; }

		// @property (copy, nonatomic) NSDictionary<NSString * __nonnull,id __nonnull> * __nullable fullStateForDocument;
		[NullAllowed, Export ("fullStateForDocument", ArgumentSemantic.Copy)]
		NSDictionary FullStateForDocument { get; set; }

		[NullAllowed, Export ("factoryPresets", ArgumentSemantic.Copy)]
		AUAudioUnitPreset [] FactoryPresets { get; }

		[NullAllowed, Export ("currentPreset", ArgumentSemantic.Retain)]
		AUAudioUnitPreset CurrentPreset { get; set; }

		[Export ("latency")]
		double Latency { get; }

		[Export ("tailTime")]
		double TailTime { get; }

		[Export ("renderQuality", ArgumentSemantic.Assign)]
		nint RenderQuality { get; set; }

		[Export ("shouldBypassEffect")]
		bool ShouldBypassEffect { get; set; }

		[Export ("canProcessInPlace")]
		bool CanProcessInPlace { get; }

		[Export ("renderingOffline")]
		bool RenderingOffline { [Bind ("isRenderingOffline")] get; set; }

		[NullAllowed, Export ("channelCapabilities", ArgumentSemantic.Copy)]
		NSNumber [] ChannelCapabilities { get; }

		[NullAllowed, Export ("contextName")]
		string ContextName { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("supportsMPE")]
		bool SupportsMpe { get; }

		[MacCatalyst (13, 1)]
		[Export ("channelMap"), NullAllowed]
		NSNumber [] ChannelMap { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("requestViewControllerWithCompletionHandler:")]
		[Async]
		void RequestViewController (Action<AUViewControllerBase> completionHandler);

		// AUAudioUnitImplementation
		[Export ("setRenderResourcesAllocated:")]
		void SetRenderResourcesAllocated (bool flag);

		[Export ("shouldChangeToFormat:forBus:")]
		bool ShouldChangeToFormat (AVAudioFormat format, AUAudioUnitBus bus);

		[Notification, Field ("kAudioComponentRegistrationsChangedNotification")]
		NSString AudioComponentRegistrationsChangedNotification { get; }

		[Notification, Field ("kAudioComponentInstanceInvalidationNotification")]
		NSString AudioComponentInstanceInvalidationNotification { get; }

		[MacCatalyst (13, 1)]
		[Export ("MIDIOutputBufferSizeHint")]
		nint MidiOutputBufferSizeHint { get; set; }

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("profileStateForCable:channel:")]
		MidiCIProfileState GetProfileState (byte cable, byte channel);

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("profileChangedBlock", ArgumentSemantic.Assign)]
		AUMidiCIProfileChangedCallback ProfileChangedCallback { get; set; }

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("disableProfile:cable:onChannel:error:")]
		bool Disable (MidiCIProfile profile, byte cable, byte channel, [NullAllowed] out NSError outError);

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("enableProfile:cable:onChannel:error:")]
		bool Enable (MidiCIProfile profile, byte cable, byte channel, [NullAllowed] out NSError outError);

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("userPresets", ArgumentSemantic.Copy)]
		AUAudioUnitPreset [] UserPresets { get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("saveUserPreset:error:")]
		bool SaveUserPreset (AUAudioUnitPreset userPreset, [NullAllowed] out NSError outError);

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("deleteUserPreset:error:")]
		bool DeleteUserPreset (AUAudioUnitPreset userPreset, [NullAllowed] out NSError outError);

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("presetStateFor:error:")]
		[return: NullAllowed]
		NSDictionary<NSString, NSObject> GetPresetState (AUAudioUnitPreset userPreset, [NullAllowed] out NSError outError);

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("supportsUserPresets")]
		bool SupportsUserPresets { get; }

		[Introduced (PlatformName.MacCatalyst, 13, 0)] // needed since it's not in iOS
		[NoWatch, NoTV, NoiOS]
		[Export ("isLoadedInProcess")]
		bool IsLoadedInProcess { get; }
	}

	// kept separate from AUAudioUnit, quote:
	// These methods will fail if the audio unit is not an input/output audio unit.
	[MacCatalyst (13, 1)]
	[Category]
	[BaseType (typeof (AUAudioUnit))]
	interface AUAudioUnit_AUAudioInputOutputUnit {

		[NoTV, NoiOS, NoWatch]
		[NoMacCatalyst]
		[Export ("deviceID")]
		uint GetDeviceId ();

		[NoTV, NoiOS, NoWatch]
		[NoMacCatalyst]
		[Export ("setDeviceID:error:")]
		bool SetDeviceId (uint deviceID, out NSError outError);

		[Export ("canPerformInput")]
		bool GetCanPerformInput ();

		[Export ("canPerformOutput")]
		bool CanPerformOutput ();

		[Export ("isInputEnabled")]
		bool IsInputEnabled ();

		[Export ("setInputEnabled:")]
		bool SetInputEnabled (bool enabled);

		[Export ("isOutputEnabled")]
		bool IsOutputEnabled ();

		[Export ("setOutputEnabled:")]
		bool SetOutputEnabled (bool enabled);

		[return: NullAllowed]
		[Export ("inputHandler", ArgumentSemantic.Copy)]
		AUInputHandler GetInputHandler ();

		[Export ("setInputHandler:")]
		void SetInputHandler ([NullAllowed] AUInputHandler handler);

		[Export ("startHardwareAndReturnError:")]
		bool StartHardware ([NullAllowed] out NSError outError);

		[Export ("stopHardware")]
		void StopHardware ();

		[return: NullAllowed]
		[Export ("outputProvider", ArgumentSemantic.Copy)]
		AURenderPullInputBlock GetOutputProvider ();

		[Export ("setOutputProvider:")]
		void SetOutputProvider ([NullAllowed] AURenderPullInputBlock provider);

		// the following are properties but we cannot have properties in Categories.
		[NoWatch, NoiOS, NoTV]
		[NoMacCatalyst]
		[Export ("deviceInputLatency")]
		double GetDeviceInputLatency ();

		[NoWatch, NoiOS, NoTV]
		[NoMacCatalyst]
		[Export ("deviceOutputLatency")]
		double GetDeviceOutputLatency ();

		[MacCatalyst (13, 1)]
		[Export ("running")]
		bool IsRunning ();
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AUAudioUnitBus {
		[Export ("initWithFormat:error:")]
		NativeHandle Constructor (AVAudioFormat format, [NullAllowed] out NSError outError);

		[Export ("format")]
		AVAudioFormat Format { get; }

		[Export ("setFormat:error:")]
		bool SetFormat (AVAudioFormat format, [NullAllowed] out NSError outError);

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[NullAllowed, Export ("name")]
		string Name { get; set; }

		[Export ("index")]
		nuint Index { get; }

		[Export ("busType")]
		AUAudioUnitBusType BusType { get; }

		[Export ("ownerAudioUnit", ArgumentSemantic.Assign)]
		AUAudioUnit OwnerAudioUnit { get; }

		[NullAllowed, Export ("supportedChannelLayoutTags", ArgumentSemantic.Copy)]
		NSNumber [] SupportedChannelLayoutTags { get; }

		[Export ("contextPresentationLatency")]
		double ContextPresentationLatency { get; set; }

		// AUAudioUnitImplementation
		[NullAllowed, Export ("supportedChannelCounts", ArgumentSemantic.Retain)]
		NSNumber [] SupportedChannelCounts { get; set; }

		[Export ("maximumChannelCount")]
		uint MaximumChannelCount { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("shouldAllocateBuffer")]
		bool ShouldAllocateBuffer { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AUAudioUnitBusArray : INSFastEnumeration {
		[Export ("initWithAudioUnit:busType:busses:")]
		[DesignatedInitializer]
		NativeHandle Constructor (AUAudioUnit owner, AUAudioUnitBusType busType, AUAudioUnitBus [] busArray);

		[Export ("initWithAudioUnit:busType:")]
		NativeHandle Constructor (AUAudioUnit owner, AUAudioUnitBusType busType);

		[Export ("count")]
		nuint Count { get; }

		// -(AUAudioUnitBus * __nonnull)objectAtIndexedSubscript:(NSUInteger)index;
		[Export ("objectAtIndexedSubscript:")]
		AUAudioUnitBus GetObject (nuint index);

		[Export ("countChangeable")]
		bool CountChangeable { [Bind ("isCountChangeable")] get; }

		[Export ("setBusCount:error:")]
		bool SetBusCount (nuint count, [NullAllowed] out NSError outError);

		// -(void)addObserverToAllBusses:(NSObject * __nonnull)observer forKeyPath:(NSString * __nonnull)keyPath options:(NSKeyValueObservingOptions)options context:(void * __nullable)context;
		[Export ("addObserverToAllBusses:forKeyPath:options:context:")]
		void AddObserver (NSObject observer, string keyPath, NSKeyValueObservingOptions options, /* void * */ IntPtr context);

		// -(void)removeObserverFromAllBusses:(NSObject * __nonnull)observer forKeyPath:(NSString * __nonnull)keyPath context:(void * __nullable)context;
		[Export ("removeObserverFromAllBusses:forKeyPath:context:")]
		void RemoveObserver (NSObject observer, string keyPath, /* void * */ IntPtr context);

		[Export ("ownerAudioUnit", ArgumentSemantic.Assign)]
		AUAudioUnit OwnerAudioUnit { get; }

		[Export ("busType")]
		AUAudioUnitBusType BusType { get; }

		//AUAudioUnitBusImplementation
		[Export ("replaceBusses:")]
		void ReplaceBusses (AUAudioUnitBus [] busArray);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AUAudioUnitPreset : NSSecureCoding {
		[Export ("number", ArgumentSemantic.Assign)]
		nint Number { get; set; }

		[Export ("name")]
		string Name { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (AUParameterNode))]
	interface AUParameter : NSSecureCoding {
		[Export ("minValue")]
		float MinValue { get; }

		[Export ("maxValue")]
		float MaxValue { get; }

		[Export ("unit")]
		AudioUnitParameterUnit Unit { get; }

		[NullAllowed, Export ("unitName")]
		string UnitName { get; }

		[Export ("flags")]
		AudioUnitParameterOptions Flags { get; }

		[Export ("address")]
		ulong Address { get; }

		[NullAllowed, Export ("valueStrings", ArgumentSemantic.Copy)]
		string [] ValueStrings { get; }

		[NullAllowed, Export ("dependentParameters", ArgumentSemantic.Copy)]
		NSNumber [] DependentParameters { get; }

		[Export ("value")]
		float Value { get; set; }

		[Export ("setValue:originator:")]
		void SetValue (float value, IntPtr originator);

		[Wrap ("SetValue (value, originator.ObserverToken)")]
		void SetValue (float value, AUParameterObserverToken originator);

		[Export ("setValue:originator:atHostTime:")]
		void SetValue (float value, IntPtr originator, ulong hostTime);

		[Wrap ("SetValue (value, originator.ObserverToken, hostTime)")]
		void SetValue (float value, AUParameterObserverToken originator, ulong hostTime);

		// -(NSString * __nonnull)stringFromValue:(const AUValue * __nullable)value;
		[Export ("stringFromValue:")]
		string GetString (ref float value);

		[Internal]
		[Sealed]
		[Export ("stringFromValue:")]
		string _GetString (IntPtr value);

		[Export ("valueFromString:")]
		float GetValue (string str);

		[MacCatalyst (13, 1)]
		[Internal]
		[Export ("setValue:originator:atHostTime:eventType:")]
		void SetValue (float value, IntPtr originator, ulong hostTime, AUParameterAutomationEventType eventType);

		[MacCatalyst (13, 1)]
		[Wrap ("SetValue (value, originator.ObserverToken, hostTime, eventType)")]
		void SetValue (float value, AUParameterObserverToken originator, ulong hostTime, AUParameterAutomationEventType eventType);
	}

	[MacCatalyst (13, 1)]
	delegate void AUParameterAutomationObserver (ulong address, float value);

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AUParameterNode {
		[Export ("identifier")]
		string Identifier { get; }

		[Export ("keyPath")]
		string KeyPath { get; }

		[Export ("displayName")]
		string DisplayName { get; }

		[Export ("displayNameWithLength:")]
		string GetDisplayName (nint maximumLength);

		[Export ("tokenByAddingParameterObserver:")]
		/* void * */
		IntPtr TokenByAddingParameterObserver (AUParameterObserver observer);

		[Wrap ("new AUParameterObserverToken { ObserverToken = TokenByAddingParameterObserver (observer) }")]
		AUParameterObserverToken CreateTokenByAddingParameterObserver (AUParameterObserver observer);

		[Export ("tokenByAddingParameterRecordingObserver:")]
		/* void * */
		IntPtr TokenByAddingParameterRecordingObserver (AUParameterRecordingObserver observer);

		[Wrap ("new AUParameterObserverToken { ObserverToken = TokenByAddingParameterRecordingObserver (observer) }")]
		AUParameterObserverToken CreateTokenByAddingParameterRecordingObserver (AUParameterRecordingObserver observer);

		[Export ("implementorValueObserver", ArgumentSemantic.Copy)]
		AUImplementorValueObserver ImplementorValueObserver { get; set; }

		[Export ("implementorValueProvider", ArgumentSemantic.Copy)]
		AUImplementorValueProvider ImplementorValueProvider { get; set; }

		[Export ("implementorValueFromStringCallback", ArgumentSemantic.Copy)]
		AUImplementorValueFromStringCallback ImplementorValueFromStringCallback { get; set; }

		[Export ("removeParameterObserver:")]
		void RemoveParameterObserver (/* void * */ IntPtr token);

		[Wrap ("RemoveParameterObserver (token.ObserverToken)")]
		void RemoveParameterObserver (AUParameterObserverToken token);

		[Export ("implementorStringFromValueCallback", ArgumentSemantic.Copy),]
		AUImplementorStringFromValueCallback ImplementorStringFromValueCallback { get; set; }

		[Export ("implementorDisplayNameWithLengthCallback", ArgumentSemantic.Copy)]
		AUImplementorDisplayNameWithLengthCallback ImplementorDisplayNameWithLengthCallback { get; set; }

		[MacCatalyst (13, 1)]
		[Internal]
		[Export ("tokenByAddingParameterAutomationObserver:")]
		IntPtr _GetToken (AUParameterAutomationObserver observer);

		[MacCatalyst (13, 1)]
		[Wrap ("new AUParameterObserverToken (_GetToken (observer))")]
		AUParameterObserverToken GetToken (AUParameterAutomationObserver observer);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (AUParameterNode))]
	interface AUParameterGroup : NSSecureCoding {
		[Export ("children")]
		AUParameterNode [] Children { get; }

		[Export ("allParameters")]
		AUParameter [] AllParameters { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (AUParameterGroup))]
	interface AUParameterTree : NSSecureCoding {
		[Export ("parameterWithAddress:")]
		[return: NullAllowed]
		AUParameter GetParameter (ulong address);

		[Export ("parameterWithID:scope:element:")]
		[return: NullAllowed]
		AUParameter GetParameter (uint paramID, uint scope, uint element);

		//Factory
		[Static]
		[Export ("createParameterWithIdentifier:name:address:min:max:unit:unitName:flags:valueStrings:dependentParameters:")]
		AUParameter CreateParameter (string identifier, string name, ulong address, float min, float max, AudioUnitParameterUnit unit, [NullAllowed] string unitName, AudioUnitParameterOptions flags, [NullAllowed] string [] valueStrings, [NullAllowed] NSNumber [] dependentParameters);

		[Static]
		[Export ("createGroupWithIdentifier:name:children:")]
		AUParameterGroup CreateGroup (string identifier, string name, AUParameterNode [] children);

		[Static]
		[Export ("createGroupTemplate:")]
		AUParameterGroup CreateGroupTemplate (AUParameterNode [] children);

		[Static]
		[Export ("createGroupFromTemplate:identifier:name:addressOffset:")]
		AUParameterGroup CreateGroup (AUParameterGroup templateGroup, string identifier, string name, ulong addressOffset);

		[Static]
		[Export ("createTreeWithChildren:")]
		AUParameterTree CreateTree (AUParameterNode [] children);
	}

	[Protocol]
	interface AUAudioUnitFactory : NSExtensionRequestHandling {
		[Abstract]
		[Export ("createAudioUnitWithComponentDescription:error:")]
		[return: NullAllowed]
		AUAudioUnit CreateAudioUnit (AudioComponentDescription desc, [NullAllowed] out NSError error);
	}
}
