//
// externalaccessory.cs: API definition for ExternalAccessory binding
//
// Copyright 2012-2014 Xamarin Inc. All rights reserved.
//

using System;
using Foundation;
using CoreFoundation;
using ObjCRuntime;
#if !MONOMAC
using UIKit;
#endif

namespace ExternalAccessory {

	[Mac (10, 13, onlyOn64: true)][TV (10,0)]
	[BaseType (typeof (NSObject), Delegates=new string [] { "WeakDelegate" }, Events=new Type [] {typeof(EAAccessoryDelegate)})]
	// Objective-C exception thrown.  Name: EAAccessoryInitException Reason: -init not supported. EAAccessoryManager is responsible for creating all objects.
	[DisableDefaultCtor]
	interface EAAccessory {
		[Export ("connected")]
		bool Connected { [Bind ("isConnected")] get; }

		[Export ("connectionID")]
		nuint ConnectionID { get; }

		[Export ("name")]
		string Name { get; }

		[Export ("manufacturer")]
		string Manufacturer { get; }

		[Export ("modelNumber")]
		string ModelNumber { get; }

		[Export ("serialNumber")]
		string SerialNumber { get; }

		[Export ("firmwareRevision")]
		string FirmwareRevision { get; }

		[Export ("hardwareRevision")]
		string HardwareRevision { get; }

		[Export ("protocolStrings")]
		string[] ProtocolStrings { get; }

		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")][NullAllowed]
		[Protocolize]
		EAAccessoryDelegate Delegate { get; set; }

		[iOS (9,0)]
		[Export ("dockType")]
		string DockType { get; }
	}

	[Mac (10, 13, onlyOn64: true)][TV (10,0)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface EAAccessoryDelegate {
		[Export ("accessoryDidDisconnect:"), EventArgs ("EAAccessory")]
		void Disconnected (EAAccessory accessory);
	}

	[Mac (10, 13, onlyOn64: true)][TV (10,0)]
	interface EAAccessoryEventArgs {
		[Export ("EAAccessoryKey")]
		EAAccessory Accessory { get; }

		[iOS (6,0)]
		[Export ("EAAccessorySelectedKey")]
		EAAccessory Selected { get; }
	}
	
	[Mac (10, 13, onlyOn64: true)][TV (10,0)]
	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: EAAccessoryManagerInitException Reason: -init is not supported. Use +sharedAccessoryManager.
	[DisableDefaultCtor]
	interface EAAccessoryManager {
		[Static][Export ("sharedAccessoryManager")]
		EAAccessoryManager SharedAccessoryManager { get ; }

		[Export ("registerForLocalNotifications")]
		void RegisterForLocalNotifications ();

		[Export ("unregisterForLocalNotifications")]
		void UnregisterForLocalNotifications ();

		[Export ("connectedAccessories")]
		EAAccessory [] ConnectedAccessories { get ; }

		[Field ("EAAccessoryDidConnectNotification")]
		[Notification (typeof (EAAccessoryEventArgs))]
		NSString DidConnectNotification { get; }

		[Field ("EAAccessoryDidDisconnectNotification")]
		[Notification (typeof (EAAccessoryEventArgs))]
		NSString DidDisconnectNotification { get; }

#if !XAMCORE_3_0 && !MONOMAC
		// now exposed with the corresponding EABluetoothAccessoryPickerError enum
		[iOS (6,0)]
		[Field ("EABluetoothAccessoryPickerErrorDomain")]
		NSString BluetoothAccessoryPickerErrorDomain { get; }
#endif

		[NoMac]
		[iOS (6,0)]
		[Export ("showBluetoothAccessoryPickerWithNameFilter:completion:")]
		[Async]
		void ShowBluetoothAccessoryPicker ([NullAllowed] NSPredicate predicate, [NullAllowed] Action<NSError> completion);
	}

	[Mac (10, 13, onlyOn64: true)][TV (10,0)]
	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: EASessionInitException Reason: -init not supported. use -initWithAccessory:forProtocol.
	[DisableDefaultCtor]
	interface EASession {
		[Export ("initWithAccessory:forProtocol:")]
		IntPtr Constructor (EAAccessory accessory, string protocol);

		[Export ("accessory")]
		EAAccessory Accessory { get; }

		[Export ("protocolString")]
		string ProtocolString { get; }

		[Export ("inputStream")]
		NSInputStream InputStream { get; }

		[Export ("outputStream")]
		NSOutputStream OutputStream { get; }
	}

	[NoMac]
	[TV (10,0)]
	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	interface EAWiFiUnconfiguredAccessory {
		[Export ("name")]
		string Name { get; }

		[Export ("manufacturer")]
		string Manufacturer { get; }

		[Export ("model")]
		string Model { get; }

		[Export ("ssid")]
		string Ssid { get; }

		[Export ("macAddress")]
		string MacAddress { get; }

		[Export ("properties")]
		EAWiFiUnconfiguredAccessoryProperties Properties { get; }		
	}


	interface IEAWiFiUnconfiguredAccessoryBrowserDelegate {}
	
	// This class is exposed for tvOS in the headers, but there's no available initializer (so it can't be constructed)
	// The API is also clearly unusable (you can list the unconfigured accessories, but you can't search for them first...)
	[NoTV] // [TV (10,0)]
	[NoMac]
	[iOS (8,0)]
#if TVOS
	[BaseType (typeof (NSObject))]
#else
	[BaseType (typeof (NSObject), Delegates=new string [] { "WeakDelegate" }, Events=new Type [] {typeof(EAWiFiUnconfiguredAccessoryBrowserDelegate)})]
#endif
#if XAMCORE_4_0
	// There's a designated initializer, which leads to think that the default ctor
	// should not be used (documentation says nothing).
	[DisableDefaultCtor]
#endif
	interface EAWiFiUnconfiguredAccessoryBrowser {

		[NoTV]
		[Export ("initWithDelegate:queue:")]
		[DesignatedInitializer] // according to header comment (but not in attributes)
		IntPtr Constructor ([NullAllowed] IEAWiFiUnconfiguredAccessoryBrowserDelegate accessoryBrowserDelegate, [NullAllowed] DispatchQueue queue);

		[NoTV] // no member is available
		[Export ("delegate", ArgumentSemantic.Weak)][NullAllowed]
		NSObject WeakDelegate { get; set; }

		[NoTV] // no member is available
		[Wrap ("WeakDelegate")][NullAllowed]
		[Protocolize]
		EAWiFiUnconfiguredAccessoryBrowserDelegate Delegate { get; set; }

		[Export ("unconfiguredAccessories", ArgumentSemantic.Copy)]
		NSSet UnconfiguredAccessories { get; }

		[NoTV]
		[Export ("startSearchingForUnconfiguredAccessoriesMatchingPredicate:")]
		void StartSearchingForUnconfiguredAccessories ([NullAllowed] NSPredicate predicate);

		[NoTV]
		[Export ("stopSearchingForUnconfiguredAccessories")]
		void StopSearchingForUnconfiguredAccessories ();

#if !MONOMAC
		[NoTV]
		[iOS (8,0)]
		[Export ("configureAccessory:withConfigurationUIOnViewController:")]
		void ConfigureAccessory (EAWiFiUnconfiguredAccessory accessory, UIViewController viewController);
#endif
	}

	[NoMac]
	[NoTV] // no member is available
	[iOS (8,0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface EAWiFiUnconfiguredAccessoryBrowserDelegate {

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("accessoryBrowser:didUpdateState:"), EventArgs ("EAWiFiUnconfiguredAccessory")]
		void DidUpdateState(EAWiFiUnconfiguredAccessoryBrowser browser, EAWiFiUnconfiguredAccessoryBrowserState state);

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("accessoryBrowser:didFindUnconfiguredAccessories:"), EventArgs ("EAWiFiUnconfiguredAccessoryBrowser")]
		void DidFindUnconfiguredAccessories (EAWiFiUnconfiguredAccessoryBrowser browser, NSSet accessories);

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("accessoryBrowser:didRemoveUnconfiguredAccessories:"), EventArgs ("EAWiFiUnconfiguredAccessoryBrowser")]
		void DidRemoveUnconfiguredAccessories (EAWiFiUnconfiguredAccessoryBrowser browser, NSSet accessories);

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("accessoryBrowser:didFinishConfiguringAccessory:withStatus:"), EventArgs ("EAWiFiUnconfiguredAccessoryDidFinish")]
		void DidFinishConfiguringAccessory (EAWiFiUnconfiguredAccessoryBrowser browser, EAWiFiUnconfiguredAccessory accessory, EAWiFiUnconfiguredAccessoryConfigurationStatus status);
	}
}
