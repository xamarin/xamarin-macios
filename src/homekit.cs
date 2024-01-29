using CoreGraphics;
using CoreLocation;
using ObjCRuntime;
using Foundation;
using UIKit;
using System;
using System.ComponentModel;

#if !NET
using NativeHandle = System.IntPtr;
#endif

#if WATCH
interface UIView {}
#endif

namespace HomeKit {

	[MacCatalyst (14, 0)]
	[Static]
	partial interface HMErrors {
		[Field ("HMErrorDomain")]
		NSString HMErrorDomain { get; }
	}

	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject), Delegates = new string [] { "WeakDelegate" }, Events = new Type [] { typeof (HMHomeManagerDelegate) })]
	partial interface HMHomeManager {

		[Export ("delegate", ArgumentSemantic.Weak)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		IHMHomeManagerDelegate Delegate { get; set; }

		[Deprecated (PlatformName.MacOSX, 13, 0, message: "No longer supported.")]
		[Deprecated (PlatformName.iOS, 16, 1, message: "No longer supported.")]
		[Deprecated (PlatformName.MacCatalyst, 16, 1, message: "No longer supported.")]
		[Deprecated (PlatformName.WatchOS, 9, 1, message: "No longer supported.")]
		[Deprecated (PlatformName.TvOS, 16, 1, message: "No longer supported.")]
		[NullAllowed, Export ("primaryHome", ArgumentSemantic.Retain)]
		HMHome PrimaryHome { get; }

		[Export ("homes", ArgumentSemantic.Copy)]
		HMHome [] Homes { get; }

		[NoTV]
		[NoWatch]
		[Deprecated (PlatformName.MacOSX, 13, 0, message: "No longer supported.")]
		[Deprecated (PlatformName.iOS, 16, 1, message: "No longer supported.")]
		[Deprecated (PlatformName.MacCatalyst, 16, 1, message: "No longer supported.")]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("updatePrimaryHome:completionHandler:")]
		void UpdatePrimaryHome (HMHome home, Action<NSError> completion);

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("addHomeWithName:completionHandler:")]
		void AddHome (string homeName, Action<HMHome, NSError> completion);

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("removeHome:completionHandler:")]
		void RemoveHome (HMHome home, Action<NSError> completion);

		[iOS (13, 0), Watch (6, 0), TV (13, 0), NoMac]
		[MacCatalyst (14, 0)]
		[Export ("authorizationStatus")]
		HMHomeManagerAuthorizationStatus AuthorizationStatus { get; }
	}

	interface IHMHomeManagerDelegate { }

	[MacCatalyst (14, 0)]
	[Model, Protocol]
	[BaseType (typeof (NSObject))]
	partial interface HMHomeManagerDelegate {

		[Export ("homeManagerDidUpdateHomes:")]
		void DidUpdateHomes (HMHomeManager manager);

		[Export ("homeManagerDidUpdatePrimaryHome:")]
		void DidUpdatePrimaryHome (HMHomeManager manager);

		[Export ("homeManager:didAddHome:"), EventArgs ("HMHomeManager")]
		void DidAddHome (HMHomeManager manager, HMHome home);

		[Export ("homeManager:didRemoveHome:"), EventArgs ("HMHomeManager")]
		void DidRemoveHome (HMHomeManager manager, HMHome home);

		[iOS (13, 0), NoWatch, NoTV, NoMac]
		[NoMacCatalyst]
		[Deprecated (PlatformName.iOS, 15, 0, message: "This method is no longer supported.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "This method is no longer supported.")]
		[Export ("homeManager:didReceiveAddAccessoryRequest:"), EventArgs ("HMHomeManagerAddAccessoryRequest")]
		void DidReceiveAddAccessoryRequest (HMHomeManager manager, HMAddAccessoryRequest request);

		[iOS (13, 0), Watch (6, 0), TV (13, 0), NoMac]
		[MacCatalyst (14, 0)]
		[Export ("homeManager:didUpdateAuthorizationStatus:"), EventArgs ("HMHomeManagerAuthorizationStatus")]
		void DidUpdateAuthorizationStatus (HMHomeManager manager, HMHomeManagerAuthorizationStatus status);
	}

	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject), Delegates = new string [] { "WeakDelegate" }, Events = new Type [] { typeof (HMAccessoryDelegate) })]
	partial interface HMAccessory {

		[Export ("name")]
		string Name { get; }

		[NoTV]
		[NoWatch]
		[Deprecated (PlatformName.iOS, 9, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("identifier", ArgumentSemantic.Copy)]
		NSUuid Identifier { get; }

		[MacCatalyst (14, 0)]
		[Export ("uniqueIdentifier", ArgumentSemantic.Copy)]
		NSUuid UniqueIdentifier { get; }

		[Export ("delegate", ArgumentSemantic.Weak)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		IHMAccessoryDelegate Delegate { get; set; }

		[Export ("reachable")]
		bool Reachable { [Bind ("isReachable")] get; }

		[Export ("bridged")]
		bool Bridged { [Bind ("isBridged")] get; }

		[NoTV]
		[NoWatch]
		[Deprecated (PlatformName.iOS, 9, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("identifiersForBridgedAccessories", ArgumentSemantic.Copy)]
		NSUuid [] IdentifiersForBridgedAccessories { get; }

		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("uniqueIdentifiersForBridgedAccessories", ArgumentSemantic.Copy)]
		NSUuid [] UniqueIdentifiersForBridgedAccessories { get; }

		[Export ("room", ArgumentSemantic.Weak)]
		HMRoom Room { get; }

		[Export ("services", ArgumentSemantic.Copy)]
		HMService [] Services { get; }

		[MacCatalyst (14, 0)]
		[Export ("profiles", ArgumentSemantic.Copy)]
		HMAccessoryProfile [] Profiles { get; }

		[Export ("blocked")]
		bool Blocked { [Bind ("isBlocked")] get; }

		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("model")]
		string Model { get; }

		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("manufacturer")]
		string Manufacturer { get; }

		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("firmwareVersion")]
		string FirmwareVersion { get; }

		[NullAllowed]
		[Mac (13, 0), iOS (16, 1), MacCatalyst (16, 2), Watch (9, 1), TV (16, 1)]
		[Export ("matterNodeID", ArgumentSemantic.Copy)]
		NSNumber MatterNodeId { get; }

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("updateName:completionHandler:")]
		void UpdateName (string name, Action<NSError> completion);

		[Async]
		[Export ("identifyWithCompletionHandler:")]
		void Identify (Action<NSError> completion);

		[MacCatalyst (14, 0)]
		[Export ("category", ArgumentSemantic.Strong)]
		HMAccessoryCategory Category { get; }

		// HMAccessory(Camera)

		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("cameraProfiles", ArgumentSemantic.Copy)]
		HMCameraProfile [] CameraProfiles { get; }

		[Watch (4, 3)]
		[MacCatalyst (14, 0)]
		[Export ("supportsIdentify")]
		bool SupportsIdentify { get; }
	}

	interface IHMAccessoryDelegate { }

	[MacCatalyst (14, 0)]
	[Model, Protocol]
	[BaseType (typeof (NSObject))]
	partial interface HMAccessoryDelegate {

		[Export ("accessoryDidUpdateName:")]
		void DidUpdateName (HMAccessory accessory);

		[Export ("accessory:didUpdateNameForService:"), EventArgs ("HMAccessoryUpdate")]
		void DidUpdateNameForService (HMAccessory accessory, HMService service);

		[Export ("accessory:didUpdateAssociatedServiceTypeForService:"), EventArgs ("HMAccessoryUpdate")]
		void DidUpdateAssociatedServiceType (HMAccessory accessory, HMService service);

		[Export ("accessoryDidUpdateServices:")]
		void DidUpdateServices (HMAccessory accessory);

		[MacCatalyst (14, 0)]
		[Export ("accessory:didAddProfile:"), EventArgs ("HMAccessoryProfile")]
		void DidAddProfile (HMAccessory accessory, HMAccessoryProfile profile);

		[MacCatalyst (14, 0)]
		[Export ("accessory:didRemoveProfile:"), EventArgs ("HMAccessoryProfile")]
		void DidRemoveProfile (HMAccessory accessory, HMAccessoryProfile profile);

		[Export ("accessoryDidUpdateReachability:")]
		void DidUpdateReachability (HMAccessory accessory);

		[Export ("accessory:service:didUpdateValueForCharacteristic:"), EventArgs ("HMAccessoryServiceUpdateCharacteristic")]
		void DidUpdateValueForCharacteristic (HMAccessory accessory, HMService service, HMCharacteristic characteristic);

		[MacCatalyst (14, 0)]
		[Export ("accessory:didUpdateFirmwareVersion:"), EventArgs ("HMAccessoryFirmwareVersion")]
		void DidUpdateFirmwareVersion (HMAccessory accessory, string firmwareVersion);
	}

#if !WATCH
	// __WATCHOS_PROHIBITED
	[NoTV]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject), Delegates = new string [] { "WeakDelegate" }, Events = new Type [] { typeof (HMAccessoryBrowserDelegate) })]
	partial interface HMAccessoryBrowser {

		[Export ("delegate", ArgumentSemantic.Weak)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		IHMAccessoryBrowserDelegate Delegate { get; set; }

		[Export ("discoveredAccessories", ArgumentSemantic.Copy)]
		HMAccessory [] DiscoveredAccessories { get; }

		[Export ("startSearchingForNewAccessories")]
		void StartSearchingForNewAccessories ();

		[Export ("stopSearchingForNewAccessories")]
		void StopSearchingForNewAccessories ();
	}

	interface IHMAccessoryBrowserDelegate { }

	[NoTV]
	[NoMacCatalyst]
	[Model, Protocol]
	[BaseType (typeof (NSObject))]
	partial interface HMAccessoryBrowserDelegate {

		[Export ("accessoryBrowser:didFindNewAccessory:"), EventArgs ("HMAccessoryBrowser")]
		void DidFindNewAccessory (HMAccessoryBrowser browser, HMAccessory accessory);

		[Export ("accessoryBrowser:didRemoveNewAccessory:"), EventArgs ("HMAccessoryBrowser")]
		void DidRemoveNewAccessory (HMAccessoryBrowser browser, HMAccessory accessory);
	}
#endif // !WATCH

	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface HMAccessoryProfile {
		[Export ("uniqueIdentifier", ArgumentSemantic.Copy)]
		NSUuid UniqueIdentifier { get; }

		[Export ("services", ArgumentSemantic.Strong)]
		HMService [] Services { get; }

		[NullAllowed, Export ("accessory", ArgumentSemantic.Weak)]
		HMAccessory Accessory { get; }
	}

	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	partial interface HMAction {

		[MacCatalyst (14, 0)]
		[Export ("uniqueIdentifier", ArgumentSemantic.Copy)]
		NSUuid UniqueIdentifier { get; }

		[Deprecated (PlatformName.iOS, 16, 4, message: "Use subclasses instead.")]
		[Deprecated (PlatformName.TvOS, 16, 4, message: "Use subclasses instead.")]
		[Deprecated (PlatformName.MacCatalyst, 16, 4, message: "Use subclasses instead.")]
		[Export ("init")]
		NativeHandle Constructor ();
	}

	[MacCatalyst (14, 0)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	partial interface HMActionSet {

		[Export ("name")]
		string Name { get; }

		[Export ("actions", ArgumentSemantic.Copy)]
		NSSet Actions { get; }

		[Export ("executing")]
		bool Executing { [Bind ("isExecuting")] get; }

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("updateName:completionHandler:")]
		void UpdateName (string name, Action<NSError> completion);

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("addAction:completionHandler:")]
		void AddAction (HMAction action, Action<NSError> completion);

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("removeAction:completionHandler:")]
		void RemoveAction (HMAction action, Action<NSError> completion);

		[Internal]
		[MacCatalyst (14, 0)]
		[Export ("actionSetType")]
		NSString _ActionSetType { get; }

		[MacCatalyst (14, 0)]
		[Export ("uniqueIdentifier", ArgumentSemantic.Copy)]
		NSUuid UniqueIdentifier { get; }

		[MacCatalyst (14, 0)]
		[NullAllowed]
		[Export ("lastExecutionDate", ArgumentSemantic.Copy)]
		NSDate LastExecutionDate { get; }
	}

	[MacCatalyst (14, 0)]
	[Static]
	[Internal]
	interface HMActionSetTypesInternal {
		[Field ("HMActionSetTypeWakeUp")]
		NSString WakeUp { get; }

		[Field ("HMActionSetTypeSleep")]
		NSString Sleep { get; }

		[Field ("HMActionSetTypeHomeDeparture")]
		NSString HomeDeparture { get; }

		[Field ("HMActionSetTypeHomeArrival")]
		NSString HomeArrival { get; }

		[Field ("HMActionSetTypeUserDefined")]
		NSString UserDefined { get; }

		[MacCatalyst (14, 0)]
		[Field ("HMActionSetTypeTriggerOwned")]
		NSString TriggerOwned { get; }
	}

	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	partial interface HMCharacteristic {

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("characteristicType", ArgumentSemantic.Copy)]
		NSString WeakCharacteristicType { get; }

		[Wrap ("HMCharacteristicTypeExtensions.GetValue (WeakCharacteristicType)")]
		HMCharacteristicType CharacteristicType { get; }

		[Export ("service", ArgumentSemantic.Weak)]
		HMService Service { get; }

		[Export ("properties", ArgumentSemantic.Copy)]
		NSString [] Properties { get; }

		[NullAllowed, Export ("metadata", ArgumentSemantic.Retain)]
		HMCharacteristicMetadata Metadata { get; }

		[NullAllowed, Export ("value", ArgumentSemantic.Copy)]
		NSObject Value { get; }

		[Export ("notificationEnabled")]
		bool NotificationEnabled { [Bind ("isNotificationEnabled")] get; }

		[Async]
		[Export ("writeValue:completionHandler:")]
		void WriteValue (NSObject value, Action<NSError> completion);

		[Async]
		[Export ("readValueWithCompletionHandler:")]
		void ReadValue (Action<NSError> completion);

		[Async]
		[Export ("enableNotification:completionHandler:")]
		void EnableNotification (bool enable, Action<NSError> completion);

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("updateAuthorizationData:completionHandler:")]
		void UpdateAuthorizationData ([NullAllowed] NSData data, Action<NSError> completion);

		[MacCatalyst (14, 0)]
		[Export ("localizedDescription")]
		string LocalizedDescription { get; }

		[MacCatalyst (14, 0)]
		[Export ("uniqueIdentifier", ArgumentSemantic.Copy)]
		NSUuid UniqueIdentifier { get; }

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicKeyPath")]
		NSString KeyPath { get; }

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicValueKeyPath")]
		NSString ValueKeyPath { get; }
	}

	[MacCatalyst (14, 0)]
	[Static]
	[Internal]
	interface HMCharacteristicPropertyInternal {

		[Field ("HMCharacteristicPropertyReadable")]
		NSString Readable { get; }

		[Field ("HMCharacteristicPropertyWritable")]
		NSString Writable { get; }

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicPropertyHidden")]
		NSString Hidden { get; }

		[Notification]
		[Field ("HMCharacteristicPropertySupportsEventNotification")]
		NSString SupportsEventNotification { get; }
	}

	[MacCatalyst (14, 0)]
	[Static]
	[Internal]
	interface HMCharacteristicMetadataUnitsInternal {
		[Field ("HMCharacteristicMetadataUnitsCelsius")]
		NSString Celsius { get; }

		[Field ("HMCharacteristicMetadataUnitsFahrenheit")]
		NSString Fahrenheit { get; }

		[Field ("HMCharacteristicMetadataUnitsPercentage")]
		NSString Percentage { get; }

		[Field ("HMCharacteristicMetadataUnitsArcDegree")]
		NSString ArcDegree { get; }

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicMetadataUnitsSeconds")]
		NSString Seconds { get; }

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicMetadataUnitsLux")]
		NSString Lux { get; }

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicMetadataUnitsPartsPerMillion")]
		NSString PartsPerMillion { get; }

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicMetadataUnitsMicrogramsPerCubicMeter")]
		NSString MicrogramsPerCubicMeter { get; }
	}

	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	partial interface HMCharacteristicMetadata {

		[NullAllowed, Export ("minimumValue")]
		NSNumber MinimumValue { get; }

		[NullAllowed, Export ("maximumValue")]
		NSNumber MaximumValue { get; }

		[NullAllowed, Export ("stepValue")]
		NSNumber StepValue { get; }

		[NullAllowed, Export ("maxLength")]
		NSNumber MaxLength { get; }

		[Internal]
		[NullAllowed, Export ("format", ArgumentSemantic.Copy)]
		NSString _Format { get; }

		[Internal]
		[NullAllowed, Export ("units", ArgumentSemantic.Copy)]
		NSString _Units { get; }

		[NullAllowed, Export ("manufacturerDescription")]
		string ManufacturerDescription { get; }

		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("validValues", ArgumentSemantic.Copy)]
		NSNumber [] ValidValues { get; }
	}

	[MacCatalyst (14, 0)]
	[DisableDefaultCtor]
	[BaseType (typeof (HMAction))]
	partial interface HMCharacteristicWriteAction {

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("initWithCharacteristic:targetValue:")]
#if XAMCORE_3_0
		NativeHandle Constructor (HMCharacteristic characteristic, INSCopying targetValue);
#else
		NativeHandle Constructor (HMCharacteristic characteristic, NSObject targetValue);
#endif

		[Export ("characteristic", ArgumentSemantic.Retain)]
		HMCharacteristic Characteristic { get; }

		[Export ("targetValue", ArgumentSemantic.Copy)]
#if XAMCORE_3_0
		INSCopying TargetValue { get; }
#else
		NSObject TargetValue { get; }
#endif

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("updateTargetValue:completionHandler:")]
#if XAMCORE_3_0
		void UpdateTargetValue (INSCopying targetValue, Action<NSError> completion);
#else
		void UpdateTargetValue (NSObject targetValue, Action<NSError> completion);
#endif
	}

	[MacCatalyst (14, 0)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject), Delegates = new string [] { "WeakDelegate" }, Events = new Type [] { typeof (HMHomeDelegate) })]
	partial interface HMHome {

		[Export ("delegate", ArgumentSemantic.Weak)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		IHMHomeDelegate Delegate { get; set; }

		[Export ("name")]
		string Name { get; }

		[Export ("primary")]
		bool Primary { [Bind ("isPrimary")] get; }

		[MacCatalyst (14, 0)]
		[Export ("homeHubState")]
		HMHomeHubState HomeHubState { get; }

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("updateName:completionHandler:")]
		void UpdateName (string name, Action<NSError> completion);

		[MacCatalyst (14, 0)]
		[Export ("uniqueIdentifier", ArgumentSemantic.Copy)]
		NSUuid UniqueIdentifier { get; }

		// HMHome(HMAccessory)

		[Export ("accessories", ArgumentSemantic.Copy)]
		HMAccessory [] Accessories { get; }

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("addAccessory:completionHandler:")]
		void AddAccessory (HMAccessory accessory, Action<NSError> completion);

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("removeAccessory:completionHandler:")]
		void RemoveAccessory (HMAccessory accessory, Action<NSError> completion);

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("assignAccessory:toRoom:completionHandler:")]
		void AssignAccessory (HMAccessory accessory, HMRoom room, Action<NSError> completion);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("servicesWithTypes:")]
		[return: NullAllowed]
		HMService [] GetServices (NSString [] serviceTypes);

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("unblockAccessory:completionHandler:")]
		void UnblockAccessory (HMAccessory accessory, Action<NSError> completion);

		[Deprecated (PlatformName.iOS, 15, 4, message: "Use 'HMAccessorySetupManager.PerformAccessorySetup' instead.")]
		[NoWatch, NoTV]
		[NoMacCatalyst]
		[Deprecated (PlatformName.MacCatalyst, 15, 4, message: "Use 'HMAccessorySetupManager.PerformAccessorySetup' instead.")]
		[Async]
		[Export ("addAndSetupAccessoriesWithCompletionHandler:")]
		void AddAndSetupAccessories (Action<NSError> completion);

		[Deprecated (PlatformName.iOS, 15, 4, message: "Use 'HMAccessorySetupManager.PerformAccessorySetup' instead.")]
		[NoWatch, NoTV, NoMacCatalyst]
		[Deprecated (PlatformName.MacCatalyst, 15, 4, message: "Use 'HMAccessorySetupManager.PerformAccessorySetup' instead.")]
		[Async]
		[Export ("addAndSetupAccessoriesWithPayload:completionHandler:")]
		void AddAndSetupAccessories (HMAccessorySetupPayload payload, Action<HMAccessory [], NSError> completion);

		// HMHome(HMRoom)

		[Export ("rooms", ArgumentSemantic.Copy)]
		HMRoom [] Rooms { get; }

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("addRoomWithName:completionHandler:")]
		void AddRoom (string roomName, Action<HMRoom, NSError> completion);

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("removeRoom:completionHandler:")]
		void RemoveRoom (HMRoom room, Action<NSError> completion);

		[Export ("roomForEntireHome")]
		HMRoom GetRoomForEntireHome ();

		// HMHome(HMZone)

		[Export ("zones", ArgumentSemantic.Copy)]
		HMZone [] Zones { get; }

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("addZoneWithName:completionHandler:")]
		void AddZone (string zoneName, Action<HMZone, NSError> completion);

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("removeZone:completionHandler:")]
		void RemoveZone (HMZone zone, Action<NSError> completion);

		// HMHome(HMServiceGroup)

		[Export ("serviceGroups", ArgumentSemantic.Copy)]
		HMServiceGroup [] ServiceGroups { get; }

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("addServiceGroupWithName:completionHandler:")]
		void AddServiceGroup (string serviceGroupName, Action<HMServiceGroup, NSError> completion);

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("removeServiceGroup:completionHandler:")]
		void RemoveServiceGroup (HMServiceGroup group, Action<NSError> completion);

		// HMHome(HMActionSet)

		[Export ("actionSets", ArgumentSemantic.Copy)]
		HMActionSet [] ActionSets { get; }

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("addActionSetWithName:completionHandler:")]
		void AddActionSet (string actionSetName, Action<HMActionSet, NSError> completion);

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("removeActionSet:completionHandler:")]
		void RemoveActionSet (HMActionSet actionSet, Action<NSError> completion);

		[Async]
		[Export ("executeActionSet:completionHandler:")]
		void ExecuteActionSet (HMActionSet actionSet, Action<NSError> completion);

		[MacCatalyst (14, 0)]
		[Export ("builtinActionSetOfType:")]
		[return: NullAllowed]
		HMActionSet GetBuiltinActionSet (string actionSetType);

		// HMHome(HMTrigger)

		[Export ("triggers", ArgumentSemantic.Copy)]
		HMTrigger [] Triggers { get; }

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("addTrigger:completionHandler:")]
		void AddTrigger (HMTrigger trigger, Action<NSError> completion);

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("removeTrigger:completionHandler:")]
		void RemoveTrigger (HMTrigger trigger, Action<NSError> completion);

		// HMHome(HMUser)

		[NoTV]
		[NoWatch]
		[Deprecated (PlatformName.iOS, 9, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("users")]
		HMUser [] Users { get; }

		[NoTV]
		[NoWatch]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'ManageUsers' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ManageUsers' instead.")]
		[Async]
		[Export ("addUserWithCompletionHandler:")]
		void AddUser (Action<HMUser, NSError> completion);

		[MacCatalyst (14, 0)]
		[Export ("currentUser", ArgumentSemantic.Strong)]
		HMUser CurrentUser { get; }

		[NoTV]
		[NoWatch]
		[MacCatalyst (14, 0)]
		[Async]
		[Export ("manageUsersWithCompletionHandler:")]
		void ManageUsers (Action<NSError> completion);

		[MacCatalyst (14, 0)]
		[Export ("homeAccessControlForUser:")]
		HMHomeAccessControl GetHomeAccessControl (HMUser user);

		// @interface Matter (HMHome)
		[TV (16, 1), iOS (16, 1), MacCatalyst (16, 1), Watch (9, 1)]
		[Export ("matterControllerID")]
		string MatterControllerId { get; }

		[TV (16, 1), iOS (16, 1), MacCatalyst (16, 1), Watch (9, 1)]
		[Export ("matterControllerXPCConnectBlock", ArgumentSemantic.Strong)]
		Func<NSXpcConnection> MatterControllerXPCConnectBlock { get; }

		// constants

		[MacCatalyst (14, 0)]
		[Field ("HMUserFailedAccessoriesKey")]
		NSString UserFailedAccessoriesKey { get; }

		[Watch (6, 1), TV (13, 2), iOS (13, 2)]
		[MacCatalyst (14, 0)]
		[Export ("supportsAddingNetworkRouter")]
		bool SupportsAddingNetworkRouter { get; }
	}

	interface IHMHomeDelegate { }

	[MacCatalyst (14, 0)]
	[Model, Protocol]
	[BaseType (typeof (NSObject))]
	partial interface HMHomeDelegate {

		[Export ("homeDidUpdateName:")]
		void DidUpdateNameForHome (HMHome home);

		[MacCatalyst (14, 0)]
		[Export ("homeDidUpdateAccessControlForCurrentUser:")]
		void DidUpdateAccessControlForCurrentUser (HMHome home);

		[Export ("home:didAddAccessory:"), EventArgs ("HMHomeAccessory")]
		void DidAddAccessory (HMHome home, HMAccessory accessory);

		[Export ("home:didRemoveAccessory:"), EventArgs ("HMHomeAccessory")]
		void DidRemoveAccessory (HMHome home, HMAccessory accessory);

		[Export ("home:didAddUser:"), EventArgs ("HMHomeUser")]
		void DidAddUser (HMHome home, HMUser user);

		[Export ("home:didRemoveUser:"), EventArgs ("HMHomeUser")]
		void DidRemoveUser (HMHome home, HMUser user);

		[Export ("home:didUpdateRoom:forAccessory:"), EventArgs ("HMHomeRoomAccessory")]
		void DidUpdateRoom (HMHome home, HMRoom room, HMAccessory accessory);

		[Export ("home:didAddRoom:"), EventArgs ("HMHomeRoom")]
		void DidAddRoom (HMHome home, HMRoom room);

		[Export ("home:didRemoveRoom:"), EventArgs ("HMHomeRoom")]
		void DidRemoveRoom (HMHome home, HMRoom room);

		[Export ("home:didUpdateNameForRoom:"), EventArgs ("HMHomeRoom")]
		void DidUpdateNameForRoom (HMHome home, HMRoom room);

		[Export ("home:didAddZone:"), EventArgs ("HMHomeZone")]
		void DidAddZone (HMHome home, HMZone zone);

		[Export ("home:didRemoveZone:"), EventArgs ("HMHomeZone")]
		void DidRemoveZone (HMHome home, HMZone zone);

		[Export ("home:didUpdateNameForZone:"), EventArgs ("HMHomeZone")]
		void DidUpdateNameForZone (HMHome home, HMZone zone);

		[Export ("home:didAddRoom:toZone:"), EventArgs ("HMHomeRoomZone")]
		void DidAddRoomToZone (HMHome home, HMRoom room, HMZone zone);

		[Export ("home:didRemoveRoom:fromZone:"), EventArgs ("HMHomeRoomZone")]
		void DidRemoveRoomFromZone (HMHome home, HMRoom room, HMZone zone);

		[Export ("home:didAddServiceGroup:"), EventArgs ("HMHomeServiceGroup")]
		void DidAddServiceGroup (HMHome home, HMServiceGroup group);

		[Export ("home:didRemoveServiceGroup:"), EventArgs ("HMHomeServiceGroup")]
		void DidRemoveServiceGroup (HMHome home, HMServiceGroup group);

		[Export ("home:didUpdateNameForServiceGroup:"), EventArgs ("HMHomeServiceGroup")]
		void DidUpdateNameForServiceGroup (HMHome home, HMServiceGroup group);

		[Export ("home:didAddService:toServiceGroup:"), EventArgs ("HMHomeServiceServiceGroup")]
		void DidAddService (HMHome home, HMService service, HMServiceGroup group);

		[Export ("home:didRemoveService:fromServiceGroup:"), EventArgs ("HMHomeServiceServiceGroup")]
		void DidRemoveService (HMHome home, HMService service, HMServiceGroup group);

		[Export ("home:didAddActionSet:"), EventArgs ("HMHomeActionSet")]
		void DidAddActionSet (HMHome home, HMActionSet actionSet);

		[Export ("home:didRemoveActionSet:"), EventArgs ("HMHomeActionSet")]
		void DidRemoveActionSet (HMHome home, HMActionSet actionSet);

		[Export ("home:didUpdateNameForActionSet:"), EventArgs ("HMHomeActionSet")]
		void DidUpdateNameForActionSet (HMHome home, HMActionSet actionSet);

		[Export ("home:didUpdateActionsForActionSet:"), EventArgs ("HMHomeActionSet")]
		void DidUpdateActionsForActionSet (HMHome home, HMActionSet actionSet);

		[Export ("home:didAddTrigger:"), EventArgs ("HMHomeTrigger")]
		void DidAddTrigger (HMHome home, HMTrigger trigger);

		[Export ("home:didRemoveTrigger:"), EventArgs ("HMHomeTrigger")]
		void DidRemoveTrigger (HMHome home, HMTrigger trigger);

		[Export ("home:didUpdateNameForTrigger:"), EventArgs ("HMHomeTrigger")]
		void DidUpdateNameForTrigger (HMHome home, HMTrigger trigger);

		[Export ("home:didUpdateTrigger:"), EventArgs ("HMHomeTrigger")]
		void DidUpdateTrigger (HMHome home, HMTrigger trigger);

		[Export ("home:didUnblockAccessory:"), EventArgs ("HMHomeAccessory")]
		void DidUnblockAccessory (HMHome home, HMAccessory accessory);

		[Export ("home:didEncounterError:forAccessory:"), EventArgs ("HMHomeErrorAccessory")]
		void DidEncounterError (HMHome home, NSError error, HMAccessory accessory);

		[MacCatalyst (14, 0)]
		[Export ("home:didUpdateHomeHubState:"), EventArgs ("HMHomeHubState")]
		void DidUpdateHomeHubState (HMHome home, HMHomeHubState homeHubState);

		[Watch (6, 1), TV (13, 2), iOS (13, 2)]
		[MacCatalyst (14, 0)]
		[Export ("homeDidUpdateSupportedFeatures:")]
		void DidUpdateSupportedFeatures (HMHome home);
	}

	[MacCatalyst (14, 0)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	partial interface HMRoom {

		[Export ("name")]
		string Name { get; }

		[Export ("accessories", ArgumentSemantic.Copy)]
		HMAccessory [] Accessories { get; }

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("updateName:completionHandler:")]
		void UpdateName (string name, Action<NSError> completion);

		[MacCatalyst (14, 0)]
		[Export ("uniqueIdentifier", ArgumentSemantic.Copy)]
		NSUuid UniqueIdentifier { get; }
	}

	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	partial interface HMService {

		[Export ("accessory", ArgumentSemantic.Weak)]
		HMAccessory Accessory { get; }

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("serviceType", ArgumentSemantic.Copy)]
		NSString WeakServiceType { get; }

		[Wrap ("HMServiceTypeExtensions.GetValue (WeakServiceType)")]
		HMServiceType ServiceType { get; }

		[Export ("name")]
		string Name { get; }

		[NullAllowed, Export ("associatedServiceType")]
		string AssociatedServiceType { get; }

		[Export ("characteristics", ArgumentSemantic.Copy)]
		HMCharacteristic [] Characteristics { get; }

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("updateName:completionHandler:")]
		void UpdateName (string name, Action<NSError> completion);

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Async]
		[Export ("updateAssociatedServiceType:completionHandler:")]
		void UpdateAssociatedServiceType ([NullAllowed] string serviceType, Action<NSError> completion);

		[MacCatalyst (14, 0)]
		[Export ("userInteractive")]
		bool UserInteractive { [Bind ("isUserInteractive")] get; }

		[MacCatalyst (14, 0)]
		[Export ("localizedDescription")]
		string LocalizedDescription { get; }

		[MacCatalyst (14, 0)]
		[Export ("uniqueIdentifier", ArgumentSemantic.Copy)]
		NSUuid UniqueIdentifier { get; }

		[MacCatalyst (14, 0)]
		[Export ("primaryService")]
		bool PrimaryService { [Bind ("isPrimaryService")] get; }

		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("linkedServices", ArgumentSemantic.Copy)]
		HMService [] LinkedServices { get; }
	}

	[MacCatalyst (14, 0)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	partial interface HMServiceGroup {

		[Export ("name")]
		string Name { get; }

		[Export ("services", ArgumentSemantic.Copy)]
		HMService [] Services { get; }

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("updateName:completionHandler:")]
		void UpdateName (string name, Action<NSError> completion);

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("addService:completionHandler:")]
		void AddService (HMService service, Action<NSError> completion);

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("removeService:completionHandler:")]
		void RemoveService (HMService service, Action<NSError> completion);

		[MacCatalyst (14, 0)]
		[Export ("uniqueIdentifier", ArgumentSemantic.Copy)]
		NSUuid UniqueIdentifier { get; }
	}

	[MacCatalyst (14, 0)]
	[DisableDefaultCtor]
	[BaseType (typeof (HMTrigger))]
	partial interface HMTimerTrigger {

		[Watch (9, 4), TV (16, 4), MacCatalyst (16, 4), iOS (16, 4)]
		[Export ("initWithName:fireDate:recurrence:")]
		NativeHandle Constructor (string name, NSDate fireDate, [NullAllowed] NSDateComponents recurrence);

		[Deprecated (PlatformName.iOS, 16, 4, message: "Use '.ctor (string, NSDate, NSDateComponents' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 16, 4, message: "Use '.ctor (string, NSDate, NSDateComponents' instead.")]
		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("initWithName:fireDate:timeZone:recurrence:recurrenceCalendar:")]
		NativeHandle Constructor (string name, NSDate fireDate, [NullAllowed] NSTimeZone timeZone, [NullAllowed] NSDateComponents recurrence, [NullAllowed] NSCalendar recurrenceCalendar);

		[Export ("fireDate", ArgumentSemantic.Copy)]
		NSDate FireDate { get; }

		[Deprecated (PlatformName.iOS, 16, 4, message: "Use 'HMEventTrigger' with 'HMCalendarEvent' for triggers based on a time-zone-relative time of day.")]
		[Deprecated (PlatformName.MacCatalyst, 16, 4, message: "Use 'HMEventTrigger' with 'HMCalendarEvent' for triggers based on a time-zone-relative time of day.")]
		[Deprecated (PlatformName.TvOS, 16, 4, message: "Use 'HMEventTrigger' with 'HMCalendarEvent' for triggers based on a time-zone-relative time of day.")]
		[Deprecated (PlatformName.WatchOS, 9, 4, message: "Use 'HMEventTrigger' with 'HMCalendarEvent' for triggers based on a time-zone-relative time of day.")]
		[NullAllowed, Export ("timeZone", ArgumentSemantic.Copy)]
		NSTimeZone TimeZone { get; }

		[NullAllowed, Export ("recurrence", ArgumentSemantic.Copy)]
		NSDateComponents Recurrence { get; }

		[Deprecated (PlatformName.iOS, 16, 4, message: "No longer supported.")]
		[Deprecated (PlatformName.MacCatalyst, 16, 4, message: "No longer supported.")]
		[Deprecated (PlatformName.TvOS, 16, 4, message: "No longer supported.")]
		[Deprecated (PlatformName.WatchOS, 9, 4, message: "No longer supported.")]
		[NullAllowed, Export ("recurrenceCalendar", ArgumentSemantic.Copy)]
		NSCalendar RecurrenceCalendar { get; }

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("updateFireDate:completionHandler:")]
		void UpdateFireDate (NSDate fireDate, Action<NSError> completion);

		[Deprecated (PlatformName.iOS, 16, 4, message: "Use 'HMEventTrigger' with 'HMCalendarEvent' for triggers based on a time-zone-relative time of day.")]
		[Deprecated (PlatformName.MacCatalyst, 16, 4, message: "Use 'HMEventTrigger' with 'HMCalendarEvent' for triggers based on a time-zone-relative time of day.")]
		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("updateTimeZone:completionHandler:")]
		void UpdateTimeZone ([NullAllowed] NSTimeZone timeZone, Action<NSError> completion);

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("updateRecurrence:completionHandler:")]
		void UpdateRecurrence ([NullAllowed] NSDateComponents recurrence, Action<NSError> completion);
	}

	[MacCatalyst (14, 0)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	partial interface HMTrigger {

		[Export ("name")]
		string Name { get; }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; }

		[Export ("actionSets", ArgumentSemantic.Copy)]
		HMActionSet [] ActionSets { get; }

		[Deprecated (PlatformName.MacOSX, 14, 0, message: "No longer supported.")]
		[Deprecated (PlatformName.iOS, 17, 0, message: "No longer supported.")]
		[Deprecated (PlatformName.WatchOS, 10, 0, message: "No longer supported.")]
		[Deprecated (PlatformName.TvOS, 17, 0, message: "No longer supported.")]
		[Deprecated (PlatformName.MacCatalyst, 17, 0, message: "No longer supported.")]
		[NullAllowed, Export ("lastFireDate", ArgumentSemantic.Copy)]
		NSDate LastFireDate { get; }

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("updateName:completionHandler:")]
		void UpdateName (string name, Action<NSError> completion);

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("addActionSet:completionHandler:")]
		void AddActionSet (HMActionSet actionSet, Action<NSError> completion);

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("removeActionSet:completionHandler:")]
		void RemoveActionSet (HMActionSet actionSet, Action<NSError> completion);

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("enable:completionHandler:")]
		void Enable (bool enable, Action<NSError> completion);

		[MacCatalyst (14, 0)]
		[Export ("uniqueIdentifier", ArgumentSemantic.Copy)]
		NSUuid UniqueIdentifier { get; }
	}

	[MacCatalyst (14, 0)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	partial interface HMZone {

		[Export ("name")]
		string Name { get; }

		[Export ("rooms", ArgumentSemantic.Copy)]
		HMRoom [] Rooms { get; }

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("updateName:completionHandler:")]
		void UpdateName (string name, Action<NSError> completion);

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("addRoom:completionHandler:")]
		void AddRoom (HMRoom room, Action<NSError> completion);

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("removeRoom:completionHandler:")]
		void RemoveRoom (HMRoom room, Action<NSError> completion);

		[MacCatalyst (14, 0)]
		[Export ("uniqueIdentifier", ArgumentSemantic.Copy)]
		NSUuid UniqueIdentifier { get; }
	}

	[Static, Internal]
	[MacCatalyst (14, 0)]
	interface HMCharacteristicMetadataFormatKeys {
		[Field ("HMCharacteristicMetadataFormatBool")]
		NSString _Bool { get; }

		[Field ("HMCharacteristicMetadataFormatInt")]
		NSString _Int { get; }

		[Field ("HMCharacteristicMetadataFormatFloat")]
		NSString _Float { get; }

		[Field ("HMCharacteristicMetadataFormatString")]
		NSString _String { get; }

		[Field ("HMCharacteristicMetadataFormatArray")]
		NSString _Array { get; }

		[Field ("HMCharacteristicMetadataFormatDictionary")]
		NSString _Dictionary { get; }

		[Field ("HMCharacteristicMetadataFormatUInt8")]
		NSString _UInt8 { get; }

		[Field ("HMCharacteristicMetadataFormatUInt16")]
		NSString _UInt16 { get; }

		[Field ("HMCharacteristicMetadataFormatUInt32")]
		NSString _UInt32 { get; }

		[Field ("HMCharacteristicMetadataFormatUInt64")]
		NSString _UInt64 { get; }

		[Field ("HMCharacteristicMetadataFormatData")]
		NSString _Data { get; }

		[Field ("HMCharacteristicMetadataFormatTLV8")]
		NSString _Tlv8 { get; }
	}

	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface HMUser {
		[Export ("name")]
		string Name { get; }

		[MacCatalyst (14, 0)]
		[Export ("uniqueIdentifier", ArgumentSemantic.Copy)]
		NSUuid UniqueIdentifier { get; }
	}

	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NSInternalInconsistencyException Reason: init is unavailable
	interface HMAccessoryCategory {
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("categoryType")]
		NSString WeakCategoryType { get; }

		[Wrap ("HMAccessoryCategoryTypeExtensions.GetValue (WeakCategoryType)")]
		HMAccessoryCategoryType CategoryType { get; }

		[Export ("localizedDescription")]
		string LocalizedDescription { get; }
	}

	[MacCatalyst (14, 0)]
	[BaseType (typeof (HMEvent))]
	[DisableDefaultCtor]
	interface HMCharacteristicEvent : NSMutableCopying {
		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("initWithCharacteristic:triggerValue:")]
		NativeHandle Constructor (HMCharacteristic characteristic, [NullAllowed] INSCopying triggerValue);

		[Export ("characteristic", ArgumentSemantic.Strong)]
		HMCharacteristic Characteristic { get; [NotImplemented] set; }

		[NullAllowed]
		[Export ("triggerValue", ArgumentSemantic.Copy)]
		INSCopying TriggerValue { get; [NotImplemented] set; }

		[Deprecated (PlatformName.iOS, 11, 0)]
		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Async]
		[Export ("updateTriggerValue:completionHandler:")]
		void UpdateTriggerValue ([NullAllowed] INSCopying triggerValue, Action<NSError> completion);
	}

	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface HMEvent {
		[Export ("uniqueIdentifier", ArgumentSemantic.Copy)]
		NSUuid UniqueIdentifier { get; }

		[MacCatalyst (14, 0)]
		[Static]
		[Export ("isSupportedForHome:")]
		bool IsSupported (HMHome home);

		[Deprecated (PlatformName.iOS, 16, 4, message: "Use subclasses instead.")]
		[Deprecated (PlatformName.TvOS, 16, 4, message: "Use subclasses instead.")]
		[Deprecated (PlatformName.MacCatalyst, 16, 4, message: "Use subclasses instead.")]
		[Export ("init")]
		NativeHandle Constructor ();
	}

	[MacCatalyst (14, 0)]
	[BaseType (typeof (HMEvent))]
	interface HMTimeEvent { }

	[MacCatalyst (14, 0)]
	[BaseType (typeof (HMTrigger))]
	[DisableDefaultCtor]
	interface HMEventTrigger {
		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("initWithName:events:predicate:")]
		NativeHandle Constructor (string name, HMEvent [] events, [NullAllowed] NSPredicate predicate);

		[NoTV]
		[NoWatch]
		[MacCatalyst (14, 0)]
		[Export ("initWithName:events:endEvents:recurrences:predicate:")]
		NativeHandle Constructor (string name, HMEvent [] events, [NullAllowed] HMEvent [] endEvents, [NullAllowed] NSDateComponents [] recurrences, [NullAllowed] NSPredicate predicate);

		[Export ("events", ArgumentSemantic.Copy)]
		HMEvent [] Events { get; }

		[MacCatalyst (14, 0)]
		[Export ("endEvents", ArgumentSemantic.Copy)]
		HMEvent [] EndEvents { get; }

		[NullAllowed, Export ("predicate", ArgumentSemantic.Copy)]
		NSPredicate Predicate { get; }

		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("recurrences", ArgumentSemantic.Copy)]
		NSDateComponents [] Recurrences { get; }

		[MacCatalyst (14, 0)]
		[Export ("executeOnce")]
		bool ExecuteOnce { get; }

		[MacCatalyst (14, 0)]
		[Export ("triggerActivationState", ArgumentSemantic.Assign)]
		HMEventTriggerActivationState TriggerActivationState { get; }

		[Static]
		[Internal]
		[Export ("predicateForEvaluatingTriggerOccurringBeforeSignificantEvent:applyingOffset:")]
		NSPredicate CreatePredicateForEvaluatingTriggerOccurringBeforeSignificantEvent (NSString significantEvent, [NullAllowed] NSDateComponents offset);

		[MacCatalyst (14, 0)]
		[Static]
		[Export ("predicateForEvaluatingTriggerOccurringBeforeSignificantEvent:")]
		NSPredicate CreatePredicateForEvaluatingTriggerOccurringBeforeSignificantEvent (HMSignificantTimeEvent significantEvent);

		[Static]
		[Internal]
		[Export ("predicateForEvaluatingTriggerOccurringAfterSignificantEvent:applyingOffset:")]
		NSPredicate CreatePredicateForEvaluatingTriggerOccurringAfterSignificantEvent (NSString significantEvent, [NullAllowed] NSDateComponents offset);

		[MacCatalyst (14, 0)]
		[Static]
		[Export ("predicateForEvaluatingTriggerOccurringAfterSignificantEvent:")]
		NSPredicate CreatePredicateForEvaluatingTriggerOccurringAfterSignificantEvent (HMSignificantTimeEvent significantEvent);

		[MacCatalyst (14, 0)]
		[Static]
		[Export ("predicateForEvaluatingTriggerOccurringBetweenSignificantEvent:secondSignificantEvent:")]
		NSPredicate CreatePredicateForEvaluatingTriggerOccurringBetweenSignificantEvent (HMSignificantTimeEvent firstSignificantEvent, HMSignificantTimeEvent secondSignificantEvent);

		[Static]
		[Export ("predicateForEvaluatingTriggerOccurringBeforeDateWithComponents:")]
		NSPredicate CreatePredicateForEvaluatingTriggerOccurringBeforeDate (NSDateComponents dateComponents);

		[Static]
		[Export ("predicateForEvaluatingTriggerOccurringOnDateWithComponents:")]
		NSPredicate CreatePredicateForEvaluatingTriggerOccurringOnDate (NSDateComponents dateComponents);

		[Static]
		[Export ("predicateForEvaluatingTriggerOccurringAfterDateWithComponents:")]
		NSPredicate CreatePredicateForEvaluatingTriggerOccurringAfterDate (NSDateComponents dateComponents);

		[MacCatalyst (14, 0)]
		[Static]
		[Export ("predicateForEvaluatingTriggerOccurringBetweenDateWithComponents:secondDateWithComponents:")]
		NSPredicate CreatePredicateForEvaluatingTriggerOccurringBetweenDates (NSDateComponents firstDateComponents, NSDateComponents secondDateComponents);

		[Static]
		[Export ("predicateForEvaluatingTriggerWithCharacteristic:relatedBy:toValue:")]
		NSPredicate CreatePredicateForEvaluatingTrigger (HMCharacteristic characteristic, NSPredicateOperatorType operatorType, NSObject value);

		[MacCatalyst (14, 0)]
		[Static]
		[Export ("predicateForEvaluatingTriggerWithPresence:")]
		NSPredicate CreatePredicateForEvaluatingTrigger (HMPresenceEvent presenceEvent);

		[NoTV]
		[NoWatch]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'UpdateEvents' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UpdateEvents' instead.")]
		[Async]
		[Export ("addEvent:completionHandler:")]
		void AddEvent (HMEvent @event, Action<NSError> completion);

		[NoTV]
		[NoWatch]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'UpdateEvents' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UpdateEvents' instead.")]
		[Async]
		[Export ("removeEvent:completionHandler:")]
		void RemoveEvent (HMEvent @event, Action<NSError> completion);

		[NoTV]
		[NoWatch]
		[MacCatalyst (14, 0)]
		[Async]
		[Export ("updateEvents:completionHandler:")]
		void UpdateEvents (HMEvent [] events, Action<NSError> completion);

		[NoTV]
		[NoWatch]
		[MacCatalyst (14, 0)]
		[Async]
		[Export ("updateEndEvents:completionHandler:")]
		void UpdateEndEvents (HMEvent [] endEvents, Action<NSError> completion);

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("updatePredicate:completionHandler:")]
		void UpdatePredicate ([NullAllowed] NSPredicate predicate, Action<NSError> completion);

		[NoTV]
		[NoWatch]
		[MacCatalyst (14, 0)]
		[Async]
		[Export ("updateRecurrences:completionHandler:")]
		void UpdateRecurrences ([NullAllowed] NSDateComponents [] recurrences, Action<NSError> completion);

		[NoTV]
		[NoWatch]
		[MacCatalyst (14, 0)]
		[Async]
		[Export ("updateExecuteOnce:completionHandler:")]
		void UpdateExecuteOnce (bool executeOnce, Action<NSError> completion);
	}

	[MacCatalyst (14, 0)]
	[BaseType (typeof (HMAccessControl))]
	[DisableDefaultCtor]
	interface HMHomeAccessControl {
		[Export ("administrator")]
		bool Administrator { [Bind ("isAdministrator")] get; }
	}

	[MacCatalyst (14, 0)]
	[BaseType (typeof (HMEvent))]
	[DisableDefaultCtor]
	interface HMLocationEvent : NSMutableCopying {
		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("initWithRegion:")]
		NativeHandle Constructor (CLRegion region);

		[NullAllowed, Export ("region", ArgumentSemantic.Strong)]
		CLRegion Region { get; [NotImplemented] set; }

		[NoTV]
		[NoWatch]
		[Deprecated (PlatformName.iOS, 11, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Async]
		[Export ("updateRegion:completionHandler:")]
		void UpdateRegion (CLRegion region, Action<NSError> completion);
	}

	[MacCatalyst (14, 0)]
	[BaseType (typeof (HMLocationEvent))]
	[DisableDefaultCtor]
	interface HMMutableLocationEvent {

		[Export ("initWithRegion:")]
		NativeHandle Constructor (CLRegion region);

		[Override]
		[NullAllowed, Export ("region", ArgumentSemantic.Strong)]
		CLRegion Region { get; set; }
	}

	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (UIView))]
	interface HMCameraView {
		// inlined ctor
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[NullAllowed, Export ("cameraSource", ArgumentSemantic.Strong)]
		HMCameraSource CameraSource { get; set; }
	}

	[MacCatalyst (14, 0)]
	[Abstract] // documented as such in header file
	[BaseType (typeof (NSObject))]
	interface HMCameraSource {

		[Watch (7, 4), TV (14, 5), Mac (11, 3), iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("aspectRatio")]
		double AspectRatio { get; }
	}

	[MacCatalyst (14, 0)]
	[BaseType (typeof (HMAccessoryProfile))]
	[DisableDefaultCtor]
	interface HMCameraProfile {
		[NullAllowed, Export ("streamControl", ArgumentSemantic.Strong)]
		HMCameraStreamControl StreamControl { get; }

		[NullAllowed, Export ("snapshotControl", ArgumentSemantic.Strong)]
		HMCameraSnapshotControl SnapshotControl { get; }

		[NullAllowed, Export ("settingsControl", ArgumentSemantic.Strong)]
		HMCameraSettingsControl SettingsControl { get; }

		[NullAllowed, Export ("speakerControl", ArgumentSemantic.Strong)]
		HMCameraAudioControl SpeakerControl { get; }

		[NullAllowed, Export ("microphoneControl", ArgumentSemantic.Strong)]
		HMCameraAudioControl MicrophoneControl { get; }
	}

	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	interface HMCameraControl { }

	[MacCatalyst (14, 0)]
	[BaseType (typeof (HMCameraControl))]
	interface HMCameraStreamControl {
		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		IHMCameraStreamControlDelegate Delegate { get; set; }

		[Export ("streamState", ArgumentSemantic.Assign)]
		HMCameraStreamState StreamState { get; }

		[NullAllowed, Export ("cameraStream", ArgumentSemantic.Strong)]
		HMCameraStream CameraStream { get; }

		[Export ("startStream")]
		void StartStream ();

		[Export ("stopStream")]
		void StopStream ();
	}

	interface IHMCameraStreamControlDelegate { }

	[MacCatalyst (14, 0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface HMCameraStreamControlDelegate {
		[Export ("cameraStreamControlDidStartStream:")]
		void DidStartStream (HMCameraStreamControl cameraStreamControl);

		[Export ("cameraStreamControl:didStopStreamWithError:")]
		void DidStopStream (HMCameraStreamControl cameraStreamControl, [NullAllowed] NSError error);
	}

	// TODO: Type still available for tvOS even if everything in it is __TVOS_PROHIBITED.
	[MacCatalyst (14, 0)]
	[BaseType (typeof (HMCameraSource))]
	interface HMCameraStream {
		[TV (14, 5)]
		[MacCatalyst (13, 1)]
		[Export ("audioStreamSetting", ArgumentSemantic.Assign)]
		HMCameraAudioStreamSetting AudioStreamSetting { get; }

		[TV (14, 5)]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("updateAudioStreamSetting:completionHandler:")]
		void UpdateAudioStreamSetting (HMCameraAudioStreamSetting audioStreamSetting, Action<NSError> completion);
	}

	[MacCatalyst (14, 0)]
	[BaseType (typeof (HMCameraControl))]
	interface HMCameraSnapshotControl {
		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		IHMCameraSnapshotControlDelegate Delegate { get; set; }

		[NullAllowed, Export ("mostRecentSnapshot", ArgumentSemantic.Strong)]
		HMCameraSnapshot MostRecentSnapshot { get; }

		[Export ("takeSnapshot")]
		void TakeSnapshot ();
	}

	interface IHMCameraSnapshotControlDelegate { }

	[MacCatalyst (14, 0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface HMCameraSnapshotControlDelegate {
		[Export ("cameraSnapshotControl:didTakeSnapshot:error:")]
		void DidTakeSnapshot (HMCameraSnapshotControl cameraSnapshotControl, [NullAllowed] HMCameraSnapshot snapshot, [NullAllowed] NSError error);

		[MacCatalyst (14, 0)]
		[Export ("cameraSnapshotControlDidUpdateMostRecentSnapshot:")]
		void DidUpdateMostRecentSnapshot (HMCameraSnapshotControl cameraSnapshotControl);
	}

	[MacCatalyst (14, 0)]
	[BaseType (typeof (HMCameraSource))]
	interface HMCameraSnapshot {
		[Export ("captureDate", ArgumentSemantic.Copy)]
		NSDate CaptureDate { get; }
	}

	[MacCatalyst (14, 0)]
	[BaseType (typeof (HMCameraControl))]
	[DisableDefaultCtor]
	interface HMCameraSettingsControl {
		[NullAllowed, Export ("nightVision", ArgumentSemantic.Strong)]
		HMCharacteristic NightVision { get; }

		[NullAllowed, Export ("currentHorizontalTilt", ArgumentSemantic.Strong)]
		HMCharacteristic CurrentHorizontalTilt { get; }

		[NullAllowed, Export ("targetHorizontalTilt", ArgumentSemantic.Strong)]
		HMCharacteristic TargetHorizontalTilt { get; }

		[NullAllowed, Export ("currentVerticalTilt", ArgumentSemantic.Strong)]
		HMCharacteristic CurrentVerticalTilt { get; }

		[NullAllowed, Export ("targetVerticalTilt", ArgumentSemantic.Strong)]
		HMCharacteristic TargetVerticalTilt { get; }

		[NullAllowed, Export ("opticalZoom", ArgumentSemantic.Strong)]
		HMCharacteristic OpticalZoom { get; }

		[NullAllowed, Export ("digitalZoom", ArgumentSemantic.Strong)]
		HMCharacteristic DigitalZoom { get; }

		[NullAllowed, Export ("imageRotation", ArgumentSemantic.Strong)]
		HMCharacteristic ImageRotation { get; }

		[NullAllowed, Export ("imageMirroring", ArgumentSemantic.Strong)]
		HMCharacteristic ImageMirroring { get; }
	}

	[MacCatalyst (14, 0)]
	[BaseType (typeof (HMCameraControl))]
	[DisableDefaultCtor]
	interface HMCameraAudioControl {
		[NullAllowed, Export ("mute", ArgumentSemantic.Strong)]
		HMCharacteristic Mute { get; }

		[NullAllowed, Export ("volume", ArgumentSemantic.Strong)]
		HMCharacteristic Volume { get; }
	}

	[MacCatalyst (14, 0)]
	[BaseType (typeof (HMTimeEvent))]
	[DisableDefaultCtor]
	interface HMCalendarEvent : NSMutableCopying {

		[Export ("initWithFireDateComponents:")]
		NativeHandle Constructor (NSDateComponents fireDateComponents);

		[Export ("fireDateComponents", ArgumentSemantic.Strong)]
		NSDateComponents FireDateComponents { get; [NotImplemented] set; }
	}

	[MacCatalyst (14, 0)]
	[BaseType (typeof (HMCalendarEvent))]
	[DisableDefaultCtor]
	interface HMMutableCalendarEvent {

		[Export ("initWithFireDateComponents:")]
		NativeHandle Constructor (NSDateComponents fireDateComponents);

		[Override]
		[Export ("fireDateComponents", ArgumentSemantic.Strong)]
		NSDateComponents FireDateComponents { get; set; }
	}

	[MacCatalyst (14, 0)]
	[BaseType (typeof (HMCharacteristicEvent))]
	[DisableDefaultCtor]
	interface HMMutableCharacteristicEvent : NSMutableCopying {

		[Export ("initWithCharacteristic:triggerValue:")]
		NativeHandle Constructor (HMCharacteristic characteristic, [NullAllowed] INSCopying triggerValue);

		[Override]
		[Export ("characteristic", ArgumentSemantic.Strong)]
		HMCharacteristic Characteristic { get; set; }

		[Override]
		[NullAllowed, Export ("triggerValue", ArgumentSemantic.Copy)]
		INSCopying TriggerValue { get; set; }
	}

	[MacCatalyst (14, 0)]
	[BaseType (typeof (HMEvent))]
	[DisableDefaultCtor]
	interface HMCharacteristicThresholdRangeEvent : NSMutableCopying {

		[Export ("initWithCharacteristic:thresholdRange:")]
		NativeHandle Constructor (HMCharacteristic characteristic, HMNumberRange thresholdRange);

		[Export ("characteristic", ArgumentSemantic.Strong)]
		HMCharacteristic Characteristic { get; [NotImplemented] set; }

		[Export ("thresholdRange", ArgumentSemantic.Copy)]
		HMNumberRange ThresholdRange { get; [NotImplemented] set; }
	}

	[MacCatalyst (14, 0)]
	[BaseType (typeof (HMCharacteristicThresholdRangeEvent))]
	[DisableDefaultCtor]
	interface HMMutableCharacteristicThresholdRangeEvent {

		[Export ("initWithCharacteristic:thresholdRange:")]
		NativeHandle Constructor (HMCharacteristic characteristic, HMNumberRange thresholdRange);

		[Override]
		[Export ("characteristic", ArgumentSemantic.Strong)]
		HMCharacteristic Characteristic { get; set; }

		[Override]
		[Export ("thresholdRange", ArgumentSemantic.Copy)]
		HMNumberRange ThresholdRange { get; set; }
	}

	[MacCatalyst (14, 0)]
	[BaseType (typeof (HMTimeEvent))]
	[DisableDefaultCtor]
	interface HMDurationEvent : NSMutableCopying {

		[Export ("initWithDuration:")]
		NativeHandle Constructor (double duration);

		[Export ("duration")]
		double Duration { get; [NotImplemented] set; }
	}

	[MacCatalyst (14, 0)]
	[BaseType (typeof (HMDurationEvent))]
	[DisableDefaultCtor]
	interface HMMutableDurationEvent {

		[Export ("initWithDuration:")]
		NativeHandle Constructor (double duration);

		[Override]
		[Export ("duration")]
		double Duration { get; set; }
	}

	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface HMNumberRange {

		[Static]
		[Export ("numberRangeWithMinValue:maxValue:")]
		HMNumberRange FromRange (NSNumber minValue, NSNumber maxValue);

		[Static]
		[Export ("numberRangeWithMinValue:")]
		HMNumberRange FromMin (NSNumber minValue);

		[Static]
		[Export ("numberRangeWithMaxValue:")]
		HMNumberRange FromMax (NSNumber maxValue);

		[NullAllowed, Export ("minValue", ArgumentSemantic.Strong)]
		NSNumber Min { get; }

		[NullAllowed, Export ("maxValue", ArgumentSemantic.Strong)]
		NSNumber Max { get; }
	}

	[iOS (13, 0), NoWatch, NoMac, NoTV, NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface HMAccessoryOwnershipToken {
		[Export ("initWithData:")]
		NativeHandle Constructor (NSData data);
	}

	[iOS (13, 0), NoWatch, NoMac, NoTV]
	[NoMacCatalyst]
	[Deprecated (PlatformName.iOS, 15, 0, message: "This class is no longer supported.")]
	[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "This class is no longer supported.")]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface HMAddAccessoryRequest {
		[Export ("home", ArgumentSemantic.Strong)]
		HMHome Home { get; }

		[Export ("accessoryCategory", ArgumentSemantic.Strong)]
		HMAccessoryCategory AccessoryCategory { get; }

		[Export ("accessoryName")]
		string AccessoryName { get; }

		[Export ("requiresSetupPayloadURL")]
		bool RequiresSetupPayloadUrl { get; }

#if false
		// marked as deprecated in tvOS headers (where the type does not exists)
		// https://github.com/xamarin/maccore/issues/1959
		[Export ("requiresOwnershipToken")]
		bool RequiresOwnershipToken { get; }
#endif

		[Export ("payloadWithOwnershipToken:")]
		[return: NullAllowed]
		HMAccessorySetupPayload GetPayload (HMAccessoryOwnershipToken ownershipToken);

		[Export ("payloadWithURL:ownershipToken:")]
		[return: NullAllowed]
		HMAccessorySetupPayload GetPayload (NSUrl setupPayloadUrl, HMAccessoryOwnershipToken ownershipToken);
	}

	[iOS (13, 0), Watch (6, 0), TV (13, 0), NoMac, MacCatalyst (14, 0)]
	[BaseType (typeof (HMAccessoryProfile))]
	[DisableDefaultCtor]
	interface HMNetworkConfigurationProfile {
		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IHMNetworkConfigurationProfileDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Export ("networkAccessRestricted")]
		bool NetworkAccessRestricted { [Bind ("isNetworkAccessRestricted")] get; }
	}

	interface IHMNetworkConfigurationProfileDelegate { }

	[Watch (6, 0), TV (13, 0), NoMac, iOS (13, 0), MacCatalyst (14, 0)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface HMNetworkConfigurationProfileDelegate {
		[Export ("profileDidUpdateNetworkAccessMode:")]
		void DidUpdateNetworkAccessMode (HMNetworkConfigurationProfile profile);
	}

	[NoWatch, NoTV, NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface HMAccessorySetupPayload {
		[Export ("initWithURL:")]
		NativeHandle Constructor ([NullAllowed] NSUrl setupPayloadUrl);

		[iOS (13, 0)]
		[Export ("initWithURL:ownershipToken:")]
		NativeHandle Constructor (NSUrl setupPayloadUrl, [NullAllowed] HMAccessoryOwnershipToken ownershipToken);
	}

	[MacCatalyst (14, 0)]
	[BaseType (typeof (HMEvent))]
	[DisableDefaultCtor]
	interface HMPresenceEvent : NSMutableCopying {

		[Export ("initWithPresenceEventType:presenceUserType:")]
		NativeHandle Constructor (HMPresenceEventType presenceEventType, HMPresenceEventUserType presenceUserType);

		[Export ("presenceEventType")]
		HMPresenceEventType PresenceEventType { get; [NotImplemented] set; }

		[Export ("presenceUserType")]
		HMPresenceEventUserType PresenceUserType { get; [NotImplemented] set; }

		[MacCatalyst (14, 0)]
		[Field ("HMPresenceKeyPath")]
		NSString KeyPath { get; }
	}

	[MacCatalyst (14, 0)]
	[BaseType (typeof (HMPresenceEvent))]
	[DisableDefaultCtor]
	interface HMMutablePresenceEvent {

		[Export ("presenceEventType", ArgumentSemantic.Assign)]
		HMPresenceEventType PresenceEventType { get; /* Radar 33883958: https://trello.com/c/TIlzWzrL*/ [NotImplemented] set; }

		[Export ("presenceUserType", ArgumentSemantic.Assign)]
		HMPresenceEventUserType PresenceUserType { get; /* Radar 33883958: https://trello.com/c/TIlzWzrL*/ [NotImplemented] set; }
	}

	[MacCatalyst (14, 0)]
	[BaseType (typeof (HMTimeEvent))]
	[DisableDefaultCtor]
	interface HMSignificantTimeEvent : NSMutableCopying {

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("initWithSignificantEvent:offset:")]
		NativeHandle Constructor (NSString significantEvent, [NullAllowed] NSDateComponents offset);

		[Wrap ("this (HMSignificantEventExtensions.GetConstant (significantEvent)!, offset)")]
		NativeHandle Constructor (HMSignificantEvent significantEvent, [NullAllowed] NSDateComponents offset);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("significantEvent", ArgumentSemantic.Strong)]
		NSString WeakSignificantEvent { get; [NotImplemented] set; }

		HMSignificantEvent SignificantEvent {
			[Wrap ("HMSignificantEventExtensions.GetValue (WeakSignificantEvent)")]
			get;
			[NotImplemented]
			set;
		}

		// subclass does not allow null
		[Export ("offset", ArgumentSemantic.Strong)]
		NSDateComponents Offset { get; [NotImplemented] set; }
	}

	[MacCatalyst (14, 0)]
	[BaseType (typeof (HMSignificantTimeEvent))]
	interface HMMutableSignificantTimeEvent {

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("initWithSignificantEvent:offset:")]
		NativeHandle Constructor (NSString significantEvent, [NullAllowed] NSDateComponents offset);

		[Wrap ("this (HMSignificantEventExtensions.GetConstant (significantEvent)!, offset)")]
		NativeHandle Constructor (HMSignificantEvent significantEvent, [NullAllowed] NSDateComponents offset);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Override]
		[Export ("significantEvent", ArgumentSemantic.Strong)]
		NSString WeakSignificantEvent { get; set; }

#if NET
		[Override]
#endif
		HMSignificantEvent SignificantEvent {
			[Wrap ("HMSignificantEventExtensions.GetValue (WeakSignificantEvent)")]
			get;
			[Wrap ("WeakSignificantEvent = HMSignificantEventExtensions.GetConstant (value)!")]
			set;
		}

		[Override]
		[Export ("offset", ArgumentSemantic.Strong)]
		NSDateComponents Offset { get; set; }
	}

	[Watch (4, 2), MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface HMAccessControl {

	}

	[NoWatch, NoTV, NoMacCatalyst, NoMac, iOS (15, 4)]
	[BaseType (typeof (NSObject))]
	interface HMAccessorySetupRequest : NSCopying {

		[NullAllowed, Export ("payload", ArgumentSemantic.Copy)]
		HMAccessorySetupPayload Payload { get; set; }

		[NullAllowed, Export ("homeUniqueIdentifier", ArgumentSemantic.Copy)]
		NSUuid HomeUniqueIdentifier { get; set; }

		[NullAllowed, Export ("suggestedRoomUniqueIdentifier", ArgumentSemantic.Copy)]
		NSUuid SuggestedRoomUniqueIdentifier { get; set; }

		[NullAllowed, Export ("suggestedAccessoryName")]
		string SuggestedAccessoryName { get; set; }
	}

	[NoWatch, NoTV, NoMacCatalyst, NoMac, iOS (15, 4)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface HMAccessorySetupResult : NSCopying {

		[Export ("homeUniqueIdentifier", ArgumentSemantic.Copy)]
		NSUuid HomeUniqueIdentifier { get; }

		[Export ("accessoryUniqueIdentifiers", ArgumentSemantic.Copy)]
		NSUuid [] AccessoryUniqueIdentifiers { get; }
	}

	[iOS (15, 2), NoWatch, NoTV, NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	interface HMAccessorySetupManager {
		[Async]
		[iOS (15, 4)]
		[Export ("performAccessorySetupUsingRequest:completionHandler:")]
		void PerformAccessorySetup (HMAccessorySetupRequest request, Action<HMAccessorySetupResult, NSError> completion);
	}

}
