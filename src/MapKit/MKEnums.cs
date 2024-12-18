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

#nullable enable

namespace MapKit {

	// NSUInteger -> MKDirectionsTypes.h
	[Native]
	[MacCatalyst (13, 1)]
	public enum MKDirectionsTransportType : ulong {
		Automobile = 1 << 0,
		Walking = 1 << 1,
		Transit = 1 << 2,
		Any = 0x0FFFFFFF,
	}

	// NSUInteger -> MKTypes.h
	[MacCatalyst (13, 1)]
	[Native]
	public enum MKMapType : ulong {
		Standard = 0,
		Satellite,
		Hybrid,
		SatelliteFlyover,
		HybridFlyover,
		[MacCatalyst (13, 1)]
		MutedStandard,
	}

	// NSUInteger -> MKDistanceFormatter.h
	[Native]
	[MacCatalyst (13, 1)]
	public enum MKDistanceFormatterUnits : ulong {
		Default,
		Metric,
		Imperial,
		ImperialWithYards,
	}

	// NSUInteger -> MKDistanceFormatter.h
	[Native]
	[MacCatalyst (13, 1)]
	public enum MKDistanceFormatterUnitStyle : ulong {
		Default = 0,
		Abbreviated,
		Full,
	}

	// NSInteger -> MKMapView.h
	[MacCatalyst (13, 1)]
	[Native]
	public enum MKOverlayLevel : long {
		AboveRoads = 0,
		AboveLabels,
	}

	// NSUInteger -> MKTypes.h
	[MacCatalyst (13, 1)]
	[Native]
	[ErrorDomain ("MKErrorDomain")]
	public enum MKErrorCode : ulong {
		Unknown = 1,
		ServerFailure,
		LoadingThrottled,
		PlacemarkNotFound,
		DirectionsNotFound,
		DecodingFailed,
	}

	// NSUInteger -> MKTypes.h
	/// <summary>An enumeration of valid states for a dragged <see cref="T:MapKit.MKAnnotationView" />.</summary>
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum MKAnnotationViewDragState : ulong {
		None, Starting, Dragging, Canceling, Ending
	}

	// NSUInteger -> MKTypes.h
	/// <summary>Color for map pins.</summary>
	[NoTV]
	[Native]
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'MKPinAnnotationView.PinTintColor' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'MKPinAnnotationView.PinTintColor' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'MKPinAnnotationView.PinTintColor' instead.")]
	public enum MKPinAnnotationColor : ulong {
		Red, Green, Purple
	}

	// NSUInteger -> MKTypes.h
	[MacCatalyst (13, 1)]
	[Native]
	public enum MKUserTrackingMode : ulong {
		None,
		Follow,
#if !XAMCORE_5_0 && !(IOS || MACCATALYST)
		[Obsolete ("This is only available on iOS and MacCatalyst.")]
		FollowWithHeading,
#elif IOS || MACCATALYST
		FollowWithHeading,
#endif
	}

	[Native]
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'MKLocalSearchCompleterResultType' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'MKLocalSearchCompleterResultType' instead.")]
	[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'MKLocalSearchCompleterResultType' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'MKLocalSearchCompleterResultType' instead.")]
	public enum MKSearchCompletionFilterType : long {
		AndQueries = 0,
		Only
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum MKAnnotationViewCollisionMode : long {
		Rectangle,
		Circle,
		[TV (14, 0)]
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		None,
	}

	[NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum MKScaleViewAlignment : long {
		Leading,
		Trailing,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum MKFeatureVisibility : long {
		Adaptive,
		Hidden,
		Visible,
	}

	[Flags]
	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum MKLocalSearchCompleterResultType : ulong {
		Address = 1 << 0,
		PointOfInterest = 1 << 1,
		Query = 1 << 2,
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		PhysicalFeature = 1 << 3,
	}

	[Flags]
	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum MKLocalSearchResultType : ulong {
		Address = 1 << 0,
		PointOfInterest = 1 << 1,
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		PhysicalFeature = 1 << 2,
	}

	[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), TV (16, 0)]
	[Native]
	public enum MKDirectionsRoutePreference : long {
		Any = 0,
		Avoid,
	}

	[Flags]
	[NoMac, iOS (16, 0), MacCatalyst (16, 0), NoTV]
	[Native]
	public enum MKMapFeatureOptions : long {
		PointsOfInterest = 1 << (int) MKMapFeatureType.PointOfInterest,
		Territories = 1 << (int) MKMapFeatureType.Territory,
		PhysicalFeatures = 1 << (int) MKMapFeatureType.PhysicalFeature,
	}

	[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoTV]
	[Native]
	public enum MKLookAroundBadgePosition : long {
		TopLeading = 0,
		TopTrailing,
		BottomTrailing,
	}

	[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), TV (16, 0)]
	[Native]
	public enum MKMapElevationStyle : long {
		Flat = 0,
		Realistic,
	}

	[NoMac, iOS (16, 0), MacCatalyst (16, 0), NoTV]
	[Native]
	public enum MKMapFeatureType : long {
		PointOfInterest = 0,
		Territory,
		PhysicalFeature,
	}

	[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), TV (16, 0)]
	[Native]
	public enum MKStandardMapEmphasisStyle : long {
		Default = 0,
		Muted,
	}

#if !WATCH
	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
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

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("MKPointOfInterestCategoryAnimalService")]
		AnimalService,

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("MKPointOfInterestCategoryAutomotiveRepair")]
		AutomotiveRepair,

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("MKPointOfInterestCategoryBaseball")]
		Baseball,

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("MKPointOfInterestCategoryBasketball")]
		Basketball,

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("MKPointOfInterestCategoryBeauty")]
		Beauty,

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("MKPointOfInterestCategoryBowling")]
		Bowling,

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("MKPointOfInterestCategoryCastle")]
		Castle,

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("MKPointOfInterestCategoryConventionCenter")]
		ConventionCenter,

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("MKPointOfInterestCategoryDistillery")]
		Distillery,

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("MKPointOfInterestCategoryFairground")]
		Fairground,

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("MKPointOfInterestCategoryFishing")]
		Fishing,

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("MKPointOfInterestCategoryFortress")]
		Fortress,

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("MKPointOfInterestCategoryGolf")]
		Golf,

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("MKPointOfInterestCategoryGoKart")]
		GoKart,

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("MKPointOfInterestCategoryHiking")]
		Hiking,

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("MKPointOfInterestCategoryKayaking")]
		Kayaking,

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("MKPointOfInterestCategoryLandmark")]
		Landmark,

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("MKPointOfInterestCategoryMailbox")]
		Mailbox,

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("MKPointOfInterestCategoryMiniGolf")]
		MiniGolf,

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("MKPointOfInterestCategoryMusicVenue")]
		MusicVenue,

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("MKPointOfInterestCategoryNationalMonument")]
		NationalMonument,

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("MKPointOfInterestCategoryPlanetarium")]
		Planetarium,

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("MKPointOfInterestCategoryRockClimbing")]
		RockClimbing,

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("MKPointOfInterestCategoryRVPark")]
		RVPark,

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("MKPointOfInterestCategorySkatePark")]
		SkatePark,

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("MKPointOfInterestCategorySkating")]
		Skating,

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("MKPointOfInterestCategorySkiing")]
		Skiing,

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("MKPointOfInterestCategorySoccer")]
		Soccer,

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("MKPointOfInterestCategorySpa")]
		Spa,

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("MKPointOfInterestCategorySurfing")]
		Surfing,

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("MKPointOfInterestCategorySwimming")]
		Swimming,

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("MKPointOfInterestCategoryTennis")]
		Tennis,

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("MKPointOfInterestCategoryVolleyball")]
		Volleyball,
	}

#endif
}
