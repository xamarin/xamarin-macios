
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
using XamCore.ObjCRuntime;
using XamCore.Foundation;
using XamCore.CoreGraphics;
using XamCore.CoreLocation;
#if !MONOMAC
using XamCore.UIKit;
#endif
using System;

namespace XamCore.CoreLocation {

	[NoTV][NoWatch]
	[iOS (7,0)]
	[Native] // NSInteger -> CLRegion.h
	public enum CLRegionState : nint {
		Unknown,
		Inside,
		Outside
	}

	[NoTV][NoWatch]
	[iOS (7,0)]
	[Native] // NSInteger -> CLRegion.h
	public enum CLProximity : nint {
		Unknown,
		Immediate,
		Near,
		Far
	}

	[NoWatch][NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // will crash, see CoreLocation.cs for compatibility stubs
	[Mac (10,7)]
	partial interface CLHeading : NSSecureCoding, NSCopying {
		[Export ("magneticHeading")]
		double MagneticHeading { get;  }
	
		[Export ("trueHeading")]
		double TrueHeading { get;  }
	
		[Export ("headingAccuracy")]
		double HeadingAccuracy { get;  }
	
		[Export ("x")]
		double X { get;  }
	
		[Export ("y")]
		double Y { get;  }
	
		[Export ("z")]
		double Z { get;  }

		[Export ("timestamp", ArgumentSemantic.Copy)]
		NSDate Timestamp { get;  }
	}
	
	[BaseType (typeof (NSObject))]
	partial interface CLLocation : NSSecureCoding, NSCopying {
		[Export ("coordinate")]
		CLLocationCoordinate2D Coordinate { get;  }
	
		[Export ("altitude")]
		double Altitude { get;  }
	
		[Export ("horizontalAccuracy")]
		double HorizontalAccuracy { get;  }
	
		[Export ("verticalAccuracy")]
		double VerticalAccuracy { get;  }
	
		[NoWatch][NoTV]
		[Export ("course")]
		double Course { get;  }
	
		[NoWatch][NoTV]
		[Export ("speed")]
		double Speed { get;  }

		[Export ("timestamp", ArgumentSemantic.Copy)]
		NSDate Timestamp { get;  }
	
		[Export ("initWithLatitude:longitude:")]
		IntPtr Constructor (double latitude, double longitude);
	
		[Export ("initWithCoordinate:altitude:horizontalAccuracy:verticalAccuracy:timestamp:")]
		IntPtr Constructor (CLLocationCoordinate2D coordinate, double altitude, double hAccuracy, double vAccuracy, NSDate timestamp);

#if !XAMCORE_2_0
		[Export ("getDistanceFrom:")]
		[Availability (Deprecated = Platform.iOS_3_2, Message = "Use DistanceFrom instead")]
		double Distancefrom (CLLocation  location);
#endif

		[Since (3,2)]
		[Export ("distanceFromLocation:")]
		double DistanceFrom (CLLocation location);

		[Since (4,2)]
		[Export ("initWithCoordinate:altitude:horizontalAccuracy:verticalAccuracy:course:speed:timestamp:")]
		IntPtr Constructor (CLLocationCoordinate2D coordinate, double altitude, double hAccuracy, double vAccuracy, double course, double speed, NSDate timestamp);

		// Apple keep changing the 'introduction' of this field (5.0->8.0->5.0) but it was not available in 6.1
		// nor in 7.0 - but it works on my iPad3 running iOS 7.1
		[NoTV][NoWatch]
		[iOS (7,1)][MountainLion]
		[Field ("kCLErrorUserInfoAlternateRegionKey")]
		NSString ErrorUserInfoAlternateRegionKey { get; }

#if XAMCORE_2_0
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
#endif

#if !MONOMAC
		[iOS (8,0)]
		[Export ("floor", ArgumentSemantic.Copy)]
		CLFloor Floor { get; }
#endif        
	}

#if !MONOMAC
	[iOS (8,0)]
	[BaseType (typeof(NSObject))]
	partial interface CLFloor : NSSecureCoding, NSCopying {
	        [Export ("level")]
	        nint Level { get; }
    }
#endif

	[BaseType (typeof (NSObject), Delegates=new string [] {"WeakDelegate"}, Events=new Type [] {typeof (CLLocationManagerDelegate)})]
	partial interface CLLocationManager {
		[Wrap ("WeakDelegate")]
		[Protocolize]
		CLLocationManagerDelegate Delegate { get; set;  }

		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
		NSObject WeakDelegate { get; set;  }
	
		[Export ("distanceFilter", ArgumentSemantic.Assign)]
		double DistanceFilter { get; set;  }
	
		[Export ("desiredAccuracy", ArgumentSemantic.Assign)]
		double DesiredAccuracy { get; set;  }
	
		[Export ("location", ArgumentSemantic.Copy)]
		CLLocation Location { get;  }
	
		 // __WATCHOS_PROHIBITED removed in Xcode 8.0 beta 2, assuming it's valid for 3.0+
		[Watch (3,0)]
		[NoTV]
		[Export ("startUpdatingLocation")]
		void StartUpdatingLocation ();
	
		[Export ("stopUpdatingLocation")]
		void StopUpdatingLocation ();

		[Since (4,0)]
		[Export ("locationServicesEnabled"), Static]
		bool LocationServicesEnabled { get; }

#if !MONOMAC
		[NoWatch][NoTV]
		[Export ("headingFilter", ArgumentSemantic.Assign)]
		double HeadingFilter { get; set;  }
	
		[NoWatch][NoTV]
		[Export ("startUpdatingHeading")]
		void StartUpdatingHeading ();
	
		[NoWatch][NoTV]
		[Export ("stopUpdatingHeading")]
		void StopUpdatingHeading ();
	
		[NoWatch][NoTV]
		[Export ("dismissHeadingCalibrationDisplay")]
		void DismissHeadingCalibrationDisplay ();
	
		[NoWatch][NoTV]
		[Since (3,2)]
		[Availability (Introduced = Platform.iOS_3_2, Deprecated = Platform.iOS_6_0)]
		// Default property value is null but it cannot be set to that value
		// it crash when a null is provided
		[Export ("purpose")]
		string Purpose { get; set; }

		[NoWatch][NoTV]
		[Since (4,0)]
		[Export ("headingAvailable"), Static]
		bool HeadingAvailable { get; }

		[NoWatch][NoTV]
		[Since (4,0)]
		[Export ("significantLocationChangeMonitoringAvailable"), Static]
		bool SignificantLocationChangeMonitoringAvailable { get; }

		[NoWatch][NoTV]
		[Availability (Introduced = Platform.iOS_4_0, Deprecated = Platform.iOS_7_0, Message = "Use IsMonitoringAvailable instead")]
		[Export ("regionMonitoringAvailable"), Static]
		bool RegionMonitoringAvailable { get; }

		[NoWatch][NoTV]
		[Since (4,0)]
		[Availability (Introduced = Platform.iOS_4_0, Deprecated = Platform.iOS_6_0, Message = "Use IsMonitoringAvailable and AuthorizationStatus instead")]
		[Export ("regionMonitoringEnabled"), Static]
		bool RegionMonitoringEnabled { get; }

		[NoWatch][NoTV]
		[Since (4,0)]
		[Export ("headingOrientation", ArgumentSemantic.Assign)]
		CLDeviceOrientation HeadingOrientation { get; set; }

		[NoWatch][NoTV]
		[Export ("heading", ArgumentSemantic.Copy)]
		[Since (4,0)]
		CLHeading Heading { get; }

		[NoWatch][NoTV]
		[Export ("maximumRegionMonitoringDistance")]
		[Since (4,0)]
		double MaximumRegionMonitoringDistance { get; }

		[NoWatch][NoTV]
		[Export ("monitoredRegions", ArgumentSemantic.Copy)]
		[Since (4,0)]
		NSSet MonitoredRegions { get; }

		[NoWatch][NoTV]
		[Since (4,0)]
		[Export ("startMonitoringSignificantLocationChanges")]
		void StartMonitoringSignificantLocationChanges ();

		[NoWatch][NoTV]
		[Since (4,0)]
		[Export ("stopMonitoringSignificantLocationChanges")]
		void StopMonitoringSignificantLocationChanges ();

		[NoWatch][NoTV]
		[Since (4,0)]
		[Availability (Introduced = Platform.iOS_4_0, Deprecated = Platform.iOS_6_0)]
		[Export ("startMonitoringForRegion:desiredAccuracy:")]
		void StartMonitoring (CLRegion region, double desiredAccuracy);

		[NoWatch][NoTV]
		[Since (4,0)]
		[Export ("stopMonitoringForRegion:")]
		void StopMonitoring (CLRegion region);

		[Since (4,2)]
		[Export ("authorizationStatus")][Static]
		CLAuthorizationStatus Status { get; }

		[NoWatch][NoTV]
		[Since (5,0)]
		[Export ("startMonitoringForRegion:")]
		void StartMonitoring (CLRegion region);

		[NoWatch][NoTV]
		[Since (6,0)]
		[Export ("activityType", ArgumentSemantic.Assign)]
		CLActivityType ActivityType  { get; set; }

		[NoWatch][NoTV]
		[Since (6,0)]
		[Export ("pausesLocationUpdatesAutomatically", ArgumentSemantic.Assign)]
		bool PausesLocationUpdatesAutomatically { get; set; }

		[NoWatch][NoTV]
		[Since (6,0)]
		[Export ("allowDeferredLocationUpdatesUntilTraveled:timeout:")]
		void AllowDeferredLocationUpdatesUntil (double distance, double timeout);

		[NoWatch][NoTV]
		[Since (6,0)]
		[Export ("disallowDeferredLocationUpdates")]
		void DisallowDeferredLocationUpdates ();

		[NoWatch][NoTV]
		[Since (6,0)]
		[Static]
		[Export ("deferredLocationUpdatesAvailable")]
		bool DeferredLocationUpdatesAvailable { get; }

		[Since (6,0)]
		[Field ("CLTimeIntervalMax")]
		double MaxTimeInterval { get; }

		[NoWatch][NoTV]
		[Since (7,0), Static, Export ("isMonitoringAvailableForClass:")]
		bool IsMonitoringAvailable (Class regionClass);

		[NoWatch][NoTV]
		[Since (7,0), Export ("rangedRegions", ArgumentSemantic.Copy)]
		NSSet RangedRegions { get; }

		[NoWatch][NoTV]
		[Since (7,0), Export ("requestStateForRegion:")]
		void RequestState (CLRegion region);

		[NoWatch][NoTV]
		[Since (7,0), Export ("startRangingBeaconsInRegion:")]
		void StartRangingBeacons (CLBeaconRegion region);

		[NoWatch][NoTV]
		[Since (7,0), Export ("stopRangingBeaconsInRegion:")]
		void StopRangingBeacons (CLBeaconRegion region);

		[NoWatch][NoTV]
		[iOS (7,0)]
		[Static]
		[Export ("isRangingAvailable")]
		bool IsRangingAvailable { get; }
		
		[iOS (8,0)]
		[Export ("requestWhenInUseAuthorization")]
		void RequestWhenInUseAuthorization ();

		[NoTV]
		[iOS (8,0)]
		[Export ("requestAlwaysAuthorization")]
		void RequestAlwaysAuthorization ();

		[NoWatch][NoTV]
		[iOS (8,0)]
		[Export ("startMonitoringVisits")]
		void StartMonitoringVisits ();

		[NoWatch][NoTV]
		[iOS (8,0)]
		[Export ("stopMonitoringVisits")]
		void StopMonitoringVisits ();

		[iOS (9,0)]
		[Export ("requestLocation")]
		void RequestLocation ();

		[NoWatch][NoTV]
		[iOS (9,0)]
		[Export ("allowsBackgroundLocationUpdates")]
		bool AllowsBackgroundLocationUpdates { get; set; }
#endif
	}
	
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	partial interface CLLocationManagerDelegate
	{
		[NoWatch][NoTV]
		[Availability (Deprecated = Platform.iOS_6_0)]
		[Export ("locationManager:didUpdateToLocation:fromLocation:"), EventArgs ("CLLocationUpdated")]
		void UpdatedLocation (CLLocationManager  manager, CLLocation newLocation, CLLocation oldLocation);
	
#if !MONOMAC
		[NoWatch][NoTV]
		[Export ("locationManager:didUpdateHeading:"), EventArgs ("CLHeadingUpdated")]
		void UpdatedHeading (CLLocationManager  manager, CLHeading newHeading);
#endif
	
		[NoWatch][NoTV]
		[Export ("locationManagerShouldDisplayHeadingCalibration:"), DelegateName ("CLLocationManagerEventArgs"), DefaultValue (true)]
		bool ShouldDisplayHeadingCalibration (CLLocationManager manager);
	
		[Export ("locationManager:didFailWithError:"), EventArgs ("NSError", true)]
		void Failed (CLLocationManager manager, NSError error);

#if !MONOMAC
		[NoWatch][NoTV]
		[Since (4,0)]
		[Export ("locationManager:didEnterRegion:"), EventArgs ("CLRegion")]
		void RegionEntered (CLLocationManager manager, CLRegion region);

		[NoWatch][NoTV]
		[Since (4,0)]
		[Export ("locationManager:didExitRegion:"), EventArgs ("CLRegion")]
		void RegionLeft (CLLocationManager manager, CLRegion region);

		[NoWatch][NoTV]
		[Since (4,0)]
		[Export ("locationManager:monitoringDidFailForRegion:withError:"), EventArgs ("CLRegionError")]
		void MonitoringFailed (CLLocationManager manager, CLRegion region, NSError error);

		[NoWatch][NoTV]
		[Since(5,0)]
		[Export ("locationManager:didStartMonitoringForRegion:"), EventArgs ("CLRegion")]
		void DidStartMonitoringForRegion (CLLocationManager manager, CLRegion region);

		[NoWatch][NoTV]
		[Since (7,0), Export ("locationManager:didDetermineState:forRegion:"), EventArgs ("CLRegionStateDetermined")]
		void DidDetermineState (CLLocationManager manager, CLRegionState state, CLRegion region);

		[NoWatch][NoTV]
		[Since (7,0), Export ("locationManager:didRangeBeacons:inRegion:"), EventArgs ("CLRegionBeaconsRanged")]
		void DidRangeBeacons (CLLocationManager manager, CLBeacon [] beacons, CLBeaconRegion region);

		[NoWatch][NoTV]
		[Since (7,0), Export ("locationManager:rangingBeaconsDidFailForRegion:withError:"), EventArgs ("CLRegionBeaconsFailed")]
		void RangingBeaconsDidFailForRegion (CLLocationManager manager, CLBeaconRegion region, NSError error);

		[NoWatch][NoTV]
		[iOS (8,0)]
		[Export ("locationManager:didVisit:"), EventArgs ("CLVisited")]
		void DidVisit (CLLocationManager manager, CLVisit visit);
#endif

		[Since (4,2)]
		[Export ("locationManager:didChangeAuthorizationStatus:"), EventArgs ("CLAuthorizationChanged")]
		void AuthorizationChanged (CLLocationManager manager, CLAuthorizationStatus status);

		[Since (6,0)]
		[Export ("locationManager:didUpdateLocations:"), EventArgs ("CLLocationsUpdated")]
		void LocationsUpdated (CLLocationManager manager, CLLocation[] locations);

		[NoWatch][NoTV]
		[Since (6,0)]
		[Export ("locationManagerDidPauseLocationUpdates:"), EventArgs ("")]
		void LocationUpdatesPaused (CLLocationManager manager);

		[NoWatch][NoTV]
		[Since (6,0)]
		[Export ("locationManagerDidResumeLocationUpdates:"), EventArgs ("")]
		void LocationUpdatesResumed (CLLocationManager manager);

		[NoWatch][NoTV]
		[Since (6,0)]
		[Export ("locationManager:didFinishDeferredUpdatesWithError:"), EventArgs ("NSError", true)]
		void DeferredUpdatesFinished (CLLocationManager manager, NSError error);
	}

	[Static]
	partial interface CLLocationDistance {

		[Since (6,0)]
		[Mac (10,9)]
		[Field ("CLLocationDistanceMax")]
		double MaxDistance { get; }

		[Field ("kCLDistanceFilterNone")]
		double FilterNone { get; }
	}
		
	[Since (4,0)]
	[Mac (10,7)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // will crash, see CoreLocation.cs for compatibility stubs
	public partial interface CLRegion : NSSecureCoding, NSCopying {
		[NoTV]
		[Availability (Deprecated = Platform.iOS_7_0 | Platform.Mac_10_10, Message = "Use CLCircularRegion instead")]
		[Export ("center")]
		CLLocationCoordinate2D Center { get;  }

		[NoTV]
		[Availability (Deprecated = Platform.iOS_7_0 | Platform.Mac_10_10, Message = "Use CLCircularRegion instead")]
		[Export ("radius")]
		double Radius { get;  }

		[Export ("identifier")]
		string Identifier { get;  }

		[NoTV]
		[Availability (Deprecated = Platform.iOS_7_0 | Platform.Mac_10_10, Message = "Use CLCircularRegion instead")]
		[Export ("initCircularRegionWithCenter:radius:identifier:")]
		IntPtr Constructor (CLLocationCoordinate2D center, double radius, string identifier);

		[NoTV]
		[Availability (Deprecated = Platform.iOS_7_0 | Platform.Mac_10_10, Message = "Use CLCircularRegion instead")]
		[Export ("containsCoordinate:")]
		bool Contains (CLLocationCoordinate2D coordinate);

		[Since (7,0), Export ("notifyOnEntry", ArgumentSemantic.Assign)]
		[Mac (10,10)]
		bool NotifyOnEntry { get; set; }

		[Since (7,0), Export ("notifyOnExit", ArgumentSemantic.Assign)]
		[Mac (10,10)]
		bool NotifyOnExit { get; set; }
	}

	[Since (5,0)]
	[Mac (10,8)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // will crash, see CoreLocation.cs for compatibility stubs
	interface CLPlacemark : NSSecureCoding, NSCopying {
		[Export("addressDictionary", ArgumentSemantic.Copy)]
		NSDictionary AddressDictionary { get; }

		[Export("administrativeArea")]
		string AdministrativeArea { get; }

		[Export("subAdministrativeArea")]
		string SubAdministrativeArea { get; }

		[Export("subLocality")]
		string SubLocality { get; }

		[Export("locality")]
		string Locality { get; }

		[Export("country")]
		string Country { get; }
	
		[Export("postalCode")]
		string PostalCode { get; }

		[Export("thoroughfare")]
		string Thoroughfare { get; }

		[Export("subThoroughfare")]
		string SubThoroughfare { get; }

		[Export ("ISOcountryCode")]
		string IsoCountryCode { get;  }

		[Export ("areasOfInterest")]
		string [] AreasOfInterest { get;  }

		[Export ("initWithPlacemark:")]
		IntPtr Constructor (CLPlacemark placemark);

		[Export ("inlandWater")]
		string InlandWater { get;  }

		[Export ("location", ArgumentSemantic.Copy)]
		CLLocation Location { get; }

		[Export ("name")]
		string Name { get;  }

		[Export ("ocean")]
		string Ocean { get;  }

		[Export ("region", ArgumentSemantic.Copy)]
		CLRegion Region { get; }

		[Export ("timeZone")]
		[iOS (9,0), Mac(10,11)]
		NSTimeZone TimeZone { get; }
	}

#if !MONOMAC
	[Since (7,0), BaseType (typeof (CLRegion))]
	public partial interface CLCircularRegion {

		[Export ("initWithCenter:radius:identifier:")]
		IntPtr Constructor (CLLocationCoordinate2D center, double radius, string identifier);

		[Export ("center")]
		CLLocationCoordinate2D Center { get; }

		[Export ("radius")]
		double Radius { get; }

		[Export ("containsCoordinate:")]
		bool ContainsCoordinate (CLLocationCoordinate2D coordinate);
	}

	[NoWatch][NoMac][NoTV]
	[Since (7,0), BaseType (typeof (CLRegion))]
	[DisableDefaultCtor] // nil-Handle on iOS8 if 'init' is used
	public partial interface CLBeaconRegion {

		[Export ("initWithProximityUUID:identifier:")]
		IntPtr Constructor (NSUuid proximityUuid, string identifier);

		[Export ("initWithProximityUUID:major:identifier:")]
		IntPtr Constructor (NSUuid proximityUuid, ushort major, string identifier);

		[Export ("initWithProximityUUID:major:minor:identifier:")]
		IntPtr Constructor (NSUuid proximityUuid, ushort major, ushort minor, string identifier);

		[Export ("peripheralDataWithMeasuredPower:")]
		NSMutableDictionary GetPeripheralData ([NullAllowed] NSNumber measuredPower);

		[Export ("proximityUUID", ArgumentSemantic.Copy)]
		NSUuid ProximityUuid { get; }

		[Export ("major", ArgumentSemantic.Copy)]
		NSNumber Major { get; }

		[Export ("minor", ArgumentSemantic.Copy)]
		NSNumber Minor { get; }

		[Export ("notifyEntryStateOnDisplay", ArgumentSemantic.Assign)]
		bool NotifyEntryStateOnDisplay { get; set; }
	}

	[NoWatch][NoMac][NoTV]
	[Since (7,0), BaseType (typeof (NSObject))]
	public partial interface CLBeacon : NSCopying, NSSecureCoding {

		[Export ("proximityUUID", ArgumentSemantic.Copy)]
		NSUuid ProximityUuid { get; }

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
	}

	delegate void CLGeocodeCompletionHandler (CLPlacemark [] placemarks, NSError error);

	[Since (5,0)]
	[BaseType (typeof (NSObject))]
	interface CLGeocoder {
		[Export ("isGeocoding")]
		bool Geocoding { get; }

		[Export ("reverseGeocodeLocation:completionHandler:")]
		[Async]
		void ReverseGeocodeLocation (CLLocation location, CLGeocodeCompletionHandler completionHandler);

		[Export ("geocodeAddressDictionary:completionHandler:")]
		[Async]
		void GeocodeAddress (NSDictionary addressDictionary, CLGeocodeCompletionHandler completionHandler);

		[Export ("geocodeAddressString:completionHandler:")]
		[Async]
		void GeocodeAddress (string addressString, CLGeocodeCompletionHandler completionHandler);

		[Export ("geocodeAddressString:inRegion:completionHandler:")]
		[Async]
		void GeocodeAddress (string addressString, CLRegion region, CLGeocodeCompletionHandler completionHandler);

		[Export ("cancelGeocode")]
		void CancelGeocode ();
	}

	[NoWatch][NoTV]
	[iOS (8,0)]
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
#endif
}
