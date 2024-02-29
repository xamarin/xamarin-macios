
// This file describes the API that the generator will produce
//
// Authors:
//   Geoff Norton
//   Miguel de Icaza
//   Aaron Bockover
//
// Copyright 2009, Novell, Inc.
// Copyright 2011, 2013, 2015 Xamarin Inc.
//
using ObjCRuntime;
using Foundation;
using CloudKit;
using CoreGraphics;
using CoreFoundation;
#if !MONOMAC
using UIKit;
#endif
#if !TVOS
using Contacts;
#endif
using System;

#if TVOS
using CNPostalAddress = Foundation.NSObject;
#endif

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreLocation {

	[NoTV]
	[NoWatch]
	[MacCatalyst (13, 1)]
	[Native] // NSInteger -> CLRegion.h
	public enum CLRegionState : long {
		Unknown,
		Inside,
		Outside
	}

	[NoTV]
	[NoWatch]
	[MacCatalyst (13, 1)]
	[Native] // NSInteger -> CLRegion.h
	public enum CLProximity : long {
		Unknown,
		Immediate,
		Near,
		Far
	}

	[ErrorDomain ("CLLocationPushServiceErrorDomain")]
#if NET // Apple fixed this in Xcode 13.1
	[iOS (15,0), NoTV, NoMacCatalyst, NoMac, NoWatch]
#else
	[iOS (15, 0), NoTV, MacCatalyst (15, 0), NoMac, NoWatch]
#endif
	[Native]
	public enum CLLocationPushServiceError : long {
		Unknown = 0,
		MissingPushExtension = 1,
		MissingPushServerEnvironment = 2,
		MissingEntitlement = 3,
		UnsupportedPlatform = 4,
	}

	[Watch (10, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0), TV (17, 0)]
	[Native]
	public enum CLMonitoringState : ulong {
		Unknown,
		Satisfied,
		Unsatisfied,
		[NoWatch, Mac (14, 2), iOS (17, 2), MacCatalyst (17, 2), NoTV]
		Unmonitored,
	}

	[Watch (10, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0), TV (17, 0)]
	[Native]
	public enum CLLiveUpdateConfiguration : long {
		Default = 0,
		AutomotiveNavigation,
		OtherNavigation,
		Fitness,
		Airborne,
	}


	[NoTV]
	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // will crash, see CoreLocation.cs for compatibility stubs
	partial interface CLHeading : NSSecureCoding, NSCopying {
		[Export ("magneticHeading")]
		double MagneticHeading { get; }

		[Export ("trueHeading")]
		double TrueHeading { get; }

		[Export ("headingAccuracy")]
		double HeadingAccuracy { get; }

		[Export ("x")]
		double X { get; }

		[Export ("y")]
		double Y { get; }

		[Export ("z")]
		double Z { get; }

		[Export ("timestamp", ArgumentSemantic.Copy)]
		NSDate Timestamp { get; }
	}

	[BaseType (typeof (NSObject))]
	partial interface CLLocation : NSSecureCoding, NSCopying, CKRecordValue {
		[Export ("coordinate")]
		CLLocationCoordinate2D Coordinate { get; }

		[Export ("altitude")]
		double Altitude { get; }

		[Export ("horizontalAccuracy")]
		double HorizontalAccuracy { get; }

		[Export ("verticalAccuracy")]
		double VerticalAccuracy { get; }

		[TV (13, 0)] // API_UNAVAILABLE(tvos) removed in Xcode 11 beta 1
		[MacCatalyst (13, 1)]
		[Export ("course")]
		double Course { get; }

		[Watch (6, 2), TV (13, 4), iOS (13, 4)]
		[MacCatalyst (13, 1)]
		[Export ("courseAccuracy")]
		double CourseAccuracy { get; }

		[TV (13, 0)] // API_UNAVAILABLE(tvos) removed in Xcode 11 beta 1
		[MacCatalyst (13, 1)]
		[Export ("speed")]
		double Speed { get; }

		[Watch (6, 2), TV (13, 4), iOS (13, 4)]
		[MacCatalyst (13, 1)]
		[Export ("speedAccuracy")]
		double SpeedAccuracy { get; }

		[Export ("timestamp", ArgumentSemantic.Copy)]
		NSDate Timestamp { get; }

		[Export ("initWithLatitude:longitude:")]
		NativeHandle Constructor (double latitude, double longitude);

		[Export ("initWithCoordinate:altitude:horizontalAccuracy:verticalAccuracy:timestamp:")]
		NativeHandle Constructor (CLLocationCoordinate2D coordinate, double altitude, double hAccuracy, double vAccuracy, NSDate timestamp);

		[Export ("distanceFromLocation:")]
		double DistanceFrom (CLLocation location);

		[Export ("initWithCoordinate:altitude:horizontalAccuracy:verticalAccuracy:course:speed:timestamp:")]
		NativeHandle Constructor (CLLocationCoordinate2D coordinate, double altitude, double hAccuracy, double vAccuracy, double course, double speed, NSDate timestamp);

		[Watch (6, 2), TV (13, 4), iOS (13, 4)]
		[MacCatalyst (13, 1)]
		[Export ("initWithCoordinate:altitude:horizontalAccuracy:verticalAccuracy:course:courseAccuracy:speed:speedAccuracy:timestamp:")]
		NativeHandle Constructor (CLLocationCoordinate2D coordinate, double altitude, double hAccuracy, double vAccuracy, double course, double courseAccuracy, double speed, double speedAccuracy, NSDate timestamp);

		[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("initWithCoordinate:altitude:horizontalAccuracy:verticalAccuracy:course:courseAccuracy:speed:speedAccuracy:timestamp:sourceInfo:")]
		NativeHandle Constructor (CLLocationCoordinate2D coordinate, double altitude, double horizontalAccuracy, double verticalAccuracy, double course, double courseAccuracy, double speed, double speedAccuracy, NSDate timestamp, CLLocationSourceInformation sourceInfo);

		// Apple keep changing the 'introduction' of this field (5.0->8.0->5.0) but it was not available in 6.1
		// nor in 7.0 - but it works on my iPad3 running iOS 7.1
		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Field ("kCLErrorUserInfoAlternateRegionKey")]
		NSString ErrorUserInfoAlternateRegionKey { get; }

		[Field ("kCLLocationAccuracyBestForNavigation")]
		double AccurracyBestForNavigation { get; }

		[Field ("kCLLocationAccuracyBest")]
		double AccuracyBest { get; }

		[Field ("kCLLocationAccuracyNearestTenMeters")]
		double AccuracyNearestTenMeters { get; }

		[Field ("kCLLocationAccuracyHundredMeters")]
		double AccuracyHundredMeters { get; }

		[Field ("kCLLocationAccuracyKilometer")]
		double AccuracyKilometer { get; }

		[Field ("kCLLocationAccuracyThreeKilometers")]
		double AccuracyThreeKilometers { get; }

		[Watch (7, 0), TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("kCLLocationAccuracyReduced")]
		double AccuracyReduced { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("floor", ArgumentSemantic.Copy)]
		CLFloor Floor { get; }

		[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("ellipsoidalAltitude")]
		double EllipsoidalAltitude { get; }

		[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("sourceInformation")]
		CLLocationSourceInformation SourceInformation { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	partial interface CLFloor : NSSecureCoding, NSCopying {
		[Export ("level")]
		nint Level { get; }
	}

	delegate void RequestHistoricalLocationsCompletionHandler (CLLocation [] locations, [NullAllowed] NSError error);

	[BaseType (typeof (NSObject), Delegates = new string [] { "WeakDelegate" }, Events = new Type [] { typeof (CLLocationManagerDelegate) })]
	partial interface CLLocationManager {
		[Wrap ("WeakDelegate")]
		ICLLocationManagerDelegate Delegate { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Export ("distanceFilter", ArgumentSemantic.Assign)]
		double DistanceFilter { get; set; }

		[Export ("desiredAccuracy", ArgumentSemantic.Assign)]
		double DesiredAccuracy { get; set; }

		[NullAllowed, Export ("location", ArgumentSemantic.Copy)]
		CLLocation Location { get; }

		// __WATCHOS_PROHIBITED removed in Xcode 8.0 beta 2, assuming it's valid for 3.0+
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("startUpdatingLocation")]
		void StartUpdatingLocation ();

		[Export ("stopUpdatingLocation")]
		void StopUpdatingLocation ();

		[Export ("locationServicesEnabled"), Static]
		bool LocationServicesEnabled { get; }

		[NoTV]
		[Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Export ("headingFilter", ArgumentSemantic.Assign)]
		double HeadingFilter { get; set; }

		[NoTV]
		[Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Export ("startUpdatingHeading")]
		void StartUpdatingHeading ();

		[NoTV]
		[Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Export ("stopUpdatingHeading")]
		void StopUpdatingHeading ();

		[NoTV]
		[Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Export ("dismissHeadingCalibrationDisplay")]
		void DismissHeadingCalibrationDisplay ();

		[NoWatch]
		[NoTV]
		[Deprecated (PlatformName.iOS, 6, 0)]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Set the purpose using the NSLocationUsageDescription key in the Info.plist instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		// Default property value is null but it cannot be set to that value
		// it crash when a null is provided
		[NullAllowed, Export ("purpose")]
		string Purpose { get; set; }

		[NoTV]
		[Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Export ("headingAvailable"), Static]
		bool HeadingAvailable { get; }

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("significantLocationChangeMonitoringAvailable"), Static]
		bool SignificantLocationChangeMonitoringAvailable { get; }

		[NoWatch]
		[NoTV]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'IsMonitoringAvailable' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'IsMonitoringAvailable' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'IsMonitoringAvailable' instead.")]
		[Export ("regionMonitoringAvailable"), Static]
		bool RegionMonitoringAvailable { get; }

		[NoWatch]
		[NoTV]
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use 'IsMonitoringAvailable' and 'AuthorizationStatus' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'IsMonitoringAvailable' and 'AuthorizationStatus' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'IsMonitoringAvailable' and 'AuthorizationStatus' instead.")]
		[Export ("regionMonitoringEnabled"), Static]
		bool RegionMonitoringEnabled { get; }

		[NoTV]
		[Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Export ("headingOrientation", ArgumentSemantic.Assign)]
		CLDeviceOrientation HeadingOrientation { get; set; }

		[NoTV]
		[Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("heading", ArgumentSemantic.Copy)]
		CLHeading Heading { get; }

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("maximumRegionMonitoringDistance")]
		double MaximumRegionMonitoringDistance { get; }

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("monitoredRegions", ArgumentSemantic.Copy)]
		NSSet MonitoredRegions { get; }

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("startMonitoringSignificantLocationChanges")]
		void StartMonitoringSignificantLocationChanges ();

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("stopMonitoringSignificantLocationChanges")]
		void StopMonitoringSignificantLocationChanges ();

		[NoWatch]
		[NoTV]
		[NoMac]
		[Deprecated (PlatformName.iOS, 6, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("startMonitoringForRegion:desiredAccuracy:")]
		void StartMonitoring (CLRegion region, double desiredAccuracy);

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.iOS, 17, 0, message: "Use 'void RemoveCondition (string identifier)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 17, 0, message: "Use 'void RemoveCondition (string identifier)' instead.")]
		[Deprecated (PlatformName.MacOSX, 14, 0, message: "Use 'void RemoveCondition (string identifier)' instead.")]
		[Export ("stopMonitoringForRegion:")]
		void StopMonitoring (CLRegion region);

		[Watch (7, 0), TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("authorizationStatus")]
		CLAuthorizationStatus AuthorizationStatus { get; }

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use the instance property 'AuthorizationStatus' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use the instance 'AuthorizationStatus' property instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use the instance property AuthorizationStatus' instead.")]
		[Deprecated (PlatformName.WatchOS, 7, 0, message: "Use instance property 'AuthorizationStatus' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use the instance property 'AuthorizationStatus' instead.")]
		[Export ("authorizationStatus")]
		[Static]
		CLAuthorizationStatus Status { get; }

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.iOS, 17, 0, message: "Use 'void AddCondition (CLCondition condition, string identifier)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 17, 0, message: "Use 'void AddCondition (CLCondition condition, string identifier)' instead.")]
		[Deprecated (PlatformName.MacOSX, 14, 0, message: "Use 'void AddCondition (CLCondition condition, string identifier)' instead.")]
		[Export ("startMonitoringForRegion:")]
		void StartMonitoring (CLRegion region);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("activityType", ArgumentSemantic.Assign)]
		CLActivityType ActivityType { get; set; }

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("pausesLocationUpdatesAutomatically", ArgumentSemantic.Assign)]
		bool PausesLocationUpdatesAutomatically { get; set; }

		[NoWatch]
		[NoTV]
		[NoMac]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Not used anymore. Call will not have any effect.")]
		[NoMacCatalyst]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Not used anymore. Call will not have any effect.")]
		[Export ("allowDeferredLocationUpdatesUntilTraveled:timeout:")]
		void AllowDeferredLocationUpdatesUntil (double distance, double timeout);

		[NoWatch]
		[NoTV]
		[NoMac]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Not used anymore. Call will not have any effect.")]
		[NoMacCatalyst]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Not used anymore. Call will not have any effect.")]
		[Export ("disallowDeferredLocationUpdates")]
		void DisallowDeferredLocationUpdates ();

		[NoWatch]
		[NoTV]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Not used anymore. It will always return 'false'.")]
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Not used anymore. It will always return 'false'.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Not used anymore. It will always return 'false'.")]
		[Static]
		[Export ("deferredLocationUpdatesAvailable")]
		bool DeferredLocationUpdatesAvailable { get; }

		[MacCatalyst (13, 1)]
		[Field ("CLTimeIntervalMax")]
		double MaxTimeInterval { get; }

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Static, Export ("isMonitoringAvailableForClass:")]
		bool IsMonitoringAvailable (Class regionClass);

		[NoWatch]
		[NoTV]
		[NoMac]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'RangedBeaconConstraints' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'RangedBeaconConstraints' instead.")]
		[Export ("rangedRegions", ArgumentSemantic.Copy)]
		NSSet RangedRegions { get; }

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("rangedBeaconConstraints", ArgumentSemantic.Copy)]
		NSSet<CLBeaconIdentityConstraint> RangedBeaconConstraints { get; }

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.iOS, 17, 0, message: "Use the class 'CLMonitor' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 17, 0, message: "Use the class 'CLMonitor' instead.")]
		[Deprecated (PlatformName.MacOSX, 14, 0, message: "Use the class 'CLMonitor' instead.")]
		[Export ("requestStateForRegion:")]
		void RequestState (CLRegion region);

		[NoWatch]
		[NoTV]
		[NoMac]
		[NoMacCatalyst]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'StartRangingBeacons(CLBeaconIdentityConstraint)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'StartRangingBeacons(CLBeaconIdentityConstraint)' instead.")]
		[Export ("startRangingBeaconsInRegion:")]
		void StartRangingBeacons (CLBeaconRegion region);

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("startRangingBeaconsSatisfyingConstraint:")]
		void StartRangingBeacons (CLBeaconIdentityConstraint constraint);

		[NoWatch]
		[NoTV]
		[NoMac]
		[NoMacCatalyst]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'StopRangingBeacons(CLBeaconIdentityConstraint)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'StopRangingBeacons(CLBeaconIdentityConstraint)' instead.")]
		[Export ("stopRangingBeaconsInRegion:")]
		void StopRangingBeacons (CLBeaconRegion region);

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("stopRangingBeaconsSatisfyingConstraint:")]
		void StopRangingBeacons (CLBeaconIdentityConstraint constraint);

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("isRangingAvailable")]
		bool IsRangingAvailable { get; }

		[MacCatalyst (13, 1)]
		[Export ("requestWhenInUseAuthorization")]
		void RequestWhenInUseAuthorization ();

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("requestAlwaysAuthorization")]
		void RequestAlwaysAuthorization ();

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("startMonitoringVisits")]
		void StartMonitoringVisits ();

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("stopMonitoringVisits")]
		void StopMonitoringVisits ();

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("allowsBackgroundLocationUpdates")]
		bool AllowsBackgroundLocationUpdates { get; set; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Export ("showsBackgroundLocationIndicator")]
		bool ShowsBackgroundLocationIndicator { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("requestLocation")]
		void RequestLocation ();

		[Watch (7, 0), TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("accuracyAuthorization")]
		CLAccuracyAuthorization AccuracyAuthorization { get; }

		[Async]
		[Watch (7, 0), TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("requestTemporaryFullAccuracyAuthorizationWithPurposeKey:completion:")]
		void RequestTemporaryFullAccuracyAuthorization (string purposeKey, [NullAllowed] Action<NSError> completion);

		[Watch (7, 0), TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("requestTemporaryFullAccuracyAuthorizationWithPurposeKey:")]
		void RequestTemporaryFullAccuracyAuthorization (string purposeKey);

		[NoWatch, NoTV, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("authorizedForWidgetUpdates")]
		bool IsAuthorizedForWidgetUpdates { [Bind ("isAuthorizedForWidgetUpdates")] get; }

		[Async]
		[NoWatch, NoTV, NoMac, NoMacCatalyst]
		[iOS (15, 0)]
		[Export ("startMonitoringLocationPushesWithCompletion:")]
		void StartMonitoringLocationPushes ([NullAllowed] Action<NSData, NSError> completion);

		[NoWatch, NoTV, NoMac, NoMacCatalyst]
		[iOS (15, 0)]
		[Export ("stopMonitoringLocationPushes")]
		void StopMonitoringLocationPushes ();

		[Watch (9, 1), NoTV, NoMac, NoiOS, NoMacCatalyst]
		[Export ("requestHistoricalLocationsWithPurposeKey:sampleCount:completionHandler:")]
		void RequestHistoricalLocations (string purposeKey, nint sampleCount, RequestHistoricalLocationsCompletionHandler handler);
	}

	interface ICLLocationManagerDelegate { }

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	partial interface CLLocationManagerDelegate {
		[NoWatch]
		[NoTV]
		[Deprecated (PlatformName.iOS, 6, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("locationManager:didUpdateToLocation:fromLocation:"), EventArgs ("CLLocationUpdated")]
		void UpdatedLocation (CLLocationManager manager, CLLocation newLocation, CLLocation oldLocation);

		[NoTV]
		[Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Export ("locationManager:didUpdateHeading:"), EventArgs ("CLHeadingUpdated")]
		void UpdatedHeading (CLLocationManager manager, CLHeading newHeading);

		[NoTV]
		[Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Export ("locationManagerShouldDisplayHeadingCalibration:"), DelegateName ("CLLocationManagerEventArgs"), DefaultValue (true)]
		bool ShouldDisplayHeadingCalibration (CLLocationManager manager);

		[Export ("locationManager:didFailWithError:"), EventArgs ("NSError", true)]
		void Failed (CLLocationManager manager, NSError error);

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("locationManager:didEnterRegion:"), EventArgs ("CLRegion")]
		void RegionEntered (CLLocationManager manager, CLRegion region);

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("locationManager:didExitRegion:"), EventArgs ("CLRegion")]
		void RegionLeft (CLLocationManager manager, CLRegion region);

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("locationManager:monitoringDidFailForRegion:withError:"), EventArgs ("CLRegionError")]
		void MonitoringFailed (CLLocationManager manager, [NullAllowed] CLRegion region, NSError error);

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("locationManager:didStartMonitoringForRegion:"), EventArgs ("CLRegion")]
		void DidStartMonitoringForRegion (CLLocationManager manager, CLRegion region);

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("locationManager:didDetermineState:forRegion:"), EventArgs ("CLRegionStateDetermined")]
		void DidDetermineState (CLLocationManager manager, CLRegionState state, CLRegion region);

		[NoWatch]
		[NoTV]
		[NoMac]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'DidRangeBeaconsSatisfyingConstraint' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'DidRangeBeaconsSatisfyingConstraint' instead.")]
		[Export ("locationManager:didRangeBeacons:inRegion:"), EventArgs ("CLRegionBeaconsRanged")]
		void DidRangeBeacons (CLLocationManager manager, CLBeacon [] beacons, CLBeaconRegion region);

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("locationManager:didRangeBeacons:satisfyingConstraint:")]
		[EventArgs ("CLRegionBeaconsConstraintRanged")]
		void DidRangeBeaconsSatisfyingConstraint (CLLocationManager manager, CLBeacon [] beacons, CLBeaconIdentityConstraint beaconConstraint);

		[NoWatch]
		[NoTV]
		[NoMac]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'DidFailRangingBeacons' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'DidFailRangingBeacons' instead.")]
		[Export ("locationManager:rangingBeaconsDidFailForRegion:withError:"), EventArgs ("CLRegionBeaconsFailed")]
		void RangingBeaconsDidFailForRegion (CLLocationManager manager, CLBeaconRegion region, NSError error);

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("locationManager:didFailRangingBeaconsForConstraint:error:")]
		[EventArgs ("CLRegionBeaconsConstraintFailed")]
		void DidFailRangingBeacons (CLLocationManager manager, CLBeaconIdentityConstraint beaconConstraint, NSError error);

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("locationManager:didVisit:"), EventArgs ("CLVisited")]
		void DidVisit (CLLocationManager manager, CLVisit visit);

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'DidChangeAuthorization' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'DidChangeAuthorization' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'DidChangeAuthorization' instead.")]
		[Deprecated (PlatformName.WatchOS, 7, 0, message: "Use 'DidChangeAuthorization' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'DidChangeAuthorization' instead.")]
		[Export ("locationManager:didChangeAuthorizationStatus:"), EventArgs ("CLAuthorizationChanged")]
		void AuthorizationChanged (CLLocationManager manager, CLAuthorizationStatus status);

		[Export ("locationManager:didUpdateLocations:"), EventArgs ("CLLocationsUpdated")]
		void LocationsUpdated (CLLocationManager manager, CLLocation [] locations);

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("locationManagerDidPauseLocationUpdates:"), EventArgs ("")]
		void LocationUpdatesPaused (CLLocationManager manager);

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("locationManagerDidResumeLocationUpdates:"), EventArgs ("")]
		void LocationUpdatesResumed (CLLocationManager manager);

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("locationManager:didFinishDeferredUpdatesWithError:"), EventArgs ("NSError", true)]
		void DeferredUpdatesFinished (CLLocationManager manager, [NullAllowed] NSError error);

		[Watch (7, 0), TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("locationManagerDidChangeAuthorization:")]
		void DidChangeAuthorization (CLLocationManager manager);

	}

	[Static]
	partial interface CLLocationDistance {

		[MacCatalyst (13, 1)]
		[Field ("CLLocationDistanceMax")]
		double MaxDistance { get; }

		[Field ("kCLDistanceFilterNone")]
		double FilterNone { get; }
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // will crash, see CoreLocation.cs for compatibility stubs
	partial interface CLRegion : NSSecureCoding, NSCopying {
		[NoTV]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'CLCircularRegion' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'CLCircularRegion' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'CLCircularRegion' instead.")]
		[Export ("center")]
		CLLocationCoordinate2D Center { get; }

		[NoTV]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'CLCircularRegion' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'CLCircularRegion' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'CLCircularRegion' instead.")]
		[Export ("radius")]
		double Radius { get; }

		[Export ("identifier")]
		string Identifier { get; }

		[NoTV]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'CLCircularRegion' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'CLCircularRegion' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'CLCircularRegion' instead.")]
		[Export ("initCircularRegionWithCenter:radius:identifier:")]
		NativeHandle Constructor (CLLocationCoordinate2D center, double radius, string identifier);

		[NoTV]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'CLCircularRegion' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'CLCircularRegion' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'CLCircularRegion' instead.")]
		[Export ("containsCoordinate:")]
		bool Contains (CLLocationCoordinate2D coordinate);

		[Export ("notifyOnEntry", ArgumentSemantic.Assign)]
		[MacCatalyst (13, 1)]
		bool NotifyOnEntry { get; set; }

		[Export ("notifyOnExit", ArgumentSemantic.Assign)]
		[MacCatalyst (13, 1)]
		bool NotifyOnExit { get; set; }
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // will crash, see CoreLocation.cs for compatibility stubs
	interface CLPlacemark : NSSecureCoding, NSCopying {
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'CLPlacemark' properties to access data.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'CLPlacemark' properties to access data.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'CLPlacemark' properties to access data.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'CLPlacemark' properties to access data.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'CLPlacemark' properties to access data.")]
		[NullAllowed, Export ("addressDictionary", ArgumentSemantic.Copy)]
		NSDictionary AddressDictionary { get; }

		[NullAllowed, Export ("administrativeArea")]
		string AdministrativeArea { get; }

		[NullAllowed, Export ("subAdministrativeArea")]
		string SubAdministrativeArea { get; }

		[NullAllowed, Export ("subLocality")]
		string SubLocality { get; }

		[NullAllowed, Export ("locality")]
		string Locality { get; }

		[NullAllowed, Export ("country")]
		string Country { get; }

		[NullAllowed, Export ("postalCode")]
		string PostalCode { get; }

		[NullAllowed, Export ("thoroughfare")]
		string Thoroughfare { get; }

		[NullAllowed, Export ("subThoroughfare")]
		string SubThoroughfare { get; }

		[NullAllowed, Export ("ISOcountryCode")]
		string IsoCountryCode { get; }

		[NullAllowed, Export ("areasOfInterest")]
		string [] AreasOfInterest { get; }

		[Export ("initWithPlacemark:")]
		NativeHandle Constructor (CLPlacemark placemark);

		[NullAllowed, Export ("inlandWater")]
		string InlandWater { get; }

		[NullAllowed, Export ("location", ArgumentSemantic.Copy)]
		CLLocation Location { get; }

		[NullAllowed, Export ("name")]
		string Name { get; }

		[NullAllowed, Export ("ocean")]
		string Ocean { get; }

		[NullAllowed, Export ("region", ArgumentSemantic.Copy)]
		CLRegion Region { get; }

		[NullAllowed, Export ("timeZone")]
		[MacCatalyst (13, 1)]
		NSTimeZone TimeZone { get; }

		// From CLPlacemark (ContactsAdditions) category.
		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("postalAddress")]
		CNPostalAddress PostalAddress { get; }
	}

	[MacCatalyst (13, 1)]
	[Obsoleted (PlatformName.MacOSX, 14, 0, message: "Use 'CLCircularGeographicCondition' instead.")]
	[Obsoleted (PlatformName.iOS, 17, 0, message: "Use 'CLCircularGeographicCondition' instead.")]
	[Obsoleted (PlatformName.WatchOS, 10, 0, message: "Use 'CLCircularGeographicCondition' instead.")]
	[Obsoleted (PlatformName.TvOS, 17, 0, message: "Use 'CLCircularGeographicCondition' instead.")]
	[BaseType (typeof (CLRegion))]
#if MONOMAC
	[DisableDefaultCtor]
#endif
	partial interface CLCircularRegion {

		[Export ("initWithCenter:radius:identifier:")]
		NativeHandle Constructor (CLLocationCoordinate2D center, double radius, string identifier);

		[Export ("center")]
		CLLocationCoordinate2D Center { get; }

		[Export ("radius")]
		double Radius { get; }

		[Export ("containsCoordinate:")]
		bool ContainsCoordinate (CLLocationCoordinate2D coordinate);
	}

	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacOSX, 14, 0, message: "Use 'CLBeaconIdentityCondition' instead.")]
	[Deprecated (PlatformName.iOS, 17, 0, message: "Use 'CLBeaconIdentityCondition' instead.")]
	[Deprecated (PlatformName.MacCatalyst, 17, 0, message: "Use 'CLBeaconIdentityCondition' instead.")]
	[BaseType (typeof (CLRegion))]
	[DisableDefaultCtor] // nil-Handle on iOS8 if 'init' is used
	partial interface CLBeaconRegion {

		[NoMac]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use the 'Create' method or the constructor using 'CLBeaconIdentityConstraint' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the 'Create' method or the constructor using 'CLBeaconIdentityConstraint' instead.")]
		[Export ("initWithProximityUUID:identifier:")]
		NativeHandle Constructor (NSUuid proximityUuid, string identifier);

		[NoMac]
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Internal] // signature conflict with deprecated API
		[Export ("initWithUUID:identifier:")]
		IntPtr _Constructor (NSUuid uuid, string identifier);

		[NoMac]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use the 'Create' method or the constructor using 'CLBeaconIdentityConstraint' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the 'Create' method or the constructor using 'CLBeaconIdentityConstraint' instead.")]
		[Export ("initWithProximityUUID:major:identifier:")]
		NativeHandle Constructor (NSUuid proximityUuid, ushort major, string identifier);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Internal] // signature conflict with deprecated API
		[Export ("initWithUUID:major:identifier:")]
		IntPtr _Constructor (NSUuid uuid, ushort major, string identifier);

		[NoMac]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use the 'Create' method or the constructor using 'CLBeaconIdentityConstraint' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the 'Create' method or the constructor using 'CLBeaconIdentityConstraint' instead.")]
		[Export ("initWithProximityUUID:major:minor:identifier:")]
		NativeHandle Constructor (NSUuid proximityUuid, ushort major, ushort minor, string identifier);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Internal] // signature conflict with deprecated API
		[Export ("initWithUUID:major:minor:identifier:")]
		IntPtr _Constructor (NSUuid uuid, ushort major, ushort minor, string identifier);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("initWithBeaconIdentityConstraint:identifier:")]
		NativeHandle Constructor (CLBeaconIdentityConstraint beaconIdentityConstraint, string identifier);

		[Export ("peripheralDataWithMeasuredPower:")]
		NSMutableDictionary GetPeripheralData ([NullAllowed] NSNumber measuredPower);

		[NoMac]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'Uuid' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Uuid' instead.")]
		[Export ("proximityUUID", ArgumentSemantic.Copy)]
		NSUuid ProximityUuid { get; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("UUID", ArgumentSemantic.Copy)]
		NSUuid Uuid { get; }

		[NullAllowed, Export ("major", ArgumentSemantic.Copy)]
		NSNumber Major { get; }

		[NullAllowed, Export ("minor", ArgumentSemantic.Copy)]
		NSNumber Minor { get; }

		[Export ("notifyEntryStateOnDisplay", ArgumentSemantic.Assign)]
		bool NotifyEntryStateOnDisplay { get; set; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("beaconIdentityConstraint", ArgumentSemantic.Copy)]
		CLBeaconIdentityConstraint BeaconIdentityConstraint { get; }
	}

	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	partial interface CLBeacon : NSCopying, NSSecureCoding {

		[NoMac]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'Uuid' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Uuid' instead.")]
		[Export ("proximityUUID", ArgumentSemantic.Copy)]
		NSUuid ProximityUuid { get; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("UUID", ArgumentSemantic.Copy)]
		NSUuid Uuid { get; }

		[Export ("major", ArgumentSemantic.Copy)]
		NSNumber Major { get; }

		[Export ("minor", ArgumentSemantic.Copy)]
		NSNumber Minor { get; }

		[Export ("proximity")]
		CLProximity Proximity { get; }

		[Export ("accuracy")]
		double Accuracy { get; }

		[Export ("rssi")]
		nint Rssi { get; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("timestamp", ArgumentSemantic.Copy)]
		NSDate Timestamp { get; }
	}

	delegate void CLGeocodeCompletionHandler (CLPlacemark [] placemarks, NSError error);

	[BaseType (typeof (NSObject))]
	interface CLGeocoder {
		[Export ("isGeocoding")]
		bool Geocoding { get; }

		[Export ("reverseGeocodeLocation:completionHandler:")]
		[Async]
		void ReverseGeocodeLocation (CLLocation location, CLGeocodeCompletionHandler completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("reverseGeocodeLocation:preferredLocale:completionHandler:")]
		[Async]
		void ReverseGeocodeLocation (CLLocation location, [NullAllowed] NSLocale locale, CLGeocodeCompletionHandler completionHandler);

		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'GeocodeAddress (string, CLRegion, NSLocale, CLGeocodeCompletionHandler)' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'GeocodeAddress (string, CLRegion, NSLocale, CLGeocodeCompletionHandler)' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'GeocodeAddress (string, CLRegion, NSLocale, CLGeocodeCompletionHandler)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'GeocodeAddress (string, CLRegion, NSLocale, CLGeocodeCompletionHandler)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GeocodeAddress (string, CLRegion, NSLocale, CLGeocodeCompletionHandler)' instead.")]
		[Export ("geocodeAddressDictionary:completionHandler:")]
		[Async]
		void GeocodeAddress (NSDictionary addressDictionary, CLGeocodeCompletionHandler completionHandler);

		[Export ("geocodeAddressString:completionHandler:")]
		[Async]
		void GeocodeAddress (string addressString, CLGeocodeCompletionHandler completionHandler);

		[Export ("geocodeAddressString:inRegion:completionHandler:")]
		[Async]
		void GeocodeAddress (string addressString, [NullAllowed] CLRegion region, CLGeocodeCompletionHandler completionHandler);

		[MacCatalyst (13, 1)]
		[Async]
		[Export ("geocodeAddressString:inRegion:preferredLocale:completionHandler:")]
		void GeocodeAddress (string addressString, [NullAllowed] CLRegion region, [NullAllowed] NSLocale locale, CLGeocodeCompletionHandler completionHandler);

		[Export ("cancelGeocode")]
		void CancelGeocode ();

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("geocodePostalAddress:completionHandler:")]
		[Async]
		void GeocodePostalAddress (CNPostalAddress postalAddress, CLGeocodeCompletionHandler completionHandler);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("geocodePostalAddress:preferredLocale:completionHandler:")]
		[Async]
		void GeocodePostalAddress (CNPostalAddress postalAddress, [NullAllowed] NSLocale locale, CLGeocodeCompletionHandler completionHandler);
	}

	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CLVisit : NSSecureCoding, NSCopying {

		[Export ("arrivalDate", ArgumentSemantic.Copy)]
		NSDate ArrivalDate { get; }

		[Export ("departureDate", ArgumentSemantic.Copy)]
		NSDate DepartureDate { get; }

		[Export ("coordinate")]
		CLLocationCoordinate2D Coordinate { get; }

		[Export ("horizontalAccuracy")]
		double HorizontalAccuracy { get; }
	}

	[NoWatch, NoTV, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacOSX, 14, 0, message: "Use 'CLBeaconIdentityCondition' instead.")]
	[Deprecated (PlatformName.iOS, 17, 0, message: "Use 'CLBeaconIdentityCondition' instead.")]
	[Deprecated (PlatformName.MacCatalyst, 17, 0, message: "Use 'CLBeaconIdentityCondition' instead.")]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: -[CLBeaconIdentityConstraint init]: unrecognized selector sent to instance 0x600001930300
	interface CLBeaconIdentityConstraint : NSCopying, NSSecureCoding {

		[Export ("initWithUUID:")]
		NativeHandle Constructor (NSUuid uuid);

		[Export ("initWithUUID:major:")]
		NativeHandle Constructor (NSUuid uuid, ushort major);

		[Export ("initWithUUID:major:minor:")]
		NativeHandle Constructor (NSUuid uuid, ushort major, ushort minor);

		[Export ("UUID", ArgumentSemantic.Copy)]
		NSUuid Uuid { get; }

		[NullAllowed, Export ("major", ArgumentSemantic.Copy)]
		[BindAs (typeof (short?))]
		NSNumber Major { get; }

		[NullAllowed, Export ("minor", ArgumentSemantic.Copy)]
		[BindAs (typeof (short?))]
		NSNumber Minor { get; }
	}

#if NET // Apple fixed this in Xcode 13.1
	[iOS (15,0), NoTV, NoMacCatalyst, NoMac, NoWatch]
#else
	[iOS (15, 0), NoTV, MacCatalyst (15, 0), NoMac, NoWatch]
#endif
	[Protocol]
	interface CLLocationPushServiceExtension {
		[Abstract]
		[Export ("didReceiveLocationPushPayload:completion:")]
		void DidReceiveLocationPushPayload (NSDictionary<NSString, NSObject> payload, Action completion);

		[Export ("serviceExtensionWillTerminate")]
		void ServiceExtensionWillTerminate ();
	}

	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	interface CLLocationSourceInformation : NSCopying, NSSecureCoding {
		[Export ("initWithSoftwareSimulationState:andExternalAccessoryState:")]
		NativeHandle Constructor (bool isSoftware, bool isAccessory);

		[Export ("isSimulatedBySoftware")]
		bool IsSimulatedBySoftware { get; }

		[Export ("isProducedByAccessory")]
		bool IsProducedByAccessory { get; }
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0), TV (17, 0)]
	[BaseType (typeof (NSObject))]
	interface CLUpdate {
		[Export ("isStationary")]
		bool IsStationary { get; }

		[NullAllowed, Export ("location")]
		CLLocation Location { get; }
	}

	[NoWatch, NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CLMonitoringRecord : NSSecureCoding {
		[Export ("condition", ArgumentSemantic.Strong)]
		CLCondition Condition { get; }

		[Export ("lastEvent", ArgumentSemantic.Strong)]
		CLMonitoringEvent LastEvent { get; }
	}

	[NoWatch, NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CLMonitoringEvent : NSSecureCoding {
		[Export ("identifier", ArgumentSemantic.Strong)]
		string Identifier { get; }

		[NullAllowed, Export ("refinement", ArgumentSemantic.Strong)]
		CLCondition Refinement { get; }

		[Export ("state")]
		CLMonitoringState State { get; }

		[Export ("date", ArgumentSemantic.Strong)]
		NSDate Date { get; }
	}

	[NoWatch, NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface CLMonitorConfiguration {
		[Export ("name")]
		string Name { get; }

		[Export ("queue")]
		DispatchQueue Queue { get; }

		[Export ("eventHandler")]
		Action<CLMonitor, CLMonitoringEvent> EventHandler { get; }

		[Static]
		[Export ("configWithMonitorName:queue:eventHandler:")]
		CLMonitorConfiguration Create (string name, DispatchQueue queue, Action<CLMonitor, CLMonitoringEvent> eventHandler);
	}

	[NoWatch, NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CLMonitor {
		[Async]
		[Static]
		[Export ("requestMonitorWithConfiguration:completion:")]
		void RequestMonitor (CLMonitorConfiguration config, Action<CLMonitor> completionHandler);

		[Export ("name")]
		string Name { get; }

		[Export ("monitoredIdentifiers")]
		string [] MonitoredIdentifiers { get; }

		[Export ("addConditionForMonitoring:identifier:")]
		void AddCondition (CLCondition condition, string identifier);

		[Export ("addConditionForMonitoring:identifier:assumedState:")]
		void AddCondition (CLCondition condition, string identifier, CLMonitoringState state);

		[Export ("removeConditionFromMonitoringWithIdentifier:")]
		void RemoveCondition (string identifier);

		[Export ("monitoringRecordForIdentifier:")]
		[return: NullAllowed]
		CLMonitoringRecord GetMonitoringRecord (string identifier);
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CLLocationUpdater {
		[Static]
		[Export ("liveUpdaterWithQueue:handler:")]
		[return: NullAllowed]
		CLLocationUpdater CreateLiveUpdates (DispatchQueue queue, Action<CLUpdate> handler);

		[Static]
		[Export ("liveUpdaterWithConfiguration:queue:handler:")]
		[return: NullAllowed]
		CLLocationUpdater CreateLiveUpdates (CLLiveUpdateConfiguration configuration, DispatchQueue queue, Action<CLUpdate> handler);

		[Export ("resume")]
		void Resume ();

		[Export ("pause")]
		void Pause ();

		[Export ("invalidate")]
		void Invalidate ();
	}

	[NoWatch, NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface CLCondition : NSSecureCoding, NSCopying { }

	[NoWatch, NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (CLCondition))]
	interface CLCircularGeographicCondition : NSSecureCoding {
		[Export ("center")]
		CLLocationCoordinate2D Center { get; }

		[Export ("radius")]
		double Radius { get; }

		[Export ("initWithCenter:radius:")]
		NativeHandle Constructor (CLLocationCoordinate2D center, double radius);
	}

	[NoWatch, NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (CLCondition))]
	[DisableDefaultCtor]
	interface CLBeaconIdentityCondition : NSCopying, NSSecureCoding {
		[Export ("UUID", ArgumentSemantic.Copy)]
		NSUuid Uuid { get; }

		[NullAllowed]
		[Export ("major", ArgumentSemantic.Copy)]
		NSNumber Major { get; }

		[NullAllowed]
		[Export ("minor", ArgumentSemantic.Copy)]
		NSNumber Minor { get; }

		[Export ("initWithUUID:")]
		NativeHandle Constructor (NSUuid uuid);

		[Export ("initWithUUID:major:")]
		NativeHandle Constructor (NSUuid uuid, ushort major);

		[Export ("initWithUUID:major:minor:")]
		NativeHandle Constructor (NSUuid uuid, ushort major, ushort minor);
	}

	[Watch (10, 0), NoTV, NoMac, iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CLBackgroundActivitySession {
		[Watch (10, 0), NoMac, iOS (17, 0)]
		[Export ("invalidate")]
		void Invalidate ();

		[Watch (10, 0), NoMac, iOS (17, 0)]
		[Static]
		[Export ("backgroundActivitySession")]
		CLBackgroundActivitySession Create ();
	}
}
