//
// UIImagePickerContrller.cs
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2009, Novell, Inc.
// Copyright 2012-2014 Xamarin Inc
//

#if !TVOS && !WATCH // __TVOS_PROHIBITED, doesn't show up in WatchOS headers

using ObjCRuntime;
using Foundation;
using CoreGraphics;
using Photos;
using System;
using System.Drawing;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace UIKit {
	public partial class UIImagePickerController {
		//
		// The following construct emulates the support for:
		// id<UINavigationControllerDelegate, UIImagePickerControllerDelegate>
		//
		// That is, the type can contain either one, but we still want it strongly typed
		//
#if NET
		public IUIImagePickerControllerDelegate ImagePickerControllerDelegate {
			get {
				return Delegate as IUIImagePickerControllerDelegate;
			}
			set {
				Delegate = (NSObject) value;
			}
		}

		public IUINavigationControllerDelegate NavigationControllerDelegate {
			get {
				return Delegate as IUINavigationControllerDelegate;
			}
			set {
				Delegate = (NSObject) value;
			}
		}
#else
		public UIImagePickerControllerDelegate ImagePickerControllerDelegate {
			get {
				return Delegate as UIImagePickerControllerDelegate;
			}

			set {
				Delegate = value;
			}
		}

		public UINavigationControllerDelegate NavigationControllerDelegate {
			get {
				return Delegate as UINavigationControllerDelegate;
			}

			set {
				Delegate = value;
			}
		}
#endif
	}

	public partial class UIImagePickerMediaPickedEventArgs {
		public string MediaType {
			get {
				return ((NSString) Info [UIImagePickerController.MediaType]).ToString ();
			}
		}

		public UIImage OriginalImage {
			get {
				return (UIImage) Info [UIImagePickerController.OriginalImage];
			}
		}

		public UIImage EditedImage {
			get {
				return (UIImage) Info [UIImagePickerController.EditedImage];
			}
		}

		public CGRect? CropRect {
			get {
				var nsv = ((NSValue) Info [UIImagePickerController.CropRect]);
				if (nsv is null)
					return null;
				return nsv.CGRectValue;
			}
		}

		public NSUrl MediaUrl {
			get {
				return (NSUrl) Info [UIImagePickerController.MediaURL];
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public PHLivePhoto LivePhoto {
			get {
				return (PHLivePhoto) Info [UIImagePickerController.LivePhoto];
			}
		}

		public NSDictionary MediaMetadata {
			get {
				return (NSDictionary) Info [UIImagePickerController.MediaMetadata];
			}
		}

		public NSUrl ReferenceUrl {
			get {
				return (NSUrl) Info [UIImagePickerController.ReferenceUrl];
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public PHAsset PHAsset {
			get {
				return (PHAsset) Info [UIImagePickerController.PHAsset];
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public NSUrl ImageUrl {
			get {
				return (NSUrl) Info [UIImagePickerController.ImageUrl];
			}
		}
	}
}

#endif // !TVOS && !WATCH
