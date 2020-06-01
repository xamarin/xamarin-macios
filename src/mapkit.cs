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
		string Title { get; }
	
		[Export ("subtitle", ArgumentSemantic.Copy)]
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
		IntPtr Constructor ([NullAllowed] IMKAnnotation annotation, [NullAllowed] string reuseIdentifier);
	
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[Export ("reuseIdentifier")]
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

#if MONOMAC
		[Export ("leftCalloutOffset")]
		CGPoint LeftCalloutOffset { get; set; }

		[Export ("rightCalloutOffset")]
		CGPoint RightCallpoutOffset { get; set; }
#endif
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

#if !MONOMAC && !TVOS
	[BaseType (typeof (MKOverlayPathView))]
	[Availability (Deprecated = Platform.iOS_7_0, Message = "Use 'MKCircleRenderer' instead.")]
	interface MKCircleView {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[Export ("circle")]
		MKCircle Circle { get;  }

		[Export ("initWithCircle:")]
		[PostGet ("Circle")]
		IntPtr Constructor (MKCircle circle);
	}
#endif
	
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
		IntPtr Constructor (NSUrl url);

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
		IntPtr Constructor (MKPlacemark placemark);

		[NoTV]
		[Export ("openInMapsWithLaunchOptions:"), Internal]
		bool _OpenInMaps ([NullAllowed] NSDictionary launchOptions);

		[NoTV]
		[Static]
		[Export ("openMapsWithItems:launchOptions:"), Internal]
		bool _OpenMaps ([NullAllowed] MKMapItem [] mapItems, [NullAllowed] NSDictionary launchOptions);

		[iOS (13, 2), NoMac, NoTV, NoWatch]
		[Introduced (PlatformName.UIKitForMac, 13, 2)]
		[Async]
		[Export ("openInMapsWithLaunchOptions:fromScene:completionHandler:")]
		void OpenInMaps ([NullAllowed] NSDictionary launchOptions, [NullAllowed] UIScene fromScene, Action<NSError> completionHandler);

		[iOS (13, 2), NoMac, NoTV, NoWatch]
		[Introduced (PlatformName.UIKitForMac, 13, 2)]
		[Static]
		[Async]
		[Export ("openMapsWithItems:launchOptions:fromScene:completionHandler:")]
		void OpenMaps ([NullAllowed] MKMapItem [] mapItems, [NullAllowed] NSDictionary launchOptions, [NullAllowed] UIScene fromScene, Action<NSError> completionHandler);

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

#if !WATCH
	[TV (9,2)]
	[BaseType (typeof (UIView), Delegates=new string [] { "WeakDelegate" }, Events=new Type [] {typeof (MKMapViewDelegate)})]
	[Mac (10,9)]
	interface MKMapView {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

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
		[Wrap ("Register (viewType == null ? null : new Class (viewType), identifier)")]
		void Register ([NullAllowed] Type viewType, string identifier);

		[Export ("selectAnnotation:animated:")]
		[PostGet ("SelectedAnnotations")]
		void SelectAnnotation (IMKAnnotation annotation, bool animated);
	
		[Export ("deselectAnnotation:animated:")]
		[PostGet ("SelectedAnnotations")]
		void DeselectAnnotation (IMKAnnotation annotation, bool animated);
	
		[NullAllowed] // by default this property is null
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

#if !MONOMAC && !TVOS
		[Export ("viewForOverlay:")]
		[Availability (Deprecated = Platform.iOS_7_0, Message = "Use 'MKOverlayRenderer.RendererForOverlay' instead.")]
		MKOverlayView ViewForOverlay (IMKOverlay overlay);
#endif // !MONOMAC && !TVOS

		[Export ("visibleMapRect")]
		MKMapRect VisibleMapRect { get; set;  }

		[Export ("annotationsInMapRect:")]
		NSSet GetAnnotations (MKMapRect mapRect);

#if !MONOMAC
		[Export ("userTrackingMode")]
		MKUserTrackingMode UserTrackingMode { get; set; }
		
		[Export ("setUserTrackingMode:animated:")]
		void SetUserTrackingMode (MKUserTrackingMode trackingMode, bool animated);
#endif

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
		[Mac(10,9), iOS(9,0)]
		[Export ("showsCompass")]
		bool ShowsCompass { get; set; }

		[Export ("showsScale")]
		[Mac (10,10), iOS(9,0)]
		bool ShowsScale { get; set; }

		[Export ("showsTraffic")]
		[Mac (10,11), iOS(9,0)]
		bool ShowsTraffic { get; set; }

#if MONOMAC
		[Export ("showsZoomControls")]
		bool ShowsZoomControls { get; set; }
#endif

		[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
		[Export ("setCameraZoomRange:animated:")]
		void SetCameraZoomRange ([NullAllowed] MKMapCameraZoomRange cameraZoomRange, bool animated);

		[TV (13, 0), NoWatch, Mac (10, 15), iOS (13, 0)]
		[Export ("cameraZoomRange", ArgumentSemantic.Copy)]
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
		MKAnnotationView GetViewForAnnotation (MKMapView mapView, IMKAnnotation annotation);
	
		[Export ("mapView:didAddAnnotationViews:"), EventArgs ("MKMapViewAnnotation")]
		void DidAddAnnotationViews (MKMapView mapView, MKAnnotationView [] views);
	
#if !MONOMAC
		[NoTV]
		[Export ("mapView:annotationView:calloutAccessoryControlTapped:"), EventArgs ("MKMapViewAccessoryTapped")]
		void CalloutAccessoryControlTapped (MKMapView mapView, MKAnnotationView view, UIControl control);
#endif // !MONOMAC

		[NoTV]
		[Export ("mapView:annotationView:didChangeDragState:fromOldState:"), EventArgs ("MKMapViewDragState")]
		void ChangedDragState (MKMapView mapView, MKAnnotationView annotationView, MKAnnotationViewDragState newState, MKAnnotationViewDragState oldState);

#if !MONOMAC && !TVOS
		[Export ("mapView:viewForOverlay:"), DelegateName ("MKMapViewOverlay"), DefaultValue (null)]
		[Availability (Deprecated = Platform.iOS_7_0, Message = "Use 'MKOverlayRenderer.RendererForOverlay' instead.")]
		MKOverlayView GetViewForOverlay (MKMapView mapView, IMKOverlay overlay);

		[Export ("mapView:didAddOverlayViews:"), EventArgs ("MKOverlayViews")]
		[Availability (Deprecated = Platform.iOS_7_0, Message = "Use 'DidAddOverlayRenderers' instead.")]
		void DidAddOverlayViews (MKMapView mapView, MKOverlayView overlayViews);
#endif // !MONOMAC && !TVOS

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

#if !MONOMAC
		[Export ("mapView:didChangeUserTrackingMode:animated:"), EventArgs ("MMapViewUserTracking")]
		void DidChangeUserTrackingMode (MKMapView mapView, MKUserTrackingMode mode, bool animated);
#endif // !MONOMAC

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
	interface MKPinAnnotationView {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[Export ("initWithAnnotation:reuseIdentifier:")]
		IntPtr Constructor ([NullAllowed] IMKAnnotation annotation, [NullAllowed] string reuseIdentifier);

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

#if IOS
	// This requires the AddressBook framework, which afaict isn't bound on Mac, tvOS and watchOS yet
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
#endif // !MONOMAC
#endif // !WATCH

	[BaseType (typeof (CLPlacemark))]
	// crash (at least) when calling 'description' when instance is created by 'init'
	[DisableDefaultCtor]
	[TV (9,2)]
	[Mac (10,9)]
	interface MKPlacemark : MKAnnotation, NSCopying {
		[Export ("initWithCoordinate:addressDictionary:")]
		IntPtr Constructor (CLLocationCoordinate2D coordinate, [NullAllowed] NSDictionary addressDictionary);

#if IOS
		// This requires the AddressBook framework, which afaict isn't bound on Mac, tvOS and watchOS yet
		[Wrap ("this (coordinate, addressDictionary.GetDictionary ())")]
		IntPtr Constructor (CLLocationCoordinate2D coordinate, MKPlacemarkAddress addressDictionary);
#endif // !MONOMAC && !WATCH

		[Watch (3,0)][TV (10,0)][iOS (10,0)]
		[Mac (10,12)]
		[Export ("initWithCoordinate:")]
		IntPtr Constructor (CLLocationCoordinate2D coordinate);

#if !TVOS
		[Watch (3,0)][iOS (10,0)]
		[Mac (10,12)]
		[NoTV]
		[Export ("initWithCoordinate:postalAddress:")]
		IntPtr Constructor (CLLocationCoordinate2D coordinate, CNPostalAddress postalAddress);
#endif
	
		[Export ("countryCode")]
		string CountryCode { get; }
	}
		
#if IOS
	[BaseType (typeof (NSObject))]
	[Availability (Deprecated = Platform.iOS_5_0, Message = "Use 'CoreLocation.CLGeocoder' instead.")]
	// crash (at least) at Dispose time when instance is created by 'init'
	[DisableDefaultCtor]
	interface MKReverseGeocoder {
		[Export ("initWithCoordinate:")]
		IntPtr Constructor (CLLocationCoordinate2D coordinate);
	
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

	[Availability (Deprecated = Platform.iOS_7_0, Message = "Use 'MKOverlayRenderer' instead.")]
	[BaseType (typeof (UIView))]
	interface MKOverlayView {
		[Export ("overlay")]
		IMKOverlay Overlay { get; }

		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[DesignatedInitializer]
		[Export ("initWithOverlay:")]
		IntPtr Constructor (IMKOverlay overlay);

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

	[Availability (Deprecated = Platform.iOS_7_0, Message = "Use 'MKOverlayPathRenderer' instead.")]
	[BaseType (typeof (MKOverlayView))]
	interface MKOverlayPathView {
		[Export ("initWithOverlay:")]
		IntPtr Constructor (IMKOverlay overlay);

		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

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
#endif // IOS

#if !WATCH
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

	[TV (9,2)]
	[Mac (10,9)]
	[DesignatedDefaultCtor]
	[BaseType (typeof (MKShape))]
	interface MKPointAnnotation : MKGeoJsonObject {
		[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
		[Export ("initWithCoordinate:")]
		IntPtr Constructor (CLLocationCoordinate2D coordinate);

		[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
		[Export ("initWithCoordinate:title:subtitle:")]
		IntPtr Constructor (CLLocationCoordinate2D coordinate, [NullAllowed] string title, [NullAllowed] string subtitle);

		[Export ("coordinate")]
		CLLocationCoordinate2D Coordinate { get; set; }
}

#if !MONOMAC && !TVOS
	[Availability (Deprecated = Platform.iOS_7_0, Message = "Use 'MKPolygonRenderer' instead.")]
	[BaseType (typeof (MKOverlayPathView))]
	interface MKPolygonView {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[Export ("initWithPolygon:")]
		[PostGet ("Polygon")]
		IntPtr Constructor (MKPolygon polygon);
		
		[Export ("polygon")]
		MKPolygon Polygon { get;  }
	}
#endif // !MONOMAC && !TVOS

	[ThreadSafe]
	[TV (9,2)]
	[Mac (10,9)]
	[BaseType (typeof (MKMultiPoint))]
	interface MKPolygon : MKOverlay, MKGeoJsonObject {
		[Export ("interiorPolygons")]
		MKPolygon [] InteriorPolygons { get;  }

		[Static]
		[Internal]
		[Export ("polygonWithPoints:count:")]
		MKPolygon _FromPoints (IntPtr points, nint count);

		[Static]
		[Internal]
		[Export ("polygonWithPoints:count:interiorPolygons:")]
		MKPolygon _FromPoints (IntPtr points, nint count, MKPolygon [] interiorPolygons);

		[Static]
		[Export ("polygonWithCoordinates:count:"), Internal]
		MKPolygon _FromCoordinates (IntPtr coords, nint count);

		[Static]
		[Internal]
		[Export ("polygonWithCoordinates:count:interiorPolygons:")]
		MKPolygon _FromCoordinates (IntPtr coords, nint count, MKPolygon [] interiorPolygons);

		#region MKAnnotation
		[Export ("coordinate")]
		CLLocationCoordinate2D Coordinate { get; }
		// note: setCoordinate: is not mandatory and is not implemented for MKPolygon (see unit tests)
		#endregion
	}

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

#if !MONOMAC && !TVOS
	[Availability (Deprecated = Platform.iOS_7_0, Message = "Use 'MKPolylineRenderer' instead.")]
	[BaseType (typeof (MKOverlayPathView))]
	interface MKPolylineView {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[Export ("initWithPolyline:")]
		[PostGet ("Polyline")]
		IntPtr Constructor (MKPolyline polyline);
		
		[Export ("polyline")]
		MKPolyline Polyline { get;  }
	}
#endif // !MONOMAC && !TVOS

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
	}

	[BaseType (typeof (NSObject))]
	[TV (9,2)]
	[Mac (10,9)]
	interface MKUserLocation : IMKAnnotation { // This is wrong. It should be MKAnnotation but we can't due to API compat. When you fix this remove hack in generator.cs to enable warning again
		[Export ("updating")]
		bool Updating { [Bind ("isUpdating")] get; }
		
		[Export ("coordinate")]
		CLLocationCoordinate2D Coordinate { get; set; }
		
		[Export ("location", ArgumentSemantic.Retain)]
		CLLocation Location { get; }

		[Export ("title", ArgumentSemantic.Copy)]
		string Title { get; set; }
		
		[NullAllowed] // by default this property is null
		[Export ("subtitle", ArgumentSemantic.Copy)]
		string Subtitle { get; set; }
		
		[NoTV]
		[Export ("heading", ArgumentSemantic.Retain)]
		CLHeading Heading { get; }
	}

#if !MONOMAC
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
		IntPtr Constructor (MKMapView mapView);
	}
#endif // !MONOMAC

	delegate void MKLocalSearchCompletionHandler (MKLocalSearchResponse response, NSError error);

	[TV (9,2)]
	[Mac (10,9)]
	[BaseType (typeof (NSObject))]
	[ThreadSafe]
	[DisableDefaultCtor] // crash on iOS8 beta
	interface MKLocalSearch {

		[DesignatedInitializer]
		[Export ("initWithRequest:")]
		IntPtr Constructor (MKLocalSearchRequest request);

		[Export ("startWithCompletionHandler:")]
		[Async]
		void Start (MKLocalSearchCompletionHandler completionHandler);

		[Export ("cancel")]
		void Cancel ();

		[Export ("searching")]
		bool IsSearching { [Bind ("isSearching")] get; }
	}

	[TV (9,2)]
	[Mac (10,9)]
	[BaseType (typeof (NSObject))]
	[ThreadSafe]
	[DesignatedDefaultCtor]
	interface MKLocalSearchRequest : NSCopying {

		[DesignatedInitializer]
		[TV (9,2)][NoWatch][iOS (9,3)][Mac (10,11,4)]
		[Export ("initWithCompletion:")]
		IntPtr Constructor (MKLocalSearchCompletion completion);

		[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
		[Export ("initWithNaturalLanguageQuery:")]
		IntPtr Constructor (string naturalLanguageQuery);

		[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
		[Export ("initWithNaturalLanguageQuery:region:")]
		IntPtr Constructor (string naturalLanguageQuery, MKCoordinateRegion region);

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

	[TV (9,2)]
	[iOS (7,0), BaseType (typeof (MKOverlayPathRenderer))]
	[Mac (10,9)]
	partial interface MKCircleRenderer {

		[Export ("initWithCircle:")]
		IntPtr Constructor (MKCircle circle);

		[Export ("circle")]
		MKCircle Circle { get; }
	}

	[TV (9,2)]
	[iOS (7,0), BaseType (typeof (NSObject))]
	[Mac (10,9)]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: Cannot initialize MKDirections with nil request
	partial interface MKDirections {

		[DesignatedInitializer]
		[Export ("initWithRequest:")]
		IntPtr Constructor (MKDirectionsRequest request);

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
#endif // !WATCH

	[TV (9,2)]
	[iOS (7,0), BaseType (typeof (NSFormatter))]
	[Mac (10,9)]
	partial interface MKDistanceFormatter {

		[Export ("stringFromDistance:")]
		string StringFromDistance (double distance);

		[Export ("distanceFromString:")]
		double DistanceFromString (string distance);

		[Export ("locale", ArgumentSemantic.Copy)]
		NSLocale Locale { get; set; }

		[Export ("units", ArgumentSemantic.Assign)]
		MKDistanceFormatterUnits Units { get; set; }

		[Export ("unitStyle", ArgumentSemantic.Assign)]
		MKDistanceFormatterUnitStyle UnitStyle { get; set; }
	}

#if !WATCH
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

	[TV (9,2)]
	[iOS (7,0), BaseType (typeof (NSObject))]
	[Mac (10,9)]
	partial interface MKMapSnapshot {

		[Export ("image")]
		UIImage Image { get; }

		[Export ("pointForCoordinate:")]
		CGPoint PointForCoordinate (CLLocationCoordinate2D coordinate);

#if MONOMAC
		[NoWatch][NoTV][NoiOS]
		[Mac (10,14)]
		[Export ("appearance")]
		NSAppearance Appearance { get; }
#endif

		[TV (13, 0), NoWatch, NoMac, iOS (13, 0)]
		[Export ("traitCollection")]
		UITraitCollection TraitCollection { get; }
	}

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

#if !MONOMAC
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use `TraitCollection.DisplayScale` instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use `TraitCollection.DisplayScale` instead.")]
		[Export ("scale", ArgumentSemantic.Assign)]
		nfloat Scale { get; set; }
#endif

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'PointOfInterestFilter' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'PointOfInterestFilter' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'PointOfInterestFilter' instead.")]
		[Export ("showsPointsOfInterest")]
		bool ShowsPointsOfInterest { get; set; }

		[Export ("showsBuildings")]
		bool ShowsBuildings { get; set; }

#if MONOMAC
		[NoWatch][NoTV][NoiOS]
		[Mac (10,14)]
		[NullAllowed, Export ("appearance", ArgumentSemantic.Strong)]
		NSAppearance Appearance { get; set; }
#endif

		[TV (13, 0), NoWatch, Mac (10, 15), iOS (13, 0)]
		[NullAllowed, Export ("pointOfInterestFilter", ArgumentSemantic.Copy)]
		MKPointOfInterestFilter PointOfInterestFilter { get; set; }

		[TV (13, 0), NoWatch, NoMac, iOS (13, 0)]
		[Export ("traitCollection", ArgumentSemantic.Copy)]
		UITraitCollection TraitCollection { get; set; }
	}

	[TV (9,2)]
	[iOS (7,0), BaseType (typeof (NSObject))]
	[Mac (10,9)]
	partial interface MKMapSnapshotter {

		[DesignatedInitializer]
		[Export ("initWithOptions:")]
		IntPtr Constructor (MKMapSnapshotOptions options);

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

	[TV (9,2)]
	[iOS (7,0), BaseType (typeof (MKOverlayRenderer))]
	[Mac (10,9)]
	[ThreadSafe]
	partial interface MKOverlayPathRenderer {

		[Export ("initWithOverlay:")]
		IntPtr Constructor (IMKOverlay overlay);

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

	[TV (9,2)]
	[iOS (7,0), BaseType (typeof (NSObject))]
	[Mac (10,9)]
	partial interface MKOverlayRenderer {

		[DesignatedInitializer]
		[Export ("initWithOverlay:")]
		IntPtr Constructor (IMKOverlay overlay);

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

	[TV (9,2)]
	[iOS (7,0), BaseType (typeof (MKOverlayPathRenderer))]
	[Mac (10,9)]
	partial interface MKPolygonRenderer {

		[Export ("initWithPolygon:")]
		IntPtr Constructor (MKPolygon polygon);

		[Export ("polygon")]
		MKPolygon Polygon { get; }
	}

	[TV (9,2)]
	[iOS (7,0), BaseType (typeof (MKOverlayPathRenderer))]
	[Mac (10,9)]
	partial interface MKPolylineRenderer {

		[Export ("initWithPolyline:")]
		IntPtr Constructor (MKPolyline polyline);

		[Export ("polyline")]
		MKPolyline Polyline { get; }
	}

	[ThreadSafe]
	[TV (9,2)]
	[Mac (10,9)]
	[iOS (7,0), BaseType (typeof (NSObject))]
	partial interface MKTileOverlay : MKOverlay {
		[DesignatedInitializer]
		[Export ("initWithURLTemplate:")]
		IntPtr Constructor (string URLTemplate);

		[Export ("tileSize")]
		CGSize TileSize { get; set; }

		[Export ("geometryFlipped")]
		bool GeometryFlipped { [Bind ("isGeometryFlipped")] get; set; }

		[Export ("minimumZ")]
		nint MinimumZ { get; set; }

		[Export ("maximumZ")]
		nint MaximumZ { get; set; }

		[Export ("URLTemplate")]
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

	[TV (9,2)]
	[iOS (7,0), BaseType (typeof (MKOverlayRenderer))]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: Expected a MKTileOverlay but got (null)
	[DisableDefaultCtor] // throw in iOS8 beta 1 ^
	[Mac (10,9)]
	partial interface MKTileOverlayRenderer {
		// This ctor is not allowed: NSInvalidArgumentEception Expected a MKTileOverlay
//		[Export ("initWithOverlay:")]
//		IntPtr Constructor (IMKOverlay toverlay);

		[Export ("initWithTileOverlay:")]
		IntPtr Constructor (MKTileOverlay overlay);

		[Export ("reloadData")]
		void ReloadData ();
	}

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

#endif // !WATCH

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
		IntPtr Constructor (IMKAnnotation[] memberAnnotations);
	}

	[NoTV][iOS (11,0)][NoMac][NoWatch]
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

	[TV (11,0)][NoWatch][iOS (11,0)][NoMac]
	[BaseType (typeof (MKAnnotationView))]
	interface MKMarkerAnnotationView {

		// inlined from base type
		[Export ("initWithAnnotation:reuseIdentifier:")]
		[PostGet ("Annotation")]
		IntPtr Constructor ([NullAllowed] IMKAnnotation annotation, [NullAllowed] string reuseIdentifier);

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
		IntPtr Constructor (double minDistance, double maxDistance);

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
		IntPtr Constructor (MKMapRect mapRect);

		[Export ("initWithCoordinateRegion:")]
		[DesignatedInitializer]
		IntPtr Constructor (MKCoordinateRegion region);

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
		IntPtr Constructor (MKPolygon[] polygons);

		[Export ("polygons", ArgumentSemantic.Copy)]
		MKPolygon[] Polygons { get; }
	}

	[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
	[BaseType (typeof(MKOverlayPathRenderer))]
	interface MKMultiPolygonRenderer
	{
		[Export ("initWithMultiPolygon:")]
		IntPtr Constructor (MKMultiPolygon multiPolygon);

		[Export ("multiPolygon")]
		MKMultiPolygon MultiPolygon { get; }
	}

	[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
	[BaseType (typeof(MKShape))]
	interface MKMultiPolyline : MKOverlay, MKGeoJsonObject
	{
		[Export ("initWithPolylines:")]
		[DesignatedInitializer]
		IntPtr Constructor (MKPolyline[] polylines);

		[Export ("polylines", ArgumentSemantic.Copy)]
		MKPolyline[] Polylines { get; }
	}

	[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
	[BaseType (typeof(MKOverlayPathRenderer))]
	interface MKMultiPolylineRenderer
	{
		[Export ("initWithMultiPolyline:")]
		IntPtr Constructor (MKMultiPolyline multiPolyline);

		[Export ("multiPolyline")]
		MKMultiPolyline MultiPolyline { get; }
	}
}
