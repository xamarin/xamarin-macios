//
// This file describes extensions to the ALAssetsGroup class
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2009, Novell, Inc.
// Copyright 2011, Xamarin, Inc.
//
using ObjCRuntime;
using Foundation;
using CoreGraphics;
using CoreLocation;
using UIKit;
using MediaPlayer;

#nullable enable

namespace AssetsLibrary {

#if !NET
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Photos' API instead.")]
	// dotnet deprecation is handled by partial class in assetslibrary.cs
#endif
	public partial class ALAssetsGroup {
		public NSString Name {
			get {
				return (NSString) ValueForProperty (_Name);
			}
		}

		public ALAssetsGroupType Type {
			get {
				return (ALAssetsGroupType) (int) ((NSNumber) ValueForProperty (_Type)).NIntValue;
			}
		}

		public string PersistentID {
			get {
				return (NSString) ValueForProperty (_PersistentID);
			}
		}

		public NSUrl PropertyUrl {
			get {
				return (NSUrl) ValueForProperty (_PropertyUrl);
			}
		}
	}

}
