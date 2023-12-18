//
// UserNotifications bindings
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

using System;
using Foundation;
using ObjCRuntime;
using CoreGraphics;
using CoreLocation;

#if !WATCH
using CoreMedia;
#endif

#if MONOMAC || WATCH
using UIScene = Foundation.NSObject;
#else
using UIKit;
#endif

namespace UserNotifications {

	[MacCatalyst (13, 1)]
	[ErrorDomain ("UNErrorDomain")]
	[Native]
	public enum UNErrorCode : long {
		NotificationsNotAllowed = 1,
		AttachmentInvalidUrl = 100,
		AttachmentUnrecognizedType,
		AttachmentInvalidFileSize,
		AttachmentNotInDataStore,
		AttachmentMoveIntoDataStoreFailed,
		AttachmentCorrupt,
		NotificationInvalidNoDate = 1400,
		NotificationInvalidNoContent,
		ContentProvidingObjectNotAllowed = 1500,
		ContentProvidingInvalid = 1501,
		BadgeInputInvalid = 1600,
	}

	[Unavailable (PlatformName.TvOS)]
	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum UNNotificationActionOptions : ulong {
		None = 0,
		AuthenticationRequired = (1 << 0),
		Destructive = (1 << 1),
		Foreground = (1 << 2)
	}

	[Unavailable (PlatformName.TvOS)]
	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum UNNotificationCategoryOptions : ulong {
		None = 0,
		CustomDismissAction = (1 << 0),
		[NoMac]
		[MacCatalyst (13, 1)]
		AllowInCarPlay = (2 << 0),
		HiddenPreviewsShowTitle = (1 << 2),
		HiddenPreviewsShowSubtitle = (1 << 3),
		[iOS (13, 0)]
		[Watch (6, 0)]
		[NoMac]
		[MacCatalyst (13, 1)]
		AllowAnnouncement = (1 << 4),
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum UNAuthorizationStatus : long {
		NotDetermined = 0,
		Denied,
		Authorized,
		[iOS (12, 0), TV (12, 0), Watch (5, 0)]
		[MacCatalyst (13, 1)]
		Provisional,
		[iOS (14, 0)]
		[NoMac, NoWatch, NoTV]
		[MacCatalyst (14, 0)]
		Ephemeral,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum UNNotificationSetting : long {
		NotSupported = 0,
		Disabled,
		Enabled
	}

	[Unavailable (PlatformName.TvOS)]
	[Unavailable (PlatformName.WatchOS)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UNAlertStyle : long {
		None = 0,
		Banner,
		Alert
	}

	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum UNAuthorizationOptions : ulong {
		None = 0,
		Badge = (1 << 0),
		Sound = (1 << 1),
		Alert = (1 << 2),
		CarPlay = (1 << 3),
		[iOS (12, 0), TV (12, 0), Watch (5, 0)]
		[MacCatalyst (13, 1)]
		CriticalAlert = (1 << 4),
		[iOS (12, 0), TV (12, 0), Watch (5, 0)]
		[MacCatalyst (13, 1)]
		ProvidesAppNotificationSettings = (1 << 5),
		[iOS (12, 0), TV (12, 0), Watch (5, 0)]
		[MacCatalyst (13, 1)]
		Provisional = (1 << 6),
		[iOS (13, 0)]
		[TV (13, 0)]
		[Watch (6, 0)]
		[Deprecated (PlatformName.iOS, 15, 0, message: "Announcement is always included.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Announcement is always included.")]
		[Deprecated (PlatformName.WatchOS, 7, 0, message: "Announcement is always included.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Announcement is always included.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Announcement is always included.")]
		Announcement = (1 << 7),
		[iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0), TV (15, 0), Watch (8, 0)]
		TimeSensitive = (1 << 8),
	}

	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum UNNotificationPresentationOptions : ulong {
		None = 0,
		Badge = (1 << 0),
		Sound = (1 << 1),
		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'List | Banner' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'List | Banner' instead.")]
		[Deprecated (PlatformName.WatchOS, 7, 0, message: "Use 'List | Banner' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'List | Banner' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'List | Banner' instead.")]
		Alert = (1 << 2),
		[iOS (14, 0)]
		[TV (14, 0)]
		[Watch (7, 0)]
		[Mac (11, 0)]
		[MacCatalyst (14, 0)]
		List = (1 << 3),
		[iOS (14, 0)]
		[TV (14, 0)]
		[Watch (7, 0)]
		[Mac (11, 0)]
		[MacCatalyst (14, 0)]
		Banner = (1 << 4),
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UNShowPreviewsSetting : long {
		Always,
		WhenAuthenticated,
		Never
	}

	[iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0), TV (15, 0), Watch (8, 0)]
	[Native]
	public enum UNNotificationInterruptionLevel : long {
#if XAMCORE_5_0
		Passive,
		Active,
		TimeSensitive,
		Critical,
#else
		[Obsolete ("Use 'Active2'.")]
		Active,
		[Obsolete ("Use 'Critical2'.")]
		Critical,
		[Obsolete ("Use 'Passive2'.")]
		Passive,
		[Obsolete ("Use 'TimeSensitive2'.")]
		TimeSensitive,
#endif // XAMCORE_5_0

		// Additional enum values to fix reordering - to be at the end of the enum
#if !XAMCORE_5_0
#pragma warning disable 0618 // warning CS0618: 'UNNotificationInterruptionLevel.[field]' is obsolete: 'Use '[replacement]'.'
		Active2 = Critical,
		Critical2 = TimeSensitive,
		Passive2 = Active,
		TimeSensitive2 = Passive,
#pragma warning restore
#endif // !XAMCORE_5_0
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // as per docs (not user created)
	interface UNNotification : NSCopying, NSSecureCoding {

		[Export ("date", ArgumentSemantic.Copy)]
		NSDate Date { get; }

		[Export ("request", ArgumentSemantic.Copy)]
		UNNotificationRequest Request { get; }
	}

	[Unavailable (PlatformName.TvOS)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // as per docs (use FromIdentifier)
	interface UNNotificationAction : NSCopying, NSSecureCoding {

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("title")]
		string Title { get; }

		[Export ("options")]
		UNNotificationActionOptions Options { get; }

		[Static]
		[Export ("actionWithIdentifier:title:options:")]
		UNNotificationAction FromIdentifier (string identifier, string title, UNNotificationActionOptions options);

		[Watch (8, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Static]
		[Export ("actionWithIdentifier:title:options:icon:")]
		UNNotificationAction FromIdentifier (string identifier, string title, UNNotificationActionOptions options, [NullAllowed] UNNotificationActionIcon icon);

		[Watch (8, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("icon", ArgumentSemantic.Copy)]
		UNNotificationActionIcon Icon { get; }
	}

	[Unavailable (PlatformName.TvOS)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UNNotificationAction))]
	[DisableDefaultCtor] // as per docs (use FromIdentifier)
	interface UNTextInputNotificationAction {

		[Static]
		[Export ("actionWithIdentifier:title:options:textInputButtonTitle:textInputPlaceholder:")]
		UNTextInputNotificationAction FromIdentifier (string identifier, string title, UNNotificationActionOptions options, string textInputButtonTitle, string textInputPlaceholder);

		[iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0), Watch (8, 0)]
		[Static]
		[Export ("actionWithIdentifier:title:options:icon:textInputButtonTitle:textInputPlaceholder:")]
		UNTextInputNotificationAction FromIdentifier (string identifier, string title, UNNotificationActionOptions options, [NullAllowed] UNNotificationActionIcon icon, string textInputButtonTitle, string textInputPlaceholder);

		[Export ("textInputButtonTitle")]
		string TextInputButtonTitle { get; }

		[Export ("textInputPlaceholder")]
		string TextInputPlaceholder { get; }
	}

	[Unavailable (PlatformName.TvOS)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // as per docs (use FromIdentifier)
	interface UNNotificationAttachment : NSCopying, NSSecureCoding {

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("URL", ArgumentSemantic.Copy)]
		NSUrl Url { get; }

		[Export ("type")]
		string Type { get; }

		[Static]
		[Export ("attachmentWithIdentifier:URL:options:error:")]
		[return: NullAllowed]
		UNNotificationAttachment FromIdentifier (string identifier, NSUrl url, [NullAllowed] NSDictionary options, [NullAllowed] out NSError error);
	}

	[Unavailable (PlatformName.TvOS)]
	[MacCatalyst (13, 1)]
	[Static]
	[Internal]
	interface UNNotificationAttachmentOptionsKeys {

		[Field ("UNNotificationAttachmentOptionsTypeHintKey")]
		NSString TypeHint { get; }

		[Field ("UNNotificationAttachmentOptionsThumbnailHiddenKey")]
		NSString ThumbnailHidden { get; }

		[Field ("UNNotificationAttachmentOptionsThumbnailClippingRectKey")]
		NSString ThumbnailClippingRect { get; }

		[Field ("UNNotificationAttachmentOptionsThumbnailTimeKey")]
		NSString ThumbnailTime { get; }
	}

	[Unavailable (PlatformName.TvOS)]
	[MacCatalyst (13, 1)]
	[StrongDictionary ("UNNotificationAttachmentOptionsKeys")]
	interface UNNotificationAttachmentOptions {

		[Export ("TypeHint")]
		string TypeHint { get; set; }

		[Export ("ThumbnailHidden")]
		bool ThumbnailHidden { get; set; }

		[Export ("ThumbnailClippingRect")]
		CGRect ThumbnailClippingRect { get; set; }
		// According to apple docs UNNotificationAttachmentOptionsThumbnailTimeKey
		// can be either a CMTime or a NSNumber (in seconds). Exposing both options
		// in the strong dictionary because watchOS does not have CMTime or
		// CoreMedia framework at all.
#if !WATCH
		[Export ("ThumbnailTime")]
		CMTime ThumbnailTime { get; set; }
#endif // !WATCH

		[Export ("ThumbnailTime")]
		double ThumbnailTimeInSeconds { get; set; }
	}

	[Unavailable (PlatformName.TvOS)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // as per docs (use FromIdentifier)
	interface UNNotificationCategory : NSCopying, NSSecureCoding {

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("actions", ArgumentSemantic.Copy)]
		UNNotificationAction [] Actions { get; }

		[Export ("intentIdentifiers", ArgumentSemantic.Copy)]
		string [] IntentIdentifiers { get; }

		[Export ("options")]
		UNNotificationCategoryOptions Options { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("hiddenPreviewsBodyPlaceholder")]
		string HiddenPreviewsBodyPlaceholder { get; }

		[Static]
		[Export ("categoryWithIdentifier:actions:intentIdentifiers:options:")]
		UNNotificationCategory FromIdentifier (string identifier, UNNotificationAction [] actions, string [] intentIdentifiers, UNNotificationCategoryOptions options);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("categoryWithIdentifier:actions:intentIdentifiers:hiddenPreviewsBodyPlaceholder:options:")]
		UNNotificationCategory FromIdentifier (string identifier, UNNotificationAction [] actions, string [] intentIdentifiers, string hiddenPreviewsBodyPlaceholder, UNNotificationCategoryOptions options);

		[NoWatch, iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("categoryWithIdentifier:actions:intentIdentifiers:hiddenPreviewsBodyPlaceholder:categorySummaryFormat:options:")]
		UNNotificationCategory FromIdentifier (string identifier, UNNotificationAction [] actions, string [] intentIdentifiers, [NullAllowed] string hiddenPreviewsBodyPlaceholder, [NullAllowed] NSString categorySummaryFormat, UNNotificationCategoryOptions options);

		[NoWatch, iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("categorySummaryFormat")]
		string CategorySummaryFormat { get; }

	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // as per docs
	interface UNNotificationContent : NSCopying, NSMutableCopying, NSSecureCoding {

		[Unavailable (PlatformName.TvOS)]
		[Export ("attachments", ArgumentSemantic.Copy)]
		UNNotificationAttachment [] Attachments { get; }

		[NullAllowed, Export ("badge", ArgumentSemantic.Copy)]
		NSNumber Badge { get; }

		[Unavailable (PlatformName.TvOS)]
		[Export ("body")]
		string Body { get; }

		[Unavailable (PlatformName.TvOS)]
		[Export ("categoryIdentifier")]
		string CategoryIdentifier { get; }

		[NoTV, NoMac]
		[MacCatalyst (13, 1)]
		[Export ("launchImageName")]
		string LaunchImageName { get; }

		[Unavailable (PlatformName.TvOS)]
		[NullAllowed, Export ("sound", ArgumentSemantic.Copy)]
		UNNotificationSound Sound { get; }

		[Unavailable (PlatformName.TvOS)]
		[Export ("subtitle")]
		string Subtitle { get; }

		[Unavailable (PlatformName.TvOS)]
		[Export ("threadIdentifier")]
		string ThreadIdentifier { get; }

		[Unavailable (PlatformName.TvOS)]
		[Export ("title")]
		string Title { get; }

		[Unavailable (PlatformName.TvOS)]
		[Export ("userInfo", ArgumentSemantic.Copy)]
		NSDictionary UserInfo { get; }

		[NoWatch, NoTV, iOS (12, 0)]
		[Deprecated (PlatformName.iOS, 15, 0, message: "This property is ignored.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "This property is ignored.")]
		[Export ("summaryArgument")]
		string SummaryArgument { get; }

		[NoWatch, NoTV, iOS (12, 0)]
		[Deprecated (PlatformName.iOS, 15, 0, message: "This property is ignored.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "This property is ignored.")]
		[Export ("summaryArgumentCount")]
		nuint SummaryArgumentCount { get; }

		[iOS (13, 0)]
		[TV (13, 0)]
		[Watch (6, 0)] // no direct mention in headers
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("targetContentIdentifier")]
		string TargetContentIdentifier { get; [NotImplemented] set; }

		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("contentByUpdatingWithProvider:error:")]
		[return: NullAllowed]
		UNNotificationContent Update (IUNNotificationContentProviding fromProvider, [NullAllowed] out NSError outError);

		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("interruptionLevel", ArgumentSemantic.Assign)]
		UNNotificationInterruptionLevel InterruptionLevel { get; }

		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("relevanceScore")]
		double RelevanceScore { get; }

		[Watch (9, 0), NoTV, Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("filterCriteria")]
		[NullAllowed]
		string FilterCriteria { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (UNNotificationContent))]
	interface UNMutableNotificationContent {

		[Unavailable (PlatformName.TvOS)]
		[Export ("attachments", ArgumentSemantic.Copy)]
		UNNotificationAttachment [] Attachments { get; set; }

		[NullAllowed, Export ("badge", ArgumentSemantic.Copy)]
		NSNumber Badge { get; set; }

		[Unavailable (PlatformName.TvOS)]
		[Export ("body")]
		string Body { get; set; }

		[Unavailable (PlatformName.TvOS)]
		[Export ("categoryIdentifier")]
		string CategoryIdentifier { get; set; }

		[NoTV, NoMac]
		[MacCatalyst (13, 1)]
		[Export ("launchImageName")]
		string LaunchImageName { get; set; }

		[Unavailable (PlatformName.TvOS)]
		[NullAllowed, Export ("sound", ArgumentSemantic.Copy)]
		UNNotificationSound Sound { get; set; }

		[Unavailable (PlatformName.TvOS)]
		[Export ("subtitle")]
		string Subtitle { get; set; }

		[Unavailable (PlatformName.TvOS)]
		[Export ("threadIdentifier")]
		string ThreadIdentifier { get; set; }

		[Unavailable (PlatformName.TvOS)]
		[Export ("title")]
		string Title { get; set; }

		[Export ("userInfo", ArgumentSemantic.Copy)]
		NSDictionary UserInfo { get; set; }

		[NoWatch, NoTV, iOS (12, 0)]
		[Deprecated (PlatformName.iOS, 15, 0, message: "This property is ignored.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "This property is ignored.")]
		[Export ("summaryArgument")]
		string SummaryArgument { get; set; }

		[NoWatch, NoTV, iOS (12, 0)]
		[Deprecated (PlatformName.iOS, 15, 0, message: "This property is ignored.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "This property is ignored.")]
		[Export ("summaryArgumentCount")]
		nuint SummaryArgumentCount { get; set; }

		[iOS (13, 0)]
		[TV (13, 0)]
		[Watch (6, 0)] // no direct mention in headers
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("targetContentIdentifier")]
		string TargetContentIdentifier { get; set; }

		[iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0), TV (15, 0), Watch (8, 0)]
		[Export ("interruptionLevel", ArgumentSemantic.Assign)]
		UNNotificationInterruptionLevel InterruptionLevel { get; set; }

		[iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0), TV (15, 0), Watch (8, 0)]
		[Export ("relevanceScore")]
		double RelevanceScore { get; set; }

		[TV (16, 0), Watch (9, 0), Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[NullAllowed, Export ("filterCriteria")]
		string FilterCriteria { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UNNotificationRequest : NSCopying, NSSecureCoding {

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("content", ArgumentSemantic.Copy)]
		UNNotificationContent Content { get; }

		[NullAllowed, Export ("trigger", ArgumentSemantic.Copy)]
		UNNotificationTrigger Trigger { get; }

		[Static]
		[Export ("requestWithIdentifier:content:trigger:")]
		UNNotificationRequest FromIdentifier (string identifier, UNNotificationContent content, [NullAllowed] UNNotificationTrigger trigger);
	}

	[Unavailable (PlatformName.TvOS)]
	[MacCatalyst (13, 1)]
	[Static]
	[Internal]
	interface UNNotificationActionIdentifier {

		[Field ("UNNotificationDefaultActionIdentifier")]
		NSString Default { get; }

		[Field ("UNNotificationDismissActionIdentifier")]
		NSString Dismiss { get; }
	}

	[Unavailable (PlatformName.TvOS)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // as per docs
	interface UNNotificationResponse : NSCopying, NSSecureCoding {

		[Export ("notification", ArgumentSemantic.Copy)]
		UNNotification Notification { get; }

		[Export ("actionIdentifier")]
		NSString ActionIdentifier { get; }

		[Wrap ("ActionIdentifier == UNNotificationActionIdentifier.Default")]
		bool IsDefaultAction { get; }

		[Wrap ("ActionIdentifier == UNNotificationActionIdentifier.Dismiss")]
		bool IsDismissAction { get; }

		[Wrap ("!IsDefaultAction && !IsDismissAction")]
		bool IsCustomAction { get; }

		[iOS (13, 0), NoWatch, NoMac]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("targetScene")]
		UIScene TargetScene { get; }
	}

	[Unavailable (PlatformName.TvOS)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UNNotificationResponse))]
	[DisableDefaultCtor] // as per docs
	interface UNTextInputNotificationResponse {

		[Export ("userText")]
		string UserText { get; }
	}

	[Watch (6, 0)]
	[Unavailable (PlatformName.TvOS)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // as per docs
	interface UNNotificationServiceExtension {

		// Not async because app developers are supposed to implement/override this method, not call it themselves.
		[Export ("didReceiveNotificationRequest:withContentHandler:")]
		void DidReceiveNotificationRequest (UNNotificationRequest request, Action<UNNotificationContent> contentHandler);

		[Export ("serviceExtensionTimeWillExpire")]
		void TimeWillExpire ();
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // as per docs
	interface UNNotificationSettings : NSCopying, NSSecureCoding {

		[Export ("authorizationStatus")]
		UNAuthorizationStatus AuthorizationStatus { get; }

		[Unavailable (PlatformName.TvOS)]
		[Export ("soundSetting")]
		UNNotificationSetting SoundSetting { get; }

		[Unavailable (PlatformName.WatchOS)]
		[Export ("badgeSetting")]
		UNNotificationSetting BadgeSetting { get; }

		[Unavailable (PlatformName.TvOS)]
		[Export ("alertSetting")]
		UNNotificationSetting AlertSetting { get; }

		[Unavailable (PlatformName.TvOS)]
		[Export ("notificationCenterSetting")]
		UNNotificationSetting NotificationCenterSetting { get; }

		[Unavailable (PlatformName.TvOS)]
		[Unavailable (PlatformName.WatchOS)]
		[Export ("lockScreenSetting")]
		UNNotificationSetting LockScreenSetting { get; }

		[NoWatch, NoTV, NoMac]
		[MacCatalyst (13, 1)]
		[Export ("carPlaySetting")]
		UNNotificationSetting CarPlaySetting { get; }

		[Unavailable (PlatformName.TvOS)]
		[Unavailable (PlatformName.WatchOS)]
		[Export ("alertStyle")]
		UNAlertStyle AlertStyle { get; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Export ("showPreviewsSetting")]
		UNShowPreviewsSetting ShowPreviewsSetting { get; }

		[Watch (5, 0), NoTV, iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("criticalAlertSetting")]
		UNNotificationSetting CriticalAlertSetting { get; }

		[Watch (5, 0), NoTV, iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("providesAppNotificationSettings")]
		bool ProvidesAppNotificationSettings { get; }

		[Watch (6, 0), NoTV, NoMac, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("announcementSetting")]
		UNNotificationSetting AnnouncementSetting { get; }

		[iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0), Watch (8, 0), TV (15, 0)]
		[Export ("timeSensitiveSetting")]
		UNNotificationSetting TimeSensitiveSetting { get; }

		[iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0), Watch (8, 0), TV (15, 0)]
		[Export ("scheduledDeliverySetting")]
		UNNotificationSetting ScheduledDeliverySetting { get; }

		[iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0), Watch (8, 0), TV (15, 0)]
		[Export ("directMessagesSetting")]
		UNNotificationSetting DirectMessagesSetting { get; }
	}

	[Unavailable (PlatformName.TvOS)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // as per docs (use provided methods)
	interface UNNotificationSound : NSCopying, NSSecureCoding {

		[Static]
		[Export ("defaultSound")]
		UNNotificationSound Default { get; }

		[NoWatch, NoTV, NoMacCatalyst, NoMac, iOS (15, 2)]
		[Static]
		[Export ("defaultRingtoneSound", ArgumentSemantic.Copy)]
		UNNotificationSound DefaultRingtoneSound { get; }

		[Unavailable (PlatformName.WatchOS)]
		[Static]
		[Export ("soundNamed:")]
		UNNotificationSound GetSound (string name);

		[Watch (5, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("defaultCriticalSound", ArgumentSemantic.Copy)]
		UNNotificationSound DefaultCriticalSound { get; }

		[Watch (5, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("defaultCriticalSoundWithAudioVolume:")]
		UNNotificationSound GetDefaultCriticalSound (float volume);

		[NoWatch, NoTV, NoMacCatalyst, NoMac, iOS (15, 2)]
		[Static]
		[Export ("ringtoneSoundNamed:")]
		UNNotificationSound GetRingtoneSound (string name);

		[NoWatch, iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("criticalSoundNamed:")]
		UNNotificationSound GetCriticalSound (string name);

		[NoWatch, iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("criticalSoundNamed:withAudioVolume:")]
		UNNotificationSound GetCriticalSound (string name, float volume);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Abstract] // as per docs
	[DisableDefaultCtor]
	interface UNNotificationTrigger : NSCopying, NSSecureCoding {

		[Export ("repeats")]
		bool Repeats { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (UNNotificationTrigger))]
	[DisableDefaultCtor] // as per docs (system created)
	interface UNPushNotificationTrigger {

	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (UNNotificationTrigger))]
	[DisableDefaultCtor] // as per doc, use supplied method (CreateTrigger)
	interface UNTimeIntervalNotificationTrigger {

		[Export ("timeInterval")]
		double TimeInterval { get; }

		[Static]
		[Export ("triggerWithTimeInterval:repeats:")]
		UNTimeIntervalNotificationTrigger CreateTrigger (double timeInterval, bool repeats);

		[NullAllowed, Export ("nextTriggerDate")]
		NSDate NextTriggerDate { get; }
	}

	[MacCatalyst (13, 1)]
	[DisableDefaultCtor] // as per doc, use supplied method (CreateTrigger)
	[BaseType (typeof (UNNotificationTrigger))]
	interface UNCalendarNotificationTrigger {

		[Export ("dateComponents", ArgumentSemantic.Copy)]
		NSDateComponents DateComponents { get; }

		[Static]
		[Export ("triggerWithDateMatchingComponents:repeats:")]
		UNCalendarNotificationTrigger CreateTrigger (NSDateComponents dateComponents, bool repeats);

		[NullAllowed, Export ("nextTriggerDate")]
		NSDate NextTriggerDate { get; }
	}

	[NoMac]
	[NoMacCatalyst]
	[Unavailable (PlatformName.TvOS)]
	[BaseType (typeof (UNNotificationTrigger))]
	[DisableDefaultCtor] // as per doc, use supplied method (CreateTrigger)
	interface UNLocationNotificationTrigger {

		[Export ("region", ArgumentSemantic.Copy)]
		CLRegion Region { get; }

		[Watch (8, 0)]
		[Static]
		[Export ("triggerWithRegion:repeats:")]
		UNLocationNotificationTrigger CreateTrigger (CLRegion region, bool repeats);
	}

	interface IUNUserNotificationCenterDelegate { }

	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface UNUserNotificationCenterDelegate {

		[Export ("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
		void WillPresentNotification (UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler);

		[Unavailable (PlatformName.TvOS)]
		[Export ("userNotificationCenter:didReceiveNotificationResponse:withCompletionHandler:")]
		void DidReceiveNotificationResponse (UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler);

		[NoWatch, NoTV, iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("userNotificationCenter:openSettingsForNotification:")]
		void OpenSettings (UNUserNotificationCenter center, [NullAllowed] UNNotification notification);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UNUserNotificationCenter {

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		IUNUserNotificationCenterDelegate Delegate { get; set; }

		[Export ("supportsContentExtensions")]
		bool SupportsContentExtensions { get; }

		[Static]
		[Export ("currentNotificationCenter")]
		UNUserNotificationCenter Current { get; }

		[Async]
		[Export ("requestAuthorizationWithOptions:completionHandler:")]
		void RequestAuthorization (UNAuthorizationOptions options, Action<bool, NSError> completionHandler);

		[Unavailable (PlatformName.TvOS)]
		[Export ("setNotificationCategories:")]
		void SetNotificationCategories (NSSet<UNNotificationCategory> categories);

		[Async]
		[Unavailable (PlatformName.TvOS)]
		[Export ("getNotificationCategoriesWithCompletionHandler:")]
		void GetNotificationCategories (Action<NSSet<UNNotificationCategory>> completionHandler);

		[Async]
		[Export ("getNotificationSettingsWithCompletionHandler:")]
		void GetNotificationSettings (Action<UNNotificationSettings> completionHandler);

		[Async]
		[Export ("addNotificationRequest:withCompletionHandler:")]
		void AddNotificationRequest (UNNotificationRequest request, [NullAllowed] Action<NSError> completionHandler);

		[Async]
		[Export ("getPendingNotificationRequestsWithCompletionHandler:")]
		void GetPendingNotificationRequests (Action<UNNotificationRequest []> completionHandler);

		[Export ("removePendingNotificationRequestsWithIdentifiers:")]
		void RemovePendingNotificationRequests (string [] identifiers);

		[Export ("removeAllPendingNotificationRequests")]
		void RemoveAllPendingNotificationRequests ();

		[Async]
		[Unavailable (PlatformName.TvOS)]
		[Export ("getDeliveredNotificationsWithCompletionHandler:")]
		void GetDeliveredNotifications (Action<UNNotification []> completionHandler);

		[Unavailable (PlatformName.TvOS)]
		[Export ("removeDeliveredNotificationsWithIdentifiers:")]
		void RemoveDeliveredNotifications (string [] identifiers);

		[Unavailable (PlatformName.TvOS)]
		[Export ("removeAllDeliveredNotifications")]
		void RemoveAllDeliveredNotifications ();

		[Async]
		[TV (16, 0), NoWatch, Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("setBadgeCount:withCompletionHandler:")]
		void SetBadgeCount (nint newBadgeCount, [NullAllowed] Action<NSError> completionHandler);
	}

	[iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0), TV (15, 0), Watch (8, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UNNotificationActionIcon : NSCopying, NSSecureCoding {
		[Static]
		[Export ("iconWithTemplateImageName:")]
		UNNotificationActionIcon CreateFromTemplate (string imageName);

		[Static]
		[Export ("iconWithSystemImageName:")]
		UNNotificationActionIcon CreateFromSystem (string imageName);
	}

	interface IUNNotificationContentProviding { }

	[iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0), TV (15, 0), Watch (8, 0)]
	[Protocol]
	interface UNNotificationContentProviding {
	}
}

