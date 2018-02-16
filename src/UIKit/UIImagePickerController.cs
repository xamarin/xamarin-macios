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

namespace UIKit {
	public partial class UIImagePickerController {

// the newer (4.1 fields) are defined in uikit.cs
#if !XAMCORE_2_0
		public readonly static NSString MediaType;
		public readonly static NSString OriginalImage;
		public readonly static NSString EditedImage;
		public readonly static NSString CropRect;
		public readonly static NSString MediaURL;
		
		static UIImagePickerController ()
		{
			var handle = Libraries.UIKit.Handle;

			MediaType  = Dlfcn.GetStringConstant (handle, "UIImagePickerControllerMediaType");
			OriginalImage = Dlfcn.GetStringConstant (handle, "UIImagePickerControllerOriginalImage");
			EditedImage = Dlfcn.GetStringConstant (handle, "UIImagePickerControllerEditedImage");
			CropRect = Dlfcn.GetStringConstant (handle, "UIImagePickerControllerCropRect");
			MediaURL = Dlfcn.GetStringConstant (handle, "UIImagePickerControllerMediaURL");
		}
#endif
		
		//
		// The following construct emulates the support for:
		// id<UINavigationControllerDelegate, UIImagePickerControllerDelegate>
		//
		// That is, the type can contain either one, btu we still want it strongly typed
		//
#if XAMCORE_4_0
		public IUIImagePickerControllerDelegate ImagePickerControllerDelegate {
			get {
				return Delegate as IUIImagePickerControllerDelegate;
			}
			set {
				Delegate = value;
			}
		}

		public IUINavigationControllerDelegate NavigationControllerDelegate {
			get {
				return Delegate as IUINavigationControllerDelegate;
			}
			set {
				Delegate = value;
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
				return ((NSString)Info [UIImagePickerController.MediaType]).ToString ();
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
				var nsv = ((NSValue)Info [UIImagePickerController.CropRect]);
				if (nsv == null)
					return null;
				return nsv.CGRectValue;
			}
		}

		public NSUrl MediaUrl {
			get {
				return (NSUrl) Info [UIImagePickerController.MediaURL];
			}
		}

		[iOS (9,1)]
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

		[iOS (11,0)]
		public PHAsset PHAsset {
			get {
				return (PHAsset) Info [UIImagePickerController.PHAsset];
			}
		}

		[iOS (11,0)]
		public NSUrl ImageUrl {
			get {
				return (NSUrl) Info [UIImagePickerController.ImageUrl];
			}
		}
	}
}

#endif // !TVOS && !WATCH
