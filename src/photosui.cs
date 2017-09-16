using XamCore.CoreGraphics;
using XamCore.ObjCRuntime;
using XamCore.Foundation;
using XamCore.UIKit;
using XamCore.Photos;
using System;

namespace XamCore.PhotosUI
{
	[NoTV]
	[iOS (8,0)]
	[Protocol]
#if !XAMCORE_4_0 && !TVOS
	// According to documentation you're supposed to implement this protocol in a UIViewController subclass,
	// which means a model (which does not inherit from UIViewController) is not useful.
	[Model]
	[BaseType (typeof (NSObject))]
#endif
	interface PHContentEditingController {

		[Abstract]
		[Export ("canHandleAdjustmentData:")]
		bool CanHandleAdjustmentData (PHAdjustmentData adjustmentData);

		[Abstract]
		[Export ("startContentEditingWithInput:placeholderImage:")]
		void StartContentEditing (PHContentEditingInput contentEditingInput, UIImage placeholderImage);

		[Abstract]
		[Export ("finishContentEditingWithCompletionHandler:")]
		void FinishContentEditing ([NullAllowed] Action<PHContentEditingOutput> completionHandler);

		[Abstract]
		[Export ("cancelContentEditing")]
		void CancelContentEditing ();

		[Abstract]
		[Export ("shouldShowCancelConfirmation")]
		bool ShouldShowCancelConfirmation { get; }
	}

	[TV (10,0)]
	[iOS (9,1)]
	[BaseType (typeof (UIView))]
	interface PHLivePhotoView {

		// inlined (designated initializer)
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[Static]
		[Export ("livePhotoBadgeImageWithOptions:")]
		UIImage GetLivePhotoBadgeImage (PHLivePhotoBadgeOptions badgeOptions);

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		[Protocolize]
		PHLivePhotoViewDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[NullAllowed, Export ("livePhoto", ArgumentSemantic.Strong)]
		PHLivePhoto LivePhoto { get; set; }

		[Export ("playbackGestureRecognizer", ArgumentSemantic.Strong)]
		UIGestureRecognizer PlaybackGestureRecognizer { get; }

		[Export ("muted")]
		bool Muted { [Bind ("isMuted")] get; set; }

		[Export ("startPlaybackWithStyle:")]
		void StartPlayback (PHLivePhotoViewPlaybackStyle playbackStyle);

		[Export ("stopPlayback")]
		void StopPlayback ();
	}

	[TV (10,0)]
	[iOS (9,1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface PHLivePhotoViewDelegate {
		[Export ("livePhotoView:willBeginPlaybackWithStyle:")]
		void WillBeginPlayback (PHLivePhotoView livePhotoView, PHLivePhotoViewPlaybackStyle playbackStyle);

		[Export ("livePhotoView:didEndPlaybackWithStyle:")]
		void DidEndPlayback (PHLivePhotoView livePhotoView, PHLivePhotoViewPlaybackStyle playbackStyle);
	}
}