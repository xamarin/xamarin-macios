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

#if !WATCH
using XamCore.CoreMedia;
#endif
#if !TV
using XamCore.CoreLocation;
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
		AttachmentCorrupt
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
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[Unavailable (PlatformName.TvOS)]
	[Native]
	public enum UNActionIdentifier : nint {
		Default,
		Dismiss
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
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[Unavailable (PlatformName.TvOS)]
	[Category]
	[BaseType (typeof (NSString))]
	interface NSString_UNUserNotificationCenterSupport {

		[Static]
		[Export ("localizedUserNotificationStringForKey:arguments:")]
		string GetLocalizedUserNotificationString (string key, [NullAllowed] NSObject [] arguments);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.TvOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
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
	[DisableDefaultCtor]
	interface UNNotificationAction : NSCopying, NSSecureCoding {

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("title")]
		string Title { get; }

		[Export ("options", ArgumentSemantic.Assign)]
		UNNotificationActionOptions Options { get; }

		[Static]
		[Export ("actionWithIdentifier:title:options:")]
		UNNotificationAction FromIdentifier (string identifier, string title, UNNotificationActionOptions options);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[Unavailable (PlatformName.TvOS)]
	[BaseType (typeof (UNNotificationAction))]
	[DisableDefaultCtor]
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
	[DisableDefaultCtor]
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

#if XAMCORE_2_0 && !TVOS
	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[Unavailable (PlatformName.TvOS)]
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
#endif // !TVOS

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[Unavailable (PlatformName.TvOS)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UNNotificationCategory : NSCopying, NSSecureCoding {

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("actions", ArgumentSemantic.Copy)]
		UNNotificationAction [] Actions { get; }

		[Export ("minimalActions", ArgumentSemantic.Copy)]
		UNNotificationAction [] MinimalActions { get; }

		[Export ("intentIdentifiers", ArgumentSemantic.Copy)]
		string [] IntentIdentifiers { get; }

		[Export ("options", ArgumentSemantic.Assign)]
		UNNotificationCategoryOptions Options { get; }

		[Static]
		[Export ("categoryWithIdentifier:actions:minimalActions:intentIdentifiers:options:")]
		UNNotificationCategory FromIdentifier (string identifier, UNNotificationAction [] actions, UNNotificationAction [] minimalActions, string [] intentIdentifiers, UNNotificationCategoryOptions options);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.TvOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[BaseType (typeof (NSObject))]
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
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UNNotificationResponse : NSCopying, NSSecureCoding {

		[Internal]
		[Field ("UNNotificationDefaultActionIdentifier")]
		NSString DefaultActionIdentifier { get; }

		[Internal]
		[Field ("UNNotificationDismissActionIdentifier")]
		NSString DismissActionIdentifier { get; }

		[Export ("notification", ArgumentSemantic.Copy)]
		UNNotification Notification { get; }

		[Internal] // Replaced with typed ActionIdentifier
		[Export ("actionIdentifier")]
		NSString _ActionIdentifier { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[Unavailable (PlatformName.TvOS)]
	[BaseType (typeof (UNNotificationResponse))]
	interface UNTextInputNotificationResponse {

		[Export ("userText")]
		string UserText { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.TvOS)]
	[Unavailable (PlatformName.WatchOS)]
	[BaseType (typeof (NSObject))]
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
	[DisableDefaultCtor]
	interface UNNotificationSettings : NSCopying, NSSecureCoding {

		[Export ("authorizationStatus", ArgumentSemantic.Assign)]
		UNAuthorizationStatus AuthorizationStatus { get; }

		[Unavailable (PlatformName.TvOS)]
		[Export ("soundSetting", ArgumentSemantic.Assign)]
		UNNotificationSetting SoundSetting { get; }

		[Unavailable (PlatformName.WatchOS)]
		[Export ("badgeSetting", ArgumentSemantic.Assign)]
		UNNotificationSetting BadgeSetting { get; }

		[Unavailable (PlatformName.TvOS)]
		[Export ("alertSetting", ArgumentSemantic.Assign)]
		UNNotificationSetting AlertSetting { get; }

		[Unavailable (PlatformName.TvOS)]
		[Export ("notificationCenterSetting", ArgumentSemantic.Assign)]
		UNNotificationSetting NotificationCenterSetting { get; }

		[Unavailable (PlatformName.TvOS)]
		[Unavailable (PlatformName.WatchOS)]
		[Export ("lockScreenSetting", ArgumentSemantic.Assign)]
		UNNotificationSetting LockScreenSetting { get; }

		[Unavailable (PlatformName.TvOS)]
		[Unavailable (PlatformName.WatchOS)]
		[Export ("carPlaySetting", ArgumentSemantic.Assign)]
		UNNotificationSetting CarPlaySetting { get; }

		[Unavailable (PlatformName.TvOS)]
		[Unavailable (PlatformName.WatchOS)]
		[Export ("alertStyle", ArgumentSemantic.Assign)]
		UNAlertStyle AlertStyle { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[Unavailable (PlatformName.TvOS)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UNNotificationSound : NSCopying, NSSecureCoding {

		[Static]
		[Export ("defaultSound")]
		UNNotificationSound DefaultSound { get; }

		[Unavailable (PlatformName.WatchOS)]
		[Static]
		[Export ("soundNamed:")]
		UNNotificationSound GetSound (string name);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.TvOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UNNotificationTrigger : NSCopying, NSSecureCoding {

		[Export ("repeats")]
		bool Repeats { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.TvOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[BaseType (typeof (UNNotificationTrigger))]
	[DisableDefaultCtor] // Objective-C exception thrown.  Name: NSInternalInconsistencyException Reason: use subclasses
	interface UNPushNotificationTrigger {
	
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.TvOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[BaseType (typeof (UNNotificationTrigger))]
	[DisableDefaultCtor]
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
	[DisableDefaultCtor]
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

#if !TV
	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[Unavailable (PlatformName.TvOS)]
	[BaseType (typeof (UNNotificationTrigger))]
	[DisableDefaultCtor]
	interface UNLocationNotificationTrigger {

		[Export ("region", ArgumentSemantic.Copy)]
		CLRegion Region { get; }

		[Unavailable (PlatformName.WatchOS)]
		[Static]
		[Export ("triggerWithRegion:repeats:")]
		UNLocationNotificationTrigger Trigger (CLRegion region, bool repeats);
	}
#endif

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

		[Static]
		[Export ("currentNotificationCenter")]
		UNUserNotificationCenter CurrentNotificationCenter { get; }

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

