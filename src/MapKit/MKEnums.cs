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
#if NET
	[SupportedOSPlatform ("tvos9.2")]
	[SupportedOSPlatform ("ios7.0")]
#else
	[NoWatch]
	[TV (9,2)]
	[iOS (7,0)]
#endif
	[Native]
	public enum MKDirectionsTransportType : ulong {
		Automobile = 1 << 0,
		Walking    = 1 << 1,
		Transit    = 1 << 2, 
		Any        = 0x0FFFFFFF,
	}

	// NSUInteger -> MKTypes.h
#if NET
	[SupportedOSPlatform ("tvos9.2")]
#else
	[TV (9,2)]
	[NoWatch]
#endif
	[Native]
	public enum MKMapType : ulong {
		Standard = 0,
		Satellite,
		Hybrid,
		SatelliteFlyover,
		HybridFlyover,
#if NET
		[SupportedOSPlatform ("ios11.0")]
		[SupportedOSPlatform ("tvos11.0")]
		[SupportedOSPlatform ("macos10.13")]
#else
		[iOS (11,0)]
		[TV (11,0)]
		[Mac (10,13)]
#endif
		MutedStandard,
	}

	// NSUInteger -> MKDistanceFormatter.h
#if NET
	[SupportedOSPlatform ("tvos9.2")]
	[SupportedOSPlatform ("ios7.0")]
#else
	[TV (9,2)]
	[iOS (7,0)]
#endif
	[Native]
	public enum MKDistanceFormatterUnits : ulong {
		Default,
		Metric,
		Imperial,
		ImperialWithYards,
	}

	// NSUInteger -> MKDistanceFormatter.h
#if NET
	[SupportedOSPlatform ("tvos9.2")]
	[SupportedOSPlatform ("ios7.0")]
#else
	[TV (9,2)]
	[iOS (7,0)]
#endif
	[Native]
	public enum MKDistanceFormatterUnitStyle : ulong {
		Default = 0,
		Abbreviated,
		Full,
	}

	// NSInteger -> MKMapView.h
#if NET
	[SupportedOSPlatform ("tvos9.2")]
	[SupportedOSPlatform ("ios7.0")]
#else
	[TV (9,2)]
	[NoWatch]
	[iOS (7,0)]
#endif
	[Native]
	public enum MKOverlayLevel : long {
		AboveRoads = 0,
		AboveLabels,
	}

	// NSUInteger -> MKTypes.h
#if NET
	[SupportedOSPlatform ("tvos9.2")]
#else
	[TV (9,2)]
	[NoWatch]
#endif
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
#if NET
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoTV]
	[NoWatch]
#endif
	[Native]
	public enum MKAnnotationViewDragState : ulong {
		None, Starting, Dragging, Canceling, Ending
	}
	
	// NSUInteger -> MKTypes.h
#if NET
	[UnsupportedOSPlatform ("ios9.0")]
#if IOS
	[Obsolete ("Starting with ios9.0 use 'MKPinAnnotationView.PinTintColor' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoTV]
	[NoWatch]
	[Deprecated (PlatformName.iOS, 9, 0, message : "Use 'MKPinAnnotationView.PinTintColor' instead.")]
#endif
	[Native]
	public enum MKPinAnnotationColor : ulong {
		Red, Green, Purple
	}

	// NSUInteger -> MKTypes.h
#if NET
	[SupportedOSPlatform ("tvos9.2")]
#else
	[TV (9,2)]
	[NoWatch]
#endif
	[Native]
	public enum MKUserTrackingMode : ulong {
		None, Follow, FollowWithHeading
	}

#if NET
	[SupportedOSPlatform ("tvos9.2")]
	[SupportedOSPlatform ("ios9.3")]
	[UnsupportedOSPlatform ("macos10.15")]
	[UnsupportedOSPlatform ("tvos13.0")]
	[UnsupportedOSPlatform ("ios13.0")]
#if MONOMAC
	[Obsolete ("Starting with macos10.15 use 'MKLocalSearchCompleterResultType' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
	[Obsolete ("Starting with tvos13.0 use 'MKLocalSearchCompleterResultType' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
	[Obsolete ("Starting with ios13.0 use 'MKLocalSearchCompleterResultType' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
	[TV (9,2)]
	[NoWatch]
	[iOS (9,3)]
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'MKLocalSearchCompleterResultType' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'MKLocalSearchCompleterResultType' instead.")]
	[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'MKLocalSearchCompleterResultType' instead.")]
#endif
	[Native]
	public enum MKSearchCompletionFilterType : long {
		AndQueries = 0,
		Only
	}

#if NET
	[SupportedOSPlatform ("tvos11.0")]
	[SupportedOSPlatform ("ios11.0")]
	[SupportedOSPlatform ("macos10.13")]
#else
	[TV (11,0)]
	[NoWatch]
	[iOS (11,0)]
	[Mac (10,13)]
#endif
	[Native]
	public enum MKAnnotationViewCollisionMode : long {
		Rectangle,
		Circle,
#if NET
		[SupportedOSPlatform ("tvos14.0")]
		[SupportedOSPlatform ("ios14.0")]
		[SupportedOSPlatform ("macos11.0")]
#else
		[TV (14,0)]
		[iOS (14,0)]
		[Mac (11,0)]
#endif
		None,
	}

#if NET
	[SupportedOSPlatform ("tvos11.0")]
	[SupportedOSPlatform ("ios11.0")]
	[UnsupportedOSPlatform ("macos")]
#else
	[TV (11,0)]
	[NoWatch]
	[iOS (11,0)]
	[NoMac]
#endif
	[Native]
	public enum MKScaleViewAlignment : long {
		Leading,
		Trailing,
	}

#if NET
	[SupportedOSPlatform ("tvos11.0")]
	[SupportedOSPlatform ("ios11.0")]
	[SupportedOSPlatform ("macos10.13")]
#else
	[TV (11,0)]
	[NoWatch]
	[iOS (11,0)]
	[Mac (10,13)]
#endif
	[Native]
	public enum MKFeatureVisibility : long {
		Adaptive,
		Hidden,
		Visible,
	}

#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("macos10.15")]
	[SupportedOSPlatform ("ios13.0")]
#else
	[TV (13,0)]
	[NoWatch]
	[Mac (10,15)]
	[iOS (13,0)]
#endif
	[Flags]
	[Native]
	public enum MKLocalSearchCompleterResultType : ulong
	{
		Address = 1 << 0,
		PointOfInterest = 1 << 1,
		Query = 1 << 2,
	}

#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("macos10.15")]
	[SupportedOSPlatform ("ios13.0")]
#else
	[TV (13,0)]
	[NoWatch]
	[Mac (10,15)]
	[iOS (13,0)]
#endif
	[Flags]
	[Native]
	public enum MKLocalSearchResultType : ulong
	{
		Address = 1 << 0,
		PointOfInterest = 1 << 1,
	}

#if !WATCH
#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("macos10.15")]
	[SupportedOSPlatform ("ios13.0")]
#else
	[TV (13, 0)]
	[NoWatch]
	[Mac (10, 15)]
	[iOS (13, 0)]
#endif
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
