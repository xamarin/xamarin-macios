using CoreGraphics;
using ObjCRuntime;
using Foundation;
#if !MONOMAC
using UIKit;
// ease compilation for [NoiOS] and [NoTV] decorated members
using NSView = Foundation.NSObject;
using PHLivePhotoViewContentMode = Foundation.NSObject;
#else
using AppKit;
using UIImage = AppKit.NSImage;
// ease compilation for [NoMac] decorated members
using UIGestureRecognizer = Foundation.NSObject;
using PHLivePhotoBadgeOptions = Foundation.NSObject;
#endif
using Photos;
using System;

namespace PhotosUI {
	[NoTV]
	[iOS (8, 0)]
	[Mac (10, 13, onlyOn64: true)]
	[Protocol]
#if !XAMCORE_4_0 && !TVOS && !MONOMAC
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
	[Mac (10,12, onlyOn64: true)]
#if MONOMAC
	[BaseType (typeof (NSView))]
#else
	[BaseType (typeof (UIView))]
#endif
	interface PHLivePhotoView {

		// inlined (designated initializer)
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[NoMac]
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

		[NoMac]
		[Export ("playbackGestureRecognizer", ArgumentSemantic.Strong)]
		UIGestureRecognizer PlaybackGestureRecognizer { get; }

		[Export ("muted")]
		bool Muted { [Bind ("isMuted")] get; set; }

		[Export ("startPlaybackWithStyle:")]
		void StartPlayback (PHLivePhotoViewPlaybackStyle playbackStyle);

		[Export ("stopPlayback")]
		void StopPlayback ();

		[NoiOS]
		[NoTV]
		[Export ("stopPlaybackAnimated:")]
		void StopPlayback (bool animated);

		[NoiOS]
		[NoTV]
		[Export ("contentMode", ArgumentSemantic.Assign)]
		PHLivePhotoViewContentMode ContentMode { get; set; }

		[NoiOS]
		[NoTV]
		[Export ("audioVolume")]
		float AudioVolume { get; set; }

		[NoiOS]
		[NoTV]
		[NullAllowed, Export ("livePhotoBadgeView", ArgumentSemantic.Strong)]
		NSView LivePhotoBadgeView { get; }
	}

	[TV (10,0)]
	[iOS (9,1)]
	[Mac (10,12)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface PHLivePhotoViewDelegate {
		[Export ("livePhotoView:willBeginPlaybackWithStyle:")]
		void WillBeginPlayback (PHLivePhotoView livePhotoView, PHLivePhotoViewPlaybackStyle playbackStyle);

		[Export ("livePhotoView:didEndPlaybackWithStyle:")]
		void DidEndPlayback (PHLivePhotoView livePhotoView, PHLivePhotoViewPlaybackStyle playbackStyle);
	}

	[Mac (10,13, onlyOn64: true)][NoiOS][NoTV][NoWatch]
	[Static]
	interface PHProjectType {
		[Field ("PHProjectTypeUndefined")]
		NSString Undefined { get; }
	}

	[Mac (10,13, onlyOn64 : true)]
	[NoiOS][NoTV]
	[DisableDefaultCtor]
	[BaseType (typeof (NSExtensionContext))]
	interface PHProjectExtensionContext : NSSecureCoding, NSCopying {

		[Export ("photoLibrary")]
		PHPhotoLibrary PhotoLibrary { get; }

		[Export ("project")]
		PHProject Project { get; }
	}

	[Mac (10,13, onlyOn64 : true)]
	[NoiOS][NoTV]
	[DisableDefaultCtor]
	[BaseType (typeof (PHProjectElement))]
	interface PHProjectJournalEntryElement : NSSecureCoding {

		[Export ("date")]
		NSDate Date { get; }

		[NullAllowed, Export ("assetElement")]
		PHProjectAssetElement AssetElement { get; }

		[NullAllowed, Export ("textElement")]
		PHProjectTextElement TextElement { get; }
	}

	[Mac (10,13, onlyOn64 : true)]
	[NoiOS][NoTV]
	[DisableDefaultCtor]
	[BaseType (typeof (PHProjectElement))]
	interface PHProjectTextElement : NSSecureCoding {

		[Export ("text")]
		string Text { get; }

		[NullAllowed, Export ("attributedText")]
		NSAttributedString AttributedText { get; }

		[Export ("textElementType")]
		PHProjectTextElementType TextElementType { get; }
	}

	[Mac (10,13, onlyOn64 : true)]
	[NoiOS][NoTV]
	[Protocol]
	interface PHProjectExtensionController {

		[Export ("supportedProjectTypes", ArgumentSemantic.Copy)]
		PHProjectTypeDescription [] GetSupportedProjectTypes ();

		[Abstract]
		[Export ("beginProjectWithExtensionContext:projectInfo:completion:")]
		void BeginProject (PHProjectExtensionContext extensionContext, PHProjectInfo projectInfo, Action<NSError> completion);

		[Abstract]
		[Export ("resumeProjectWithExtensionContext:completion:")]
		void ResumeProject (PHProjectExtensionContext extensionContext, Action<NSError> completion);

		[Abstract]
		[Export ("finishProjectWithCompletionHandler:")]
		void FinishProject (Action completion);
	}

	[Mac (10,13, onlyOn64 : true)]
	[NoiOS][NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PHProjectTypeDescription : NSSecureCoding {

		[Export ("projectType")]
		NSString ProjectType { get; }

		[Export ("localizedTitle")]
		string LocalizedTitle { get; }

		[NullAllowed, Export ("localizedDescription")]
		string LocalizedDescription { get; }

		[NullAllowed, Export ("image", ArgumentSemantic.Copy)]
		UIImage Image { get; }

		[Export ("subtypeDescriptions", ArgumentSemantic.Copy)]
		PHProjectTypeDescription[] SubtypeDescriptions { get; }

		[Export ("initWithProjectType:title:description:image:subtypeDescriptions:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSString projectType, string localizedTitle, [NullAllowed] string localizedDescription, [NullAllowed] UIImage image, PHProjectTypeDescription[] subtypeDescriptions);

		[Export ("initWithProjectType:title:description:image:")]
		IntPtr Constructor (NSString projectType, string localizedTitle, [NullAllowed] string localizedDescription, [NullAllowed] UIImage image);
	}

	[Mac (10,13, onlyOn64 : true)]
	[NoiOS][NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PHProjectRegionOfInterest : NSSecureCoding {

		[Export ("rect")]
		CGRect Rect { get; }

		[Export ("weight")]
		double Weight { get; }

		[Export ("identifier")]
		string Identifier { get; }
	}

	[Mac (10,13, onlyOn64 : true)]
	[NoiOS][NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PHProjectElement : NSSecureCoding {

		[Export ("weight")]
		double Weight { get; }

		[Export ("placement")]
		CGRect Placement { get; }
	}

	[Mac (10,13, onlyOn64 : true)]
	[NoiOS][NoTV]
	[DisableDefaultCtor]
	[BaseType (typeof (PHProjectElement))]
	interface PHProjectAssetElement : NSSecureCoding {

		[Export ("cloudAssetIdentifier")]
		PHCloudIdentifier CloudAssetIdentifier { get; }

		[Export ("annotation")]
		string Annotation { get; }

		[Export ("cropRect")]
		CGRect CropRect { get; }

		[Export ("regionsOfInterest")]
		PHProjectRegionOfInterest[] RegionsOfInterest { get; }
	}

	[Mac (10,13, onlyOn64 : true)]
	[NoiOS][NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PHProjectInfo : NSSecureCoding {

		[Export ("creationSource")]
		PHProjectCreationSource CreationSource { get; }

		[Export ("projectType")]
		NSString ProjectType { get; }

		[Export ("sections")]
		PHProjectSection[] Sections { get; }
	}

	[Mac (10,13, onlyOn64 : true)]
	[NoiOS][NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PHProjectSection : NSSecureCoding {

		[Export ("sectionContents")]
		PHProjectSectionContent[] SectionContents { get; }

		[Export ("sectionType")]
		PHProjectSectionType SectionType { get; }

		[Export ("title")]
		string Title { get; }
	}

	[Mac (10,13, onlyOn64 : true)]
	[NoiOS][NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PHProjectSectionContent : NSSecureCoding {

		[Export ("elements")]
		PHProjectElement[] Elements { get; }

		[Export ("numberOfColumns")]
		nint NumberOfColumns { get; }

		[Export ("aspectRatio")]
		double AspectRatio { get; }

		[Export ("cloudAssetIdentifiers")]
		PHCloudIdentifier[] CloudAssetIdentifiers { get; }
	}
}