// Copyright 2013, 2015 Xamarin Inc. All rights reserved

#if !XAMCORE_2_0 && !MONOMAC

using System;

namespace MapKit {

	public partial class MKCircle {

		[Obsolete ("Use BoundingMapRect as it's part of the MKOverlay protocol")]
		public MKMapRect BoundingMap { 
			get { return BoundingMapRect; }
		}
	}
}

#endif
