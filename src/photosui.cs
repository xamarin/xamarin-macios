using CoreGraphics;
using CoreLocation;
using ObjCRuntime;
using Foundation;
#if !MONOMAC
using UIKit;
using NSView = Foundation.NSObject;
using PHLivePhotoViewContentMode = Foundation.NSObject;
#else
using AppKit;
using UITouch = Foundation.NSObject;
using UIImage = AppKit.NSImage;
using UIColor = AppKit.NSColor;
using UIGestureRecognizer = Foundation.NSObject;
using PHLivePhotoBadgeOptions = Foundation.NSObject;
using UIViewController = AppKit.NSViewController;
#endif
using MapKit;
using Photos;
using System;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace PhotosUI {
	[NoTV]
	[MacCatalyst (14, 0)]
	[Protocol]
#if !NET && !TVOS && !MONOMAC
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
		void FinishContentEditing (Action<PHContentEditingOutput> completionHandler);

		[Abstract]
		[Export ("cancelContentEditing")]
		void CancelContentEditing ();

		[Abstract]
		[Export ("shouldShowCancelConfirmation")]
		bool ShouldShowCancelConfirmation { get; }
	}

	[MacCatalyst (13, 1)]
#if MONOMAC
	[BaseType (typeof (NSView))]
#else
	[BaseType (typeof (UIView))]
#endif
	interface PHLivePhotoView {

		// inlined (designated initializer)
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("livePhotoBadgeImageWithOptions:")]
		UIImage GetLivePhotoBadgeImage (PHLivePhotoBadgeOptions badgeOptions);

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IPHLivePhotoViewDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[NullAllowed, Export ("livePhoto", ArgumentSemantic.Strong)]
		PHLivePhoto LivePhoto { get; set; }

		[NoMac]
		[MacCatalyst (13, 1)]
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
		[NoMacCatalyst]
		[Export ("stopPlaybackAnimated:")]
		void StopPlayback (bool animated);

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Export ("contentMode", ArgumentSemantic.Assign)]
		PHLivePhotoViewContentMode ContentMode { get; set; }

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Export ("audioVolume")]
		float AudioVolume { get; set; }

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[NullAllowed, Export ("livePhotoBadgeView", ArgumentSemantic.Strong)]
		NSView LivePhotoBadgeView { get; }

		[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("contentsRect", ArgumentSemantic.Assign)]
		CGRect ContentsRect { get; set; }
	}

	interface IPHLivePhotoViewDelegate { }

	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface PHLivePhotoViewDelegate {
		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("livePhotoView:canBeginPlaybackWithStyle:")]
		bool CanBeginPlayback (PHLivePhotoView livePhotoView, PHLivePhotoViewPlaybackStyle playbackStyle);

		[Export ("livePhotoView:willBeginPlaybackWithStyle:")]
		void WillBeginPlayback (PHLivePhotoView livePhotoView, PHLivePhotoViewPlaybackStyle playbackStyle);

		[Export ("livePhotoView:didEndPlaybackWithStyle:")]
		void DidEndPlayback (PHLivePhotoView livePhotoView, PHLivePhotoViewPlaybackStyle playbackStyle);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("livePhotoView:extraMinimumTouchDurationForTouch:withStyle:")]
		double GetExtraMinimumTouchDuration (PHLivePhotoView livePhotoView, UITouch touch, PHLivePhotoViewPlaybackStyle playbackStyle);
	}

	[NoiOS]
	[NoTV]
	[NoWatch]
	[NoMacCatalyst]
	[Static]
	interface PHProjectType {
		[Field ("PHProjectTypeUndefined")]
		NSString Undefined { get; }
	}

	[NoiOS]
	[NoTV]
	[NoMacCatalyst]
	[DisableDefaultCtor]
	[BaseType (typeof (NSExtensionContext))]
	interface PHProjectExtensionContext : NSSecureCoding, NSCopying {

		[Export ("photoLibrary")]
		PHPhotoLibrary PhotoLibrary { get; }

		[Export ("project")]
		PHProject Project { get; }

		[NoMacCatalyst]
		[Export ("showEditorForAsset:")]
		void ShowEditor (PHAsset asset);

		[NoMacCatalyst]
		[Export ("updatedProjectInfoFromProjectInfo:completion:")]
		NSProgress UpdatedProjectInfo ([NullAllowed] PHProjectInfo existingProjectInfo, Action<PHProjectInfo> completionHandler);
	}

	[NoiOS]
	[NoTV]
	[NoMacCatalyst]
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

	[NoiOS]
	[NoTV]
	[NoMacCatalyst]
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

	[NoiOS]
	[NoTV]
	[NoMacCatalyst]
	[Protocol]
	interface PHProjectExtensionController {

		[Deprecated (PlatformName.MacOSX, 10, 14)]
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

		[Protected]
		[NoMacCatalyst]
		[Export ("typeDescriptionDataSourceForCategory:invalidator:")]
		IPHProjectTypeDescriptionDataSource GetTypeDescriptionDataSource (NSString category, IPHProjectTypeDescriptionInvalidator invalidator);

		[Wrap ("GetTypeDescriptionDataSource (category.GetConstant(), invalidator)")]
		IPHProjectTypeDescriptionDataSource GetTypeDescriptionDataSource (PHProjectCategory category, IPHProjectTypeDescriptionInvalidator invalidator);
	}

	[NoiOS]
	[NoTV]
	[NoMacCatalyst]
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
		PHProjectTypeDescription [] SubtypeDescriptions { get; }

		[Export ("initWithProjectType:title:description:image:subtypeDescriptions:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSString projectType, string localizedTitle, [NullAllowed] string localizedDescription, [NullAllowed] UIImage image, PHProjectTypeDescription [] subtypeDescriptions);

		[Export ("initWithProjectType:title:description:image:")]
		NativeHandle Constructor (NSString projectType, string localizedTitle, [NullAllowed] string localizedDescription, [NullAllowed] UIImage image);

		[NoMacCatalyst]
		[Export ("initWithProjectType:title:attributedDescription:image:subtypeDescriptions:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSString projectType, string localizedTitle, [NullAllowed] NSAttributedString localizedAttributedDescription, [NullAllowed] UIImage image, PHProjectTypeDescription [] subtypeDescriptions);

		[NoMacCatalyst]
		[Export ("initWithProjectType:title:attributedDescription:image:canProvideSubtypes:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSString projectType, string localizedTitle, [NullAllowed] NSAttributedString localizedAttributedDescription, [NullAllowed] UIImage image, bool canProvideSubtypes);

		[NoMacCatalyst]
		[Export ("initWithProjectType:title:description:image:canProvideSubtypes:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSString projectType, string localizedTitle, [NullAllowed] string localizedDescription, [NullAllowed] UIImage image, bool canProvideSubtypes);

		[NoMacCatalyst]
		[Export ("canProvideSubtypes")]
		bool CanProvideSubtypes { get; }

		[NoMacCatalyst]
		[NullAllowed, Export ("localizedAttributedDescription", ArgumentSemantic.Copy)]
		NSAttributedString LocalizedAttributedDescription { get; }
	}

	[NoiOS]
	[NoTV]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PHProjectRegionOfInterest : NSSecureCoding {

		[Export ("rect")]
		CGRect Rect { get; }

		[Export ("weight")]
		double Weight { get; }

		[Export ("identifier")]
		string Identifier { get; }

		[NoMacCatalyst]
		[Export ("quality")]
		double Quality { get; }
	}

	[NoiOS]
	[NoTV]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PHProjectElement : NSSecureCoding {

		[Export ("weight")]
		double Weight { get; }

		[Export ("placement")]
		CGRect Placement { get; }
	}

	[NoiOS]
	[NoTV]
	[NoMacCatalyst]
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
		PHProjectRegionOfInterest [] RegionsOfInterest { get; }

		[NoMacCatalyst]
		[Export ("horizontallyFlipped")]
		bool HorizontallyFlipped { get; }

		[NoMacCatalyst]
		[Export ("verticallyFlipped")]
		bool VerticallyFlipped { get; }
	}

	[NoiOS]
	[NoTV]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PHProjectInfo : NSSecureCoding {

		[Export ("creationSource")]
		PHProjectCreationSource CreationSource { get; }

		[Export ("projectType")]
		NSString ProjectType { get; }

		[Export ("sections")]
		PHProjectSection [] Sections { get; }

		[NoMacCatalyst]
		[Export ("brandingEnabled")]
		bool BrandingEnabled { get; }

		[NoMacCatalyst]
		[Export ("pageNumbersEnabled")]
		bool PageNumbersEnabled { get; }

		[NoMacCatalyst]
		[NullAllowed, Export ("productIdentifier")]
		string ProductIdentifier { get; }

		[NoMacCatalyst]
		[NullAllowed, Export ("themeIdentifier")]
		string ThemeIdentifier { get; }
	}

	[NoiOS]
	[NoTV]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PHProjectSection : NSSecureCoding {

		[Export ("sectionContents")]
		PHProjectSectionContent [] SectionContents { get; }

		[Export ("sectionType")]
		PHProjectSectionType SectionType { get; }

		[Export ("title")]
		string Title { get; }
	}

	[NoiOS]
	[NoTV]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PHProjectSectionContent : NSSecureCoding {

		[Export ("elements")]
		PHProjectElement [] Elements { get; }

		[Export ("numberOfColumns")]
		nint NumberOfColumns { get; }

		[Export ("aspectRatio")]
		double AspectRatio { get; }

		[Export ("cloudAssetIdentifiers")]
		PHCloudIdentifier [] CloudAssetIdentifiers { get; }

		[NoMacCatalyst]
		[NullAllowed, Export ("backgroundColor")]
		UIColor BackgroundColor { get; }
	}

	[NoiOS]
	[NoTV]
	[NoMacCatalyst]
	[DisableDefaultCtor]
	[BaseType (typeof (PHProjectElement))]
	interface PHProjectMapElement : NSSecureCoding {
		[Export ("mapType")]
		MKMapType MapType { get; }

		[Export ("centerCoordinate")]
		CLLocationCoordinate2D CenterCoordinate { get; }

		[Export ("heading")]
		double Heading { get; }

		[Export ("pitch")]
		nfloat Pitch { get; }

		[Export ("altitude")]
		double Altitude { get; }

		[Export ("annotations", ArgumentSemantic.Copy)]
		IMKAnnotation [] Annotations { get; }
	}

	interface IPHProjectTypeDescriptionDataSource { }
	[NoiOS]
	[NoTV]
	[NoMacCatalyst]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface PHProjectTypeDescriptionDataSource {
		[Abstract]
		[Export ("subtypesForProjectType:")]
		PHProjectTypeDescription [] GetSubtypes (NSString projectType);

		[Abstract]
		[Export ("typeDescriptionForProjectType:")]
		[return: NullAllowed]
		PHProjectTypeDescription GetTypeDescription (NSString projectType);

		[Abstract]
		[Export ("footerTextForSubtypesOfProjectType:")]
		[return: NullAllowed]
		NSAttributedString GetFooterTextForSubtypes (NSString projectType);

		[Export ("extensionWillDiscardDataSource")]
		void WillDiscardDataSource ();
	}

	interface IPHProjectTypeDescriptionInvalidator { }
	[NoiOS]
	[NoTV]
	[NoMacCatalyst]
	[Protocol]
	interface PHProjectTypeDescriptionInvalidator {
		[Abstract]
		[Export ("invalidateTypeDescriptionForProjectType:")]
		void InvalidateTypeDescription (NSString projectType);

		[Abstract]
		[Export ("invalidateFooterTextForSubtypesOfProjectType:")]
		void InvalidateFooterTextForSubtypes (NSString projectType);
	}

	[NoMac]
	[NoTV]
	[DisableDefaultCtor]
	[NoMacCatalyst]
#if !NET // Can't apply Deprecated and Obsoleted to same element
	[Deprecated (PlatformName.iOS, 13, 0)]
#endif
	[Obsoleted (PlatformName.iOS, 14, 0)] // Removed from headers completely
	[BaseType (typeof (NSExtensionContext))]
	interface PHEditingExtensionContext {
	}

	interface IPHPickerViewControllerDelegate { }

	[NoWatch, NoTV, Mac (13, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface PHPickerViewControllerDelegate {
		[Abstract]
		[Export ("picker:didFinishPicking:")]
		void DidFinishPicking (PHPickerViewController picker, PHPickerResult [] results);
	}

	[NoWatch, NoTV, Mac (13, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (UIViewController))]
	[Advice ("This type should not be subclassed.")]
	[DisableDefaultCtor]
	interface PHPickerViewController {
		[Export ("configuration", ArgumentSemantic.Copy)]
		PHPickerConfiguration Configuration { get; }

		[Wrap ("WeakDelegate")]
		IPHPickerViewControllerDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Export ("initWithConfiguration:")]
		[DesignatedInitializer]
		NativeHandle Constructor (PHPickerConfiguration configuration);

		[NoWatch, NoTV, Mac (13, 0), iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Export ("deselectAssetsWithIdentifiers:")]
		void DeselectAssets (string [] identifiers);

		[NoWatch, NoTV, Mac (13, 0), iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Export ("moveAssetWithIdentifier:afterAssetWithIdentifier:")]
		void MoveAsset (string identifier, [NullAllowed] string afterIdentifier);

		[NoWatch, NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("updatePickerUsingConfiguration:")]
		void UpdatePicker (PHPickerUpdateConfiguration configuration);

		[NoWatch, NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("scrollToInitialPosition")]
		void ScrollToInitialPosition ();

		[NoWatch, NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("zoomIn")]
		void ZoomIn ();

		[NoWatch, NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("zoomOut")]
		void ZoomOut ();
	}

	[NoWatch, NoTV, Mac (13, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[Advice ("This type should not be subclassed.")]
	interface PHPickerConfiguration : NSCopying {
		[Export ("preferredAssetRepresentationMode", ArgumentSemantic.Assign)]
		PHPickerConfigurationAssetRepresentationMode PreferredAssetRepresentationMode { get; set; }

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("selection", ArgumentSemantic.Assign)]
		PHPickerConfigurationSelection Selection { get; set; }

		[Export ("selectionLimit")]
		nint SelectionLimit { get; set; }

		[NullAllowed, Export ("filter", ArgumentSemantic.Copy)]
		PHPickerFilter Filter { get; set; }

		[Export ("initWithPhotoLibrary:")]
		NativeHandle Constructor (PHPhotoLibrary photoLibrary);

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("preselectedAssetIdentifiers", ArgumentSemantic.Copy)]
		string [] PreselectedAssetIdentifiers { get; set; }

		[Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("mode", ArgumentSemantic.Assign)]
		PHPickerMode Mode { get; set; }

		[Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("edgesWithoutContentMargins", ArgumentSemantic.Assign)]
		NSDirectionalRectEdge EdgesWithoutContentMargins { get; set; }

		[Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("disabledCapabilities", ArgumentSemantic.Assign)]
		PHPickerCapabilities DisabledCapabilities { get; set; }
	}

	[NoWatch, NoTV, Mac (13, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[Advice ("This type should not be subclassed.")]
	[DisableDefaultCtor]
	interface PHPickerFilter : NSCopying {
		[Static]
		[Export ("imagesFilter")]
		PHPickerFilter ImagesFilter { get; }

		[Static]
		[Export ("videosFilter")]
		PHPickerFilter VideosFilter { get; }

		[Static]
		[Export ("livePhotosFilter")]
		PHPickerFilter LivePhotosFilter { get; }

		[Static]
		[Export ("anyFilterMatchingSubfilters:")]
		PHPickerFilter GetAnyFilterMatchingSubfilters (PHPickerFilter [] subfilters);

		[NoWatch, NoTV, Mac (13, 0), iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Static]
		[Export ("depthEffectPhotosFilter")]
		PHPickerFilter DepthEffectPhotosFilter { get; }

		[NoWatch, NoTV, Mac (13, 0), iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Static]
		[Export ("burstsFilter")]
		PHPickerFilter BurstsFilter { get; }

		[NoWatch, NoTV, Mac (13, 0), iOS (15, 0)]
		[MacCatalyst (15, 0)]
		[Static]
		[Export ("panoramasFilter")]
		PHPickerFilter PanoramasFilter { get; }

		[NoWatch, NoTV, Mac (13, 0), iOS (15, 0)]
		[MacCatalyst (15, 0)]
		[Static]
		[Export ("screenshotsFilter")]
		PHPickerFilter ScreenshotsFilter { get; }

		[NoWatch, NoTV, Mac (13, 0), iOS (15, 0)]
		[MacCatalyst (15, 0)]
		[Static]
		[Export ("screenRecordingsFilter")]
		PHPickerFilter ScreenRecordingsFilter { get; }

		[NoWatch, NoTV, Mac (13, 0), iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Static]
		[Export ("cinematicVideosFilter")]
		PHPickerFilter CinematicVideosFilter { get; }

		[NoWatch, NoTV, Mac (13, 0), iOS (15, 0)]
		[MacCatalyst (15, 0)]
		[Static]
		[Export ("slomoVideosFilter")]
		PHPickerFilter SlomoVideosFilter { get; }

		[NoWatch, NoTV, Mac (13, 0), iOS (15, 0)]
		[MacCatalyst (15, 0)]
		[Static]
		[Export ("timelapseVideosFilter")]
		PHPickerFilter TimelapseVideosFilter { get; }

		[NoWatch, NoTV, Mac (13, 0), iOS (15, 0)]
		[MacCatalyst (15, 0)]
		[Static]
		[Export ("playbackStyleFilter:")]
		PHPickerFilter GetPlaybackStyleFilter (PHAssetPlaybackStyle playbackStyle);

		[NoWatch, NoTV, Mac (13, 0), iOS (15, 0)]
		[MacCatalyst (15, 0)]
		[Static]
		[Export ("allFilterMatchingSubfilters:")]
		PHPickerFilter GetAllFilterMatchingSubfilters (PHPickerFilter [] subfilters);

		[NoWatch, NoTV, Mac (13, 0), iOS (15, 0)]
		[MacCatalyst (15, 0)]
		[Static]
		[Export ("notFilterOfSubfilter:")]
		PHPickerFilter GetNotFilterOfSubfilter (PHPickerFilter subfilter);
	}

	[NoWatch, NoTV, Mac (13, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[Advice ("This type should not be subclassed.")]
	[DisableDefaultCtor]
	interface PHPickerResult {
		[Export ("itemProvider")]
		NSItemProvider ItemProvider { get; }

		[NullAllowed, Export ("assetIdentifier")]
		string AssetIdentifier { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Category]
	[BaseType (typeof (PHPhotoLibrary))]
	interface PHPhotoLibrary_PhotosUISupport {
		[Export ("presentLimitedLibraryPickerFromViewController:")]
		void PresentLimitedLibraryPicker (UIViewController controller);

		[Async]
		[iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("presentLimitedLibraryPickerFromViewController:completionHandler:")]
		void PresentLimitedLibraryPicker (UIViewController controller, Action<string []> completionHandler);
	}

	[NoWatch, NoTV, Mac (13, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum PHPickerConfigurationSelection : long {
		Default = 0,
		Ordered = 1,
		[Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		Continuous = 2,
		[Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		ContinuousAndOrdered = 3,
	}

	[NoWatch, NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface PHPickerUpdateConfiguration : NSCopying, NSSecureCoding {
		[Export ("selectionLimit")]
		nint SelectionLimit { get; set; }

		[Export ("edgesWithoutContentMargins", ArgumentSemantic.Assign)]
		NSDirectionalRectEdge EdgesWithoutContentMargins { get; set; }
	}
}
