// Copyright 2015 Xamarin Inc.
// Copyright 2018 Microsoft Corporation

using System;
using System.ComponentModel;
using AVFoundation;
using Foundation;
using JavaScriptCore;
using ObjCRuntime;
using UIKit;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace TVMLKit {

	[Native]
	public enum TVColorType : long {
		None,
		Plain,
		LinearGradientTopToBottom,
		LinearGradientLeftToRight
	}

	[Native]
	public enum TVElementAlignment : long {
		Undefined,
		Left,
		Center,
		Right,
		Leading,
		Trailing,
	}

	[Native]
	public enum TVElementContentAlignment : long {
		Undefined,
		Top,
		Center,
		Bottom
	}

	[Native]
	public enum TVElementPosition : long {
		Undefined,
		Center,
		Top,
		Bottom,
		Left,
		Right,
		TopLeft,
		TopRight,
		BottomLeft,
		BottomRight,
		Header,
		Footer,
		Leading,
		Trailing,
		TopLeading,
		TopTrailing,
		BottomLeading,
		BottomTrailing,
	}

	[Native]
	public enum TVElementEventType : long {
		Play = 1,
		Select,
		HoldSelect,
		Highlight,
		Change
	}

	[Native]
	public enum TVElementUpdateType : long {
		None,
		Subtree,
#if NET
		Styles,
		Children,
		Self,
#else
		Children,
		Self,
		Styles,
#endif
	}

	[Native]
	public enum TVElementResettableProperty : long {
		UpdateType,
		AutoHighlightIdentifier
	}

	[Native]
	public enum TVImageType : long {
		Image,
		Fullscreen,
		Decoration,
		Hero
	}

	[Native]
	[ErrorDomain ("TVMLKitErrorDomain")]
	public enum TVMLKitError : long {
		Unknown = 1,
		InternetUnavailable,
		FailedToLaunch,
		Last
	}

	[Native]
	public enum TVViewElementStyleType : long {
		Integer = 1,
		Double,
		Point,
		String,
		Color,
		Url,
		Transform,
		EdgeInsets
	}

	[Native]
	public enum TVTextElementStyle : long {
		None,
		Title,
		Subtitle,
		Description,
		Decoration
	}

	[Native]
	public enum TVPlaybackState : long {
		Undefined,
		Begin,
		Loading,
		Playing,
		Paused,
		Scanning,
		FastForwarding,
		Rewinding,
		End,
	}

	[Native]
	public enum TVPlaylistRepeatMode : long {
		None = 0,
		All,
		One,
	}

	[Native]
	public enum TVPlaylistEndAction : long {
		Stop = 0,
		Pause,
		WaitForMoreItems,
	}

	public enum TVMediaItemType {
		// NS_TYPED_EXTENSIBLE_ENUM
		[DefaultEnumValue]
		UnknownCustomExtension = -1,
		None,
		[Field ("TVMediaItemTypeAudio")]
		Audio,
		[Field ("TVMediaItemTypeVideo")]
		Video,
	}

	public enum TVMediaItemContentRatingDomain {
		// NS_TYPED_EXTENSIBLE_ENUM
		[DefaultEnumValue]
		UnknownCustomExtension = -1,
		[Field (null)] // property is nullable
		None,
		[Field ("TVMediaItemContentRatingDomainMovie")]
		Movie,
		[Field ("TVMediaItemContentRatingDomainTVShow")]
		TVShow,
		[Field ("TVMediaItemContentRatingDomainMusic")]
		Music,
	}

	[BaseType (typeof (NSObject))]
	interface TVApplicationControllerContext : NSCopying {
		[Export ("javaScriptApplicationURL", ArgumentSemantic.Copy)]
		NSUrl JavaScriptApplicationUrl { get; set; }

		[NullAllowed, Export ("storageIdentifier")]
		string StorageIdentifier { get; set; }

		[Export ("launchOptions", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> LaunchOptions { get; set; }

		[TV (14, 0)]
		[Export ("supportsPictureInPicturePlayback")]
		bool SupportsPictureInPicturePlayback { get; set; }
	}

	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface TVApplicationControllerDelegate {
		[Export ("appController:evaluateAppJavaScriptInContext:")]
		void EvaluateAppJavaScript (TVApplicationController appController, JSContext jsContext);

		[Export ("appController:didFinishLaunchingWithOptions:")]
		void DidFinishLaunching (TVApplicationController appController, [NullAllowed] NSDictionary<NSString, NSObject> options);

		[Export ("appController:didFailWithError:")]
		void DidFail (TVApplicationController appController, NSError error);

		[Export ("appController:didStopWithOptions:")]
		void DidStop (TVApplicationController appController, [NullAllowed] NSDictionary<NSString, NSObject> options);

		[Export ("playerForAppController:")]
		[return: NullAllowed]
		TVPlayer GetPlayer (TVApplicationController appController);
	}

	interface ITVApplicationControllerDelegate { }

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface TVApplicationController {
		[Export ("initWithContext:window:delegate:")]
		[DesignatedInitializer]
		NativeHandle Constructor (TVApplicationControllerContext context, [NullAllowed] UIWindow window, [NullAllowed] ITVApplicationControllerDelegate @delegate);

		[NullAllowed, Export ("window")]
		UIWindow Window { get; }

		[Export ("context")]
		TVApplicationControllerContext Context { get; }

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		ITVApplicationControllerDelegate Delegate { get; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; }

		[Export ("navigationController")]
		UINavigationController NavigationController { get; }

		[Export ("evaluateInJavaScriptContext:completion:")]
		[Async]
		void Evaluate (Action<JSContext> evaluation, [NullAllowed] Action<bool> completion);

		[Export ("stop")]
		void Stop ();
	}

	[BaseType (typeof (NSObject))]
	interface TVColor : NSCopying {
		[Export ("colorType")]
		TVColorType ColorType { get; }

		[NullAllowed, Export ("color")]
		UIColor Color { get; }

		[NullAllowed, Export ("gradientColors")]
		UIColor [] GradientColors { get; }

		[NullAllowed, Export ("gradientPoints")]
		NSNumber [] GradientPoints { get; }
	}

#if false // TVMLKit/TVElementTypes.h was removed from Xcode 7.1 beta 3 (mistake?)
	[Static]
	interface TVAttributeKey {
		// FIXME: does it fit here ?
		[Field ("TVAttributeCanonicalIdentifier")]
		NSString CanonicalIdentifier { get; }

		[Field ("TVAttributeKeyAccessibility")]
		NSString Accessibility { get; }

		[Field ("TVAttributeKeyAllowsZooming")]
		NSString AllowsZooming { get; }

		[Field ("TVAttributeKeyAspectFill")]
		NSString AspectFill { get; }

		[Field ("TVAttributeKeyAutoHighlight")]
		NSString AutoHighlight { get; }

		[Field ("TVAttributeKeyCentered")]
		NSString Centered { get; }

		[Field ("TVAttributeKeyDataMetrics")]
		NSString DataMetrics { get; }

		[Field ("TVAttributeKeyDisabled")]
		NSString Disabled { get; }

		[Field ("TVAttributeKeyFirstItem")]
		NSString FirstItem { get; }

		[Field ("TVAttributeKeyFirstName")]
		NSString FirstName { get; }

		[Field ("TVAttributeKeyFocusEnabled")]
		NSString FocusEnabled { get; }

		[Field ("TVAttributeKeyImg_Height")]
		NSString Img_Height { get; }

		[Field ("TVAttributeKeyImg_Src")]
		NSString Img_Src { get; }

		[Field ("TVAttributeKeyImg_Srcset")]
		NSString Img_Srcset { get; }

		[Field ("TVAttributeKeyImg_Width")]
		NSString Img_Width { get; }

		[Field ("TVAttributeKeyImpressionViewableThreshold")]
		NSString ImpressionViewableThreshold { get; }

		[Field ("TVAttributeKeyIndex")]
		NSString Index { get; }

		[Field ("TVAttributeKeyLastName")]
		NSString LastName { get; }

		[Field ("TVAttributeKeyMinLength")]
		NSString MinLength { get; }

		[Field ("TVAttributeKeyMode")]
		NSString Mode { get; }

		[Field ("TVAttributeKeyShowsScrollIndicator")]
		NSString ShowsScrollIndicator { get; }

		[Field ("TVAttributeKeyTextInput_KeyboardType")]
		NSString TextInput_KeyboardType { get; }

		[Field ("TVAttributeKeyTextInput_Secure")]
		NSString TextInput_Secure { get; }

		[Field ("TVAttributeKeyTheme")]
		NSString Theme { get; }

		[Field ("TVAttributeKeyType")]
		NSString Type { get; }

		[Field ("TVAttributeKeyValue")]
		NSString Value { get; }

		[Field ("TVAttributeMappedIndexPaths")]
		NSString TVAttributeMappedIndexPaths { get; }

		[Field ("TVAttributeNumStars")]
		NSString TVAttributeNumStars { get; }

		[Field ("TVAttributePageControl")]
		NSString TVAttributePageControl { get; }

		[Field ("TVAttributeSearchIdentifier")]
		NSString TVAttributeSearchIdentifier { get; }

		[Field ("TVAttributeSiriData")]
		NSString TVAttributeSiriData { get; }
	}

	[Static]
	interface TVElementKey {
		[Field ("TVElementKeyActivityIndicator")]
		NSString ActivityIndicator { get; }

		[Field ("TVElementKeyAsset")]
		NSString Asset { get; }

		[Field ("TVElementKeyAudio")]
		NSString Audio { get; }

		[Field ("TVElementKeyBackground")]
		NSString Background { get; }

		[Field ("TVElementKeyAlertTemplate")]
		NSString AlertTemplate { get; }

		[Field ("TVElementKeyAppInfoCard")]
		NSString AppInfoCard { get; }

		[Field ("TVElementKeyAppProductTemplate")]
		NSString AppProductTemplate { get; }

		[Field ("TVElementKeyBadge")]
		NSString Badge { get; }

		[Field ("TVElementKeyButton")]
		NSString Button { get; }

		[Field ("TVElementKeyButtonLockup")]
		NSString ButtonLockup { get; }

		[Field ("TVElementKeyCard")]
		NSString Card { get; }

		[Field ("TVElementKeyCarousel")]
		NSString Carousel { get; }

		[Field ("TVElementKeyCatalogTemplate")]
		NSString CatalogTemplate { get; }

		[Field ("TVElementKeyCollectionList")]
		NSString CollectionList { get; }

		[Field ("TVElementKeyColumnList")]
		NSString ColumnList { get; }

		[Field ("TVElementKeyCommonSenseTemplate")]
		NSString CommonSenseTemplate { get; }

		[Field ("TVElementKeyCompilationTemplate")]
		NSString CompilationTemplate { get; }

		[Field ("TVElementKeyCuratedPlaylistImg")]
		NSString CuratedPlaylistImg { get; }

		[Field ("TVElementKeyDecorationImage")]
		NSString DecorationImage { get; }

		[Field ("TVElementKeyDecorationLabel")]
		NSString DecorationLabel { get; }

		[Field ("TVElementKeyDescription")]
		NSString Description { get; }

		[Field ("TVElementKeyDescriptiveAlertTemplate")]
		NSString DescriptiveAlertTemplate { get; }

		[Field ("TVElementKeyDialogTemplate")]
		NSString DialogTemplate { get; }

		[Field ("TVElementKeyDivTemplate")]
		NSString DivTemplate { get; }

		[Field ("TVElementKeyDocumentBanner")]
		NSString DocumentBanner { get; }

		[Field ("TVElementKeyFooter")]
		NSString Footer { get; }

		[Field ("TVElementKeyFormTemplate")]
		NSString FormTemplate { get; }

		[Field ("TVElementKeyFullscreenImg")]
		NSString FullscreenImg { get; }

		[Field ("TVElementKeyGalleryTemplate")]
		NSString GalleryTemplate { get; }

		[Field ("TVElementKeyGrid")]
		NSString Grid { get; }

		[Field ("TVElementKeyHead")]
		NSString Head { get; }

		[Field ("TVElementKeyHeader")]
		NSString Header { get; }

		[Field ("TVElementKeyHeroImage")]
		NSString HeroImage { get; }

		[Field ("TVElementKeyIdentityBanner")]
		NSString IdentityBanner { get; }

		[Field ("TVElementKeyImageDeck")]
		NSString ImageDeck { get; }

		[Field ("TVElementKeyImg")]
		NSString Img { get; }

		[Field ("TVElementKeyInfo")]
		NSString Info { get; }

		[Field ("TVElementKeyInfoList")]
		NSString InfoList { get; }

		[Field ("TVElementKeyInfoTable")]
		NSString InfoTable { get; }

		[Field ("TVElementKeyItemBanner")]
		NSString ItemBanner { get; }

		[Field ("TVElementKeyList")]
		NSString List { get; }

		[Field ("TVElementKeyListAltTemplate")]
		NSString ListAltTemplate { get; }

		[Field ("TVElementKeyListItemLockup")]
		NSString ListItemLockup { get; }

		[Field ("TVElementKeyListTemplate")]
		NSString ListTemplate { get; }

		[Field ("TVElementKeyLoadingTemplate")]
		NSString LoadingTemplate { get; }

		[Field ("TVElementKeyLockup")]
		NSString Lockup { get; }

		[Field ("TVElementKeyMainTemplate")]
		NSString MainTemplate { get; }

		[Field ("TVElementKeyMenuBar")]
		NSString MenuBar { get; }

		[Field ("TVElementKeyMenuBarTemplate")]
		NSString MenuBarTemplate { get; }

		[Field ("TVElementKeyMenuItem")]
		NSString MenuItem { get; }

		[Field ("TVElementKeyMonogram")]
		NSString Monogram { get; }

		[Field ("TVElementKeyMonogramLockup")]
		NSString MonogramLockup { get; }

		[Field ("TVElementKeyMusicVideoProductTemplate")]
		NSString MusicVideoProductTemplate { get; }

		[Field ("TVElementKeyOneupTemplate")]
		NSString OneupTemplate { get; }

		[Field ("TVElementKeyOrdinal")]
		NSString Ordinal { get; }

		[Field ("TVElementKeyOverlay")]
		NSString Overlay { get; }

		[Field ("TVElementKeyParadeTemplate")]
		NSString ParadeTemplate { get; }

		[Field ("TVElementKeyPhotoAlbumTemplate")]
		NSString PhotoAlbumTemplate { get; }

		[Field ("TVElementKeyPhotoLibraryTemplate")]
		NSString PhotoLibraryTemplate { get; }

		[Field ("TVElementKeyPlaceholder")]
		NSString Placeholder { get; }

		[Field ("TVElementKeyPlaylistFolderImg")]
		NSString PlaylistFolderImg { get; }

		[Field ("TVElementKeyPlaylistImg")]
		NSString PlaylistImg { get; }

		[Field ("TVElementKeyProductBundleTemplate")]
		NSString ProductBundleTemplate { get; }

		[Field ("TVElementKeyProductInfo")]
		NSString ProductInfo { get; }

		[Field ("TVElementKeyProductTemplate")]
		NSString ProductTemplate { get; }

		[Field ("TVElementKeyProgressBar")]
		NSString ProgressBar { get; }

		[Field ("TVElementKeyRatingBadge")]
		NSString RatingBadge { get; }

		[Field ("TVElementKeyRatingButton")]
		NSString RatingButton { get; }

		[Field ("TVElementKeyRatingCard")]
		NSString RatingCard { get; }

		[Field ("TVElementKeyRatingTemplate")]
		NSString RatingTemplate { get; }

		[Field ("TVElementKeyRelatedContent")]
		NSString TVElementKeyRelatedContent { get; }

		[Field ("TVElementKeyReviewCard")]
		NSString ReviewCard { get; }

		[Field ("TVElementKeyRow")]
		NSString Row { get; }

		[Field ("TVElementKeySearchField")]
		NSString SearchField { get; }

		[Field ("TVElementKeySearchTemplate")]
		NSString SearchTemplate { get; }

		[Field ("TVElementKeySeasonBadge")]
		NSString SeasonBadge { get; }

		[Field ("TVElementKeySection")]
		NSString Section { get; }

		[Field ("TVElementKeySegmentBar")]
		NSString SegmentBar { get; }

		[Field ("TVElementKeySegmentBarHeader")]
		NSString SegmentBarHeader { get; }

		[Field ("TVElementKeySegmentBarItem")]
		NSString SegmentBarItem { get; }

		[Field ("TVElementKeySeparator")]
		NSString Separator { get; }

		[Field ("TVElementKeyShelf")]
		NSString Shelf { get; }

		[Field ("TVElementKeyShowcaseTemplate")]
		NSString ShowcaseTemplate { get; }

		[Field ("TVElementKeySlideshow")]
		NSString Slideshow { get; }

		[Field ("TVElementKeySpriteLockup")]
		NSString SpriteLockup { get; }

		[Field ("TVElementKeyStack")]
		NSString Stack { get; }

		[Field ("TVElementKeyStackTemplate")]
		NSString StackTemplate { get; }

		[Field ("TVElementKeyStarBar")]
		NSString StarBar { get; }

		[Field ("TVElementKeyStarHistogram")]
		NSString StarHistogram { get; }

		[Field ("TVElementKeySubtitle")]
		NSString Subtitle { get; }

		[Field ("TVElementKeyText")]
		NSString Text { get; }

		[Field ("TVElementKeyTextBadge")]
		NSString TextBadge { get; }

		[Field ("TVElementKeyTextField")]
		NSString TextField { get; }

		[Field ("TVElementKeyTitle")]
		NSString Title { get; }

		[Field ("TVElementKeyTumblerBar")]
		NSString TumblerBar { get; }

		[Field ("TVElementKeyTumblerItem")]
		NSString TumblerItem { get; }

		[Field ("TVElementKeyVideo")]
		NSString Video { get; }
	}

	// FIXME: enum'ify ?
	[Static]
	interface TVKeyboardType {
		[Field ("TVKeyboardTypeEmailAddress")]
		NSString EmailAddress { get; }

		[Field ("TVKeyboardTypeNumberPad")]
		NSString NumberPad { get; }

		[Field ("TVKeyboardTypeURL")]
		NSString TypeURL { get; }
	}
#endif

	[BaseType (typeof (NSObject))]
	interface TVElementFactory {
		// FIXME: provide System.Type overload
		[Static]
		[Export ("registerViewElementClass:forElementName:")]
		void RegisterViewElementClass (Class elementClass, string elementName);
	}

	[BaseType (typeof (NSObject))]
	interface TVViewElementStyle : NSCopying {
		// FIXME: badly named, unsure of return value
		[Export ("valueForStyleProperty:")]
		[return: NullAllowed]
		NSObject ValueForStyleProperty (string name);

		[NullAllowed, Export ("backgroundColor")]
		TVColor BackgroundColor { get; }

		[NullAllowed, Export ("color")]
		TVColor Color { get; }

		[Export ("fontSize")]
		nfloat FontSize { get; }

		// FIXME: that's likely an (existing?) set of constants
		[NullAllowed, Export ("fontWeight")]
		NSString FontWeight { get; }

		[Export ("height")]
		nfloat Height { get; }

		[Export ("margin")]
		UIEdgeInsets Margin { get; }

		[Export ("focusMargin")]
		UIEdgeInsets FocusMargin { get; }

		[Export ("maxHeight")]
		nfloat MaxHeight { get; }

		[Export ("maxWidth")]
		nfloat MaxWidth { get; }

		[Export ("minHeight")]
		nfloat MinHeight { get; }

		[Export ("minWidth")]
		nfloat MinWidth { get; }

		[Export ("padding")]
		UIEdgeInsets Padding { get; }

		[Export ("textAlignment")]
		UITextAlignment TextAlignment { get; }

		[Export ("width")]
		nfloat Width { get; }

		[Export ("alignment")]
		TVElementAlignment Alignment { get; }

		[Export ("contentAlignment")]
		TVElementContentAlignment ContentAlignment { get; }

		[NullAllowed, Export ("highlightColor")]
		TVColor HighlightColor { get; }

		[NullAllowed, Export ("imageTreatmentName")]
		string ImageTreatmentName { get; }

		[Export ("interitemSpacing")]
		nfloat InteritemSpacing { get; }

		[NullAllowed, Export ("textHighlightStyle")]
		string TextHighlightStyle { get; }

		[Export ("textMinimumScaleFactor")]
		nfloat TextMinimumScaleFactor { get; }

		[Export ("position")]
		TVElementPosition Position { get; }

		[NullAllowed, Export ("ratingStyle")]
		string RatingStyle { get; }

		[Export ("maxTextLines")]
		nuint MaxTextLines { get; }

		[NullAllowed, Export ("textStyle")]
		string TextStyle { get; }

		[NullAllowed, Export ("tintColor")]
		TVColor TintColor { get; }
	}

	[BaseType (typeof (NSObject))]
	interface TVViewElement : NSCopying {
		[Export ("elementIdentifier")]
		string ElementIdentifier { get; }

		[Export ("elementName")]
		string ElementName { get; }

		[NullAllowed, Export ("parentViewElement", ArgumentSemantic.Weak)]
		TVViewElement ParentViewElement { get; }

		[NullAllowed, Export ("childViewElements")]
		TVViewElement [] ChildViewElements { get; }

		[NullAllowed, Export ("attributes")]
		NSDictionary<NSString, NSString> Attributes { get; }

		[NullAllowed, Export ("style")]
		TVViewElementStyle Style { get; }

		[NullAllowed, Export ("autoHighlightIdentifier")]
		string AutoHighlightIdentifier { get; }

		[Export ("disabled")]
		bool Disabled {
			[Bind ("isDisabled")]
			get;
			set;
		}

		[Internal]
		[Sealed]
		[Export ("updateType")]
		TVElementUpdateType _UpdateType { get; }

		[Export ("resetProperty:")]
		void Reset (TVElementResettableProperty resettableProperty);

		[Export ("dispatchEventOfType:canBubble:cancellable:extraInfo:completion:")]
		[Async (ResultType = typeof (TVViewElementDispatchResult))]
		void DispatchEvent (TVElementEventType type, bool canBubble, bool isCancellable, [NullAllowed] NSDictionary<NSString, NSObject> extraInfo, [NullAllowed] Action<bool, bool> completion);

		[Export ("dispatchEventWithName:canBubble:cancellable:extraInfo:completion:")]
		[Async (ResultType = typeof (TVViewElementDispatchResult))]
		void DispatchEvent (string eventName, bool canBubble, bool isCancellable, [NullAllowed] NSDictionary<NSString, NSObject> extraInfo, [NullAllowed] Action<bool, bool> completion);

		[TV (13, 0)]
		[Export ("elementData")]
		NSDictionary<NSString, NSObject> ElementData { get; }
	}

	[BaseType (typeof (TVViewElement))]
	interface TVImageElement {
		[NullAllowed, Export ("URL")]
		NSUrl Url { get; }

		[NullAllowed, Export ("srcset")]
		NSDictionary<NSString, NSUrl> SourceSet { get; }

		[Export ("imageType")]
		TVImageType ImageType { get; }
	}

	[Protocol]
	interface TVInterfaceCreating {
		[Export ("viewForElement:existingView:")]
		[return: NullAllowed]
		UIView GetViewForElement (TVViewElement element, [NullAllowed] UIView existingView);

		[Export ("viewControllerForElement:existingViewController:")]
		[return: NullAllowed]
		UIViewController GetViewControllerForElement (TVViewElement element, [NullAllowed] UIViewController existingViewController);

		[Export ("URLForResource:")]
		[return: NullAllowed]
		NSUrl GetUrlForResource (string resourceName);

		[Export ("imageForResource:")]
		[return: NullAllowed]
		UIImage GetImageForResource (string resourceName);

		[Export ("collectionViewCellClassForElement:")]
		[return: NullAllowed]
		Class GetCollectionViewCellClass (TVViewElement element);

		[Export ("playerViewControllerForPlayer:")]
		[return: NullAllowed]
		UIViewController GetPlayerViewController (TVPlayer player);
	}

	interface ITVInterfaceCreating { }

	[BaseType (typeof (NSObject))]
	interface TVInterfaceFactory : TVInterfaceCreating {
		[Static]
		[Export ("sharedInterfaceFactory")]
		TVInterfaceFactory SharedInterfaceFactory { get; }

		[NullAllowed, Export ("extendedInterfaceCreator", ArgumentSemantic.Strong)]
		ITVInterfaceCreating ExtendedInterfaceCreator { get; set; }
	}

	[BaseType (typeof (NSObject))]
	interface TVStyleFactory {
		[Static]
		[Export ("registerStyle:withType:inherited:")]
		void RegisterStyle (string styleName, TVViewElementStyleType type, bool inherited);
	}

	[BaseType (typeof (TVViewElement))]
	interface TVTextElement {
		[NullAllowed, Export ("attributedText")]
		NSAttributedString AttributedText { get; }

		[Export ("textStyle")]
		TVTextElementStyle TextStyle { get; }

		[Export ("attributedStringWithFont:")]
		NSAttributedString GetAttributedString (UIFont font);

		[Export ("attributedStringWithFont:foregroundColor:textAlignment:")]
		NSAttributedString GetAttributedString (UIFont font, [NullAllowed] UIColor foregroundColor, UITextAlignment alignment);
	}

	interface ITVPlaybackEventMarshaling { }

	[Protocol]
	interface TVPlaybackEventMarshaling {
		[Abstract]
		[NullAllowed, Export ("properties", ArgumentSemantic.Strong)]
		NSDictionary<NSString, NSObject> Properties { get; }

		[Export ("processReturnJSValue:inContext:")]
		void ProcessReturn (JSValue value, JSContext jsContext);
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface TVPlaybackCustomEventUserInfo : TVPlaybackEventMarshaling {
		[Export ("initWithProperties:expectsReturnValue:")]
		NativeHandle Constructor ([NullAllowed] NSDictionary<NSString, NSObject> properties, bool expectsReturnValue);

		[Export ("expectsReturnValue")]
		bool ExpectsReturnValue { get; set; }

		[NullAllowed, Export ("returnValue", ArgumentSemantic.Strong)]
		NSObject ReturnValue { get; }
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface TVPlaylist {
		[Export ("mediaItems", ArgumentSemantic.Copy)]
		TVMediaItem [] MediaItems { get; }

		[Export ("endAction", ArgumentSemantic.Assign)]
		TVPlaylistEndAction EndAction { get; }

		[Export ("repeatMode", ArgumentSemantic.Assign)]
		TVPlaylistRepeatMode RepeatMode { get; }

		[NullAllowed, Export ("userInfo", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> UserInfo { get; }
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface TVMediaItem {
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[NullAllowed, Export ("type", ArgumentSemantic.Strong)]
		NSString /* NS_TYPED_EXTENSIBLE_ENUM */ WeakType { get; }

		[Wrap ("TVMediaItemTypeExtensions.GetValue (WeakType!)")]
		TVMediaItemType Type { get; }

		[NullAllowed, Export ("url", ArgumentSemantic.Strong)]
		NSUrl Url { get; }

		[NullAllowed, Export ("title", ArgumentSemantic.Strong)]
		string Title { get; }

		[NullAllowed, Export ("subtitle", ArgumentSemantic.Strong)]
		string Subtitle { get; }

		[NullAllowed, Export ("itemDescription", ArgumentSemantic.Strong)]
		string ItemDescription { get; }

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[NullAllowed, Export ("contentRatingDomain", ArgumentSemantic.Strong)]
		NSString /* NS_TYPED_EXTENSIBLE_ENUM */ WeakContentRatingDomain { get; }

		[Wrap ("TVMediaItemContentRatingDomainExtensions.GetValue (WeakContentRatingDomain)")]
		TVMediaItemContentRatingDomain ContentRatingDomain { get; }

		[NullAllowed, Export ("contentRatingRanking", ArgumentSemantic.Strong)]
		NSNumber ContentRatingRanking { get; }

		[NullAllowed, Export ("artworkImageURL", ArgumentSemantic.Strong)]
		NSUrl ArtworkImageUrl { get; }

		[Export ("containsExplicitContent")]
		bool ExplicitContent { get; }

		[Export ("resumeTime")]
		/* NSInterval */
		double ResumeTime { get; }

		[Export ("interstitials", ArgumentSemantic.Strong)]
		TVTimeRange [] Interstitials { get; }

		[Export ("highlightGroups", ArgumentSemantic.Strong)]
		TVHighlightGroup [] HighlightGroups { get; }

		[Export ("userInfo", ArgumentSemantic.Strong)]
		NSDictionary<NSString, NSObject> UserInfo { get; }
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface TVTimeRange {
		[Export ("startTime")]
		/* NSInterval */
		double StartTime { get; }

		[Export ("endTime")]
		/* NSInterval */
		double EndTime { get; }

		[Export ("duration")]
		/* NSInterval */
		double Duration { get; }
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface TVHighlightGroup {
		[NullAllowed, Export ("localizedName", ArgumentSemantic.Strong)]
		string LocalizedName { get; }

		[Export ("highlights", ArgumentSemantic.Strong)]
		TVHighlight [] Highlights { get; }
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface TVHighlight {
		[NullAllowed, Export ("localizedName", ArgumentSemantic.Strong)]
		string LocalizedName { get; }

		[NullAllowed, Export ("highlightDescription", ArgumentSemantic.Strong)]
		string HighlightDescription { get; }

		[Export ("timeRange", ArgumentSemantic.Strong)]
		TVTimeRange TimeRange { get; }

		[NullAllowed, Export ("imageURL", ArgumentSemantic.Strong)]
		NSUrl ImageUrl { get; }
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface TVPlayer {
		[Export ("initWithPlayer:")]
		NativeHandle Constructor (AVPlayer player);

		[Export ("player", ArgumentSemantic.Strong)]
		AVPlayer Player { get; }

		[NullAllowed, Export ("playlist", ArgumentSemantic.Strong)]
		TVPlaylist Playlist { get; }

		[Export ("state", ArgumentSemantic.Assign)]
		TVPlaybackState State { get; }

		[NullAllowed, Export ("currentMediaItem", ArgumentSemantic.Strong)]
		TVMediaItem CurrentMediaItem { get; }

		[NullAllowed, Export ("nextMediaItem", ArgumentSemantic.Strong)]
		TVMediaItem NextMediaItem { get; }

		[NullAllowed, Export ("previousMediaItem", ArgumentSemantic.Strong)]
		TVMediaItem PreviousMediaItem { get; }

		[Async]
		[Export ("dispatchEvent:userInfo:completion:")]
		void DispatchEvent (string @event, [NullAllowed] ITVPlaybackEventMarshaling userInfo, [NullAllowed] Action<bool> completion);

		[Export ("pause")]
		void Pause ();

		[Export ("next")]
		void Next ();

		[Export ("previous")]
		void Previous ();

		[Export ("changeToMediaItemAtIndex:")]
		void ChangeToMediaItem (nint index);

		[TV (13, 0)]
		[Export ("presentWithAnimation:")]
		void Present (bool animated);
	}

	[TV (13, 0)]
	[Native]
	[ErrorDomain ("TVDocumentErrorDomain")]
	public enum TVDocumentError : long {
		Failed,
		Cancelled,
	}

	interface ITVBrowserViewControllerDataSource { }

	[TV (13, 0)]
#if NET
	[Protocol][Model]
#else
	[Protocol]
	[Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface TVBrowserViewControllerDataSource {
		[Abstract]
		[Export ("browserViewController:documentViewControllerForElement:")]
		[return: NullAllowed]
		TVDocumentViewController GetCorrespondingDocumentViewController (TVBrowserViewController browserViewController, TVViewElement viewElement);
	}

	interface ITVBrowserViewControllerDelegate { }

	[TV (13, 0)]
#if NET
	[Protocol][Model]
#else
	[Protocol]
	[Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface TVBrowserViewControllerDelegate {
		[Export ("browserViewController:willCenterOnViewElement:")]
		void WillCenterOnViewElement (TVBrowserViewController browserViewController, TVViewElement viewElement);

		[Export ("browserViewController:didCenterOnViewElement:")]
		void DidCenterOnViewElement (TVBrowserViewController browserViewController, TVViewElement viewElement);
	}

	[TV (13, 0)]
	[BaseType (typeof (UIViewController))]
	[DisableDefaultCtor]
	interface TVBrowserViewController {

		// inlined from base class
		[DesignatedInitializer]
		[Export ("initWithNibName:bundle:")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("viewElement", ArgumentSemantic.Strong)]
		TVViewElement ViewElement { get; }

		[Export ("cornerRadius")]
		nfloat CornerRadius { get; set; }

		[Export ("interitemSpacing")]
		nfloat InteritemSpacing { get; set; }

		[Export ("maskInset", ArgumentSemantic.Assign)]
		UIEdgeInsets MaskInset { get; set; }

		[Export ("centeredViewElement", ArgumentSemantic.Strong)]
		TVViewElement CenteredViewElement { get; set; }

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		ITVBrowserViewControllerDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDataSource")]
		[NullAllowed]
		ITVBrowserViewControllerDataSource DataSource { get; set; }

		[NullAllowed, Export ("dataSource", ArgumentSemantic.Weak)]
		NSObject WeakDataSource { get; set; }

		[Static]
		[Export ("viewControllerForElement:")]
		[return: NullAllowed]
		TVBrowserViewController GetCorrespondingViewController (TVViewElement viewElement);
	}

	interface ITVDocumentViewControllerDelegate { }

	[TV (13, 0)]
#if NET
	[Protocol][Model]
#else
	[Protocol]
	[Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface TVDocumentViewControllerDelegate {

		[Export ("documentViewControllerWillUpdate:")]
		void WillUpdate (TVDocumentViewController documentViewController);

		[Export ("documentViewControllerDidUpdate:")]
		void DidUpdate (TVDocumentViewController documentViewController);

		[Export ("documentViewController:didUpdateWithContext:")]
		void DidUpdate (TVDocumentViewController documentViewController, NSDictionary<NSString, NSObject> context);

		[Export ("documentViewController:didFailUpdateWithError:")]
		void DidFailUpdate (TVDocumentViewController documentViewController, NSError error);

		[Export ("documentViewController:handleEvent:withElement:")]
		bool HandleEvent (TVDocumentViewController documentViewController, NSString /* TVDocumentEvent */ @event, TVViewElement element);
	}

	[TV (13, 0)]
	enum TVDocumentEvent {
		[Field ("TVDocumentEventPlay")]
		Play,
		[Field ("TVDocumentEventSelect")]
		Select,
		[Field ("TVDocumentEventHoldSelect")]
		HoldSelect,
		[Field ("TVDocumentEventHighlight")]
		Highlight,
		[Field ("TVDocumentEventLoad")]
		Load,
		[Field ("TVDocumentEventUnload")]
		Unload,
		[Field ("TVDocumentEventAppear")]
		Appear,
		[Field ("TVDocumentEventDisappear")]
		Disappear,
	}

	[TV (13, 0)]
	[BaseType (typeof (UIViewController))]
	[DisableDefaultCtor]
	interface TVDocumentViewController {

		// initWithNibName:bundle: and initWithCoder: are both marked as unavailable (and not inlined)

		[Export ("documentContext", ArgumentSemantic.Strong)]
		NSDictionary<NSString, NSObject> DocumentContext { get; }

		[NullAllowed, Export ("appController", ArgumentSemantic.Weak)]
		TVApplicationController AppController { get; }

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		ITVDocumentViewControllerDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Static]
		[Export ("viewControllerWithContext:forAppController:")]
		TVDocumentViewController CreateViewController (NSDictionary<NSString, NSObject> context, TVApplicationController appController);

		[Export ("updateUsingContext:")]
		void Update (NSDictionary<NSString, NSObject> context);
	}

	[TV (13, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface TVBrowserTransitionAnimator : UIViewControllerAnimatedTransitioning {
	}
}
