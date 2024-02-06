//
// coremidi.cs: API definition for CoreMidi's Foundation based types
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2012 Xamarin Inc
//
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
using Foundation;
using CoreGraphics;
using ObjCRuntime;

using MidiObjectRef = System.Int32;

#if !NET
using NativeHandle = System.IntPtr;
#endif

#if TVOS || WATCH
using MidiEndpoint = System.Object;
using MidiCIDeviceIdentification = System.Object;
using MidiCIDeviceIdentification_Blittable = System.Object;
#endif

namespace CoreMidi {


	[Watch (8, 0)]
	[TV (15, 0)]
	[MacCatalyst (13, 1)]
	// NSUInteger -> MIDINetworkSession.h
	[Native]
	public enum MidiNetworkConnectionPolicy : ulong {
		NoOne,
		HostsInContactsList,
		Anyone,
	}

	[Mac (11, 0), iOS (14, 0), TV (15, 0), Watch (8, 0)]
	[MacCatalyst (14, 0)]
	[NativeName ("MIDIProtocolID")]
	public enum MidiProtocolId {
		Protocol_1_0 = 1,
		Protocol_2_0 = 2,
	}

	[Mac (11, 0), iOS (14, 0), TV (15, 0), Watch (8, 0)]
	[MacCatalyst (14, 0)]
	[NativeName ("MIDICVStatus")]
	public enum MidiCVStatus : uint {
		RegisteredPnc = 0,
		AssignablePnc = 1,
		RegisteredControl = 2,
		AssignableControl = 3,
		RelRegisteredControl = 4,
		RelAssignableControl = 5,
		PerNotePitchBend = 6,
		// 7 is not present in the headers..
		NoteOff = 8,
		NoteOn = 9,
		PolyPressure = 10,
		ControlChange = 11,
		ProgramChange = 12,
		ChannelPressure = 13,
		PitchBend = 14,
		PerNoteMgmt = 15,
	}

	[Mac (11, 0), iOS (14, 0), TV (15, 0), Watch (8, 0)]
	[MacCatalyst (14, 0)]
	[NativeName ("MIDIMessageType")]
	public enum MidiMessageType : uint {
		Utility = 0,
		System = 1,
		ChannelVoice1 = 2,
		SysEx = 3,
		ChannelVoice2 = 4,
		Data128 = 5,
		UnknownF = 15,
	}

	[Mac (11, 0), iOS (14, 0), TV (15, 0), Watch (8, 0)]
	[MacCatalyst (14, 0)]
	[NativeName ("MIDISysExStatus")]
	public enum MidiSysExStatus : uint {
		Complete = 0,
		Start = 1,
		Continue = 2,
		End = 3,
		MixedDataSetHeader = 8,
		MixedDataSetPayload = 9,
	}

	[Mac (11, 0), iOS (14, 0), TV (15, 0), Watch (8, 0)]
	[MacCatalyst (14, 0)]
	[NativeName ("MIDISystemStatus")]
	public enum MidiSystemStatus : uint {
		StartOfExclusive = 240,
		EndOfExclusive = 247,
		Mtc = 241,
		SongPosPointer = 242,
		SongSelect = 243,
		TuneRequest = 246,
		TimingClock = 248,
		Start = 250,
		Continue = 251,
		Stop = 252,
		ActiveSending = 254,
		SystemReset = 255,
	}

	[iOS (16, 0), Mac (13, 0), MacCatalyst (16, 0), TV (15, 0), Watch (8, 0)]
	[NativeName ("MIDINoteAttribute")]
	public enum MidiNoteAttribute : byte {
		None = 0,
		ManufacturerSpecific = 1,
		ProfileSpecific = 2,
		Pitch = 3,
	}

	[iOS (16, 0), Mac (13, 0), MacCatalyst (16, 0), TV (15, 0), Watch (8, 0)]
	[NativeName ("MIDIPerNoteManagementOptions")]
	[Flags]
	public enum MidiPerNoteManagementOptions : byte {
		Reset = 1 << 0,
		Detach = 1 << 1,
	}

	[iOS (16, 0), Mac (13, 0), MacCatalyst (16, 0), TV (15, 0), Watch (8, 0)]
	[NativeName ("MIDIProgramChangeOptions")]
	[Flags]
	public enum MidiProgramChangeOptions : byte {
		BankValid = 1 << 0,
	}

	[iOS (16, 0), Mac (13, 0), MacCatalyst (16, 0), TV (15, 0), Watch (8, 0)]
	[NativeName ("MIDIUtilityStatus")]
	public enum MidiUtilityStatus : uint {
		Noop = 0,
		JitterReductionClock = 1,
		JitterReductionTimestamp = 2,
	}

	[NoTV]
	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "MIDINetworkHost")]
	[DisableDefaultCtor]
	interface MidiNetworkHost {
		[Export ("name", ArgumentSemantic.Retain)]
		string Name { get; }

		[Export ("address", ArgumentSemantic.Retain)]
		string Address { get; }

		[Export ("port")]
		nint Port { get; }

		[NullAllowed]
		[Export ("netServiceName", ArgumentSemantic.Retain)]
		string NetServiceName { get; }

		[NullAllowed]
		[Export ("netServiceDomain", ArgumentSemantic.Retain)]
		string NetServiceDomain { get; }

		[Static]
		[Export ("hostWithName:netService:")]
		MidiNetworkHost Create (string hostName, NSNetService netService);

		[Static]
		[Export ("hostWithName:netServiceName:netServiceDomain:")]
		MidiNetworkHost Create (string hostName, string netServiceName, string netServiceDomain);

		[Static]
		[Export ("hostWithName:address:port:")]
		MidiNetworkHost Create (string hostName, string address, nint port);

		[Export ("hasSameAddressAs:")]
		bool HasSameAddressAs (MidiNetworkHost other);
	}

	[NoTV]
	[NoWatch]
	[MacCatalyst (13, 1)]
	[Static]
	interface Midi {
		[Field ("MIDINetworkNotificationContactsDidChange")]
		[Notification]
		NSString NetworkNotificationContactsDidChange { get; }

		[Field ("MIDINetworkNotificationSessionDidChange")]
		[Notification]
		NSString NetworkNotificationSessionDidChange { get; }

		[Field ("MIDINetworkBonjourServiceType")]
		NSString NetworkBonjourServiceType { get; }
	}

	[NoTV]
	[NoWatch]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject), Name = "MIDINetworkConnection")]
	interface MidiNetworkConnection {
		[Export ("host", ArgumentSemantic.Retain)]
		MidiNetworkHost Host { get; }

		[Static, Export ("connectionWithHost:")]
		MidiNetworkConnection FromHost (MidiNetworkHost host);
	}

	[NoTV]
	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "MIDINetworkSession")]
	// default 'init' crash the application
	[DisableDefaultCtor]
	interface MidiNetworkSession {
		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[Export ("networkPort")]
		nint NetworkPort { get; }

		[Export ("networkName")]
		string NetworkName { get; }

		[Export ("localName")]
		string LocalName { get; }

		[Export ("connectionPolicy")]
		MidiNetworkConnectionPolicy ConnectionPolicy { get; set; }

		[Static]
		[Export ("defaultSession")]
		MidiNetworkSession DefaultSession { get; }

		[Export ("contacts")]
		NSSet Contacts { get; }

		[Export ("addContact:")]
		bool AddContact (MidiNetworkHost contact);

		[Export ("removeContact:")]
		bool RemoveContact (MidiNetworkHost contact);

		[Export ("connections")]
		NSSet Connections { get; }

		[Export ("addConnection:")]
		bool AddConnection (MidiNetworkConnection connection);

		[Export ("removeConnection:")]
		bool RemoveConnection (MidiNetworkConnection connection);

		[Export ("sourceEndpoint")]
		[Internal]
		int /* MIDIObjectRef = UInt32 */ _SourceEndpoint { get; }

#if NET
		[Wrap ("new MidiEndpoint (_SourceEndpoint)")]
		MidiEndpoint GetSourceEndpoint ();
#else
		[Wrap ("new MidiEndpoint (_SourceEndpoint)")]
		MidiEndpoint SourceEndpoint { get; }
#endif

		[Export ("destinationEndpoint")]
		[Internal]
		int /* MIDIObjectRef = UInt32 */ _DestinationEndpoint { get; }

#if NET
		[Wrap ("new MidiEndpoint (_DestinationEndpoint)")]
		MidiEndpoint GetDestinationEndPoint ();
#else
		[Wrap ("new MidiEndpoint (_DestinationEndpoint)")]
		MidiEndpoint DestinationEndPoint { get; }
#endif

	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "MIDICIProfile")]
	[DisableDefaultCtor]
	interface MidiCIProfile : NSSecureCoding {
		[Export ("name")]
		string Name { get; }

		[Export ("profileID")]
		NSData ProfileId { get; }

		[Export ("initWithData:name:")]
		NativeHandle Constructor (NSData data, string inName);

		[Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithData:")]
		NativeHandle Constructor (NSData data);
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "MIDICIProfileState")]
	[DisableDefaultCtor]
	interface MidiCIProfileState : NSSecureCoding {
		[Export ("enabledProfiles")]
		MidiCIProfile [] EnabledProfiles { get; }

		[Export ("disabledProfiles")]
		MidiCIProfile [] DisabledProfiles { get; }

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use the '(byte midiChannel, MidiCIProfile[] enabled, MidiCIProfile[] disabled)' constructor instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use the '(byte midiChannel, MidiCIProfile[] enabled, MidiCIProfile[] disabled)' constructor instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use the '(byte midiChannel, MidiCIProfile[] enabled, MidiCIProfile[] disabled)' constructor instead.")]
		[Export ("initWithEnabledProfiles:disabledProfiles:")]
		NativeHandle Constructor (MidiCIProfile [] enabled, MidiCIProfile [] disabled);

		[Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithChannel:enabledProfiles:disabledProfiles:")]
		NativeHandle Constructor (byte midiChannelNumber, MidiCIProfile [] enabled, MidiCIProfile [] disabled);

		[Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("midiChannel")]
		byte MidiChannel { get; }
	}

	delegate void MidiCIProfileChangedHandler (MidiCISession session, byte channel, MidiCIProfile profile, bool enabled);
	delegate void MidiCIPropertyResponseHandler (MidiCISession session, byte channel, NSData response, NSError error);
	delegate void MidiCIPropertyChangedHandler (MidiCISession session, byte channel, NSData data);
	delegate void MidiCIProfileSpecificDataHandler (MidiCISession session, byte channel, MidiCIProfile profile, NSData data);
	delegate void MidiCISessionDisconnectHandler (MidiCISession session, NSError error);

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "MIDICISession")]
	[DisableDefaultCtor]
	interface MidiCISession {
		[Export ("entity")]
		uint Entity { get; }

		[Export ("supportsProfileCapability")]
		bool SupportsProfileCapability { get; }

		[Export ("supportsPropertyCapability")]
		bool SupportsPropertyCapability { get; }

#if XAMCORE_5_0
		[Export ("deviceIdentification")]
		MidiCIDeviceIdentification DeviceIdentification { get; }
#else
		[Internal]
		[Export ("deviceIdentification")]
		MidiCIDeviceIdentification_Blittable _DeviceIdentification { get; }

		[Wrap ("_DeviceIdentification.ToMidiCIDeviceIdentification ()", IsVirtual = true)]
		MidiCIDeviceIdentification DeviceIdentification { get; }
#endif

		[Export ("profileStateForChannel:")]
		MidiCIProfileState GetProfileState (byte channel);

		[Export ("enableProfile:onChannel:error:")]
		bool EnableProfile (MidiCIProfile profile, byte channel, [NullAllowed] out NSError outError);

		[Export ("disableProfile:onChannel:error:")]
		bool DisableProfile (MidiCIProfile profile, byte channel, [NullAllowed] out NSError outError);

		[NullAllowed, Export ("profileChangedCallback", ArgumentSemantic.Assign)]
		MidiCIProfileChangedHandler ProfileChangedCallback { get; set; }

		[Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithDiscoveredNode:dataReadyHandler:disconnectHandler:")]
		NativeHandle Constructor (MidiCIDiscoveredNode discoveredNode, Action dataReadyHandler, MidiCISessionDisconnectHandler disconnectHandler);

		[Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("sendProfile:onChannel:profileData:")]
		bool SendProfile (MidiCIProfile profile, byte channel, NSData profileSpecificData);

		[Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("deviceInfo")]
		MidiCIDeviceInfo DeviceInfo { get; }

		[Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[BindAs (typeof (ulong))]
		[Export ("maxSysExSize")]
		NSNumber MaxSysExSize { get; }

		[Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[BindAs (typeof (int))]
		[Export ("maxPropertyRequests")]
		NSNumber MaxPropertyRequests { get; }

		[Internal]
		[Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("midiDestination")]
		MidiObjectRef _MidiDestination { get; }

		[Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("profileSpecificDataHandler", ArgumentSemantic.Copy)]
		MidiCIProfileSpecificDataHandler ProfileSpecificDataHandler { get; set; }
	}

	[Mac (11, 0), iOS (14, 0), NoTV, NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject), Name = "MIDICIDeviceInfo")]
	[DisableDefaultCtor]
	interface MidiCIDeviceInfo : NSSecureCoding {
		[Export ("manufacturerID")]
		NSData ManufacturerId { get; }

		[Export ("family")]
		NSData Family { get; }

		[Export ("modelNumber")]
		NSData ModelNumber { get; }

		[Export ("revisionLevel")]
		NSData RevisionLevel { get; }

		[Internal]
		[Export ("midiDestination")]
		MidiObjectRef _MidiDestination { get; }

		[Wrap ("new MidiEndpoint (_MidiDestination)")]
		MidiEndpoint GetMidiDestination ();

		[Internal]
		[Export ("initWithDestination:manufacturer:family:model:revision:")]
		NativeHandle Constructor (MidiObjectRef midiDestination, NSData manufacturer, NSData family, NSData modelNumber, NSData revisionLevel);

		[Wrap ("this (midiDestination?.Handle ?? throw new ArgumentNullException (nameof (midiDestination)), manufacturer, family, modelNumber, revisionLevel)")]
		NativeHandle Constructor (MidiEndpoint midiDestination, NSData manufacturer, NSData family, NSData modelNumber, NSData revisionLevel);
	}

	[Mac (11, 0), iOS (14, 0), NoTV, NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject), Name = "MIDICIDiscoveredNode")]
	[DisableDefaultCtor]
	interface MidiCIDiscoveredNode : NSSecureCoding {
		[Internal]
		[Export ("destination")]
		MidiObjectRef _Destination { get; }

		[Wrap ("new MidiEndpoint (_Destination)")]
		MidiEndpoint GetDestination ();

		[Export ("deviceInfo")]
		MidiCIDeviceInfo DeviceInfo { get; }

		[Export ("supportsProfiles")]
		bool SupportsProfiles { get; }

		[Export ("supportsProperties")]
		bool SupportsProperties { get; }

		[Export ("maximumSysExSize")]
		[BindAs (typeof (ulong))]
		NSNumber MaximumSysExSize { get; }
	}

	delegate void MidiCIDiscoveryResponseDelegate (MidiCIDiscoveredNode [] discoveredNodes);

	[Mac (11, 0), iOS (14, 0), NoTV, NoWatch]
	[MacCatalyst (14, 0)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject), Name = "MIDICIDiscoveryManager")]
	interface MidiCIDiscoveryManager {
		[Static]
		[Export ("sharedInstance")]
		MidiCIDiscoveryManager SharedInstance { get; }

		[Export ("discoverWithHandler:")]
		void Discover (MidiCIDiscoveryResponseDelegate completedHandler);
	}

	interface IMidiCIProfileResponderDelegate { }

	[Mac (11, 0), iOS (14, 0), NoTV, NoWatch]
	[MacCatalyst (14, 0)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject), Name = "MIDICIProfileResponderDelegate")]
	interface MidiCIProfileResponderDelegate {
		[Abstract]
		[Export ("connectInitiator:withDeviceInfo:")]
		bool ConnectInitiator (NSNumber initiatorMuid, MidiCIDeviceInfo deviceInfo);

		[Abstract]
		[Export ("initiatorDisconnected:")]
		void InitiatorDisconnected (NSNumber initiatorMuid);

		[Export ("willSetProfile:onChannel:enabled:")]
		bool WillSetProfile (MidiCIProfile profile, byte channel, bool shouldEnable);

		[Export ("handleDataForProfile:onChannel:data:")]
		void HandleData (MidiCIProfile profile, byte channel, NSData inData);
	}

	[Mac (11, 0), iOS (14, 0), NoTV, NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject), Name = "MIDICIResponder")]
	[DisableDefaultCtor]
	interface MidiCIResponder {
		[BindAs (typeof (int []))]
		[Export ("initiators")]
		NSNumber [] Initiators { get; }

		[Wrap ("WeakProfileDelegate")]
		IMidiCIProfileResponderDelegate ProfileDelegate { get; }

		[Export ("profileDelegate", ArgumentSemantic.Retain)]
		NSObject WeakProfileDelegate { get; }

		[Export ("deviceInfo")]
		MidiCIDeviceInfo DeviceInfo { get; }

		[Export ("initWithDeviceInfo:profileDelegate:profileStates:supportProperties:")]
		NativeHandle Constructor (MidiCIDeviceInfo deviceInfo, IMidiCIProfileResponderDelegate @delegate, MidiCIProfileState [] profileList, bool propertiesSupported);

		[Export ("notifyProfile:onChannel:isEnabled:")]
		bool NotifyProfile (MidiCIProfile profile, byte channel, bool enabledState);

		[Export ("sendProfile:onChannel:profileData:")]
		bool SendProfile (MidiCIProfile profile, byte channel, NSData profileSpecificData);

		[Export ("start")]
		bool Start ();

		[Export ("stop")]
		void Stop ();
	}

}
