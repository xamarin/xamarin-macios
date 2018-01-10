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

namespace AudioUnit {
#if XAMCORE_2_0 || !MONOMAC
	delegate AudioUnitStatus AUInternalRenderBlock (ref AudioUnitRenderActionFlags actionFlags, ref AudioTimeStamp timestamp, uint frameCount, nint outputBusNumber, AudioBuffers outputData, AURenderEventEnumerator realtimeEventListHead, [BlockCallback][NullAllowed]AURenderPullInputBlock pullInputBlock);
	delegate AudioUnitStatus AURenderBlock (ref AudioUnitRenderActionFlags actionFlags, ref AudioTimeStamp timestamp, uint frameCount, nint outputBusNumber, AudioBuffers outputData, [BlockCallback][NullAllowed] AURenderPullInputBlock pullInputBlock);

	internal delegate AudioUnitStatus AURenderPullInputBlock (ref AudioUnitRenderActionFlags actionFlags, ref AudioTimeStamp timestamp,
			uint frameCount, nint inputBusNumber, AudioBuffers inputData);

	delegate void AUScheduleParameterBlock (AUEventSampleTime eventSampleTime, uint rampDurationSampleFrames, ulong parameterAddress, float value);
	[iOS (11, 0), Mac (10,13), TV (11,0), NoWatch]
	delegate int AUMidiOutputEventBlock (long eventSampleTime, byte cable, nint length, IntPtr midiBytes);
	delegate void AUImplementorValueObserver (AUParameter param, float value);
	delegate float AUImplementorValueProvider (AUParameter param);

	delegate void AUParameterObserver (ulong address, float value);

// 	AUAudioTODO - We need testing for these bindings
// 	delegate void AUScheduleMidiEventBlock (AUEventSampleTime eventSampleTime, byte cable, nint length, ref byte midiBytes);
// 	delegate bool AUHostMusicalContextBlock (ref double currentTempo, ref double timeSignatureNumerator, ref nint timeSignatureDenominator, ref double currentBeatPosition, ref nint sampleOffsetToNextBeat, ref double currentMeasureDownbeatPosition);
#if !XAMCORE_4_0
	[Advice ("The signature will change in the future to return a string")]
	delegate NSString
#else
	delegate string
#endif
	AUImplementorStringFromValueCallback (AUParameter param, ref float? value);

	delegate string AUImplementorDisplayNameWithLengthCallback (AUParameterNode node, nint desiredLength);
	delegate void AUParameterRecordingObserver (nint numberOfEvents, ref AURecordedParameterEvent events);
	delegate void AUInputHandler (ref AudioUnitRenderActionFlags actionFlags, ref AudioTimeStamp timestamp, uint frameCount, nint inputBusNumber);
	delegate bool AUHostTransportStateBlock (ref AUHostTransportStateFlags transportStateFlags, ref double currentSamplePosition, ref double cycleStartBeatPosition, ref double cycleEndBeatPosition);
	delegate void AURenderObserver (AudioUnitRenderActionFlags actionFlags, ref AudioTimeStamp timestamp, uint frameCount, nint outputBusNumber);
	delegate float AUImplementorValueFromStringCallback (AUParameter param, string str);

	[iOS (9,0), Mac(10,11, onlyOn64 : true)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface AUAudioUnit
	{
#if XAMCORE_2_0 // AudioComponentDescription went under large changes between Classic and Unified. Since this is a new API, no reason to pollute it with the Classic hacks
		[Static]
		[Export ("registerSubclass:asComponentDescription:name:version:")] // AUAudioUnitImplementation
		void RegisterSubclass (Class cls, AudioComponentDescription componentDescription, string name, uint version);

		[Export ("initWithComponentDescription:options:error:")]
		[DesignatedInitializer]
		IntPtr Constructor (AudioComponentDescription componentDescription, AudioComponentInstantiationOptions options, [NullAllowed] out NSError outError);

		[Export ("initWithComponentDescription:error:")]
		IntPtr Constructor (AudioComponentDescription componentDescription, [NullAllowed] out NSError outError);

		[Static]
		[Export ("instantiateWithComponentDescription:options:completionHandler:")]
		[Async]
		void FromComponentDescription (AudioComponentDescription componentDescription, AudioComponentInstantiationOptions options, Action<AUAudioUnit, NSError> completionHandler);

		[Export ("componentDescription")]
		AudioComponentDescription ComponentDescription { get; }
#endif

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

		[iOS (11, 0), Mac (10, 13), TV (11, 0), NoWatch]
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

// 		[Export ("tokenByAddingRenderObserver:")]
// 		nint GetToken (AURenderObserver observer);

// 		[NullAllowed, Export ("scheduleMIDIEventBlock")]
// 		AUScheduleMidiEventBlock ScheduleMidiEventBlock { get; }

// 		[NullAllowed, Export ("musicalContextBlock", ArgumentSemantic.Copy)]
// 		AUHostMusicalContextBlock MusicalContextBlock { get; set; }

		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[Export ("MIDIOutputNames", ArgumentSemantic.Copy)]
		string[] MidiOutputNames { get; }

		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[Export ("providesUserInterface")]
		bool ProvidesUserInterface { get; }

		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[NullAllowed, Export ("MIDIOutputEventBlock", ArgumentSemantic.Copy)]
		AUMidiOutputEventBlock MidiOutputEventBlock { get; set; }

		[NullAllowed, Export ("transportStateBlock", ArgumentSemantic.Copy)]
		AUHostTransportStateBlock TransportStateBlock { get; set; }

		[Export ("removeRenderObserver:")]
		void RemoveRenderObserver (nint token);

		[Export ("maximumFramesToRender")]
		uint MaximumFramesToRender { get; set; }

		[NullAllowed, Export ("parameterTree")]
		AUParameterTree ParameterTree { get; [NotImplemented] set;}

		[Export ("parametersForOverviewWithCount:")]
		NSNumber[] GetParametersForOverview (nint count);

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
		AUAudioUnitPreset[] FactoryPresets { get; }

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
		NSNumber[] ChannelCapabilities { get; }

		[NullAllowed, Export ("contextName")]
		string ContextName { get; set; }

		[iOS (10,0), Mac (10,12, onlyOn64 : true), TV (10,0), Watch (4, 0)]
		[Export ("supportsMPE")]
		bool SupportsMpe { get; }

		[iOS (10,0), Mac (10,12, onlyOn64 : true), TV (10,0)]
		[Export ("channelMap"), NullAllowed]
		NSNumber [] ChannelMap { get; set; }

		[NoTV]
		[Export ("requestViewControllerWithCompletionHandler:")]
		[Async]
		void RequestViewController (Action<AUViewControllerBase> completionHandler);
		
		// AUAudioUnitImplementation
		[Export ("setRenderResourcesAllocated:")]
		void SetRenderResourcesAllocated (bool flag);

		[Export ("shouldChangeToFormat:forBus:")]
		bool ShouldChangeToFormat (AVAudioFormat format, AUAudioUnitBus bus);

		[Mac (10,11)][iOS (7,0)]
		[Notification, Field ("kAudioComponentRegistrationsChangedNotification")]
		NSString AudioComponentRegistrationsChangedNotification { get; }

		[Mac (10,11)][iOS (7,0)]
		[Notification, Field ("kAudioComponentInstanceInvalidationNotification")]
		NSString AudioComponentInstanceInvalidationNotification { get; }

		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[Export ("MIDIOutputBufferSizeHint")]
		nint MidiOutputBufferSizeHint { get; set; }

	}

	// kept separate from AUAudioUnit, quote:
	// These methods will fail if the audio unit is not an input/output audio unit.
	[iOS (9,0), Mac(10,11, onlyOn64: true)]
	[Category]
	[BaseType (typeof (AUAudioUnit))]
	interface AUAudioUnit_AUAudioInputOutputUnit {

		[Mac (10,12), NoTV, NoiOS, NoWatch]
		[Export ("deviceID")]
		uint GetDeviceId ();

		[Mac (10,12), NoTV, NoiOS, NoWatch]
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
		
		[NullAllowed, Export ("inputHandler", ArgumentSemantic.Copy)]
		AUInputHandler GetInputHandler ();

		[NullAllowed, Export ("setInputHandler:")]
		void SetInputHandler (AUInputHandler handler);

		[Export ("startHardwareAndReturnError:")]
		bool StartHardware ([NullAllowed] out NSError outError);

		[Export ("stopHardware")]
		void StopHardware ();

		[NullAllowed, Export ("outputProvider", ArgumentSemantic.Copy)]
		AURenderPullInputBlock GetOutputProvider ();

		[NullAllowed, Export ("setOutputProvider:")]
		void SetOutputProvider (AURenderPullInputBlock provider);

		// the following are properties but we cannot have properties in Categories.
		[Mac (10, 13), NoWatch, NoiOS, NoTV]
		[Export ("deviceInputLatency")]
		double GetDeviceInputLatency ();

		[Mac (10, 13), NoWatch, NoiOS, NoTV]
		[Export ("deviceOutputLatency")]
		double GetDeviceOutputLatency ();

		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[Export ("running")]
		bool IsRunning ();
	}

	[iOS (9,0), Mac(10,11, onlyOn64 : true)]
	[BaseType (typeof(NSObject))]
	interface AUAudioUnitBus
	{
		[Export ("initWithFormat:error:")]
		IntPtr Constructor (AVAudioFormat format, [NullAllowed] out NSError outError);

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
		NSNumber[] SupportedChannelLayoutTags { get; }

		[Export ("contextPresentationLatency")]
		double ContextPresentationLatency { get; set; }

		// AUAudioUnitImplementation
		[NullAllowed, Export ("supportedChannelCounts", ArgumentSemantic.Retain)]
		NSNumber[] SupportedChannelCounts { get; set; }

		[Export ("maximumChannelCount")]
		uint MaximumChannelCount { get; set; }

		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[Export ("shouldAllocateBuffer")]
		bool ShouldAllocateBuffer { get; set; }
	}

	[iOS (9,0), Mac(10,11, onlyOn64 : true)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface AUAudioUnitBusArray : INSFastEnumeration
	{
		[Export ("initWithAudioUnit:busType:busses:")]
		[DesignatedInitializer]
		IntPtr Constructor (AUAudioUnit owner, AUAudioUnitBusType busType, AUAudioUnitBus[] busArray);

		[Export ("initWithAudioUnit:busType:")]
		IntPtr Constructor (AUAudioUnit owner, AUAudioUnitBusType busType);

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
		void ReplaceBusses (AUAudioUnitBus[] busArray);
	}

	[iOS (9,0), Mac(10,11, onlyOn64 : true)]
	[BaseType (typeof(NSObject))]
	interface AUAudioUnitPreset : NSSecureCoding
	{
		[Export ("number", ArgumentSemantic.Assign)]
		nint Number { get; set; }

		[Export ("name")]
		string Name { get; set; }
	}

	[iOS (9,0), Mac(10,11, onlyOn64 : true)]
	[BaseType (typeof(AUParameterNode))]
	interface AUParameter : NSSecureCoding
	{
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
		string[] ValueStrings { get; }

		[NullAllowed, Export ("dependentParameters", ArgumentSemantic.Copy)]
		NSNumber[] DependentParameters { get; }

		[Export ("value")]
		float Value { get; set; }

		// -(void)setValue:(AUValue)value originator:(AUParameterObserverToken __nullable)originator;
#if XAMCORE_4_0 // undo breaking change introduced in xamarin/maccore @ 1f207bd3f3df363cb5a74e59b93acd8eb6e1fec2
		[Internal][Sealed]
#else
		[Obsolete ("Use the 'AUParameterObserverToken' overload.")]
#endif
		[Export ("setValue:originator:")]
		void SetValue (float value, IntPtr originator);

		// -(void)setValue:(AUValue)value originator:(AUParameterObserverToken __nullable)originator atHostTime:(uint64_t)hostTime;
#if XAMCORE_4_0 // undo breaking change introduced in xamarin/maccore @ 1f207bd3f3df363cb5a74e59b93acd8eb6e1fec2
		[Internal][Sealed]
#else
		[Obsolete ("Use the 'AUParameterObserverToken' overload.")]
#endif
		[Export ("setValue:originator:atHostTime:")]
		void SetValue (float value, IntPtr originator, ulong hostTime);

		// -(NSString * __nonnull)stringFromValue:(const AUValue * __nullable)value;
		[Export ("stringFromValue:")]
		string GetString (ref float value);

		[Internal][Sealed][Export ("stringFromValue:")]
		string _GetString (IntPtr value);

		[Export ("valueFromString:")]
		float GetValue (string str);

		[iOS (10,0), Mac (10,12, onlyOn64 : true)]
		[TV (10,0)]
		[Internal]
		[Export ("setValue:originator:atHostTime:eventType:")]
		void SetValue (float value, IntPtr originator, ulong hostTime, AUParameterAutomationEventType eventType);

		[iOS (10,0), Mac (10,12, onlyOn64 : true), Watch (4, 0), TV (10, 0)]
		[Wrap ("SetValue (value, originator.ObserverToken, hostTime, eventType)")]
		void SetValue (float value, AUParameterObserverToken originator, ulong hostTime, AUParameterAutomationEventType eventType);
	}

	[iOS (10,0), Mac (10,12, onlyOn64 : true)]
	delegate void AUParameterAutomationObserver (ulong address, float value);

	[iOS (9,0), Mac(10,11, onlyOn64 : true)]
	[BaseType (typeof(NSObject))]
	interface AUParameterNode
	{
		[Export ("identifier")]
		string Identifier { get; }

		[Export ("keyPath")]
		string KeyPath { get; }

		[Export ("displayName")]
		string DisplayName { get; }

		[Export ("displayNameWithLength:")]
		string GetDisplayName (nint maximumLength);

		// -(AUParameterObserverToken __nonnull)tokenByAddingParameterObserver:(AUParameterObserver __nonnull)observer;
#if XAMCORE_4_0 // undo breaking change introduced in xamarin/maccore @ 1f207bd3f3df363cb5a74e59b93acd8eb6e1fec2
		[Internal][Sealed]
#else
		[Obsolete ("Use the 'CreateTokenByAddingParameterObserver' instead.")]
#endif
		[Export ("tokenByAddingParameterObserver:")]
		/* void * */ IntPtr TokenByAddingParameterObserver (AUParameterObserver observer);

 		// -(AUParameterObserverToken __nonnull)tokenByAddingParameterRecordingObserver:(AUParameterRecordingObserver __nonnull)observer;
#if XAMCORE_4_0 // undo breaking change introduced in xamarin/maccore @ 1f207bd3f3df363cb5a74e59b93acd8eb6e1fec2
		[Internal][Sealed]
#else
		[Obsolete ("Use the 'CreateTokenByAddingParameterRecordingObserver' instead.")]
#endif
		[Export ("tokenByAddingParameterRecordingObserver:")]
		/* void * */ IntPtr TokenByAddingParameterRecordingObserver (AUParameterRecordingObserver observer);

		[Export ("implementorValueObserver", ArgumentSemantic.Copy)]
		AUImplementorValueObserver ImplementorValueObserver { get; set; }

		[Export ("implementorValueProvider", ArgumentSemantic.Copy)]
		AUImplementorValueProvider ImplementorValueProvider { get; set; }

 		[Export ("implementorValueFromStringCallback", ArgumentSemantic.Copy)]
 		AUImplementorValueFromStringCallback ImplementorValueFromStringCallback { get; set; }

 		// -(void)removeParameterObserver:(AUParameterObserverToken __nonnull)token;
#if XAMCORE_4_0 // undo breaking change introduced in xamarin/maccore @ 1f207bd3f3df363cb5a74e59b93acd8eb6e1fec2
		[Internal][Sealed]
#else
		[Obsolete ("Use the 'AUParameterObserverToken' overload.")]
#endif
		[Export ("removeParameterObserver:")]
		void RemoveParameterObserver (/* void * */ IntPtr token);

		[Export ("implementorStringFromValueCallback", ArgumentSemantic.Copy),]
		AUImplementorStringFromValueCallback ImplementorStringFromValueCallback { get; set; }

		[Export ("implementorDisplayNameWithLengthCallback", ArgumentSemantic.Copy)]
		AUImplementorDisplayNameWithLengthCallback ImplementorDisplayNameWithLengthCallback { get; set; }

		[iOS (10,0), Mac (10,12, onlyOn64 : true)]
		[TV (10,0)]
		[Internal]
		[Export ("tokenByAddingParameterAutomationObserver:")]
		IntPtr _GetToken (AUParameterAutomationObserver observer);

		[iOS (10,0), Mac (10,12, onlyOn64 : true)]
		[TV (10,0)]
		[Wrap ("new AUParameterObserverToken (_GetToken (observer))")]
		AUParameterObserverToken GetToken (AUParameterAutomationObserver observer);
	}

	[iOS (9,0), Mac(10,11, onlyOn64 : true)]
	[BaseType (typeof(AUParameterNode))]
	interface AUParameterGroup : NSSecureCoding
	{
		[Export ("children")]
		AUParameterNode[] Children { get; }

		[Export ("allParameters")]
		AUParameter[] AllParameters { get; }
	}

	[iOS (9,0), Mac(10,11, onlyOn64 : true)]
	[BaseType (typeof(AUParameterGroup))]
	interface AUParameterTree : NSSecureCoding
	{
		[Export ("parameterWithAddress:")]
		[return: NullAllowed]
		AUParameter GetParameter (ulong address);

		[Export ("parameterWithID:scope:element:")]
		[return: NullAllowed]
		AUParameter GetParameter (uint paramID, uint scope, uint element);

		//Factory
		[Static]
		[Export ("createParameterWithIdentifier:name:address:min:max:unit:unitName:flags:valueStrings:dependentParameters:")]
		AUParameter CreateParameter (string identifier, string name, ulong address, float min, float max, AudioUnitParameterUnit unit, [NullAllowed] string unitName, AudioUnitParameterOptions flags, [NullAllowed] string[] valueStrings, [NullAllowed] NSNumber[] dependentParameters);

		[Static]
		[Export ("createGroupWithIdentifier:name:children:")]
		AUParameterGroup CreateGroup (string identifier, string name, AUParameterNode[] children);

		[Static]
		[Export ("createGroupTemplate:")]
		AUParameterGroup CreateGroupTemplate (AUParameterNode[] children);

		[Static]
		[Export ("createGroupFromTemplate:identifier:name:addressOffset:")]
		AUParameterGroup CreateGroup (AUParameterGroup templateGroup, string identifier, string name, ulong addressOffset);

		[Static]
		[Export ("createTreeWithChildren:")]
		AUParameterTree CreateTree (AUParameterNode[] children);
	}

#if XAMCORE_2_0
	[Protocol]
	interface AUAudioUnitFactory : NSExtensionRequestHandling
	{
		[Abstract]
		[Export ("createAudioUnitWithComponentDescription:error:")]
		[return: NullAllowed]
		AUAudioUnit CreateAudioUnit (AudioComponentDescription desc, [NullAllowed] out NSError error);
	}
#endif
#endif
}
