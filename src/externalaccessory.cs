//
// externalaccessory.cs: API definition for ExternalAccessory binding
//
// Copyright 2012-2014 Xamarin Inc. All rights reserved.
//

using System;
using XamCore.Foundation;
using XamCore.CoreFoundation;
using XamCore.ObjCRuntime;
using XamCore.UIKit;

namespace XamCore.ExternalAccessory {

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

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface EAAccessoryDelegate {
		[Export ("accessoryDidDisconnect:"), EventArgs ("EAAccessory")]
		void Disconnected (EAAccessory accessory);
	}

	interface EAAccessoryEventArgs {
		[Export ("EAAccessoryKey")]
		EAAccessory Accessory { get; }

		[Since (6,0)]
		[Export ("EAAccessorySelectedKey")]
		EAAccessory Selected { get; }
	}
	
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

#if !XAMCORE_3_0
		// now exposed with the corresponding EABluetoothAccessoryPickerError enum
		[Since (6,0)]
		[Field ("EABluetoothAccessoryPickerErrorDomain")]
		NSString BluetoothAccessoryPickerErrorDomain { get; }
#endif

		[Since (6,0)]
		[Export ("showBluetoothAccessoryPickerWithNameFilter:completion:")]
		[Async]
		void ShowBluetoothAccessoryPicker ([NullAllowed] NSPredicate predicate, [NullAllowed] Action<NSError> completion);
	}

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


	public interface IEAWiFiUnconfiguredAccessoryBrowserDelegate {}
	
	[iOS (8,0)]
	[BaseType (typeof (NSObject), Delegates=new string [] { "WeakDelegate" }, Events=new Type [] {typeof(EAWiFiUnconfiguredAccessoryBrowserDelegate)})]
	interface EAWiFiUnconfiguredAccessoryBrowser {

		[DesignatedInitializer]
		[Export ("initWithDelegate:queue:")]
		IntPtr Constructor ([NullAllowed] IEAWiFiUnconfiguredAccessoryBrowserDelegate accessoryBrowserDelegate, [NullAllowed] DispatchQueue queue);

		[Export ("delegate", ArgumentSemantic.Weak)][NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")][NullAllowed]
		[Protocolize]
		EAWiFiUnconfiguredAccessoryBrowserDelegate Delegate { get; set; }

		[Export ("unconfiguredAccessories", ArgumentSemantic.Copy)]
		NSSet UnconfiguredAccessories { get; }

		[Export ("startSearchingForUnconfiguredAccessoriesMatchingPredicate:")]
		void StartSearchingForUnconfiguredAccessories ([NullAllowed] NSPredicate predicate);

		[Export ("stopSearchingForUnconfiguredAccessories")]
		void StopSearchingForUnconfiguredAccessories ();

		[iOS (8,0)]
		[Export ("configureAccessory:withConfigurationUIOnViewController:")]
		void ConfigureAccessory (EAWiFiUnconfiguredAccessory accessory, UIViewController viewController);
	}

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
