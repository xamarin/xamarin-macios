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
using CoreFoundation;
using CoreLocation;
using UIKit;
using MediaPlayer;

#nullable enable

namespace AssetsLibrary {

#if !NET
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Photos' API instead.")]
	// dotnet deprecation is handled by partial class in assetslibrary.cs
#endif
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
				var n = ValueForProperty (_PropertyDuration) as NSNumber;
				return n?.DoubleValue ?? double.NaN;
			}
		}

		public ALAssetOrientation Orientation {
			get {
				NSNumber n = (NSNumber) ValueForProperty (_PropertyOrientation);
				return (ALAssetOrientation) (int) n.NIntValue;
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
				return CFArray.StringArrayFromHandle (k.Handle)!;
			}
		}

		public NSDictionary UtiToUrlDictionary {
			get {
				return (NSDictionary) ValueForProperty (_PropertyURLs);
			}
		}

		public NSUrl? AssetUrl {
			get {
				// do not show an ArgumentNullException inside the
				// debugger for releases before 6.0
				return _PropertyAssetURL is not null ? (NSUrl) ValueForProperty (_PropertyAssetURL) : null;
			}
		}
	}

}
