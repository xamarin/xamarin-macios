//
// UserNotifications bindings
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;
using XamCore.CoreGraphics;
using XamCore.CoreLocation;

#if !WATCH
using XamCore.CoreMedia;
#endif

namespace XamCore.UserNotifications {

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.TvOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[ErrorDomain ("UNErrorDomain")]
	[Native]
	public enum UNErrorCode : nint {
		NotificationsNotAllowed = 1,
		AttachmentInvalidUrl = 100,
		AttachmentUnrecognizedType,
		AttachmentInvalidFileSize,
		AttachmentNotInDataStore,
		AttachmentMoveIntoDataStoreFailed,
		AttachmentCorrupt,
		NotificationInvalidNoDate = 1400,
		NotificationInvalidNoContent
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[Unavailable (PlatformName.TvOS)]
	[Native]
	[Flags]
	public enum UNNotificationActionOptions : nuint {
		None = 0,
		AuthenticationRequired = (1 << 0),
		Destructive = (1 << 1),
		Foreground = (1 << 2)
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[Unavailable (PlatformName.TvOS)]
	[Native]
	[Flags]
	public enum UNNotificationCategoryOptions : nuint {
		None = 0,
		CustomDismissAction = (1 << 0),
		AllowInCarPlay = (2 << 0)
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.TvOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[Native]
	public enum UNAuthorizationStatus : nint {
		NotDetermined = 0,
		Denied,
		Authorized
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.TvOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[Native]
	public enum UNNotificationSetting : nint {
		NotSupported = 0,
		Disabled,
		Enabled
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.TvOS)]
	[Unavailable (PlatformName.WatchOS)]
	[Native]
	public enum UNAlertStyle : nint {
		None = 0,
		Banner,
		Alert
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.TvOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[Native]
	[Flags]
	public enum UNAuthorizationOptions : nuint {
		None = 0,
		Badge = (1 << 0),
		Sound = (1 << 1),
		Alert = (1 << 2),
		CarPlay = (1 << 3)
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.TvOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[Native]
	[Flags]
	public enum UNNotificationPresentationOptions : nuint {
		None = 0,
		Badge = (1 << 0),
		Sound = (1 << 1),
		Alert = (1 << 2)
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.TvOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // as per docs (not user created)
	interface UNNotification : NSCopying, NSSecureCoding {

		[Export ("date", ArgumentSemantic.Copy)]
		NSDate Date { get; }

		[Export ("request", ArgumentSemantic.Copy)]
		UNNotificationRequest Request { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[Unavailable (PlatformName.TvOS)]
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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[Unavailable (PlatformName.TvOS)]
	[BaseType (typeof (UNNotificationAction))]
	[DisableDefaultCtor] // as per docs (use FromIdentifier)
	interface UNTextInputNotificationAction {

		[Static]
		[Export ("actionWithIdentifier:title:options:textInputButtonTitle:textInputPlaceholder:")]
		UNTextInputNotificationAction FromIdentifier (string identifier, string title, UNNotificationActionOptions options, string textInputButtonTitle, string textInputPlaceholder);

		[Export ("textInputButtonTitle")]
		string TextInputButtonTitle { get; }

		[Export ("textInputPlaceholder")]
		string TextInputPlaceholder { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[Unavailable (PlatformName.TvOS)]
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

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[Unavailable (PlatformName.TvOS)]
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

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[Unavailable (PlatformName.TvOS)]
	[StrongDictionary ("UNNotificationAttachmentOptionsKeys")]
	interface UNNotificationAttachmentOptions {

		[Export ("TypeHint")]
		string TypeHint { get; set; }

		[Export ("ThumbnailHidden")]
		bool ThumbnailHidden { get; set; }

#if XAMCORE_2_0
		[Export ("ThumbnailClippingRect")]
		CGRect ThumbnailClippingRect { get; set; }
#endif
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

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[Unavailable (PlatformName.TvOS)]
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

		[Static]
		[Export ("categoryWithIdentifier:actions:intentIdentifiers:options:")]
		UNNotificationCategory FromIdentifier (string identifier, UNNotificationAction [] actions, string [] intentIdentifiers, UNNotificationCategoryOptions options);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.TvOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
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

		[Unavailable (PlatformName.TvOS)]
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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.TvOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[BaseType (typeof (UNNotificationContent))]
	interface UNMutableNotificationContent {

		[Unavailable (PlatformName.TvOS)]
		[Export ("attachments", ArgumentSemantic.Copy)]
		UNNotificationAttachment[] Attachments { get; set; }

		[NullAllowed, Export ("badge", ArgumentSemantic.Copy)]
		NSNumber Badge { get; set; }

		[Unavailable (PlatformName.TvOS)]
		[Export ("body")]
		string Body { get; set; }

		[Unavailable (PlatformName.TvOS)]
		[Export ("categoryIdentifier")]
		string CategoryIdentifier { get; set; }

		[Unavailable (PlatformName.TvOS)]
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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.TvOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
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

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[Unavailable (PlatformName.TvOS)]
	[Static]
	interface UNNotificationActionIdentifier {

		[Field ("UNNotificationDefaultActionIdentifier")]
		NSString Default { get; }

		[Field ("UNNotificationDismissActionIdentifier")]
		NSString Dismiss { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[Unavailable (PlatformName.TvOS)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // as per docs
	interface UNNotificationResponse : NSCopying, NSSecureCoding {

		[Export ("notification", ArgumentSemantic.Copy)]
		UNNotification Notification { get; }

		[Advice ("Check against UNNotificationActionIdentifier or your custom registered identifier")]
		[Export ("actionIdentifier")]
		NSString ActionIdentifier { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[Unavailable (PlatformName.TvOS)]
	[BaseType (typeof (UNNotificationResponse))]
	[DisableDefaultCtor] // as per docs
	interface UNTextInputNotificationResponse {

		[Export ("userText")]
		string UserText { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.TvOS)]
	[Unavailable (PlatformName.WatchOS)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // as per docs
	interface UNNotificationServiceExtension {

		[Export ("didReceiveNotificationRequest:withContentHandler:")]
		void DidReceiveNotificationRequest (UNNotificationRequest request, Action<UNNotificationContent> contentHandler);

		[Export ("serviceExtensionTimeWillExpire")]
		void TimeWillExpire ();
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.TvOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
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

		[Unavailable (PlatformName.TvOS)]
		[Unavailable (PlatformName.WatchOS)]
		[Export ("carPlaySetting")]
		UNNotificationSetting CarPlaySetting { get; }

		[Unavailable (PlatformName.TvOS)]
		[Unavailable (PlatformName.WatchOS)]
		[Export ("alertStyle")]
		UNAlertStyle AlertStyle { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[Unavailable (PlatformName.TvOS)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // as per docs (use provided methods)
	interface UNNotificationSound : NSCopying, NSSecureCoding {

		[Static]
		[Export ("defaultSound")]
		UNNotificationSound Default { get; }

		[Unavailable (PlatformName.WatchOS)]
		[Static]
		[Export ("soundNamed:")]
		UNNotificationSound GetSound (string name);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.TvOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[BaseType (typeof (NSObject))]
	[Abstract] // as per docs
	[DisableDefaultCtor]
	interface UNNotificationTrigger : NSCopying, NSSecureCoding {

		[Export ("repeats")]
		bool Repeats { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.TvOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[BaseType (typeof (UNNotificationTrigger))]
	[DisableDefaultCtor] // as per docs (system created)
	interface UNPushNotificationTrigger {
	
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.TvOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[BaseType (typeof (UNNotificationTrigger))]
	[DisableDefaultCtor] // as per doc, use supplied method (Trigger)
	interface UNTimeIntervalNotificationTrigger {

		[Export ("timeInterval")]
		double TimeInterval { get; }

		[Static]
		[Export ("triggerWithTimeInterval:repeats:")]
		UNTimeIntervalNotificationTrigger Trigger (double timeInterval, bool repeats);

		[NullAllowed, Export ("nextTriggerDate")]
		NSDate NextTriggerDate { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.TvOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[DisableDefaultCtor] // as per doc, use supplied method (Trigger)
	[BaseType (typeof (UNNotificationTrigger))]
	interface UNCalendarNotificationTrigger {

		[Export ("dateComponents", ArgumentSemantic.Copy)]
		NSDateComponents DateComponents { get; }

		[Static]
		[Export ("triggerWithDateMatchingComponents:repeats:")]
		UNCalendarNotificationTrigger Trigger (NSDateComponents dateComponents, bool repeats);

		[NullAllowed, Export ("nextTriggerDate")]
		NSDate NextTriggerDate { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[Unavailable (PlatformName.TvOS)]
	[BaseType (typeof (UNNotificationTrigger))]
	[DisableDefaultCtor] // as per doc, use supplied method (Trigger)
	interface UNLocationNotificationTrigger {

		[Export ("region", ArgumentSemantic.Copy)]
		CLRegion Region { get; }

		[Unavailable (PlatformName.WatchOS)]
		[Static]
		[Export ("triggerWithRegion:repeats:")]
		UNLocationNotificationTrigger Trigger (CLRegion region, bool repeats);
	}

	interface IUNUserNotificationCenterDelegate { }

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.TvOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface UNUserNotificationCenterDelegate {

		[Export ("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
		void WillPresentNotification (UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler);

		[Unavailable (PlatformName.TvOS)]
		[Export ("userNotificationCenter:didReceiveNotificationResponse:withCompletionHandler:")]
		void DidReceiveNotificationResponse (UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.TvOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
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
		void RemovePendingNotificationRequests (string[] identifiers);

		[Export ("removeAllPendingNotificationRequests")]
		void RemoveAllPendingNotificationRequests ();

		[Async]
		[Unavailable (PlatformName.TvOS)]
		[Export ("getDeliveredNotificationsWithCompletionHandler:")]
		void GetDeliveredNotifications (Action<UNNotification []> completionHandler);

		[Unavailable (PlatformName.TvOS)]
		[Export ("removeDeliveredNotificationsWithIdentifiers:")]
		void RemoveDeliveredNotificationsWithIdentifiers (string[] identifiers);

		[Unavailable (PlatformName.TvOS)]
		[Export ("removeAllDeliveredNotifications")]
		void RemoveAllDeliveredNotifications ();
	}
}

