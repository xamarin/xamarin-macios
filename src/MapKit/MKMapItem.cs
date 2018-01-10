//
// Authors:
//   Miguel de Icaza
//
// Copyright 2011-2014 Xamarin Inc.
//

#if !TVOS && (XAMCORE_2_0 || !MONOMAC)

using Foundation;
using CoreLocation;
using ObjCRuntime;

namespace MapKit {

	// it's similar to MKDirectionsTransportType values but it's something only used on the managed side
	// to replace NSString fields
	public enum MKDirectionsMode {
		Driving, Walking, Transit,
		[iOS (10,0)][NoTV][Watch (3,0)][Mac (10,12)]
		Default
	}
	
	public class MKLaunchOptions
	{
		public MKDirectionsMode? DirectionsMode { get; set; }
#if !WATCH // MapType: __WATCHOS_PROHIBITED
		public MKMapType? MapType { get; set; }
#endif
		public CLLocationCoordinate2D? MapCenter { get; set; }
		public MKCoordinateSpan? MapSpan { get; set; }
#if !WATCH // ShowTraffic: __WATCHOS_PROHIBITED
		public bool? ShowTraffic { get; set; }
#endif

#if !WATCH // The corresponding key (MKLaunchOptionsCameraKey) is allowed in WatchOS, but there's no MKMapCamera type.
		[iOS (7,0)]
		public MKMapCamera Camera { get; set; }
#endif

		internal NSDictionary ToDictionary ()
		{
			int n = 0;
			if (DirectionsMode.HasValue) n++;
#if !WATCH
			if (MapType.HasValue) n++;
#endif
			if (MapCenter.HasValue) n++;
			if (MapSpan.HasValue) n++;
#if !WATCH
			if (ShowTraffic.HasValue) n++;
			if (Camera != null) n++;
#endif
			if (n == 0)
				return null;
			
			var keys = new NSObject [n];
			var values = new NSObject [n];
			int i = 0;
			if (DirectionsMode.HasValue){
				keys [i] = MKMapItem.MKLaunchOptionsDirectionsModeKey;
				NSString v = MKMapItem.MKLaunchOptionsDirectionsModeDriving;
				switch (DirectionsMode.Value){
				case MKDirectionsMode.Driving:
					v = MKMapItem.MKLaunchOptionsDirectionsModeDriving;
					break;
				case MKDirectionsMode.Transit:
					v = MKMapItem.MKLaunchOptionsDirectionsModeTransit;
					break;
				case MKDirectionsMode.Walking:
					v = MKMapItem.MKLaunchOptionsDirectionsModeWalking;
					break;
#if !TV
				case MKDirectionsMode.Default:
					v = MKMapItem.MKLaunchOptionsDirectionsModeDefault;
					break;
#endif
				}
				values [i++] = v;
			}

#if !WATCH // MapType: __WATCHOS_PROHIBITED
			if (MapType.HasValue){
				keys [i] = MKMapItem.MKLaunchOptionsMapTypeKey;
				values [i++] = new NSNumber ((int) MapType.Value);
			}
#endif
			if (MapCenter.HasValue){
				keys [i] = MKMapItem.MKLaunchOptionsMapCenterKey;
				values [i++] = NSValue.FromMKCoordinate (MapCenter.Value);
			}
			if (MapSpan.HasValue){
				keys [i] = MKMapItem.MKLaunchOptionsMapSpanKey;
				values [i++] = NSValue.FromMKCoordinateSpan (MapSpan.Value);
			}
#if !WATCH // ShowsTraffic: __WATCHOS_PROHIBITED
			if (ShowTraffic.HasValue){
				keys [i] = MKMapItem.MKLaunchOptionsShowsTrafficKey;
				values [i++] = new NSNumber (ShowTraffic.Value);
			}
#endif
#if !WATCH // MKLaunchOptionsCameraKey is allowed in WatchOS, but there's no MKMapCamera type.
			if (Camera != null) {
				keys [i] = MKMapItem.MKLaunchOptionsCameraKey;
				values [i++] = Camera;
			}
#endif
			return NSDictionary.FromObjectsAndKeys (values, keys);
		}
	}
	
	public partial class MKMapItem {
		public void OpenInMaps (MKLaunchOptions launchOptions = null)
		{
			_OpenInMaps (launchOptions != null ? launchOptions.ToDictionary () : null);
		}

		public static bool OpenMaps (MKMapItem [] mapItems = null, MKLaunchOptions launchOptions = null)
		{
			return _OpenMaps (mapItems, launchOptions != null ? launchOptions.ToDictionary () : null);
		}
	}
	
}
#endif
