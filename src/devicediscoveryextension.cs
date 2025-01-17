//
// DeviceDiscoveryExtension C# bindings
//
// Authors:
//   Israel Soto (issoto@microsoft.com)
//   Rolf Bjarne Kvinge (rolf@xamarin.com)
//
// Copyright 2022, 2024 Microsoft Corporation.
//

using System;
using ObjCRuntime;
using Foundation;
using UniformTypeIdentifiers;

using nw_endpoint_t = System.IntPtr;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace DeviceDiscoveryExtension {

	[Static]
	[Mac (15, 0), iOS (16, 0), MacCatalyst (18, 0), NoTV]
	interface DDDeviceProtocolStrings {
		[Field ("DDDeviceProtocolStringInvalid")]
		NSString Invalid { get; }

		[Field ("DDDeviceProtocolStringDIAL")]
		NSString Dial { get; }
	}

	[Mac (15, 0), iOS (16, 0), MacCatalyst (18, 0), NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface DDDevice {
		[Export ("initWithDisplayName:category:protocolType:identifier:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string displayName, DDDeviceCategory category, UTType protocolType, string identifier);

		[NullAllowed, Export ("bluetoothIdentifier", ArgumentSemantic.Strong)]
		NSUuid BluetoothIdentifier { get; set; }

		[Export ("category", ArgumentSemantic.Assign)]
		DDDeviceCategory Category { get; set; }

		[Export ("displayName")]
		string DisplayName { get; set; }

		[Export ("identifier")]
		string Identifier { get; set; }

		[Internal]
		[Export ("networkEndpoint", ArgumentSemantic.Strong)]
		nw_endpoint_t _NetworkEndpoint { get; set; }

		[Export ("protocol", ArgumentSemantic.Assign)]
		DDDeviceProtocol Protocol { get; set; }

		[Export ("protocolType", ArgumentSemantic.Strong)]
		UTType ProtocolType { get; set; }

		[Export ("state", ArgumentSemantic.Assign)]
		DDDeviceState State { get; set; }

		[NullAllowed, Export ("txtRecordData", ArgumentSemantic.Copy)]
		NSData TxtRecordData { get; set; }

		[Export ("url", ArgumentSemantic.Copy)]
		NSUrl Url { get; set; }

		[Export ("mediaPlaybackState", ArgumentSemantic.Assign)]
		DDDeviceMediaPlaybackState MediaPlaybackState { get; set; }

		[NullAllowed, Export ("mediaContentTitle")]
		string MediaContentTitle { get; set; }

		[NullAllowed, Export ("mediaContentSubtitle")]
		string MediaContentSubtitle { get; set; }

		[iOS (17, 0)]
		[Export ("supportsGrouping")]
		bool SupportsGrouping { get; set; }

		[iOS (18, 0)]
		[Export ("deviceSupports", ArgumentSemantic.Assign)]
		DDDeviceSupports DeviceSupports { get; set; }

		[iOS (18, 0)]
		[NullAllowed]
		[Export ("displayImageName", ArgumentSemantic.Copy)]
		string DisplayImageName { get; set; }

		[iOS (18, 0)]
		[NullAllowed]
		[Export ("SSID", ArgumentSemantic.Copy)]
		string Ssid { get; set; }
	}

	[Mac (15, 0), iOS (16, 0), MacCatalyst (18, 0), NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface DDDeviceEvent {
		[Export ("initWithEventType:device:")]
		NativeHandle Constructor (DDEventType type, DDDevice device);

		[Export ("device", ArgumentSemantic.Strong)]
		DDDevice Device { get; }

		[Export ("eventType", ArgumentSemantic.Assign)]
		DDEventType EventType { get; }
	}

	[Mac (15, 0), iOS (16, 0), MacCatalyst (18, 0), NoTV]
	[BaseType (typeof (NSObject))]
	interface DDDiscoverySession {
		[Export ("reportEvent:")]
		void ReportEvent (DDDeviceEvent inEvent);
	}
}
