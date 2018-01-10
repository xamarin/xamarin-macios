//
// Extensions to the ALAsset class
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2009, Novell, Inc.
// Copyright 2011-2014, Xamarin Inc.
//
using ObjCRuntime;
using Foundation;
using CoreGraphics;
using CoreLocation;
using UIKit;
using MediaPlayer;

namespace AssetsLibrary {

	// internally used (not exposed by ObjC)
	[Deprecated (PlatformName.iOS, 9, 0, message : "Use the 'Photos' API instead.")]
	public enum ALAssetType {
		Video, Photo, Unknown
	}

	[Deprecated (PlatformName.iOS, 9, 0, message : "Use the 'Photos' API instead.")]
	public partial class ALAsset {
		public ALAssetType AssetType {
			get {
				var a = ValueForProperty (_PropertyType);
				if (a.Handle == _TypePhoto.Handle)
					return ALAssetType.Photo;
				else if (a.Handle == _TypeVideo.Handle)
					return ALAssetType.Video;
				return ALAssetType.Unknown;
			}
		}

		public CLLocation Location {
			get {
				return (CLLocation) ValueForProperty (_PropertyLocation);
			}
		}

		public double Duration {
			get {
				// note: this can return an NSString like: ALErrorInvalidProperty
				// which causes an InvalidCastException with a normal cast
				NSNumber n = ValueForProperty (_PropertyDuration) as NSNumber;
				return n == null ? double.NaN : n.DoubleValue;
			}
		}

		public ALAssetOrientation Orientation {
			get {
				NSNumber n = (NSNumber) ValueForProperty (_PropertyOrientation);
#if XAMCORE_2_0
				return (ALAssetOrientation) (int)n.NIntValue;
#else
				return (ALAssetOrientation) (int)n.IntValue;
#endif
			}
		}

		public NSDate Date {
			get {
				return (NSDate) ValueForProperty (_PropertyDate);
			}
		}

		public string [] Representations {
			get {
				var k = ValueForProperty (_PropertyRepresentations);
				return NSArray.StringArrayFromHandle (k.Handle);
			}
		}

		public NSDictionary UtiToUrlDictionary {
			get {
				return (NSDictionary) ValueForProperty (_PropertyURLs);
			}
		}

		[iOS (6,0)]
		public NSUrl AssetUrl {
			get {
				// do not show an ArgumentNullException inside the
				// debugger for releases before 6.0
				if (_PropertyAssetURL == null)
					return null;
				return (NSUrl) ValueForProperty (_PropertyAssetURL);
			}
		}
	}

}

