//
// MKUserTrackingBarButtonItem Unit Tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
using System.Reflection;
#if XAMCORE_2_0
using Foundation;
using MapKit;
using ObjCRuntime;
#else
using MonoTouch.Foundation;
using MonoTouch.MapKit;
using MonoTouch.ObjCRuntime;
#endif

using NUnit.Framework;

namespace MonoTouchFixtures.MapKit {
	
	class UserTrackingBarButtonItemPoker : MKUserTrackingBarButtonItem {
		
		static FieldInfo bkMapView;
		
		static UserTrackingBarButtonItemPoker ()
		{
			var t = typeof (MKUserTrackingBarButtonItem);
			bkMapView = t.GetField ("__mt_MapView_var", BindingFlags.Instance | BindingFlags.NonPublic);
		}
		
		public static bool NewRefcountEnabled ()
		{
			return NSObject.IsNewRefcountEnabled ();
		}
		
		public UserTrackingBarButtonItemPoker (MKMapView view) : base (view)
		{
		}
		
		public MKMapView MapViewBackingField {
			get {
				return (MKMapView) bkMapView.GetValue (this);
			}
		}
	}

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class UserTrackingBarButtonItemTest {
		
		[Test]
		public void Ctor_Defaults ()
		{
			using (var a = new NSObject ())
			using (MKUserTrackingBarButtonItem ut = new MKUserTrackingBarButtonItem ()) {
				Assert.Null (ut.MapView, "MapView");
			}
		}

		[Test]
		public void Ctor_MKMapView ()
		{
			using (MKMapView mv = new MKMapView ())
			using (MKUserTrackingBarButtonItem ut = new MKUserTrackingBarButtonItem (mv)) {
				Assert.AreSame (mv, ut.MapView, "MapView");
			}
		}

		[Test]
		public void MapView_BackingFields ()
		{
			if (UserTrackingBarButtonItemPoker.NewRefcountEnabled ())
				Assert.Inconclusive ("backing fields are removed when newrefcount is enabled");
			
			using (var mv = new MKMapView ())
			using (var ut = new UserTrackingBarButtonItemPoker (mv)) {
				Assert.AreSame (mv, ut.MapViewBackingField, "1a");
				Assert.AreSame (mv, ut.MapView, "2a");
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
