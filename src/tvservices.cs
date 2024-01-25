// Copyright 2015 Xamarin Inc.
// Copyright 2019 Microsoft Corporation

using System;

using CoreGraphics;

using Foundation;

using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace TVServices {

	[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'TVTopShelfContentProvider' instead.")]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface TVContentIdentifier : NSCopying, NSSecureCoding {
		[Export ("identifier")]
		string Identifier { get; }

		[NullAllowed, Export ("container", ArgumentSemantic.Copy)]
		TVContentIdentifier Container { get; }

		[Export ("initWithIdentifier:container:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string identifier, [NullAllowed] TVContentIdentifier container);
	}

	[BaseType (typeof (NSObject))]
	[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'TVTopShelfItem' instead.")]
	[DisableDefaultCtor]
	interface TVContentItem : NSCopying, NSSecureCoding {
		[Export ("contentIdentifier", ArgumentSemantic.Copy)]
		TVContentIdentifier ContentIdentifier { get; }

		[NullAllowed, Export ("imageURL", ArgumentSemantic.Copy)]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'SetImageUrl' instead.")]
		NSUrl ImageUrl { get; set; }

		[Export ("imageShape", ArgumentSemantic.Assign)]
		TVContentItemImageShape ImageShape { get; set; }

		[NullAllowed, Export ("title")]
		string Title { get; set; }

		[NullAllowed, Export ("lastAccessedDate", ArgumentSemantic.Copy)]
		NSDate LastAccessedDate { get; set; }

		[NullAllowed, Export ("expirationDate", ArgumentSemantic.Copy)]
		NSDate ExpirationDate { get; set; }

		[NullAllowed, Export ("creationDate", ArgumentSemantic.Copy)]
		NSDate CreationDate { get; set; }

		[NullAllowed, Export ("badgeCount", ArgumentSemantic.Copy)]
		NSNumber BadgeCount { get; set; }

		[NullAllowed, Export ("duration", ArgumentSemantic.Copy)]
		NSNumber Duration { get; set; }

		[NullAllowed, Export ("currentPosition", ArgumentSemantic.Copy)]
		NSNumber CurrentPosition { get; set; }

		[NullAllowed, Export ("hasPlayedToEnd", ArgumentSemantic.Copy)]
		NSNumber HasPlayedToEnd { get; set; }

		[NullAllowed, Export ("playURL", ArgumentSemantic.Copy)]
		NSUrl PlayUrl { get; set; }

		[NullAllowed, Export ("displayURL", ArgumentSemantic.Copy)]
		NSUrl DisplayUrl { get; set; }

		[NullAllowed, Export ("topShelfItems", ArgumentSemantic.Copy)]
		TVContentItem [] TopShelfItems { get; set; }

		[Export ("initWithContentIdentifier:")]
		[DesignatedInitializer]
		NativeHandle Constructor (TVContentIdentifier ident);

		[Export ("imageURLForTraits:")]
		[return: NullAllowed]
		NSUrl GetImageUrl (TVContentItemImageTrait traits);

		[Export ("setImageURL:forTraits:")]
		void SetImageUrl ([NullAllowed] NSUrl aUrl, TVContentItemImageTrait traits);
	}

	[Protocol]
	interface TVTopShelfProvider {
		[Abstract]
		[Export ("topShelfStyle")]
		TVTopShelfContentStyle TopShelfStyle { get; }

		[Abstract]
		[Export ("topShelfItems")]
		TVContentItem [] TopShelfItems { get; }
	}

	[Static]
	interface TVTopShelfItems {
		[Notification]
		[Field ("TVTopShelfItemsDidChangeNotification")]
		NSString DidChangeNotification { get; }
	}

	[Deprecated (PlatformName.TvOS, 16, 0, message: "Use runs-as-current-user-with-user-independent-keychain entitlement instead.")]
	[TV (13, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // Name property can't be null
	interface TVAppProfileDescriptor : NSCopying, NSSecureCoding {

		[Export ("initWithName:")]
		NativeHandle Constructor (string name);

		[Export ("name")]
		string Name { get; set; }
	}

	[TV (13, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface TVTopShelfAction {
		[Export ("URL", ArgumentSemantic.Copy)]
		NSUrl Url { get; }

		[Export ("initWithURL:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSUrl url);
	}

	[TV (13, 0)]
	[Native]
	enum TVTopShelfCarouselContentStyle : long {
		Actions,
		Details,
	}

	interface ITVTopShelfContent { }

	[TV (13, 0)]
	[Protocol]
	interface TVTopShelfContent { }

	[TV (13, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface TVTopShelfCarouselContent : TVTopShelfContent {
		[Export ("style")]
		TVTopShelfCarouselContentStyle Style { get; }

		[Export ("items", ArgumentSemantic.Copy)]
		TVTopShelfCarouselItem [] Items { get; }

		[Export ("initWithStyle:items:")]
		[DesignatedInitializer]
		NativeHandle Constructor (TVTopShelfCarouselContentStyle style, TVTopShelfCarouselItem [] items);
	}

	[TV (13, 0)]
	[Flags]
	[Native]
	public enum TVTopShelfCarouselItemMediaOptions : ulong {
		VideoResolutionHD = 1uL << 0,
		VideoResolution4K = 2uL << 0,
		VideoColorSpaceHdr = 1uL << 6,
		VideoColorSpaceDolbyVision = 2uL << 6,
		AudioDolbyAtmos = 1uL << 12,
		AudioTranscriptionClosedCaptioning = 1uL << 13,
		AudioTranscriptionSdh = 1uL << 14,
		AudioDescription = 1uL << 15,
	}

	[TV (13, 0)]
	[BaseType (typeof (TVTopShelfItem))]
	[DisableDefaultCtor] // -[TVTopShelfCarouselItem init]: unrecognized selector sent to instance 0x600000eb18c0
	interface TVTopShelfCarouselItem {

		// inlined from base class
		[Export ("initWithIdentifier:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string identifier);

		[NullAllowed, Export ("contextTitle")]
		string ContextTitle { get; set; }

		[NullAllowed, Export ("summary")]
		string Summary { get; set; }

		[NullAllowed, Export ("genre")]
		string Genre { get; set; }

		[Export ("duration")]
		double /* NSTimeInterval */ Duration { get; set; }

		[NullAllowed, Export ("creationDate", ArgumentSemantic.Copy)]
		NSDate CreationDate { get; set; }

		[Export ("mediaOptions", ArgumentSemantic.Assign)]
		TVTopShelfCarouselItemMediaOptions MediaOptions { get; set; }

		[NullAllowed, Export ("previewVideoURL", ArgumentSemantic.Copy)]
		NSUrl PreviewVideoUrl { get; set; }

		[NullAllowed, Export ("cinemagraphURL", ArgumentSemantic.Copy)]
		NSUrl CinemagraphUrl { get; set; }

		[Export ("namedAttributes", ArgumentSemantic.Copy)]
		TVTopShelfNamedAttribute [] NamedAttributes { get; set; }
	}

	[TV (13, 0)]
	[BaseType (typeof (NSObject))]
	interface TVTopShelfContentProvider {
		[Async]
		[Export ("loadTopShelfContentWithCompletionHandler:")]
		void LoadTopShelfContent (Action<ITVTopShelfContent> completionHandler);

		[Static]
		[Export ("topShelfContentDidChange")]
		void DidChange ();
	}

	[TV (13, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface TVTopShelfInsetContent : TVTopShelfContent {
		[Export ("items", ArgumentSemantic.Copy)]
		TVTopShelfItem [] Items { get; }

		[Static]
		[Export ("imageSize")]
		CGSize ImageSize { get; }

		[Export ("initWithItems:")]
		NativeHandle Constructor (TVTopShelfItem [] items);
	}

	[TV (13, 0)]
	[BaseType (typeof (TVTopShelfObject))]
	[DisableDefaultCtor] // identifier can't be null and we have a designated initializer
	interface TVTopShelfItem {

		[Export ("identifier")]
		string Identifier { get; }

		[NullAllowed, Export ("playAction", ArgumentSemantic.Strong)]
		TVTopShelfAction PlayAction { get; set; }

		[NullAllowed, Export ("displayAction", ArgumentSemantic.Strong)]
		TVTopShelfAction DisplayAction { get; set; }

		[NullAllowed, Export ("expirationDate", ArgumentSemantic.Copy)]
		NSDate ExpirationDate { get; set; }

		[Export ("setImageURL:forTraits:")]
		void SetImageUrl ([NullAllowed] NSUrl imageUrl, TVTopShelfItemImageTraits traits);

		[Export ("imageURLForTraits:")]
		[return: NullAllowed]
		NSUrl GetImageUrl (TVTopShelfItemImageTraits traits);

		[Export ("initWithIdentifier:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string identifier);
	}

	[TV (13, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface TVTopShelfNamedAttribute {

		[Export ("name")]
		string Name { get; }

		[Export ("values", ArgumentSemantic.Copy)]
		string [] Values { get; }

		[Export ("initWithName:values:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string name, string [] values);
	}

	[TV (13, 0)]
	[BaseType (typeof (NSObject))]
	[Abstract]
	[DisableDefaultCtor]
	interface TVTopShelfObject {
		[NullAllowed, Export ("title")]
		string Title { get; set; }
	}

	[TV (13, 0)]
	[Flags]
	[Native]
	enum TVTopShelfItemImageTraits : ulong {
		Scale1x = 1,
		Scale2x = 2,
	}

	[TV (13, 0)]
	[BaseType (typeof (NSObject))]
	interface TVUserManager {

		[Deprecated (PlatformName.TvOS, 16, 0, message: "Use runs-as-current-user-with-user-independent-keychain entitlement instead.")]
		[NullAllowed, Export ("currentUserIdentifier")]
		string CurrentUserIdentifier { get; }

		[Deprecated (PlatformName.TvOS, 16, 0, message: "Use runs-as-current-user-with-user-independent-keychain entitlement instead.")]
		[Export ("userIdentifiersForCurrentProfile", ArgumentSemantic.Copy)]
		string [] UserIdentifiersForCurrentProfile { get; set; }

		[Deprecated (PlatformName.TvOS, 16, 0, message: "Use runs-as-current-user-with-user-independent-keychain entitlement instead.")]
		[Async]
		[Export ("presentProfilePreferencePanelWithCurrentSettings:availableProfiles:completion:")]
		void PresentProfilePreferencePanel (NSDictionary<NSString, TVAppProfileDescriptor> currentSettings, TVAppProfileDescriptor [] availableProfiles, Action<NSDictionary<NSString, TVAppProfileDescriptor>> completion);

		[Deprecated (PlatformName.TvOS, 16, 0, message: "Use runs-as-current-user-with-user-independent-keychain entitlement instead.")]
		[Async]
		[Export ("shouldStorePreferenceForCurrentUserToProfile:completion:")]
		void ShouldStorePreferenceForCurrentUser (TVAppProfileDescriptor profile, Action<bool> completion);

		[Deprecated (PlatformName.TvOS, 16, 0, message: "Use runs-as-current-user-with-user-independent-keychain entitlement instead.")]
		[Notification]
		[Field ("TVUserManagerCurrentUserIdentifierDidChangeNotification")]
		NSString CurrentUserIdentifierDidChangeNotification { get; }

		[TV (16, 0)]
		[Export ("shouldStorePreferencesForCurrentUser")]
		bool ShouldStorePreferencesForCurrentUser { get; }
	}

	[TV (13, 0)]
	[BaseType (typeof (TVTopShelfObject))]
	[DisableDefaultCtor] // null is not allowed for items
	interface TVTopShelfItemCollection {

		[Export ("items", ArgumentSemantic.Copy)]
		TVTopShelfItem [] Items { get; }

		[Export ("initWithItems:")]
		NativeHandle Constructor (TVTopShelfItem [] items);
	}

	[TV (13, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface TVTopShelfSectionedContent : TVTopShelfContent {

		[Export ("sections", ArgumentSemantic.Copy)]
		TVTopShelfItemCollection [] Sections { get; }

		[Export ("initWithSections:")]
		[DesignatedInitializer]
		NativeHandle Constructor (TVTopShelfItemCollection [] sections);

		[Static]
		[Export ("imageSizeForImageShape:")]
		CGSize GetImageSize (TVTopShelfSectionedItemImageShape shape);
	}

	[TV (13, 0)]
	[Native]
	public enum TVTopShelfSectionedItemImageShape : long {
		Square,
		Poster,
		Hdtv,
	}

	[TV (13, 0)]
	[BaseType (typeof (TVTopShelfItem))]
	[DisableDefaultCtor] // -[TVTopShelfSectionedItem init]: unrecognized selector sent to instance 0x600001f251a0
	interface TVTopShelfSectionedItem {

		// inlined from base type
		[Export ("initWithIdentifier:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string identifier);

		[Export ("imageShape", ArgumentSemantic.Assign)]
		TVTopShelfSectionedItemImageShape ImageShape { get; set; }

		[Export ("playbackProgress")]
		double PlaybackProgress { get; set; }
	}

	[TV (14, 3)]
	[Static]
	interface TVUserActivityType {

		[Field ("TVUserActivityTypeBrowsingChannelGuide")]
		NSString BrowsingChannelGuide { get; }
	}
}
