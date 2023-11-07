#if !__TVOS__

using System;
using Foundation;
using MapKit;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.MapKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class GeometryTest {

		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 9, throwIfOtherPlatform: false);
		}

		[Test]
		public void MapPointsPerMeterAtLatitude ()
		{
			Assert.That (MKGeometry.MapPointsPerMeterAtLatitude (0), Is.EqualTo (6.743).Within (0.001), "0");

			Assert.That (MKGeometry.MapPointsPerMeterAtLatitude (90), Is.EqualTo (3.936).Within (0.001), "90");
			Assert.That (MKGeometry.MapPointsPerMeterAtLatitude (91), Is.EqualTo (0), "91");

			Assert.That (MKGeometry.MapPointsPerMeterAtLatitude (-90), Is.EqualTo (2399.37).Within (0.01), "-90");
			Assert.That (MKGeometry.MapPointsPerMeterAtLatitude (-91), Is.EqualTo (0), "-91");

			Assert.That (MKGeometry.MapPointsPerMeterAtLatitude (Double.NaN), Is.NaN, "NaN");
			Assert.That (MKGeometry.MapPointsPerMeterAtLatitude (Double.NegativeInfinity), Is.NaN, "NegativeInfinity");
			Assert.That (MKGeometry.MapPointsPerMeterAtLatitude (Double.PositiveInfinity), Is.NaN, "PositiveInfinity");
		}

		[Test]
		public void MetersPerMapPointAtLatitude ()
		{
			Assert.That (MKGeometry.MetersPerMapPointAtLatitude (0), Is.EqualTo (0.148).Within (0.001), "0");

			Assert.That (MKGeometry.MetersPerMapPointAtLatitude (90), Is.EqualTo (0.254).Within (0.001), "90");
			Assert.That (MKGeometry.MetersPerMapPointAtLatitude (91), Is.EqualTo (Double.PositiveInfinity), "91");

			Assert.That (MKGeometry.MetersPerMapPointAtLatitude (-90), Is.EqualTo (0.000416).Within (0.000001), "-90");
			Assert.That (MKGeometry.MetersPerMapPointAtLatitude (-91), Is.EqualTo (Double.PositiveInfinity), "-91");

			Assert.That (MKGeometry.MetersPerMapPointAtLatitude (Double.NaN), Is.NaN, "NaN");
			Assert.That (MKGeometry.MetersPerMapPointAtLatitude (Double.NegativeInfinity), Is.NaN, "NegativeInfinity");
			Assert.That (MKGeometry.MetersPerMapPointAtLatitude (Double.PositiveInfinity), Is.NaN, "PositiveInfinity");
		}

		[Test]
		public void MetersBetweenMapPoints ()
		{
			MKMapPoint a = new MKMapPoint ();
			Assert.That (MKGeometry.MetersBetweenMapPoints (a, a), Is.EqualTo (0.0), "a-a");

			MKMapPoint b = new MKMapPoint (1000, 1000);
			Assert.That (MKGeometry.MetersBetweenMapPoints (b, b), Is.EqualTo (0.0), "b-b");

			Assert.That (MKGeometry.MetersBetweenMapPoints (a, b), Is.EqualTo (18.153).Within (0.001), "a-b");
			Assert.That (MKGeometry.MetersBetweenMapPoints (b, a), Is.EqualTo (18.153).Within (0.001), "b-a");

			MKMapPoint c = new MKMapPoint (Double.NaN, Double.NaN);
			Assert.That (MKGeometry.MetersBetweenMapPoints (a, c), Is.NaN, "NaN");
		}
	}
}

#endif // !__TVOS__
