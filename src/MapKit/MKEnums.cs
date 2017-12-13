//
// MapKit enumerations
//
// Author:
//   Miguel de Icaza
//
// Copyright 2009 Novell, Inc.
// Copyright 2014-2016 Xamarin Inc.
//
#if XAMCORE_2_0 || !MONOMAC
using System;
using System.Runtime.InteropServices;
using XamCore.CoreGraphics;
using XamCore.CoreLocation;
using XamCore.Foundation;
using XamCore.ObjCRuntime;
#if !COREBUILD
using MacAttribute = XamCore.ObjCRuntime.Extensions.MacAttribute;
using iOSAttribute = XamCore.ObjCRuntime.Extensions.iOSAttribute;
#endif
using AvailabilityAttribute = XamCore.ObjCRuntime.Extensions.AvailabilityAttribute;
using Platform = XamCore.ObjCRuntime.Extensions.Platform;


namespace XamCore.MapKit {

	// NSUInteger -> MKDirectionsTypes.h
	[NoWatch]
	[Native]
	[TV (9,2)]
	[iOS (7,0)]
	public enum MKDirectionsTransportType : nuint_compat_int {
		Automobile = 1 << 0,
		Walking    = 1 << 1,
		Transit    = 1 << 2, 
		Any        = 0x0FFFFFFF,
	}

	// NSUInteger -> MKTypes.h
	[TV (9,2)]
	[NoWatch]
	[Native]
	public enum MKMapType : nuint_compat_int {
		Standard = 0,
		Satellite,
		Hybrid,
		SatelliteFlyover,
		HybridFlyover,
		[iOS (11,0)][TV (11,0)][Mac (10,13, onlyOn64: true)]
		MutedStandard,
	}

	// NSUInteger -> MKDistanceFormatter.h
	[Native]
	[TV (9,2)]
	[iOS (7,0)]
	public enum MKDistanceFormatterUnits : nuint_compat_int {
		Default,
		Metric,
		Imperial,
		ImperialWithYards,
	}

	// NSUInteger -> MKDistanceFormatter.h
	[Native]
	[TV (9,2)]
	[iOS (7,0)]
	public enum MKDistanceFormatterUnitStyle : nuint_compat_int {
		Default = 0,
		Abbreviated,
		Full,
	}

	// NSInteger -> MKMapView.h
	[TV (9,2)]
	[NoWatch]
	[Native]
	[iOS (7,0)]
	public enum MKOverlayLevel : nint {
		AboveRoads = 0,
		AboveLabels,
	}

	// NSUInteger -> MKTypes.h
	[TV (9,2)]
	[NoWatch]
	[Native]
	[ErrorDomain ("MKErrorDomain")]
	public enum MKErrorCode : nuint_compat_int {
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
	public enum MKAnnotationViewDragState : nuint_compat_int {
		None, Starting, Dragging, Canceling, Ending
	}
	
	// NSUInteger -> MKTypes.h
	[NoTV]
	[NoWatch]
	[Native]
	[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'MKPinAnnotationView.PinTintColor' instead.")]
	public enum MKPinAnnotationColor : nuint_compat_int {
		Red, Green, Purple
	}

	// NSUInteger -> MKTypes.h
	[TV (9,2)]
	[NoWatch]
	[Native]
	public enum MKUserTrackingMode : nuint_compat_int {
		None, Follow, FollowWithHeading
	}

	[TV (9,2)][NoWatch][iOS (9,3)]
	[Native]
	public enum MKSearchCompletionFilterType : nint {
		AndQueries = 0,
		Only
	}

	[TV (11,0)][NoWatch][iOS (11,0)][Mac (10,13, onlyOn64: true)]
	[Native]
	public enum MKAnnotationViewCollisionMode : nint {
		Rectangle,
		Circle,
	}

	[TV (11,0)][NoWatch][iOS (11,0)][NoMac]
	[Native]
	public enum MKScaleViewAlignment : nint {
		Leading,
		Trailing,
	}

	[TV (11,0)][NoWatch][iOS (11,0)][Mac (10,13, onlyOn64: true)]
	[Native]
	public enum MKFeatureVisibility : nint {
		Adaptive,
		Hidden,
		Visible,
	}
}

#endif
