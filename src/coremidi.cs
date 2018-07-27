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

namespace CoreMidi {
	
#if !MONOMAC
	[BaseType (typeof (NSObject), Name="MIDINetworkHost")]
	interface MidiNetworkHost {
		[Export ("name", ArgumentSemantic.Retain)]
		string Name { get;  }

		[Export ("address", ArgumentSemantic.Retain)]
		string Address { get;  }

		[Export ("port")]
		nint Port { get;  }

		[Export ("netServiceName", ArgumentSemantic.Retain)]
		string NetServiceName { get;  }

		[Export ("netServiceDomain", ArgumentSemantic.Retain)]
		string NetServiceDomain { get;  }

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

	[BaseType (typeof (NSObject), Name="MIDINetworkConnection")]
	interface MidiNetworkConnection {
		[Export ("host", ArgumentSemantic.Retain)]
		MidiNetworkHost Host { get;  }

		[Static, Export ("connectionWithHost:")]
		MidiNetworkConnection FromHost (MidiNetworkHost host);
	}

	[BaseType (typeof (NSObject), Name="MIDINetworkSession")]
	// default 'init' crash the application
	[DisableDefaultCtor]
	interface MidiNetworkSession {
		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set;  }

		[Export ("networkPort")]
		nint NetworkPort { get;  }

		[Export ("networkName")]
		string NetworkName { get;  }

		[Export ("localName")]
		string LocalName { get;  }

		[Export ("connectionPolicy")]
		MidiNetworkConnectionPolicy ConnectionPolicy { get; set;  }

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

		[Export ("sourceEndpoint")] [Internal]
		int /* MIDIObjectRef = UInt32 */ _SourceEndpoint { get; }

		[Wrap ("new MidiEndpoint (_SourceEndpoint)")]
		MidiEndpoint SourceEndpoint { get; }

		[Export ("destinationEndpoint")] [Internal]
		int /* MIDIObjectRef = UInt32 */ _DestinationEndpoint { get; }

		[Wrap ("new MidiEndpoint (_DestinationEndpoint)")]
		MidiEndpoint DestinationEndPoint { get; }
	}
#endif

	[NoWatch, NoTV, Mac (10,14, onlyOn64: true), iOS (12,0)]
	[BaseType (typeof(NSObject), Name="MIDICIProfile")]
	[DisableDefaultCtor]
	interface MidiCIProfile : NSSecureCoding
	{
		[Export ("name")]
		string Name { get; }

		[Export ("profileID")]
		NSData ProfileId { get; }

		[Export ("initWithData:name:")]
		IntPtr Constructor (NSData data, string inName);
	}

	[NoWatch, NoTV, Mac (10,14, onlyOn64: true), iOS (12,0)]
	[BaseType (typeof(NSObject), Name="MIDICIProfileState")]
	[DisableDefaultCtor]
	interface MidiCIProfileState : NSSecureCoding
	{
		[Export ("enabledProfiles")]
		MidiCIProfile[] EnabledProfiles { get; }

		[Export ("disabledProfiles")]
		MidiCIProfile[] DisabledProfiles { get; }

		[Export ("initWithEnabledProfiles:disabledProfiles:")]
		IntPtr Constructor (MidiCIProfile[] enabled, MidiCIProfile[] disabled);
	}

	delegate void MidiCIProfileChangedHandler (MidiCISession session, byte channel, MidiCIProfile profile, bool enabled);
	delegate void MidiCIPropertyResponseHandler (MidiCISession session, byte channel, NSData response, NSError error);
	delegate void MidiCIPropertyChangedHandler (MidiCISession session, byte channel, NSData data);

	[NoWatch, NoTV, Mac (10,14, onlyOn64: true), iOS (12,0)]
	[BaseType (typeof(NSObject), Name="MIDICISession")]
	[DisableDefaultCtor]
	interface MidiCISession
	{
		[Export ("initWithMIDIEntity:dataReadyHandler:")]
		IntPtr Constructor (uint entity, Action handler);

		[Export ("entity")]
		uint Entity { get; }

		[Export ("supportsProfileCapability")]
		bool SupportsProfileCapability { get; }

		[Export ("supportsPropertyCapability")]
		bool SupportsPropertyCapability { get; }

		[Export ("deviceIdentification")]
		MidiCIDeviceIdentification DeviceIdentification { get; }

		[Export ("profileStateForChannel:")]
		MidiCIProfileState GetProfileState (byte channel);

		[Export ("enableProfile:onChannel:error:")]
		bool EnableProfile (MidiCIProfile profile, byte channel, [NullAllowed] out NSError outError);

		[Export ("disableProfile:onChannel:error:")]
		bool DisableProfile (MidiCIProfile profile, byte channel, [NullAllowed] out NSError outError);

		[NullAllowed, Export ("profileChangedCallback", ArgumentSemantic.Assign)]
		MidiCIProfileChangedHandler ProfileChangedCallback { get; set; }

		[Export ("hasProperty:onChannel:responseHandler:")]
		void HasProperty (NSData inquiry, byte channel, MidiCIPropertyResponseHandler handler);

		[Export ("getProperty:onChannel:responseHandler:")]
		void GetProperty (NSData inquiry, byte channel, MidiCIPropertyResponseHandler handler);

		[Export ("setProperty:onChannel:responseHandler:")]
		void SetProperty (NSData inquiry, byte channel, MidiCIPropertyResponseHandler handler);

		[NullAllowed, Export ("propertyChangedCallback", ArgumentSemantic.Assign)]
		MidiCIPropertyChangedHandler PropertyChangedCallback { get; set; }
	}
}
