//
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
#if TVOS
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
	[MacCatalyst (13, 1)]
	delegate int AUMidiOutputEventBlock (long eventSampleTime, byte cable, nint length, IntPtr midiBytes);
	/// <param name="param">The parameter that changed.</param>
	///     <param name="value">The new parameter value.</param>
	///     <summary>A delegate that notifies the audio unit when a parameter value changes.</summary>
	delegate void AUImplementorValueObserver (AUParameter param, float value);
	/// <param name="param">The parameter whose value to get.</param>
	///     <summary>A delegate that retrieves a parameter value.</summary>
	///     <returns>The value of the parameter that is identified by <paramref name="param" />.</returns>
	delegate float AUImplementorValueProvider (AUParameter param);

	/// <param name="address">The parameter address.</param>
	///     <param name="value">The current parameter value.</param>
	///     <summary>Observer that notifies an audio unit when a parameter value changes.</summary>
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

	/// <param name="node">The parameter node for which to get a possibly shortened name.</param>
	///     <param name="desiredLength">The maximum desired length of the display name.</param>
	///     <summary>A delegate that returns the display name, possibly shortened to <paramref name="desiredLength" /> characters.</summary>
	///     <returns>The display name, possibly shortened to <paramref name="desiredLength" /> characters.</returns>
	delegate string AUImplementorDisplayNameWithLengthCallback (AUParameterNode node, nint desiredLength);
	/// <param name="numberOfEvents">The number of automation events.</param>
	///     <param name="events">The delivered events.</param>
	///     <summary>Delegate that records parameter changes as automation events.</summary>
	delegate void AUParameterRecordingObserver (nint numberOfEvents, ref AURecordedParameterEvent events);
	/// <param name="actionFlags">The action flags that configure the audio unit rendering process.</param>
	///     <param name="timestamp">The unconverted, uncompressed HAL time when the input will render.</param>
	///     <param name="frameCount">The number of available audio frames.</param>
	///     <param name="inputBusNumber">The input bus index.</param>
	///     <summary>Delegate that tells an I/O host when input is available.</summary>
	delegate void AUInputHandler (ref AudioUnitRenderActionFlags actionFlags, ref AudioTimeStamp timestamp, uint frameCount, nint inputBusNumber);
	/// <param name="transportStateFlags">The state of the audio transport.</param>
	///     <param name="currentSamplePosition">The host sample position, in audio unit samples.</param>
	///     <param name="cycleStartBeatPosition">The starting beat position for the cycle. <see langword="null" /> if not cycling.</param>
	///     <param name="cycleEndBeatPosition">The ending beat position for the cycle. <see langword="null" /> if not cycling.</param>
	///     <summary>A delegate block that a host uses to provide information about its transport state.</summary>
	///     <returns>
	///       <para>
	///         <see langword="true" /> if the state was successfully retrieved. Otherwise, <see langword="false" />.</para>
	///     </returns>
	///     <remarks>
	///       <para>Developers may optionally assign an instance of this class to the <see cref="P:AudioUnit.AUAudioUnit.TransportStateBlock" /> property so that they can call it at the beginning of render cycles to get the transport state at the cycle start.</para>
	///     </remarks>
	delegate bool AUHostTransportStateBlock (ref AUHostTransportStateFlags transportStateFlags, ref double currentSamplePosition, ref double cycleStartBeatPosition, ref double cycleEndBeatPosition);
	delegate void AURenderObserver (AudioUnitRenderActionFlags actionFlags, ref AudioTimeStamp timestamp, uint frameCount, nint outputBusNumber);
	/// <param name="param">The parameter that will be assigned to the value that is converted from <paramref name="str" />.</param>
	///     <param name="str">The string to convert.</param>
	///     <summary>Converts <paramref name="str" /> to the appropriate type and assigns it to <paramref name="param" />.</summary>
	///     <returns>The new audio unit value.</returns>
	delegate float AUImplementorValueFromStringCallback (AUParameter param, string str);
	[NoTV]
	[MacCatalyst (13, 1)]
	delegate void AUMidiCIProfileChangedCallback (byte cable, byte channel, MidiCIProfile profile, bool enabled);

	/// <summary>A subclass of <see cref="T:AVFoundation.AVAudioNode" /> whose subclasses process audio.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/AudioUnit/Reference/AUAudioUnit_ClassReference/index.html">Apple documentation for <c>AUAudioUnit</c></related>
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

		/// <summary>Gets the component from the description with which the audio unit was created.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("componentDescription")]
		AudioComponentDescription ComponentDescription { get; }

		/// <summary>Gets the render block that is used for rendering audio.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("renderBlock")]
		AURenderBlock RenderBlock { get; }

		/// <summary>Gets the developer-provided block that performs audio rendering.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("internalRenderBlock")]
		AUInternalRenderBlock InternalRenderBlock { get; }

		// @property (readonly, nonatomic) AudioComponent __nonnull component;
		/// <summary>Gets the component from the description with which the audio unit was created.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("component")]
		unsafe AudioComponent Component { get; }

		/// <summary>Gets the audio units component name.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed, Export ("componentName")]
		string ComponentName { get; }

		/// <summary>Gets the name of the audio unit.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed, Export ("audioUnitName")]
		string AudioUnitName { get; }

		/// <summary>Gets the manufacturer name for the audio unit.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed, Export ("manufacturerName")]
		string ManufacturerName { get; }

		/// <summary>To be added.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("audioUnitShortName")]
		string ShortName { get; }

		/// <summary>Gets the version of the audio unit.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("componentVersion")]
		uint ComponentVersion { get; }

		[Export ("allocateRenderResourcesAndReturnError:")]
		bool AllocateRenderResources ([NullAllowed] out NSError outError);

		[Export ("deallocateRenderResources")]
		void DeallocateRenderResources ();

		/// <summary>Gets a Boolean value that tells whether rendering resources have been allocated.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("renderResourcesAllocated")]
		bool RenderResourcesAllocated { get; }

		[Export ("reset")]
		void Reset ();

		/// <summary>Gets an array that contains the input connections for the audio unit.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("inputBusses")]
		AUAudioUnitBusArray InputBusses { get; }

		/// <summary>Gets an array that contains the output connections for the audio unit.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("outputBusses")]
		AUAudioUnitBusArray OutputBusses { get; }

		/// <summary>Block that schedules parameters.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("scheduleParameterBlock")]
		AUScheduleParameterBlock ScheduleParameterBlock { get; }

		// TODO: https://github.com/xamarin/xamarin-macios/issues/12489
		// [TV (15,0), iOS (15,0), MacCatalyst (15,0)]
		// [NullAllowed]
		// [Export ("scheduleMIDIEventListBlock")]
		// AUMidiEventListBlock ScheduleMidiEventListBlock { get; }

		// 		[Export ("tokenByAddingRenderObserver:")]
		// 		nint GetToken (AURenderObserver observer);

		// 		[NullAllowed, Export ("scheduleMIDIEventBlock")]
		// 		AUScheduleMidiEventBlock ScheduleMidiEventBlock { get; }

		// 		[NullAllowed, Export ("musicalContextBlock", ArgumentSemantic.Copy)]
		// 		AUHostMusicalContextBlock MusicalContextBlock { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("MIDIOutputNames", ArgumentSemantic.Copy)]
		string [] MidiOutputNames { get; }

		// TODO: https://github.com/xamarin/xamarin-macios/issues/12489
		// [TV (15,0), iOS (15,0), MacCatalyst (15,0)]
		// [NullAllowed]
		// [Export ("MIDIOutputEventListBlock", ArgumentSemantic.Copy)]
		// AUMidiEventListBlock MidiOutputEventListBlock { get; set; }

		// TODO: https://github.com/xamarin/xamarin-macios/issues/12489
		// [TV (15,0), iOS (15,0), MacCatalyst (15,0)]
		// [Export ("AudioUnitMIDIProtocol")]
		// MIDIProtocolID AudioUnitMidiProtocol { get; }

		// TODO: https://github.com/xamarin/xamarin-macios/issues/12489
		// [TV (15,0), iOS (15,0), MacCatalyst (15,0)]
		// [Export ("hostMIDIProtocol", ArgumentSemantic.Assign)]
		// MIDIProtocolID HostMIDIProtocol { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("providesUserInterface")]
		bool ProvidesUserInterface { get; }

		/// <summary>To be added.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("MIDIOutputEventBlock", ArgumentSemantic.Copy)]
		AUMidiOutputEventBlock MidiOutputEventBlock { get; set; }

		/// <summary>A delegate block that provides information about its transport state.</summary>
		///         <value>
		///           <para>Developers may optionally assign an instance of the <see cref="P:AudioUnit.AUAudioUnit.TransportStateBlock" /> delegate to thi property so that they can call it at the beginning of render cycles to get the transport state at the cycle start.</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed, Export ("transportStateBlock", ArgumentSemantic.Copy)]
		AUHostTransportStateBlock TransportStateBlock { get; set; }

		[Export ("removeRenderObserver:")]
		void RemoveRenderObserver (nint token);

		/// <summary>Gets or sets the maximum number of audio frames to render simultaneously.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("maximumFramesToRender")]
		uint MaximumFramesToRender { get; set; }

		/// <summary>Gets or sets the entire tree of output parameters.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed, Export ("parameterTree")]

		AUParameterTree ParameterTree {
			get;
			[TV (13, 0), iOS (13, 0)]
			[MacCatalyst (13, 1)]
			set;
		}

		[Export ("parametersForOverviewWithCount:")]
		NSNumber [] GetParametersForOverview (nint count);

		/// <summary>Gets a special value that is required for key-value observing notifications</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("allParameterValues")]
		bool AllParameterValues { get; }

		/// <summary>Gets a value that is <see langword="true" /> if the audio unit is either a music device or an effect.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("musicDeviceOrEffect")]
		bool MusicDeviceOrEffect { [Bind ("isMusicDeviceOrEffect")] get; }

		/// <summary>Gets the number of virtual MIDI cables that are implemented by the device or effect.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("virtualMIDICableCount")]
		nint VirtualMidiCableCount { get; }

		// @property (copy, nonatomic) NSDictionary<NSString * __nonnull,id __nonnull> * __nullable fullState;
		/// <summary>Gets or sets the snapshot of the audio unit's entire state, for saving presets.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed, Export ("fullState", ArgumentSemantic.Copy)]
		NSDictionary FullState { get; set; }

		// @property (copy, nonatomic) NSDictionary<NSString * __nonnull,id __nonnull> * __nullable fullStateForDocument;
		/// <summary>Gets or sets the snapshot of the audio unit's entire state, for saving in a document.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed, Export ("fullStateForDocument", ArgumentSemantic.Copy)]
		NSDictionary FullStateForDocument { get; set; }

		/// <summary>Gets the factory presets for the audio unit.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed, Export ("factoryPresets", ArgumentSemantic.Copy)]
		AUAudioUnitPreset [] FactoryPresets { get; }

		/// <summary>Gets the most recently selected preset.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed, Export ("currentPreset", ArgumentSemantic.Retain)]
		AUAudioUnitPreset CurrentPreset { get; set; }

		/// <summary>Gets the latency of the audio unit, in seconds</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("latency")]
		double Latency { get; }

		/// <summary>Gets the tail time, in seconds, of the audio unit.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("tailTime")]
		double TailTime { get; }

		/// <summary>Gets or sets the rendering quality, a value in [0,127].</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("renderQuality", ArgumentSemantic.Assign)]
		nint RenderQuality { get; set; }

		/// <summary>Gets or sets a Boolean value that contols whether an effect should skip processing and go directly to the output.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("shouldBypassEffect")]
		bool ShouldBypassEffect { get; set; }

		/// <summary>Gets a Boolean value that tells whether the audio unit can transform input to output without allocating a new buffer.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("canProcessInPlace")]
		bool CanProcessInPlace { get; }

		/// <summary>Gets or sets a Boolean value that controls whether the audio unit is currently rendering offline.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("renderingOffline")]
		bool RenderingOffline { [Bind ("isRenderingOffline")] get; set; }

		/// <summary>Gets an array that describes valid combinations of input and output channels.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed, Export ("channelCapabilities", ArgumentSemantic.Copy)]
		NSNumber [] ChannelCapabilities { get; }

		/// <summary>Gets the name of the context to which the audio unit is connected.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed, Export ("contextName")]
		string ContextName { get; set; }

		/// <summary>Gets a Boolean values that tells whether MPE audio is supported.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("supportsMPE")]
		bool SupportsMpe { get; }

		/// <summary>Gets or sets the channel map.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
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

		/// <include file="../docs/api/AudioUnit/AUAudioUnit.xml" path="/Documentation/Docs[@DocId='P:AudioUnit.AUAudioUnit.AudioComponentRegistrationsChangedNotification']/*" />
		[Notification, Field ("kAudioComponentRegistrationsChangedNotification")]
		NSString AudioComponentRegistrationsChangedNotification { get; }

		/// <include file="../docs/api/AudioUnit/AUAudioUnit.xml" path="/Documentation/Docs[@DocId='P:AudioUnit.AUAudioUnit.AudioComponentInstanceInvalidationNotification']/*" />
		[Notification, Field ("kAudioComponentInstanceInvalidationNotification")]
		NSString AudioComponentInstanceInvalidationNotification { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("MIDIOutputBufferSizeHint")]
		nint MidiOutputBufferSizeHint { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("profileStateForCable:channel:")]
		MidiCIProfileState GetProfileState (byte cable, byte channel);

		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("profileChangedBlock", ArgumentSemantic.Assign)]
		AUMidiCIProfileChangedCallback ProfileChangedCallback { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("disableProfile:cable:onChannel:error:")]
		bool Disable (MidiCIProfile profile, byte cable, byte channel, [NullAllowed] out NSError outError);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("enableProfile:cable:onChannel:error:")]
		bool Enable (MidiCIProfile profile, byte cable, byte channel, [NullAllowed] out NSError outError);

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("userPresets", ArgumentSemantic.Copy)]
		AUAudioUnitPreset [] UserPresets { get; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("saveUserPreset:error:")]
		bool SaveUserPreset (AUAudioUnitPreset userPreset, [NullAllowed] out NSError outError);

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("deleteUserPreset:error:")]
		bool DeleteUserPreset (AUAudioUnitPreset userPreset, [NullAllowed] out NSError outError);

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("presetStateFor:error:")]
		[return: NullAllowed]
		NSDictionary<NSString, NSObject> GetPresetState (AUAudioUnitPreset userPreset, [NullAllowed] out NSError outError);

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("supportsUserPresets")]
		bool SupportsUserPresets { get; }

		[Introduced (PlatformName.MacCatalyst, 13, 0)] // needed since it's not in iOS
		[NoTV, NoiOS]
		[Export ("isLoadedInProcess")]
		bool IsLoadedInProcess { get; }

		[TV (16, 0), Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("migrateFromPlugin")]
		NSData [] MigrateFromPlugin { get; }
	}

	// kept separate from AUAudioUnit, quote:
	// These methods will fail if the audio unit is not an input/output audio unit.
	/// <summary>Defines the interface of a host to an audio unit.</summary>
	[MacCatalyst (13, 1)]
	[Category]
	[BaseType (typeof (AUAudioUnit))]
	interface AUAudioUnit_AUAudioInputOutputUnit {

		[NoTV, NoiOS]
		[NoMacCatalyst]
		[Export ("deviceID")]
		uint GetDeviceId ();

		[NoTV, NoiOS]
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
		[NoiOS, NoTV]
		[NoMacCatalyst]
		[Export ("deviceInputLatency")]
		double GetDeviceInputLatency ();

		[NoiOS, NoTV]
		[NoMacCatalyst]
		[Export ("deviceOutputLatency")]
		double GetDeviceOutputLatency ();

		[MacCatalyst (13, 1)]
		[Export ("running")]
		bool IsRunning ();
	}

	/// <summary>An input or output connection to an audio unit.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/AudioUnit/Reference/AUAudioUnitBus_ClassReference/index.html">Apple documentation for <c>AUAudioUnitBus</c></related>
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

	/// <summary>A container that holds <see cref="T:AudioUnit.AUAudioUnitBus" /> objects for an audio unit.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/AudioUnit/Reference/AUAudioUnitBusArray_ClassReference/index.html">Apple documentation for <c>AUAudioUnitBusArray</c></related>
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

	/// <summary>A name and identifier for a custom parameter preset.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/AudioUnit/Reference/AUAudioUnitPreset_ClassReference/index.html">Apple documentation for <c>AUAudioUnitPreset</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AUAudioUnitPreset : NSSecureCoding {
		[Export ("number", ArgumentSemantic.Assign)]
		nint Number { get; set; }

		[Export ("name")]
		string Name { get; set; }
	}

	/// <summary>An audio unit parameter.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/AudioUnit/Reference/AUParameter_ClassReference/index.html">Apple documentation for <c>AUParameter</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AUParameterNode))]
	interface AUParameter : NSSecureCoding {
		/// <summary>Gets the parameter's minimum value.</summary>
		///         <value>The parameter's minimum value.</value>
		///         <remarks>To be added.</remarks>
		[Export ("minValue")]
		float MinValue { get; }

		/// <summary>Gets the parameter's maximum value.</summary>
		///         <value>The parameter's maximum value.</value>
		///         <remarks>To be added.</remarks>
		[Export ("maxValue")]
		float MaxValue { get; }

		/// <summary>Gets the unit of measurement for the value of the parameter.</summary>
		///         <value>The unit of measurement for the value of the parameter.</value>
		///         <remarks>To be added.</remarks>
		[Export ("unit")]
		AudioUnitParameterUnit Unit { get; }

		/// <summary>Gets the localized unit of measurement for the value of the parameter.</summary>
		///         <value>
		///           <para>The localized unit of measurement for the value of the parameter.</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed, Export ("unitName")]
		string UnitName { get; }

		/// <summary>Gets the parameter option flags for this parameter.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("flags")]
		AudioUnitParameterOptions Flags { get; }

		/// <summary>Gets the address (a non-persistent numerical identifier) for the parameter.</summary>
		///         <value>A non-persistent numeric identifier for the parameter.</value>
		///         <remarks>To be added.</remarks>
		[Export ("address")]
		ulong Address { get; }

		/// <summary>Gets an array that contains allowable localized value names for the parameter.</summary>
		///         <value>
		///           <para>An array that contains allowable localized value names for the parameter.</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed, Export ("valueStrings", ArgumentSemantic.Copy)]
		string [] ValueStrings { get; }

		/// <summary>Gets an array of addresses for parameters that may be changed when the developer changes this parameter.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed, Export ("dependentParameters", ArgumentSemantic.Copy)]
		NSNumber [] DependentParameters { get; }

		/// <summary>Gets the current value of the parameter.</summary>
		///         <value>The current value of the parameter.</value>
		///         <remarks>To be added.</remarks>
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

	/// <summary>A node which represents a parameter or parameter group in an <see cref="T:AudioUnit.AUParameterTree" />.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/AudioUnit/Reference/AUParameterNode_ClassReference/index.html">Apple documentation for <c>AUParameterNode</c></related>
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

	/// <summary>A group of <see cref="T:AudioUnit.AUParameter" /> objects for an audio unit.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/AudioUnit/Reference/AUParameterGroup_ClassReference/index.html">Apple documentation for <c>AUParameterGroup</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AUParameterNode))]
	interface AUParameterGroup : NSSecureCoding {
		[Export ("children")]
		AUParameterNode [] Children { get; }

		[Export ("allParameters")]
		AUParameter [] AllParameters { get; }
	}

	/// <summary>A tree that contains all of the audio unit parameters for an audio unit.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/AudioUnit/Reference/AUParameterTree_ClassReference/index.html">Apple documentation for <c>AUParameterTree</c></related>
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

	/// <summary>Interface that version 3 Audio Unit extensions must implement.</summary>
	///     <remarks>
	///       <para>Developers who want to create a version 3 Audio Unit extension must implement this interface on a class that inherits from <see cref="T:Foundation.NSObject" /> or <see cref="T:CoreAudioKit.AUViewController" />.</para>
	///     </remarks>
	[Protocol]
	interface AUAudioUnitFactory : NSExtensionRequestHandling {
		[Abstract]
		[Export ("createAudioUnitWithComponentDescription:error:")]
		[return: NullAllowed]
		AUAudioUnit CreateAudioUnit (AudioComponentDescription desc, [NullAllowed] out NSError error);
	}
}
