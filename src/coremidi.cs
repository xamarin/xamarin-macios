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
using MidiEndpointRef = System.Int32;
using MidiUmpFunctionBlockId = System.Byte;
using MidiUmpGroupNumber = System.Byte;
using MidiChannelNumber = System.Byte;

#if !NET
using NativeHandle = System.IntPtr;
#endif

#if TVOS
using MidiEndpoint = System.Object;
using MidiCIDeviceIdentification = System.Object;
using MidiCIDeviceIdentification_Blittable = System.Object;
using MidiCIProfileId = System.Object;
using Midi2DeviceManufacturer = System.Object;
using Midi2DeviceRevisionLevel = System.Object;
#endif

namespace CoreMidi {


	[TV (15, 0)]
	[MacCatalyst (13, 1)]
	// NSUInteger -> MIDINetworkSession.h
	[Native]
	public enum MidiNetworkConnectionPolicy : ulong {
		NoOne,
		HostsInContactsList,
		Anyone,
	}

	[iOS (14, 0), TV (15, 0)]
	[MacCatalyst (14, 0)]
	[NativeName ("MIDIProtocolID")]
	public enum MidiProtocolId {
		Protocol_1_0 = 1,
		Protocol_2_0 = 2,
	}

	[iOS (14, 0), TV (15, 0)]
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

	[iOS (14, 0), TV (15, 0)]
	[MacCatalyst (14, 0)]
	[NativeName ("MIDIMessageType")]
	public enum MidiMessageType : uint {
		Utility = 0,
		System = 1,
		ChannelVoice1 = 2,
		SysEx = 3,
		ChannelVoice2 = 4,
		Data128 = 5,
		FlexData = 0xD,
#if !XAMCORE_5_0
		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Stream' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Stream' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Stream' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Stream' instead.")]
		UnknownF = Stream,
#endif
		Stream = 0xF,
		Invalid = 0xFF,
	}

	[iOS (14, 0), TV (15, 0)]
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

	[iOS (14, 0), TV (15, 0)]
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

	[iOS (16, 0), Mac (13, 0), MacCatalyst (16, 0), TV (15, 0)]
	[NativeName ("MIDINoteAttribute")]
	public enum MidiNoteAttribute : byte {
		None = 0,
		ManufacturerSpecific = 1,
		ProfileSpecific = 2,
		Pitch = 3,
	}

	[iOS (16, 0), Mac (13, 0), MacCatalyst (16, 0), TV (15, 0)]
	[NativeName ("MIDIPerNoteManagementOptions")]
	[Flags]
	public enum MidiPerNoteManagementOptions : byte {
		Reset = 1 << 0,
		Detach = 1 << 1,
	}

	[iOS (16, 0), Mac (13, 0), MacCatalyst (16, 0), TV (15, 0)]
	[NativeName ("MIDIProgramChangeOptions")]
	[Flags]
	public enum MidiProgramChangeOptions : byte {
		BankValid = 1 << 0,
	}

	[iOS (16, 0), Mac (13, 0), MacCatalyst (16, 0), TV (15, 0)]
	[NativeName ("MIDIUtilityStatus")]
	public enum MidiUtilityStatus : uint {
		Noop = 0,
		JitterReductionClock = 1,
		JitterReductionTimestamp = 2,
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		DeltaClockstampTicksPerQuarterNote = 0x3,
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		TicksSinceLastEvent = 0x4
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[NativeName ("UMPStreamMessageStatus")]
	public enum UmpStreamMessageStatus : uint {
		EndpointDiscovery = 0x00,
		EndpointInfoNotification = 0x01,
		DeviceIdentityNotification = 0x02,
		EndpointNameNotification = 0x03,
		ProductInstanceIDNotification = 0x04,
		StreamConfigurationRequest = 0x05,
		StreamConfigurationNotification = 0x06,
		FunctionBlockDiscovery = 0x10,
		FunctionBlockInfoNotification = 0x11,
		FunctionBlockNameNotification = 0x12,
		StartOfClip = 0x20,
		EndOfClip = 0x21,
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[NativeName ("MIDIUMPFunctionBlockMIDI1Info")]
	public enum MidiUmpFunctionBlockMidi1Info {
		NotMidi1 = 0,
		UnrestrictedBandwidth = 1,
		RestrictedBandwidth = 2
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[NativeName ("MIDIUMPFunctionBlockUIHint")]
	public enum MidiUmpFunctionBlockUIHint {
		Unknown = 0,
		Receiver = 1,
		Sender = 2,
		SenderReceiver = 3
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[NativeName ("MIDIUMPFunctionBlockDirection")]
	public enum MidiUmpFunctionBlockDirection {
		Unknown = 0,
		Input = 1,
		Output = 2,
		Bidirectional = 3
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[NativeName ("UMPStreamMessageFormat")]
	public enum UmpStreamMessageFormat : byte {
		Complete = 0x00,
		Start = 0x01,
		Continuing = 0x02,
		End = 0x03
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[NativeName ("MIDICICategoryOptions")]
	public enum MidiCICategoryOptions : byte /* MIDIUInteger7 = typedef UInt8 MIDIUInteger7;   //! 7  bits usable; allowed values 0x0~0x7F */ {
		ProtocolNegotiation = (1 << 1),
		ProfileConfigurationSupported = (1 << 2),
		PropertyExchangeSupported = (1 << 3),
		ProcessInquirySupported = (1 << 4),
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[NativeName ("MIDICIDeviceType")]
	public enum MidiCIDeviceType : byte /* UInt8 */ {
		Unknown = 0,
		LegacyMidi1,
		Virtual,
		UsbMidi,
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[NativeName ("MIDICIProfileMessageType")]
	public enum MidiCIProfileMessageType : byte /* MIDIUInteger7 = typedef UInt8 MIDIUInteger7;   //! 7  bits usable; allowed values 0x0~0x7F */ {
		ProfileInquiry = 0x20,
		ReplyToProfileInquiry = 0x21,
		SetProfileOn = 0x22,
		SetProfileOff = 0x23,
		ProfileEnabledReport = 0x24,
		ProfileDisabledReport = 0x25,
		ProfileAdded = 0x26,
		ProfileRemoved = 0x27,
		DetailsInquiry = 0x28,
		ReplyToDetailsInquiry = 0x29,
		ProfileSpecificData = 0x2F,
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[NativeName ("MIDICIPropertyExchangeMessageType")]
	public enum MidiCIPropertyExchangeMessageType : byte /* MIDIUInteger7 = typedef UInt8 MIDIUInteger7;   //! 7  bits usable; allowed values 0x0~0x7F */ {
		InquiryPropertyExchangeCapabilities = 0x30,
		ReplyToPropertyExchangeCapabilities = 0x31,
		InquiryHasPropertyData_Reserved = 0x32,
		InquiryReplyToHasPropertyData_Reserved = 0x33,
		InquiryGetPropertyData = 0x34,
		ReplyToGetProperty = 0x35,
		InquirySetPropertyData = 0x36,
		ReplyToSetPropertyData = 0x37,
		Subscription = 0x38,
		ReplyToSubscription = 0x39,
		Notify = 0x3F,
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[NativeName ("MIDICIProcessInquiryMessageType")]
	public enum MidiCIProcessInquiryMessageType : byte /* MIDIUInteger7 = typedef UInt8 MIDIUInteger7;   //! 7  bits usable; allowed values 0x0~0x7F */ {
		InquiryProcessInquiryCapabilities = 0x40,
		ReplyToProcessInquiryCapabilities = 0x41,
		InquiryMidiMessageReport = 0x42,
		ReplyToMidiMessageReport = 0x43,
		EndOfMidiMessageReport = 0x44,
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[NativeName ("MIDICIManagementMessageType")]
	public enum MidiCIManagementMessageType : byte /* MIDIUInteger7 = typedef UInt8 MIDIUInteger7;   //! 7  bits usable; allowed values 0x0~0x7F */ {
		Discovery = 0x70,
		ReplyToDiscovery = 0x71,
		InquiryEndpointInformation = 0x72,
		ReplyToEndpointInformation = 0x73,
		MIDICIACK = 0x7D,
		InvalidateMUID = 0x7E,
		MIDICINAK = 0x7F,
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[NativeName ("MIDICIProfileType")]
	public enum MidiCIProfileType : byte /* UInt8  */ {
		SingleChannel = 1,
		Group = 2,
		FunctionBlock = 3,
		Multichannel = 4,
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[NativeName ("MIDIUMPCIObjectBackingType")]
	public enum MidiUmpCIObjectBackingType : byte /* UInt8  */ {
		Unknown = 0,
		Virtual,
		DriverDevice,
		UsbMidi,
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[NativeName ("MIDICIPropertyExchangeRequestID")]
	public enum MidiCIPropertyExchangeRequestID : byte /* UInt8  */ {
		BadRequest = 0xFF,
	}

	/// <summary>A remote MIDI host.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreMidi/Reference/MIDINetworkHost_ClassReference/index.html">Apple documentation for <c>MIDINetworkHost</c></related>
	[NoTV]
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

	/// <summary>Global methods and constants for using CoreMidi.</summary>
	[NoTV]
	[MacCatalyst (13, 1)]
	[Static]
	interface Midi {
		/// <include file="../docs/api/CoreMidi/Midi.xml" path="/Documentation/Docs[@DocId='P:CoreMidi.Midi.NetworkNotificationContactsDidChange']/*" />
	[Field ("MIDINetworkNotificationContactsDidChange")]
		[Notification]
		NSString NetworkNotificationContactsDidChange { get; }

		/// <include file="../docs/api/CoreMidi/Midi.xml" path="/Documentation/Docs[@DocId='P:CoreMidi.Midi.NetworkNotificationSessionDidChange']/*" />
	[Field ("MIDINetworkNotificationSessionDidChange")]
		[Notification]
		NSString NetworkNotificationSessionDidChange { get; }

		/// <summary>Represents the value associated with the constant MIDINetworkBonjourServiceType</summary>
		///         <value>
		///         </value>
		///         <remarks>You can pass this type to <format type="text/html"><a href="https://docs.microsoft.com/en-us/search/index?search=M:Foundation.NSNetServiceBrowser.SearchForServices(System.String type, System.String domain)&amp;scope=Xamarin" title="M:Foundation.NSNetServiceBrowser.SearchForServices(System.String type, System.String domain)">M:Foundation.NSNetServiceBrowser.SearchForServices(System.String type, System.String domain)</a></format> to find MIDI devices available
		///         over bonjour.</remarks>
		[Field ("MIDINetworkBonjourServiceType")]
		NSString NetworkBonjourServiceType { get; }
	}

	/// <summary>A connection to a MIDI network host, using a <see cref="T:CoreMidi.MidiNetworkSession" />.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreMidi/Reference/MIDINetworkConnection_ClassReference/index.html">Apple documentation for <c>MIDINetworkConnection</c></related>
	[NoTV]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject), Name = "MIDINetworkConnection")]
	interface MidiNetworkConnection {
		[Export ("host", ArgumentSemantic.Retain)]
		MidiNetworkHost Host { get; }

		[Static, Export ("connectionWithHost:")]
		MidiNetworkConnection FromHost (MidiNetworkHost host);
	}

	/// <summary>A singleton class that maintains the <see cref="T:CoreMidi.MidiNetworkConnection" />s between various MIDI entities.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreMidi/Reference/MIDINetworkSession_ClassReference/index.html">Apple documentation for <c>MIDINetworkSession</c></related>
	[NoTV]
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

	[NoTV]
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

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithData:")]
		NativeHandle Constructor (NSData data);

		/* Notifications */
		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("MIDICIProfileWasUpdatedNotification")]
		[Notification]
		NSString WasUpdatedNotification { get; }

		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("MIDICIProfileWasRemovedNotification")]
		[Notification]
		NSString WasRemovedNotification { get; }
	}

	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "MIDICIProfileState")]
	[DisableDefaultCtor]
	interface MidiCIProfileState : NSSecureCoding {
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("enabledProfiles")]
		MidiCIProfile [] EnabledProfiles { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("disabledProfiles")]
		MidiCIProfile [] DisabledProfiles { get; }

		[Export ("initWithEnabledProfiles:disabledProfiles:")]
		NativeHandle Constructor (MidiCIProfile [] enabled, MidiCIProfile [] disabled);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithChannel:enabledProfiles:disabledProfiles:")]
		[Deprecated (PlatformName.iOS, 18, 0, message: "Use the other constructor instead (without the 'midiChannelNumber' parameter).")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use the other constructor instead (without the 'midiChannelNumber' parameter).")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use the other constructor instead (without the 'midiChannelNumber' parameter).")]
		NativeHandle Constructor (byte midiChannelNumber, MidiCIProfile [] enabled, MidiCIProfile [] disabled);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("midiChannel")]
		byte MidiChannel { get; }
	}

	delegate void MidiCIProfileChangedHandler (MidiCISession session, byte channel, MidiCIProfile profile, bool enabled);
	delegate void MidiCIProfileSpecificDataHandler (MidiCISession session, byte channel, MidiCIProfile profile, NSData data);
	delegate void MidiCISessionDisconnectHandler (MidiCISession session, NSError error);

	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "MIDICISession")]
	[DisableDefaultCtor]
	[Deprecated (PlatformName.iOS, 18, 0, message: "No longer supported.")]
	[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "No longer supported.")]
	[Deprecated (PlatformName.MacOSX, 15, 0, message: "No longer supported.")]
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

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithDiscoveredNode:dataReadyHandler:disconnectHandler:")]
		NativeHandle Constructor (MidiCIDiscoveredNode discoveredNode, Action dataReadyHandler, MidiCISessionDisconnectHandler disconnectHandler);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("sendProfile:onChannel:profileData:")]
		bool SendProfile (MidiCIProfile profile, byte channel, NSData profileSpecificData);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("deviceInfo")]
		MidiCIDeviceInfo DeviceInfo { get; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[BindAs (typeof (ulong))]
		[Export ("maxSysExSize")]
		NSNumber MaxSysExSize { get; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[BindAs (typeof (int))]
		[Export ("maxPropertyRequests")]
		NSNumber MaxPropertyRequests { get; }

		[Internal]
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("midiDestination")]
		MidiObjectRef _MidiDestination { get; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("profileSpecificDataHandler", ArgumentSemantic.Copy)]
		MidiCIProfileSpecificDataHandler ProfileSpecificDataHandler { get; set; }
	}

	[iOS (14, 0), NoTV]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject), Name = "MIDICIDeviceInfo")]
	[DisableDefaultCtor]
	[Deprecated (PlatformName.iOS, 18, 0, message: "No longer supported.")]
	[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "No longer supported.")]
	[Deprecated (PlatformName.MacOSX, 15, 0, message: "No longer supported.")]
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

	[iOS (14, 0), NoTV]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject), Name = "MIDICIDiscoveredNode")]
	[DisableDefaultCtor]
	[Deprecated (PlatformName.iOS, 18, 0, message: "No longer supported.")]
	[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "No longer supported.")]
	[Deprecated (PlatformName.MacOSX, 15, 0, message: "No longer supported.")]
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

	[iOS (14, 0), NoTV]
	[MacCatalyst (14, 0)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject), Name = "MIDICIDiscoveryManager")]
	[Deprecated (PlatformName.iOS, 18, 0, message: "No longer supported.")]
	[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "No longer supported.")]
	[Deprecated (PlatformName.MacOSX, 15, 0, message: "No longer supported.")]
	interface MidiCIDiscoveryManager {
		[Static]
		[Export ("sharedInstance")]
		MidiCIDiscoveryManager SharedInstance { get; }

		[Export ("discoverWithHandler:")]
		void Discover (MidiCIDiscoveryResponseDelegate completedHandler);
	}

	interface IMidiCIProfileResponderDelegate { }

	[iOS (14, 0), NoTV]
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

	[iOS (14, 0), NoTV]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject), Name = "MIDICIResponder")]
	[DisableDefaultCtor]
	[Deprecated (PlatformName.iOS, 18, 0, message: "No longer supported.")]
	[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "No longer supported.")]
	[Deprecated (PlatformName.MacOSX, 15, 0, message: "No longer supported.")]
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

	[Internal]
	enum MidiProperty {
		[NoTV]
		[Field ("kMIDIPropertyName")]
		Name,

		[NoTV]
		[Field ("kMIDIPropertyManufacturer")]
		Manufacturer,

		[NoTV]
		[Field ("kMIDIPropertyModel")]
		Model,

		[NoTV]
		[Field ("kMIDIPropertyUniqueID")]
		UniqueId,

		[NoTV]
		[Field ("kMIDIPropertyDeviceID")]
		DeviceId,

		[NoTV]
		[Field ("kMIDIPropertyReceiveChannels")]
		ReceiveChannels,

		[NoTV]
		[Field ("kMIDIPropertyTransmitChannels")]
		TransmitChannels,

		[NoTV]
		[Field ("kMIDIPropertyMaxSysExSpeed")]
		MaxSysExSpeed,

		[NoTV]
		[Field ("kMIDIPropertyAdvanceScheduleTimeMuSec")]
		AdvanceScheduleTimeMuSec,

		[NoTV]
		[Field ("kMIDIPropertyIsEmbeddedEntity")]
		IsEmbeddedEntity,

		[NoTV]
		[Field ("kMIDIPropertyIsBroadcast")]
		IsBroadcast,

		[NoTV]
		[Field ("kMIDIPropertySingleRealtimeEntity")]
		SingleRealtimeEntity,

		[NoTV]
		[Field ("kMIDIPropertyConnectionUniqueID")]
		ConnectionUniqueId,

		[NoTV]
		[Field ("kMIDIPropertyOffline")]
		Offline,

		[NoTV]
		[Field ("kMIDIPropertyPrivate")]
		Private,

		[NoTV]
		[Field ("kMIDIPropertyDriverOwner")]
		DriverOwner,

		[NoiOS, NoTV, NoMacCatalyst]
		[Field ("kMIDIPropertyFactoryPatchNameFile")]
		FactoryPatchNameFile,

		[NoiOS, NoTV, NoMacCatalyst]
		[Field ("kMIDIPropertyUserPatchNameFile")]
		UserPatchNameFile,

		[NoTV]
		[Field ("kMIDIPropertyNameConfiguration")]
		NameConfiguration,

		[NoTV]
		[Field ("kMIDIPropertyNameConfigurationDictionary")]
		NameConfigurationDictionary,

		[NoTV]
		[Field ("kMIDIPropertyImage")]
		Image,

		[NoTV]
		[Field ("kMIDIPropertyDriverVersion")]
		DriverVersion,

		[NoTV]
		[Field ("kMIDIPropertySupportsGeneralMIDI")]
		SupportsGeneralMidi,

		[NoTV]
		[Field ("kMIDIPropertySupportsMMC")]
		SupportsMmc,

		[NoTV]
		[Field ("kMIDIPropertyCanRoute")]
		CanRoute,

		[NoTV]
		[Field ("kMIDIPropertyReceivesClock")]
		ReceivesClock,

		[NoTV]
		[Field ("kMIDIPropertyReceivesMTC")]
		ReceivesMtc,

		[NoTV]
		[Field ("kMIDIPropertyReceivesNotes")]
		ReceivesNotes,

		[NoTV]
		[Field ("kMIDIPropertyReceivesProgramChanges")]
		ReceivesProgramChanges,

		[NoTV]
		[Field ("kMIDIPropertyReceivesBankSelectMSB")]
		ReceivesBankSelectMsb,

		[NoTV]
		[Field ("kMIDIPropertyReceivesBankSelectLSB")]
		ReceivesBankSelectLsb,

		[NoTV]
		[Field ("kMIDIPropertyTransmitsClock")]
		TransmitsClock,

		[NoTV]
		[Field ("kMIDIPropertyTransmitsMTC")]
		TransmitsMtc,

		[NoTV]
		[Field ("kMIDIPropertyTransmitsNotes")]
		TransmitsNotes,

		[NoTV]
		[Field ("kMIDIPropertyTransmitsProgramChanges")]
		TransmitsProgramChanges,

		[NoTV]
		[Field ("kMIDIPropertyTransmitsBankSelectMSB")]
		TransmitsBankSelectMsb,

		[NoTV]
		[Field ("kMIDIPropertyTransmitsBankSelectLSB")]
		TransmitsBankSelectLsb,

		[NoTV]
		[Field ("kMIDIPropertyPanDisruptsStereo")]
		PanDisruptsStereo,

		[NoTV]
		[Field ("kMIDIPropertyIsSampler")]
		IsSampler,

		[NoTV]
		[Field ("kMIDIPropertyIsDrumMachine")]
		IsDrumMachine,

		[NoTV]
		[Field ("kMIDIPropertyIsMixer")]
		IsMixer,

		[NoTV]
		[Field ("kMIDIPropertyIsEffectUnit")]
		IsEffectUnit,

		[NoTV]
		[Field ("kMIDIPropertyMaxReceiveChannels")]
		MaxReceiveChannels,

		[NoTV]
		[Field ("kMIDIPropertyMaxTransmitChannels")]
		MaxTransmitChannels,

		[NoTV]
		[Field ("kMIDIPropertyDriverDeviceEditorApp")]
		DriverDeviceEditorApp,

		[NoTV]
		[Field ("kMIDIPropertySupportsShowControl")]
		SupportsShowControl,

		[NoTV]
		[Field ("kMIDIPropertyDisplayName")]
		DisplayName,

		[NoTV]
		[Field ("kMIDIPropertyProtocolID")]
		ProtocolId,

		[Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0), NoTV]
		[Field ("kMIDIPropertyUMPActiveGroupBitmap")]
		UmpActiveGroupBitmap,

		[Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0), NoTV]
		[Field ("kMIDIPropertyUMPCanTransmitGroupless")]
		UmpCanTransmitGroupless,

		[Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0), NoTV]
		[Field ("kMIDIPropertyAssociatedEndpoint")]
		AssociatedEndpoint,
	}

	[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (NSObject), Name = "MIDICIDevice")]
	[DisableDefaultCtor]
	interface MidiCIDevice {
		[Export ("deviceInfo")]
		Midi2DeviceInfo DeviceInfo { get; }

		[Export ("MUID")]
		/* MIDICIMUID -> MIDIUInteger28 -> UInt32 */
		uint Muid { get; }

		[Export ("supportsProtocolNegotiation")]
		bool SupportsProtocolNegotiation { get; }

		[Export ("supportsProfileConfiguration")]
		bool SupportsProfileConfiguration { get; }

		[Export ("supportsPropertyExchange")]
		bool SupportsPropertyExchange { get; }

		[Export ("supportsProcessInquiry")]
		bool SupportsProcessInquiry { get; }

		[Export ("maxSysExSize")]
		nuint MaxSysExSize { get; }

		[Export ("maxPropertyExchangeRequests")]
		nuint MaxPropertyExchangeRequests { get; }

		[Export ("deviceType")]
		MidiCIDeviceType DeviceType { get; }

		[Export ("profiles")]
		MidiUmpCIProfile [] Profiles { get; }

		/* Notifications */
		[Field ("MIDICIDeviceWasAddedNotification")]
		[Notification]
		NSString WasAddedNotification { get; }

		[Field ("MIDICIDeviceWasRemovedNotification")]
		[Notification]
		NSString WasRemovedNotification { get; }

	}

	[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (NSObject), Name = "MIDICIDeviceManager")]
	[DisableDefaultCtor]
	interface MidiCIDeviceManager {
		[Static]
		[Export ("sharedInstance")]
		MidiCIDeviceManager SharedInstance { get; }

		[Export ("discoveredCIDevices", ArgumentSemantic.Copy)]
		MidiCIDevice [] DiscoveredCIDevices { get; }

	}

	[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (NSObject), Name = "MIDIUMPCIProfile")]
	[DisableDefaultCtor]
	interface MidiUmpCIProfile {
		[Export ("name")]
		string Name { get; }

		[Export ("profileID")]
		MidiCIProfileId ProfileId { get; }

		[Export ("profileType")]
		MidiCIProfileType ProfileType { get; }

		[Export ("groupOffset")]
		MidiUmpGroupNumber GroupOffset { get; }

		[Export ("firstChannel")]
		MidiChannelNumber FirstChannel { get; }

		[Export ("enabledChannelCount")]
		ushort /* MIDIUInteger14 */ EnabledChannelCount { get; }

		[Export ("totalChannelCount")]
		ushort /* MIDIUInteger14 */ TotalChannelCount { get; }

		[Export ("isEnabled")]
		bool IsEnabled { get; }

		[Export ("setProfileState:enabledChannelCount:error:")]
		bool SetProfileState (bool isEnabled, ushort enabledChannelCount, out NSError error);
	}

	[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (NSObject), Name = "MIDI2DeviceInfo")]
	[DisableDefaultCtor]
	interface Midi2DeviceInfo {
		[Export ("manufacturerID")]
		Midi2DeviceManufacturer ManufacturerId { get; }

		[Export ("family")]
		ushort /* MIDIUInteger14 */ Family { get; }

		[Export ("modelNumber")]
		ushort /* MIDIUInteger14 */ ModelNumber { get; }

		[Export ("revisionLevel")]
		Midi2DeviceRevisionLevel RevisionLevel { get; }

		[Export ("initWithManufacturerID:family:modelNumber:revisionLevel:")]
		NativeHandle Constructor (Midi2DeviceManufacturer manufacturerId, ushort family, ushort modelNumber, Midi2DeviceRevisionLevel revisionLevel);
	}

	[Flags]
	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[NativeName ("MIDIUMPProtocolOptions")]
	public enum MidiUmpProtocolOptions : byte /* MIDIUInteger4 */ {
		SupportedProtocolMidi1 = 1,
		SupportedProtocolMidi2 = 1 << 1,
	}

	[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (NSObject), Name = "MIDIUMPEndpoint")]
	[DisableDefaultCtor]
	interface MidiUmpEndpoint {
		[Export ("name")]
		string Name { get; }

		[Export ("MIDIProtocol")]
		MidiProtocolId MidiProtocol { get; }

		[Export ("supportedMIDIProtocols")]
		MidiUmpProtocolOptions SupportedMidiProtocols { get; }

		[Export ("MIDIDestination")]
		MidiEndpointRef MidiDestination { get; }

		[Export ("MIDISource")]
		MidiEndpointRef MidiSource { get; }

		[Export ("deviceInfo")]
		Midi2DeviceInfo DeviceInfo { get; }

		[Export ("productInstanceID")]
		string ProductInstanceId { get; }

		[Export ("hasStaticFunctionBlocks")]
		bool HasStaticFunctionBlocks { get; }

		[Export ("hasJRTSReceiveCapability")]
		bool HasJrtsReceiveCapability { get; }

		[Export ("hasJRTSTransmitCapability")]
		bool HasJrtsTransmitCapability { get; }

		[Export ("endpointType")]
		MidiUmpCIObjectBackingType EndpointType { get; }

		[Export ("functionBlocks", ArgumentSemantic.Copy)]
		MidiUmpFunctionBlock [] FunctionBlocks { get; set; }

		/* Notifications */
		[Notification]
		[Field ("MIDIUMPEndpointWasAddedNotification")]
		NSString WasAddedNotification { get; }

		[Notification]
		[Field ("MIDIUMPEndpointWasRemovedNotification")]
		NSString WasRemovedNotification { get; }

		[Notification]
		[Field ("MIDIUMPEndpointWasUpdatedNotification")]
		NSString WasUpdatedNotification { get; }
	}

	[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (NSObject), Name = "MIDIUMPFunctionBlock")]
	[DisableDefaultCtor]
	interface MidiUmpFunctionBlock {
		[Export ("name")]
		string Name { get; }

		[Export ("functionBlockID")]
		MidiUmpFunctionBlockId FunctionBlockId { get; }

		[Export ("direction")]
		MidiUmpFunctionBlockDirection Direction { get; }

		[Export ("firstGroup")]
		MidiUmpGroupNumber FirstGroup { get; }

		[Export ("totalGroupsSpanned")]
		byte /* MIDIUInteger7 */ TotalGroupsSpanned { get; }

		[Export ("maxSysEx8Streams")]
		byte MaxSysEx8Streams { get; }

		[Export ("MIDI1Info")]
		MidiUmpFunctionBlockMidi1Info Midi1Info { get; }

		[Export ("UIHint")]
		MidiUmpFunctionBlockUIHint UIHint { get; }

		[Export ("UMPEndpoint", ArgumentSemantic.Weak), NullAllowed]
		MidiUmpEndpoint UmpEndpoint { get; }

		[Export ("midiCIDevice", ArgumentSemantic.Weak), NullAllowed]
		MidiCIDevice MidiCIDevice { get; }

		[Export ("isEnabled")]
		bool IsEnabled { get; }

		/* Notifications */
		[Notification]
		[Field ("MIDIUMPFunctionBlockWasUpdatedNotification")]
		NSString WasUpdatedNotification { get; }
	}

	[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[Static]
	interface MidiCIDeviceManagerDictionaryKey {
		[Field ("MIDICIDeviceObjectKey")]
		NSString CIDeviceObject { get; }

		[Field ("MIDICIProfileObjectKey")]
		NSString CIProfileObjectKey { get; }
	}

	[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[Static]
	interface MidiUmpEndpointManagerDictionaryKey {
		[Field ("MIDIUMPEndpointObjectKey")]
		NSString UmpEndpointObject { get; }

		[Field ("MIDIUMPFunctionBlockObjectKey")]
		NSString UmpFunctionBlockObject { get; }
	}

	[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (NSObject), Name = "MIDIUMPEndpointManager")]
	[DisableDefaultCtor]
	interface MidiUmpEndpointManager {
		[Static]
		[Export ("sharedInstance")]
		MidiUmpEndpointManager SharedInstance { get; }

		[Export ("UMPEndpoints", ArgumentSemantic.Copy)]
		MidiUmpEndpoint [] UmpEndpoints { get; }
	}

	delegate void MidiReceiveBlock (IntPtr eventList, IntPtr srcConnRefCon);

	[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (MidiUmpEndpoint), Name = "MIDIUMPMutableEndpoint")]
	[DisableDefaultCtor]
	interface MidiUmpMutableEndpoint {
		[Export ("mutableFunctionBlocks", ArgumentSemantic.Copy)]
		MidiUmpMutableFunctionBlock [] MutableFunctionBlocks { get; set; }

		[Export ("isEnabled")]
		bool IsEnabled { get; }

		[Export ("initWithName:deviceInfo:productInstanceID:MIDIProtocol:destinationCallback:")]
		NativeHandle Constructor (string name, Midi2DeviceInfo deviceInfo, string productInstanceId, MidiProtocolId midiProtocol, MidiReceiveBlock destinationCallback);

		[Export ("setName:error:")]
		bool SetName (string name, out NSError error);

		[Export ("registerFunctionBlocks:markAsStatic:error:")]
		bool RegisterFunctionBlocks (MidiUmpMutableFunctionBlock [] functionBlocks, bool markAsStatic, out NSError error);

		[Export ("setEnabled:error:")]
		bool SetEnabled (bool isEnabled, out NSError error);
	}

	[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (MidiUmpFunctionBlock), Name = "MIDIUMPMutableFunctionBlock")]
	[DisableDefaultCtor]
	interface MidiUmpMutableFunctionBlock {
		[Export ("UMPEndpoint", ArgumentSemantic.Weak), NullAllowed]
		MidiUmpMutableEndpoint UmpEndpoint { get; }

		[Export ("initWithName:direction:firstGroup:totalGroupsSpanned:maxSysEx8Streams:MIDI1Info:UIHint:isEnabled:")]
		NativeHandle Constructor (string name, MidiUmpFunctionBlockDirection direction, MidiUmpGroupNumber firstGroup, byte /* MIDIUInteger7 */ totalGroupsSpanned, byte /* MIDIUInteger7 */ maxSysEx8Streams, MidiUmpFunctionBlockMidi1Info midi1Info, MidiUmpFunctionBlockUIHint uiHint, bool isEnabled);

		[Export ("setEnabled:error:")]
		bool SetEnabled (bool isEnabled, out NSError error);

		[Export ("setName:error:")]
		bool SetName (string name, out NSError error);

		[Export ("reconfigureWithFirstGroup:direction:MIDI1Info:UIHint:error:")]
		bool ReconfigureWithFirstGroup (MidiUmpGroupNumber firstGroup, MidiUmpFunctionBlockDirection direction, MidiUmpFunctionBlockMidi1Info midi1Info, MidiUmpFunctionBlockUIHint uiHint, out NSError error);
	}
}
