// Copyright 2015 Xamarin Inc.

using System;
using Foundation;
using ObjCRuntime;

namespace TVServices {

	[TV (9,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface TVContentIdentifier : NSCopying, NSSecureCoding {
		[Export ("identifier")]
		string Identifier { get; }

		[NullAllowed, Export ("container", ArgumentSemantic.Copy)]
		TVContentIdentifier Container { get; }

		[Export ("initWithIdentifier:container:")]
		[DesignatedInitializer]
		IntPtr Constructor (string identifier, [NullAllowed] TVContentIdentifier container);
	}

	[TV (9,0)]
	[BaseType (typeof (NSObject))]
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
		TVContentItem[] TopShelfItems { get; set; }

		[Export ("initWithContentIdentifier:")]
		[DesignatedInitializer]
		IntPtr Constructor (TVContentIdentifier ident);

		[TV (11,0)]
		[Export ("imageURLForTraits:")]
		[return: NullAllowed]
		NSUrl GetImageUrl (TVContentItemImageTrait traits);

		[TV (11,0)]
		[Export ("setImageURL:forTraits:")]
		void SetImageUrl ([NullAllowed] NSUrl aUrl, TVContentItemImageTrait traits);
	}

	[TV (9,0)]
	[Protocol]
	interface TVTopShelfProvider {
		[Abstract]
		[Export ("topShelfStyle")]
		TVTopShelfContentStyle TopShelfStyle { get; }

		[Abstract]
		[Export ("topShelfItems")]
		TVContentItem[] TopShelfItems { get; }
	}

	[TV (9,0)]
	[Static]
	interface TVTopShelfItems {
		[Notification]
		[Field ("TVTopShelfItemsDidChangeNotification")]
		NSString DidChangeNotification { get; }
	}
}