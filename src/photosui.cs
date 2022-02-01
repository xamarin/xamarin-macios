using CoreGraphics;
using CoreLocation;
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
using UIColor = AppKit.NSColor;
// ease compilation for [NoMac] decorated members
using UIGestureRecognizer = Foundation.NSObject;
using PHLivePhotoBadgeOptions = Foundation.NSObject;
using UIViewController = Foundation.NSObject;
#endif
using MapKit;
using Photos;
using System;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace PhotosUI {
	[NoTV]
	[iOS (8, 0)]
	[Mac (10, 13)]
	[MacCatalyst (14,0)]
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

	[TV (10,0)]
	[iOS (9,1)]
	[Mac (10,12)]
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
		[TV (15,0), iOS (15,0), MacCatalyst (15,0), Mac (12,0)]
		[Export ("livePhotoView:canBeginPlaybackWithStyle:")]
		bool CanBeginPlayback (PHLivePhotoView livePhotoView, PHLivePhotoViewPlaybackStyle playbackStyle);

		[Export ("livePhotoView:willBeginPlaybackWithStyle:")]
		void WillBeginPlayback (PHLivePhotoView livePhotoView, PHLivePhotoViewPlaybackStyle playbackStyle);

		[Export ("livePhotoView:didEndPlaybackWithStyle:")]
		void DidEndPlayback (PHLivePhotoView livePhotoView, PHLivePhotoViewPlaybackStyle playbackStyle);
	}

	[Mac (10,13)][NoiOS][NoTV][NoWatch]
	[Static]
	interface PHProjectType {
		[Field ("PHProjectTypeUndefined")]
		NSString Undefined { get; }
	}

	[Mac (10,13)]
	[NoiOS][NoTV]
	[DisableDefaultCtor]
	[BaseType (typeof (NSExtensionContext))]
	interface PHProjectExtensionContext : NSSecureCoding, NSCopying {

		[Export ("photoLibrary")]
		PHPhotoLibrary PhotoLibrary { get; }

		[Export ("project")]
		PHProject Project { get; }

		[Mac (10,14)]
		[Export ("showEditorForAsset:")]
		void ShowEditor (PHAsset asset);

		[Mac (10,14)]
		[Export ("updatedProjectInfoFromProjectInfo:completion:")]
		NSProgress UpdatedProjectInfo ([NullAllowed] PHProjectInfo existingProjectInfo, Action<PHProjectInfo> completionHandler);
	}

	[Mac (10,13)]
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

	[Mac (10,13)]
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

	[Mac (10,13)]
	[NoiOS][NoTV]
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
		[Mac (10,14)]
		[Export ("typeDescriptionDataSourceForCategory:invalidator:")]
		IPHProjectTypeDescriptionDataSource GetTypeDescriptionDataSource (NSString category, IPHProjectTypeDescriptionInvalidator invalidator);

		[Wrap ("GetTypeDescriptionDataSource (category.GetConstant(), invalidator)")]
		IPHProjectTypeDescriptionDataSource GetTypeDescriptionDataSource (PHProjectCategory category, IPHProjectTypeDescriptionInvalidator invalidator);
	}

	[Mac (10,13)]
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
		NativeHandle Constructor (NSString projectType, string localizedTitle, [NullAllowed] string localizedDescription, [NullAllowed] UIImage image, PHProjectTypeDescription[] subtypeDescriptions);

		[Export ("initWithProjectType:title:description:image:")]
		NativeHandle Constructor (NSString projectType, string localizedTitle, [NullAllowed] string localizedDescription, [NullAllowed] UIImage image);

		[Mac (10,14)]
		[Export ("initWithProjectType:title:attributedDescription:image:subtypeDescriptions:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSString projectType, string localizedTitle, [NullAllowed] NSAttributedString localizedAttributedDescription, [NullAllowed] UIImage image, PHProjectTypeDescription[] subtypeDescriptions);

		[Mac (10,14)]
		[Export ("initWithProjectType:title:attributedDescription:image:canProvideSubtypes:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSString projectType, string localizedTitle, [NullAllowed] NSAttributedString localizedAttributedDescription, [NullAllowed] UIImage image, bool canProvideSubtypes);

		[Mac (10,14)]
		[Export ("initWithProjectType:title:description:image:canProvideSubtypes:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSString projectType, string localizedTitle, [NullAllowed] string localizedDescription, [NullAllowed] UIImage image, bool canProvideSubtypes);

		[Mac (10, 14)]
		[Export ("canProvideSubtypes")]
		bool CanProvideSubtypes { get; }

		[Mac (10, 14)]
		[NullAllowed, Export ("localizedAttributedDescription", ArgumentSemantic.Copy)]
		NSAttributedString LocalizedAttributedDescription { get; }
	}

	[Mac (10,13)]
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

		[Mac (10, 14)]
		[Export ("quality")]
		double Quality { get; }
	}

	[Mac (10,13)]
	[NoiOS][NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PHProjectElement : NSSecureCoding {

		[Export ("weight")]
		double Weight { get; }

		[Export ("placement")]
		CGRect Placement { get; }
	}

	[Mac (10,13)]
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

		[Mac (10, 14)]
		[Export ("horizontallyFlipped")]
		bool HorizontallyFlipped { get; }

		[Mac (10, 14)]
		[Export ("verticallyFlipped")]
		bool VerticallyFlipped { get; }
	}

	[Mac (10,13)]
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

		[Mac (10, 14)]
		[Export ("brandingEnabled")]
		bool BrandingEnabled { get; }

		[Mac (10, 14)]
		[Export ("pageNumbersEnabled")]
		bool PageNumbersEnabled { get; }

		[Mac (10, 14)]
		[NullAllowed, Export ("productIdentifier")]
		string ProductIdentifier { get; }

		[Mac (10, 14)]
		[NullAllowed, Export ("themeIdentifier")]
		string ThemeIdentifier { get; }
	}

	[Mac (10,13)]
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

	[Mac (10,13)]
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

		[Mac (10, 14)]
		[NullAllowed, Export ("backgroundColor")]
		UIColor BackgroundColor { get; }
	}

	[Mac (10,14)]
	[NoiOS][NoTV]
	[DisableDefaultCtor]
	[BaseType (typeof(PHProjectElement))]
	interface PHProjectMapElement : NSSecureCoding
	{
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
		IMKAnnotation[] Annotations { get; }
	}

	interface IPHProjectTypeDescriptionDataSource {}
	[Mac (10,14)]
	[NoiOS][NoTV]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface PHProjectTypeDescriptionDataSource
	{
		[Abstract]
		[Export ("subtypesForProjectType:")]
		PHProjectTypeDescription[] GetSubtypes (NSString projectType);

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

	interface IPHProjectTypeDescriptionInvalidator {}
	[Mac (10,14)]
	[NoiOS][NoTV]
	[Protocol]
	interface PHProjectTypeDescriptionInvalidator
	{
		[Abstract]
		[Export ("invalidateTypeDescriptionForProjectType:")]
		void InvalidateTypeDescription (NSString projectType);

		[Abstract]
		[Export ("invalidateFooterTextForSubtypesOfProjectType:")]
		void InvalidateFooterTextForSubtypes (NSString projectType);
	}

	[iOS (8,0)]
	[NoMac][NoTV]
	[DisableDefaultCtor]
	[NoMacCatalyst]
	[Deprecated (PlatformName.iOS, 13, 0)]
	[BaseType (typeof (NSExtensionContext))]
	interface PHEditingExtensionContext
	{
	}

	interface IPHPickerViewControllerDelegate {}

	[NoWatch, NoTV, NoMac, iOS (14,0)]
	[MacCatalyst (14,0)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof(NSObject))]
	interface PHPickerViewControllerDelegate
	{
		[Abstract]
		[Export ("picker:didFinishPicking:")]
		void DidFinishPicking (PHPickerViewController picker, PHPickerResult[] results);
	}

	[NoWatch, NoTV, NoMac, iOS (14,0)]
	[MacCatalyst (14,0)]
	[BaseType (typeof (UIViewController))]
	[Advice ("This type should not be subclassed.")]
	[DisableDefaultCtor]
	interface PHPickerViewController
	{
		[Export ("configuration", ArgumentSemantic.Copy)]
		PHPickerConfiguration Configuration { get; }

		[Wrap ("WeakDelegate")]
		IPHPickerViewControllerDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Export ("initWithConfiguration:")]
		[DesignatedInitializer]
		NativeHandle Constructor (PHPickerConfiguration configuration);
	}

	[NoWatch, NoTV, NoMac, iOS (14,0)]
	[MacCatalyst (14,0)]
	[BaseType (typeof (NSObject))]
	[Advice ("This type should not be subclassed.")]
	interface PHPickerConfiguration : NSCopying
	{
		[Export ("preferredAssetRepresentationMode", ArgumentSemantic.Assign)]
		PHPickerConfigurationAssetRepresentationMode PreferredAssetRepresentationMode { get; set; }

		[iOS (15,0), MacCatalyst (15,0)]
		[Export ("selection", ArgumentSemantic.Assign)]
		PHPickerConfigurationSelection Selection { get; set; }

		[Export ("selectionLimit")]
		nint SelectionLimit { get; set; }

		[NullAllowed, Export ("filter", ArgumentSemantic.Copy)]
		PHPickerFilter Filter { get; set; }

		[Export ("initWithPhotoLibrary:")]
		NativeHandle Constructor (PHPhotoLibrary photoLibrary);

		[iOS (15,0), MacCatalyst (15,0)]
		[Export ("preselectedAssetIdentifiers", ArgumentSemantic.Copy)]
		string[] PreselectedAssetIdentifiers { get; set; }
	}

	[NoWatch, NoTV, NoMac, iOS (14,0)]
	[MacCatalyst (14,0)]
	[BaseType (typeof (NSObject))]
	[Advice ("This type should not be subclassed.")]
	[DisableDefaultCtor]
	interface PHPickerFilter : NSCopying
	{
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
		PHPickerFilter GetAnyFilterMatchingSubfilters (PHPickerFilter[] subfilters);
	}

	[NoWatch, NoTV, NoMac, iOS (14,0)]
	[MacCatalyst (14,0)]
	[BaseType (typeof (NSObject))]
	[Advice ("This type should not be subclassed.")]
	[DisableDefaultCtor]
	interface PHPickerResult
	{
		[Export ("itemProvider")]
		NSItemProvider ItemProvider { get; }

		[NullAllowed, Export ("assetIdentifier")]
		string AssetIdentifier { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (14,0)]
	[Category]
	[BaseType (typeof (PHPhotoLibrary))]
	interface PHPhotoLibrary_PhotosUISupport
	{
		[Export ("presentLimitedLibraryPickerFromViewController:")]
		void PresentLimitedLibraryPicker (UIViewController controller);

		[Async]
		[iOS (15,0), MacCatalyst (15,0)]
		[Export ("presentLimitedLibraryPickerFromViewController:completionHandler:")]
		void PresentLimitedLibraryPicker (UIViewController controller, Action<string[]> completionHandler);
	}

	[NoWatch, NoTV, NoMac, iOS (15,0), MacCatalyst (15,0)]
	[Native]
	public enum PHPickerConfigurationSelection : long
	{
		Default = 0,
		Ordered = 1,
	}
}
