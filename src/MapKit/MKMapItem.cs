//
// Authors:
//   Miguel de Icaza
//
// Copyright 2011-2014 Xamarin Inc.
//

#if !TVOS


using Foundation;
using CoreLocation;
using ObjCRuntime;

#nullable enable

namespace MapKit {

	// it's similar to MKDirectionsTransportType values but it's something only used on the managed side
	// to replace NSString fields
	public enum MKDirectionsMode {
		Driving, Walking, Transit,
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[NoTV]
#endif
		Default
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#endif
	public class MKLaunchOptions {
		public MKDirectionsMode? DirectionsMode { get; set; }
		public MKMapType? MapType { get; set; }
		public CLLocationCoordinate2D? MapCenter { get; set; }
		public MKCoordinateSpan? MapSpan { get; set; }
		public bool? ShowTraffic { get; set; }


#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
#endif
		public MKMapCamera? Camera { get; set; }

		internal NSDictionary? ToDictionary ()
		{
			int n = 0;
			if (DirectionsMode.HasValue) n++;
			if (MapType.HasValue) n++;
			if (MapCenter.HasValue) n++;
			if (MapSpan.HasValue) n++;
			if (ShowTraffic.HasValue) n++;
			if (Camera is not null) n++;
			if (n == 0)
				return null;

			var keys = new NSObject [n];
			var values = new NSObject [n];
			int i = 0;
			if (DirectionsMode.HasValue) {
				keys [i] = MKMapItem.MKLaunchOptionsDirectionsModeKey;
				NSString v = MKMapItem.MKLaunchOptionsDirectionsModeDriving;
				switch (DirectionsMode.Value) {
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

			if (MapType.HasValue) {
				keys [i] = MKMapItem.MKLaunchOptionsMapTypeKey;
				values [i++] = new NSNumber ((int) MapType.Value);
			}
			if (MapCenter.HasValue) {
				keys [i] = MKMapItem.MKLaunchOptionsMapCenterKey;
				values [i++] = NSValue.FromMKCoordinate (MapCenter.Value);
			}
			if (MapSpan.HasValue) {
				keys [i] = MKMapItem.MKLaunchOptionsMapSpanKey;
				values [i++] = NSValue.FromMKCoordinateSpan (MapSpan.Value);
			}
			if (ShowTraffic.HasValue) {
				keys [i] = MKMapItem.MKLaunchOptionsShowsTrafficKey;
				values [i++] = new NSNumber (ShowTraffic.Value);
			}
			if (Camera is not null) {
				keys [i] = MKMapItem.MKLaunchOptionsCameraKey;
				values [i++] = Camera;
			}
			return NSDictionary.FromObjectsAndKeys (values, keys);
		}
	}

	public partial class MKMapItem {
		public void OpenInMaps (MKLaunchOptions? launchOptions = null)
		{
			_OpenInMaps (launchOptions?.ToDictionary ());
		}

		public static bool OpenMaps (MKMapItem [] mapItems, MKLaunchOptions? launchOptions = null)
		{
			return _OpenMaps (mapItems, launchOptions?.ToDictionary ());
		}
	}

}
#endif
