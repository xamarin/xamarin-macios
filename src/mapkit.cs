//
// This file describes the API that the generator will produce
//
// Authors:
//   Geoff Norton
//   Miguel de Icaza
//   Whitney Schmidt
//
// Copyright 2009, Novell, Inc.
// Copyright 2011-2013 Xamarin Inc.
// Copyright 2019 Microsoft Corp.
//

using CoreFoundation;
using ObjCRuntime;
using Foundation;
using CoreGraphics;
using CoreLocation;
#if MONOMAC
using AppKit;
using UITraitCollection = System.Int32;
#else
using UIKit;
#endif
#if !TVOS
using Contacts;
#endif
using System;

#if MONOMAC
using UIImage=AppKit.NSImage;
using UIView=AppKit.NSView;
using UIEdgeInsets=AppKit.NSEdgeInsets;
using UIColor=AppKit.NSColor;
using UIScene=AppKit.NSColor;
using UIControl = AppKit.NSControl;
using UIBarButtonItem = Foundation.NSObject;
#else
using NSAppearance = Foundation.NSObject;
#endif
#if WATCH
// helper for [NoWatch]
using MKMapView=Foundation.NSObject;
using MKAnnotationView=Foundation.NSObject;
using MKShape = Foundation.NSObject;
using MKOverlay = Foundation.NSObjectProtocol;
using MKPolygon = Foundation.NSObject;
using MKPolyline = Foundation.NSObject;
using MKOverlayPathRenderer = Foundation.NSObject;
using IMKOverlay = Foundation.NSObject;
using MKDirectionsRequest = Foundation.NSObject;
using UITraitCollection = Foundation.NSObject;
using UIControl = Foundation.NSObject;
using MKTileOverlayPath = Foundation.NSObject;
using UIBarButtonItem = Foundation.NSObject;
using MKCircle = Foundation.NSObject;
#endif
#if TVOS
using CNPostalAddress = Foundation.NSObject;
#endif

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace MapKit {

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	[Mac (10,9)]
	interface MKAnnotation {
		[Export ("coordinate")][Abstract]
		CLLocationCoordinate2D Coordinate { get; }

		[Export ("title", ArgumentSemantic.Copy)]
		[NullAllowed]
		string Title { get; }
	
		[Export ("subtitle", ArgumentSemantic.Copy)]
		[NullAllowed]
		string Subtitle { get; } 

		[Export ("setCoordinate:")]
		[Mac (10,9)]
		void SetCoordinate (CLLocationCoordinate2D value);
	}

	interface IMKAnnotation {}

#if !WATCH
	[BaseType (typeof (MKAnnotation))]
	[Model]
	[Protocol]
	[Mac (10,9)]
	interface MKOverlay {
		[Abstract]
		[Export ("boundingMapRect")]
		MKMapRect BoundingMapRect { get; }

		[Export ("intersectsMapRect:")]
		bool Intersects (MKMapRect rect);

		// optional, not implemented by MKPolygon, MKPolyline and MKCircle
		// implemented by MKTileOverlay (and defined there)
		[OptionalImplementation]
		[iOS (7,0), Export ("canReplaceMapContent")]
		bool CanReplaceMapContent { get; }
	}

	interface IMKOverlay {}
	
	[BaseType (typeof (UIView))]
	[NoWatch]
	[TV (9,2)]
	[Mac (10,9)]
	interface MKAnnotationView {
		[DesignatedInitializer]
		[Export ("initWithAnnotation:reuseIdentifier:")]
		[PostGet ("Annotation")]
		NativeHandle Constructor ([NullAllowed] IMKAnnotation annotation, [NullAllowed] string reuseIdentifier);
	
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[Export ("reuseIdentifier")]
		[NullAllowed]
		string ReuseIdentifier { get; }
	
		[Export ("prepareForReuse")]
		void PrepareForReuse ();
	
		[Export ("annotation", ArgumentSemantic.Retain)]
		[ThreadSafe] // Sometimes iOS will request the annotation from a non-UI thread (see https://bugzilla.xamarin.com/show_bug.cgi?27609)
		[NullAllowed]
		IMKAnnotation Annotation { get; set; }
	
		[Export ("image", ArgumentSemantic.Retain)]
		[NullAllowed]
		UIImage Image { get; set; }
	
		[Export ("centerOffset")]
		CGPoint CenterOffset { get; set; }
	
		[Export ("calloutOffset")]
		CGPoint CalloutOffset { get; set; }
	
		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }
	
		[Export ("highlighted")]
		bool Highlighted { [Bind ("isHighlighted")] get; set; }
	
		[Export ("selected")]
		bool Selected { [Bind ("isSelected")] get; set; }
	
		[Export ("setSelected:animated:")]
		void SetSelected (bool selected, bool animated);
	
		[Export ("canShowCallout")]
		bool CanShowCallout { get; set; }
	
		[Export ("leftCalloutAccessoryView", ArgumentSemantic.Retain)]
		[NullAllowed]
		UIView LeftCalloutAccessoryView { get; set; }
	
		[Export ("rightCalloutAccessoryView", ArgumentSemantic.Retain)]
		[NullAllowed]
		UIView RightCalloutAccessoryView { get; set; }
	
		[NoTV]
		[Export ("setDragState:animated:")]
		void SetDragState (MKAnnotationViewDragState newDragState, bool animated);

		[Export ("dragState")]
		[NoTV]
		MKAnnotationViewDragState DragState { get; set;  }

		[NoTV]
		[Export ("draggable")]
		bool Draggable { [Bind ("isDraggable")] get; set;  }

		[iOS (9,0), Mac(10,11)]
		[Export ("detailCalloutAccessoryView")]
		[NullAllowed]
		UIView DetailCalloutAccessoryView { get; set; }

		[NoiOS][NoWatch][NoTV][MacCatalyst (13,0)]
		[Export ("leftCalloutOffset")]
		CGPoint LeftCalloutOffset { get; set; }

		[NoiOS][NoWatch][NoTV][MacCatalyst (13,0)]
		[Export ("rightCalloutOffset")]
		CGPoint RightCallpoutOffset { get; set; }

		[TV (11,0)][iOS (11,0)][Mac (10,13)]
		[NullAllowed, Export ("clusteringIdentifier")]
		string ClusteringIdentifier { get; set; }

		[TV (11,0)][iOS (11,0)][Mac (10,13)]
		[NullAllowed, Export ("clusterAnnotationView", ArgumentSemantic.Weak)]
		MKAnnotationView ClusterAnnotationView { get; }

		[TV (11,0)][iOS (11,0)][Mac (10,13)]
		[Advice ("Pre-defined constants are available from 'MKFeatureDisplayPriority'.")]
		[Export ("displayPriority")]
		float DisplayPriority { get; set; }

		[TV (11,0)][iOS (11,0)][Mac (10,13)]
		[Export ("collisionMode", ArgumentSemantic.Assign)]
		MKAnnotationViewCollisionMode CollisionMode { get; set; }

		[TV (11,0)][iOS (11,0)][Mac (10,13)]
		[Export ("prepareForDisplay")]
		[RequiresSuper]
		void PrepareForDisplay ();

		[TV (14, 0), NoWatch, Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14,0)]
		[Export ("zPriority")]
		float ZPriority { get; set; }

		[TV (14, 0), NoWatch, Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14,0)]
		[Export ("selectedZPriority")]
		float SelectedZPriority { get; set; }
	}

	[ThreadSafe]
	[TV (9,2)]
	[Mac (10,9)]
	[BaseType (typeof (MKShape))]
	interface MKCircle : MKOverlay {
		[Export ("radius")]
		double Radius { get;  }

		[Static]
		[Export ("circleWithCenterCoordinate:radius:")]
		MKCircle Circle (CLLocationCoordinate2D withcenterCoordinate, double radius);

		[Static]
		[Export ("circleWithMapRect:")]
		MKCircle CircleWithMapRect (MKMapRect mapRect);

		#region MKAnnotation
		[Export ("coordinate")]
		CLLocationCoordinate2D Coordinate { get; }
		// note: setCoordinate: is not mandatory and is not implemented for MKCircle
		#endregion
	}

	[NoMac][NoTV]
	[BaseType (typeof (MKOverlayPathView))]
	[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'MKCircleRenderer' instead.")]
	interface MKCircleView {
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[Export ("circle")]
		MKCircle Circle { get;  }

		[Export ("initWithCircle:")]
		[PostGet ("Circle")]
		NativeHandle Constructor (MKCircle circle);
	}
	
	[TV (9,2)]
	[Mac (10,9)]
	[BaseType (typeof (NSObject))]
	interface MKDirectionsRequest {
		[NullAllowed] // by default this property is null
		[Export ("destination")]
		MKMapItem Destination { get; [iOS (7,0)] set; }

		[NullAllowed] // by default this property is null
		[Export ("source")]
		MKMapItem Source { get; [iOS (7,0)] set; }

		[Export ("initWithContentsOfURL:")]
		NativeHandle Constructor (NSUrl url);

		[Static]
		[Export ("isDirectionsRequestURL:")]
		bool IsDirectionsRequestUrl (NSUrl url);

		[iOS (7,0), Export ("transportType")]
		MKDirectionsTransportType TransportType { get; set; }

		[iOS (7,0), Export ("requestsAlternateRoutes")]
		bool RequestsAlternateRoutes { get; set; }

		[NullAllowed] // by default this property is null
		[iOS (7,0), Export ("departureDate", ArgumentSemantic.Copy)]
		NSDate DepartureDate { get; set; }

		[NullAllowed] // by default this property is null
		[iOS (7,0), Export ("arrivalDate", ArgumentSemantic.Copy)]
		NSDate ArrivalDate { get; set; }
	}
#endif // !WATCH

	[BaseType (typeof (NSObject))]
	[TV (9,2)]
	[Mac (10,9)]
	interface MKMapItem : NSSecureCoding
#if IOS // #if TARGET_OS_IOS
		, NSItemProviderReading, NSItemProviderWriting
#endif
	{
		[Export ("placemark", ArgumentSemantic.Retain)]
		MKPlacemark Placemark { get;  }

		[Export ("isCurrentLocation")]
		bool IsCurrentLocation { get;  }

		[NullAllowed] // it's null by default on iOS 6.1
		[Export ("name", ArgumentSemantic.Copy)]
		string Name { get; set;  }

		[NullAllowed] // by default this property is null
		[Export ("phoneNumber", ArgumentSemantic.Copy)]
		string PhoneNumber { get; set;  }

		[NullAllowed] // by default this property is null
		[Export ("url", ArgumentSemantic.Retain)]
		NSUrl Url { get; set;  }

		[Static]
		[Export ("mapItemForCurrentLocation")]
		MKMapItem MapItemForCurrentLocation ();

		[Export ("initWithPlacemark:")]
		NativeHandle Constructor (MKPlacemark placemark);

		[NoTV]
		[Export ("openInMapsWithLaunchOptions:"), Internal]
		bool _OpenInMaps ([NullAllowed] NSDictionary launchOptions);

		[NoTV]
		[Static]
		[Export ("openMapsWithItems:launchOptions:"), Internal]
		bool _OpenMaps (MKMapItem [] mapItems, [NullAllowed] NSDictionary launchOptions);

		[iOS (13, 2), NoMac, NoTV, NoWatch]
		[Introduced (PlatformName.MacCatalyst, 13, 2)]
		[Async]
		[Export ("openInMapsWithLaunchOptions:fromScene:completionHandler:")]
		void OpenInMaps ([NullAllowed] NSDictionary launchOptions, [NullAllowed] UIScene fromScene, [NullAllowed] Action<NSError> completionHandler);

		[iOS (13, 2), NoMac, NoTV, NoWatch]
		[Introduced (PlatformName.MacCatalyst, 13, 2)]
		[Static]
		[Async]
		[Export ("openMapsWithItems:launchOptions:fromScene:completionHandler:")]
		void OpenMaps (MKMapItem [] mapItems, [NullAllowed] NSDictionary launchOptions, [NullAllowed] UIScene fromScene, [NullAllowed] Action<NSError> completionHandler);

		[NoTV]
		[Field ("MKLaunchOptionsDirectionsModeKey"), Internal]
		NSString MKLaunchOptionsDirectionsModeKey { get; }

		[NoTV]
		[NoWatch]
		[Field ("MKLaunchOptionsMapTypeKey"), Internal]
		NSString MKLaunchOptionsMapTypeKey { get; }

		[NoTV]
		[Field ("MKLaunchOptionsMapCenterKey"), Internal]
		NSString MKLaunchOptionsMapCenterKey { get; }

		[NoTV]
		[Field ("MKLaunchOptionsMapSpanKey"), Internal]
		NSString MKLaunchOptionsMapSpanKey { get; }

		[NoTV]
		[NoWatch]
		[Field ("MKLaunchOptionsShowsTrafficKey"), Internal]
		NSString MKLaunchOptionsShowsTrafficKey { get; }

		[NoTV]
		[iOS (7,1)] // latest documentation says 7.1 and the field is not present in the simulator (7.0.3)
		[Mac (10,10)]
		[Field ("MKLaunchOptionsCameraKey"), Internal]
		NSString MKLaunchOptionsCameraKey { get; }

		[NoTV]
		[Field ("MKLaunchOptionsDirectionsModeDriving"), Internal]
		NSString MKLaunchOptionsDirectionsModeDriving { get; }

		[NoTV]
		[Field ("MKLaunchOptionsDirectionsModeWalking"), Internal]
		NSString MKLaunchOptionsDirectionsModeWalking { get; }

		[NoTV]
		[iOS (9,0)][Mac (10,11)]
		[Field ("MKLaunchOptionsDirectionsModeTransit"), Internal]
		NSString MKLaunchOptionsDirectionsModeTransit { get; }

		[NoTV]
		[iOS (10,0)][Mac (10,12)][Watch (3,0)]
		[Field ("MKLaunchOptionsDirectionsModeDefault"), Internal]
		NSString MKLaunchOptionsDirectionsModeDefault { get; }

		[Export ("timeZone")]
		[iOS (9,0), Mac(10,11)]
		[NullAllowed]
		NSTimeZone TimeZone { get; set; }

		[iOS (11,0), Mac (10,13), Watch (4,0), TV (11,0)]
		[Field ("MKMapItemTypeIdentifier")]
		NSString TypeIdentifier { get; }

		[TV (13, 0), NoWatch, Mac (10, 15), iOS (13, 0)]
		[NullAllowed, Export ("pointOfInterestCategory")]
		string PointOfInterestCategory { get; set; }
	}

	[NoWatch]
	[TV (9,2)]
	[BaseType (typeof (UIView), Delegates=new string [] { "WeakDelegate" }, Events=new Type [] {typeof (MKMapViewDelegate)})]
	[Mac (10,9)]
	interface MKMapView {
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[Export ("delegate", ArgumentSemantic.Weak)][NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		MKMapViewDelegate Delegate { get; set; }
	
		[Export ("mapType")]
		MKMapType MapType { get; set; }
	
		[Export ("region")]
		MKCoordinateRegion Region { get; set; }
	
		[Export ("setRegion:animated:")]
		void SetRegion (MKCoordinateRegion region, bool animated);
	
		[Export ("centerCoordinate")]
		CLLocationCoordinate2D CenterCoordinate { get; set; }
	
		[Export ("setCenterCoordinate:animated:")]
		void SetCenterCoordinate (CLLocationCoordinate2D coordinate, bool animated);
	
		[Export ("regionThatFits:")]
		MKCoordinateRegion RegionThatFits (MKCoordinateRegion region);
	
		[Export ("convertCoordinate:toPointToView:")]
		CGPoint ConvertCoordinate (CLLocationCoordinate2D coordinate, [NullAllowed] UIView toPointToView);
	
		[Export ("convertPoint:toCoordinateFromView:")]
		CLLocationCoordinate2D ConvertPoint (CGPoint point, [NullAllowed] UIView toCoordinateFromView);
	
		[Export ("convertRegion:toRectToView:")]
		CGRect ConvertRegion (MKCoordinateRegion region, [NullAllowed] UIView toRectToView);
	
		[Export ("convertRect:toRegionFromView:")]
		MKCoordinateRegion ConvertRect (CGRect rect, [NullAllowed] UIView toRegionFromView);
	
		[Export ("zoomEnabled")]
		bool ZoomEnabled { [Bind ("isZoomEnabled")] get; set; }
	
		[Export ("scrollEnabled")]
		bool ScrollEnabled { [Bind ("isScrollEnabled")] get; set; }
	
		[Export ("showsUserLocation")]
		bool ShowsUserLocation { get; set; }
	
		[Export ("userLocation")]
		MKUserLocation UserLocation { get; }
	
		[Export ("userLocationVisible")]
		bool UserLocationVisible { [Bind ("isUserLocationVisible")] get; }
	
		[Export ("addAnnotation:")]
		[PostGet ("Annotations")]
		void AddAnnotation (IMKAnnotation annotation);
	
		[Export ("addAnnotations:")]
		[PostGet ("Annotations")]
		void AddAnnotations ([Params] IMKAnnotation [] annotations);
	
		[Export ("removeAnnotation:")]
		[PostGet ("Annotations")]
		void RemoveAnnotation (IMKAnnotation annotation);
	
		[Export ("removeAnnotations:")]
		[PostGet ("Annotations")]
		void RemoveAnnotations ([Params] IMKAnnotation [] annotations);
	
		[Export ("annotations")]
		IMKAnnotation [] Annotations { get; }
	
		[Export ("viewForAnnotation:")]
		[return: NullAllowed]
		MKAnnotationView ViewForAnnotation (IMKAnnotation annotation);
	
		[Export ("dequeueReusableAnnotationViewWithIdentifier:")]
		[return: NullAllowed]
		MKAnnotationView DequeueReusableAnnotation (string withViewIdentifier);
	
		[TV (11,0)][iOS (11,0)][Mac (10,13)]
		[Export ("dequeueReusableAnnotationViewWithIdentifier:forAnnotation:")]
		MKAnnotationView DequeueReusableAnnotation (string identifier, IMKAnnotation annotation);

		[TV (11,0)][iOS (11,0)][Mac (10,13)]
		[Export ("registerClass:forAnnotationViewWithReuseIdentifier:")]
		void Register ([NullAllowed] Class viewClass, string identifier);

		[TV (11,0)][iOS (11,0)][Mac (10,13)]
		[Wrap ("Register (viewType is null ? null : new Class (viewType), identifier)")]
		void Register ([NullAllowed] Type viewType, string identifier);

		[Export ("selectAnnotation:animated:")]
		[PostGet ("SelectedAnnotations")]
		void SelectAnnotation (IMKAnnotation annotation, bool animated);
	
		[Export ("deselectAnnotation:animated:")]
		[PostGet ("SelectedAnnotations")]
		void DeselectAnnotation ([NullAllowed] IMKAnnotation annotation, bool animated);
	
		[Export ("selectedAnnotations", ArgumentSemantic.Copy)]
		IMKAnnotation [] SelectedAnnotations { get; set;	}
	
		[Export ("annotationVisibleRect")]
		CGRect AnnotationVisibleRect { get; }

		[Export ("addOverlay:")][PostGet ("Overlays")]
		void AddOverlay (IMKOverlay overlay);

		[Export ("addOverlays:")][PostGet ("Overlays")]
		void AddOverlays (IMKOverlay [] overlays);

		[Export ("removeOverlay:")][PostGet ("Overlays")]
		void RemoveOverlay (IMKOverlay overlay);

		[Export ("removeOverlays:")][PostGet ("Overlays")]
		void RemoveOverlays ([Params] IMKOverlay [] overlays);

		[Export ("overlays")]
		IMKOverlay [] Overlays { get;  }

		[Export ("insertOverlay:atIndex:")][PostGet ("Overlays")]
		void InsertOverlay (IMKOverlay overlay, nint index);

		[Export ("insertOverlay:aboveOverlay:")][PostGet ("Overlays")]
		void InsertOverlayAbove (IMKOverlay overlay, IMKOverlay sibling);

		[Export ("insertOverlay:belowOverlay:")][PostGet ("Overlays")]
		void InsertOverlayBelow (IMKOverlay overlay, IMKOverlay sibling);

		[Export ("exchangeOverlayAtIndex:withOverlayAtIndex:")]
		void ExchangeOverlays (nint index1, nint index2);

		[Export ("mapRectThatFits:")]
		MKMapRect MapRectThatFits (MKMapRect mapRect);

		[Export ("setVisibleMapRect:edgePadding:animated:")]
		void SetVisibleMapRect (MKMapRect mapRect, UIEdgeInsets edgePadding, bool animate);

		[Export ("setVisibleMapRect:animated:")]
		void SetVisibleMapRect (MKMapRect mapRect, bool animate);

		[Export ("mapRectThatFits:edgePadding:")]
		MKMapRect MapRectThatFits (MKMapRect mapRect, UIEdgeInsets edgePadding);

		[NoMac][NoTV]
		[Export ("viewForOverlay:")]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'MKOverlayRenderer.RendererForOverlay' instead.")]
		MKOverlayView ViewForOverlay (IMKOverlay overlay);

		[Export ("visibleMapRect")]
		MKMapRect VisibleMapRect { get; set;  }

		[Export ("annotationsInMapRect:")]
		NSSet GetAnnotations (MKMapRect mapRect);

		[Mac (11, 0)]
		[Export ("userTrackingMode")]
		MKUserTrackingMode UserTrackingMode { get; set; }
		
		[Mac (11, 0)]
		[Export ("setUserTrackingMode:animated:")]
		void SetUserTrackingMode (MKUserTrackingMode trackingMode, bool animated);

		[iOS (7,0), Export ("camera", ArgumentSemantic.Copy)]
		MKMapCamera Camera { get; set; }

		[iOS (7,0), Export ("setCamera:animated:")]
		void SetCamera (MKMapCamera camera, bool animated);

		[NoTV]
		[iOS (7,0), Export ("rotateEnabled")]
		bool RotateEnabled { [Bind ("isRotateEnabled")] get; set; }

		[NoTV]
		[iOS (7,0), Export ("pitchEnabled")]
		bool PitchEnabled { [Bind ("isPitchEnabled")] get; set; }

		[iOS (7,0), Export ("showAnnotations:animated:")]
		void ShowAnnotations (IMKAnnotation [] annotations, bool animated);

		[iOS (7,0), Export ("addOverlay:level:")]
		[PostGet ("Overlays")]
		void AddOverlay (IMKOverlay overlay, MKOverlayLevel level);

		[iOS (7,0), Export ("addOverlays:level:")]
		[PostGet ("Overlays")]
		void AddOverlays (IMKOverlay [] overlays, MKOverlayLevel level);

		[iOS (7,0), Export ("exchangeOverlay:withOverlay:")]
		[PostGet ("Overlays")]
		void ExchangeOverlay (IMKOverlay overlay1, IMKOverlay overlay2);

		[iOS (7,0), Export ("insertOverlay:atIndex:level:")]
		[PostGet ("Overlays")]
		void InsertOverlay (IMKOverlay overlay, nuint index, MKOverlayLevel level);

		[iOS (7,0), Export ("overlaysInLevel:")]
		IMKOverlay [] OverlaysInLevel (MKOverlayLevel level);

		[iOS (7,0), Export ("rendererForOverlay:")]
		[return: NullAllowed]
		MKOverlayRenderer RendererForOverlay (IMKOverlay overlay);

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'PointOfInterestFilter' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'PointOfInterestFilter' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'PointOfInterestFilter' instead.")]
		[iOS (7,0)]
		[Export ("showsPointsOfInterest")]
		bool ShowsPointsOfInterest { get; set; }

		[iOS (7,0)]
		[Export ("showsBuildings")]
		bool ShowsBuildings { get; set; }

		// MKMapView.h headers says "To be used in testing only" which means it's likely won't be accepted in the appstore
//		[iOS (9,0), Mac(10,11)]
//		[Export ("_handleSelectionAtPoint:")]
//		void _HandleSelectionAtPoint (CGPoint locationInView);

		[NoTV]
		[iOS(9,0)]
		[Export ("showsCompass")]
		bool ShowsCompass { get; set; }

		[Export ("showsScale")]
		[Mac (10,10), iOS(9,0)]
		bool ShowsScale { get; set; }

		[Export ("showsTraffic")]
		[Mac (10,11), iOS(9,0)]
		bool ShowsTraffic { get; set; }

		[NoiOS][NoWatch][NoTV][MacCatalyst (13,0)]
		[Export ("showsZoomControls")]
		bool ShowsZoomControls { get; set; }

		[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
		[Export ("setCameraZoomRange:animated:")]
		void SetCameraZoomRange ([NullAllowed] MKMapCameraZoomRange cameraZoomRange, bool animated);

		[TV (13, 0), NoWatch, Mac (10, 15), iOS (13, 0)]
		[Export ("cameraZoomRange", ArgumentSemantic.Copy)]
		[NullAllowed]
		MKMapCameraZoomRange CameraZoomRange { get; set; }

		[TV (13, 0), NoWatch, Mac (10, 15), iOS (13, 0)]
		[NullAllowed, Export ("cameraBoundary", ArgumentSemantic.Copy)]
		MKMapCameraBoundary CameraBoundary { get; set; }

		[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
		[Export ("setCameraBoundary:animated:")]
		void SetCameraBoundary ([NullAllowed] MKMapCameraBoundary cameraBoundary, bool animated);

		[TV (13, 0), NoWatch, Mac (10, 15), iOS (13, 0)]
		[NullAllowed, Export ("pointOfInterestFilter", ArgumentSemantic.Copy)]
		MKPointOfInterestFilter PointOfInterestFilter { get; set; }

		[Introduced (PlatformName.MacCatalyst, 14, 0)]
		[NoWatch, NoTV, NoiOS, Mac (11, 0)]
		[Export ("showsPitchControl")]
		bool ShowsPitchControl { get; set; }
	}

	[Static]
	[TV (11,0)][iOS (11,0)][Mac (10,13)]
	[NoWatch]
	interface MKMapViewDefault {
		[Field ("MKMapViewDefaultAnnotationViewReuseIdentifier")]
		NSString AnnotationViewReuseIdentifier { get; }

		[Field ("MKMapViewDefaultClusterAnnotationViewReuseIdentifier")]
		NSString ClusterAnnotationViewReuseIdentifier { get; }
	}

	[NoWatch]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	[Mac (10,9)]
	interface MKMapViewDelegate {
		[Export ("mapView:regionWillChangeAnimated:"), EventArgs ("MKMapViewChange")]
		void RegionWillChange (MKMapView mapView, bool animated);
	
		[Export ("mapView:regionDidChangeAnimated:"), EventArgs ("MKMapViewChange")]
		void RegionChanged (MKMapView mapView, bool animated);
	
		[Export ("mapViewWillStartLoadingMap:")]
		void WillStartLoadingMap (MKMapView mapView);
	
		[Export ("mapViewDidFinishLoadingMap:")]
		void MapLoaded (MKMapView mapView);
	
		[Export ("mapViewDidFailLoadingMap:withError:"), EventArgs ("NSError", true)]
		void LoadingMapFailed (MKMapView mapView, NSError error);
	
		[Export ("mapView:viewForAnnotation:"), DelegateName ("MKMapViewAnnotation"), DefaultValue (null)]
		[return: NullAllowed]
		MKAnnotationView GetViewForAnnotation (MKMapView mapView, IMKAnnotation annotation);
	
		[Export ("mapView:didAddAnnotationViews:"), EventArgs ("MKMapViewAnnotation")]
		void DidAddAnnotationViews (MKMapView mapView, MKAnnotationView [] views);
	
		[NoMac]
		[NoTV]
		[Export ("mapView:annotationView:calloutAccessoryControlTapped:"), EventArgs ("MKMapViewAccessoryTapped")]
		void CalloutAccessoryControlTapped (MKMapView mapView, MKAnnotationView view, UIControl control);

		[NoTV]
		[Export ("mapView:annotationView:didChangeDragState:fromOldState:"), EventArgs ("MKMapViewDragState")]
		void ChangedDragState (MKMapView mapView, MKAnnotationView annotationView, MKAnnotationViewDragState newState, MKAnnotationViewDragState oldState);

		[NoMac][NoTV]
		[Export ("mapView:viewForOverlay:"), DelegateName ("MKMapViewOverlay"), DefaultValue (null)]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'MKOverlayRenderer.RendererForOverlay' instead.")]
		MKOverlayView GetViewForOverlay (MKMapView mapView, IMKOverlay overlay);

		[NoMac][NoTV]
		[Export ("mapView:didAddOverlayViews:"), EventArgs ("MKOverlayViews")]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'DidAddOverlayRenderers' instead.")]
		void DidAddOverlayViews (MKMapView mapView, MKOverlayView overlayViews);

		[Export ("mapView:didSelectAnnotationView:"), EventArgs ("MKAnnotationView")]
		void DidSelectAnnotationView (MKMapView mapView, MKAnnotationView view);

		[Export ("mapView:didFailToLocateUserWithError:"), EventArgs ("NSError", true)]
		void DidFailToLocateUser (MKMapView mapView, NSError error);

		[Export ("mapView:didDeselectAnnotationView:"), EventArgs ("MKAnnotationView")]
		void DidDeselectAnnotationView (MKMapView mapView, MKAnnotationView view);

		[Export ("mapViewWillStartLocatingUser:")]
		void WillStartLocatingUser (MKMapView mapView);

		[Export ("mapViewDidStopLocatingUser:")]
		void DidStopLocatingUser (MKMapView mapView);

		[Export ("mapView:didUpdateUserLocation:"), EventArgs ("MKUserLocation")]
		void DidUpdateUserLocation (MKMapView mapView, MKUserLocation userLocation);

		[Mac (11, 0)]
		[Export ("mapView:didChangeUserTrackingMode:animated:"), EventArgs ("MMapViewUserTracking")]
		void DidChangeUserTrackingMode (MKMapView mapView, MKUserTrackingMode mode, bool animated);

		[iOS (7,0), Export ("mapView:rendererForOverlay:"), DelegateName ("MKRendererForOverlayDelegate"), DefaultValue (null)]
		MKOverlayRenderer OverlayRenderer (MKMapView mapView, IMKOverlay overlay);

		[iOS (7,0), Export ("mapView:didAddOverlayRenderers:"), EventArgs ("MKDidAddOverlayRenderers")]
		void DidAddOverlayRenderers (MKMapView mapView, MKOverlayRenderer [] renderers);

		[iOS (7,0), Export ("mapViewWillStartRenderingMap:")]
		void WillStartRenderingMap (MKMapView mapView);

		[iOS (7,0), Export ("mapViewDidFinishRenderingMap:fullyRendered:"), EventArgs ("MKDidFinishRenderingMap")]
		void DidFinishRenderingMap (MKMapView mapView, bool fullyRendered);

		[TV (11,0)][NoWatch][iOS (11,0)][Mac (10,13)]
		[Export ("mapView:clusterAnnotationForMemberAnnotations:"), DelegateName ("MKCreateClusterAnnotation"), DefaultValue (null)]
		MKClusterAnnotation CreateClusterAnnotation (MKMapView mapView, IMKAnnotation[] memberAnnotations);

		[TV (11,0)][NoWatch][iOS (11,0)][Mac (10,13)]
		[Export ("mapViewDidChangeVisibleRegion:")]
		void DidChangeVisibleRegion (MKMapView mapView);
	}

	[BaseType (typeof (MKAnnotationView))]
	// crash on Dispose when created from 'init'
	[DisableDefaultCtor]
	[TV (9,2)]
	[Mac (10,9)]
	[NoWatch]
	[Deprecated (PlatformName.MacOSX, 12, 0)]
	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.TvOS, 15, 0)]	
	interface MKPinAnnotationView {
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[Export ("initWithAnnotation:reuseIdentifier:")]
		NativeHandle Constructor ([NullAllowed] IMKAnnotation annotation, [NullAllowed] string reuseIdentifier);

		[NoTV]
		[Export ("pinColor")]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'PinTintColor' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'PinTintColor' instead.")]
		MKPinAnnotationColor PinColor { get; set; }
	
		[Export ("animatesDrop")]
		bool AnimatesDrop { get; set; }

		[iOS(9,0), Mac(10,11)]
		[Appearance]
		[Export ("pinTintColor")]
		[NullAllowed]
		UIColor PinTintColor { get; set; }

		[iOS(9,0), Mac(10,11)]
		[Static, Export ("redPinColor")]
		UIColor RedPinColor { get; }

		[iOS(9,0), Mac(10,11)]
		[Static, Export ("greenPinColor")]
		UIColor GreenPinColor { get; }

		[iOS(9,0), Mac(10,11)]
		[Static, Export ("purplePinColor")]
		UIColor PurplePinColor { get; }
	}

	// This requires the AddressBook framework, which afaict isn't bound on Mac, tvOS and watchOS yet
	[NoMac][NoMacCatalyst][NoWatch][NoTV]
	[StrongDictionary ("global::AddressBook.ABPersonAddressKey")]
	interface MKPlacemarkAddress {
		[Export ("City")]
		string City { get; set; }
		[Export ("Country")]
		string Country { get; set; }
		[Export ("CountryCode")]
		string CountryCode { get; set; }
		[Export ("State")]
		string State { get; set; }
		[Export ("Street")]
		string Street { get; set; }
		[Export ("Zip")]
		string Zip { get; set; }
	}

	[BaseType (typeof (CLPlacemark))]
	// crash (at least) when calling 'description' when instance is created by 'init'
	[DisableDefaultCtor]
	[TV (9,2)]
	[Mac (10,9)]
	interface MKPlacemark : MKAnnotation, NSCopying {
		[Export ("initWithCoordinate:addressDictionary:")]
		NativeHandle Constructor (CLLocationCoordinate2D coordinate, [NullAllowed] NSDictionary addressDictionary);

		// This requires the AddressBook framework, which afaict isn't bound on Mac, tvOS and watchOS yet
		[NoMac][NoMacCatalyst][NoWatch][NoTV]
		[Wrap ("this (coordinate, addressDictionary.GetDictionary ())")]
		NativeHandle Constructor (CLLocationCoordinate2D coordinate, MKPlacemarkAddress addressDictionary);

		[Watch (3,0)][TV (10,0)][iOS (10,0)]
		[Mac (10,12)]
		[Export ("initWithCoordinate:")]
		NativeHandle Constructor (CLLocationCoordinate2D coordinate);

		[Watch (3,0)][iOS (10,0)]
		[Mac (10,12)]
		[NoTV]
		[Export ("initWithCoordinate:postalAddress:")]
		NativeHandle Constructor (CLLocationCoordinate2D coordinate, CNPostalAddress postalAddress);
	
		[Export ("countryCode")]
		[NullAllowed]
		string CountryCode { get; }
	}
		
	[NoMac][NoMacCatalyst][NoWatch][NoTV]
	[BaseType (typeof (NSObject))]
	[Deprecated (PlatformName.iOS, 5, 0, message: "Use 'CoreLocation.CLGeocoder' instead.")]
	// crash (at least) at Dispose time when instance is created by 'init'
	[DisableDefaultCtor]
	interface MKReverseGeocoder {
		[Export ("initWithCoordinate:")]
		NativeHandle Constructor (CLLocationCoordinate2D coordinate);
	
		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		MKReverseGeocoderDelegate Delegate { get; set; }
	
		[Export ("coordinate")]
		CLLocationCoordinate2D coordinate { get; }
	
		[Export ("start")]
		void Start ();
	
		[Export ("querying")]
		bool Querying { [Bind ("isQuerying")] get; }
	
		[Export ("cancel")]
		void Cancel ();
	
		[Export ("placemark")]
		MKPlacemark Placemark { get; }
	}

#pragma warning disable 618
	[NoMac][NoMacCatalyst][NoWatch][NoTV]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface MKReverseGeocoderDelegate {
		[Abstract]
		[Export ("reverseGeocoder:didFailWithError:")]
		void FailedWithError (MKReverseGeocoder geocoder, NSError error);

		[Abstract]
		[Export ("reverseGeocoder:didFindPlacemark:")]
		void FoundWithPlacemark (MKReverseGeocoder geocoder, MKPlacemark placemark);
	}
#pragma warning restore 618

	[NoMac][NoWatch][NoTV]
	[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'MKOverlayRenderer' instead.")]
	[BaseType (typeof (UIView))]
	interface MKOverlayView {
		[Export ("overlay")]
		IMKOverlay Overlay { get; }

		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[DesignatedInitializer]
		[Export ("initWithOverlay:")]
		NativeHandle Constructor (IMKOverlay overlay);

		[Export ("pointForMapPoint:")]
		[ThreadSafe]
		CGPoint PointForMapPoint (MKMapPoint mapPoint);

		[Export ("mapPointForPoint:")]
		[ThreadSafe]
		MKMapPoint MapPointForPoint (CGPoint point);

		[Export ("rectForMapRect:")]
		[ThreadSafe]
		CGRect RectForMapRect (MKMapRect mapRect);

		[Export ("mapRectForRect:")]
		[ThreadSafe]
		MKMapRect MapRectForRect (CGRect rect);

		[Export ("canDrawMapRect:zoomScale:")]
		bool CanDrawMapRect (MKMapRect mapRect, /* MKZoomScale */ nfloat zoomScale);

		[Export ("drawMapRect:zoomScale:inContext:")]
		[ThreadSafe]
		void DrawMapRect (MKMapRect mapRect, /* MKZoomScale */ nfloat zoomScale, CGContext context);

		[Export ("setNeedsDisplayInMapRect:")]
		void SetNeedsDisplay (MKMapRect mapRect);

		[Export ("setNeedsDisplayInMapRect:zoomScale:")]
		void SetNeedsDisplay (MKMapRect mapRect, /* MKZoomScale */ nfloat zoomScale);
	}

	[NoMac][NoWatch][NoTV]
	[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'MKOverlayPathRenderer' instead.")]
	[BaseType (typeof (MKOverlayView))]
	interface MKOverlayPathView {
		[Export ("initWithOverlay:")]
		NativeHandle Constructor (IMKOverlay overlay);

		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[NullAllowed] // by default this property is null
		[Export ("fillColor", ArgumentSemantic.Retain)]
		UIColor FillColor { get; set;  }

		[NullAllowed] // by default this property is null
		[Export ("strokeColor", ArgumentSemantic.Retain)]
		UIColor StrokeColor { get; set;  }

		[Export ("lineWidth")]
		nfloat LineWidth { get; set;  }

		[Export ("lineJoin")]
		CGLineJoin LineJoin { get; set;  }

		[Export ("lineCap")]
		CGLineCap Linecap { get; set;  }

		[Export ("miterLimit")]
		nfloat MiterLimit  { get; set;  }

		[Export ("lineDashPhase")]
		nfloat LineDashPhase { get; set;  }

		[NullAllowed] // by default this property is null
		[Export ("lineDashPattern", ArgumentSemantic.Copy)]
		NSNumber [] LineDashPattern { get; set;  }

		[NullAllowed]
		[Export ("path")]
		CGPath Path { get; set;  }

		[Export ("createPath")]
		void CreatePath ();

		[Export ("invalidatePath")]
		void InvalidatePath ();

		[Export ("applyStrokePropertiesToContext:atZoomScale:")]
		void ApplyStrokeProperties (CGContext context, /* MKZoomScale */ nfloat zoomScale);

		[Export ("applyFillPropertiesToContext:atZoomScale:")]
		void ApplyFillProperties (CGContext context, /* MKZoomScale */ nfloat zoomScale);

		[Export ("strokePath:inContext:")]
		void StrokePath (CGPath path, CGContext context);

		[Export ("fillPath:inContext:")]
		void FillPath (CGPath path, CGContext context);
	}

	[NoWatch]
	[TV (9,2)]
	[Mac (10,9)]
	[BaseType (typeof (NSObject))]
	[Abstract]
	interface MKShape : MKAnnotation {
		[NullAllowed] // by default this property is null
		[Export ("title", ArgumentSemantic.Copy)]
		new string Title { get; set; }
	
		[NullAllowed] // by default this property is null
		[Export ("subtitle", ArgumentSemantic.Copy)]
		new string Subtitle { get; set; } 
	}

	[NoWatch]
	[TV (9,2)]
	[Mac (10,9)]
	[DesignatedDefaultCtor]
	[BaseType (typeof (MKShape))]
	interface MKPointAnnotation : MKGeoJsonObject {
		[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
		[Export ("initWithCoordinate:")]
		NativeHandle Constructor (CLLocationCoordinate2D coordinate);

		[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
		[Export ("initWithCoordinate:title:subtitle:")]
		NativeHandle Constructor (CLLocationCoordinate2D coordinate, [NullAllowed] string title, [NullAllowed] string subtitle);

		[Export ("coordinate")]
		CLLocationCoordinate2D Coordinate { get; set; }
	}

	[NoMac][NoTV][NoWatch]
	[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'MKPolygonRenderer' instead.")]
	[BaseType (typeof (MKOverlayPathView))]
	interface MKPolygonView {
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[Export ("initWithPolygon:")]
		[PostGet ("Polygon")]
		NativeHandle Constructor (MKPolygon polygon);
		
		[Export ("polygon")]
		MKPolygon Polygon { get;  }
	}

	[NoWatch]
	[ThreadSafe]
	[TV (9,2)]
	[Mac (10,9)]
	[BaseType (typeof (MKMultiPoint))]
	interface MKPolygon : MKOverlay, MKGeoJsonObject {
		[Export ("interiorPolygons")]
		[NullAllowed]
		MKPolygon [] InteriorPolygons { get;  }

		[Static]
		[Internal]
		[Export ("polygonWithPoints:count:")]
		MKPolygon _FromPoints (IntPtr points, nint count);

		[Static]
		[Internal]
		[Export ("polygonWithPoints:count:interiorPolygons:")]
		MKPolygon _FromPoints (IntPtr points, nint count, [NullAllowed] MKPolygon [] interiorPolygons);

		[Static]
		[Export ("polygonWithCoordinates:count:"), Internal]
		MKPolygon _FromCoordinates (IntPtr coords, nint count);

		[Static]
		[Internal]
		[Export ("polygonWithCoordinates:count:interiorPolygons:")]
		MKPolygon _FromCoordinates (IntPtr coords, nint count, [NullAllowed] MKPolygon [] interiorPolygons);

		#region MKAnnotation
		[Export ("coordinate")]
		CLLocationCoordinate2D Coordinate { get; }
		// note: setCoordinate: is not mandatory and is not implemented for MKPolygon (see unit tests)
		#endregion
	}

	[NoWatch]
	[ThreadSafe]
	[TV (9,2)]
	[Mac (10,9)]
	[BaseType (typeof (MKMultiPoint))]
	interface MKPolyline : MKOverlay, MKGeoJsonObject {
		[Static]
		[Export ("polylineWithCoordinates:count:")]
		[Internal]
		MKPolyline _FromCoordinates (IntPtr coords, nint count);
		
		[Static]
		[Internal]
		[Export ("polylineWithPoints:count:")]
		MKPolyline _FromPoints (IntPtr points, nint count);

		#region MKAnnotation
		[Export ("coordinate")]
		CLLocationCoordinate2D Coordinate { get; }
		// note: setCoordinate: is not mandatory and is not implemented for MKPolygon (see unit tests)
		#endregion
	}

	[NoMac][NoTV][NoWatch]
	[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'MKPolylineRenderer' instead.")]
	[BaseType (typeof (MKOverlayPathView))]
	interface MKPolylineView {
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[Export ("initWithPolyline:")]
		[PostGet ("Polyline")]
		NativeHandle Constructor (MKPolyline polyline);
		
		[Export ("polyline")]
		MKPolyline Polyline { get;  }
	}

	[NoWatch]
	[BaseType (typeof (MKShape))]
	[TV (9,2)]
	[Mac (10,9)]
	interface MKMultiPoint : MKGeoJsonObject {
		[Export ("points"), Internal]
		IntPtr _Points { get;  }

		[Export ("pointCount")]
		nint PointCount { get; }

		[Export ("getCoordinates:range:"), Internal]
		void GetCoords (IntPtr dest, NSRange range);

		[TV (14,0), NoWatch, Mac (11,0), iOS (14,0)]
		[MacCatalyst (14,0)]
		[Export ("locationAtPointIndex:")]
		nfloat GetLocation (nuint pointIndex);

		[TV (14,0), NoWatch, Mac (11,0), iOS (14,0)]
		[MacCatalyst (14,0)]
		[return: BindAs (typeof (nfloat []))]
		[Export ("locationsAtPointIndexes:")]
		NSNumber[] GetLocations (NSIndexSet indexes);
	}

	[NoWatch]
	[BaseType (typeof (NSObject))]
	[TV (9,2)]
	[Mac (10,9)]
	interface MKUserLocation : IMKAnnotation { // This is wrong. It should be MKAnnotation but we can't due to API compat. When you fix this remove hack in generator.cs to enable warning again
		[Export ("updating")]
		bool Updating { [Bind ("isUpdating")] get; }
		
		[Export ("coordinate")]
		CLLocationCoordinate2D Coordinate { get; set; }
		
		[Export ("location", ArgumentSemantic.Retain)]
		[NullAllowed]
		CLLocation Location { get; }

		[Export ("title", ArgumentSemantic.Copy)]
		[NullAllowed]
		string Title { get; set; }
		
		[NullAllowed] // by default this property is null
		[Export ("subtitle", ArgumentSemantic.Copy)]
		string Subtitle { get; set; }
		
		[NoTV]
		[Export ("heading", ArgumentSemantic.Retain)]
		[NullAllowed]
		CLHeading Heading { get; }
	}

	[NoMac]
	[NoWatch]
	[NoTV]
	[BaseType (typeof (UIBarButtonItem))]
	[DisableDefaultCtor]
	interface MKUserTrackingBarButtonItem {
		[NullAllowed] // by default this property is null
		[Export ("mapView", ArgumentSemantic.Retain)]
		MKMapView MapView { get; set;  }

		[DesignatedInitializer]
		[Export ("initWithMapView:")]
		[PostGet ("MapView")]
		NativeHandle Constructor ([NullAllowed] MKMapView mapView);
	}

	delegate void MKLocalSearchCompletionHandler (MKLocalSearchResponse response, NSError error);

	[NoWatch]
	[TV (9,2)]
	[Mac (10,9)]
	[BaseType (typeof (NSObject))]
	[ThreadSafe]
	[DisableDefaultCtor] // crash on iOS8 beta
	interface MKLocalSearch {

		[DesignatedInitializer]
		[Export ("initWithRequest:")]
		NativeHandle Constructor (MKLocalSearchRequest request);

		[TV (14,0), NoWatch, Mac (11,0), iOS (14,0)]
		[MacCatalyst (14,0)]
		[Export ("initWithPointsOfInterestRequest:")]
		[DesignatedInitializer]
		NativeHandle Constructor (MKLocalPointsOfInterestRequest request);

		[Export ("startWithCompletionHandler:")]
		[Async]
		void Start (MKLocalSearchCompletionHandler completionHandler);

		[Export ("cancel")]
		void Cancel ();

		[Export ("searching")]
		bool IsSearching { [Bind ("isSearching")] get; }
	}

	[NoWatch]
	[TV (9,2)]
	[Mac (10,9)]
	[BaseType (typeof (NSObject))]
	[ThreadSafe]
	[DesignatedDefaultCtor]
	interface MKLocalSearchRequest : NSCopying {

		[DesignatedInitializer]
		[NoWatch][iOS (9,3)][Mac (10,11,4)]
		[Export ("initWithCompletion:")]
		NativeHandle Constructor (MKLocalSearchCompletion completion);

		[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
		[Export ("initWithNaturalLanguageQuery:")]
		NativeHandle Constructor (string naturalLanguageQuery);

		[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
		[Export ("initWithNaturalLanguageQuery:region:")]
		NativeHandle Constructor (string naturalLanguageQuery, MKCoordinateRegion region);

		[Export ("naturalLanguageQuery", ArgumentSemantic.Copy)]
		[NullAllowed]
		string NaturalLanguageQuery { get; set; }

		[Export ("region", ArgumentSemantic.Assign)]
		MKCoordinateRegion Region { get; set; }

		[TV (13, 0), NoWatch, Mac (10, 15), iOS (13, 0)]
		[Export ("resultTypes", ArgumentSemantic.Assign)]
		MKLocalSearchResultType ResultTypes { get; set; }

		[TV (13, 0), NoWatch, Mac (10, 15), iOS (13, 0)]
		[NullAllowed, Export ("pointOfInterestFilter", ArgumentSemantic.Copy)]
		MKPointOfInterestFilter PointOfInterestFilter { get; set; }
	}

	[NoWatch]
	[TV (9,2)]
	[Mac (10,9)]
	[BaseType (typeof (NSObject))]
	[ThreadSafe]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: *** setObjectForKey: object cannot be nil (key: mapItems)
	[DisableDefaultCtor]
	interface MKLocalSearchResponse {

		[Export ("boundingRegion")]
		MKCoordinateRegion Region { get; }

		[Export ("mapItems")]
		MKMapItem[] MapItems { get; }
	}

	[NoWatch]
	[TV (9,2)]
	[iOS (7,0), BaseType (typeof (MKOverlayPathRenderer))]
	[Mac (10,9)]
	partial interface MKCircleRenderer {

		[Export ("initWithCircle:")]
		NativeHandle Constructor (MKCircle circle);

		[Export ("circle")]
		MKCircle Circle { get; }

		[TV (14, 0), NoWatch, Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14,0)]
		[Export ("strokeStart")]
		nfloat StrokeStart { get; set; }

		[TV (14, 0), NoWatch, Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14,0)]
		[Export ("strokeEnd")]
		nfloat StrokeEnd { get; set; }
	}

	[NoWatch]
	[TV (9,2)]
	[iOS (7,0), BaseType (typeof (NSObject))]
	[Mac (10,9)]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: Cannot initialize MKDirections with nil request
	partial interface MKDirections {

		[DesignatedInitializer]
		[Export ("initWithRequest:")]
		NativeHandle Constructor (MKDirectionsRequest request);

		[Export ("calculateDirectionsWithCompletionHandler:")]
		[Async]
		void CalculateDirections (MKDirectionsHandler completionHandler);

		[Export ("cancel")]
		void Cancel ();

		[Export ("calculating")]
		bool Calculating { [Bind ("isCalculating")] get; }

		[Export ("calculateETAWithCompletionHandler:")]
		[Async]
		void CalculateETA (MKETAHandler completionHandler);
	}

	delegate void MKDirectionsHandler (MKDirectionsResponse response, NSError error);

	delegate void MKETAHandler (MKETAResponse response, NSError error);

	[NoWatch]
	[TV (9,2)]
	[iOS (7,0), BaseType (typeof (NSObject))]
	[Mac (10,9)]
	partial interface MKETAResponse {
		[Export ("source")]
		MKMapItem Source { get; }

		[Export ("destination")]
		MKMapItem Destination { get; }

		[Export ("expectedTravelTime")]
		double ExpectedTravelTime { get; }

		[iOS (9,0)][Mac (10,11)]
		[Export ("distance")]
		double /* CLLocationDistance */ Distance { get; }

		[Export ("transportType")]
		[iOS (9,0), Mac(10,11)]
		MKDirectionsTransportType TransportType { get; }

		[Export ("expectedArrivalDate")]
		[iOS (9,0), Mac(10,11)]
		NSDate ExpectedArrivalDate { get; }

		[Export ("expectedDepartureDate")]
		[iOS (9,0), Mac(10,11)]
		NSDate ExpectedDepartureDate { get; }
	}

	[NoWatch]
	[TV (9,2)]
	[iOS (7,0), BaseType (typeof (NSObject))]
	[Mac (10,9)]
	partial interface MKDirectionsResponse {

		[Export ("source")]
		MKMapItem Source { get; }

		[Export ("destination")]
		MKMapItem Destination { get; }

		[Export ("routes")]
		MKRoute [] Routes { get; }
	}

	[NoWatch]
	[TV (9,2)]
	[iOS (7,0), BaseType (typeof (NSObject))]
	[Mac (10,9)]
	partial interface MKRoute {

		[Export ("name")]
		string Name { get; }

		[Export ("advisoryNotices")]
		string [] AdvisoryNotices { get; }

		[Export ("distance")]
		double Distance { get; }

		[Export ("expectedTravelTime")]
		double ExpectedTravelTime { get; }

		[Export ("transportType")]
		MKDirectionsTransportType TransportType { get; }

		[Export ("polyline")]
		MKPolyline Polyline { get; }

		[Export ("steps")]
		MKRouteStep [] Steps { get; }
	}

	[NoWatch]
	[TV (9,2)]
	[iOS (7,0), BaseType (typeof (NSObject))]
	[Mac (10,9)]
	partial interface MKRouteStep {

		[Export ("instructions")]
		string Instructions { get; }

		[Export ("notice")]
		[NullAllowed]
		string Notice { get; }

		[Export ("polyline")]
		MKPolyline Polyline { get; }

		[Export ("distance")]
		double Distance { get; }

		[Export ("transportType")]
		MKDirectionsTransportType TransportType { get; }
	}

	[TV (9,2)]
	[iOS (7,0), BaseType (typeof (NSFormatter))]
	[Mac (10,9)]
	partial interface MKDistanceFormatter {

		[Export ("stringFromDistance:")]
		string StringFromDistance (double distance);

		[Export ("distanceFromString:")]
		double DistanceFromString (string distance);

		[Export ("locale", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSLocale Locale { get; set; }

		[Export ("units", ArgumentSemantic.Assign)]
		MKDistanceFormatterUnits Units { get; set; }

		[Export ("unitStyle", ArgumentSemantic.Assign)]
		MKDistanceFormatterUnitStyle UnitStyle { get; set; }
	}

	[NoWatch]
	[TV (9,2)]
	[iOS (7,0), BaseType (typeof (MKPolyline))]
	[Mac (10,9)]
	partial interface MKGeodesicPolyline {

		[Static, Export ("polylineWithPoints:count:")]
		[Internal]
		MKGeodesicPolyline PolylineWithPoints (IntPtr points, nint count);

		[Static, Export ("polylineWithCoordinates:count:")]
		[Internal]
		MKGeodesicPolyline PolylineWithCoordinates (IntPtr coords, nint count);
	}

	[NoWatch]
	[TV (9,2)]
	[iOS (7,0), BaseType (typeof (NSObject))]
	[Mac (10,9)]
	partial interface MKMapCamera : NSCopying, NSSecureCoding {

		[Export ("centerCoordinate")]
		CLLocationCoordinate2D CenterCoordinate { get; set; }

		[Export ("heading")]
		double Heading { get; set; }

		[Export ("pitch")]
		nfloat Pitch { get; set; }

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'CenterCoordinateDistance' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'CenterCoordinateDistance' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'CenterCoordinateDistance' instead.")]
		[Export ("altitude")]
		double Altitude { get; set; }

		[Static, Export ("camera")]
		MKMapCamera Camera { get; }

		[Static, Export ("cameraLookingAtCenterCoordinate:fromEyeCoordinate:eyeAltitude:")]
		MKMapCamera CameraLookingAtCenterCoordinate (CLLocationCoordinate2D centerCoordinate, CLLocationCoordinate2D eyeCoordinate, double eyeAltitude);

		[Static]
		[iOS(9,0)][Mac(10,11)]
		[Export ("cameraLookingAtCenterCoordinate:fromDistance:pitch:heading:")]
		MKMapCamera CameraLookingAtCenterCoordinate (CLLocationCoordinate2D centerCoordinate, double locationDistance, nfloat pitch, double locationDirectionHeading);
		
		[TV (13, 0), NoWatch, Mac (10, 15), iOS (13, 0)]
		[Export ("centerCoordinateDistance")]
		double CenterCoordinateDistance { get; set; }
	}

	[NoWatch]
	[TV (9,2)]
	[iOS (7,0), BaseType (typeof (NSObject))]
	[Mac (10,9)]
	partial interface MKMapSnapshot {

		[Export ("image")]
		UIImage Image { get; }

		[Export ("pointForCoordinate:")]
		CGPoint PointForCoordinate (CLLocationCoordinate2D coordinate);

		[NoWatch][NoTV][NoiOS]
		[Mac (10,14)]
		[Export ("appearance")]
		NSAppearance Appearance { get; }

		[TV (13, 0), NoWatch, NoMac, iOS (13, 0)]
		[Export ("traitCollection")]
		UITraitCollection TraitCollection { get; }
	}

	[NoWatch]
	[TV (9,2)]
	[iOS (7,0), BaseType (typeof (NSObject))]
	[Mac (10,9)]
	partial interface MKMapSnapshotOptions : NSCopying {

		[Export ("camera", ArgumentSemantic.Copy)]
		MKMapCamera Camera { get; set; }

		[Export ("mapRect", ArgumentSemantic.Assign)]
		MKMapRect MapRect { get; set; }

		[Export ("region", ArgumentSemantic.Assign)]
		MKCoordinateRegion Region { get; set; }

		[Export ("mapType", ArgumentSemantic.Assign)]
		MKMapType MapType { get; set; }

		[Export ("size", ArgumentSemantic.Assign)]
		CGSize Size { get; set; }

		[NoMac]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'TraitCollection.DisplayScale' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'TraitCollection.DisplayScale' instead.")]
		[Export ("scale", ArgumentSemantic.Assign)]
		nfloat Scale { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'PointOfInterestFilter' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'PointOfInterestFilter' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'PointOfInterestFilter' instead.")]
		[Export ("showsPointsOfInterest")]
		bool ShowsPointsOfInterest { get; set; }

		[Export ("showsBuildings")]
		bool ShowsBuildings { get; set; }

		[NoWatch][NoTV][NoiOS][NoMacCatalyst]
		[Mac (10,14)]
		[NullAllowed, Export ("appearance", ArgumentSemantic.Strong)]
		NSAppearance Appearance { get; set; }

		[TV (13, 0), NoWatch, Mac (10, 15), iOS (13, 0)]
		[NullAllowed, Export ("pointOfInterestFilter", ArgumentSemantic.Copy)]
		MKPointOfInterestFilter PointOfInterestFilter { get; set; }

		[TV (13, 0), NoWatch, NoMac, iOS (13, 0)]
		[Export ("traitCollection", ArgumentSemantic.Copy)]
		UITraitCollection TraitCollection { get; set; }
	}

	[NoWatch]
	[TV (9,2)]
	[iOS (7,0), BaseType (typeof (NSObject))]
	[Mac (10,9)]
	partial interface MKMapSnapshotter {

		[DesignatedInitializer]
		[Export ("initWithOptions:")]
		NativeHandle Constructor (MKMapSnapshotOptions options);

		[Export ("startWithCompletionHandler:")]
		[Async]
		void Start (MKMapSnapshotCompletionHandler completionHandler);

		[Export ("startWithQueue:completionHandler:")]
		[Async]
		void Start (DispatchQueue queue, MKMapSnapshotCompletionHandler completionHandler);

		[Export ("cancel")]
		void Cancel ();

		[Export ("loading")]
		bool Loading { [Bind ("isLoading")] get; }
	}

	delegate void MKMapSnapshotCompletionHandler (MKMapSnapshot snapshot, NSError error);

	[NoWatch]
	[TV (9,2)]
	[iOS (7,0), BaseType (typeof (MKOverlayRenderer))]
	[Mac (10,9)]
	[ThreadSafe]
	partial interface MKOverlayPathRenderer {

		[Export ("initWithOverlay:")]
		NativeHandle Constructor (IMKOverlay overlay);

		[NullAllowed] // by default this property is null
		[Export ("fillColor", ArgumentSemantic.Retain)]
		UIColor FillColor { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("strokeColor", ArgumentSemantic.Retain)]
		UIColor StrokeColor { get; set; }

		[Export ("lineWidth")]
		nfloat LineWidth { get; set; }

		[Export ("lineJoin")]
		CGLineJoin LineJoin { get; set; }

		[Export ("lineCap")]
		CGLineCap LineCap { get; set; }

		[Export ("miterLimit")]
		nfloat MiterLimit { get; set; }

		[Export ("lineDashPhase")]
		nfloat LineDashPhase { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("lineDashPattern", ArgumentSemantic.Copy)]
		NSNumber [] LineDashPattern { get; set; }

		[Export ("createPath")]
		void CreatePath ();

		[NullAllowed]
		[Export ("path")]
		CGPath Path { get; set; }

		[Export ("invalidatePath")]
		void InvalidatePath ();

		[Export ("applyStrokePropertiesToContext:atZoomScale:")]
		void ApplyStrokePropertiesToContext (CGContext context, /* MKZoomScale */ nfloat zoomScale);

		[Export ("applyFillPropertiesToContext:atZoomScale:")]
		void ApplyFillPropertiesToContext (CGContext context, /* MKZoomScale */ nfloat zoomScale);

		[Export ("strokePath:inContext:")]
		void StrokePath (CGPath path, CGContext context);

		[Export ("fillPath:inContext:")]
		void FillPath (CGPath path, CGContext context);

		[TV (13, 0), NoWatch, Mac (10, 15), iOS (13, 0)]
		[Export ("shouldRasterize")]
		bool ShouldRasterize { get; set; }
	}

	[NoWatch]
	[TV (9,2)]
	[iOS (7,0), BaseType (typeof (NSObject))]
	[Mac (10,9)]
	partial interface MKOverlayRenderer {

		[DesignatedInitializer]
		[Export ("initWithOverlay:")]
		NativeHandle Constructor (IMKOverlay overlay);

		[Export ("overlay")]
		IMKOverlay Overlay { get; }

		[ThreadSafe]
		[Export ("pointForMapPoint:")]
		CGPoint PointForMapPoint (MKMapPoint mapPoint);

		[ThreadSafe]
		[Export ("mapPointForPoint:")]
		MKMapPoint MapPointForPoint (CGPoint point);

		[ThreadSafe]
		[Export ("rectForMapRect:")]
		CGRect RectForMapRect (MKMapRect mapRect);

		[ThreadSafe]
		[Export ("mapRectForRect:")]
		MKMapRect MapRectForRect (CGRect rect);

		[Export ("canDrawMapRect:zoomScale:")]
		bool CanDrawMapRect (MKMapRect mapRect, /* MKZoomScale */ nfloat zoomScale);

		[ThreadSafe]
		[Export ("drawMapRect:zoomScale:inContext:")]
		void DrawMapRect (MKMapRect mapRect, /* MKZoomScale */ nfloat zoomScale, CGContext context);

		[Export ("setNeedsDisplay")]
		void SetNeedsDisplay ();

		[Export ("setNeedsDisplayInMapRect:")]
		void SetNeedsDisplay (MKMapRect mapRect);

		[Export ("setNeedsDisplayInMapRect:zoomScale:")]
		void SetNeedsDisplay (MKMapRect mapRect, /* MKZoomScale */ nfloat zoomScale);

		[Export ("alpha")]
		nfloat Alpha { get; set; }

		[Export ("contentScaleFactor")]
		nfloat ContentScaleFactor { get; }
	}

	[NoWatch]
	[TV (9,2)]
	[iOS (7,0), BaseType (typeof (MKOverlayPathRenderer))]
	[Mac (10,9)]
	partial interface MKPolygonRenderer {

		[Export ("initWithPolygon:")]
		NativeHandle Constructor (MKPolygon polygon);

		[Export ("polygon")]
		MKPolygon Polygon { get; }

		[TV (14, 0), NoWatch, Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14,0)]
		[Export ("strokeStart")]
		nfloat StrokeStart { get; set; }

		[TV (14, 0), NoWatch, Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14,0)]
		[Export ("strokeEnd")]
		nfloat StrokeEnd { get; set; }
	}

	[NoWatch]
	[TV (9,2)]
	[iOS (7,0), BaseType (typeof (MKOverlayPathRenderer))]
	[Mac (10,9)]
	partial interface MKPolylineRenderer {

		[Export ("initWithPolyline:")]
		NativeHandle Constructor (MKPolyline polyline);

		[Export ("polyline")]
		MKPolyline Polyline { get; }

		[TV (14, 0), NoWatch, Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14,0)]
		[Export ("strokeStart")]
		nfloat StrokeStart { get; set; }

		[TV (14, 0), NoWatch, Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14,0)]
		[Export ("strokeEnd")]
		nfloat StrokeEnd { get; set; }
	}

	[NoWatch]
	[TV (14, 0), NoWatch, Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14,0)]
	[BaseType (typeof (MKPolylineRenderer))]
	partial interface MKGradientPolylineRenderer
	{
		[Export ("locations", ArgumentSemantic.Copy)]
		[BindAs (typeof (nfloat[]))]
		NSNumber[] Locations { get; }

		[Export ("colors", ArgumentSemantic.Copy)]
		UIColor[] Colors { get; }

		[Export ("setColors:atLocations:")]
		void SetColors (UIColor[] colors, [BindAs (typeof (nfloat[]))]NSNumber[] locations);
	}

	[NoWatch]
	[ThreadSafe]
	[TV (9,2)]
	[Mac (10,9)]
	[iOS (7,0), BaseType (typeof (NSObject))]
	partial interface MKTileOverlay : MKOverlay {
		[DesignatedInitializer]
		[Export ("initWithURLTemplate:")]
		NativeHandle Constructor ([NullAllowed] string URLTemplate);

		[Export ("tileSize")]
		CGSize TileSize { get; set; }

		[Export ("geometryFlipped")]
		bool GeometryFlipped { [Bind ("isGeometryFlipped")] get; set; }

		[Export ("minimumZ")]
		nint MinimumZ { get; set; }

		[Export ("maximumZ")]
		nint MaximumZ { get; set; }

		[Export ("URLTemplate")]
		[NullAllowed]
		string URLTemplate { get; }

		[Export ("canReplaceMapContent")]
		new bool CanReplaceMapContent { get; set; }

		[Export ("URLForTilePath:")]
		NSUrl URLForTilePath (MKTileOverlayPath path);

		[Export ("loadTileAtPath:result:")]
		void LoadTileAtPath (MKTileOverlayPath path, MKTileOverlayLoadTileCompletionHandler result);

		[Export ("coordinate")]
		CLLocationCoordinate2D Coordinate { get; }
	}

	delegate void MKTileOverlayLoadTileCompletionHandler (NSData tileData, NSError error);

	[NoWatch]
	[TV (9,2)]
	[iOS (7,0), BaseType (typeof (MKOverlayRenderer))]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: Expected a MKTileOverlay but got (null)
	[DisableDefaultCtor] // throw in iOS8 beta 1 ^
	[Mac (10,9)]
	partial interface MKTileOverlayRenderer {
		// This ctor is not allowed: NSInvalidArgumentEception Expected a MKTileOverlay
//		[Export ("initWithOverlay:")]
//		NativeHandle Constructor (IMKOverlay toverlay);

		[Export ("initWithTileOverlay:")]
		NativeHandle Constructor (MKTileOverlay overlay);

		[Export ("reloadData")]
		void ReloadData ();
	}

	[NoWatch]
	[TV (9,2)][NoWatch][iOS (9,3)][Mac(10,11,4)]
	[BaseType (typeof (NSObject))]
	interface MKLocalSearchCompleter {
		[Export ("queryFragment")]
		string QueryFragment { get; set; }

		[Export ("region", ArgumentSemantic.Assign)]
		MKCoordinateRegion Region { get; set; }

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'ResultTypes' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'ResultTypes' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'ResultTypes' instead.")]
		[Export ("filterType", ArgumentSemantic.Assign)]
		MKSearchCompletionFilterType FilterType { get; set; }

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		[Protocolize]
		MKLocalSearchCompleterDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Export ("results", ArgumentSemantic.Strong)]
		MKLocalSearchCompletion[] Results { get; }

		[Export ("searching")]
		bool Searching { [Bind ("isSearching")] get; }

		[Export ("cancel")]
		void Cancel ();

		[TV (13, 0), NoWatch, Mac (10, 15), iOS (13, 0)]
		[Export ("resultTypes", ArgumentSemantic.Assign)]
		MKLocalSearchCompleterResultType ResultTypes { get; set; }

		[TV (13, 0), NoWatch, Mac (10, 15), iOS (13, 0)]
		[NullAllowed, Export ("pointOfInterestFilter", ArgumentSemantic.Copy)]
		MKPointOfInterestFilter PointOfInterestFilter { get; set; }
	}

	[NoWatch]
	[TV (9,2)][NoWatch][iOS (9,3)]
	[Protocol]
	[Model]
	[BaseType (typeof (NSObject))]
	interface MKLocalSearchCompleterDelegate {
		[Export ("completerDidUpdateResults:")]
		void DidUpdateResults (MKLocalSearchCompleter completer);

		[Export ("completer:didFailWithError:")]
		void DidFail (MKLocalSearchCompleter completer, NSError error);
	}

	[NoWatch]
	[TV (9,2)][NoWatch][iOS (9,3)]
	[BaseType (typeof(NSObject))]
#if MONOMAC || XAMCORE_3_0 // "You do not create instances of this class directly"
	[DisableDefaultCtor]
#endif
	interface MKLocalSearchCompletion {
		[Export ("title", ArgumentSemantic.Strong)]
		string Title { get; }

		// NSValue-wrapped NSRanges
		[Export ("titleHighlightRanges", ArgumentSemantic.Strong)]
		NSValue[] TitleHighlightRanges { get; }

		[Export ("subtitle", ArgumentSemantic.Strong)]
		string Subtitle { get; }

		// NSValue-wrapped NSRanges
		[Export ("subtitleHighlightRanges", ArgumentSemantic.Strong)]
		NSValue[] SubtitleHighlightRanges { get; }
	}

	[Category]
	[BaseType (typeof (NSUserActivity))]
	interface NSUserActivity_MKMapItem {
		[Watch (3,0)][TV (10,0)][iOS (10,0)][Mac (10,12)]
		[Export ("mapItem")]
		MKMapItem GetMapItem ();

		[Watch (3,0)][TV (10,0)][iOS (10,0)][Mac (10,12)]
		[Export ("setMapItem:")]
		void SetMapItem (MKMapItem item);
	}

[TV (11,0)][NoWatch][iOS (11,0)][Mac (10,13)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MKClusterAnnotation : MKAnnotation {
		[NullAllowed, Export ("title")]
		new string Title { get; set; }

		[NullAllowed, Export ("subtitle")]
		new string Subtitle { get; set; }

		[Export ("memberAnnotations")]
		IMKAnnotation[] MemberAnnotations { get; }

		[Export ("initWithMemberAnnotations:")]
		[DesignatedInitializer]
		NativeHandle Constructor (IMKAnnotation[] memberAnnotations);
	}

	[NoTV][iOS (11,0)][Mac (11, 0)][NoWatch]
	[BaseType (typeof (UIView))]
	[DisableDefaultCtor]
	interface MKCompassButton {
		[Static]
		[Export ("compassButtonWithMapView:")]
		MKCompassButton FromMapView ([NullAllowed] MKMapView mapView);

		[NullAllowed, Export ("mapView", ArgumentSemantic.Weak)]
		MKMapView MapView { get; set; }

		[Export ("compassVisibility", ArgumentSemantic.Assign)]
		MKFeatureVisibility CompassVisibility { get; set; }
	}

	[TV (11,0)][NoWatch][iOS (11,0)][Mac (11, 0)]
	[BaseType (typeof (MKAnnotationView))]
	interface MKMarkerAnnotationView {

		// inlined from base type
		[Export ("initWithAnnotation:reuseIdentifier:")]
		[PostGet ("Annotation")]
		NativeHandle Constructor ([NullAllowed] IMKAnnotation annotation, [NullAllowed] string reuseIdentifier);

		[Export ("titleVisibility", ArgumentSemantic.Assign)]
		MKFeatureVisibility TitleVisibility { get; set; }

		[Export ("subtitleVisibility", ArgumentSemantic.Assign)]
		MKFeatureVisibility SubtitleVisibility { get; set; }

		[Appearance]
		[NullAllowed, Export ("markerTintColor", ArgumentSemantic.Copy)]
		UIColor MarkerTintColor { get; set; }

		[Appearance]
		[NullAllowed, Export ("glyphTintColor", ArgumentSemantic.Copy)]
		UIColor GlyphTintColor { get; set; }

		[Appearance]
		[NullAllowed, Export ("glyphText")]
		string GlyphText { get; set; }

		[Appearance]
		[NullAllowed, Export ("glyphImage", ArgumentSemantic.Copy)]
		UIImage GlyphImage { get; set; }

		[Appearance]
		[NullAllowed, Export ("selectedGlyphImage", ArgumentSemantic.Copy)]
		UIImage SelectedGlyphImage { get; set; }

		[Export ("animatesWhenAdded")]
		bool AnimatesWhenAdded { get; set; }
	}

	[TV (11,0)][NoWatch][iOS (11,0)][NoMac]
	[BaseType (typeof (UIView))]
	[DisableDefaultCtor]
	interface MKScaleView {

		[Static]
		[Export ("scaleViewWithMapView:")]
		MKScaleView FromMapView ([NullAllowed] MKMapView mapView);

		[NullAllowed, Export ("mapView", ArgumentSemantic.Weak)]
		MKMapView MapView { get; set; }

		[Export ("scaleVisibility", ArgumentSemantic.Assign)]
		MKFeatureVisibility ScaleVisibility { get; set; }

		[Export ("legendAlignment", ArgumentSemantic.Assign)]
		MKScaleViewAlignment LegendAlignment { get; set; }
	}

	[NoTV][iOS (11,0)][NoWatch][NoMac]
	[BaseType (typeof (UIView))]
	[DisableDefaultCtor]
	interface MKUserTrackingButton {
		[Static]
		[Export ("userTrackingButtonWithMapView:")]
		MKUserTrackingButton FromMapView ([NullAllowed] MKMapView mapView);

		[NullAllowed, Export ("mapView", ArgumentSemantic.Weak)]
		MKMapView MapView { get; set; }
	}

#if WATCH
	interface MKPointOfInterestCategory {}
#endif 

	[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
	[BaseType (typeof(NSObject))]
	interface MKPointOfInterestFilter : NSSecureCoding, NSCopying
	{
		[Static]
		[Export ("filterIncludingAllCategories")]
		MKPointOfInterestFilter FilterIncludingAllCategories { get; }

		[Static]
		[Export ("filterExcludingAllCategories")]
		MKPointOfInterestFilter FilterExcludingAllCategories { get; }

		[Internal]
		[Export ("initIncludingCategories:")]
		IntPtr InitIncludingCategories ([BindAs (typeof (MKPointOfInterestCategory[]))] NSString [] categories);

		[Internal]
		[Export ("initExcludingCategories:")]
		IntPtr InitExcludingCategories ([BindAs (typeof (MKPointOfInterestCategory[]))] NSString [] categories);

		[Export ("includesCategory:")]
		bool IncludesCategory ([BindAs (typeof (MKPointOfInterestCategory))] NSString category);

		[Export ("excludesCategory:")]
		bool ExcludesCategory ([BindAs (typeof (MKPointOfInterestCategory))] NSString category);
	}

	[TV (13, 0), NoWatch, Mac (10, 15), iOS (13, 0)]
	[Protocol (Name = "MKGeoJSONObject")]
	interface MKGeoJsonObject {}

	interface IMKGeoJsonObject {}

	[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
	[BaseType (typeof(NSObject), Name = "MKGeoJSONDecoder")]
	interface MKGeoJsonDecoder
	{
		[Export ("geoJSONObjectsWithData:error:")]
		[return: NullAllowed]
		IMKGeoJsonObject[] GeoJsonObjects (NSData data, [NullAllowed] out NSError error);
	}

	[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
	[BaseType (typeof(NSObject), Name = "MKGeoJSONFeature")]
	interface MKGeoJsonFeature : MKGeoJsonObject
	{
		[NullAllowed, Export ("identifier")]
		string Identifier { get; }

		[NullAllowed, Export ("properties")]
		NSData Properties { get; }

		[Export ("geometry")]
		IMKGeoJsonObject[] Geometry { get; }
	}

	[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
	[BaseType (typeof(NSObject))]
	interface MKMapCameraZoomRange : NSSecureCoding, NSCopying
	{
		[Export ("initWithMinCenterCoordinateDistance:maxCenterCoordinateDistance:")]
		[DesignatedInitializer]
		NativeHandle Constructor (double minDistance, double maxDistance);

		[Internal]
		[Export ("initWithMinCenterCoordinateDistance:")]
		IntPtr InitWithMinCenterCoordinateDistance (double minDistance);

		[Internal]
		[Export ("initWithMaxCenterCoordinateDistance:")]
		IntPtr InitWithMaxCenterCoordinateDistance (double maxDistance);

		[Export ("minCenterCoordinateDistance")]
		double MinCenterCoordinateDistance { get; }

		[Export ("maxCenterCoordinateDistance")]
		double MaxCenterCoordinateDistance { get; }

		[Field ("MKMapCameraZoomDefault")]
		double ZoomDefault { get; }
	}

	[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
	[BaseType (typeof(NSObject))]
	interface MKMapCameraBoundary : NSSecureCoding, NSCopying
	{
		[Export ("initWithMapRect:")]
		[DesignatedInitializer]
		NativeHandle Constructor (MKMapRect mapRect);

		[Export ("initWithCoordinateRegion:")]
		[DesignatedInitializer]
		NativeHandle Constructor (MKCoordinateRegion region);

		[Export ("mapRect")]
		MKMapRect MapRect { get; }

		[Export ("region")]
		MKCoordinateRegion Region { get; }
	}

	[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
	[BaseType (typeof(MKShape))]
	interface MKMultiPolygon : MKOverlay, MKGeoJsonObject
	{
		[Export ("initWithPolygons:")]
		[DesignatedInitializer]
		NativeHandle Constructor (MKPolygon[] polygons);

		[Export ("polygons", ArgumentSemantic.Copy)]
		MKPolygon[] Polygons { get; }
	}

	[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
	[BaseType (typeof(MKOverlayPathRenderer))]
	interface MKMultiPolygonRenderer
	{
		[Export ("initWithMultiPolygon:")]
		NativeHandle Constructor (MKMultiPolygon multiPolygon);

		[Export ("multiPolygon")]
		MKMultiPolygon MultiPolygon { get; }
	}

	[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
	[BaseType (typeof(MKShape))]
	interface MKMultiPolyline : MKOverlay, MKGeoJsonObject
	{
		[Export ("initWithPolylines:")]
		[DesignatedInitializer]
		NativeHandle Constructor (MKPolyline[] polylines);

		[Export ("polylines", ArgumentSemantic.Copy)]
		MKPolyline[] Polylines { get; }
	}

	[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
	[BaseType (typeof(MKOverlayPathRenderer))]
	interface MKMultiPolylineRenderer
	{
		[Export ("initWithMultiPolyline:")]
		NativeHandle Constructor (MKMultiPolyline multiPolyline);

		[Export ("multiPolyline")]
		MKMultiPolyline MultiPolyline { get; }
	}

	[TV (14, 0), NoWatch, Mac (11, 0), iOS (14, 0)]
	[BaseType (typeof (MKAnnotationView))]
	interface MKUserLocationView { 
		[DesignatedInitializer]
		[Export ("initWithAnnotation:reuseIdentifier:")]
		NativeHandle Constructor ([NullAllowed] IMKAnnotation annotation, [NullAllowed] string reuseIdentifier);
	
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);
	}

	[TV (14, 0), NoWatch, Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface MKLocalPointsOfInterestRequest : NSCopying
	{
		[Field ("MKPointsOfInterestRequestMaxRadius")]
		double RequestMaxRadius { get; }

		[Export ("initWithCenterCoordinate:radius:")]
		[DesignatedInitializer]
		NativeHandle Constructor (CLLocationCoordinate2D centerCoordinate, double radius);

		[Export ("initWithCoordinateRegion:")]
		[DesignatedInitializer]
		NativeHandle Constructor (MKCoordinateRegion region);

		[Export ("coordinate")]
		CLLocationCoordinate2D Coordinate { get; }

		[Export ("radius")]
		double Radius { get; }

		[Export ("region")]
		MKCoordinateRegion Region { get; }

		[NullAllowed, Export ("pointOfInterestFilter", ArgumentSemantic.Copy)]
		MKPointOfInterestFilter PointOfInterestFilter { get; set; }
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch, NoTV, NoiOS, Mac (11, 0)]
	[DisableDefaultCtor]
	[BaseType (typeof (UIView))]
	interface MKPitchControl
	{
		[DesignatedInitializer]
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frameRect);

		[Static]
		[Export ("pitchControlWithMapView:")]
		MKPitchControl Create ([NullAllowed] MKMapView mapView);

		[NullAllowed, Export ("mapView", ArgumentSemantic.Weak)]
		MKMapView MapView { get; set; }
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch, NoTV, NoiOS, Mac (11, 0)]
	[DisableDefaultCtor]
	[BaseType (typeof (UIView))]
	interface MKZoomControl
	{

		[DesignatedInitializer]
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frameRect);

		[Static]
		[Export ("zoomControlWithMapView:")]
		MKZoomControl Create ([NullAllowed] MKMapView mapView);

		[NullAllowed, Export ("mapView", ArgumentSemantic.Weak)]
		MKMapView MapView { get; set; }
	}

}
