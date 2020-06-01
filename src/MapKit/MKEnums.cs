//
// MapKit enumerations
//
// Author:
//   Miguel de Icaza
//
// Copyright 2009 Novell, Inc.
// Copyright 2014-2016 Xamarin Inc.
//

using System;
using System.Runtime.InteropServices;
using CoreGraphics;
using CoreLocation;
using Foundation;
using ObjCRuntime;

namespace MapKit {

	// NSUInteger -> MKDirectionsTypes.h
	[NoWatch]
	[Native]
	[TV (9,2)]
	[iOS (7,0)]
	public enum MKDirectionsTransportType : ulong {
		Automobile = 1 << 0,
		Walking    = 1 << 1,
		Transit    = 1 << 2, 
		Any        = 0x0FFFFFFF,
	}

	// NSUInteger -> MKTypes.h
	[TV (9,2)]
	[NoWatch]
	[Native]
	public enum MKMapType : ulong {
		Standard = 0,
		Satellite,
		Hybrid,
		SatelliteFlyover,
		HybridFlyover,
		[iOS (11,0)][TV (11,0)][Mac (10,13)]
		MutedStandard,
	}

	// NSUInteger -> MKDistanceFormatter.h
	[Native]
	[TV (9,2)]
	[iOS (7,0)]
	public enum MKDistanceFormatterUnits : ulong {
		Default,
		Metric,
		Imperial,
		ImperialWithYards,
	}

	// NSUInteger -> MKDistanceFormatter.h
	[Native]
	[TV (9,2)]
	[iOS (7,0)]
	public enum MKDistanceFormatterUnitStyle : ulong {
		Default = 0,
		Abbreviated,
		Full,
	}

	// NSInteger -> MKMapView.h
	[TV (9,2)]
	[NoWatch]
	[Native]
	[iOS (7,0)]
	public enum MKOverlayLevel : long {
		AboveRoads = 0,
		AboveLabels,
	}

	// NSUInteger -> MKTypes.h
	[TV (9,2)]
	[NoWatch]
	[Native]
	[ErrorDomain ("MKErrorDomain")]
	public enum MKErrorCode : ulong {
		Unknown = 1,
		ServerFailure,
		LoadingThrottled,
		PlacemarkNotFound,
		DirectionsNotFound,
	}

	// NSUInteger -> MKTypes.h
	[NoTV]
	[NoWatch]
	[Native]
	public enum MKAnnotationViewDragState : ulong {
		None, Starting, Dragging, Canceling, Ending
	}
	
	// NSUInteger -> MKTypes.h
	[NoTV]
	[NoWatch]
	[Native]
	[Deprecated (PlatformName.iOS, 9, 0, message : "Use 'MKPinAnnotationView.PinTintColor' instead.")]
	public enum MKPinAnnotationColor : ulong {
		Red, Green, Purple
	}

	// NSUInteger -> MKTypes.h
	[TV (9,2)]
	[NoWatch]
	[Native]
	public enum MKUserTrackingMode : ulong {
		None, Follow, FollowWithHeading
	}

	[TV (9,2)][NoWatch][iOS (9,3)]
	[Native]
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'MKLocalSearchCompleterResultType' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'MKLocalSearchCompleterResultType' instead.")]
	[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'MKLocalSearchCompleterResultType' instead.")]
	public enum MKSearchCompletionFilterType : long {
		AndQueries = 0,
		Only
	}

	[TV (11,0)][NoWatch][iOS (11,0)][Mac (10,13)]
	[Native]
	public enum MKAnnotationViewCollisionMode : long {
		Rectangle,
		Circle,
	}

	[TV (11,0)][NoWatch][iOS (11,0)][NoMac]
	[Native]
	public enum MKScaleViewAlignment : long {
		Leading,
		Trailing,
	}

	[TV (11,0)][NoWatch][iOS (11,0)][Mac (10,13)]
	[Native]
	public enum MKFeatureVisibility : long {
		Adaptive,
		Hidden,
		Visible,
	}

	[Flags]
	[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
	[Native]
	public enum MKLocalSearchCompleterResultType : ulong
	{
		Address = 1 << 0,
		PointOfInterest = 1 << 1,
		Query = 1 << 2,
	}

	[Flags]
	[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
	[Native]
	public enum MKLocalSearchResultType : ulong
	{
		Address = 1 << 0,
		PointOfInterest = 1 << 1,
	}

#if !WATCH
	[TV (13, 0), NoWatch, Mac (10, 15), iOS (13, 0)]
	public enum MKPointOfInterestCategory {

		[Field ("MKPointOfInterestCategoryAirport")]
		Airport,

		[Field ("MKPointOfInterestCategoryAmusementPark")]
		AmusementPark,

		[Field ("MKPointOfInterestCategoryAquarium")]
		Aquarium,

		[Field ("MKPointOfInterestCategoryATM")]
		Atm,

		[Field ("MKPointOfInterestCategoryBakery")]
		Bakery,

		[Field ("MKPointOfInterestCategoryBank")]
		Bank,

		[Field ("MKPointOfInterestCategoryBeach")]
		Beach,

		[Field ("MKPointOfInterestCategoryBrewery")]
		Brewery,

		[Field ("MKPointOfInterestCategoryCafe")]
		Cafe,

		[Field ("MKPointOfInterestCategoryCampground")]
		Campground,

		[Field ("MKPointOfInterestCategoryCarRental")]
		CarRental,

		[Field ("MKPointOfInterestCategoryEVCharger")]
		EVCharger,

		[Field ("MKPointOfInterestCategoryFireStation")]
		FireStation,

		[Field ("MKPointOfInterestCategoryFitnessCenter")]
		FitnessCenter,

		[Field ("MKPointOfInterestCategoryFoodMarket")]
		FoodMarket,

		[Field ("MKPointOfInterestCategoryGasStation")]
		GasStation,

		[Field ("MKPointOfInterestCategoryHospital")]
		Hospital,

		[Field ("MKPointOfInterestCategoryHotel")]
		Hotel,

		[Field ("MKPointOfInterestCategoryLaundry")]
		Laundry,

		[Field ("MKPointOfInterestCategoryLibrary")]
		Library,

		[Field ("MKPointOfInterestCategoryMarina")]
		Marina,

		[Field ("MKPointOfInterestCategoryMovieTheater")]
		MovieTheater,

		[Field ("MKPointOfInterestCategoryMuseum")]
		Museum,

		[Field ("MKPointOfInterestCategoryNationalPark")]
		NationalPark,

		[Field ("MKPointOfInterestCategoryNightlife")]
		Nightlife,

		[Field ("MKPointOfInterestCategoryPark")]
		Park,

		[Field ("MKPointOfInterestCategoryParking")]
		Parking,

		[Field ("MKPointOfInterestCategoryPharmacy")]
		Pharmacy,

		[Field ("MKPointOfInterestCategoryPolice")]
		Police,

		[Field ("MKPointOfInterestCategoryPostOffice")]
		PostOffice,

		[Field ("MKPointOfInterestCategoryPublicTransport")]
		PublicTransport,

		[Field ("MKPointOfInterestCategoryRestaurant")]
		Restaurant,

		[Field ("MKPointOfInterestCategoryRestroom")]
		Restroom,

		[Field ("MKPointOfInterestCategorySchool")]
		School,

		[Field ("MKPointOfInterestCategoryStadium")]
		Stadium,

		[Field ("MKPointOfInterestCategoryStore")]
		Store,

		[Field ("MKPointOfInterestCategoryTheater")]
		Theater,

		[Field ("MKPointOfInterestCategoryUniversity")]
		University,

		[Field ("MKPointOfInterestCategoryWinery")]
		Winery,

		[Field ("MKPointOfInterestCategoryZoo")]
		Zoo,
	}

#endif
}
