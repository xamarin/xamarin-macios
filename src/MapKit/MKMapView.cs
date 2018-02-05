//
// MapKit definitions
//
// Author:
//   Miguel de Icaza
//
// Copyright 2009 Novell, Inc.
// Copyright 2012-2014 Xamarin Inc. All rights reserved.
//

#if !XAMCORE_2_0 && !MONOMAC

using System;
using Foundation;

namespace MapKit
{
	public partial class MKMapView {
		public void AddAnnotation (MKAnnotation annotation)
		{
			AddAnnotationObject (annotation);
		}

		[Obsolete ("Use 'AddAnnotations'.")]
		public void AddAnnotation (MKAnnotation [] annotations)
		{
			AddAnnotationObjects (annotations);
		}

		// Just an alias to AddAnnotation (MKAnnotation [] annotations)
		// See x#478
		public void AddAnnotations (params MKAnnotation [] annotations)
		{
			AddAnnotationObjects (annotations);
		}

		public void AddPlacemark (MKPlacemark placemark)
		{
			AddAnnotationObject (placemark);
		}

		[Obsolete ("Use 'AddPlacemarks'.")]
		public void AddAnnotation (MKPlacemark [] placemarks)
		{
			AddAnnotationObjects (placemarks);
		}

		public void AddPlacemarks (params MKPlacemark [] placemarks)
		{
			AddAnnotationObjects (placemarks);
		}

		[Obsolete ("Use 'VisibleMapRect'")]
		public virtual MKMapRect visibleMapRect {
			get { return VisibleMapRect; }
			set { VisibleMapRect = value; }
		}

		public void RemoveOverlays (params IMKOverlay[] overlays)
		{
			foreach (var overlay in overlays)
				RemoveOverlay ((NSObject) overlay);
		}
	}
}
#endif
