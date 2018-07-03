// Copyright 2011 Xamarin Inc. All rights reserved

#if !__TVOS__ && !__WATCHOS__

using System;
using System.Drawing;
#if XAMCORE_2_0
using Foundation;
using CoreLocation;
using MapKit;
#else
using MonoTouch.CoreLocation;
using MonoTouch.Foundation;
using MonoTouch.MapKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.MapKit {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PolygonTest {
		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertMacSystemVersion (10, 9, throwIfOtherPlatform: false);
		}
		
		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void FromPoints_Null ()
		{
			MKPolygon.FromPoints (null);
		}

		[Test]
		public void FromPoints_Interior_Null ()
		{
			MKPolygon pg = MKPolygon.FromPoints (new MKMapPoint [] { }, null);
			CheckEmpty (pg);
		}
		
		void CheckEmpty (MKPolygon pg)
		{
			// MKAnnotation
			Assert.That (pg.Coordinate.Longitude, Is.NaN, "Coordinate.Longitude");
			Assert.That (pg.Coordinate.Latitude, Is.NaN, "Coordinate.Latitude");
			Assert.Null (pg.Title, "Title");
			Assert.Null (pg.Subtitle, "Subtitle");
			// MKOverlay
			Assert.True (Double.IsPositiveInfinity (pg.BoundingMapRect.Origin.X), "BoundingMapRect.Origin.X");
			Assert.True (Double.IsPositiveInfinity (pg.BoundingMapRect.Origin.Y), "BoundingMapRect.Origin.Y");
			Assert.True (Double.IsNegativeInfinity (pg.BoundingMapRect.Size.Height), "BoundingMapRect.Size.Height");
			Assert.True (Double.IsNegativeInfinity (pg.BoundingMapRect.Size.Width), "BoundingMapRect.Size.Width");
			Assert.False (pg.Intersects (pg.BoundingMapRect), "Intersect/Self");
			MKMapRect rect = new MKMapRect (0, 0, 0, 0);
			Assert.False (pg.Intersects (rect), "Intersect/Empty");
			
			ShapeTest.CheckShape (pg);
		}
		
		[Test]
		public void FromPoints_Empty ()
		{
			MKPolygon pg = MKPolygon.FromPoints (new MKMapPoint [] { });
			CheckEmpty (pg);
		}

		[Test]
		public void FromPoints_Interior_Empty ()
		{
			MKPolygon pg = MKPolygon.FromPoints (new MKMapPoint [] { }, new MKPolygon [] { });
			CheckEmpty (pg);
		}
		
		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void FromCoordinates_Null ()
		{
			MKPolygon.FromCoordinates (null);
		}

		[Test]
		public void FromCoordinates_Interior_Null ()
		{
			MKPolygon pg = MKPolygon.FromCoordinates (new CLLocationCoordinate2D [] { }, null);
			CheckEmpty (pg);
		}
		
		[Test]
		public void FromCoordinates_Empty ()
		{
			MKPolygon pg = MKPolygon.FromCoordinates (new CLLocationCoordinate2D [] { });
			CheckEmpty (pg);
		}

		[Test]
		public void FromCoordinates_Interior_Empty ()
		{
			MKPolygon pg = MKPolygon.FromCoordinates (new CLLocationCoordinate2D [] { }, new MKPolygon [] { });
			CheckEmpty (pg);
		}
		
#if false
		// Annotations that support dragging should implement this method to update the position of the annotation.
		// keyword is SHOULD - it's not working for MKPolygon
		// http://developer.apple.com/library/ios/#documentation/MapKit/Reference/MKAnnotation_Protocol/Reference/Reference.html#//apple_ref/occ/intf/MKAnnotation
		[Test]
		public void setCoordinate_Selector ()
		{
			MKPolygon pg = MKPolygon.FromPoints (new MKMapPoint [] { });
			try {
				pg.Coordinate = new CLLocationCoordinate2D (10, 20);
			}
			catch (MonoTouchException mte) {
				Assert.True (mte.Message.Contains ("unrecognized selector sent to instance"));
			}
			catch {
				Assert.Fail ("API could be working/implemented");
			}
		}
#endif
	}
}

#endif // !__TVOS__ && !__WATCHOS__
